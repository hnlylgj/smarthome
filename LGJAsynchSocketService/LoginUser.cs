using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace LGJAsynchSocketService
{

    [Serializable()]
    public class LoginUser
    {
        #region ------数据成员-------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        private int _LoginID;
        private string _SocketInfor;
        private DateTime _LoginTime;
        private DateTime _KeepTime;
 
        private string _LockID;
        private string _MobileID;
        private int _ChannelStatus; //0:正常 1：未认证 2：3：；

        public SocketServiceReadWriteChannel MyReadWriteSocketChannel;
        public uint WorkStatus;//0:命令,大于0:文件流分块读取累加
        public byte[] MyByteBuffer;

        public bool IsCloseSocket;
             
        //---------------------------------------------------------------------------------------
        //private IPAddress _ClientIP;
        //private IPEndPoint _RemoteEndIP;
        //private int _ViewManner;
       //--------------------------------------------------------------------------------------     
        //public TcpClient MyTCPClient;
        public uint WorkCountSum;
        public int SnapTypeID;
        public long SnapID;
        public uint FileReadMode;//0：命令，其他为文件流分块读取模式：1：后续分块不包括消息头，2;后续分块包括消息头
        public uint LoopReadCount;//循环计数
        public string TempString;
        public string LoginUserPlatform;
        //------------------------------------------
        public int ReplyChannelLoginID;


      #endregion ------------------------------------------------------------------------------


        #region ------智能字段-------------------------------------------------------------------


        public int LoginID
        {
            get
            {
                return _LoginID;
            }
            set
            {
                _LoginID = value;
            }
        }


        public string GetClientIP
        {
            get
            {
                return null;// return _ClientIP.ToString();
            }
           
        }

        /*

        public IPAddress SetClientIP
        {
          
            set
            {
                _ClientIP = value;
            }
        }


        public string GetRemoteEndIP
        {
            get
            {
                return _RemoteEndIP.ToString();
            }

        }

        public IPEndPoint SetRemoteEndIP
        {

            set
            {
                _RemoteEndIP = value;
            }
        }


        public IPEndPoint RemoteEndIP
        {


            get
            {
                return _RemoteEndIP;
            }
            
            set
            {
                _RemoteEndIP = value;
            }
        }
        */

        public string GetLoginTime
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _LoginTime);
            }
          
        }

        public DateTime SetLoginTime
        {
            
            set
            {
                _LoginTime = value;
            }
        }


        public DateTime LoginTime
        {
            get
            {
                return _LoginTime;
            }
            set
            {
                _LoginTime = value;
            }
        }

        public string GetKeepTime
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _KeepTime); 
            }
            
        }

        public DateTime SetKeepTime
        {            
            set
            {
                _KeepTime = value;
            }
        }

        public DateTime KeepTime
        {

            get
            {
                return _KeepTime;
            }
            set
            {
                _KeepTime = value;
            }
        }

        public string LockID
        {
            get
            {
                return _LockID;
            }
            set
            {
                _LockID = value;
            }
        }

        public string MobileID
        {
            get
            {
                return _MobileID;
            }
            set
            {
                _MobileID = value;
            }
        }

        public string SocketInfor
        {
            get
            {
                return _SocketInfor;
            }
            set
            {
                _SocketInfor = value;
            }
        }
        public int ChannelStatus
        {
            get
            {
                return _ChannelStatus;
            }
            set
            {
                _ChannelStatus = value;
            }
        }

       

        #endregion --------------------------------------------------------------


        #region ------构造函数-----------------------------------------------------------

        public LoginUser()
        {
            this.LoginID=1;
            //this.SetClientIP = null;
            //this.SetRemoteEndIP = null;// new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], 15888);
            this.SetLoginTime = DateTime.Now;
            this.SetKeepTime = DateTime.Now;
            this.WorkStatus = 0;
            this.ChannelStatus = 1;//默认为：1
            //this.ViewManner=0;//默认为：0
            //MyLongFileReceiveProc = new LockServerLib.LongFileReceiveProc(); 
         
          
        }

        public LoginUser(int  InLoginID,ref SocketServiceReadWriteChannel MeSocketServiceReadWriteChannel)
        {
            this.LoginID = InLoginID;
            this.MyReadWriteSocketChannel = MeSocketServiceReadWriteChannel;           
            this.SetLoginTime = DateTime.Now;
            this.SetKeepTime = DateTime.Now;
            this.SocketInfor = MeSocketServiceReadWriteChannel.MyRemoteEndPointStr;//.Client.RemoteEndPoint.ToString();
            this.WorkStatus = 0;//默认为：0
            this.ChannelStatus = 1;//默认为：1
           //this.ViewManner = 0;
            //MyLongFileReceiveProc = new LockServerLib.LongFileReceiveProc(); 

        }        
        #endregion ----------------------------------------------------------------------

        public void ByteBufferCopy(int MeRecieveCount)
        {

            this.MyByteBuffer = new byte[MeRecieveCount];

            //填充
            for (int i = 0; i < MeRecieveCount; i++)
            {

                this.MyByteBuffer[i] = this.MyReadWriteSocketChannel.MyReadBuffers[i];

            }

        }
        public void FileByteBuffer(int MeCount)
        {

            this.MyByteBuffer = new byte[MeCount];
            

        }
        public void ClearSet()
        {
           this.WorkStatus = 0; 
           this.WorkCountSum=0;;
           this.SnapTypeID = 0;
           this.SnapID = 0;
           this.FileReadMode = 0;
           this.LoopReadCount = 0;
           this.MyByteBuffer = null;


        }
    }
}
