using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace LGJAsyncSocketClientLib
{
    public class LGJSocketClientAPIBase
    {

        protected bool ThreadIsExit;
        private bool AsynchID;
        public int ConnectID;
        public int ReceiveIDFlag;
        public int SendIDFlag;
        protected int RecieveTimeOutValue;
        protected int SendTimeOutValue;

        protected StringBuilder SynchReceiveDataStr;
        protected byte[] ReceiveMessageBuffers;

        protected int PortNumber;//8900
        public TcpClient MyTcpClient;
        protected string RemoteServerIPStr;
        protected NetworkStream MyNetworkStream;
        protected EventWaitHandle MyEventWaitDone;


        public delegate void RemoteServerColseHandler();
        public RemoteServerColseHandler MyRemoteServerColseCallBack;

        public delegate void SendMessageHandler(string str);
        public SendMessageHandler ReturnSendMessageInvoke;

        public delegate void RecieveMessageHandler(byte[] RecieveMessageBytes, int RecieveCount);
        public RecieveMessageHandler MyRecieveMessageCallback;

        public LGJSocketClientAPIBase()
        {
            RemoteServerIPStr = "127.0.0.1";
            AsynchID = true;
            ThreadIsExit = false;
            ReceiveIDFlag = 1;
            ConnectID = 1;
            SynchReceiveDataStr = new StringBuilder();

            //InitServerConfig();

            RecieveTimeOutValue = 1000 * 60 * 5;//5分钟
            SendTimeOutValue = 1000 * 60 * 5; //5分钟
            MyEventWaitDone = new EventWaitHandle(false, EventResetMode.ManualReset);



        }

        public LGJSocketClientAPIBase(string IPStr, int PortID)
        {
            AsynchID = true;
            ThreadIsExit = false;
            ConnectID = 1;
            RemoteServerIPStr = IPStr;
            PortNumber = PortID;
            SynchReceiveDataStr = new StringBuilder();
            RecieveTimeOutValue = 8000;//2秒
            SendTimeOutValue = 5000;  //1秒
            MyEventWaitDone = new EventWaitHandle(false, EventResetMode.ManualReset);

            //ReturnComputeResult += new EventHandler(OnResultReturnEX);
            //RemoteServerColseEvent += new EventHandler(OnRemoteServerColseEvent);

         





        }
        public void SetServerIP(string IPStr)
        {
            this.RemoteServerIPStr = IPStr;
        }
        public void SetServerIP(string IPStr, int PortID)
        {
            this.RemoteServerIPStr = IPStr; PortNumber = PortID;

        }
        public void SetSocketParas(int RecieveTimeOutValue, int SendTimeOutValue)
        {
            this.RecieveTimeOutValue = RecieveTimeOutValue;
            this.SendTimeOutValue = SendTimeOutValue;

        }

        public int OpenConnect()
        {

            if (ConnectID == 0)
            {
                return 0;
            }
            else
            {
                ThreadIsExit = false;
                CreateConnect();
                if (ConnectID == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
        public int CloseConnect()
        {
            //Interlocked.Decrement
            //if (ConnectID != 0)
            if ((ConnectID) != 0)
            {
                return 0;//已关闭
            }
            else
            {
                if (MyTcpClient != null && MyTcpClient.Connected == true)
                //if (MyTcpClient != null)
                {

                    ThreadIsExit = true;
                    MyTcpClient.Client.Close();
                    // MyTcpClient.Close();
                    ConnectID = 1;
                }
                return 0;
            }

        }

        private void CreateConnect()
        {

            MyTcpClient = new TcpClient(AddressFamily.InterNetwork);
            MyTcpClient.ReceiveTimeout = this.RecieveTimeOutValue;
            MyTcpClient.SendTimeout = this.SendTimeOutValue;
            AsyncCallback AcceptRequestCallback = new AsyncCallback(ServerAcceptCallback);
            IPAddress RecieveServerIP = IPAddress.Parse(this.RemoteServerIPStr);
            MyEventWaitDone.Reset();
            MyTcpClient.BeginConnect(RecieveServerIP, PortNumber, AcceptRequestCallback, MyTcpClient);
            //---------------------------------------------------------------------------------------
            MyEventWaitDone.WaitOne();

        }
        private void ServerAcceptCallback(IAsyncResult InIAsyncResult)
        {
            try
            {

                MyTcpClient = (TcpClient)InIAsyncResult.AsyncState;
                MyTcpClient.EndConnect(InIAsyncResult);
                if (MyTcpClient.Connected)
                {
                    ConnectID = 0;//成功连接标志为0
                    ReceiveIDFlag = 1;
                    //Interlocked.Decrement(ConnectID);
                    MyNetworkStream = MyTcpClient.GetStream();
                    ReadWriteSocketChannel MyReadWriteSocketChannel = new ReadWriteSocketChannel(MyNetworkStream, MyTcpClient.ReceiveBufferSize);
                    MyNetworkStream.BeginRead(MyReadWriteSocketChannel.RecieveMessageBytes, 0, MyReadWriteSocketChannel.RecieveMessageBytes.Length, AsynchReadDataCallback, MyReadWriteSocketChannel);

                }
                else
                {
                    ConnectID = 1;
                }


            }
            catch (Exception ExceptionInfor)
            {
                ConnectID = 2;

            }
            MyEventWaitDone.Set();




        }
        //===========Receive=================================================================
        protected virtual void AsynchReadDataCallback(IAsyncResult InIAsyncResult)
        {
            ReadWriteSocketChannel MyReadWriteSocketChannel = (ReadWriteSocketChannel)InIAsyncResult.AsyncState;
            try
            {
                int RecieveCount = MyReadWriteSocketChannel.netStream.EndRead(InIAsyncResult);
                if (RecieveCount == 0)
                {
                    ReceiveIDFlag = 1;
                    ConnectID = 1;//对方主机关闭！
                    //--1.------------------------------
                    //OnRemoteServerColseEvent(new EventArgs());                  
                    //--2.--------------------------------
                    if (MyRemoteServerColseCallBack != null) MyRemoteServerColseCallBack();//引发关机事件！
                    //--3..--------------------------------
                    //if (RemoteServerColseCallBackEvent != null)
                    //{
                        //MessageBox.Show("COM Client Call!!!...ServerColse ");
                        //RemoteServerColseCallBackEvent.Invoke();
                    //}
                }
                else
                {
                    //ReceiveDataStr.Append(Encoding.UTF8.GetString(MyReadObject.RecieveMessageBytes, 0, count));
                    ReceiveMessageBuffers = MyReadWriteSocketChannel.RecieveMessageBytes;

                    RecieveMessageParser(ReceiveMessageBuffers, RecieveCount);
                    ReceiveIDFlag = 0;


                    if (ThreadIsExit == false)
                    {
                        MyReadWriteSocketChannel = new ReadWriteSocketChannel(MyNetworkStream, MyTcpClient.ReceiveBufferSize);
                        MyNetworkStream.BeginRead(MyReadWriteSocketChannel.RecieveMessageBytes, 0, MyReadWriteSocketChannel.RecieveMessageBytes.Length, AsynchReadDataCallback, MyReadWriteSocketChannel);//继续接收数据！
                    }
                }




            }
            catch (Exception ExceptionInfor)
            {

                bool IsConnected = MyTcpClient.Client.Connected;
                if (IsConnected)
                {

                    if (ExceptionInfor.Message.IndexOf("无法从传输连接中读取数据") > -1)
                    {
                        ReceiveIDFlag = 2;
                        ConnectID = 1;//对方主机关闭！
                        //--1.---------------------------------------------------------------------
                        //OnRemoteServerColseEvent(new EventArgs());

                        //--2.-----------------------------------------------------------------------
                        if (MyRemoteServerColseCallBack != null) MyRemoteServerColseCallBack();

                        //--3..-----------------------------------------------------------------------                        
                        //if (RemoteServerColseCallBackEvent != null)
                        //{
                            //MessageBox.Show("COM Client Call!!!...ServerColse ");
                            //RemoteServerColseCallBackEvent.Invoke();
                        //}

                    }
                    else
                    {
                        ReceiveIDFlag = 3;//待验证


                    }


                }
                else
                {
                    ConnectID = 1;//对方主机关闭！
                    ReceiveIDFlag = 4;
                    //OnRemoteServerColseEvent(new EventArgs());
                    if (MyRemoteServerColseCallBack != null) MyRemoteServerColseCallBack();
                    
                    //if (RemoteServerColseCallBackEvent != null)
                    //{
                        //MessageBox.Show("COM Client Call!!!...ServerColse ");
                        //RemoteServerColseCallBackEvent.Invoke();
                    //}
                }




            }
            finally
            {
                /*
               if (ThreadIsExit == false)
               {
                   MyReadObject = new ReadObject(MyNetworkStream, MyTcpClient.ReceiveBufferSize);
                   MyNetworkStream.BeginRead(MyReadObject.RecieveMessageBytes, 0, MyReadObject.RecieveMessageBytes.Length, AsynchReadDataCallback, MyReadObject);//继续接收数据！
               }
               */
                // MyEventWaitDone.Set();//结束读取数据！


            }



        }

        protected virtual void RecieveMessageParser(byte[] RecieveMessageBytes, int RecieveCount)
        {

            if (MyRecieveMessageCallback != null)
                MyRecieveMessageCallback(RecieveMessageBytes, RecieveCount);

         //----COM Visual------------------------------------------------------------------------------------
            /*
            string HexRecieveMessageString = "";
            for (int i = 0; i < RecieveCount; i++)
            {

                HexRecieveMessageString = HexRecieveMessageString + string.Format("{0:X2}", RecieveMessageBytes[i]);

            }

            //--------------------------------------------------------------------------------------------------
            bool DataFormateFlag = true;
            string HexRecieveMessageIDString;
            if (DataFormateFlag)
            {
                HexRecieveMessageIDString = string.Format("{0:X2}", RecieveMessageBytes[9]) + string.Format("{0:X2}", RecieveMessageBytes[8]);
            }
            else
            {
                HexRecieveMessageIDString = string.Format("{0:X2}", RecieveMessageBytes[8]) + string.Format("{0:X2}", RecieveMessageBytes[9]);
            }

            */

           /*
            if (CalculationCompletedCallBackEvent != null)
            {

                if (HexRecieveMessageIDString != "4001")
                {
                    if (HexRecieveMessageIDString != "676F")
                    {
                        //MessageBox.Show("COM Client Call! ...RecieveMessage:" + HexRecieveMessageIDString);
                        CalculationCompletedCallBackEvent.Invoke(10, HexRecieveMessageString, HexRecieveMessageIDString);
                    }

                }

            }
            */

        /*
            switch (HexRecieveMessageIDString)
            {
                case "2003":
                    ResponseUpdateKey();
                    break;
                case "3003":
                    ResponseGetPower();
                    break;

                case "3005":
                    ResponseSynchTime();
                    break;

                case "2205":
                    ResponseAddKey();
                    break;

                case "2007":
                    ResponseDeleteKey();
                    break;

                case "2009":
                    ResponseTempKey();
                    break;
                default:
                    break;




            }
           */

         /*
                    //--解析字符串-----------------------------------------------------------------------------------------------------
                    if (LockMobileClientFlag)//--模拟移动端模式
                    {
                        string BaseMessageString;
                        if (RecieveMessageBytes[2] > 250)
                        {

                                if (RecieveMessageBytes[2] == 254)
                                {
                                    BaseMessageString = Encoding.Unicode.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
                                }
                                else
                                {
                                    BaseMessageString = Encoding.ASCII.GetString(RecieveMessageBytes, 3, RecieveCount - 3);
                                }


                                string CommandMessageTypeIDStr = BaseMessageString.Substring(0, BaseMessageString.IndexOf("#"));

                                string CommandMessageResultStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf(",") + 1);

                                CommandMessageResultStr = CommandMessageResultStr.Substring(0, CommandMessageResultStr.IndexOf("!"));

                                string EndMessageResultStr = CommandMessageTypeIDStr + ": " + CommandMessageResultStr;

                                ReturnMobileEndMessageInvoke(EndMessageResultStr);

                                ReturnRecieveMessageInvoke(BaseMessageString);
                        }
                        else
                        {
                            SmartLockNotifyMessageParser(RecieveMessageBytes, RecieveCount);  //======智能锁反向通知==========================================================


                        }




                    }
                    else//--模拟智能锁模式
                    {

                        string HexRecieveMessageString = "";
                        for (int i = 0; i < RecieveCount; i++)
                        {

                            HexRecieveMessageString = HexRecieveMessageString + string.Format("{0:X2}", RecieveMessageBytes[i]);

                        }

                        ReturnRecieveMessageInvoke(HexRecieveMessageString);
                        //----------------------------------------------------------------------------------------------

                        string HexRecieveMessageIDString;
                        if (DataFormateFlag)
                        {
                            HexRecieveMessageIDString = string.Format("{0:X2}", RecieveMessageBytes[9]) + string.Format("{0:X2}", RecieveMessageBytes[8]);
                        }
                        else
                        {
                            HexRecieveMessageIDString = string.Format("{0:X2}", RecieveMessageBytes[8]) + string.Format("{0:X2}", RecieveMessageBytes[9]);
                        }

                        switch (HexRecieveMessageIDString)
                        {
                            case "2003":
                                ResponseUpdateKey();
                                break;
                            case "3003":
                                ResponseGetPower();
                                break;

                            case "3005":
                                ResponseSynchTime();
                                break;

                            case "2205":
                                ResponseAddKey();
                                break;

                            case "2007":
                                ResponseDeleteKey();
                                break;

                            case "2009":
                                ResponseTempKey();
                                break;
                            default:
                                break;




                        }
                        //-----------------------------------------------------------------------------------------------------

                    }
                    */

        }

        //-----Start-Send------------------------------------------------------------------------

        public virtual int SynchSendCommand(byte[] InSendMessageBuffers)
        {
            ReceiveIDFlag = 1;

            //ReceiveDataStr.Remove(0, ReceiveDataStr.Length);
            // int SendMessageGropCount = (InSendMessageBuffers.Length)/65536;
            //int FinallySendMessageCount = InSendMessageBuffers.Length - SendMessageGropCount * 65536;


            ;

            SendBinaryMessage(InSendMessageBuffers);

            return SendIDFlag;
        }

        public int AsynchSendVideoFrameBuffer(byte[] InSendVideoFrameBuffers)
        {
            ReceiveIDFlag = 1;
            //-------------------

            SendBinaryMessage(InSendVideoFrameBuffers);

            return SendIDFlag;
        }

        public int AsynchSendSnapVideoFrame(byte[] InSendVideoFrameBuffers)
        {
            ReceiveIDFlag = 1;

            try
            {
                bool DataFormateFlag = true;
                int PreHeadLenght = 40;
                UInt32 SendImageFileLenght;
                string HexSendMessageIDString;
                SendImageFileLenght = (UInt32)InSendVideoFrameBuffers.Length;
                byte[] MySendMessageBytes = new byte[SendImageFileLenght + PreHeadLenght];

                MySendMessageBytes[2] = 1;//推送消息标志
                MySendMessageBytes[15] = 0xFF;//虚拟锁标志！
                string HexSendLenghtString;
                if (DataFormateFlag)
                {
                    HexSendMessageIDString = "0930";//消息类别！
                }
                else
                {
                    HexSendMessageIDString = "3009";
                }


                MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
                MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);

                if (DataFormateFlag)
                {
                    MySendMessageBytes[10] = 1;
                    MySendMessageBytes[12] = 1;

                }
                else
                {
                    MySendMessageBytes[11] = 1;
                    MySendMessageBytes[13] = 1;
                }
                //------------------------------------------------------------------------------
                if (SendImageFileLenght <= 65536 - PreHeadLenght)//---短文件状态！
                {
                    //填充22个字节信息头开头两个字节
                    HexSendLenghtString = string.Format("{0:X4}", (UInt16)SendImageFileLenght + 22);
                    if (DataFormateFlag)
                    {
                        MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                        MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                    }
                    else
                    {
                        MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                        MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                    }



                }
                //-----文件长度信息！
                MySendMessageBytes[14] = 0xFF;//文件传输标志！
                string HexSendImageFileLenghtString = string.Format("{0:X8}", SendImageFileLenght);//文件长度：4个字节
                                                                                                   //填充40个字节信息头
                                                                                                   /*
                                                                                                                                                                                MySendMessageBytes[2] = 1;


                                                                                                                                                                                if (DataFormateFlag)
                                                                                                                                                                                {
                                                                                                                                                                                    HexSendMessageIDString = "0730";
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    HexSendMessageIDString = "3007";
                                                                                                                                                                                }

                                                                                                                                                                               MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
                                                                                                                                                                                MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);

                                                                                                                                                                                if (DataFormateFlag)
                                                                                                                                                                                {
                                                                                                                                                                                    MySendMessageBytes[10] = 1;
                                                                                                                                                                                    MySendMessageBytes[12] = 1;

                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    MySendMessageBytes[11] = 1;
                                                                                                                                                                                    MySendMessageBytes[13] = 1;
                                                                                                                                                                                }
                                                                                                                                                                                */

                if (DataFormateFlag)//小端模式
                {
                    MySendMessageBytes[18] = Convert.ToByte(HexSendImageFileLenghtString.Substring(6, 2), 16);
                    MySendMessageBytes[19] = Convert.ToByte(HexSendImageFileLenghtString.Substring(4, 2), 16);

                    MySendMessageBytes[20] = Convert.ToByte(HexSendImageFileLenghtString.Substring(2, 2), 16);
                    MySendMessageBytes[21] = Convert.ToByte(HexSendImageFileLenghtString.Substring(0, 2), 16);

                }
                else
                {
                    MySendMessageBytes[18] = Convert.ToByte(HexSendImageFileLenghtString.Substring(0, 2), 16);
                    MySendMessageBytes[19] = Convert.ToByte(HexSendImageFileLenghtString.Substring(2, 2), 16);

                    MySendMessageBytes[20] = Convert.ToByte(HexSendImageFileLenghtString.Substring(4, 2), 16);
                    MySendMessageBytes[21] = Convert.ToByte(HexSendImageFileLenghtString.Substring(6, 2), 16);


                }

                //-----------------------------------------------------------------------------------
                //byte[] MySendImageBytes = MyMemoryStream.GetBuffer();
                InSendVideoFrameBuffers.CopyTo(MySendMessageBytes, PreHeadLenght);//腾充图像数据


                if (SynchSendCommand(MySendMessageBytes) == 0)
                {
                    SendIDFlag = 0;
                }




            }
            catch (Exception ExceptionInfor)
            {
                SendIDFlag = 2;

            }
            finally
            {
                ;


            }
            return SendIDFlag;

            return SendIDFlag;

        }
        protected virtual void SendBinaryMessage(byte[] InSendMessageBuffers)
        {
            try
            {
                SendIDFlag = 1;
                /*
             MyEventWaitDone.Reset();
             MyNetworkStream.BeginWrite(InSendMessageBuffers, 0, InSendMessageBuffers.Length, new AsyncCallback(SendMessageCallback), MyNetworkStream);
             MyNetworkStream.Flush();
             MyEventWaitDone.WaitOne();
             SendMessageForString(InSendMessageBuffers);
              * */
                /*
          //=======================================================================================
           if(LockMobileClientFlag)
           {

               if (InSendMessageBuffers[2] == 254)
               {
                   string ALLRequstMessageString = Encoding.Unicode.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                   ReturnSendMessageInvoke(ALLRequstMessageString);
               }
               else
               {
                   string ALLRequstMessageString = Encoding.ASCII.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                   ReturnSendMessageInvoke(ALLRequstMessageString);
               }


           }
          else
          {
              string HexSendMessageString = "";
              for (int i = 0; i < InSendMessageBuffers.Length; i++)
              {

                  HexSendMessageString = HexSendMessageString + string.Format("{0:X2}", InSendMessageBuffers[i]);

              }
              ReturnSendMessageInvoke(HexSendMessageString);
          }
          */

                byte[] RequestAttachmentSendMessageBuffers = InSendMessageBuffers;
                MyNetworkStream.BeginWrite(InSendMessageBuffers, 0, InSendMessageBuffers.Length, new AsyncCallback(SendMessageCallback), RequestAttachmentSendMessageBuffers);
                MyNetworkStream.Flush();
                //SendMessageForString(InSendMessageBuffers);
                SendIDFlag = 0;




            }
            catch (Exception ExceptionInfor)
            {
                SendIDFlag = 2;
            }
        }

        protected virtual void SendMessageForString(Byte[] InSendMessageBuffers)
        {

            //子类实现

        }

        private void SendMessageCallback(IAsyncResult InAsyncResult)
        {
            byte[] RequestAttachmentSendMessageBuffers = (byte[])InAsyncResult.AsyncState;
            try
            {
                MyNetworkStream.EndWrite(InAsyncResult);
                SendMessageForString(RequestAttachmentSendMessageBuffers);

                SendIDFlag = 0;


            }
            catch (Exception ExceptionInfor)
            {
                SendIDFlag = 3;
            }



        }

        //----Send-End---------------------------------------------------------------------------------

    }
}
