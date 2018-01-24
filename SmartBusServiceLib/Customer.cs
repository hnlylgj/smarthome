using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
    
    public class Customer
    {
        #region ------数据成员-------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
          private int _CustomerID;
          private string _CustomerName;
          private string _LoginName;
          private string _MobileID;
          private string _Password;
          private DateTime _CreateTime;
          private string _PersonID;
          private string _TeleCode;
          private string _EMail;
          private string _Address;
          private string _BirthDate;
         #endregion ------------------------------------------------------------------------------

          #region ------智能字段-------------------------------------------------------------------

          public int CustomerID
          {
              get
              {
                  return _CustomerID;
              }
              set
              {
                  _CustomerID = value;
              }
          }

          public string CustomerName
          {
              get
              {
                  return _CustomerName;
              }
              set
              {
                  _CustomerName = value;
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
         
        public string Password
          {
              get
              {
                  return _Password;;
              }
              set
              {
                  _Password = value;
              }
          }

          public string MobileID
          {
              get
              {
                  return _MobileID ;
              }
              set
              {
                  _MobileID = value;
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

          public string PersonID
          {
              get
              {
                  return _PersonID;
              }
              set
              {
                  _PersonID = value;
              }
          }

          public string TeleCode
          {
              get
              {
                  return _TeleCode;
              }
              set
              {
                  _TeleCode = value;
              }
          }

          public string EMail
          {
              get
              {
                  return _EMail;
              }
              set
              {
                  _EMail = value;
              }
          }

          public string Address
          {
              get
              {
                  return _Address;
              }
              set
              {
                  _Address = value;
              }
          }

          public string BirthDate
          {
              get
              {
                  return _BirthDate;
              }
              set
              {
                  _BirthDate = value;
              }
          }

          #endregion --------------------------------------------------------------
       
          #region ------构造函数-----------------------------------------------------------

          public Customer(int CustomerID, string CustomerName, string LoginName, string Password)
        {
            this.CustomerID = CustomerID;
            this.CustomerName = CustomerName;
            this.LoginName = LoginName;
            this.Password = Password;
            this.CreateTime = DateTime.Now;
          
        }

          public Customer(int CustomerID, string CustomerName, string LoginName, string Password, string PersonID, string TeleCode, string EMail, string Address)
          {
              this.CustomerID = CustomerID;
              this.CustomerName = CustomerName;
              this.LoginName = LoginName;
              this.Password = Password;
              this.CreateTime = DateTime.Now;

              this.PersonID = PersonID;
              this.TeleCode = TeleCode;
              this.EMail = EMail;
              this.Address = Address;

          }


        public Customer()
        {
            

        }
       
        #endregion ----------------------------------------------------------------------


    }
}
