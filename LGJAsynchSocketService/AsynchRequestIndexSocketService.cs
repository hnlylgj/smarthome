using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService
{
     public class AsynchRequestIndexSocketService : AsynchSocketServiceBaseFrame
    {
       
          public AsynchRequestIndexSocketService():base()
         {
           
       
          }
          public AsynchRequestIndexSocketService(ManagerSocketLoginUser InManagerSocketLoginUser) : base(InManagerSocketLoginUser)
        {
           

        }

        protected void InitServiceParas()
        {
            MyTCPListerPort = 8920; MySocketServiceTypeID = SocketServiceTypeID.RequestIndexServer; 

            if (MyManagerSocketLoginUser == null) { MyManagerSocketLoginUser = new  ManagerLoginMobileUser(); }
            MyManagerSocketLoginUser.MyAsynchSendMessageCallback = new ManagerSocketLoginUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);
          
            //MyLGJSocketClientAPIBase = new LGJSocketClientAPILib.MobileServerClientAPI();
            //MyLGJSocketClientAPIBase.MyRemoteServerColseCallBack = new LGJSocketClientAPIBase.RemoteServerColseCallBack(this.SocketClientColseServerCallBack);
            //MyLGJSocketClientAPIBase.MyRecieveMessageCallback = new LGJSocketClientAPIBase.RecieveMessageCallback(this.SocketClientResponseCallBack);
            //MyLGJSocketClientAPIBase.ReturnSendMessageInvoke = new LGJSocketClientAPIBase.SendRecieveMessageCallback(this.SocketClientRequestCallBack);   
             
            
        }

         public override void CommandDefineDispatch(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] MyReadBuffers, int RecieveMessageFlag, int InputRecieveCount)
        {
            ;
        }
        protected override void SendMessageForString(Byte[] InSendMessageBuffers, SocketServiceReadWriteChannel InputReadWriteChannel)
        {

            ;


        }
    }
}
