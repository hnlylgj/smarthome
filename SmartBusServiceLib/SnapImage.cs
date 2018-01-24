using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmartBusServiceLib
{
    public class SnapImage
    {
        #region ------数据成员-------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        private string _LockID;
        private long _SnapID;
        private int _SnapTypeID;
        private string _SnapUUID;
        private int _ViewFlag;

        private DateTime _CreateTime;
        private MemoryStream _ImageMemStream;
        private byte[] _ImageByteBuffer;
        private string _MenuInfor;


        #endregion ------------------------------------------------------------------------------


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

        public long SnapID
        {
            get
            {
                return _SnapID;
            }
            set
            {
                _SnapID = value;
            }
        }
        public int SnapTypeID
        {
            get
            {
                return _SnapTypeID;
            }
            set
            {
                _SnapTypeID = value;
            }
        }
        public int ViewFlag
        {
            get
            {
                return _ViewFlag;
            }
            set
            {
                _ViewFlag = value;
            }
        }
        public string SnapUUID
        {
            get
            {
                return _SnapUUID;
            }
            set
            {
                _SnapUUID = value;
            }
        }

        public string GetCreateTime
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _CreateTime);
            }

        }


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

        public MemoryStream ImageMemStream
        {
            get
            {
                return _ImageMemStream;
            }
            set
            {
                _ImageMemStream = value;
            }
        }

        public byte[] ImageByteBuffer
        {
            get
            {
                return _ImageByteBuffer;
            }
            set
            {
                _ImageByteBuffer = value;
            }
        }

        public string MenuInfor
        {
            get
            {
                return _MenuInfor;
            }
            set
            {
                _MenuInfor = value;
            }
        }


        #endregion --------------------------------------------------------------

        #region ------构造函数-----------------------------------------------------------

        public SnapImage(string LockID, int SnapTypeID, string SnapUUID, string MenuInfor, MemoryStream ImageMemStream)
        {
            this.LockID = LockID;
            this.SnapUUID = SnapUUID;
            this.SnapTypeID = SnapTypeID;
            this.MenuInfor = MenuInfor;
            this.ImageMemStream = ImageMemStream;
            this.CreateTime = DateTime.Now;

        }
        public SnapImage()
        {


        }

        #endregion ----------------------------------------------------------------------


    }
}
