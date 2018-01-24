using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
      public class PingCommandParser
    {
        //AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         LoginUser MyLoginUser;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;
         Byte  MessagePowerID;

         public PingCommandParser(AsynchLockServerSocketService InAsynchLockServerSocketService,LoginUser MeLoginUser, int MeRecieveCount)
        {
            //最新版本
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }

        public PingCommandParser(AsynchLockServerSocketService InAsynchLockServerSocketService, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
         {
             this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
             this.MyReadWriteChannel = InputReadWriteObject;
             this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
             this.MyRecieveCount = InputRecieveCount;
         }

         public void CompleteCommandEx()
         {
              //最新版本
             if (MyLoginUser.LockID == null || MyLoginUser.MobileID == null)
             {               
                 ResponseMessageToLock(0xFF, 0);//发送失败响应消息
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "没有正常注册通道错误！[1003]");
                 return;

             }
             
             MyLockIDStr = MyLoginUser.LockID;
             MyMobileIDStr = MyLoginUser.MobileID;

             MessagePowerID = MyReadBuffers[40];
             if (MessagePowerID == 1) CreateForwardMessageToMobile();                  

             MyLoginUser.KeepTime = DateTime.Now;//更新通道活动时间

             ResponseMessageToLock(0, 0);//发送成功响应消息
             MyAsynchLockServerSocketService.DisplayResultInfor(4, "");
             //return;

        }
         public void CompleteCommand()
         {
             //---1.找智能锁本身通道的路由表记录-------------------------------------
             LockServerLib.FindLockChannel MyBindedLockChannel = new LockServerLib.FindLockChannel(this.MyReadWriteChannel);
             LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
             if (MyLoginUser.LockID == null || MyLoginUser.MobileID == null)
             {
                            
                 //MyAsynchLockServerSocketService.MyManagerLoginLockUser.RemoveNotLoginUser(ref MyReadWriteChannel.MyTCPClient);
                 ResponseMessageToLock(0xFF, 0);//发送响应消息
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "没有正常注册通道错误！[1003]");
                 return;

             }
             
             
             MyLockIDStr = MyLoginUser.LockID;
             MyMobileIDStr = MyLoginUser.MobileID;


     
               //AsynchSocketServiceBaseFrame.FileReceiveFlag++;
             //
             //
             //System.Diagnostics.Debug.WriteLine(AsynchSocketServiceBaseFrame.FileReceiveFlag);
             //
             //System.Diagnostics.Debug.WriteLine(AsynchSocketServiceBaseFrame.MyLockIDStr);
             //System.Diagnostics.Debug.WriteLine(AsynchSocketServiceBaseFrame.MyMobileIDStr);

             MessagePowerID = MyReadBuffers[40];
             if (MessagePowerID == 1) CreateForwardMessageToMobile();

             //1.缓冲区按原样全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------
            /*
             string HexRecieveMessageString = "";
             for (int i = 0; i < MyRecieveCount; i++)
             {

                 HexRecieveMessageString = HexRecieveMessageString + string.Format("{0:X2}", MyReadBuffers[i]);

             }

             //2.摘要字符串显示-----------------------------------------------------------------------------------------------------------------------
             string RecieveMessageLenghtString = "";
             RecieveMessageLenghtString = string.Format("{0:X2}", MyReadBuffers[0]);
             if (MyAsynchSocketServiceBaseFrame.DataFormateFlag)
             {
                 RecieveMessageLenghtString = string.Format("{0:X2}", MyReadBuffers[1] + RecieveMessageLenghtString);
             }
             else
             {
                 RecieveMessageLenghtString = RecieveMessageLenghtString + string.Format("{0:X2}", MyReadBuffers[1]);
             }
             //RecieveMessageLenghtString = RecieveMessageLenghtString + string.Format("{0:X2}", MyReadBuffers[1]);
             UInt32 RecieveMessageLenght = Convert.ToUInt32(RecieveMessageLenghtString, 16);

             Byte RecieveMessageFlag = MyReadBuffers[2];


             string HexRecieveMessageIDString ;
             if (MyAsynchSocketServiceBaseFrame.DataFormateFlag)
             {
                 HexRecieveMessageIDString = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
             }
             else
             {
                 HexRecieveMessageIDString= string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
             }
             */
             //==============================================================================================
               //string MyLockIDName = Encoding.ASCII.GetString(MyReadBuffers, 22, 15);      // "9876ABCD1234AA9";
             //string BaseInforString = RecieveMessageLenght.ToString() + "--" + RecieveMessageFlag.ToString() + "--" + HexRecieveMessageIDString + "--" + MyLockIDName + "--";
            // string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
             //MyAsynchSocketServiceBaseFrame.DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint, HexRecieveMessageString + "-->" + BaseInforString));
            //--------------------------------       

           MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserList(ref MyReadWriteChannel.MyTCPClient, 1); //更新路由注册表通道活动时间

           ResponseMessageToLock(0, 0);//发送响应消息

             return ;
         }

         private void CreateForwardMessageToMobile()
         {

             string CommandMessageStr = "lowpower";
             CommandMessageStr = CommandMessageStr + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#[" + GetDateTimeWeekIndex() + "]!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 250;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

            

             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[作为客户端]--------------------------------------------------------------------------------
             try
             {
                  this.NewReadWriteChannel = this.MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                  this.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes); 

             }
             catch
             {
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }



         }

         private void ResponseMessageToLock(Byte ProcessID, Byte DataSynchID)
         {
                         
             //----无校验字节--------------------------------------------------
             int MySendByteCount = 25;//加校验字节
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0410";
             }
             else
             {
                HexSendMessageIDString = "1004";
             }
           

       

             //填充字节信息头
             if (MyAsynchLockServerSocketService.DataFormateFlag)
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

             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[10] = 1;
                 MySendMessageBytes[12] = 1;

             }
             else
             {
                 MySendMessageBytes[11] = 1;
                 MySendMessageBytes[13] = 1;
             }

             //填充命令处理结果
         
             MySendMessageBytes[22] = ProcessID;// 0;
             MySendMessageBytes[23] = DataSynchID;//1;



             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes); 
           
         }

         private string GetDateTimeWeekIndex()
         {
             //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
             //string Teststr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
             string DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
             string WeekIndexStr = DateTime.Now.ToString("dddd");
             string nReturn = null;
             switch (WeekIndexStr)
             {
                 case "星期一": nReturn = "1";
                     break;

                 case "星期二": nReturn = "2";
                     break;

                 case "星期三": nReturn = "3";
                     break;

                 case "星期四": nReturn = "4";
                     break;

                 case "星期五": nReturn = "5";
                     break;

                 case "星期六": nReturn = "6";
                     break;

                 case "星期日": nReturn = "7";
                     break;

                 default:
                     nReturn = "0";
                     break;

             }


             return DateTimeStr + nReturn;
         }
    }
}
