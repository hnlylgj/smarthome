using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
    public class OPenDoorMessageProcess
    {

         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         //SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount = 55;
         byte[] MyReadBuffers;
         string MessageLockStr;
         string MessageMobileStr;
         string MessageKeyIDStr;

         public OPenDoorMessageProcess(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }

         public OPenDoorMessageProcess(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteObject;
            this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            this.MyRecieveCount = InputRecieveCount;
        }

         public void CompleteCommand()
         {
            
             //---1.找智能锁本身通道的路由表记录-------------------------------------
             //LockServerLib.FindLockChannel MyBindedLockChannel = new LockServerLib.FindLockChannel(this.MyReadWriteChannel);
            // LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
            
             MessageLockStr = MyLoginUser.LockID;
             MessageMobileStr = MyLoginUser.MobileID;
             MessageKeyIDStr = string.Format("{0:D2}", MyReadBuffers[22]);

            
             CreaterForwardMessageToMobile();
             ReplyLoginMessageToLock(0);
                         
         }

         private void CreaterForwardMessageToMobile()
         {

             string CommandMessageStr = "opendoor";
             CommandMessageStr = CommandMessageStr + "#" + MessageMobileStr + "-" + MessageLockStr + "#[" + MessageKeyIDStr+ ","+ GetDateTimeWeekIndex()+"]!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 250;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }
              
           
             try
             {

                SocketServiceReadWriteChannel NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                /*
                 int ReplyChannelIndex = MyLoginUser.ReplyChannelLoginID - 1;
                 if (ReplyChannelIndex < 2)
                 {
                     SocketServiceReadWriteChannel NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[ReplyChannelIndex].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                 }
                 else
                 {
                     //通道错误；
                     MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应再转发移动端寻找通道错误：" + ReplyChannelIndex.ToString());
                 }
                 */

                /*
                 SocketServiceReadWriteChannel NewReadWriteChannel;
                 if (MyLoginUser.ReplyChannelLoginID < 1)
                 {
                     //进行广播！
                     NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                     NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                 }
                 else
                 {

                     int ReplyChannelIndex = MyLoginUser.ReplyChannelLoginID - 1;
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
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应再转发转发移动端通道错误[100]");
             }

         }

       
        
         private void ReplyLoginMessageToLock(Byte ProcessID)
         {
             
             //------校验字节------------------------------------------------
             int MySendByteCount = 24;//加校验字节
            
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0204";
             }
             else
             {
                HexSendMessageIDString = "0402";
             }
           

             string MyDateTimeString = GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.ASCII.GetBytes(MyDateTimeString);

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
         //---------------------------------------
                 
      
    }
}
