using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
//using System.Windows.Forms;
using System.Diagnostics;
using SmartBusServiceLib;
using LGJAsyncSocketClientLib;
using LGJAsynchSocketService.MessageQueue;

namespace LGJAsynchSocketService
{
    public enum SocketServiceTypeID
    {
        lockServer, MobileAppServer, RequestIndexServer, ResponseIndexServer

    }

    public class AsynchSocketServiceBaseFrame
    {

        protected int MyTCPListerPort = 8900;
        protected string MyRemoteServerIPStr;
        protected int MyRemoteServerPort;// 8900;

        public int AlgorithmFlag;

        public SocketServiceTypeID MySocketServiceTypeID;
        public bool DataFormateFlag;
        public bool EvetLogFlag;

        public delegate void AsynchRefreshNotifyHandler(string MessageStr,int MessageID);
        public  AsynchRefreshNotifyHandler MyRefreshNotifyCallBack;
        public EventLog MyEventLog;

              /*
              private delegate void DelegateRefreshRichTextBoxStatus(string str);
              public DelegateRefreshRichTextBoxStatus MyAppendRichTextBoxStatus;

              private delegate void delegateAddListBoxItemCallback(string str);
              private delegateAddListBoxItemCallback MyListBoxSendCallback;
              private delegateAddListBoxItemCallback MyListBoxRecieveCallback;
              */
        public ManagerSocketLoginUser MyManagerSocketLoginUser;

        public IPAddress[] LocalIPAddressArray;
        public IPAddress LocalListenerIPAddress;
        private TcpListener MyMainTCPlistener;
        int MyRecieveBufferSize = 65536;

        public bool IsExit = false;
        private ThreadStart MyThreadStartTCPService;
        private Thread MyThreadTCPService;
        private EventWaitHandle MyEventWaitAllDone;// = new EventWaitHandle(false, EventResetMode.ManualReset);


       
        private int TCPListerRunCountID = 0;
        private bool TCPListerStateID;

        //public LockVMSocketClient MyLockVMSocketClient;
        public LGJSocketClientAPIBase MyLGJSocketClientAPIBase;
        public SqlConnection MySqlConnection;

        public bool IsSaveToFile;

        public bool IsEMailPush;
        public bool IsNotePush;
        public bool IsSMSPush;

        //---2014.12-------------------------------------------------------------------------------------------------------------
        //public MessageEntityManager<LGJMessageEntity> MyMessageEntityManager = new MessageEntityManager<LGJMessageEntity>();
        int LoginID;

        //private int AroundDisplay = 1;//0--;1--
        //private string RightString;
        //-------threadlocal----------------
        //[ThreadStaticAttribute]
        //public static string MyLockIDStr;
        //[ThreadStaticAttribute]
        //public static string MyMobileIDStr;
        // [ThreadStaticAttribute]
        //public static uint FileReceiveFlag;
        // [ThreadStaticAttribute]
        //private static byte[] FileRecieveBuffer;
      

        #region ------构造函数-----------------------------------------------------------
        public AsynchSocketServiceBaseFrame()
        {
            LocalIPAddressArray = Dns.GetHostAddresses(Dns.GetHostName());
            MyEventWaitAllDone = new EventWaitHandle(false, EventResetMode.ManualReset);
            AlgorithmFlag = 0;
            DataFormateFlag = true;//默认为小端格式
            EvetLogFlag = true;////默认为记录日志
         


        }

        public AsynchSocketServiceBaseFrame(ManagerSocketLoginUser InManagerSocketLoginUser)
        {
            LocalIPAddressArray = Dns.GetHostAddresses(Dns.GetHostName());
            MyManagerSocketLoginUser = InManagerSocketLoginUser;
            MyEventWaitAllDone = new EventWaitHandle(false, EventResetMode.ManualReset);
            AlgorithmFlag = 0;
      
        }
        #endregion ----------------------------------------------------------------------


        public virtual void SetPortID(int PortID)
        {
            //MyManagerSocketLoginUser.MyAsynchSendMessageCallback = new ManagerSocketLoginUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);   
            MyTCPListerPort = PortID;
        }

        private void InitServiceParas()
        {
  /*
            MyTCPListerPort = 8900;
            MySocketServiceTypeID = SocketServiceTypeID.lockServer;
            if (MyManagerSocketLoginUser == null) { MyManagerSocketLoginUser = new ManagerSocketLoginUser(); }
            MyManagerSocketLoginUser.MyAsynchSendMessageCallback = new ManagerSocketLoginUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);
            MyLGJSocketClientAPIBase=new LGJSocketClientAPILib.LockServerSocketClient();
             * */

        }

        public void SelectedLocalHostIPListIndex(int IndexID)
        {
            //LocalListenerIPAddress = LocalIPAddressArray[IndexID];
            //MyRemoteServerIPStr = LocalListenerIPAddress.ToString();
            //MyRemoteServerIPStr = "127.0.0.1";//目前只有移动端应用客户端Socket
        }

        public void DisplayResultInfor(int nFlagID, string Str)
        {
            if (!EvetLogFlag) return;

           try
            { 
           
                if (MyRefreshNotifyCallBack != null)
                    MyRefreshNotifyCallBack(Str, nFlagID);

                if (MyEventLog != null)
                {
                    if (Str != null && Str != "")
                        MyEventLog.WriteEntry(Str);
                }
            }
            catch (Exception ex)
            {


                //MyEventLog.WriteEntry();
                //return;

            }

        
               

            /*
             switch (nFlagID)
             {

                 case 0://MessageNotify
                     //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(Str);

                     break;

                 case 1: //RichTextBoxStatus
                     //MyCoudLockSeverMainForm.MyAppendRichTextBoxStatus(Str); 
                     MyCoudLockSeverMainForm.InvokeAppendRichTextBoxStatus(Str);
                     break;

                 case 2: //listBoxRecieveData  
                     MyCoudLockSeverMainForm.InvokeSetRecieveListBox(Str);
                     break;

                 case 3: //listBoxSendData
                     MyCoudLockSeverMainForm.InvokeSetSendListBox(Str);
                     break;

                 default:
                     break;

             }
             */

        }

        public bool StartAsynchSocketListerService()
        {

            try
            {
                if (TCPListerStateID == true) return true;
                 
               //if (IsExit == true)
                //{
                    
                    //MyMainTCPlistener.Server.Close();
                    //TCPListerRunCountID = 0;
                //}

                IsExit = false;
                MyThreadStartTCPService = new ThreadStart(AcceptTCPConnect);
                MyThreadTCPService = new Thread(MyThreadStartTCPService);
                MyThreadTCPService.IsBackground = true;
                MyThreadTCPService.Start();
                TCPListerStateID=true;
                       
                //==============================================
                if (MyLGJSocketClientAPIBase != null)
                {
                    MyLGJSocketClientAPIBase.SetServerIP(MyRemoteServerIPStr, MyRemoteServerPort);

                    if (MyLGJSocketClientAPIBase.OpenConnect() == 0)
                    {
                        DisplayResultInfor(1, "◆连接到云智能总线服务器成功◆");
                    }
                    else
                    {
                        DisplayResultInfor(1, "◆连接到云智能总线服务器失败◆");
                    }
                }

               //------------------------------------------------
                if (MySqlConnection!= null)
                {
                    if (MySqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        DisplayResultInfor(1, "◆连接数据库服务器成功◆");
                        MySqlConnection.Close();
                    }
                    else
                    {
                        DisplayResultInfor(1, "◆连接数据库服务器失败◆");
                    }
                }


                return true;
            }
            
           catch (Exception ExceptionInfor)
            {

                    DisplayResultInfor(1, "启动TCP[Socket]服务器错误[{0}]:[10040]" + ExceptionInfor.Message + "\r\n");
                    TCPListerStateID = false;
                    return false;
           }

        }

        public void CloseAsynchSocketListerService()
        {
            if (TCPListerStateID == true)
            {
                if (MyManagerSocketLoginUser.DoAsynchCRUBListForCloseCallback != null) MyManagerSocketLoginUser.DoAsynchCRUBListForCloseCallback();
                System.Threading.Thread.Sleep(1000); //当前线程休眠
                TCPListerStateID = false;
              
                IsExit = true;
                MyEventWaitAllDone.Set();  

                System.Threading.Thread.Sleep(1000); //当前线程休眠
                MyMainTCPlistener.Server.Close();    //释放端口资源
                TCPListerRunCountID = 0;
                //=================================================
                if (MyLGJSocketClientAPIBase != null)
                {
                    MyLGJSocketClientAPIBase.CloseConnect();
                    DisplayResultInfor(1, "已关闭服务器连接管道");
                }
                if (MySqlConnection != null)
                {
                    if (MySqlConnection.State == System.Data.ConnectionState.Open)  
                    MySqlConnection.Close();
                    DisplayResultInfor(1, "已关闭数据库服务器连接");
                }


                DisplayResultInfor(1, "已关闭库服务器[10001]");

                /*
             if (MyMainTCPlistener != null&& MyMainTCPlistener.Server.Connected)
           {
               MyMainTCPlistener.Server.Close();
               //MyMainTCPlistener.Stop();
               //MyThreadTCPService.Abort();
               //MyThreadTCPService.Join();

           }
           */

                //MyManagerSocketLoginUser.DoAsynchCRUBListForCloseCallback.Invoke();
                //System.Threading.Thread.Sleep(1000);  
                //MyMainTCPlistener.Server.Close();
                //TCPListerRunCountID = 0;

                //MyManagerSocketLoginUser.CRUDLoginUserListForClose();
                //MyMainTCPlistener.Server.Close();
                //TCPListerRunCountID = 0;

            }

        }

        private void AcceptTCPConnect()
        {
            try
            {

                //===1.绑定端口与地址=========================================================
                IPEndPoint MyListerIPGroup = new IPEndPoint(IPAddress.Any, MyTCPListerPort);
                MyMainTCPlistener = new TcpListener(MyListerIPGroup);
                //MyMainTCPlistener = new TcpListener(LocalListenerIPAddress, MyTCPListerPort);
                MyMainTCPlistener.Server.ReceiveBufferSize = MyRecieveBufferSize;
                MyMainTCPlistener.Server.SendBufferSize = MyRecieveBufferSize;
                MyMainTCPlistener.Start();

                //ProcessMessageEntity<LGJMessageEntity, MessageEntityManager<LGJMessageEntity>>.Start(MyMessageEntityManager,this);

            }
            catch (Exception ex)
            {

                //MyEventLog.WriteEntry("可能是服务侦听套接字错误不能运行将退出[10020]！")
                //System.Windows.Forms.MessageBox.Show("可能是服务侦听套接字错误不能运行将退出！");
                //System.Windows.Forms.Application.Exit();

             DisplayResultInfor(6, string.Format("◆{0}◆服务侦听器套接字错误错误:[10010]", MySocketServiceTypeID));
             return;

            }
            //==2.如成功==============================================
            while (IsExit == false)
            {
                try
                {
                    //由于TCP协议三次握手阶段是独占资源的，无须加锁控制
                    MyEventWaitAllDone.Reset();
                    AsyncCallback MyAsyncAcceptCallback = new AsyncCallback(AcceptTcpClientConnectCallback);
                    if (TCPListerRunCountID == 0)
                    {

                        //MyCoudLockSeverMainForm.MyAppendRichTextBoxStatus("开始等待客户连接\r\n"); 
                        //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBackCallback("等待客户连接");
                        //DisplayResultInfor(0, "准备等待连接");
                        //DisplayResultInfor(1, string.Format("◆{0}◆准备等待连接[{1}][{2}]", MySocketServiceTypeID, MyTCPListerPort, MyMainTCPlistener.Server.ReceiveBufferSize));
                        DisplayResultInfor(1, string.Format("{0}准备等待连接[{1}][{2}]", MySocketServiceTypeID, MyTCPListerPort, MyMainTCPlistener.Server.ReceiveBufferSize));

                    }
                    TCPListerRunCountID = 1;
                    MyMainTCPlistener.BeginAcceptTcpClient(MyAsyncAcceptCallback, MyMainTCPlistener);
                   
                    MyEventWaitAllDone.WaitOne();//异步等待！Proactor设计模式
              
                
                }
                catch (Exception ExceptionInfor)
                {

                    //MyCoudLockSeverMainForm.MyAppendRichTextBoxStatus("连接异常错误:" + err.Message + "\r\n");
                    //DisplayResultInfor(1, "Socket服务侦听器错误:" + ExceptionInfor.Message + "\r\n");

                    DisplayResultInfor(1, string.Format("{0}服务器侦听错误:[10020]", MySocketServiceTypeID));

                    break;
                }
            }


        }

        private void AcceptTcpClientConnectCallback(IAsyncResult InputAsynchResult)
        {
            try
            {
                //这个函数在另外一个线程中执行
                 MyEventWaitAllDone.Set(); //侦听线程继续等待写一个客户端连接

                 TcpListener MyListener = (TcpListener)InputAsynchResult.AsyncState;
                 TcpClient MyTCPClient = MyListener.EndAcceptTcpClient(InputAsynchResult);

                 MyTCPClient.Client.IOControl(IOControlCode.KeepAliveValues, SetkeepalivInPARA(),null);//  here set keep-alive
                              
                //---------------------------------------------------------------------------------------------------------------------
                //if (AroundDisplay == 1)
                ///{
                //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTimeMarker.Substring(0,MyTimeMarker.IndexOf("[")) + "已接受客户连接：" + MyTCPClient.Client.RemoteEndPoint);
                //DisplayResultInfor(1, string.Format(MyTimeMarker + "已接受客户端[{0}]连接\r\n", MyTCPClient.Client.RemoteEndPoint));

                //}
                //-----------------------------------------------------------------------------------------------------------------------
                 //----回显信息--------------------------------------------------------------------------------------------------------
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond) + "[" + DateTime.Now.Ticks.ToString() + "]";
                //string MyTimeMarker = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);

                //DisplayResultInfor(1, string.Format(MyTimeMarker + "已接受客户端[{0}]连接", MyTCPClient.Client.RemoteEndPoint));

                if (MySocketServiceTypeID == SocketServiceTypeID.lockServer)
                {
                    DisplayResultInfor(1, string.Format(MyTimeMarker + " I[{0}]连接", MyTCPClient.Client.RemoteEndPoint));
                }
                if (MySocketServiceTypeID == SocketServiceTypeID.MobileAppServer)
                {
                    DisplayResultInfor(1, string.Format(MyTimeMarker + " M[{0}]连接", MyTCPClient.Client.RemoteEndPoint));
                }

                //---添加通道【涉及递归、无锁化设计、Proactor模式】---------------------------------------------------------------------------------------------------------
                SocketServiceReadWriteChannel MyReadWriteChannel = new SocketServiceReadWriteChannel(MyTCPClient);               

                //MyReadWriteChannel.MyNetWorkStream.BeginRead(MyReadWriteChannel.MyReadBuffers, 0, MyReadWriteChannel.MyReadBuffers.Length, AsynchReadClientCallback, MyReadWriteChannel);
                //MyManagerSocketLoginUser.CRUDLoginUserList(MyReadWriteChannel, 0);
                //LGJMessageEntity MyLGJMessageEntity = new LGJMessageEntity("create", 0, MyReadWriteChannel);
                //MyMessageEntityManager.AddMessageEntity(MyLGJMessageEntity);

                //增加会话状态记录
                LoginID++;
                LoginUser MyLoginUser = new LoginUser(LoginID, ref MyReadWriteChannel);

                this.MyManagerSocketLoginUser.MyLoginUserList.Add(MyLoginUser);

                MyReadWriteChannel.MyNetWorkStream.BeginRead(MyReadWriteChannel.MyReadBuffers, 0, MyReadWriteChannel.MyReadBuffers.Length, AsynchReadClientCallback, MyLoginUser);
               
                //MyEventWaitAllDone.Set();  //新改进！继续等待写一个客户端连接

                this.DisplayResultInfor(4, "");
                
                
                //MySocketServiceList.Add(MyReadWriteObject);
                //CommandFromInOut = 1;
                //CommandDefine(MyReadWriteObject, "#login!");
                //MySocketServiceList.Add(MyReadWriteObject);
                //CRUDLoginUserList(ref MyTCPClient, 0);
                //ClientListComboBox.Invoke(setComboBoxCallback, MyTCPClient.Client.RemoteEndPoint.ToString());
                //OnReceiveCommand(new CommandMessageEventArgs(MyReadWriteObject,"#login!","",0));


            }
            catch (Exception ExceptionInfor)
            {

                //DisplayResultInfor(1, "侦听器关闭[停止客户连接服务]" + "\r\n");
                DisplayResultInfor(1, string.Format("服务器侦听错误[{0}][10030]", ExceptionInfor.Message));
                //return;

            }



        }

        private  byte[] SetkeepalivInPARA()
        {
           
                uint dummy = 0;
                byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];

                BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);//是否启用Keep-Alive
                BitConverter.GetBytes((uint)5000 * 4).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间开始第一次探测单位：毫秒[20秒]
                BitConverter.GetBytes((uint)1000).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);//探测时间间隔单位：毫秒
                return inOptionValues;
              
        }

        private void AsynchReadClientCallbackXYZ(IAsyncResult InputAsynchResult)
        {
            /*
            SocketServiceReadWriteChannel MyReadWriteObject = (SocketServiceReadWriteChannel)InputAsynchResult.AsyncState;
            try
            {
                bool MyConnected = MyReadWriteObject.MyTCPClient.Connected;
                if (MyConnected)//-连接状态
                {
                    uint MyRecieveCount = (uint)MyReadWriteObject.MyNetWorkStream.EndRead(InputAsynchResult);
                    int MyRecieveAvailable = MyReadWriteObject.MyTCPClient.Available;

                    if (MyRecieveCount != 0)
                    {
                        //正常Transmission Data状态
                        LockServerLib.LongFileReceiveProc MyLongFileReceiveProc = MyManagerSocketLoginUser.FindLongFileReceiveProcList(ref MyReadWriteObject.MyTCPClient);
                        if (MyLongFileReceiveProc == null)
                        {
                            byte MessageTypeFirstFlag = MyReadWriteObject.MyReadBuffers[2];
                            CommandDefineDispatch(MyReadWriteObject, MyReadWriteObject.MyReadBuffers, (int)MessageTypeFirstFlag, (int)MyRecieveCount);//命令状态
                        }
                        else
                        {
                            if (MyRecieveCount < 65536 && MyLongFileReceiveProc.LoopReadCount == 0)
                            {
                                byte MessageTypeFirstFlag = MyReadWriteObject.MyReadBuffers[2];
                                CommandDefineDispatch(MyReadWriteObject, MyReadWriteObject.MyReadBuffers, (int)MessageTypeFirstFlag, (int)MyRecieveCount);//命令状态
                            }
                            else
                            {
                                MyLongFileReceiveProc.MyReadWriteChannel = MyReadWriteObject;
                                MyLongFileReceiveProc.MyAsynchSocketServiceBaseFrame = this;
                                LongFileReceiveLoopProcess(MyLongFileReceiveProc, MyReadWriteObject.MyReadBuffers, MyRecieveCount);//长文件传输状态

                            }
                        }


                        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                        if (IsExit == false)
                        {
                            MyReadWriteObject.InitReadArray();
                            MyReadWriteObject.MyNetWorkStream.BeginRead(MyReadWriteObject.MyReadBuffers, 0, MyReadWriteObject.MyReadBuffers.Length, AsynchReadClientCallback1, MyReadWriteObject);//继续循环读取
                        }

                        //===================================================================

                    }
                    else
                    {
                        //正常Colse Socket状态
                        string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                        DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]已主动断开连接（11）", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint));

                        MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteObject.MyTCPClient, 2);
                    }

                }
                else //非连接状态：由客户端Socket各种因素如掉线、断网、非正常关机引起的Socket通讯链路不通引发异常！
                {

                    string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond) + "[" + DateTime.Now.Ticks.ToString() + "]";
                    DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]已非正常断开连接（12）", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint));
                    MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteObject.MyTCPClient, 2);
                }

            }
            catch (Exception ExceptionInfor)
            {

                string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond) + "[" + DateTime.Now.Ticks.ToString() + "]";
                if (ExceptionInfor.Message.IndexOf("无法从传输连接中读取数据") > -1)//keep-alive机制 here detected!
                {

                    DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]已主动断开连接（10）", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint));
                    MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteObject.MyTCPClient, 2);
                    //MyManagerSocketLoginUser.CRUDLoginUserListForDelete(ref MyReadWriteObject.MyTCPClient);
                    return;

                }

                if (ExceptionInfor.Message.IndexOf("无法访问已释放的对象") > -1)
                {

                    //DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]已自动断开连接（10）", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint));
                    //MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteObject.MyTCPClient, 2);    
                    return;

                }

                else
                {
                    DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]读取错误：", ExceptionInfor));
                }
                //------------------------------------------------------------------------------------------------------
            }

            */


        }
       
        protected virtual void AsynchReadClientCallback(IAsyncResult InAsynchResult)
        {
           
            LoginUser MyLoginUser = (LoginUser)InAsynchResult.AsyncState;
            try
            {
                bool MyConnected = MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Connected;
                if (MyConnected)//-连接状态
                {
                    uint MyRecieveCount = (uint)MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);
                    int MyRecieveAvailable = MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Available;

                    if (MyRecieveCount != 0)
                    {

                        //==正常Transmission Data状态======================================
                        ProcessMessageEntry(MyLoginUser, MyRecieveCount, MyRecieveAvailable);//子类实现具体方法
            
                        /*
                        LockServerLib.LongFileReceiveProc MyLongFileReceiveProc = MyManagerSocketLoginUser.FindLongFileReceiveProcList(ref MyReadWriteObject.MyTCPClient);
                        if (MyLongFileReceiveProc == null)
                        {
                            byte MessageTypeFirstFlag = MyReadWriteObject.MyReadBuffers[2];
                            CommandDefineDispatch(MyReadWriteObject, MyReadWriteObject.MyReadBuffers, (int)MessageTypeFirstFlag, (int)MyRecieveCount);//命令状态
                        }
                        else
                        {
                            if (MyRecieveAvailable==0 && MyLongFileReceiveProc.LoopReadCount == 0)
                           {
                                byte MessageTypeFirstFlag = MyReadWriteObject.MyReadBuffers[2];
                                CommandDefineDispatch(MyReadWriteObject, MyReadWriteObject.MyReadBuffers, (int)MessageTypeFirstFlag, (int)MyRecieveCount);//命令、短文件传输状态
                           }
                           else
                           {
                                MyLongFileReceiveProc.MyReadWriteChannel = MyReadWriteObject;
                                MyLongFileReceiveProc.MyAsynchSocketServiceBaseFrame = this;
                                LongFileReceiveLoopProcess(MyLongFileReceiveProc, MyReadWriteObject.MyReadBuffers, MyRecieveCount);//长文件传输状态
                              
                           }
                        }
                        */

                        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                        if (IsExit == false)
                        {
                            MyLoginUser.MyReadWriteSocketChannel.InitReadArray();
                            MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.BeginRead(MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers, 0, MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers.Length, AsynchReadClientCallback, MyLoginUser);//继续循环读取
                        }

                        //===================================================================

                    }
                    else
                    {
                        MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);//后来完善加入
                        //--零字节调用--
                        //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                        //string MyTimeMarker = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                        //DisplayResultInfor(1, string.Format(MyTimeMarker + "客户端[{0}]已主动断开连接（11）", MyLoginUser.MyReadWriteSocketChannel.MyRemoteEndPointStr));

                        string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                        DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}]零字节调用已断开（13）", MyLoginUser.MyReadWriteSocketChannel.MyRemoteEndPointStr));

                        this.MyManagerSocketLoginUser.MyLoginUserList.Remove(MyLoginUser);//线程安全有待考虑

                        ClientCloseEventCallback(MyLoginUser.MobileID, MyLoginUser.LockID);
                        DisplayResultInfor(4, "");
                      
                      


                    }

                }
                else //非连接状态：由客户端Socket各种因素如掉线、断网、非正常关机、无认证取消通道引起的Socket通讯链路不通！
                {
                    MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);//后来完善加入
                    //--> + "[" + DateTime.Now.Ticks.ToString() + "]"
                    //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                    //string MyTimeMarker = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                    string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                    DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}]已非正常断开连接（12）", MyLoginUser.SocketInfor));
                    //MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteObject.MyTCPClient, 2);
                    ClientCloseEventCallback(MyLoginUser.MobileID, MyLoginUser.LockID);
                    this.MyManagerSocketLoginUser.MyLoginUserList.Remove(MyLoginUser);
                    DisplayResultInfor(4, "");

                }

            }
            catch (Exception ExceptionInfor)
            {
                //+ "[" + DateTime.Now.Ticks.ToString() + "]"
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond) ;
                //string MyTimeMarker = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                if (ExceptionInfor.Message.IndexOf("无法从传输连接中读取数据") > -1)
                {

                    MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);//后来完善加入

                    DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}]已主动断开连接（10）", MyLoginUser.SocketInfor));
                    ClientCloseEventCallback(MyLoginUser.MobileID, MyLoginUser.LockID);
                    this.MyManagerSocketLoginUser.MyLoginUserList.Remove(MyLoginUser);//
                    DisplayResultInfor(4, "");
                    return;

                }

                //-------------------------------------------------------------------------------
                if (ExceptionInfor.Message.IndexOf("无法访问已释放的对象") > -1)
                {
                    MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);//后来完善加入
                    DisplayResultInfor(1, string.Format(MyTimeMarker + "服务器已自动断开客户端连接[已释放的对象:Client-SocketChannel]"));
                    return;

                }

                else
                {
                    //------继续读取：细节完善---------------------------------------------------------
                    if (IsExit == false)
                    {
                        MyLoginUser.MyReadWriteSocketChannel.InitReadArray();
                        MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.BeginRead(MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers, 0, MyLoginUser.MyReadWriteSocketChannel.MyReadBuffers.Length, AsynchReadClientCallback, MyLoginUser);//继续循环读取
                    }

                    DisplayResultInfor(1, string.Format(MyTimeMarker + "传输解析数据错误：[{0}]", ExceptionInfor));
                }
               
                //------------------------------------------------------------------------------------------------------
            }
            finally
            {
                //MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndRead(InAsynchResult);

            }

        }

        protected virtual void ProcessMessageEntry(LoginUser MyLoginUser, uint MyRecieveCount, int MyRecieveAvailable)
        {
            //虚函数，子类必须实现：



        }

        public virtual void CommandDefineDispatch(SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] MyReadBuffers, int RecieveMessageFlag, int InputRecieveCount)
        {

            //虚函数，子类实现具体方法
            /*
            string BaseMessageString = null;
            string HexSendMessageIDString = null; 
            if (RecieveMessageFlag < 250)
            {
                //1.缓冲区按原样全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------
                if (InputRecieveCount < 256)
                {
                    for (int i = 0; i < InputRecieveCount; i++)
                    {

                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[i]);

                    }
                }
                else
                {

                    if (InputRecieveCount <=65536)
                    {
                        BaseMessageString = string.Format("{0:X2}", MyReadBuffers[1]);
                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[0]);
                        UInt16 RecieveMessageLenght = Convert.ToUInt16(BaseMessageString, 16);
                        BaseMessageString = string.Format("文件传输[{0}]", RecieveMessageLenght);

                    }
                    else
                    {
                        BaseMessageString = string.Format("{0:X2}", MyReadBuffers[21]);
                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[20]);
                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[19]);
                        BaseMessageString = BaseMessageString + string.Format("{0:X2}", MyReadBuffers[18]);

                        UInt32 RecieveMessageLenght = Convert.ToUInt32(BaseMessageString, 16);
                        BaseMessageString = string.Format("文件传输[{0}]", RecieveMessageLenght);
                    }



                  

                }

                //-------------------------------------
                       
                if (DataFormateFlag)
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
                }
                else
                {
                    HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
                }


            }
            else
            {
                //2.把来自移动端的消息字符串显示-----------------------------------------------------------------------------------------------------------------------
                  /*
                if (RecieveMessageFlag == 254)
                {
                    BaseMessageString = Encoding.Unicode.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
                }
                else
                {
                    BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);
                }
                */
            //BaseMessageString = Encoding.UTF8.GetString(MyReadBuffers, 3, InputRecieveCount - 3);

            //}
            /*
            string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            DisplayResultInfor(2, string.Format(MyTimeMarkerStr + "[{0}]{1}", InputSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, BaseMessageString ));
                  
            //====================================================================================
           switch (RecieveMessageFlag)
            {
                case 0:
                     //智能锁响应消息
                    SamrtLockResponseMessage(InputSocketServiceReadWriteChannel, InputRecieveCount, HexSendMessageIDString);
                    break;
                case 1:
                    //智能锁主动上传消息
                    SamrtLockRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount, HexSendMessageIDString);
              
                    break;

                case 255:
                    //移动端请求消息[]ASCII]
                    SamrtMobileRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount);
                    break;

                case 254:
                    //移动端请求消息[UNICODE]
                    SamrtMobileRequestMessage(InputSocketServiceReadWriteChannel, InputRecieveCount);


                    break;
                default:
                    break;

            }
            */

        }

        protected virtual void ClientCloseEventCallback(string InMobileIDStr, string InLockIDStr)
        {
            
            //虚函数，子类实现具体方法
            /*
           string CommandMessageStr = "lockoutoff";

           CommandMessageStr = CommandMessageStr + "#" + InMobileIDStr + "-" + InLockIDStr + "#[" + "howlock" + "," + GetDateTimeWeekIndex() + "]!";
           byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

           int nBuffersLenght = MySendBaseMessageBytes.Length;// CommandMessageStr.Length;
           byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


           MySendMessageBytes[2] = 250;

           //填充
           for (int i = 0; i < nBuffersLenght; i++)
           {

               MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

           }

           //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------

           try
           {

               this.StartAsynchSendMessage(MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel, MySendMessageBytes);

           }
           catch
           {

               DisplayResultInfor(1, "转发移动端通道错误");
           }
            * */


        }


        //==============Write Start===================================================================================================================================

        public void StartAsynchSendMessage(SocketServiceReadWriteChannel MyReadWriteChannel, byte[] SendMessageBuffers)
        {
            //------12.26改进：
            try
            {
                MyReadWriteChannel.MyWriteBuffers = SendMessageBuffers;// Encoding.UTF8.GetBytes(Sendstr);
                MyReadWriteChannel.MyNetWorkStream.BeginWrite(MyReadWriteChannel.MyWriteBuffers, 0, MyReadWriteChannel.MyWriteBuffers.Length, new AsyncCallback(AsynchWriteMessageCallback), MyReadWriteChannel);
                MyReadWriteChannel.MyNetWorkStream.Flush();
             
                /*
                 string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();//DateTime.Now.ToString()
                //1.全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------
                string HexSendMessageString = null;
                for (int i = 0; i < SendMessageBuffers.Length; i++)
                {

                    HexSendMessageString = HexSendMessageString + string.Format("{0:X2}", SendMessageBuffers[i]);

                }
                this.DisplayResultInfor(3, string.Format(MyTimeMarkerStr + "--[{0}]{1}", MyReadWriteObject.MyTCPClient.Client.RemoteEndPoint, HexSendMessageString));
               */

            }
            catch (Exception ExceptionInfor)
            {


                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                DisplayResultInfor(1, string.Format(MyTimeMarker + "向[{0}]发送数据失败（1）\r\n", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
                //System.Diagnostics.Debug.WriteLine(ExceptionInfor.Message);    

            }
        }

        private void AsynchWriteMessageCallback(IAsyncResult InIAsyncResult)
        {
            SocketServiceReadWriteChannel MyReadWriteChannel = (SocketServiceReadWriteChannel)InIAsyncResult.AsyncState;
            try
            {
                MyReadWriteChannel.MyNetWorkStream.EndWrite(InIAsyncResult);
                SendMessageForString(MyReadWriteChannel.MyWriteBuffers, MyReadWriteChannel);
                //-----------判断关闭该通道-------------------------------------------------------
                if (MyReadWriteChannel.IsCloseSocket) MyReadWriteChannel.MyTCPClient.Client.Close();

               
            }
            catch (Exception ExceptionInfor)            {


              //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
              string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
              DisplayResultInfor(1, string.Format(MyTimeMarker + "向客户端发送数据失败（2）[{0}]\r\n",ExceptionInfor));
             //DisplayResultInfor(1, string.Format(MyTimeMarker + "向[{0}]发送数据失败（2）\r\n", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
              
            }
        }

        public void StartAsynchSendMessageEx(LoginUser MyLoginUser, byte[] SendMessageBuffers)
        {
              //------12.26改进[关于关闭如何通道]------------------------------------------------------------
            try
            {
                MyLoginUser.MyReadWriteSocketChannel.MyWriteBuffers = SendMessageBuffers;// Encoding.UTF8.GetBytes(Sendstr);
                MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.BeginWrite(MyLoginUser.MyReadWriteSocketChannel.MyWriteBuffers, 0, MyLoginUser.MyReadWriteSocketChannel.MyWriteBuffers.Length, new AsyncCallback(AsynchWriteMessageCallbackEx), MyLoginUser);
                MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.Flush();
                

            }
            catch (Exception ExceptionInfor)
            {
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                DisplayResultInfor(1, string.Format(MyTimeMarker + "向[{0}]发送数据失败（1）\r\n", MyLoginUser.SocketInfor));
                //System.Diagnostics.Debug.WriteLine(ExceptionInfor.Message);    

            }
        }

        private void AsynchWriteMessageCallbackEx(IAsyncResult InIAsyncResult)
        {
            LoginUser MyLoginUser = (LoginUser)InIAsyncResult.AsyncState;
            try
            {
                MyLoginUser.MyReadWriteSocketChannel.MyNetWorkStream.EndWrite(InIAsyncResult);
                SendMessageForString(MyLoginUser.MyReadWriteSocketChannel.MyWriteBuffers, MyLoginUser.MyReadWriteSocketChannel);
                //-----------判断关闭该通道【可能定时关闭更合适】-------------------------------------------------------
                //if (MyLoginUser.IsCloseSocket) MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();


            }
            catch (Exception ExceptionInfor)
            {


                //string MyTimeMarker = DateTime.Now.ToString() + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                DisplayResultInfor(1, string.Format(MyTimeMarker + "向客户端发送数据失败（2）[{0}]\r\n", ExceptionInfor));
                //DisplayResultInfor(1, string.Format(MyTimeMarker + "向[{0}]发送数据失败（2）\r\n", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));

            }
        }

        protected virtual void SendMessageForString(Byte[] InSendMessageBuffers, SocketServiceReadWriteChannel InputReadWriteChannel)
        {
          
               //string MyTimeMarker = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
              string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
              string SendMessageString = null;
               if (InSendMessageBuffers[2] < 250)
               {
                   //1.全部转换为16进制字符串表示---------------------------------------------------------------------------------------------------------

                   for (int i = 0; i < InSendMessageBuffers.Length; i++)
                   {

                       SendMessageString = SendMessageString + string.Format("{0:X2}", InSendMessageBuffers[i]);

                   }

                   //SendMessageForSaveDB(InSendMessageBuffers);//测试需要！
               }
               else
               {
                   //2.把来自移动端的消息字符串显示-----------------------------------------------------------------------------------------------------------------------

                   if (InSendMessageBuffers[2] == 254)
                   {

                       SendMessageString = Encoding.Unicode.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                       
                   }
                   else
                   {
                       SendMessageString = Encoding.UTF8.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
                   }


               }

                  this.DisplayResultInfor(3, string.Format(MyTimeMarker + " Tx[{0}]{1}", InputReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, SendMessageString));
              

        }
       
        //===============Write End==================================================================================================================================
     
        public virtual void SocketClientResponseCallBack(byte[] RecieveMessageBytes, int RecieveCount)
        {
            ; ;
        }
     
        public virtual void SocketClientRequestCallBack(string MessageStr)
        {
            ; ;
        }
        
        protected void SendMessageForSaveDB(Byte[] InSendMessageBuffers)
        {

            string LockID = "1234567890ABCDE";         
            string MessageID = string.Format("{0:X2}", InSendMessageBuffers[9]) + string.Format("{0:X2}", InSendMessageBuffers[8]);
            string SendMessageString = null;
            for (int i = 0; i < InSendMessageBuffers.Length; i++)
            {

                SendMessageString = SendMessageString + string.Format("{0:X2}", InSendMessageBuffers[i]);

            }
            MessageListManager MyMessageListManager = new MessageListManager();
            MyMessageListManager.InsertMessageSave(LockID, MessageID,SendMessageString); 

        }

        public string GetDateTimeWeekIndex()
        {
   //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
            //string Teststr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            string DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            string WeekIndexStr = DateTime.Now.ToString("dddd");
            string nReturn = null;
            switch (WeekIndexStr)
            {
                case "星期一": nReturn = "1";
                    break;

                case "星期二": nReturn = "2";
                    break;

                case "星期三": nReturn = "3";
                    break;

                case "星期四": nReturn = "4";
                    break;

                case "星期五": nReturn = "5";
                    break;

                case "星期六": nReturn = "6";
                    break;

                case "星期日": nReturn = "7";
                    break;

                default:
                    nReturn = "0";
                    break;

            }


            return DateTimeStr + nReturn;
        }
    
    }
}
