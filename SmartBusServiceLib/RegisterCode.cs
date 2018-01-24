using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
    public class RegisterCode
    {
        #region ------数据成员-------------------------------------------------------------------

        private int _ManagerID;
        private string _LockID;
        private string _RegisterCodeStr;
        private DateTime _Date;
        #endregion ------------------------------------------------------------------------------

         #region ------智能字段-------------------------------------------------------------------

        public int ManagerID
          {
              get
              {
                  return _ManagerID;
              }
              set
              {
                  _ManagerID = value;
              }
          }

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
        public string RegisterCodeStr
          {
              get
              {
                  return _RegisterCodeStr;
              }
              set
              {
                  _RegisterCodeStr = value;
              }
          }
       

          public string GetCreateTime
          {
              get
              {
                  return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _Date);
              }

          }



          public DateTime Date
          {
              get
              {
                  return _Date;
              }
              set
              {
                  _Date = value;
              }
          }

         


          #endregion --------------------------------------------------------------
       
        #region ------构造函数-----------------------------------------------------------

          public RegisterCode(int ManagerID, string RegisterCodeStr, string LockID)
        {
            this.ManagerID = ManagerID;
            this.LockID = LockID;
            this.RegisterCodeStr = RegisterCodeStr;
            this.Date = DateTime.Now;
          
        }
          public RegisterCode()
        {
            

        }
       
        #endregion ----------------------------------------------------------------------


    }
}
