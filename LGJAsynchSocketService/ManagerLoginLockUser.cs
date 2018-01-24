using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace LGJAsynchSocketService
{
    public  class ManagerLoginLockUser:ManagerSocketLoginUser
    {

        int LockConnectCount;
        public List<LockServerLib.LongFileReceiveProc> MyLongFileReceiveProcList = new List<LockServerLib.LongFileReceiveProc>();

        public ManagerLoginLockUser(): base()
        {


        }
        protected override void StartCleanGarbageChannel()
        {
            MyTimeSpanValue = 70;
            FindGarbageChannel MyFindGarbageChannel = new FindGarbageChannel(MyTimeSpanValue);
            //LoginUser MyLoginUser = MyLoginUserList.Find(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            List<LoginUser> MyLoginGarbageList = MyLoginUserList.FindAll(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            foreach (LoginUser MyLoginUser in MyLoginGarbageList)
            {
                if (MyLoginUser.ChannelStatus == 1 && MyLoginUser.LoginID > 1)//未认证与响应通道
                {
                    MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                    MyLoginUserList.Remove(MyLoginUser);

                }


            }


        }


        public LockServerLib.LongFileReceiveProc FindLongFileReceiveProcList(ref TcpClient MyTcpClient)
        {
            try
            {
                for (int i = 0; i < MyLongFileReceiveProcList.Count; i++)
                {

                    if (MyLongFileReceiveProcList[i].SocketInfor == MyTcpClient.Client.RemoteEndPoint.ToString())
                    {

                        return MyLongFileReceiveProcList[i];

                    }
                }

            }
            catch (Exception ex)
            {

            }

            return null;
        }
        public int AuthChannel(ref SocketServiceReadWriteChannel MyReadWriteChannel)
        {
            int ReturnCode = 1;
            try
            {
                for (int i = 0; i < MyLongFileReceiveProcList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString())
                    {

                        ReturnCode = MyLoginUserList[i].ChannelStatus;
                      

                    }
                }

            }
            catch (Exception ex)
            {

            }

            return ReturnCode;
        }

        ///override//
        public  void CRUDLoginUserList( ref SocketServiceReadWriteChannel MyReadWriteChannel, int CRUDFlag)
         {
             try
             {
                 //Monitor.Enter(MyLoginUserList);
                 switch (CRUDFlag)
                 {
                     case 0:// ★New Add ★
                         LockConnectCount++;
                         NewLoginUserObj = new LoginUser();
                         if (LockConnectCount == 1)
                         {
                             NewLoginUserObj.LockID = "***************";
                             NewLoginUserObj.MobileID = "***************";
                             NewLoginUserObj.ChannelStatus = 0;
                         }
                         //NewLoginUserObj.LoginID =CreateLoginID();//CurrentMaxLoginID;// MyLoginUserList.Count + 1;
                         NewLoginUserObj.SocketInfor = MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString();
                         //NewLoginUserObj.MyReadWriteSocketChannel = MyReadWriteChannel;
                         MyLoginUserList.Add(NewLoginUserObj);


                         DisplayResultInfor(4, "");
                         //DisplayResultInfor(0, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已连接!");

                         DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", LockConnectCount));


                         break;
                           
                     case 1:
                         for (int i = 0; i < MyLoginUserList.Count; i++)
                         {
                             //if (MyLoginUserList[i].GetRemoteEndIP == MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString())
                             if (MyLoginUserList[i].SocketInfor == MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString())
                             {

                                 MyLoginUserList[i].SetKeepTime = DateTime.Now;
                                 MyLoginUserList[i].WorkCountSum = MyLoginUserList[i].WorkCountSum + 1;
                                 break;
                             }

                         }


                         DisplayResultInfor(4, "");
                         DisplayResultInfor(0, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：正在收发数据!");

                         // MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                         // MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：正在收发数据!"); 
                         //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                         //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：提交业务!");
                         break;

                     case 2:
                         for (int i = 0; i < MyLoginUserList.Count; i++)
                         {
                             //if (MyLoginUserList[i].GetRemoteEndIP == MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString())
                             if (MyLoginUserList[i].SocketInfor == MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString())
                             {

                                 MyLoginUserList.RemoveAt(i);
                                 //break;
                             }

                         }

                         DisplayResultInfor(4, "");
                         DisplayResultInfor(0, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已断开连接!");

                         //MyCoudLockSeverMainForm.InvokeLoginUserRefresh ();
                         ///MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已断开连接!"); 
                         //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                         //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "已断开连接!");

                         break;



                     default:
                         break;


                 }
             }
             catch (Exception InforEx)
             {
                 DisplayResultInfor(1, string.Format("增加Socket连接错误[{0}]", InforEx.Message));
             }
             finally
             {

                 ;//Monitor.Exit(MyLoginUserList);

             }


         }


        /*
        public override  void CRUDLoginUserListForLogin(ref TcpClient MyTcpClientObj, string InLockIDStr, string InMobileIDStr)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);
                //LoginUser MyLoginUserObj=new LoginUser();
                int nCount=-1;;
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                    {
                        nCount=i;
                        MyLoginUserList[i].LoginID = CreateLoginID();
                        MyLoginUserList[i].SetKeepTime = DateTime.Now;
                        MyLoginUserList[i].LockID = InLockIDStr;
                        MyLoginUserList[i].MobileID = InMobileIDStr;
                        MyLongFileReceiveProcList.Add(new LockServerLib.LongFileReceiveProc(MyTcpClientObj.Client.RemoteEndPoint.ToString(), InLockIDStr, InMobileIDStr));
                      
                        break;
                    }

                }

                if(nCount>-1)
                {
                               
                   DisplayResultInfor(4, "");
                   //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                   DisplayResultInfor(1, string.Format("锁通道端[{0}]注册[LockID：{1}][MobileID：{2}]", MyLoginUserList[nCount].SocketInfor, MyLoginUserList[nCount].LockID, MyLoginUserList[nCount].MobileID));
                }
                else
                {
                    DisplayResultInfor(1, "注册错误!");

                }
            }
            catch (Exception ex)
            {
                ;
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }


        }
        */

       
        public void CRUDLoginUserListForLoginEx(ref TcpClient MyTcpClientObj, string InLockIDStr, string InMobileIDStr)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);
                int TempCount = LockConnectCount;
                //--1.删除现有残余垃圾连接通道-------------------------------------------------------------------------------------------
                /*
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].LockID == InLockIDStr && MyLoginUserList[i].MobileID == InMobileIDStr)
                    {
                        MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                        MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id
                        MyLoginUserList.RemoveAt(i);
                        LockConnectCount--;


                    }

                }
                 * */
                //---注册新连接通道---------------------------------------------------------------------------------------------------
                int nCount = -1; ;
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                    {
                        nCount = i;
                        MyLoginUserList[i].LoginID = CreateLoginID();
                        MyLoginUserList[i].SetKeepTime = DateTime.Now;
                        MyLoginUserList[i].LockID = InLockIDStr;
                        MyLoginUserList[i].MobileID = InMobileIDStr;
                        MyLongFileReceiveProcList.Add(new LockServerLib.LongFileReceiveProc(MyTcpClientObj.Client.RemoteEndPoint.ToString(), InLockIDStr, InMobileIDStr));//建立图像流通道！

                        //break;
                    }

                }

                if (nCount > -1)
                {

                    DisplayResultInfor(4, "");
                    DisplayResultInfor(1, string.Format("锁通道端[{0}]成功注册[LockID：{1}][MobileID：{2}]", MyLoginUserList[nCount].SocketInfor, MyLoginUserList[nCount].LockID, MyLoginUserList[nCount].MobileID));
                    if (TempCount != LockConnectCount)
                    {
                        DisplayResultInfor(1, string.Format("当前锁端清除残余垃圾连接通道后连接数[{0}]", LockConnectCount));
                    }
                }
                else
                {
                    DisplayResultInfor(1, string.Format("没有找到注册通道错误[{0}][{1}]", InLockIDStr, InMobileIDStr));

                }
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("注册通道错误[{0}]-[{1}]", InLockIDStr, InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }


        }

        public int CRUDLoginUserListForLoginMobileEx(string InLockIDStr, string InMobileIDStr)
        {
                      
            int ReturnCodeID = -1;
            try
            {
                Monitor.Enter(MyLoginUserList);
                //--1.-------------------------------------------------
                
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].LockID == InLockIDStr && MyLoginUserList[i].MobileID == InMobileIDStr)
                    {
                         ReturnCodeID = i;
                     }

                }
                if (ReturnCodeID > -1)
                {
                    DisplayResultInfor(1, string.Format("查找通道成功[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }
                else
                {

                    DisplayResultInfor(1, string.Format("查找通道失败[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }
                           
               
             }
            catch (Exception InforEx)
             {
                 DisplayResultInfor(1, string.Format("查找通道错误[{0}]", InforEx.Message));
             }
             finally
             {
            
                 Monitor.Exit(MyLoginUserList);
                 
             }
            
          
            return ReturnCodeID;
           
        }

        public  int CRUDLoginUserListForMobileFind(string InLockIDStr, string InMobileIDStr)
        {

            int ReturnCodeID = -1;
            try
            {
                Monitor.Enter(MyLoginUserList);
                //--1.-------------------------------------------------

                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].LockID == InLockIDStr && MyLoginUserList[i].MobileID == InMobileIDStr)
                    {
                        ReturnCodeID = i;
                       
                    }

                }

                if (ReturnCodeID > -1)
                {
                    DisplayResultInfor(1, string.Format("查找通道成功[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }
                else
                {

                    DisplayResultInfor(1, string.Format("查找通道失败[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }


            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("查找通道错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);


            }
            return ReturnCodeID;

        }

        public SocketServiceReadWriteChannel CRUDLoginUserListForMobileFindEx(string InLockIDStr, string InMobileIDStr)
        {

            int ReturnCodeID = -1;
            //SocketServiceReadWriteChannel MyReadWriteSocketChannel;
            try
            {
                Monitor.Enter(MyLoginUserList);
                //--1.-------------------------------------------------

                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].LockID == InLockIDStr && MyLoginUserList[i].MobileID == InMobileIDStr)
                    {
                        ReturnCodeID = i;

                    }

                }
                if (ReturnCodeID > -1)
                {
                    DisplayResultInfor(1, string.Format("查找通道成功[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }
                else
                {

                    DisplayResultInfor(1, string.Format("查找通道失败[{0}]--[{1}][{2}]", ReturnCodeID, InLockIDStr, InMobileIDStr));
                }
           
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("查找通道错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);


            }
            if (ReturnCodeID > 0)
            {
                return MyLoginUserList[ReturnCodeID].MyReadWriteSocketChannel;
            }
            else
            {
                return null;

            }

        }
        

  
        /*
        public override void CRUDLoginUserList(ref TcpClient MyTcpClientObj, int CRUDFlag)
        {
            try
            {
                //Monitor.Enter(MyLoginUserList);
                switch (CRUDFlag)
                {

                        case 0://****New Add***

                        //NewLoginUserObj = new LoginUser(ref MyTcpClientObj);
                        //NewLoginUserObj.LoginID = MyLoginUserList.Count + 1;
                        //MyLoginUserList.Add(NewLoginUserObj);

                        //DisplayResultInfor(4, "");
                        //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!");
                      
                        break;

                       case 1: //Update通道活动时间
                        for (int i = 0; i < MyLoginUserList.Count; i++)
                        {
                            
                            if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                            {

                                MyLoginUserList[i].SetKeepTime = DateTime.Now;
                                MyLoginUserList[i].WorkCountSum = MyLoginUserList[i].WorkCountSum + 1;
                                break;
                            }

                        }

                        DisplayResultInfor(4, "");
                        DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                       
                        break;

                    case 2://主动Close Socket

                        //--1.--------------------------------------------------------------------------------------------
                        string TempMobileIDString = null;
                        string TempLockIDString = null;
                        //LoginUser BindedLoginUser = null;
                        string InSocketInfor = MyTcpClientObj.Client.RemoteEndPoint.ToString();
                        //---1.---------------------------------------------------------------------------------------------
                        for (int i = 0; i < MyLoginUserList.Count; i++)
                        {

                            if (MyLoginUserList[i].SocketInfor == InSocketInfor)
                            {
                                //MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                                MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id
                               
                                TempMobileIDString = MyLoginUserList[i].MobileID;
                                TempLockIDString = MyLoginUserList[i].LockID;
                                MyLoginUserList.RemoveAt(i);
                                

                                break;

                            }

                        }
                        //--2.-------------------------------------------------------------------------------------
                        for (int i = 0; i < MyLongFileReceiveProcList.Count; i++)
                        {

                            if (MyLongFileReceiveProcList[i].SocketInfor == InSocketInfor)
                            {
                               
                                MyLongFileReceiveProcList.RemoveAt(i);
                                
                                break;

                            }

                        }

                        if (TempMobileIDString != null)
                            OffLineResponseToMobile(TempMobileIDString, TempLockIDString);

                        //-------------------------------------------------------------------------------------------------
                        DisplayResultInfor(4, "");
                        //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!");
                        DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", MyLoginUserList.Count-1));
                        break;

                        case 3:

                        if (MyLoginUserList.Count > 0)
                        {
                            for (int i = 0; i < MyLoginUserList.Count; i++)
                            {

                                MyTimeSpan = DateTime.Now.Subtract(MyLoginUserList[i].KeepTime);
                                if (MyTimeSpan.Seconds > 5)
                                {

                                    MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                                    MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);
                                    MyLoginUserList.RemoveAt(i);

                                }


                            }
                            //-----------------------------------------------------------------
                            for (int i = 0; i < MyLongFileReceiveProcList.Count; i++)
                            {

                                if (MyLongFileReceiveProcList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                                {

                                    MyLongFileReceiveProcList.RemoveAt(i);


                                    break;

                                }

                            }
                            //-----------------------------------------------------------------
                        }

                        DisplayResultInfor(4, "");
                        //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已自动断开连接!");
                        DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", MyLoginUserList.Count - 1));
                     ;
                        break;
                  
                        default:
                        break;
  

                }
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("断开连接错误[{0}]", InforEx.Message));
            }
            finally
            {

                ;//Monitor.Exit(MyLoginUserList);

            }


        }
        */
        public void CRUDLoginUserListForCreate(SocketServiceReadWriteChannel MyReadWriteChannel)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);
                  //★New Add ★               
                NewLoginUserObj = new LoginUser();
                //NewLoginUserObj.LoginID =CreateLoginID();//CurrentMaxLoginID;// MyLoginUserList.Count + 1;
                NewLoginUserObj.SocketInfor = MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString();
                NewLoginUserObj.MyReadWriteSocketChannel = MyReadWriteChannel;
                MyLoginUserList.Add(NewLoginUserObj);

                DisplayResultInfor(4, "");
                DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", MyLoginUserList.Count - 1));
               

                   
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("增加连接错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }


        }

        public new void CRUDLoginUserListForDelete(ref TcpClient MyTcpClientObj)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);
                 //--1.--------------------------------------------------------------------------------------------
                 string TempMobileIDString = null;
                 string TempLockIDString = null;
                 //LoginUser BindedLoginUser = null;
                 string InSocketInfor = MyTcpClientObj.Client.RemoteEndPoint.ToString();
                 //---1.---------------------------------------------------------------------------------------------
                  for (int i = 0; i < MyLoginUserList.Count; i++)
                  {

                    if (MyLoginUserList[i].SocketInfor == InSocketInfor)
                    {
                        MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                        MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id

                        TempMobileIDString = MyLoginUserList[i].MobileID;
                        TempLockIDString = MyLoginUserList[i].LockID;
                        MyLoginUserList.RemoveAt(i);
                                
                        break;

                    }

                  }
                    //--2.-------------------------------------------------------------------------------------
                for (int i = 0; i < MyLongFileReceiveProcList.Count; i++)
                {

                    if (MyLongFileReceiveProcList[i].SocketInfor == InSocketInfor)
                    {

                        MyLongFileReceiveProcList.RemoveAt(i);
                        break;

                    }

                }

                if (TempMobileIDString != null)
                    OffLineResponseToMobile(TempMobileIDString, TempLockIDString);

                //-------------------------------------------------------------------------------------------------
                DisplayResultInfor(4, "");
                //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!");
                LockConnectCount--;
                DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", LockConnectCount));
                       
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("断开连接错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }


        }
        
        public  void RemoveNotLoginUser(ref TcpClient MyTcpClientObj)
        {
            try
            {
                //Monitor.Enter(MyLoginUserList);
                string TempSocketInfor = MyTcpClientObj.Client.RemoteEndPoint.ToString();
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == TempSocketInfor)
                    {
                        //TempString = MyLoginUserList[i].SocketInfor;
                        MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                        MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id
                        MyLoginUserList.RemoveAt(i);
                        LockConnectCount--;
                        break;

                    }

                }

                //-------------------------------------------------------------------------------------------------
                DisplayResultInfor(4, "");
                //DisplayResultInfor(1, TempSocketInfor + "：因没有注册已断开连接!");
                DisplayResultInfor(1, string.Format("当前锁端连接数[{0}]", LockConnectCount));
                     
                    
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("断开连接错误[{0}]", InforEx.Message));
            }
            finally
            {

                ;//Monitor.Exit(MyLoginUserList);

            }


        }
        
        private void OffLineResponseToMobile(string InTempMobileIDString, string InTempLockIDString)
        {

            string CommandMessageStr = "lockoutoff";
            //---1.找智能锁本身通道的路由表记录-------------------------------------
             //FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
            //LoginUser MyLoginUser = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));

            //string MessageMobileStr = "";         
            //string MyLockIDStr = InTempLockIDString;
            //string MyMobileIDStr = InBindedLoginUser.MobileID;

            CommandMessageStr = CommandMessageStr + "#" + InTempMobileIDString + "-" + InTempLockIDString + "#["  +"howlock"+","+ GetDateTimeWeekIndex() + "]!";
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

                MyAsynchSendMessageCallback(MyLoginUserList[0].MyReadWriteSocketChannel, MySendMessageBytes);

            }
            catch
            {

                DisplayResultInfor(1, "转发移动端通道错误");
            }

     /*
             * try
            {
                FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MessageMobileIDStr);
                this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;

                MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

            }
            catch
            {

            }
            */

            //MyAsynchSendMessageCallback(InBindedLoginUser.MyReadWriteSocketChannel, MySendMessageBytes);


            //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
            //MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

            //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
            //FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MessageMobileStr);
            //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;


            //--再转发出去-----------
            //MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

        }

        private void OnLineResponseToMobile()
        {
            /*
            string CommandMessageStr = "locklogin";
            //---1.找智能锁本身通道的路由表记录-------------------------------------
            //FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
            //LoginUser MyLoginUser = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
            string MessageMobileStr = "";// MyLoginUser.MobileID;
            string MessageLockStr = "";// MyLoginUser.LockID;

            CommandMessageStr = CommandMessageStr + "#" + MessageLockStr + "!";
            byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

            int nBuffersLenght = CommandMessageStr.Length;
            byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


            MySendMessageBytes[2] = 250;

            //填充
            for (int i = 0; i < nBuffersLenght; i++)
            {

                MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

            }

            //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
            //MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

            //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
            //FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MessageMobileStr);
            //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;


            //--再转发出去-----------
            //MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
            */
        }

        private string GetDateTimeWeekIndex()
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
