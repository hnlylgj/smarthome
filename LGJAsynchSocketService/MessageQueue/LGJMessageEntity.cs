using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.MessageQueue
{
    public class LGJMessageEntity : ILGJMessageEntity
    {
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
        }

        private int resultFlag;
        public int ResultFlag
        {
            get
            {
                return resultFlag;
            }
        }
         
        private SocketServiceReadWriteChannel readWriteSocketChannel;
        public SocketServiceReadWriteChannel ReadWriteSocketChannel
        {
            get
            {
                return readWriteSocketChannel;
            }
        }

        private string attachmentInfor;
        public string AttachmentInfor
        {
            get
            {
                return attachmentInfor;
            }
        }


        private byte[] myByteBuffer;

        public byte[] MyByteBuffer
        {
            get
            {
                return myByteBuffer;
            }

        }

        private int myRecieveAvailable;
        public int MyRecieveAvailable
        {
            get
            {
                return myRecieveAvailable;
            }

        }
        private int myRecieveCount;
        public int MyRecieveCount
        {
            get
            {
                return myRecieveCount;
            }

        }

        public LGJMessageEntity(string InTitle, int ResultFlag, SocketServiceReadWriteChannel InReadWriteSocketChannel)
        {
            this.title = InTitle;
            this.resultFlag = ResultFlag;
            this.readWriteSocketChannel = InReadWriteSocketChannel;
            
        }
        public LGJMessageEntity(string InTitle, int ResultFlag, SocketServiceReadWriteChannel InReadWriteSocketChannel, string InAttachmentInfor)
        {
            this.title = InTitle;
            this.resultFlag = ResultFlag;
            this.readWriteSocketChannel = InReadWriteSocketChannel;
            this.attachmentInfor = InAttachmentInfor;
        }
        public LGJMessageEntity(string InTitle, int MeRecieveAvailable, SocketServiceReadWriteChannel InReadWriteSocketChannel,int MeRecieveCount, byte[] MeByteBuffer)
        {
            this.title = InTitle;
            this.myRecieveAvailable = MeRecieveAvailable;
            this.readWriteSocketChannel = InReadWriteSocketChannel;
            this.myByteBuffer = MeByteBuffer;
            this.myRecieveCount = MeRecieveCount;
        }
    }
}
