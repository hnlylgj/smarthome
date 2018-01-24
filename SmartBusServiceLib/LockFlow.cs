using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartBusServiceLib
{
      public class LockFlow
    {
        #region ------数据成员-------------------------------------------------------------------
                 
        private int _FlowID;
        private string _LockID;
        private int _ManagerID;
        private int _OperatorID;
        private DateTime _Date;
        #endregion ------------------------------------------------------------------------------

        #region ------智能字段-------------------------------------------------------------------

        public int FlowID
          {
              get
              {
                  return _FlowID;
              }
              set
              {
                  _FlowID = value;
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
        public int OperatorID
          {
              get
              {
                  return _OperatorID; ;
              }
              set
              {
                  _OperatorID = value;
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

          public LockFlow(int FlowID, int ManagerID, int OperatorID, string LockID)
        {
            this.FlowID = FlowID;
            this.ManagerID = ManagerID;
            this.OperatorID = OperatorID;
            this.LockID = LockID;
            this.Date = DateTime.Now;
          
        }
          public LockFlow()
        {
            

        }
       
        #endregion ----------------------------------------------------------------------

    }
}
