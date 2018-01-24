using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.LGJAynchWork
{
    internal class RequestAttatchment
    {
        public AsynchLockServerSocketService MyAsynchLockServerSocketService;
        public LoginUser MyLoginUser;
        public LiAsyncSendMail MyAsyncLiSendMail;

        public RequestAttatchment(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService, LiAsyncSendMail MeAsyncLiSendMail)
        {
            this.MyLoginUser = MeLoginUser;
            this.MyAsynchLockServerSocketService = MeAsynchLockServerSocketService;
            MyAsyncLiSendMail = MeAsyncLiSendMail;
         }

    }
}
