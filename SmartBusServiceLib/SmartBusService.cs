using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SmartBusServiceLib
{
    public class SmartBusService
    {
        private string MobileServerIPStr;
        private int RemoteServerPortNo;
        private String MobileIDstr;
        private String CloudLockIDstr;

        private String SendMessageStr;
        private String ReceiveMessageStr;

        public int ReturnCode;
        public String ResultParaString;
        public String ResultErrorString;

        private Socket MySocket;
        private NetworkStream MyNetworkStream;
        private int MyReadTimeout;

        public SmartBusService(string InMobileIDstr, string InCloudLockIDstr, string RemoteServerIP, int InReadTimeout=60000)
        {
            MobileIDstr = InMobileIDstr;
            CloudLockIDstr = InCloudLockIDstr;
            ReturnCode = 1;
            RemoteServerPortNo = 8910;
            if (RemoteServerIP == null)
            {
                MobileServerIPStr = HelpTool.ServiceBusServerIP;// RemoteServerIP;// "192.168.1.190";
            }
            else
            {
                MobileServerIPStr =RemoteServerIP;

            }
            this.MyReadTimeout = InReadTimeout;


        
        }
        public SmartBusService(string InMobileIDstr, string InCloudLockIDstr, string RemoteServerIP, int InRemoteServerPortNo, int InReadTimeout = 60000)
        {
            MobileIDstr = InMobileIDstr;
            CloudLockIDstr = InCloudLockIDstr;
            ReturnCode = 1;
            if (InRemoteServerPortNo==0)
            RemoteServerPortNo = 9910;
            else
            RemoteServerPortNo = InRemoteServerPortNo;

            if (RemoteServerIP == null)
            {
                MobileServerIPStr = HelpTool.ServiceBusServerIP;// RemoteServerIP;// "192.168.1.190";
            }
            else
            {
                MobileServerIPStr = RemoteServerIP;

            }
            this.MyReadTimeout = InReadTimeout;



        }
        //-------------------------------------------------------------------------------------------------------
        public void LoginBus()
        {
            //SendMessageStr = "login#89765432BCDA820-9876DDDD8989100#!";
            SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
          
                SocketIOProc();
                if (ReceiveMessageStr != null)
                {
                    if (ReceiveMessageStr.IndexOf("true") > -1)
                    {
                        ReturnCode = 0;
                    }
                    else
                    {
                        ReturnCode =1;
                    }


                }
                else
                {
                    ReturnCode = 2;
                }
             ResultParaString = ReceiveMessageStr;

        }
        private void SocketIOProc() 
	   {
          try
          {
             Socket MySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
             //MySocket.Connect("LGJ2", 8910);
             MySocket.Connect(MobileServerIPStr, RemoteServerPortNo);
             NetworkStream MyNetworkStream = new NetworkStream(MySocket);
             MyNetworkStream.ReadTimeout = 20000;
             MyNetworkStream.WriteTimeout = 15000;


             //string SendStr = "login#89765432BCDA820-9876DDDD8989100#!";

             byte[] SendMessageBytes = Encoding.UTF8.GetBytes(SendMessageStr);

             byte[] SendAllBytes = new byte[SendMessageBytes.Length + 3];

             SendMessageBytes.CopyTo(SendAllBytes, 3);
             SendAllBytes[1] =1;
             SendAllBytes[2] = 255;

             MyNetworkStream.Write(SendAllBytes, 0, SendAllBytes.Length);

             byte[] ReadBytes = new byte[1024];

             int RecieveCount = MyNetworkStream.Read(ReadBytes, 0, 1024);

             ReceiveMessageStr = Encoding.UTF8.GetString(ReadBytes, 3, RecieveCount - 3);


             MyNetworkStream.Close();

             MySocket.Close();

         }
         catch (System.Exception Except)
         {
             ResultErrorString = Except.Message; 
             ReturnCode = 1; 

         }


  
		//------------------------------------------------------------
		
		

	}
        //--------------------------------------------------------------------------------------------------------
        public void SynchTime()
        {
            if (CreateBusSocket() != true)
            {

                return;
            }

            //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
            //if (ReturnCode != 0)
            //{
            //    CloseBusSocket();
            //    return;
            //
            //}

            SendMessageStr = "synchtime#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }
        
        public void UpdateKey(string KeyIDStr)
        {

            if (CreateBusSocket() != true)
            {
               
                return;
            }

            //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
            //if (ReturnCode != 0)
            //{
            //    CloseBusSocket();
            //    return;
            //
            //}

            SendMessageStr = "updatekey#" + CloudLockIDstr + "-" + MobileIDstr + "#" + KeyIDStr + "!";
            SocketIOProcEnter();
            CloseBusSocket();
            
            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;

        }

        public void ClearLockKey()
        {
            if (CreateBusSocket() != true)
            {

                return;
            }

            SendMessageStr = "clearlockkey#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }
      
        public void GetPower()
        {

            if (CreateBusSocket() != true)
            {

                return;
            }
          
            //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
            //if (ReturnCode != 0)
            //{
             //   CloseBusSocket();
              //  return;
            //
            //}

            SendMessageStr = "getpower#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }

        public void AddElectKey(string NameIDStr, string KeyIDStr)
        {
            if (CreateBusSocket() != true)
            {

                return;
            }
            //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
           // if (ReturnCode != 0)
            //{
            //    CloseBusSocket();
           //     return;
           //
           // }

            SendMessageStr = "addkey#" + CloudLockIDstr + "-" + MobileIDstr + "#" + NameIDStr +","+KeyIDStr+ "!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }

        public void TempElectKey(string TempKeyIDStr)
        {

           if (CreateBusSocket() != true)
            {

                return;
            }
             //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
            //if (ReturnCode != 0)
            //{
            //    CloseBusSocket();
            //    return;
            //
            //}

            SendMessageStr = "tempkey#" + CloudLockIDstr + "-" + MobileIDstr + "#"+TempKeyIDStr+"!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }

        public void DeleteElectKey(byte NumberID)
        {

            string NumberIDStr = NumberID.ToString();
            if (CreateBusSocket() != true)
            {

                return;
            }
            //SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
            //SocketIOProcEnter();
            ///if (ReturnCode != 0)
            //{
            //    CloseBusSocket();
           //     return;
           //
           // }

            SendMessageStr = "deletekey#" + CloudLockIDstr + "-" + MobileIDstr + "#" + NumberIDStr + "!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }

        public void RemoteSnap(Byte SnapID=1)
        {
                
            
            if (CreateBusSocket() != true)
            {

                return;
            }
            
            // SendMessageStr = "login#" + CloudLockIDstr + "-" + MobileIDstr + "#!";
           // SocketIOProcEnter();
           // if (ReturnCode != 0)
           /// {
           //     CloseBusSocket();
           //     return;
          ///
           // }

            SendMessageStr = "remotesnap#" + CloudLockIDstr + "-" + MobileIDstr + "#"  + "1!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }

        public void OpenDoorList()
        {


            if (CreateBusSocket() != true)
            {

                return;
            }

            SendMessageStr = "getopen#" + CloudLockIDstr + "-" + MobileIDstr + "#" + "!";
            SocketIOProcEnter();
            CloseBusSocket();

            if (ReceiveMessageStr != null)
            {
                if (ReceiveMessageStr.IndexOf("true") > -1)
                {
                    ReturnCode = 0;
                }
                else
                {
                    ReturnCode = 1;
                }


            }
            else
            {
                ReturnCode = 2;
            }
            ResultParaString = ReceiveMessageStr;
        }
        //-----------------------------------------------------------------------------------------------------------      
        private bool CreateBusSocket()
        {

            try
            {
                MySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //MySocket.Connect("LGJ2", 8910);
                MySocket.Connect(MobileServerIPStr, RemoteServerPortNo);
                MyNetworkStream = new NetworkStream(MySocket);
                MyNetworkStream.ReadTimeout = MyReadTimeout;// 20000;
                MyNetworkStream.WriteTimeout = 15000;
              
                ReturnCode = 0;
                return true;
            }
            catch (System.Exception Except)
            {
                ResultErrorString = Except.Message;
                ReturnCode = 1;
                return false;

            }

        }
        private bool CloseBusSocket()
        {

            try
            {
                MyNetworkStream.Close();
                MySocket.Close();
                ReturnCode = 0;
                return true;
            }
            catch (System.Exception Except)
            {
                ResultErrorString = Except.Message;
                ReturnCode = 1;
                return false;

            }
            
          
        }
        private void SocketIOProcEnter()
        {

            try
            {
                byte[] SendMessageBytes = Encoding.UTF8.GetBytes(SendMessageStr);

                byte[] SendAllBytes = new byte[SendMessageBytes.Length + 3];

                SendMessageBytes.CopyTo(SendAllBytes, 3);

                SendAllBytes[1] = 1;//同步Socket客户端标志
                SendAllBytes[2] = 255;//移动端请求标志

                MyNetworkStream.Write(SendAllBytes, 0, SendAllBytes.Length);

                byte[] ReadBytes = new byte[1024];

                int RecieveCount = MyNetworkStream.Read(ReadBytes, 0, 1024);

                ReceiveMessageStr = Encoding.UTF8.GetString(ReadBytes, 3, RecieveCount - 3);

                ReturnCode = 0;
                
               
            }
            catch (System.Exception Except)
            {
                ResultErrorString = Except.Message;
                ReturnCode = 1;
               

            }
            
          //---------------------------------------------
           

        }
        //-----------------------------------------------------------------------------------------------------------
    }
}
