using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
     public class Manager
    {
        #region ------数据成员-------------------------------------------------------------------


        private int _ManagerID;
        private string _Name;
        private string _LoginName;
        private string _PassWord;
        private DateTime _CreateTime;
        private int _RightType;
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

          public string Name
          {
              get
              {
                  return _Name;
              }
              set
              {
                  _Name = value;
              }
          }
          public string LoginName
          {
              get
              {
                  return _LoginName;
              }
              set
              {
                  _LoginName = value;
              }
          }
          public string PassWord
          {
              get
              {
                  return _PassWord;;
              }
              set
              {
                  _PassWord = value;
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


          public int RightType
          {
              get
              {
                  return _RightType;
              }
              set
              {
                  _RightType = value;
              }
          }

          #endregion --------------------------------------------------------------
       
        #region ------构造函数-----------------------------------------------------------

          public Manager(int ManagerID, string Name, string LoginName, string PassWord)
        {
            this._ManagerID = ManagerID;
            this.Name = Name;
            this.LoginName = LoginName;
            this.PassWord = PassWord;
            this.CreateTime = DateTime.Now;
          
        }
          public Manager()
        {
            

        }
       
        #endregion ----------------------------------------------------------------------


    }
}
