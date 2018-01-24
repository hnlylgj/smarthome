using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
    public class OpenDoor
    {
        private string _LockID;
        private int _KeyID;
        private string _OwerName;
        private DateTime _OpenDate;
        private string _OpenDateStr;
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

        public int KeyID
        {
            get
            {
                return _KeyID;
            }
            set
            {
                _KeyID = value;
            }
        }
        public string GetLockKeyID
        {
            get
            {
                return string.Format("{0:D2}", _KeyID);
            }

        }
        public string OwerName
        {
            get
            {
                return _OwerName;
            }
            set
            {
                _OwerName = value;
            }
        }
        public string OpenDateStr
        {
            get
            {
                return _OpenDateStr;
            }
            set
            {
                _OpenDateStr = value;
            }
        }

        public string GetOpenTime
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _OpenDate);
            }

        }



        public DateTime OpenDate
        {
            get
            {
                return _OpenDate;
            }
            set
            {
                _OpenDate = value;
            }
        }




        #endregion --------------------------------------------------------------


    }
}
