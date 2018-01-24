using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBusServiceLib;
//using SmartLockGridViewLib;
namespace LGJAsynchSocketService.LockServerLib
{
     public class ClearLockKey
    {
         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;

         public ClearLockKey(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }

         public ClearLockKey(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteChannel, int InRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteChannel;
            this.MyReadBuffers = InputReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = InRecieveCount;
        }
         
         public void RequestToLock()
         {
             //1.解析来自移动端的消息-----------------------------------------------------------------------------------------------------------------------
             string BaseMessageString = Encoding.UTF8.GetString(this.MyReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
             MyLockIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
             MyMobileIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);      
             //----2.驱动智能锁----------------------------   
                //--找智能锁通道--------------------------------------------------------------------------------
             FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr); //测试正确
             MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));

             this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);
             //--再转发出去-------------
             if (MyLoginUser != null)
             {
                 this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
                 DriveToSmartLock();
             }
             else
             {
                 //--否则原路或者走响应通道快速返回[0、1]
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "没有找到锁错误！");
                 NotFindLockResponseToMobile();

             }

           }

         public void ResponseToMobile()
         {

             string CommandMessageID = "clearlockkey";
             //---1.找智能锁本身通道的路由表记录-------------------------------------
             //LoginUser MyLoginUser = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.FindLoginUserList(ref  this.MyReadWriteChannel.MyTCPClient);  
             //FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
            // LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
             
             
             MyLockIDStr = MyLoginUser.LockID;
             MyMobileIDStr = MyLoginUser.MobileID;

             Byte RecieveMessageResult = MyReadBuffers[22];//结果标志

             string ResultStr;
             if (RecieveMessageResult == 0)
             {
                 SynchDBaseLockKey();
                 ResultStr = "true";
             }
             else
             {
                 ResultStr = "false[10]";
             }

             string CommandMessageStr;
             CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             //----1.保存数据库备查----------//---------------------------------------------------------------------------------------------------
             //MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);
             try
             {

                 this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;

                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误！");
             }

         }

         private void NotFindLockResponseToMobile()
         {
             string CommandMessageID = "clearlockkey";
             string ResultStr;
             ResultStr = "false[11]";
             string CommandMessageStr;
             CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             //----1.保存数据库备查-------------------------------------------------------------------------------------------------------------
             MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);
            try
             {

                 MyAsynchLockServerSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误！");
             }

         }

         private void SynchDBaseLockKey()
         {
             
              //LockKeyManager MyLockKeyManager = new LockKeyManager();
             //MyLockKeyManager.ClearLockKey(MyLockIDStr);

             LockKeyManager MyLockKeyManager = new LockKeyManager();
             MyLockKeyManager.ClearLockKey(MyLockIDStr);


         }

         private string GetDateTimeWeekIndex()
         {
             //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
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

         private void DriveToSmartLock()
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 39;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0D20";
             }
             else
             {
                 HexSendMessageIDString = "200D";
             }

             string MyDateTimeString = GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(MyDateTimeString);

             //填充缓冲区信息头
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

             MySendMessageBytes[2] = 2; //移动端请求

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


             //--填充日期字节----
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[22 + i] = MyDateTimeBytes[i];

             }

             MySendMessageBytes[37] = 1;//删除用户信息
             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }
    }
}
