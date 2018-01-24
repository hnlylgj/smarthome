using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LGJAsyncSocketClientLib
{
    public class ReadWriteSocketChannel
    {
        public NetworkStream netStream;
        public byte[] RecieveMessageBytes;
        public ReadWriteSocketChannel(NetworkStream netStream, int bufferSize)
        {
            this.netStream = netStream;
            RecieveMessageBytes = new byte[bufferSize];
        }
    }
}
