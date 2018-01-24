using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.MobileAppServerLib
{
    public class SynchTcpClientChannel
    {
         LoginUser MyLoginUser;
         //AsynchMobileAppSocketService MyAsynchMobileAppSocketService;
         AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockID;
         string MyMobileID;
         string MyCommandMessage;


         public SynchTcpClientChannel(AsynchSocketServiceBaseFrame InAsynchSocketServiceBaseFrame, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchSocketServiceBaseFrame = InAsynchSocketServiceBaseFrame;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }

         public SynchTcpClientChannel(AsynchSocketServiceBaseFrame InAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
        {
            //this.MyAsynchSocketServiceBaseFrame = InputAsynchSocketServiceBaseFrame;
            this.MyAsynchSocketServiceBaseFrame = InAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteObject;
            this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            this.MyRecieveCount = InputRecieveCount;
    
         }

         public void CompleteCommand()
         {            
            
              //string BaseMessageString = Encoding.ASCII.GetString(MyReadBuffers, 3, MyRecieveCount -3);
              string BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, MyRecieveCount - 3);
              MyCommandMessage = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));

              MyLockID =BaseMessageString.Substring(BaseMessageString.IndexOf("#")+1,15);
              MyMobileID = BaseMessageString.Substring(BaseMessageString.IndexOf("-")+1, 15);

              MyLoginUser.LockID = MyLockID;
              MyLoginUser.MobileID = MyMobileID;

              string MyTimeMarkerStr = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
              MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "接收移动端[{0}]请求命令：{1}", MyMobileID, MyCommandMessage));
              MyAsynchSocketServiceBaseFrame.DisplayResultInfor(4, "");
             
     //int  ReturnCode=MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.MobileUserrLoginProc(ref MyReadWriteChannel.MyTCPClient, MyLockID, MyMobileID); //更新路由注册表
             


     // if (ReturnCode < 0)
            // {
            //     FailTotResponseMobile();
            //     return;
            // }
             //else
             //{
                //------转发给云锁服务器！
                 if (MyAsynchSocketServiceBaseFrame.MyLGJSocketClientAPIBase.ConnectID == 0)//如果连接到云锁服务器
                {
                    byte[] MySendMessageBytes = new byte[MyRecieveCount];
                    for (int i = 0; i < MyRecieveCount; i++)
                    {

                        MySendMessageBytes[i] = MyReadBuffers[i];

                    }

                    MyAsynchSocketServiceBaseFrame.MyLGJSocketClientAPIBase.SynchSendCommand(MySendMessageBytes);
                }
                else//--如果没有来接到云锁服务器
                {
                    FailTotResponseMobile();
                    MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, "转发给云锁服务器通道没有打开");
                    //---原路打回----------------------------------------------------------------------------------
                    /*
                    string ReplyCommandMessageStr = CommandMessageStr;
                    ReplyCommandMessageStr = ReplyCommandMessageStr + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + "false" + "!";
                    byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

                    int nBuffersLenght = MySendBaseMessageBytes.Length;
                    MySendMessageBytes = new byte[nBuffersLenght + 3];


                    MySendMessageBytes[2] = 255;

                    //--填充---------
                    for (int i = 0; i < nBuffersLenght; i++)
                    {

                        MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

                    }

                    MyAsynchMobileAppSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);
                    MyAsynchMobileAppSocketService.DisplayResultInfor(1, "转发给云锁服务器失败");
                    */

                }
            
         

           
         }

         protected void FailTotResponseMobile()
         {

             //---原路快速打回----------------------------------------------------------------------------------
             string ReplyCommandMessageStr=null;
             ReplyCommandMessageStr = MyCommandMessage + "#" + MyMobileID + "-" + MyLockID + "#" + "false[12]" + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);
             int nBuffersLenght = MySendBaseMessageBytes.Length;
             byte[]  MySendMessageBytes = new byte[nBuffersLenght + 3];
             MySendMessageBytes[2] = 255;

             //--填充-----------------------------------------------
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);
             MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, "请求转发给云锁服务器查找通道失败");

         }



    }
}
