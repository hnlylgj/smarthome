using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace SmartBusServiceLib
{
     [DataContract(Namespace = "http://hnlylgj/HomeCloud/MessageBusService/2012/10/LockKey")]
     public class LockKey
    {
        #region ------数据成员-------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        private string _LockID;
        private int _LockKeyID;
        private string _OwerName;
        private string _KeyString;
        private DateTime _CreateTime;
        private string _KeyDateStr;
       
        #endregion ------------------------------------------------------------------------------


        #region ------智能字段-------------------------------------------------------------------

          [DataMember]
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
          [DataMember]
        public int LockKeyID
        {
            get
            {
                return _LockKeyID;
            }
            set
            {
                _LockKeyID = value;
            }
        }
        public string GetLockKeyID
        {
            get
            {
                return string.Format("{0:D3}", _LockKeyID);
            }

        }
          [DataMember]
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

        public string GetCreateTime
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _CreateTime);
            }

        }

        public DateTime SetCreateTime
        {

            set
            {
                _CreateTime = value;
            }
        }
          [DataMember]
        public DateTime CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                _CreateTime = value;
            }
        }

        public string KeyString
        {
            get
            {
                return _KeyString;
            }
            set
            {
                _KeyString = value;
            }
        }

        public string KeyDateStr
        {
            get
            {
                return _KeyDateStr;
            }
            set
            {
                _KeyDateStr = value;
            }
        }


        #endregion --------------------------------------------------------------

        #region ------构造函数-----------------------------------------------------------

        public LockKey(string LockID, int LockKeyID, string OwerName, string KeyString)
        {
            this.LockID = LockID;
            this.LockKeyID = LockKeyID;
            this.OwerName = OwerName;
            this.KeyString = KeyString;
            this.CreateTime = DateTime.Now;

        }
        public LockKey()
        {


        }

        #endregion ----------------------------------------------------------------------
    }
}
