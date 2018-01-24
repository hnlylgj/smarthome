using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
     public class QueryTimeParser
    {
         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;

         public QueryTimeParser(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public QueryTimeParser(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteObject;
            this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            this.MyRecieveCount = InputRecieveCount;
        }
         public void CompleteCommand()
         {
                         
             ResponseMessageToLock();//发送成功响应消息

           
         }
                  
         private void ResponseMessageToLock()
         {
                 
             int MySendByteCount = 38;//加校验字节
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0C30";
             }
             else
             {
                HexSendMessageIDString = "300C";
             }
           
             //--填充字节信息头-----------------------------------------------
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

             MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);//默认小端格式
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

             //--填充日期----
             string MyDateTimeString = GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.ASCII.GetBytes(MyDateTimeString);
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[22 + i] = MyDateTimeBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes); 
           
         }

         private string GetDateTimeWeekIndex()
         {
            
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
