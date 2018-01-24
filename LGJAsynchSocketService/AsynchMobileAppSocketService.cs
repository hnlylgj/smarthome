using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using LGJAsyncSocketClientLib;
using  SmartBusServiceLib;
using LGJAsynchSocketService.MobileAppServerLib;

namespace LGJAsynchSocketService
{
    public class AsynchMobileAppSocketService : AsynchSocketServiceBaseFrame
    {

        public ManagerLoginMobileUser MyManagerLoginMobileUser;
        public MobileServerClientAPI MyMobileServerClientAPI;

        public AsynchMobileAppSocketService():base()
        {
                     
            MyManagerLoginMobileUser = new ManagerLoginMobileUser();
            MyManagerSocketLoginUser = MyManagerLoginMobileUser;

            MyMobileServerClientAPI = new MobileServerClientAPI();
            MyLGJSocketClientAPIBase = MyMobileServerClientAPI;   

            InitServiceParas();
       

        }
      
        public AsynchMobileAppSocketService(ManagerSocketLoginUser InManagerSocketLoginUser):base(InManagerSocketLoginUser)
        {
          
            
            //MyManagerLoginMobileUser = new ManagerLoginMobileUser();
            //MyManagerSocketLoginUser = MyManagerLoginMobileUser;

            //MyMobileServerClientAPI = new LGJSocketClientAPILib.MobileServerClientAPI();
            //MyLGJSocketClientAPIBase = MyMobileServerClientAPI; 
       
        
        }
    
        private  void InitServiceParas()
        {
            //----------------------------------------------------
            //1.打开配置文件,获取相应的appSettings配置节
            string SmartBusIPSet;
            string SmartBusPortSet;
            string LocalListenPort;
            string EvetLogFlagStr;
            SmartBusIPSet = (string)ConfigurationManager.AppSettings["SmartBusIPSet"];
            SmartBusPortSet = (string)ConfigurationManager.AppSettings["SmartBusPortSet"];
            LocalListenPort = (string)ConfigurationManager.AppSettings["LocalListenPort"];
            EvetLogFlagStr = (string)ConfigurationManager.AppSettings["EvetLogFlag"];

            EvetLogFlag = string.IsNullOrEmpty(EvetLogFlagStr) || EvetLogFlagStr == "0" ? true : false;
            //MyMobileServerClientAPI.SetServerIP("127.0.0.1", 8900);
            MyMobileServerClientAPI.SetServerIP(SmartBusIPSet, int.Parse(SmartBusPortSet));

            MyTCPListerPort = int.Parse(LocalListenPort);//8910; 
            MySocketServiceTypeID = SocketServiceTypeID.MobileAppServer;

            MyRemoteServerIPStr = SmartBusIPSet;// "127.0.0.1";
            MyRemoteServerPort = int.Parse(SmartBusPortSet);// 8900;
           
            //if (MyManagerSocketLoginUser == null) { MyManagerSocketLoginUser = new  ManagerLoginMobileUser(); }
            //if (MyManagerSocketLoginUser == null)
            //{
                //MyManagerLoginMobileUser = new ManagerLoginMobileUser();
                //MyManagerSocketLoginUser = MyManagerLoginMobileUser;

            //}

            MyManagerLoginMobileUser.MyAsynchSendMessageCallback = new ManagerLoginMobileUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);
     
            MyMobileServerClientAPI.MyRemoteServerColseCallBack = new MobileServerClientAPI.RemoteServerColseHandler(this.SocketClientColseServerCallBack);
            MyMobileServerClientAPI.MyRecieveMessageCallback = new MobileServerClientAPI.RecieveMessageHandler(this.SocketClientResponseCallBack);
            MyMobileServerClientAPI.ReturnSendMessageInvoke = new MobileServerClientAPI.SendMessageHandler(this.SocketClientRequestCallBack);   
             
            

         }
        protected override void ClientCloseEventCallback(string InMobileIDStr, string InLockIDStr)
        {
            if (InMobileIDStr == null) return;
            if (InLockIDStr == null) return;
            try
            {
                string CurrentCommandStr = "mobileoffline";
                string LockIDStr = InLockIDStr;
                string MobileIDStr = InMobileIDStr;
                string MySendMessageString = CurrentCommandStr + "#" + LockIDStr + "-" + MobileIDStr + "#" + "!"; ;
                int nBuffersLenght = 0;
                byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(MySendMessageString);
                nBuffersLenght = MySendBaseMessageBytes.Length;// MySendMessageString.Length;
                byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


                MySendMessageBytes[2] = 255;

                for (int i = 0; i < nBuffersLenght; i++)
                {

                    MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

                }


                //直接转发给云锁服务器！
                MyMobileServerClientAPI.SynchSendCommand(MySendMessageBytes);

            }
            catch (Exception ExceptionInfor)
            {
                ;//SendIDFlag = 2;
            }

        }
        protected override void ProcessMessageEntry(LoginUser MeLoginUser, uint MyRecieveCount, int MyRecieveAvailable)
        {
            /*
            byte MessageTypeFirstFlag = MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[2];

            if (MyLoginUser.LoginID > 1)
            {
                //--1.锁端-----------------------------------------------------------
                string HexSendMessageIDString;
                if (DataFormateFlag)
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[9]) + string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[8]);
                }
                else
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[8]) + string.Format("{0:X2}", MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers[9]);
                }
                //--2.---------------------------------------------------------------

                if (MyLoginUser.ChannelStatus == 1)
                {
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
                        CommandDefineDispatchEx(MyLoginUser, (int)MyRecieveCount); ;//所有命令[和第一次文件传输]能传输再处理
                    }

                }
            }
            else
            {
                //--移动端进入通道------------------------------
                CommandDefineDispatchEx(MyLoginUser, (int)MyRecieveCount); ;//所有来自移动端的命令



            }
            */

            CommandDefineDispatchEx(MeLoginUser, (int)MyRecieveCount);
        
        }
                
        public override void CommandDefineDispatch(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] MyReadBuffers, int RecieveMessageFlag, int InputRecieveCount)
        {
            /*
                byte WebFlagID = MyReadBuffers[1];
            
                string BaseMessageString = null;
                
                BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);

                string MyTimeMarkerStr = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", InputSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));

                if (WebFlagID == 0)//非Web：第二个字节为0：异步Socket客户端；1：同步Socket客户端
                {
                  string CommandMessageStr = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));

                  switch (CommandMessageStr)
                  {
                    case "login":
                        MobileAppServerLib.MobileLoginManager MyMobileLoginManager;
                        MyMobileLoginManager = new MobileAppServerLib.MobileLoginManager(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                        MyMobileLoginManager.CompleteCommand();
                        break;

                    case "ping":
                        DirectResponsePing(InputSocketServiceReadWriteChannel);
                        break;


                    default:
                        //直接转发给云锁服务器！
                        byte[] MySendMessageBytes = new byte[InputRecieveCount];
                        for (int i = 0; i < InputRecieveCount; i++)
                        {

                            MySendMessageBytes[i] = MyReadBuffers[i];

                        }
                        MyMobileServerClientAPI.SynchSendCommand(MySendMessageBytes);
                        break;

                 }
             }
          else //Web:同步Socket客户端和其他各种同步客户端
             {
                 string CommandMessageStr = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
                 int IndexStart = BaseMessageString.IndexOf("#") + 1;
                 switch (CommandMessageStr)
                 {
                     case "authtication":
                         string UserCertID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         FromDBAuthtication(UserCertID, InputSocketServiceReadWriteChannel);
                         break;

                     case "getchannel":
                         string CustomerID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         FromDBGetChannel(CustomerID, InputSocketServiceReadWriteChannel);
                         break;


                     case "getimage":
                         string LockSnapID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         FromDBGetImage(LockSnapID, InputSocketServiceReadWriteChannel);
                         break;


                     case "getkeylist":
                         //string CustomerID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         //FromDBGetChannel(CustomerID, InputSocketServiceReadWriteChannel);
                         break;

                     case "getopenlist":
                         //string CustomerID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         //FromDBGetChannel(CustomerID, InputSocketServiceReadWriteChannel);
                         break;

                     case "getsnaplist":
                         //string CustomerID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                         //FromDBGetChannel(CustomerID, InputSocketServiceReadWriteChannel);
                         break;

                     default:
                         MobileAppServerLib.SynchTcpClientChannel MySynchTcpClientChannel;
                         MySynchTcpClientChannel = new MobileAppServerLib.SynchTcpClientChannel(this, InputSocketServiceReadWriteChannel, InputRecieveCount);
                         MySynchTcpClientChannel.CompleteCommand();
                        break;

                 }
                

             }
             */

        }

        public void CommandDefineDispatchEx(LoginUser MeLoginUser, int InputRecieveCount)
        {
          
            /*
            //byte[] MyReadBuffer = MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers;
            //string MessageString = null;
           // MessageString = Encoding.UTF8.GetString(MyReadBuffer, 0, InputRecieveCount);
           // string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            // DisplayResultInfor(2, string.Format(MyTimeMarker + "[{0}]{1}]", MeLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint, MessageString));
            // DisplayResultInfor(1, string.Format("[{0}]{1}", MeLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint, MessageString));
            //return;
            */
            if (InputRecieveCount < 5) return;

            byte[] MyReadBuffers = MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers;
            SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            byte MessageFlagID = MyReadBuffers[1];

            string BaseMessageString = null;

            BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);

            //string MyTimeMarkerStr = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", InputSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString));
            
            if (MessageFlagID == 0|| MessageFlagID == 16)//第二个字节为0、16：异步Socket客户端；1：同步Socket客户端(非Web）：
            {
                string CommandMessageStr = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));

                switch (CommandMessageStr)
                {
                    case "login":
                        MobileAppServerLib.MobileLoginManager MyMobileLoginManager;
                        MyMobileLoginManager = new MobileAppServerLib.MobileLoginManager(this, MeLoginUser, InputRecieveCount);
                        MyMobileLoginManager.CompleteCommand();
                        break;

                    case "ping":
                        //DirectResponsePing(InputSocketServiceReadWriteChannel);
                        DirectResponsePingEx(MeLoginUser);
                        break;

                    case "close":
                         DirectCloseLoginUser(MeLoginUser);
                        break;


                    default:
                        //直接转发给云锁服务器！
                        byte[] MySendMessageBytes = new byte[InputRecieveCount];
                        for (int i = 0; i < InputRecieveCount; i++)
                        {

                            MySendMessageBytes[i] = MyReadBuffers[i];

                        }
                        MyMobileServerClientAPI.SynchSendCommand(MySendMessageBytes);
                        break;

                }
            }
            else 
            {
                if (MessageFlagID == 100)//第二个字节为100：IM
                {

                    MessageRountToEndpoit(MyReadBuffers, InputRecieveCount);

                }
                else//同步Socket客户端
                {
                    string CommandMessageStr = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
                    int IndexStart = BaseMessageString.IndexOf("#") + 1;
                    switch (CommandMessageStr)
                    {
                        case "authtication":
                            string UserCertID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            FromDBAuthtication(UserCertID, InputSocketServiceReadWriteChannel);//通过客户LoginID号获得通道
                            break;

                        case "getchannel":
                            string CustomerID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            FromDBGetChannel(CustomerID, InputSocketServiceReadWriteChannel);//通过客户ID号获得通道[为TOMCAT定制]
                            break;

                        case "mobilelogin":
                            string UserPassCertID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            MobileLoginAuthtication(UserPassCertID, InputSocketServiceReadWriteChannel);
                            break;


                        case "managerlogin":
                            string managerPassCertID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            ManagerLoginAuthtication(managerPassCertID, InputSocketServiceReadWriteChannel);
                            break;


                        case "lockstatus":
                            string LockStatusIDStr = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            LockStatusManagerment(LockStatusIDStr, InputSocketServiceReadWriteChannel);
                            break;


                        case "customercreate":
                            string customercreateStr = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            CustomerCreateManagerment(customercreateStr, InputSocketServiceReadWriteChannel);
                            break;

                        case "getimage":
                            string LockSnapID = BaseMessageString.Substring(IndexStart, BaseMessageString.LastIndexOf("#") - IndexStart);
                            FromDBGetImage(LockSnapID, InputSocketServiceReadWriteChannel);
                            break;


                        case "getkeylist":

                            break;

                        case "getopenlist":

                            break;

                        case "getsnaplist":

                            break;

                        default:
                            MobileAppServerLib.SynchTcpClientChannel MySynchTcpClientChannel;
                            MySynchTcpClientChannel = new MobileAppServerLib.SynchTcpClientChannel(this, MeLoginUser, InputRecieveCount);
                            MySynchTcpClientChannel.CompleteCommand();
                            break;

                    }

                }


            }



        }


        protected override void SendMessageForString(Byte[] InSendMessageBuffers, SocketServiceReadWriteChannel InputReadWriteChannel)
        {
            //--向从移动端发的回显消息
            //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);

            string SendMessageString = null;
              //把来自移动端的消息字符串显示-----------------------------------------------------------------------------------------------------------------------
             /*
              if (InSendMessageBuffers[2] == 254)
                {

                    SendMessageString = Encoding.Unicode.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                       
                }
                else
                {
                    SendMessageString = Encoding.ASCII.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                }
            */

              SendMessageString = Encoding.UTF8.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
              this.DisplayResultInfor(3, string.Format(MyTimeMarkerStr + "[{0}]{1}", InputReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, SendMessageString));
              


        }
       
        //========================================================================================
        public override void SocketClientResponseCallBack(byte[] RecieveMessageBytes, int RecieveCount)
        {
            string BaseMessageString;
             /*
            if (RecieveMessageBytes[2] == 254)
            {
                BaseMessageString = Encoding.Unicode.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
            }
            else
            {
                BaseMessageString = Encoding.ASCII.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
            }
            */

            BaseMessageString = Encoding.UTF8.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
            string MyMobileID = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
            string MyLockID = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);
            string  MyCommandMessage = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));
            
            //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string ClientSocketEndPointStr = MyMobileServerClientAPI.MyTcpClient.Client.LocalEndPoint.ToString();
            //--找移动端通道--------------------------------------------------------------------------------
            /*
         LGJAsynchSocketService.LockServerLib.FindMobileChannel MyBindedMobileChannel = new LGJAsynchSocketService.LockServerLib.FindMobileChannel(MyMobileID);
         SocketServiceReadWriteChannel MyReadWriteSocketChannel;


         try
         {
             MyReadWriteSocketChannel = this.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.SelectMobileChannel)).MyReadWriteSocketChannel;
         }
         catch
         {
             MyReadWriteSocketChannel = null;
         }
         */

            FindMobileChannel MyFindMobileChannel = new FindMobileChannel(MyLockID);
            LoginUser MyLoginUser = this.MyManagerLoginMobileUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyFindMobileChannel.BindedMobileChannel));

            if (MyLoginUser == null)
            {
                //不存在或者不在线                
                this.DisplayResultInfor(1, string.Format("[{0}]所绑定的移动端通道不在线", MyLockID));    
              
                //this.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "|<-{0}",  BaseMessageString));
                this.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[<-{0}]{1}", ClientSocketEndPointStr, BaseMessageString));
                return;

            }
            else
            {
                //this.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "|<-{0}",  BaseMessageString));
                this.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[<-{0}]{1}", ClientSocketEndPointStr, BaseMessageString));

            }
            //下一步处理
            SocketServiceReadWriteChannel MyMobileSocketChannel = MyLoginUser.MyReadWriteSocketChannel;

            //--再转发出去-----------------------------------------------------------------------------------
            if (MyMobileSocketChannel != null)
            {


                byte[] MySendMessageBytes = new byte[RecieveCount];

                for (int i = 0; i < RecieveCount; i++)
                {

                    MySendMessageBytes[i] = RecieveMessageBytes[i];

                }
               

                StartAsynchSendMessage(MyMobileSocketChannel, MySendMessageBytes);
                //MyTimeMarkerStr = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                //MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);
                //this.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + " 接收云智能总线[{0}]响应命令:{1}", MyLockID, MyCommandMessage));

            }
            else
            {

                this.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]转发命令{1}出错！", MyLockID, MyCommandMessage));
            }



        }

        public void MessageRountToEndpoit(byte[] RecieveMessageBytes, int RecieveCount)
        {
            string BaseMessageString;
           

            BaseMessageString = Encoding.UTF8.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
            string MySrcMobileID = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
            string MyDesMobileID = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);
            string MyCommandMessage = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));

            //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
          
            //--找移动端通道--------------------------------------------------------------------------------
           

            FindMobileChannel MyFindMobileChannel = new FindMobileChannel(MyDesMobileID,10);
            LoginUser MyLoginUser = this.MyManagerLoginMobileUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyFindMobileChannel.BindedMobileChannel));
           
            if (MyLoginUser == null)
            {
                //不存在或者不在线                
                this.DisplayResultInfor(1, string.Format("[{0}]对方通道不在线", MyDesMobileID));
                //this.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "|<-{0}",  BaseMessageString));
                //this.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[<-{0}]{1}", ClientSocketEndPointStr, BaseMessageString));
                return;

            }
            else
            {
                //string ClientSocketEndPointStr = MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.LocalEndPoint.ToString();
                //this.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[<-{0}]{1}", ClientSocketEndPointStr, BaseMessageString));

            }
            //--再转发出去-----------------------------------------------------------------------------------
            SocketServiceReadWriteChannel MyMobileSocketChannel = MyLoginUser.MyReadWriteSocketChannel;           
            if (MyMobileSocketChannel != null)
            {


                byte[] MySendMessageBytes = new byte[RecieveCount];

                for (int i = 0; i < RecieveCount; i++)
                {

                    MySendMessageBytes[i] = RecieveMessageBytes[i];

                }
                
                StartAsynchSendMessage(MyMobileSocketChannel, MySendMessageBytes);
               
            }
            else
            {

                this.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]IM转发命令{1}出错！", MyDesMobileID, MyCommandMessage));
            }



        }

        public override void SocketClientRequestCallBack(string MessageStr)
        {

            this.DisplayResultInfor(3, MessageStr);//只是回显一下而已
        }

        protected  void SocketClientColseServerCallBack()
        {

            this.DisplayResultInfor(1, "云智能总线已关闭Socket服务！");

        }
             
        protected void DirectResponsePing(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
            //老版本
            string ReplyCommandMessageStr = "ping";
            ReplyCommandMessageStr = ReplyCommandMessageStr + "#true!";
            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

            int nBuffersLenght = MySendBaseMessageBytes.Length;
            byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];

            MySendMessageBytes[2] = 255;
            //填充
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }

            StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);       
            

        }

        protected void DirectResponsePingEx(LoginUser MeLoginUser)
        {
            //最新版本
            string ReplyCommandMessageStr = "ping";
            ReplyCommandMessageStr = ReplyCommandMessageStr + "#true!";
            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

            int nBuffersLenght = MySendBaseMessageBytes.Length;
            byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];

            MySendMessageBytes[2] = 255;
            //填充
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }
            //处理完毕开始应答
            StartAsynchSendMessage(MeLoginUser.MyReadWriteSocketChannel, MySendMessageBytes);
            MeLoginUser.KeepTime = DateTime.Now;//更新通道活动时间
            this.DisplayResultInfor(4, "");

        }

        protected void DirectCloseLoginUser(LoginUser MeLoginUser)
        {

            MeLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
            ClientCloseEventCallback(MeLoginUser.MobileID, MeLoginUser.LockID);
            this.MyManagerLoginMobileUser.MyLoginUserList.Remove(MeLoginUser);
            DisplayResultInfor(4, "");


        }
        //===========同步方法==============================================================================
        protected void FromDBAuthtication(string UserCertID, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
            int ReturnCode = -1;
            string ReplyCommandMessageStr = "authtication";
            //----1--------------------------------------------------------------
            string UserName = UserCertID.Substring(0, UserCertID.IndexOf("-"));
            string UserPass = UserCertID.Substring(UserCertID.IndexOf("-")+1);
             CustomerManager MyCustomerManager=new CustomerManager();
             Customer MyCustomer = MyCustomerManager.FindCustomer(UserName);
             if (MyCustomer == null)
             {
                 ReturnCode = -1;
                 ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
             }
             else
             {
                 if (MyCustomer.LoginName == UserName && MyCustomer.Password == UserPass)
                 {
                     ReturnCode = MyCustomer.CustomerID;
                     ReplyCommandMessageStr =string.Format(ReplyCommandMessageStr + "#true[{0}]!", ReturnCode);
                 }
                 else
                 {
                     ReturnCode = 0;
                     ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[11]!";
                 }

             }
            //----2.---------------------------------------------------------

             ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);


        }
        
        protected void MobileLoginAuthtication(string UserCertID, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
            int ReturnCode = -1;
            string ChannelIDStr = null; 
            string ReplyCommandMessageStr = "mobilelogin";
            //----1--------------------------------------------------------------
            string UserName = UserCertID.Substring(0, UserCertID.IndexOf("-"));
            string UserPass = UserCertID.Substring(UserCertID.IndexOf("-") + 1);
            CustomerManager MyCustomerManager = new CustomerManager();
            Customer MyCustomer = MyCustomerManager.FindCustomer(UserName);//这个地方存在IO瓶紧
            if (MyCustomer == null)
            {
                ReturnCode = -1;
                ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
            }
            else
            {
                if (MyCustomer.LoginName == UserName && MyCustomer.Password == UserPass)
                {
                    ReturnCode = MyCustomer.CustomerID;
                    //ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", ReturnCode);
                    Channel MyChannel;
                    ChannelManager MyChannelManager = new ChannelManager();
                    MyChannel = MyChannelManager.FindChannel(MyCustomer.CustomerID);
                    if (MyChannel == null)
                    {
                        ChannelIDStr = null;
                        ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
                    }
                    else
                    {

                        ChannelIDStr = MyChannel.LockID + "-" + MyChannel.MobileID;
                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", ChannelIDStr);

                    }


                }
                else
                {
                    ReturnCode = 0;
                    ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[11]!";
                }

            }
            //----2.---------------------------------------------------------

            ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);


        }


        protected void ManagerLoginAuthtication(string ManagerCertID, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
            int ReturnCode = -1;
            string ReplyCommandMessageStr = "managerlogin";
            //----1--------------------------------------------------------------
            string UserName = ManagerCertID.Substring(0, ManagerCertID.IndexOf("-"));
            string UserPass = ManagerCertID.Substring(ManagerCertID.IndexOf("-") + 1);
            Manager MyManager;
            ManagerCRUD MyManagerCRUD = new ManagerCRUD();
            MyManager = MyManagerCRUD.FindManager(UserName);

            if (MyManager == null)
            {
                ReturnCode = -1;
                ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
            }
            else
            {
                if (MyManager.LoginName == UserName && MyManager.PassWord == UserPass)
                {
                    ReturnCode = MyManager.ManagerID;
                    ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]{1}!", ReturnCode, MyManager.RightType);//尾随ID码与权力码
                }
                else
                {
                    
                    ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[11]!";
                }

            }
            //----2.---------------------------------------------------------

            ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);


        }


        protected void LockStatusManagerment(string LockStatusStr, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
             string ReplyCommandMessageStr = "lockstatus";
            //----1------------------------------------------------------------------------------------------------

            string ManagerIDStr = LockStatusStr.Substring(0, LockStatusStr.IndexOf(":"));
            int IndexStart = LockStatusStr.IndexOf(":") + 1;
            string LockID = LockStatusStr.Substring(IndexStart, LockStatusStr.IndexOf("-") - IndexStart);
            string StatusIDStr = LockStatusStr.Substring(LockStatusStr.IndexOf("-") + 1);
             int StatusID=0;
            if(StatusIDStr=="create")
            {
                 StatusID=1;
                 Lock MyLock = new Lock(LockID, StatusID);
                 LockManager MyLockManager = new LockManager();

                 if (MyLockManager.InsertLockEx(MyLock, int.Parse(ManagerIDStr)) != 0)
                 {
                     //登记云锁失败
                     ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[{0}]!", StatusIDStr);

                 }
                 else
                 {
                     //登记云锁成功
                     ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", StatusIDStr);

                 }


            }
            if(StatusIDStr=="sale")
            {
                 StatusID=2;
                 LockManager MyLockManager = new LockManager();// (CloudLockConnectString.ConnectionString);
                 string MyRegisterCodeStr = MyLockManager.UpdateLockForSale(LockID, int.Parse(ManagerIDStr));

                 if (MyRegisterCodeStr == null)
                 {
                     //出售云锁操作失败;
                     ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[{0}]!", StatusIDStr);

                 }
                 else
                 {
                    //出售云锁注册码操作成功!;
                     ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", MyRegisterCodeStr);//尾随注册码

                 }

            }
            if (StatusIDStr == "line")
            {
                StatusID = 3;
            }
            if (StatusIDStr == "none")
            {
                StatusID = 0;
            }

          
           
            //----2.Reply-----------------------------------------------------------------------------
            ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);


        }


        protected void CustomerCreateManagerment(string CustomerInforStr, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {
            string ReplyCommandMessageStr = "customercreate";
            //----1------------------------------------------------------------------------------------------------
            string ActionID = CustomerInforStr.Substring(0, CustomerInforStr.IndexOf(":"));
           
            if (ActionID == "authenlockid")
            {
                int IndexStart = CustomerInforStr.IndexOf(":") + 1;
                string LockID = CustomerInforStr.Substring(IndexStart, CustomerInforStr.IndexOf("-") - IndexStart);
                LockManager MyLockManager = new LockManager();
                Lock MyLock = MyLockManager.FindLock(LockID);
                if (MyLock == null)
                {
                    //MessageBox.Show("此锁ID号不存在！");
                    ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[110]!");
                   

                }
                if (MyLock.Status != 2)
                {

                    //MessageBox.Show("此锁ID号不没有出售或授权！");
                     ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[120]!");
                }
                else
                {

                    string TempStr = CustomerInforStr.Substring(CustomerInforStr.IndexOf("-") + 1);
                    string RegsisterCode = TempStr;//循环截取

                    RegisterCodeCRUD MyRegisterCodeCRUD = new RegisterCodeCRUD();
                    RegisterCode MyRegisterCode = MyRegisterCodeCRUD.FindRegisterCode(LockID);
                    if (MyRegisterCode == null)
                    {
                        //MessageBox.Show("此注册号码不存在！");
                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[130]!");


                    }
                    if (MyRegisterCode.RegisterCodeStr != RegsisterCode)
                    {
                        //MessageBox.Show("此注册号码有错误！");
                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[140]!");


                    }
                    else
                    {

                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[0]!");
                    }

                }

            }
          
            if (ActionID == "authenloginid")
            {
                int IndexStart = CustomerInforStr.IndexOf(":") + 1;
                string LockID = CustomerInforStr.Substring(IndexStart, CustomerInforStr.IndexOf("-") - IndexStart);
                string TempStr = CustomerInforStr.Substring(CustomerInforStr.IndexOf("-") + 1);
                string CustomerLoginID = TempStr;//循环截取
                CustomerManager MyCustomerManager = new CustomerManager();
                Customer MyCustomer = MyCustomerManager.FindCustomer(CustomerLoginID);

                if (MyCustomer != null)
                {
                    //已经注册;
                    ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[210]!");

                }
                else
                {
                    //还没有注册;
                    ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[0]!");

                }


            }


           if (ActionID == "createcustomer")
            {
                int IndexStart = CustomerInforStr.IndexOf(":") + 1;
                string LockID = CustomerInforStr.Substring(IndexStart, CustomerInforStr.IndexOf(",") - IndexStart);
              
                IndexStart = CustomerInforStr.IndexOf(",") + 1;               
                string TempStr = CustomerInforStr.Substring(IndexStart);
                string RegsisterCode = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string CustomerName = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string LoginName = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string PassWord = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string PersonID = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string TeleCode = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取

              
                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string EMail = TempStr.Substring(0, TempStr.IndexOf(",")); //循环截取


                IndexStart = TempStr.IndexOf(",") + 1;
                TempStr = TempStr.Substring(IndexStart);
                string Address = TempStr; //最后截取

                Channel MyChannel;
                MyChannel = new Channel();
                MyChannel.LockID = LockID;
                MyChannel.RegisterCodeStr = RegsisterCode;

                Customer MyNewCustomer = new Customer(); ;

                MyNewCustomer.CustomerName = CustomerName;
                MyNewCustomer.LoginName = LoginName;
                MyNewCustomer.PersonID = PersonID;
                MyNewCustomer.TeleCode = TeleCode;
                MyNewCustomer.Password = PassWord;
                MyNewCustomer.EMail = EMail;
                MyNewCustomer.Address = Address; 


                CustomerManager MyCustomerManager = new CustomerManager();

                string MobileID = MyCustomerManager.InsertCustomerExxx(MyNewCustomer, MyChannel);
                if (MobileID != null)
                {
                    Customer MyCustomer;
                    MyCustomer = MyCustomerManager.FindCustomerEx(MobileID);
                    if (MyCustomer != null)
                    {
                        // 注册操作成功,客户ID：" + MyCustomer.CustomerID + " ,云锁ID：" + MyChannel.LockID + ", 移动端ID：" + MobileID + "\r\n");
                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", MyCustomer.CustomerID + "," + MyChannel.LockID+"," +MobileID);

                    }
                    else
                    {
                        //注册操作失败！;
                        ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[310]!");

                    }

                }
                else
                {
                    //注册操作失败！;
                    ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#false[320]!");


                }

            }
            //----2.Reply-----------------------------------------------------------------------------
            ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);





        }

        
        protected void FromDBGetChannel(string CustomerIDStr, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {

            string ChannelIDStr;
            string ReplyCommandMessageStr = "getchannel";
            //----------------------------------------------------------------------------------------------------
            int CustomerID = int.Parse(CustomerIDStr);
            Channel MyChannel;
            ChannelManager MyChannelManager = new ChannelManager();
            MyChannel = MyChannelManager.FindChannel(CustomerID);
            if (MyChannel == null)
            {
                ChannelIDStr=null;
                ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
            }
            else
            {

                ChannelIDStr = MyChannel.LockID + "-" + MyChannel.MobileID;
                ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", ChannelIDStr);

            }
            //----------------------------------------------------------------------------------------------------

            ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);




        }


        protected void FromDBGetImage(string InLockSnapID, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel)
        {

            string LockID=InLockSnapID.Substring(0,15);
            string SnapID = InLockSnapID.Substring(16);
            string ReplyCommandMessageStr = "getimage#" + LockID+"";
            //----------------------------------------------------------------------------------------------------
            SnapManager MySnapManager = new SnapManager();
            byte[] MyImageBytes = MySnapManager.DownImageFromDB(long.Parse(SnapID),LockID);


            if (MyImageBytes == null)
            {
              
                ReplyCommandMessageStr = ReplyCommandMessageStr + "#false[10]!";
                ResponseToSynchClient(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr);
            }
            else
            {
                int ImageLenght = MyImageBytes.Length; 
                ReplyCommandMessageStr = string.Format(ReplyCommandMessageStr + "#true[{0}]!", ImageLenght);//N个字节头

                ResponseToSynchClientEx(InputSocketServiceReadWriteChannel, ReplyCommandMessageStr, MyImageBytes);
                this.DisplayResultInfor(1, string.Format("发送图像应答消息[{0}]", ImageLenght));


            }
            //----------------------------------------------------------------------------------------------------

           

        }


        protected void ResponseToSynchClient(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel,string ResultStr)
        {

            //string ReplyCommandMessageStr = "authtication";
            //ReplyCommandMessageStr = ReplyCommandMessageStr + "#true!";

            string ReplyCommandMessageStr = ResultStr;

            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

            int nBuffersLenght = MySendBaseMessageBytes.Length;
            byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];

            MySendMessageBytes[2] = (byte)nBuffersLenght;// 255;
            //填充
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }

            StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);


        }


        protected void ResponseToSynchClientEx(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, string ResultStr,byte[] ImageBuffer)
        {
            //专程发送图像
           string ReplyCommandMessageStr = ResultStr;

            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

            int nBuffersLenght = MySendBaseMessageBytes.Length;
            int FixSendMessageLenght = nBuffersLenght + ImageBuffer.Length + 3;
            byte[] MySendMessageBytes = new byte[FixSendMessageLenght];

            MySendMessageBytes[2] = (byte)nBuffersLenght;// 第三个字节为消息头长度（成功：37）;
            //填充消息头--之内
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }

            int ImageIndex = nBuffersLenght + 3;
            ImageBuffer.CopyTo(MySendMessageBytes, ImageIndex);

            StartAsynchSendMessage(InputSocketServiceReadWriteChannel, MySendMessageBytes);


        }

  
    
    
    }
}
