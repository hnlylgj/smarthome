using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.MessageQueue
{
    public interface ILGJMessageEntity
    {
        
        string Title
        {
            get;
        }
        int ResultFlag
        {
            get;
        }
        SocketServiceReadWriteChannel ReadWriteSocketChannel
        {
            get;
        }

        string AttachmentInfor
        {
            get;
        }

       int MyRecieveAvailable
        {
            get;
        }
       byte[] MyByteBuffer
       {

           get;
       }

       int MyRecieveCount
       {
           get;

       }

    }
}
