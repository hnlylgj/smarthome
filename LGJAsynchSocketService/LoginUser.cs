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
        #region ------���ݳ�Ա-------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        private int _LoginID;
        private string _SocketInfor;
        private DateTime _LoginTime;
        private DateTime _KeepTime;
 
        private string _LockID;
        private string _MobileID;
        private int _ChannelStatus; //0:���� 1��δ��֤ 2��3����

        public SocketServiceReadWriteChannel MyReadWriteSocketChannel;
        public uint WorkStatus;//0:����,����0:�ļ����ֿ��ȡ�ۼ�
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
        public uint FileReadMode;//0���������Ϊ�ļ����ֿ��ȡģʽ��1�������ֿ鲻������Ϣͷ��2;�����ֿ������Ϣͷ
        public uint LoopReadCount;//ѭ������
        public string TempString;
        public string LoginUserPlatform;
        //------------------------------------------
        public int ReplyChannelLoginID;


      #endregion ------------------------------------------------------------------------------


        #region ------�����ֶ�-------------------------------------------------------------------


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


        #region ------���캯��-----------------------------------------------------------

        public LoginUser()
        {
            this.LoginID=1;
            //this.SetClientIP = null;
            //this.SetRemoteEndIP = null;// new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], 15888);
            this.SetLoginTime = DateTime.Now;
            this.SetKeepTime = DateTime.Now;
            this.WorkStatus = 0;
            this.ChannelStatus = 1;//Ĭ��Ϊ��1
            //this.ViewManner=0;//Ĭ��Ϊ��0
            //MyLongFileReceiveProc = new LockServerLib.LongFileReceiveProc(); 
         
          
        }

        public LoginUser(int  InLoginID,ref SocketServiceReadWriteChannel MeSocketServiceReadWriteChannel)
        {
            this.LoginID = InLoginID;
            this.MyReadWriteSocketChannel = MeSocketServiceReadWriteChannel;           
            this.SetLoginTime = DateTime.Now;
            this.SetKeepTime = DateTime.Now;
            this.SocketInfor = MeSocketServiceReadWriteChannel.MyRemoteEndPointStr;//.Client.RemoteEndPoint.ToString();
            this.WorkStatus = 0;//Ĭ��Ϊ��0
            this.ChannelStatus = 1;//Ĭ��Ϊ��1
           //this.ViewManner = 0;
            //MyLongFileReceiveProc = new LockServerLib.LongFileReceiveProc(); 

        }        
        #endregion ----------------------------------------------------------------------

        public void ByteBufferCopy(int MeRecieveCount)
        {

            this.MyByteBuffer = new byte[MeRecieveCount];

            //���
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
