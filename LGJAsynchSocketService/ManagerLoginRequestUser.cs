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
    public class ManagerLoginRequestUser : ManagerSocketLoginUser
    {
        /*
        public override void CRUDLoginUserList(ref TcpClient MyTcpClientObj, int CRUDFlag)
        {
            try
            {
                Monitor.Enter(MyLoginUserList);

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
    }
}
