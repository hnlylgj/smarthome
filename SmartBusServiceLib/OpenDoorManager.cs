using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SmartBusServiceLib
{
     [System.ComponentModel.DataObjectAttribute]
     public class OpenDoorManager
    {
         string ConnectionString;
         SqlConnection MySqlConnection;
          public OpenDoorManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            this.ConnectionString = HelpTool.ConnectionString;
          
          }
          public OpenDoorManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
          public OpenDoorManager(SqlConnection InSqlConnection)
        {
            this.MySqlConnection = InSqlConnection;
        }

          public List<OpenDoor> FindOpenDoorBase(string LockID)
          {

              List<OpenDoor> MyAllOpenDoor = new List<OpenDoor>();
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

              string MyCommandText = "select * from  OpenDoor where LockID='" + LockID + "'";

              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlConnection.Open();

              SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

              OpenDoor AnyOpenDoor = null;

              while (MySqlDataReader.Read())
              {

                  AnyOpenDoor = new OpenDoor();
                  AnyOpenDoor.LockID = (string)MySqlDataReader["LockID"];
                  AnyOpenDoor.OwerName = (string)MySqlDataReader["Name"];
                  AnyOpenDoor.KeyID = (int)MySqlDataReader["KeyID"];
                  AnyOpenDoor.OpenDate = (DateTime)MySqlDataReader["CreateDate"];

                  MyAllOpenDoor.Add(AnyOpenDoor);


              }

              MySqlDataReader.Close();

              MySqlConnection.Close();

              return MyAllOpenDoor;



          }

          public List<string> FindKeyOwer(string LockID)
          {

              List<string> MyAllKeyOwerName = new List<string>();
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

              string MyCommandText = "select Name from  LockKey where LockID='" + LockID + "'" + " and StateID=0";

              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlConnection.Open();

              SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();           

              while (MySqlDataReader.Read())   
              {

                  string  OwerName = (string)MySqlDataReader["Name"];
                  MyAllKeyOwerName.Add(OwerName);

              }

              MySqlDataReader.Close();

              MySqlConnection.Close();

              return MyAllKeyOwerName;



          }

          [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
          public List<LockKey> FindKeyOwerEx(string LockID)
          {

              List<LockKey> MyAllKeyOwerName = new List<LockKey>();
              MyAllKeyOwerName.Add(new LockKey(null,0, "全部",null)); 
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

              //string MyCommandText = "select Number,Name from  LockKey where LockID='" + LockID + "'" + " and StateID=0";
              string MySQLViewName = "key" + LockID;
              string MyCommandText = "select Number,Name from  " + MySQLViewName + "  where StateID=0 ORDER BY Number DESC";

              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlConnection.Open();
            
              SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

              while (MySqlDataReader.Read())
              {

                  LockKey AnyLockKey = new LockKey();
                  AnyLockKey.LockKeyID = (int)MySqlDataReader["Number"];
                  AnyLockKey.OwerName = (string)MySqlDataReader["Name"];

                  MyAllKeyOwerName.Add(AnyLockKey);

              }

              MySqlDataReader.Close();

              MySqlConnection.Close();

              return MyAllKeyOwerName;



          }

          public List<OpenDoor> FindOpenDoor(string LockID, string InConnectionString)
          {

              ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
              return FindOpenDoorBase(LockID);

              /*
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

              string MyCommandText = "select * from Lock where LockID='" + LockID + "'";

              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlConnection.Open();

              SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

              Lock AnyLock = null;

              while (MySqlDataReader.Read())
              {

                  AnyLock = new Lock();

                  AnyLock.LockID = (string)MySqlDataReader["LockID "];
                  AnyLock.Status = (int)MySqlDataReader["Status"];



              }

              MySqlDataReader.Close();

              MySqlConnection.Close();

              return AnyLock;
              */

          }

         [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
          public List<OpenDoor> FindOpenDoorEx(string LockID, string InConnectionString, string Name, string TimeRange)
          {

              List<OpenDoor> MyAllOpenDoor = new List<OpenDoor>();
              if (InConnectionString != null)
                  ConnectionString = InConnectionString;
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

              string MyNameFilter = "";
              string MyDateFilter ="";
              string MyAllFilter = "";

              if(Name=="全部")
              {
                  MyNameFilter = "";
              }
              else
              {
                  MyNameFilter = " AND Name='" + Name + "' ";

              }
              //--2.-------------------------------------------------
              string NowDateStr = DateTime.Now.ToString();
              NowDateStr = NowDateStr.Substring(0, NowDateStr.IndexOf(" "));
              string AddSubNowDateStr = NowDateStr.Replace("-", "");

              //Select DATEADD(DAY,1,'20130101')            
              if (TimeRange == "0")//当天
              {
                  MyDateFilter = " and CreateDate>='" + NowDateStr + "'";
              }

              if (TimeRange == "1")//7天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-7,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;
              }
              if (TimeRange == "2")//10天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-10,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;
              }
              if (TimeRange == "3")//30天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-30,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;
              }
              if (TimeRange == "4")//2*30天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-60,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;
              }
              if (TimeRange == "5")//3*30天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-90,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;
              }
              if (TimeRange == "6")//6*30天之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-180,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;

              }
              if (TimeRange == "7")//一年之内
              {
                  AddSubNowDateStr = "(Select DATEADD(DAY,-365,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;

              }
              if (TimeRange == "8")//三年之内
              {
                  //string TempDateStr = "-1095";
                  //AddSubNowDateStr = "(Select DATEADD(DAY,"+TempDateStr+",'" + AddSubNowDateStr + "')) ";
                  AddSubNowDateStr = "(Select DATEADD(DAY,-1095,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;

              }

              if (TimeRange == "9")//五年之内
              {
                    //string TempDateStr = "-1825";
                  //AddSubNowDateStr = "(Select DATEADD(DAY," + TempDateStr + ",'" + AddSubNowDateStr + "')) ";
                  AddSubNowDateStr = "(Select DATEADD(DAY,-1825,'" + AddSubNowDateStr + "')) ";
                  MyDateFilter = " and CreateDate>=" + AddSubNowDateStr;

              }

              if (TimeRange == "10")//全部
              {
                  MyDateFilter = "";
                 
              }
              //===========================================================================================
              if (MyNameFilter == "" && MyDateFilter == "")
              {
                  MyAllFilter = "";
              }
              else
              {
                 
                  MyAllFilter = MyNameFilter + MyDateFilter;
                   /*
                 if (MyNameFilter == "")
                 {
                     MyAllFilter = MyAllFilter.Trim();
                     MyAllFilter = MyAllFilter.Substring(3); //去掉“and”；
                 }
                  * */
              }


              //string MyCommandText = "select * from  OpenDoor where LockID='" + LockID + "'";
              //string MyCommandText = "select Name,KeyID,CreateDate from OpenDoor  where  " + MyNameFilter + MyDateFilter + " ORDER BY CreateDate DESC";
              string MyCommandText = "select Name,KeyID,CreateDate from OpenDoor  where  LockID=" + "'" + LockID +"'"+ MyAllFilter + " ORDER BY CreateDate DESC";
                        
              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlConnection.Open();

              SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

              OpenDoor AnyOpenDoor = null;

              while (MySqlDataReader.Read())
              {

                  AnyOpenDoor = new OpenDoor();
                  //AnyOpenDoor.LockID = (string)MySqlDataReader["LockID"];
                  AnyOpenDoor.OwerName = (string)MySqlDataReader["Name"];
                  AnyOpenDoor.KeyID = (int)MySqlDataReader["KeyID"];
                  AnyOpenDoor.OpenDate = (DateTime)MySqlDataReader["CreateDate"];

                  MyAllOpenDoor.Add(AnyOpenDoor);

              }

              MySqlDataReader.Close();

              MySqlConnection.Close();

              return MyAllOpenDoor;



          }

         public int InsertOpenDoor(OpenDoor MyOpenDoor)
          {
                          
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
              string MyCommandText = "insert into OpenDoor (LockID,KeyID,CreateDate,DateStr) values (@LockID,@KeyID,@CreateDate,@DateStr)";
              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

              MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyOpenDoor.LockID));
              MySqlCommand.Parameters.Add(new SqlParameter("@KeyID", MyOpenDoor.KeyID));
              MySqlCommand.Parameters.Add(new SqlParameter("@CreateDate", MyOpenDoor.OpenDate));
              MySqlCommand.Parameters.Add(new SqlParameter("@DateStr", MyOpenDoor.OpenDateStr));
              MySqlConnection.Open();
              int RowCount = MySqlCommand.ExecuteNonQuery();
              MySqlConnection.Close();
              return RowCount;

          }

          public void InsertOpenDoorEx(List<OpenDoor> MyNewListOpen)
          {
              int Count = MyNewListOpen.Count;               
              SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
              string MyCommandText = "insert into OpenDoor (LockID,KeyID,CreateDate,DateStr) values (@LockID,@KeyID,@CreateDate,@DateStr)";
              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
              MySqlConnection.Open();
              for (int i = 0; i < Count; i++)
              {

                  MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyNewListOpen[i].LockID));
                  MySqlCommand.Parameters.Add(new SqlParameter("@KeyID", MyNewListOpen[i].KeyID));
                  MySqlCommand.Parameters.Add(new SqlParameter("@CreateDate", MyNewListOpen[i].OpenDate));
                  MySqlCommand.Parameters.Add(new SqlParameter("@DateStr", MyNewListOpen[i].OpenDateStr));
                  MySqlCommand.ExecuteNonQuery();
                  MySqlCommand.Parameters.Clear(); 

              }

              MySqlConnection.Close();

          }

          public void ShareXInsertOpenDoorEx(List<OpenDoor> MyNewListOpen)
          {
              int Count = MyNewListOpen.Count;
               //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
              string MyCommandText = "insert into OpenDoor (LockID,KeyID,CreateDate,DateStr) values (@LockID,@KeyID,@CreateDate,@DateStr)";
              SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
                  //MySqlConnection.Open();
              for (int i = 0; i < Count; i++)
              {

                  MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyNewListOpen[i].LockID));
                  MySqlCommand.Parameters.Add(new SqlParameter("@KeyID", MyNewListOpen[i].KeyID));
                  MySqlCommand.Parameters.Add(new SqlParameter("@CreateDate", MyNewListOpen[i].OpenDate));
                  MySqlCommand.Parameters.Add(new SqlParameter("@DateStr", MyNewListOpen[i].OpenDateStr));
                  MySqlCommand.ExecuteNonQuery();
                  MySqlCommand.Parameters.Clear();

              }

               //MySqlConnection.Close();

          }

    }
}
