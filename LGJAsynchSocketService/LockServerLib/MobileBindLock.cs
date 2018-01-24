using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
     public class MobileBindLockParser
    {

         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount = 0;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;


          public MobileBindLockParser(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
             //最新版本
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
            //this.ReplyChannelLoginID = MeLoginUser.LoginID;
        }
         public MobileBindLockParser(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InRecieveCount)
        {
             //老版本
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteObject;
            this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            this.MyRecieveCount = InRecieveCount;
        }



         public void CompleteCommand()
         {

              //2.摘要字符串-----------------------------------------------------------------------------------------------------------------------
             string BaseMessageString = Encoding.UTF8.GetString(this.MyReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
           
             MyLockIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
             MyMobileIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);

             //int ReturnCodeID = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForLoginMobileEx(MyLockIDStr, MyMobileIDStr); //找注册表通道
             int KeepReplyChannelLoginID = MyLoginUser.LoginID;

             FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr);
             MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));
              
             string ResultID;
             if (MyLoginUser!=null) 
             {
                 //--找智能锁通道-------------------------------------------------------------------------------------------------------
                  //---1.-------------------------------------------------------------------------------------
                 //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);
                 //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[ReturnCodeID].MyReadWriteSocketChannel;
                 //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel,null); 

                 ResultID = "true[closelock-中]";//“中”：为了测试中文传输情况
                 MyLoginUser.ReplyChannelLoginID = KeepReplyChannelLoginID;//记住返回通道
                 this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;//发给锁端
                 DriveToSmartLock();
           
             }
             else
             {
                 ResultID = "false[11]";//11：表示未能找到智能锁
                
             }
             ReplyMobileBindLockParserMessage(MyMobileIDStr, MyLockIDStr, ResultID);//原路快速发送响应消息

            
         }

         private void ReplyMobileBindLockParserMessage(string MessageMobileStr , string   MessageLockStr , string ResultStr)
         {

             //------------------------------------------------------------------------------------
             string ReplyCommandMessageStr = "login";
             ReplyCommandMessageStr = ReplyCommandMessageStr + "#" + MessageMobileStr + "-" + MessageLockStr + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

             int nBuffersLenght = MySendBaseMessageBytes.Length;// ReplyCommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];
                            

             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes); 
           
         }

         private void DriveToSmartLock()
         {
             int MySendByteCount = 23;//22+1字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0350";
             }
             else
             {
                 HexSendMessageIDString = "5003";
             }

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


             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }



    }
}
