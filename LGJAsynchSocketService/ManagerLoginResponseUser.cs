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
    public class ManagerLoginResponseUser : ManagerSocketLoginUser
    {
        public void AddLoginUserToList(SocketServiceReadWriteChannel MeSocketServiceReadWriteChannel)
        {
            //try
            //{
                //Monitor.Enter(MyLoginUserList);


                        NewLoginUserObj = new LoginUser(1,ref MeSocketServiceReadWriteChannel);
                        NewLoginUserObj.LoginID = MyLoginUserList.Count + 1;
                        MyLoginUserList.Add(NewLoginUserObj);

                        DisplayResultInfor(4, "");
                        DisplayResultInfor(0, MeSocketServiceReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString() + "：已连接!");
                      

                //Monitor.Exit(MyLoginUserList);

           // }


        }

    }
}
