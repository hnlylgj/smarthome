using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.MobileAppServerLib
{
    public class MobileLoginManager
    {
         LoginUser MyLoginUser;
         AsynchMobileAppSocketService MyAsynchMobileAppSocketService;
         //AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         //SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockID;
         string MyMobileID;

         public MobileLoginManager(AsynchMobileAppSocketService InAsynchMobileAppSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            //新版本
            this.MyAsynchMobileAppSocketService = InAsynchMobileAppSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public MobileLoginManager(AsynchSocketServiceBaseFrame InAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
        {
            //老版本
            //this.MyAsynchSocketServiceBaseFrame = InputAsynchSocketServiceBaseFrame;
            //this.MyAsynchSocketServiceBaseFrame = InAsynchSocketServiceBaseFrame;
            //this.MyReadWriteChannel = InputReadWriteObject;
            //this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            //this.MyRecieveCount = InputRecieveCount;
        }

         public void CompleteCommand()
         {            
             //================================================================================================================================
              //string BaseMessageString = Encoding.ASCII.GetString(MyReadBuffers, 3, MyRecieveCount -3);
              string BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, MyRecieveCount - 3);
              MyLockID =BaseMessageString.Substring(BaseMessageString.IndexOf("#")+1,15);
              MyMobileID = BaseMessageString.Substring(BaseMessageString.IndexOf("-")+1, 15); ;

            // string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            // string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);

          //MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册移动端：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyMobileIDStr));
            //MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.CRUDLoginUserListForLogin(ref MyReadWriteChannel.MyTCPClient, MyLockIDStr, MyMobileIDStr); //更新路由注册表


          //MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册移动端：{1}", MyReadWriteChannel.MyRemoteEndPointStr, MyMobileID));

            //MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.CRUDLoginUserListForLogin(ref MyReadWriteChannel.MyTCPClient, MyLockID, MyMobileID); //更新路由注册表
            //int ReturnCode = MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.MobileUserrLoginProc(ref MyReadWriteChannel.MyTCPClient, MyLockID, MyMobileID); //更新路由注册表
           
            //注册信息
           MyLoginUser.LockID = MyLockID;
           MyLoginUser.MobileID = MyMobileID;


             //if (ReturnCode < 0)
             //{
              //   FailTotResponseMobile();
               //  return;
            // }
            // else
            // {


             //------转发给云锁服务器！
             //if (MyAsynchSocketServiceBaseFrame.MyLGJSocketClientAPIBase.ConnectID == 0)//如果连接到云锁服务器
             if (MyAsynchMobileAppSocketService.MyMobileServerClientAPI.ConnectID == 0)//如果连接到云锁服务器
                {
                byte[] MySendMessageBytes = new byte[MyRecieveCount];
                for (int i = 0; i < MyRecieveCount; i++)
                {

                    MySendMessageBytes[i] = MyReadBuffers[i];

                }

                //MyAsynchSocketServiceBaseFrame.MyLGJSocketClientAPIBase.SynchSendCommand(MySendMessageBytes);
                MyAsynchMobileAppSocketService.MyMobileServerClientAPI.SynchSendCommand(MySendMessageBytes);

            }
            else//--如果没有来接到云锁服务器
            {
                FailTotResponseMobile();
                //MyAsynchSocketServiceBaseFrame.DisplayResultInfor(1, "转发给云锁服务器通道没有打开");
                MyAsynchMobileAppSocketService.DisplayResultInfor(1, "转发给云锁服务器通道没有打开");



            }
             //}
             
             /*
             byte[] MySendMessageBytes = new byte[MyRecieveCount];

              for (int i = 0; i < MyRecieveCount; i++)
              {

                  MySendMessageBytes[i] = MyReadBuffers[i];

              }
              if (MyAsynchMobileAppSocketService.MyMobileServerClientAPI.ConnectID == 0)//如果连接到云锁服务器
              {
                  
                  
                  MyAsynchMobileAppSocketService.MyMobileServerClientAPI.SynchSendCommand(MySendMessageBytes);
              }
              else//----如果没有来接到云锁服务器
              {
                  //---原路打回--
                  //------------------------------------------------------------------------------------
                  string ReplyCommandMessageStr = "login";
                  ReplyCommandMessageStr = ReplyCommandMessageStr + "#" + MyMobileID + "-" + MyLockID + "#" + "false" + "!";
                  byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);

                  int nBuffersLenght = MySendBaseMessageBytes.Length;// ReplyCommandMessageStr.Length;
                   MySendMessageBytes = new byte[nBuffersLenght + 3];


                  MySendMessageBytes[2] = 255;

                  //填充
                  for (int i = 0; i < nBuffersLenght; i++)
                  {

                      MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

                  }

                  MyAsynchMobileAppSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes); 
                  MyAsynchMobileAppSocketService.DisplayResultInfor(1, "转发给云锁服务器失败");


              }
         */

           
         }
        
         protected void FailTotResponseMobile()
         {

             //---原路快速打回----------------------------------------------------------------------------------
             string ReplyCommandMessageStr = null;
             ReplyCommandMessageStr = "login" + "#" + MyMobileID + "-" + MyLockID + "#" + "false[12]" + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(ReplyCommandMessageStr);
             int nBuffersLenght = MySendBaseMessageBytes.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];
             MySendMessageBytes[2] = 255;

             //--填充-----------------------------------------------
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

            MyAsynchMobileAppSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);
            MyAsynchMobileAppSocketService.DisplayResultInfor(1, "转发给云锁服务器查找通道失败");

            

         }

      

    }
}
