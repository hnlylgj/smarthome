using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.AsynchFileIO
{
    // public
    internal class RequestAttachment
    {
        public AsynchLockServerSocketService MyAsynchLockServerSocketService;
        public LoginUser MyLoginUser;
        public FileStream MyFileStream;
        int MyBufferSize=65536;
        public RequestAttachment(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService ,string FileName,int FlagID)
        {
            this.MyLoginUser = MeLoginUser;
            this.MyAsynchLockServerSocketService = MeAsynchLockServerSocketService;
            if (FlagID == 0)
            {
                MyFileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None, MyBufferSize, true);

            }
            else
            {
                MyFileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None, MyBufferSize, true);
            }
           

        }


    }
}
