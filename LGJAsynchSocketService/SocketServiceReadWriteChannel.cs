//--------SocketServiceReadWriteChannel.cs---------//
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace LGJAsynchSocketService
{
    public class SocketServiceReadWriteChannel
    {
        public TcpClient MyTCPClient;
        public string MyRemoteEndPointStr;
        public NetworkStream MyNetWorkStream;
        public byte[] MyReadBuffers;
        public byte[] MyWriteBuffers;
        public string TempString;
        public int TempNumber;
        public bool IsCloseSocket;
        public SocketServiceReadWriteChannel(TcpClient NewTCPClient)
        {
            this.MyTCPClient = NewTCPClient;
            this.MyRemoteEndPointStr = MyTCPClient.Client.RemoteEndPoint.ToString();   
            MyNetWorkStream = MyTCPClient.GetStream();
            MyReadBuffers = new byte[MyTCPClient.ReceiveBufferSize];
            MyWriteBuffers = new byte[MyTCPClient.SendBufferSize];
            IsCloseSocket = false;
        }
        public void InitReadArray()
        {
            MyReadBuffers = new byte[MyTCPClient.ReceiveBufferSize];
        }
        public void InitWriteArray()
        {
            MyWriteBuffers = new byte[MyTCPClient.SendBufferSize];
        }
    }
}
