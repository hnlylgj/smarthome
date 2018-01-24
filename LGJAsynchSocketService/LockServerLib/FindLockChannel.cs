using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
    public class FindLockChannel
    {
        private string _LockID;
        private SocketServiceReadWriteChannel _SocketServiceReadWriteChannel;
       
        public FindLockChannel(string LockID)
        {
            this._LockID=LockID;

        }
        public FindLockChannel(SocketServiceReadWriteChannel InSocketServiceReadWriteChannel)
        {
            this._SocketServiceReadWriteChannel = InSocketServiceReadWriteChannel;

        }
        public bool BindedLockChannelForLockID(LoginUser AnyLoginUser)
        {
           
            
            return AnyLoginUser.LockID == this._LockID; 
        }

        public bool BindedLockChannelForSocket(LoginUser AnyLoginUser)
        {
            return true;//AnyLoginUser.MyReadWriteSocketChannel  == this._SocketServiceReadWriteChannel;//对象相等方法无须“流水号”杂项！
        }
                                                                                                   
    }
}
