using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using SmartBusServiceLib;
//using LGJSocketClientAPILib;


namespace LGJAsynchSocketService
{
    public class AsynchLockServerSocketService : AsynchSocketServiceBaseFrame
    {
        public ManagerLoginLockUser MyManagerLoginLockUser;
        //public LockServerSocketClient MyLockServerSocketClient;//暂时没有用
              
        //public SqlConnection MySqlConnection;
        public int SaveSnapFlag;
        //public bool IsSendEMail;

        public AsynchLockServerSocketService(): base()
        {        
            
           MyManagerLoginLockUser = new ManagerLoginLockUser();
           MyManagerSocketLoginUser = MyManagerLoginLockUser;
           MyManagerLoginLockUser.InitSmartAutoDetect();

           //MyLockServerSocketClient = new LGJSocketClientAPILib.LockServerSocketClient(); 
           //MyLGJSocketClientAPIBase = MyLockServerSocketClient;

           SaveSnapFlag = 0;//保存到本系统还是阿里云OSS
           //IsSendEMail = true;
           InitServiceParas();
       

        }
       
        protected  void InitServiceParas()
        {
            //1.打开配置文件,获取相应的appSettings配置节
            //string SmartBusIPSet;
            //string SmartBusPortSet;
            string LocalListenPort;
            string EvetLogFlagStr;
            //SmartBusIPSet = (string)ConfigurationManager.AppSettings["SmartBusIPSet"];
            //SmartBusPortSet = (string)ConfigurationManager.AppSettings["SmartBusPortSet"];
            LocalListenPort = (string)ConfigurationManager.AppSettings["LocalListenPort"];
            EvetLogFlagStr = (string)ConfigurationManager.AppSettings["EvetLogFlag"];

            EvetLogFlag = string.IsNullOrEmpty(EvetLogFlagStr)|| EvetLogFlagStr=="0" ? true : false;
            MyTCPListerPort = int.Parse(LocalListenPort);// 8900;
            MySocketServiceTypeID = SocketServiceTypeID.lockServer;        

            MyManagerLoginLockUser.MyAsynchSendMessageCallback = new ManagerLoginLockUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);
            //--------------------------------------------
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            try
            {
                MySqlConnection.Open();
            }
            catch(Exception ex)
            {
                DisplayResultInfor(1, ex.Message);
            }
     
           

        }
              
     
        protected override void ProcessMessageEntry(LoginUser MyLoginUser, uint MyRecieveCount, int MyRecieveAvailable)
        {

            //TestSnapSQLServerEntry(MyLoginUser, MyRecieveCount, MyRecieveAvailable);
            //return;

            //DisplayResultInfor(2, string.Format("Test1：[{0}]", MyRecieveCount));
            //====================================================================================
            byte MessageTypeFirstFlag = MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[2];

            if (MyLoginUser.LoginID > 2)
            {
                //--1.锁端-----------------------------------------------------------
                string HexSendMessageIDString;
                if (DataFormateFlag)
                {
                    //小端
                    HexSendMessageIDString = string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[9]) + string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[8]);
                }
                else
                {
                    //大端
                    HexSendMessageIDString = string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[8]) + string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[9]);
                }
                //--2.---------------------------------------------------------------

                if (MyLoginUser.ChannelStatus == 1)
                {
                    //DisplayResultInfor(2, string.Format("Test2：[{0}]", HexSendMessageIDString));
                    //无认证
                    if (HexSendMessageIDString == "1001")
                        CommandDefineDispatchEx(MyLoginUser, (int)MyRecieveCount);//只有【Login-命令】能传输再处理


                }
                else
                {
                    //已认证
                    if (MyLoginUser.WorkStatus > 0)
                    {

                        //[第二次文件传输]传输再处理
                        LockServerLib.ImageFileReceiveProcess MyImageFileReceiveProcess;
                        MyImageFileReceiveProcess = new LockServerLib.ImageFileReceiveProcess(this, MyLoginUser, (int)MyRecieveCount);
                        MyImageFileReceiveProcess.CompleteCommand();

                    }
                    else
                    {
                        CommandDefineDispatchEx(MyLoginUser, (int)MyRecieveCount); ;//所有命令[和第一次文件传输:收件、异常、监控]能传输再处理
                    }

                }
            }
            else
            {
                
                //--移动端进入通道------------------------------
                CommandDefineDispatchEx(MyLoginUser, (int)MyRecieveCount); ;//所有来自移动端的命令



            }

        }


        protected  void TestSnapSQLServerEntry(LoginUser MyLoginUser, uint MyRecieveCount, int MyRecieveAvailable)
        {
            //----把来自客户端缓冲区按原样全部转换为16进制字符串表示--------
            string BaseMessageString = null;
            byte[] MyReadBuffers = MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers;
            int InputRecieveCount=(int)MyRecieveCount;
            for (int i = 0; i < InputRecieveCount; i++)
            {

                BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[i]);

            }

            //-----回显消息----------------------------------------------------------------------------------------------------------
            SocketServiceReadWriteChannel InSocketServiceReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
            string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", InSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));

                   

        }
        public override void CommandDefineDispatch(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] MyReadBuffers, int RecieveMessageFlag, int InputRecieveCount)
        {
            string BaseMessageString = null;
            string HexSendMessageIDString = null;
            //====1.消息预处理=====================================================================
            if (RecieveMessageFlag < 250)
            {              
                //----把来自锁端缓冲区按原样全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------
                if (InputRecieveCount <=1024)//-----命令字状态
                {
                    //----锁端普通消息接收-确认回复----------------------------------------
                    string RecieveMsgIndexStr = string.Format("{0:X2}", MyReadBuffers[12]);//取低位！
                    //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", MyReadBuffers[12]);
                    int PackgeIndex = Convert.ToInt16(RecieveMsgIndexStr, 16);

                    CloudServerAckToLockEx(InputSocketServiceReadWriteChannel, PackgeIndex);
                    //----------------------------------------------------------------------
                    
                    for (int i = 0; i < InputRecieveCount; i++)
                    {

                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[i]);

                    }
                }
                else//----------------------------图像流传输状态-----------------------------
                {
                    //--图像流无须应答----

                    if (InputRecieveCount <= 65536)
                    {
                           //BaseMessageString = string.Format("{0:X2}", MyReadBuffers[1]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[0]);
                        //UInt16 RecieveMessageLenght = Convert.ToUInt16(BaseMessageString, 16);
                        //BaseMessageString = string.Format("标准文件传输[{0}]", RecieveMessageLenght);
                         BaseMessageString = string.Format("标准文件传输[{0}]", InputRecieveCount);

                    }
                    else
                    {
                          
                        //BaseMessageString = string.Format("{0:X2}", MyReadBuffers[21]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[20]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[19]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[18]);
                        //UInt32 RecieveMessageLenght = Convert.ToUInt32(BaseMessageString, 16);
                        //BaseMessageString = string.Format("扩展文件传输[{0}]", RecieveMessageLenght);
                        BaseMessageString = string.Format("扩展文件传输[{0}]", InputRecieveCount);
                    }
                }

                //--------------------------------------------------------------------------------------------------------------

                if (DataFormateFlag)
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
                }
                else
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
                }


            }

            else//来自移动端消息
            {
                    
                //把来自移动端的消息字符串显示-----------------------------------------------------------------------------------------------------------------------
                     /*
              if (RecieveMessageFlag == 254)
              {
                  BaseMessageString = Encoding.Unicode.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
              }
              else
              {
                  BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
              }
              */
                    BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);

            }
            //-----回显消息-----------
            string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", InputSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));
          
            //===2.消息进一步分析应用=================================================================================
            switch (RecieveMessageFlag)
            {
                case 0:
                    //智能锁响应消息
                    SamrtLockResponseMessage(InputSocketServiceReadWriteChannel, InputRecieveCount, HexSendMessageIDString);
                    break;
                case 1:
                    //智能锁主动上传消息
                    SamrtLockRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount, HexSendMessageIDString);
                    break;

                case 255:
                    //移动端请求消息[]ASCII]
                    SamrtMobileRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount);
                    break;

                case 254:
                    //移动端请求消息[UNICODE]
                    SamrtMobileRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount);
                    
                    break;
                default:
                    //如：来自图像流标志2，则不处理
                    break;

            }


        }

        public  void CommandDefineDispatchEx(LoginUser MeLoginUser, int InputRecieveCount)
        {
            SocketServiceReadWriteChannel InSocketServiceReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            byte[] MyReadBuffers = MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers;
            int RecieveMessageFlag=MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers[2];;

            string BaseMessageString = null;
            string HexSendMessageIDString = null;

            //DisplayResultInfor(2, string.Format("[{0}]", InputRecieveCount));
            //DisplayResultInfor(2, string.Format("Test3：[{0}]", InputRecieveCount));
            //====1.消息预处理=====================================================================            
            if (MeLoginUser.LoginID>1)//2
            {
                //----把来自锁端缓冲区按原样全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------
                if (InputRecieveCount <= 1024)//-----命令字状态
                {
                    //----锁端普通消息接收-确认回复----------------------------------------
                    string RecieveMsgIndexStr = string.Format("{0:X2}", MyReadBuffers[12]);//取低位！
                    //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", MyReadBuffers[12]);
                    int PackgeIndex = Convert.ToInt16(RecieveMsgIndexStr, 16);

                    CloudServerAckToLockEx(InSocketServiceReadWriteChannel, PackgeIndex);
                    //----------------------------------------------------------------------

                    for (int i = 0; i < InputRecieveCount; i++)
                    {

                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[i]);

                    }

                  
                    //-----回显消息----------------------------------------------------------------------------------------------------------
                    //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                    string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);
                    DisplayResultInfor(2, string.Format(MyTimeMarkerStr + " ARx[{0}][{1}]", InSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));

                }
              
                /*
                else//----------------------------图像流传输状态-----------------------------
                {
                    //--图像流无须应答----

                    if (InputRecieveCount <= 65536)
                    {
                           //BaseMessageString = string.Format("{0:X2}", MyReadBuffers[1]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[0]);
                        //UInt16 RecieveMessageLenght = Convert.ToUInt16(BaseMessageString, 16);
                        //BaseMessageString = string.Format("标准文件传输[{0}]", RecieveMessageLenght);
                        BaseMessageString = string.Format("标准文件传输[{0}]", InputRecieveCount);

                    }
                    else
                    {

                        //BaseMessageString = string.Format("{0:X2}", MyReadBuffers[21]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[20]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[19]);
                        //BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[18]);
                        //UInt32 RecieveMessageLenght = Convert.ToUInt32(BaseMessageString, 16);
                        //BaseMessageString = string.Format("扩展文件传输[{0}]", RecieveMessageLenght);
                        BaseMessageString = string.Format("扩展文件传输[{0}]", InputRecieveCount);
                    }
                }
                */

                if (DataFormateFlag)
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
                }
                else
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
                }
              


            }

            else//来自移动端消息
            {

                  //把来自移动端的消息字符串显示-----------------------------------------------------------------------------------------------------------------------
                /*
              if (RecieveMessageFlag == 254)
              {
             BaseMessageString = Encoding.Unicode.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
              }
            else
             {
             BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
             }
            */
                BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
                //-----回显消息-----------
                //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);
                DisplayResultInfor(2, string.Format(MyTimeMarkerStr + " MRx[{0}]{1}", InSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));


            }
            
            //===2.消息进一步分析应用=================================================================================
            switch (RecieveMessageFlag)
            {
                case 0:
                    //智能锁响应消息
                    SamrtLockResponseMessageEx(MeLoginUser, InputRecieveCount, HexSendMessageIDString);
                    break;
                case 1:
                    //智能锁主动上传消息
                    SamrtLockRequestMessageEx(MeLoginUser, InputRecieveCount, HexSendMessageIDString);
                    break;

                case 255:
                    //移动端请求消息[UTF-8]
                    //SamrtMobileRequestMessageEx(InSocketServiceReadWriteChannel, InputRecieveCount);
                    SamrtMobileRequestMessageExx(MeLoginUser, InputRecieveCount, null);
                    break;

                case 254:
                    //移动端请求消息[UNICODE]
                    //SamrtMobileRequestMessageEx(InSocketServiceReadWriteChannel, InputRecieveCount);
                    break;
                default:
                    //如：来自图像流标志2，则不处理
                    break;

            }


        }

        protected override void ClientCloseEventCallback(string InMobileIDStr, string InLockIDStr)
        {
            if (InMobileIDStr == null) return;
            if (InLockIDStr == null) return;


            string CommandMessageStr = "lockoutoff";

            CommandMessageStr = CommandMessageStr + "#" + InMobileIDStr + "-" + InLockIDStr + "#[" + "howlock" + "," + GetDateTimeWeekIndex() + "]!";
            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

            int nBuffersLenght = MySendBaseMessageBytes.Length;
            byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


            MySendMessageBytes[2] = 250;

            //填充
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }

            //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[1通道和2号通道]--------------------------------------------------------------------------------

            try
            {

                this.StartAsynchSendMessage(MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel, MySendMessageBytes);
                this.StartAsynchSendMessage(MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel, MySendMessageBytes);

                /*
                SocketServiceReadWriteChannel NewReadWriteChannel;
                if (MeLoginUser.ReplyChannelLoginID < 1)
                {
                    //进行广播！
                    NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                    MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                    NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
                    MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                }
                else
                {

                    int ReplyChannelIndex = MeLoginUser.ReplyChannelLoginID - 1;
                    if (ReplyChannelIndex < 2)
                    {
                        NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[ReplyChannelIndex].MyReadWriteSocketChannel;
                        MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                    }
                    else
                    {
                        //通道错误；
                        MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应再转发移动端寻找通道错误：" + ReplyChannelIndex.ToString());
                    }
                }
                */

            }
            catch
            {

                DisplayResultInfor(1, "转发移动端通道错误[100]");
            }


        }
        
        
        //-----------------------------------------
        protected virtual void LongFileReceiveLoopProcess(LockServerLib.LongFileReceiveProc InLongFileReceiveProc, byte[] MyReadBuffers, uint InputRecieveCount)
        {
            InLongFileReceiveProc.LoopReadCount++;
            InLongFileReceiveProc.RecieveMessageLenght += InputRecieveCount;
            InLongFileReceiveProc.StartLongFileReceive(MyReadBuffers, InputRecieveCount);//文件传输

        }

        protected virtual void LongFileReceiveLoopProcessEx(LockServerLib.LongFileReceiveProc InLongFileReceiveProc, byte[] MyReadBuffers, uint InputRecieveCount)
        {
            //-----兼容物理锁-----------------------------------------------------
            InLongFileReceiveProc.LoopReadCount++;
            InLongFileReceiveProc.RecieveMessageLenght += InputRecieveCount;
            InLongFileReceiveProc.StartLongFileReceiveEx(MyReadBuffers, InputRecieveCount);//文件传输

        }


        //===============================
        private void SamrtLockRequestMessage(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, int InputRecieveCount, string MessageTypeID)
        {

            switch (MessageTypeID)
            {
                case "1001":
                    LockServerLib.LoginCommandParser MyLoginCommandParser;
                    MyLoginCommandParser = new LockServerLib.LoginCommandParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyLoginCommandParser.CompleteCommand();
                    break;
                case "1003":
                    LockServerLib.PingCommandParser MyPingCommandParser;
                    MyPingCommandParser = new LockServerLib.PingCommandParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyPingCommandParser.CompleteCommand();
                    break;

                case "1011":
                    LockServerLib.SynchInforParser MyMasterKeySynchInforParser;
                    MyMasterKeySynchInforParser = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyMasterKeySynchInforParser.CompleteCommand();
                    break;

                case "1013":
                    LockServerLib.SynchInforParser MyAddKeySynchInforParser;
                    MyAddKeySynchInforParser = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyAddKeySynchInforParser.CompleteCommand();
                    break;

                case "1015":
                    LockServerLib.SynchInforParser MyDeleteKeySynchInforParser;
                    MyDeleteKeySynchInforParser = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyDeleteKeySynchInforParser.CompleteCommand();
                    break;

                case "1017":
                    LockServerLib.SynchInforParser MySynchInforParser;
                    MySynchInforParser = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MySynchInforParser.CompleteCommand();
                    break;

                case "2003":
                    LockServerLib.SynchInforParser MyMasterKeySynchInforParserEx;
                    MyMasterKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyMasterKeySynchInforParserEx.CompleteCommand();
                    break;

                case "2007":
                    LockServerLib.SynchInforParser MyDeleteKeySynchInforParserEx;
                    MyDeleteKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyDeleteKeySynchInforParserEx.CompleteCommand();
                    break;

                case "2105":
                    LockServerLib.SynchInforParser MyAddKeySynchInforParserEx;
                    MyAddKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyAddKeySynchInforParserEx.CompleteCommand();
                    break;



                case "3007":
                    LockServerLib.ImageFileReceiveProcess MyImageFileReceiveProcess;
                    MyImageFileReceiveProcess = new LockServerLib.ImageFileReceiveProcess(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyImageFileReceiveProcess.CompleteCommand();
                    break;

                case "300B":
                    LockServerLib.QueryTimeParser MyQueryTimeParser;
                    MyQueryTimeParser = new LockServerLib.QueryTimeParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyQueryTimeParser.CompleteCommand();
                    break;

                case "0401":
                    LockServerLib.OPenDoorMessageProcess MyOPenDoorMessageProcess;
                    MyOPenDoorMessageProcess = new LockServerLib.OPenDoorMessageProcess(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyOPenDoorMessageProcess.CompleteCommand();
                    break;

                case "0403":
                    LockServerLib.LockStatusManager MyLockStatusManager;
                    MyLockStatusManager = new LockServerLib.LockStatusManager(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyLockStatusManager.CompleteCommand();
                    break;

                default:
                    SamrtLockResponseMessage(InputSocketServiceReadWriteChannel, InputRecieveCount, MessageTypeID);
                    break;

            }



        }
             
        private void SamrtLockResponseMessage(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, int InputRecieveCount, string MessageTypeID)
        {

            switch (MessageTypeID)
            {
                //修改母码
                case "2004":
                    LockServerLib.UpdateKeyParser MyUpdateKeyParser = new LockServerLib.UpdateKeyParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyUpdateKeyParser.ResponseToMobile();
                    break;

                //清除密钥信息
                case "200E":
                    LockServerLib.ClearLockKey MyClearLockKey = new LockServerLib.ClearLockKey(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyClearLockKey.ResponseToMobile();
                    break;

                //查询智能锁电池剩余电量
                case "3004":
                    LockServerLib.GetPowerParser MyGetPowerParser = new LockServerLib.GetPowerParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyGetPowerParser.ResponseToMobile();
                    break;
                //校对智能锁时间
                case "3006":
                    LockServerLib.SynchTimeParser MySynchTimeParser = new LockServerLib.SynchTimeParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MySynchTimeParser.ResponseToMobile();
                    break;

                //addkey
                case "2206":
                    LockServerLib.ElectKeyManager MyElectKeyManager1 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, InputRecieveCount, 0);
                    MyElectKeyManager1.ResponseToMobile();
                    break;

                //deletekey
                case "2008":
                    LockServerLib.ElectKeyManager MyElectKeyManager2 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, InputRecieveCount, 1);
                    MyElectKeyManager2.ResponseToMobile();
                    break;
                //deletekey
                case "200A":
                    LockServerLib.ElectKeyManager MyElectKeyManager3 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, InputRecieveCount, 2);
                    MyElectKeyManager3.ResponseToMobile();
                    break;

                //remotesnap
                case "200C":
                    LockServerLib.RemoteSnapParser MyRemoteSnapParser = new LockServerLib.RemoteSnapParser(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyRemoteSnapParser.ResponseToMobile();
                    break;

                  //getopen
                case "3002":
                    LockServerLib.GetOpenDoor MyGetOpenDoor = new LockServerLib.GetOpenDoor(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                    MyGetOpenDoor.ResponseToMobile();
                    break;

                default:
                    break;

            }



        }
        
        private void SamrtMobileRequestMessage(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, int MyRecieveCount)
        {
            string BaseMessageString = null;
            /*
                      if (InputSocketServiceReadWriteChannel.MyReadBuffers[2] == 254)
                      {
                         BaseMessageString = Encoding.Unicode.GetString(InputSocketServiceReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
                      }
                      else
                      {
                        BaseMessageString = Encoding.ASCII.GetString(InputSocketServiceReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
                      }
                      */
            BaseMessageString = Encoding.UTF8.GetString(InputSocketServiceReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);

            string CommandMessageKey = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
            switch (CommandMessageKey)
            {
                case "login":
                    LockServerLib.MobileBindLockParser MyMobileBindLockParser;
                    MyMobileBindLockParser = new LockServerLib.MobileBindLockParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyMobileBindLockParser.CompleteCommand();
                    break;
                case "updatekey":
                    LockServerLib.UpdateKeyParser MyUpdateKeyParser = new LockServerLib.UpdateKeyParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyUpdateKeyParser.RequestToLock();
                    break;

                case "clearlockkey":
                    LockServerLib.ClearLockKey MyClearLockKey = new LockServerLib.ClearLockKey(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyClearLockKey.RequestToLock();
                    break;
                case "getpower":
                    LockServerLib.GetPowerParser MyGetPowerParser = new LockServerLib.GetPowerParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyGetPowerParser.RequestToLock();
                    break;

                case "synchtime":
                    LockServerLib.SynchTimeParser MySynchTimeParser = new LockServerLib.SynchTimeParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MySynchTimeParser.RequestToLock();

                    break;

                case "addkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager1 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 0);
                    MyElectKeyManager1.RequestToLock();

                    break;

                case "deletekey":
                    LockServerLib.ElectKeyManager MyElectKeyManager2 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 1);
                    MyElectKeyManager2.RequestToLock();

                    break;

                case "tempkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager3 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 2);
                    MyElectKeyManager3.RequestToLock();

                    break;
                case "remotesnap":
                    LockServerLib.RemoteSnapParser MyRemoteSnapParser = new LockServerLib.RemoteSnapParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyRemoteSnapParser.RequestToLock();

                    break;
                case "getopen":
                    LockServerLib.GetOpenDoor MyGetOpenDoor = new LockServerLib.GetOpenDoor(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyGetOpenDoor.RequestToLock();

                    break;

                default:
                    break;

            }



        }
      
        //---------------------------------------------------
        private void SamrtLockRequestMessageEx(LoginUser MeLoginUser, int InputRecieveCount, string MessageTypeID)
        {

            switch (MessageTypeID)
            {
                case "1001":
                    LockServerLib.LoginCommandParser MyLoginCommandParser;
                    MyLoginCommandParser = new LockServerLib.LoginCommandParser(this, MeLoginUser, InputRecieveCount);
                    MyLoginCommandParser.CompleteCommand();
                    break;
                case "1003":
                    LockServerLib.PingCommandParser MyPingCommandParser;
                    MyPingCommandParser = new LockServerLib.PingCommandParser(this, MeLoginUser, InputRecieveCount);
                    MyPingCommandParser.CompleteCommandEx();
                    break;

                case "1011":
                    LockServerLib.SynchInforParser MyMasterKeySynchInforParser;
                    MyMasterKeySynchInforParser = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyMasterKeySynchInforParser.CompleteCommand();
                    break;

                case "1013":
                    LockServerLib.SynchInforParser MyAddKeySynchInforParser;
                    MyAddKeySynchInforParser = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyAddKeySynchInforParser.CompleteCommand();
                    break;

                case "1015":
                    LockServerLib.SynchInforParser MyDeleteKeySynchInforParser;
                    MyDeleteKeySynchInforParser = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyDeleteKeySynchInforParser.CompleteCommand();
                    break;

                case "1017":
                    LockServerLib.SynchInforParser MySynchInforParser;
                    MySynchInforParser = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MySynchInforParser.CompleteCommand();
                    break;

                case "2003":
                    LockServerLib.SynchInforParser MyMasterKeySynchInforParserEx;
                    MyMasterKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyMasterKeySynchInforParserEx.CompleteCommand();
                    break;

                case "2007":
                    LockServerLib.SynchInforParser MyDeleteKeySynchInforParserEx;
                    MyDeleteKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyDeleteKeySynchInforParserEx.CompleteCommand();
                    break;

                case "2105":
                    LockServerLib.SynchInforParser MyAddKeySynchInforParserEx;
                    MyAddKeySynchInforParserEx = new LockServerLib.SynchInforParser(this, MeLoginUser, InputRecieveCount);
                    MyAddKeySynchInforParserEx.CompleteCommand();
                    break;


                 //收件抓拍
                case "3007":
                    LockServerLib.ImageFileReceiveProcess MyImageFileReceiveProcess;
                    MyImageFileReceiveProcess = new LockServerLib.ImageFileReceiveProcess(this, MeLoginUser, InputRecieveCount);
                    MyImageFileReceiveProcess.CompleteCommand();
                    break;

                //异常抓拍
                case "3009":
                    LockServerLib.ImageFileReceiveProcess MyImageFileReceiveProcess2;
                    MyImageFileReceiveProcess2 = new LockServerLib.ImageFileReceiveProcess(this, MeLoginUser, InputRecieveCount);
                    MyImageFileReceiveProcess2.CompleteCommand();
                    break;


                case "300B":
                    LockServerLib.QueryTimeParser MyQueryTimeParser;
                    MyQueryTimeParser = new LockServerLib.QueryTimeParser(this, MeLoginUser, InputRecieveCount);
                    MyQueryTimeParser.CompleteCommand();
                    break;

                case "0401":
                    LockServerLib.OPenDoorMessageProcess MyOPenDoorMessageProcess;
                    MyOPenDoorMessageProcess = new LockServerLib.OPenDoorMessageProcess(this, MeLoginUser, InputRecieveCount);
                    MyOPenDoorMessageProcess.CompleteCommand();
                    break;

                case "0403":
                    LockServerLib.LockStatusManager MyLockStatusManager;
                    MyLockStatusManager = new LockServerLib.LockStatusManager(this, MeLoginUser, InputRecieveCount);
                    MyLockStatusManager.CompleteCommand();
                    break;

                default:
                    SamrtLockResponseMessageEx(MeLoginUser, InputRecieveCount, MessageTypeID);
                    break;

            }



        }

        private void SamrtLockResponseMessageEx(LoginUser MeLoginUser, int InputRecieveCount, string MessageTypeID)
        {

            switch (MessageTypeID)
            {
                //修改母码
                case "2004":
                    LockServerLib.UpdateKeyParser MyUpdateKeyParser = new LockServerLib.UpdateKeyParser(this, MeLoginUser, InputRecieveCount);
                    MyUpdateKeyParser.ResponseToMobile();
                    break;

                //清除密钥信息
                case "200E":
                    LockServerLib.ClearLockKey MyClearLockKey = new LockServerLib.ClearLockKey(this, MeLoginUser, InputRecieveCount);
                    MyClearLockKey.ResponseToMobile();
                    break;

                //查询智能锁电池剩余电量
                case "3004":
                    LockServerLib.GetPowerParser MyGetPowerParser = new LockServerLib.GetPowerParser(this, MeLoginUser, InputRecieveCount);
                    MyGetPowerParser.ResponseToMobile();
                    break;
                //校对智能锁时间
                case "3006":
                    LockServerLib.SynchTimeParser MySynchTimeParser = new LockServerLib.SynchTimeParser(this, MeLoginUser, InputRecieveCount);
                    MySynchTimeParser.ResponseToMobile();
                    break;

                //addkey
                case "2206":
                    LockServerLib.ElectKeyManager MyElectKeyManager1 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 0);
                    MyElectKeyManager1.ResponseToMobile();
                    break;

                //deletekey
                case "2008":
                    LockServerLib.ElectKeyManager MyElectKeyManager2 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 1);
                    MyElectKeyManager2.ResponseToMobile();
                    break;
                //deletekey
                case "200A":
                    LockServerLib.ElectKeyManager MyElectKeyManager3 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 2);
                    MyElectKeyManager3.ResponseToMobile();
                    break;

                //remotesnap
                case "200C":
  
                     //LockServerLib.RemoteSnapParser MyRemoteSnapParser = new LockServerLib.RemoteSnapParser(this, MeLoginUser, InputRecieveCount);
                    //MyRemoteSnapParser.ResponseToMobile();
                     LockServerLib.ImageFileReceiveProcess MyImageFileReceiveProcess;
                     MyImageFileReceiveProcess = new LockServerLib.ImageFileReceiveProcess(this, MeLoginUser, (int)InputRecieveCount);
                     MyImageFileReceiveProcess.CompleteCommand();
                    break;

                //getopen
                case "3002":
                    LockServerLib.GetOpenDoor MyGetOpenDoor = new LockServerLib.GetOpenDoor(this, MeLoginUser, InputRecieveCount);
                    MyGetOpenDoor.ResponseToMobile();
                    break;


                //remoteopen
                case "5002":
                    LockServerLib.RemoteOpen MyRemoteOpen = new LockServerLib.RemoteOpen(this, MeLoginUser, InputRecieveCount);
                    MyRemoteOpen.ResponseToMobile();
                    break;


                default:
                    break;

            }



        }
        
        private void SamrtMobileRequestMessageEx(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, int MyRecieveCount)
        {
            string BaseMessageString = null;
          
            BaseMessageString = Encoding.UTF8.GetString(InputSocketServiceReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);

            string CommandMessageKey = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
            switch (CommandMessageKey)
            {
                case "login":
                    LockServerLib.MobileBindLockParser MyMobileBindLockParser;
                    MyMobileBindLockParser = new LockServerLib.MobileBindLockParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyMobileBindLockParser.CompleteCommand();
                    break;
                case "updatekey":
                    LockServerLib.UpdateKeyParser MyUpdateKeyParser = new LockServerLib.UpdateKeyParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyUpdateKeyParser.RequestToLock();
                    break;

                case "clearlockkey":
                    LockServerLib.ClearLockKey MyClearLockKey = new LockServerLib.ClearLockKey(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyClearLockKey.RequestToLock();
                    break;
                case "getpower":
                    LockServerLib.GetPowerParser MyGetPowerParser = new LockServerLib.GetPowerParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyGetPowerParser.RequestToLock();
                    break;

                case "synchtime":
                    LockServerLib.SynchTimeParser MySynchTimeParser = new LockServerLib.SynchTimeParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MySynchTimeParser.RequestToLock();

                    break;

                case "addkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager1 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 0);
                    MyElectKeyManager1.RequestToLock();

                    break;

                case "deletekey":
                    LockServerLib.ElectKeyManager MyElectKeyManager2 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 1);
                    MyElectKeyManager2.RequestToLock();

                    break;

                case "tempkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager3 = new LockServerLib.ElectKeyManager(this, InputSocketServiceReadWriteChannel, MyRecieveCount, 2);
                    MyElectKeyManager3.RequestToLock();

                    break;
                case "remotesnap":
                    LockServerLib.RemoteSnapParser MyRemoteSnapParser = new LockServerLib.RemoteSnapParser(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyRemoteSnapParser.RequestToLock();

                    break;
                case "getopen":
                    LockServerLib.GetOpenDoor MyGetOpenDoor = new LockServerLib.GetOpenDoor(this, InputSocketServiceReadWriteChannel, MyRecieveCount);
                    MyGetOpenDoor.RequestToLock();

                    break;

                default:
                    break;

            }



        }

        private void SamrtMobileRequestMessageExx(LoginUser MeLoginUser, int InputRecieveCount, string MessageTypeID)
        {

            //--最新版本--
            string BaseMessageString = null;
            BaseMessageString = Encoding.UTF8.GetString(MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers, 3, InputRecieveCount - 3);

            string CommandMessageKey = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
            switch (CommandMessageKey)
            {
                case "login":
                    LockServerLib.MobileBindLockParser MyMobileBindLockParser;
                    MyMobileBindLockParser = new LockServerLib.MobileBindLockParser(this, MeLoginUser, InputRecieveCount);
                    MyMobileBindLockParser.CompleteCommand();
                    break;
                    
                case "updatekey":
                    LockServerLib.UpdateKeyParser MyUpdateKeyParser = new LockServerLib.UpdateKeyParser(this, MeLoginUser, InputRecieveCount);
                    MyUpdateKeyParser.RequestToLock();
                    break;

                case "clearlockkey":
                    LockServerLib.ClearLockKey MyClearLockKey = new LockServerLib.ClearLockKey(this, MeLoginUser, InputRecieveCount);
                    MyClearLockKey.RequestToLock();
                    break;
                case "getpower":
                    LockServerLib.GetPowerParser MyGetPowerParser = new LockServerLib.GetPowerParser(this, MeLoginUser, InputRecieveCount);
                    MyGetPowerParser.RequestToLock();
                    break;

                case "synchtime":
                    LockServerLib.SynchTimeParser MySynchTimeParser = new LockServerLib.SynchTimeParser(this, MeLoginUser, InputRecieveCount);
                    MySynchTimeParser.RequestToLock();

                    break;

                case "addkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager1 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 0);
                    MyElectKeyManager1.RequestToLock();

                    break;

                case "deletekey":
                    LockServerLib.ElectKeyManager MyElectKeyManager2 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 1);
                    MyElectKeyManager2.RequestToLock();

                    break;

                case "tempkey":
                    LockServerLib.ElectKeyManager MyElectKeyManager3 = new LockServerLib.ElectKeyManager(this, MeLoginUser, InputRecieveCount, 2);
                    MyElectKeyManager3.RequestToLock();

                    break;
                case "remotesnap":
                    LockServerLib.RemoteSnapParser MyRemoteSnapParser = new LockServerLib.RemoteSnapParser(this, MeLoginUser, InputRecieveCount);
                    MyRemoteSnapParser.RequestToLock();

                    break;
                case "getopen":
                    LockServerLib.GetOpenDoor MyGetOpenDoor = new LockServerLib.GetOpenDoor(this, MeLoginUser, InputRecieveCount);
                    MyGetOpenDoor.RequestToLock();

                    break;
                case "remoteopen":
                    LockServerLib.RemoteOpen MyRemoteOpen = new LockServerLib.RemoteOpen(this, MeLoginUser, InputRecieveCount);
                    MyRemoteOpen.RequestToLock();

                    break;

                case "mobileoffline":
                    LockServerLib.mobileoffline Mymobileoffline = new LockServerLib.mobileoffline(this, MeLoginUser, InputRecieveCount);
                    Mymobileoffline.RequestToLock();

                    break;
                default:
                    break;

            }



        }
      
        //===================================================================================
        protected void SocketClientColseServerCallBack()
        {

            /////this.DisplayResultInfor(1, "云锁服务器已关闭Socket服务！");

        }
        
        public void CloudServerAckToLock(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {

            int MySendByteCount = 23;//加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;

            if (this.DataFormateFlag)
            {
                HexSendMessageIDString = "0140";
            }
            else
            {
                HexSendMessageIDString = "4001";
            }


            //填充字节信息头
            if (this.DataFormateFlag)
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
            }
            else
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
            }



            MySendMessageBytes[2] = 1;

            MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
            MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);



            if (this.DataFormateFlag)
            {
                MySendMessageBytes[10] = 1;
                MySendMessageBytes[12] = 1;

            }
            else
            {
                MySendMessageBytes[11] = 1;
                MySendMessageBytes[13] = 1;
            }

            //发送命令消息
            this.StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);



        }

        public void CloudServerAckToLockEx(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel,int PackgeIndex)
        {

            int MySendByteCount = 24;//包序号+加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;

            if (this.DataFormateFlag)
            {
                HexSendMessageIDString = "0140";
            }
            else
            {
                HexSendMessageIDString = "4001";
            }


            //填充字节信息头
            if (this.DataFormateFlag)
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
            }
            else
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
            }



            MySendMessageBytes[2] = 1;

            MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
            MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);



            if (this.DataFormateFlag)
            {
                MySendMessageBytes[10] = 1;
                MySendMessageBytes[12] = 1;

            }
            else
            {
                MySendMessageBytes[11] = 1;
                MySendMessageBytes[13] = 1;
            }
            MySendMessageBytes[22] = (byte)PackgeIndex;// 0x1;//包序号
            //发送命令消息
            this.StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);



        }
        
        public void SamrtLockResponseToSave(string LockID, string MessageID, string MessageStr)
        {
            
           
            /*
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];           
            SqlConnection MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            MySqlConnection.Open();

            SqlCommand MySqlCommand = new SqlCommand("AddMessage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@MessageID";
            parm.Value = MessageID;
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@MessageStr";
            parm.Value = MessageStr;
            MySqlCommand.Parameters.Add(parm);

            int MyCount = MySqlCommand.ExecuteNonQuery();
            MySqlConnection.Close();

             * */

            MessageListManager MyMessageListManager = new MessageListManager();

            MyMessageListManager.InsertMessageSave(LockID, MessageID, MessageStr); 


              //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            //MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            //MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            
           
             /*
            if (MyCount == 0)
            {
                          
                string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库成功", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


            }
            else
            {
                
                string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库失败", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
            }
          */
           



        }

        public void SamrtLockMessageToSave(string LockID, string MessageID, string MessageStr)
        {
              //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            //SqlConnection MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            //MySqlConnection.Open();

            SqlCommand MySqlCommand = new SqlCommand("AddMessage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@MessageID";
            parm.Value = MessageID;
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@MessageStr";
            parm.Value = MessageStr;
            MySqlCommand.Parameters.Add(parm);

               //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            //MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            //MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            int MyCount = MySqlCommand.ExecuteNonQuery();
           
            //MySqlConnection.Close();

              /*
           if (MyCount == 0)
           {
                          
               string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
               this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库成功", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


           }
           else
           {
                
               string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
               this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库失败", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
           }
         */




        }
        

        //-----***现在没有用到***----------------------------------------------------------
        public void CloudServerFailToLockEx(LoginUser MeLoginUser, string MessageTypeID)
        {
            //快速响应失败消息
            int MySendByteCount = 24;//加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;

            HexSendMessageIDString = MessageTypeID; //小端格式;
        
                 
            //if (this.DataFormateFlag)
            //{
            //    HexSendMessageIDString = MessageID;// "0140";
            //}
            /////else
            //{
            //    //HexSendMessageIDString = "4001";
            //}


            //填充字节信息头
            if (this.DataFormateFlag)
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
            }
            else
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
            }



            MySendMessageBytes[2] = 1;

            MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
            MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);



            if (this.DataFormateFlag)
            {
                MySendMessageBytes[10] = 1;
                MySendMessageBytes[12] = 1;

            }
            else
            {
                MySendMessageBytes[11] = 1;
                MySendMessageBytes[13] = 1;
            }
            MySendMessageBytes[22] = 0xFF;
            //---发送失败应答消息
            this.StartAsynchSendMessage(MeLoginUser.MyReadWriteSocketChannel, MySendMessageBytes);

            //--补充操作：即将关闭Socket作准备-----------------------------------------
            //MyManagerLoginLockUser.CRUDLoginUserList(ref InputSocketServiceReadWriteChannel.MyTCPClient, 1);   //改变Socket通道标志
            //MyManagerLoginLockUser.RemoveNotLoginUser(ref InputSocketServiceReadWriteChannel.MyTCPClient);
             MeLoginUser.ChannelStatus = 1;

        }

        public void CloudServerFailToLock(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, string MessageTypeID)
        {
            //快速响应失败消息
            int MySendByteCount = 24;//加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;

            HexSendMessageIDString = MessageTypeID; //小端格式;


            //if (this.DataFormateFlag)
            //{
            //    HexSendMessageIDString = MessageID;// "0140";
            //}
            /////else
            //{
            //    //HexSendMessageIDString = "4001";
            //}


            //填充字节信息头
            if (this.DataFormateFlag)
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
            }
            else
            {
                MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
            }



            MySendMessageBytes[2] = 1;

            MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
            MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);



            if (this.DataFormateFlag)
            {
                MySendMessageBytes[10] = 1;
                MySendMessageBytes[12] = 1;

            }
            else
            {
                MySendMessageBytes[11] = 1;
                MySendMessageBytes[13] = 1;
            }
            MySendMessageBytes[22] = 0xFF;
            //---发送失败应答消息
            this.StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);

            //--补充操作：即将关闭Socket作准备-----------------------------------------
            MyManagerLoginLockUser.CRUDLoginUserList(ref InputSocketServiceReadWriteChannel.MyTCPClient, 1);   //改变Socket通道标志

            //MyManagerLoginLockUser.RemoveNotLoginUser(ref InputSocketServiceReadWriteChannel.MyTCPClient);


        }
        
        public void QuickReplyAuthToLock(SocketServiceReadWriteChannel MyReadWriteChannel, uint MyRecieveCount)
        {
            string HexMessageTypeID = string.Format("{0:X2}", MyReadWriteChannel.MyReadBuffers[9]) + string.Format("{0:X2}", MyReadWriteChannel.MyReadBuffers[8]);
            switch (HexMessageTypeID)
            {
                case "1003":
                    CloudServerFailToLock(MyReadWriteChannel, "0410");
                    break;

                case "1011":
                    CloudServerFailToLock(MyReadWriteChannel, "1210");
                    break;

                case "1013":
                    CloudServerFailToLock(MyReadWriteChannel, "1410");
                    break;

                case "1015":
                    CloudServerFailToLock(MyReadWriteChannel, "1610");
                    break;

                case "1017":
                    CloudServerFailToLock(MyReadWriteChannel, "1810");
                    break;

                case "2003":
                    CloudServerFailToLock(MyReadWriteChannel, "0420");
                    break;

                case "2007":
                    CloudServerFailToLock(MyReadWriteChannel, "0820");
                    break;

                case "2105":
                    CloudServerFailToLock(MyReadWriteChannel, "0621");
                    break;


                case "3007":
                    CloudServerFailToLock(MyReadWriteChannel, "0830");
                    break;

                case "3009":
                    CloudServerFailToLock(MyReadWriteChannel, "0A30");
                    break;

                case "300B":
                    CloudServerFailToLock(MyReadWriteChannel, "0C30");
                    break;

                default:
                    byte MessageTypeFirstFlag = MyReadWriteChannel.MyReadBuffers[2];
                    CommandDefineDispatch(MyReadWriteChannel, MyReadWriteChannel.MyReadBuffers, (int)MessageTypeFirstFlag, (int)MyRecieveCount);//只有【Login-命令】能传输再处理
                    break;


            }


        }

       //---------------------------------------------------------------------------------------


    }
}
