using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers; 
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace LGJAsynchSocketService
{
    public class ManagerSocketLoginUser
    {
       
        public List<LoginUser> MyLoginUserList = new List<LoginUser>(); 
          
        protected  LoginUser NewLoginUserObj;
        private int CurrentMaxLoginID;
        private int[] LeaveLoginIDArray;
        protected List<int> MyLeaveLoginIDList = new List<int>();

        //-----------------------------------------------------------------
        private System.Timers.Timer SmartAutoUpdateTimer;
        private int AutoUpdateTime = 20000;  //3000：3秒；60000：1分钟
        private double AutoUpdateTimeD;   // (double)AutoUpdateTime;

        protected TimeSpan MyTimeSpan;
        protected Thread SmartAutoUpdateThread;
        protected int MyTimeSpanValue;
        //--------------------------------------------------------------------
        public delegate void AsynchRefreshNotifyHandler(string MessageStr, int MessageID);
        public AsynchRefreshNotifyHandler MyRefreshNotifyCallBack;

        public delegate void AsynchSendMessageCallback(SocketServiceReadWriteChannel MyReadWriteObject, byte[] SendMessageBuffers);
        public AsynchSendMessageCallback MyAsynchSendMessageCallback;

        public delegate void AsynchCRUBListForCloseCallback();
        public AsynchCRUBListForCloseCallback DoAsynchCRUBListForCloseCallback;

        //============================================================================================================================
        public ManagerSocketLoginUser()
          {
              CurrentMaxLoginID = 0;
              LeaveLoginIDArray = new int[100];
              //DoAsynchCRUBListForCloseCallback = new AsynchCRUBListForCloseCallback(this.CRUDLoginUserListForClose);
              MyTimeSpanValue = 15;
              InitSmartAutoDetect();

            

          }

        public void InitSmartAutoDetect()
        {
            //---Timer-Thread------------------------
            AutoUpdateTimeD = (double)AutoUpdateTime;
            SmartAutoUpdateTimer = new System.Timers.Timer(AutoUpdateTimeD);
            SmartAutoUpdateTimer.Elapsed += new ElapsedEventHandler(this.SmartAutoUpdateTimer_Elapsed);
            SmartAutoUpdateTimer.Enabled = true;//开启定时检测机制
            //----Thread-----------------------------
            //SmartAutoUpdateThread = new Thread(new ThreadStart(this.ThreadSmartAutoUpdateList));
        }

        public void SetAutoDetectMode(bool IsModeID)
        {
            //---****Timer--Thread*****---------------------

            if (IsModeID)//--true--
            {
                SmartAutoUpdateTimer.Enabled = true; 
                SmartAutoUpdateThread.Abort();
                SmartAutoUpdateThread.Join();

            }
            else   //--false--
            {

                SmartAutoUpdateTimer.Enabled = false;
                SmartAutoUpdateThread.Start();
               

            }
        }

        #region ------Socket-Update-------------------------------------------------------------------
        protected int CreateLoginID()
        {
           
            CurrentMaxLoginID++;
            int LeaveLoginID;
            if (CurrentMaxLoginID > 100)
            {

                   CurrentMaxLoginID = 100;
                   if (MyLeaveLoginIDList.Count > 0)
                    {
                        LeaveLoginID = MyLeaveLoginIDList[0];
                        MyLeaveLoginIDList.RemoveAt(0);
                        return LeaveLoginID;

                    }
                    else
                    {
                       CurrentMaxLoginID=0;//无法继续承载
                       return 0;

                    }

                
                
            }
            else
            {
               
                return CurrentMaxLoginID;
            }



        }


        protected void DisplayResultInfor(int nFlagID, string Str)
        {
            if (MyRefreshNotifyCallBack != null)
                MyRefreshNotifyCallBack(Str, nFlagID);

          
         }

        public void CRUDLoginUserList(ref TcpClient MyTcpClientObj, uint ChannelFlag)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);

                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                    {

                        MyLoginUserList[i].ChannelStatus = (int)ChannelFlag;
                        break;

                    }

                }
                //DisplayResultInfor(4, "");
                //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                DisplayResultInfor(1, string.Format("[{0}]非法连接传输数据即将关闭", MyTcpClientObj.Client.RemoteEndPoint));
                //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBackCallback(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");






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
    
        /*
        public virtual void CRUDLoginUserList(SocketServiceReadWriteChannel MyReadWriteChannel, int CRUDFlag)
        {
            try
            {
                //Monitor.Enter(MyLoginUserList);
                switch (CRUDFlag)
                {
                    case 0:// ★New Add ★

                        NewLoginUserObj = new LoginUser();
                        //NewLoginUserObj.LoginID =CreateLoginID();//CurrentMaxLoginID;// MyLoginUserList.Count + 1;
                        NewLoginUserObj.SocketInfor = MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString();
                        NewLoginUserObj.MyReadWriteSocketChannel = MyReadWriteChannel;
                        MyLoginUserList.Add(NewLoginUserObj);

                        DisplayResultInfor(4, "");
                        DisplayResultInfor(0, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已连接!");


                        //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                        //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已连接!"); 

                        //MyCoudLockSeverMainForm.MyAsynchAppendClientEndPoint(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString());
                        //MyCoudLockSeverMainForm.MyRefreshDataGridCallBack();
                        //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                        //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "已登录连接!");

                        break;

                    case 1:

                    /*
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
                         **/


        // MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
        //  MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：正在收发数据!"); 
        //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
        //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：提交业务!");

        /*
        break;

    case 2:

    /*
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
         * */

        //  MyCoudLockSeverMainForm.InvokeLoginUserRefresh ();
        ///  MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已断开连接!"); 
        //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
        //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "已断开连接!");
        /* 

       break;

    default:
       break;


    }
    }
    catch (Exception InforEx)
    {
    DisplayResultInfor(1, string.Format("增加Socket-User连接错误[{0}]", InforEx.Message));
    }
    finally
    {

    ;//Monitor.Exit(MyLoginUserList);

    }


    }

    */

        /*
    public virtual void CRUDLoginUserList(ref TcpClient MyTcpClientObj, int CRUDFlag)
    {

try
{
    Monitor.Enter(MyLoginUserList);

    switch (CRUDFlag)
    {

         case 0://****New Add***

            NewLoginUserObj = new LoginUser(ref MyTcpClientObj);
            NewLoginUserObj.LoginID = MyLoginUserList.Count + 1;
            MyLoginUserList.Add(NewLoginUserObj);

            DisplayResultInfor(4, "");
            DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!");
             //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
           //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!"); 

            //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
            //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已登录连接!");
            break;

          case 1: //Update通道活动时间
            for (int i = 0; i < MyLoginUserList.Count; i++)
            {
                //if (MyLoginUserList[i].GetRemoteEndIP == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                {

                    MyLoginUserList[i].SetKeepTime = DateTime.Now;
                    MyLoginUserList[i].WorkCountSum = MyLoginUserList[i].WorkCountSum + 1;
                    break;
                }

            }

             DisplayResultInfor(4, "");
             DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
             //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
             //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!"); 

            //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
            //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在提交业务!");
            break;

        case 2://主动Close Socket

            //--1.--------------------------------------------------------------------------------------------
            string TempMobileIDString=null;
            string TempLockIDString = null;
            //LoginUser BindedLoginUser = null;
            for (int i = 0; i < MyLoginUserList.Count; i++)
            {

               if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                {
                   MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                   MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id
                   /*
                   if (MyLoginUserList[i].LockID != "*********")//Lock Close
                   {
                       TempMobileIDString = MyLoginUserList[i].MobileID;
                       TempLockIDString = MyLoginUserList[i].LockID;
                   }
                   else
                   {
                       TempMobileIDString = MyLoginUserList[i].MobileID;//Mobile Close
                       TempLockIDString = null;
                   }
                    * */
        //TempMobileIDString = MyLoginUserList[i].MobileID;
        // TempLockIDString = MyLoginUserList[i].LockID;
        // MyLoginUserList.RemoveAt(i);


        // break;

        //  }

        // }
        //  */    /*
        /* 
      if (TempLockIDString == null) //---2.如果是移动端关闭则删除绑定的云锁端移动ID---
      {
          for (int i = 0; i < MyLoginUserList.Count; i++)
          {

              if (MyLoginUserList[i].MobileID == TempMobileIDString)
              {
                  MyLoginUserList[i].MobileID = null; break;

              }

          }
      }
      else //---2.如果是云锁端关闭则向绑定的移动端移发送离线通知消息---
      {
          LoginUser BindedLoginUser = null;

          for (int i = 0; i < MyLoginUserList.Count; i++)
          {

              if (MyLoginUserList[i].MobileID == TempMobileIDString)
              {
                  BindedLoginUser = MyLoginUserList[i]; break;
              }

          }


          //---3.发送离线通知消息----------------------------------------------------------------------------

     // }
         */
        // if (TempMobileIDString != null)
        // OffLineResponseToMobile(TempMobileIDString, TempLockIDString);     

        //-------------------------------------------------------------------------------------------------
        // DisplayResultInfor(4, "");
        // DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!");
        // MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
        // MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!"); 

        //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
        //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已主动断开连接!");
        //break;
        /*
         case 3:

             if (MyLoginUserList.Count > 0)
             {
                 for (int i = 0; i < MyLoginUserList.Count; i++)
                 {

                     MyTimeSpan = DateTime.Now.Subtract(MyLoginUserList[i].KeepTime);
                     if (MyTimeSpan.Seconds > 5)
                     {

                         //MyLoginUserList[i].MyTCPClient.Client.Close();
                         //MyLoginUserList[i].MyTCPClient.Close();
                         MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                         MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);
                         MyLoginUserList.RemoveAt(i);

                     }


                 }

             }

             DisplayResultInfor(4, "");
             DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已自动断开连接!");

                  //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
            // MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已自动断开连接!"); 
             //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
             //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已自动断开连接!");
             break;
         default:
             break;


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


        /*
        public virtual void CRUDLoginUserListForLogin(ref TcpClient MyTcpClientObj, string InLockIDStr, string InMobileIDStr)
        {

            try
            {
                Monitor.Enter(MyLoginUserList);

                    //Update LockID--MobileID

                    for (int i = 0; i < MyLoginUserList.Count; i++)
                    {

                        if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                        {
                            MyLoginUserList[i].LoginID = CreateLoginID();
                            MyLoginUserList[i].SetKeepTime = DateTime.Now;
                            MyLoginUserList[i].LockID = InLockIDStr;
                            MyLoginUserList[i].MobileID = InMobileIDStr;
                            //MyLoginUserList[i].MyLongFileReceiveProc.MyLockIDStr = InLockIDStr;
                            MyLongFileReceiveProcList.Add(new LockServerLib.LongFileReceiveProc(MyTcpClientObj.Client.RemoteEndPoint.ToString(), InLockIDStr, InMobileIDStr)); 

                            break;
                        }

                    }

                       //////*
                    foreach (LoginUser AnyLoginUser in MyLoginUserList)
                    {

                        if (AnyLoginUser.SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                        {

                            AnyLoginUser.SetKeepTime = DateTime.Now;
                            AnyLoginUser.LockID = InLockIDStr;
                            AnyLoginUser.MobileID = InMobileIDStr;
                            break;
                        }
                    }
                 //////*/
        /*
        DisplayResultInfor(4, "");
         DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");


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


        /*
        public void CRUDLoginUserListForDelete(ref TcpClient MyTcpClientObj)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);            

                string TempMobileIDString = null;
                string TempLockIDString = null;
                string TempSocketInforStr = null;
                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                    {
                        MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                        MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);
                        TempSocketInforStr = MyLoginUserList[i].SocketInfor;
                        /*
                        if (MyLoginUserList[i].LockID != "*********")//Lock Close
                        {
                            TempMobileIDString = MyLoginUserList[i].MobileID;
                            TempLockIDString = MyLoginUserList[i].LockID;
                        }
                        else
                        {
                            TempMobileIDString = MyLoginUserList[i].MobileID;//Mobile Close
                            TempLockIDString = null;
                        }
                        */
        //MyLoginUserList.RemoveAt(i);
        //break;

        //}

        //}

        /*
     if (TempLockIDString == null) //---2.如果是移动端关闭则删除绑定的云锁端移动ID---
     {
         for (int i = 0; i < MyLoginUserList.Count; i++)
         {

             if (MyLoginUserList[i].MobileID == TempMobileIDString)
             {
                 MyLoginUserList[i].MobileID = null; break;

             }

         }
     }
     else //---2.如果是云锁端关闭则向绑定的移动端移发送离线通知消息---
     {
         LoginUser BindedLoginUser = null;

         for (int i = 0; i < MyLoginUserList.Count; i++)
         {

             if (MyLoginUserList[i].MobileID == TempMobileIDString)
             {
                 BindedLoginUser = MyLoginUserList[i]; break;
             }

         }


         //---3.发送离线通知消息----------------------------------------------------------------------------
         if (BindedLoginUser != null)
             OffLineResponseToMobile(BindedLoginUser, TempLockIDString);
     }

       ////*/

        /*
         //-------------------------------------------------------------------------------------------------
         DisplayResultInfor(4, "");
         DisplayResultInfor(1, TempSocketInforStr);
         DisplayResultInfor(0, TempSocketInforStr + "：已主动断开连接!");


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
        /*
        public void CRUDLoginUserList(ref TcpClient MyTcpClientObj, string InMobileLockIDStr, int CRUDFlag)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);

                switch (CRUDFlag)
                {

                    case 0://Update LockID

                        for (int i = 0; i < MyLoginUserList.Count; i++)
                        {
                           
                            if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                            {

                                MyLoginUserList[i].SetKeepTime = DateTime.Now;
                                MyLoginUserList[i].LockID=InMobileLockIDStr;
                               
                            }

                        }

                         DisplayResultInfor(4, "");
                         DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                        //  MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                        //  MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!"); 
                        break;

                    case 1: //Update MobileID
                       
                            for (int i = 0; i < MyLoginUserList.Count; i++)
                            {
                                
                                if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                                {
                                   
                                    MyLoginUserList[i].SetKeepTime = DateTime.Now;
                                    MyLoginUserList[i].MobileID = InMobileLockIDStr;
                                   
                                }

                            }
                    

                         DisplayResultInfor(4, "");
                         DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                           //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                        //MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");

                        
                        break;





                    case 2:
                        
                        break;

                    case 3:

                        
                        break;

                    default:
                        break;


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
        /*
      public virtual int CRUDLoginUserListForLoginMobile(ref TcpClient MyTcpClientObj, string InLockIDStr, string InMobileIDStr)
      {
          return 0;

          int ReturnCodeID = 1;
          try
          {
              Monitor.Enter(MyLoginUserList);
              //--1.-------------------------------------------------

              //--2.-----------------------------------------------------

              for (int i = 0; i < MyLoginUserList.Count; i++)
              {

                  if (MyLoginUserList[i].LockID == InLockIDStr)
                  {
                      //MyLoginUserList[i].MobileID = InMobileIDStr;
                      ReturnCodeID = 0;
                   }

              }

              DisplayResultInfor(4, "");
              DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：移动端登录!");

                /*
              switch (CRUDFlag)
              {

                  case 0://****New Add***

                      NewLoginUserObj = new LoginUser(ref MyTcpClientObj);
                      NewLoginUserObj.LoginID = MyLoginUserList.Count + 1;
                      MyLoginUserList.Add(NewLoginUserObj);

                      DisplayResultInfor(4, "");
                      DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!");
                      //  MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                      //  MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!"); 

                      //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                      //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已登录连接!");
                      break;

                  case 1://Update通道活动时间
                      for (int i = 0; i < MyLoginUserList.Count; i++)
                      {
                          //if (MyLoginUserList[i].GetRemoteEndIP == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                          if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                          {

                              MyLoginUserList[i].SetKeepTime = DateTime.Now;
                              MyLoginUserList[i].WorkCountSum = MyLoginUserList[i].WorkCountSum + 1;
                              break;
                          }

                      }


                      DisplayResultInfor(4, "");
                      DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                      //  MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                      //  MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!"); 

                      //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                      //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在提交业务!");
                      break;





                  case 2:
                      for (int i = 0; i < MyLoginUserList.Count; i++)
                      {
                          // if (MyLoginUserList[i].GetRemoteEndIP == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                          if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                          {
                              MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);
                              MyLoginUserList.RemoveAt(i);

                              //break;
                          }

                      }

                      DisplayResultInfor(4, "");
                      DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!");
                      // MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                      // MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!"); 

                      //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                      //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已主动断开连接!");
                      break;

                  case 3:

                      if (MyLoginUserList.Count > 0)
                      {
                          for (int i = 0; i < MyLoginUserList.Count; i++)
                          {

                              MyTimeSpan = DateTime.Now.Subtract(MyLoginUserList[i].KeepTime);
                              if (MyTimeSpan.Seconds > 5)
                              {

                                  //MyLoginUserList[i].MyTCPClient.Client.Close();
                                  //MyLoginUserList[i].MyTCPClient.Close();
                                  MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                                  MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);
                                  MyLoginUserList.RemoveAt(i);

                              }


                          }

                      }

                      DisplayResultInfor(4, "");
                      DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已自动断开连接!");

                      //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                      // MyCoudLockSeverMainForm.MyRefreshPrimeNotifyIconCallBack(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已自动断开连接!"); 
                      //DataGridLoginUser.Invoke(this.MyRefreshDataGridCallBack);
                      //MyRefreshPrimeNotifyIconCallBackCallback.Invoke(MyTcpClientObj.Client.RemoteEndPoint.ToString() + "已自动断开连接!");
                      break;
                  default:
                      break;


              }
               * */
        // }
        // catch (Exception ex)
        // {
        //     ;
        // }
        // finally
        // {
        //
        ///        Monitor.Exit(MyLoginUserList);
        //     
        //
        //   }
        // return ReturnCodeID;
        ///
        //}

        /*
       public SocketServiceReadWriteChannel CRUDLoginUserList(ref TcpClient MyTcpClientObj)
       {
           try
           {
                   for (int i = 0; i < MyLoginUserList.Count; i++)
                   {

                       if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                       {

                           return MyLoginUserList[i].MyReadWriteSocketChannel;

                       }
                    }

           }
           catch (Exception ex)
           {

           }

           return null;
       }

       public LoginUser FindLoginUserList(ref TcpClient MyTcpClientObj)
       {
           try
           {
               for (int i = 0; i < MyLoginUserList.Count; i++)
               {

                   if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                   {

                       return MyLoginUserList[i];

                   }
               }

           }
           catch (Exception ex)
           {

           }

           return null;
       }
        */
        /*
         public void CRUDLoginUserListForClose()
         {
             try
             {
                 Monitor.Enter(MyLoginUserList);
                 /*
                 foreach (LoginUser AnyLoginUser in MyLoginUserList)
                 {
                     AnyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Close();
                     string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                     DisplayResultInfor(0, AnyLoginUser.SocketInfor + "：服务器已断开其连接!");
                     DisplayResultInfor(1, string.Format(MyTimeMarker + "服务器已断开{0}连接（15）\r\n", AnyLoginUser.SocketInfor));
                 }
                 */

        /*
               if(MyLoginUserList.Count>0)
               {

                 for (int i = 0; i < MyLoginUserList.Count; i++)
                  {
                   //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                      string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                   //DisplayResultInfor(0, MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：服务器已断开自动断开其连接!");
                   DisplayResultInfor(1, string.Format(MyTimeMarker + "服务器已断开[{0}]连接（13）\r\n", MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint));
                   MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                   //MyLoginUserList.RemoveAt(i);
                   //DisplayResultInfor(4, "");

                  }

                 MyLoginUserList.Clear();  

               }
               else                
               {
                 DisplayResultInfor(1, "服务器已断开全部连接!");
               }


               DisplayResultInfor(4, "");
               DisplayResultInfor(1, "服务器已断开全部连接!");
               DisplayResultInfor(0, "服务器已断开全部连接!");
           }
           catch (Exception ex)
           {

           }
           finally
           {
               Monitor.Exit(MyLoginUserList);
           }

       }
       */

        #endregion ------------------------------------------------------------------------------

        #region ------Auto-Update-------------------------------------------------------------------
        private void SmartAutoUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //this.TimerSmartAutoUpdateList();
            //-----------------------
            this.StartCleanGarbageChannel();

        }

        protected virtual void StartCleanGarbageChannel()
        {
            //子类必须实现
            //MyTimeSpanValue = 35;
              /*
            FindGarbageChannel MyFindGarbageChannel = new FindGarbageChannel(MyTimeSpanValue);
            //LoginUser MyLoginUser = MyLoginUserList.Find(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            List<LoginUser> MyLoginGarbageList = MyLoginUserList.FindAll(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            foreach(LoginUser MyLoginUser in MyLoginGarbageList)
            {
                if (MyLoginUser.ChannelStatus == 1 && MyLoginUser.LoginID>1)//未认证与响应通道
                {
                    MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                    MyLoginUserList.Remove(MyLoginUser);   

                }
                          

            }
           */

        }


        public void TimerSmartAutoUpdateList()
        {
            
            try
            {
                Monitor.Enter(MyLoginUserList);
                if (MyLoginUserList.Count > 0)
                {
                    
                    for (int i = 0; i < MyLoginUserList.Count; i++)
                    {
                        //---1.取消无效注册通道-------------------------------------------------------------------------
                        if (MyLoginUserList[i].MobileID == null && MyLoginUserList[i].LockID == null)
                        {


                            //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                            string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                            DisplayResultInfor(0, MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "自动断开连接!");
                            DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}]无效注册自动断开连接（13）\r\n", MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint));
                            MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                            MyLoginUserList.RemoveAt(i);
                            DisplayResultInfor(4, "");
                        }
                    }
                    //---2.-----------------------------------------------------------------------------------------------
                    TimeSpan MyTimeSpan;
                    for (int i = 0; i < MyLoginUserList.Count; i++)
                    {
                        //--1.检测"死长连接“自动注销-------
                        MyTimeSpan = DateTime.Now.Subtract(MyLoginUserList[i].KeepTime);
                        if (MyTimeSpan.Minutes > 36)
                        {


                            //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                            string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                            DisplayResultInfor(0, MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "自动断开连接!");
                            DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}]死连接自动断开连接（14）\r\n", MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint));
                            MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                            MyLoginUserList.RemoveAt(i);
                            DisplayResultInfor(4, "");
                        }


                    }
                    //-------------------------------------------------------------------------------------------------

                }


            }

            catch (Exception ExcepInfor)
            {
                DisplayResultInfor(1, "自动断开连接错误：" + ExcepInfor.Message);
            }
            finally
            {
                Monitor.Exit(MyLoginUserList);
            }

          


        }

        private void TimerSmartAutoUpdateList2()
        {
            //检测"死长连接“自动注销
            try
            {
                Monitor.Enter(MyLoginUserList);
                if (MyLoginUserList.Count > 0)
                {
                    TimeSpan MyTimeSpan;
                    foreach (LoginUser AnyLoginUser in MyLoginUserList)
                    {

                        MyTimeSpan = DateTime.Now.Subtract(AnyLoginUser.KeepTime);
                        AnyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Close();
                        if (MyTimeSpan.Minutes > 1)
                        {
                            //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + "[" + DateTime.Now.Ticks.ToString() + "]";
                            string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                            DisplayResultInfor(0, AnyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "自动断开连接!");
                            DisplayResultInfor(1, string.Format(MyTimeMarker + "客户端[{0}]自动断开连接（13）\r\n", AnyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint));
                            AnyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                            MyLoginUserList.Remove(AnyLoginUser); 
                            DisplayResultInfor(4, "");
                        }



                    }
                        

                }


            }

            catch (Exception ex)
            {

            }
            finally
            {
                Monitor.Exit(MyLoginUserList);
            }




        }

        private void ThreadSmartAutoUpdateList()
        {
            //刷新显示登录信息(检测线程自动注销) 
            int TreadSleepTime = 100;
            while (true)
            {
                Thread.Sleep(TreadSleepTime); //10秒--10000;1分钟--60000;5分钟--300000;10分钟--600000;
                TimeSpan MyTimeSpan;
                if (MyLoginUserList.Count > 0)
                {
                    for (int i = 0; i < MyLoginUserList.Count; i++)//
                    {

                        MyTimeSpan = DateTime.Now.Subtract(MyLoginUserList[i].KeepTime);
                        if (MyTimeSpan.Seconds > 6) //20分钟
                        {
                            MyLoginUserList.RemoveAt(i);
                            //break;
                        }

                    }
                    DisplayResultInfor(4, "");
                    //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                    //MyCoudLockSeverMainForm.InvokeLoginUserRefresh();
                   // DataGridLoginUser.Invoke(this.RefreshDataGridcallback);

                }
                TreadSleepTime = 10000;
            }


        }

        #endregion ------------------------------------------------------------------------------

        
    }
}
