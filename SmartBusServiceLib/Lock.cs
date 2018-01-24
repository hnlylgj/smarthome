using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
    public class Lock
    {
        private string _LockID;
        private int _Status;


        #region ------智能字段-------------------------------------------------------------------
        public string LockID
        {
            get
            {
                return _LockID;
            }
            set
            {
                _LockID = value;
            }
        }

        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        #endregion ----------------------------------------------------------------------

        #region ------构造函数-----------------------------------------------------------


        public Lock()
        {
            

        }

        public Lock(string LockID, int Status)
        {
          
            this.LockID = LockID;
            this.Status = Status;
          

        }
       
         #endregion ----------------------------------------------------------------------


    }
}
