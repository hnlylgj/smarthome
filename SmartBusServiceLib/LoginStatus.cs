using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
     public class LoginStatus
    {
        #region ------数据成员-------------------------------------------------------------------

         private int _CustomerID;
         private int _LoginFlag;
         private DateTime _CreateDate;
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
         public int LoginFlag
         {
             get
             {
                 return _LoginFlag; ;
             }
             set
             {
                 _LoginFlag = value;
             }
         }

         public string GetCreateTime
         {
             get
             {
                 return string.Format("{0:yyyy-MM-dd HH:mm:ss}", _CreateDate);
             }

         }



         public DateTime CreateDate
         {
             get
             {
                 return _CreateDate;
             }
             set
             {
                 _CreateDate = value;
             }
         }




         #endregion --------------------------------------------------------------




    }
}
