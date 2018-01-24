using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
     public class Channel
    {
        #region ------数据成员-------------------------------------------------------------------

         private int _ChannelID;
         private string _LockID;
         private string _MobileID;
         private string _RegisterCodeStr;
         private int _CustomerID;
         private DateTime _CreateTime;
        

        #endregion ------------------------------------------------------------------------------

         #region ------智能字段-------------------------------------------------------------------

         public int ChannelID
         {
             get
             {
                 return _ChannelID;
             }
             set
             {
                 _ChannelID = value;
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
         public string MobileID
         {
             get
             {
                 return _MobileID;
             }
             set
             {
                 _MobileID = value;
             }
         }
         public int CustomerID
         {
             get
             {
                 return _CustomerID; ;
             }
             set
             {
                 _CustomerID = value;
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

         #endregion ----------------------------------------------------------------------

         #region ------构造函数-----------------------------------------------------------

       
        public Channel()
        {
            

        }

        public Channel(int ChannelID, string LockID, string MobileID, int CustomerID)
        {
            this.ChannelID = ChannelID;
            this.LockID = LockID;
            this.MobileID = MobileID;
            this.CustomerID = CustomerID;
            this.CreateTime = DateTime.Now;

        }
       
         #endregion ----------------------------------------------------------------------


    }
}
