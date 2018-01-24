using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService
{
    public class AsynchResponseIndexSocketService : AsynchSocketServiceBaseFrame
    {

        public AsynchResponseIndexSocketService()
            : base()
         {
                  
        
       
          }
          public AsynchResponseIndexSocketService(ManagerSocketLoginUser InManagerSocketLoginUser) : base(InManagerSocketLoginUser)
        {
           
            

        }
         protected  void InitServiceParas()
        {
            MyTCPListerPort = 8930; MySocketServiceTypeID = SocketServiceTypeID.ResponseIndexServer;  
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
