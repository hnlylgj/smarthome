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
    public class ManagerLoginMobileUser : ManagerSocketLoginUser
    {


        public ManagerLoginMobileUser(): base()
        {


        }
        protected override void StartCleanGarbageChannel()
        {
            MyTimeSpanValue = 90;
            FindGarbageChannel MyFindGarbageChannel = new FindGarbageChannel(MyTimeSpanValue);
            //LoginUser MyLoginUser = MyLoginUserList.Find(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            List<LoginUser> MyLoginGarbageList = MyLoginUserList.FindAll(new Predicate<LoginUser>(MyFindGarbageChannel.SelectGarbageChannel));
            foreach (LoginUser MyLoginUser in MyLoginGarbageList)
            {
                
                MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
                MyLoginUserList.Remove(MyLoginUser);
                


            }


        }

        //============下面是保留代码=================================================================================
        /*
        public override void CRUDLoginUserList(ref TcpClient MyTcpClientObj, int CRUDFlag)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);

                switch (CRUDFlag)
                {
                     
                    case 0://*New Add

                         //NewLoginUserObj = new LoginUser(ref MyTcpClientObj);
                        //NewLoginUserObj.LoginID = MyLoginUserList.Count + 1;
                        //MyLoginUserList.Add(NewLoginUserObj);

                        //DisplayResultInfor(4, "");
                        //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已连接!");
                        break;

                    case 1: //Update通道活动时间
                        for (int i = 0; i < MyLoginUserList.Count; i++)
                        {
                            //if (MyLoginUserList[i].GetRemoteEndIP == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                            if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                            {

                                MyLoginUserList[i].SetKeepTime = DateTime.Now;
                                //MyLoginUserList[i].WorkCountSum = MyLoginUserList[i].WorkCountSum + 1;
                                break;
                            }

                        }

                        DisplayResultInfor(4, "");
                        DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                        break;

                    case 2://主动Close Socket
                     
                        for (int i = 0; i < MyLoginUserList.Count; i++)
                        {

                            if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                            {
                                //MyLoginUserList[i].MyReadWriteSocketChannel.MyTCPClient.Close();
                                MyLeaveLoginIDList.Add(MyLoginUserList[i].LoginID);//回收分配的通道Id                             
                                MyLoginUserList.RemoveAt(i);
                                break;

                            }

                        }
                      

                        //-------------------------------------------------------------------------------------------------
                        DisplayResultInfor(4, "");
                        DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：已主动断开连接!");
                        break;

                    
                    default:
                        break;


                }
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("删除Socket-User连接错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }


        }
         */   
        public int MobileUserrLoginProc(ref TcpClient MyTcpClientObj, string InLockIDStr, string InMobileIDStr)
        {
            int ReturnCode = -1; 
            try
            {
                    Monitor.Enter(MyLoginUserList);
                  
                    for (int i = 0; i < MyLoginUserList.Count; i++)
                    {

                        if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                        {
                            ReturnCode = i;
                            MyLoginUserList[i].LoginID = CreateLoginID();
                            MyLoginUserList[i].SetKeepTime = DateTime.Now;
                            MyLoginUserList[i].LockID = InLockIDStr;
                            MyLoginUserList[i].MobileID = InMobileIDStr;
                           
                            break;
                        }

                    }

                    if (ReturnCode > -1)
                    {

                        DisplayResultInfor(4, "");
                        //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                        DisplayResultInfor(1, string.Format("[{0}]请求注册通道成功[LockID：{1}][MobileID：{2}]", MyLoginUserList[ReturnCode].SocketInfor, MyLoginUserList[ReturnCode].LockID, MyLoginUserList[ReturnCode].MobileID));
                    }
                    else
                    {
                        DisplayResultInfor(1, string.Format("请求注册通道失败[LockID：{0}][MobileID：{1}]", MyLoginUserList[ReturnCode].LockID, MyLoginUserList[ReturnCode].MobileID));

                    }   

               }
              catch (Exception InforEx)
              {
                  DisplayResultInfor(1, string.Format("请求注册通道错误[{0}]", InforEx.Message));
              }
              finally
             {
            
                Monitor.Exit(MyLoginUserList);
            
             }
            return ReturnCode;

        }

        public int MobileUserrLoginProcEx(ref TcpClient MyTcpClientObj, string InLockID, string InMobileID,string MessageID)
        {
            int ReturnCode = -1;
            try
            {
                Monitor.Enter(MyLoginUserList);

                for (int i = 0; i < MyLoginUserList.Count; i++)
                {

                    if (MyLoginUserList[i].SocketInfor == MyTcpClientObj.Client.RemoteEndPoint.ToString())
                    {
                        ReturnCode = i;
                        MyLoginUserList[i].LoginID = CreateLoginID();
                        MyLoginUserList[i].SetKeepTime = DateTime.Now;
                        MyLoginUserList[i].LockID = InLockID;
                        MyLoginUserList[i].MobileID = InMobileID;
                        MyLoginUserList[i].TempString = MessageID;
                        break;
                    }

                }

                if (ReturnCode > -1)
                {

                    DisplayResultInfor(4, "");
                    //DisplayResultInfor(0, MyTcpClientObj.Client.RemoteEndPoint.ToString() + "：正在收发数据!");
                    DisplayResultInfor(1, string.Format("[{0}]注册通道成功[LockID：{1}][MobileID：{2}]", MyLoginUserList[ReturnCode].SocketInfor, MyLoginUserList[ReturnCode].LockID, MyLoginUserList[ReturnCode].MobileID));
                }
                else
                {
                    DisplayResultInfor(1, string.Format("注册通道失败[LockID：{0}][MobileID：{1}]", MyLoginUserList[ReturnCode].LockID, MyLoginUserList[ReturnCode].MobileID));

                }

            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("注册通道错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);

            }
            return ReturnCode;

        }

        public int LoginMobileUserResponseForFind(string InLockIDStr, string InMobileIDStr)
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
                    DisplayResultInfor(1, string.Format("响应查找通道成功[{0}]", ReturnCodeID));
                }
                else
                {
                    DisplayResultInfor(1, string.Format("响应查找通道失败[{0}]", ReturnCodeID));
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

        public SocketServiceReadWriteChannel LoginMobileUserResponseForFindEx(string InLockIDStr, string InMobileIDStr)
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
               
            }
            catch (Exception InforEx)
            {
                DisplayResultInfor(1, string.Format("云锁响应查找通道错误[{0}]", InforEx.Message));
            }
            finally
            {

                Monitor.Exit(MyLoginUserList);


            }
            if (ReturnCodeID > -1)
            {
                DisplayResultInfor(1, string.Format("云锁响应查找通道成功[{0}]", ReturnCodeID));
                return MyLoginUserList[ReturnCodeID].MyReadWriteSocketChannel;
            }
            else
            {
                DisplayResultInfor(1, string.Format("云锁响应查找通道失败[{0}]", ReturnCodeID));
                return null;

            }

        }

    }
}
