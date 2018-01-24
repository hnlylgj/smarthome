using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SmartBusServiceLib
{
    public class LockManager
    {
        string ConnectionString;
        SqlConnection MySqlConnection;
        public LockManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            this.ConnectionString = HelpTool.ConnectionString;
        
        }
        public LockManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
       
        public LockManager(SqlConnection InSqlConnection)
        {
            this.MySqlConnection = InSqlConnection;
        }


        public List<Lock> LoadAllLock()
        {
           
            return null;

             /*
           int nCount = 1;
           string NameStr="王大山";
           while (nCount<11)
           {
               string NewNameStr=NameStr+nCount.ToString(); 
               string NewKeyStr="123456";
               LockKey OneLockKey = new LockKey(nCount,NewNameStr,NewKeyStr);
               MyAllLockKey.Add(OneLockKey);
               nCount++;

           }
            
           return MyAllLockKey;
           */


            //-2.-------------------------------------------------------
            /*
            List<LockKey> MyAllLockKey = new List<LockKey>();
            string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            
            string commandText = "select * from LockKey where LockID='89765432BCDA820'";

            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {

                LockKey AnyLockKey = new LockKey();

                AnyLockKey.LockKeyID = (int)MySqlDataReader["Number"];

                AnyLockKey.OwerName = (string)MySqlDataReader["Name"];

                AnyLockKey.CreateTime = (DateTime)MySqlDataReader["Date"];

                AnyLockKey.KeyString = (string)MySqlDataReader["KeyStr"];
                
                MyAllLockKey.Add(AnyLockKey);

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllLockKey;
          */


        }

        public Lock FindLock(string LockID)
        {       
            //HelpTool.MySqlConnection;

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Lock where LockID='" + LockID + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Lock AnyLock = null;

            while (MySqlDataReader.Read())
            {

                AnyLock = new Lock();

                 AnyLock.LockID = (string)MySqlDataReader["LockID"];
                 AnyLock.Status = (int)MySqlDataReader["Status"];
             


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyLock;



        }

        public Lock ShareXFindLock(string LockID)
        {

              //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Lock where LockID='" + LockID + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

             //MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Lock AnyLock = null;

            while (MySqlDataReader.Read())
            {

                AnyLock = new Lock();

                AnyLock.LockID = (string)MySqlDataReader["LockID"];
                AnyLock.Status = (int)MySqlDataReader["Status"];



            }

            MySqlDataReader.Close();

            //MySqlConnection.Close();

            return AnyLock;



        }

        public Lock FindLock(string LockID, string InConnectionString)
        {

         
            ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection =new SqlConnection(ConnectionString);

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




        }

        public int UpdateLock(Lock MyLock)
        {


            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();

            string MyUpdateCMDText = "UPDATE  Lock set Status=@Status where LockID=@LockID";

            SqlCommand MySqlCommand = new SqlCommand(MyUpdateCMDText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Status", MyLock.Status));

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLock.LockID));
            
            int RowCount=MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();

            return RowCount;

        }
       
     
        
        public void DeleteLock(Lock MyLock)
        {
             /*
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MySQLViewName = "key" + MyLockKey.LockID;
            string MyCommandText = "delete from  " + MySQLViewName + "  where Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();
             * */


        }

        public int InsertLock(Lock MyLock)
        {
            
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MyCommandText = "insert into Lock (LockID,Status) values (@LockID,@Status)";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLock.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Status", MyLock.Status));
            MySqlConnection.Open();
            int RowCount = MySqlCommand.ExecuteNonQuery();
            MySqlConnection.Close();
            return RowCount;

        }

        public int InsertLockEx(Lock MyLock, int MyManagerID)
        {

                //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            //string MyCommandText = "insert into Lock (LockID,Status) values (@LockID,@Status)";

            ///SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            //MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLock.LockID));
            //MySqlCommand.Parameters.Add(new SqlParameter("@Status", MyLock.Status));


            //CreateMobileID MyCreateMobileID = new CreateMobileID();

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString); //HelpTool.MySqlConnection;//

            SqlCommand MySqlCommand = new SqlCommand("AddLockForOut", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;


            MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", MyManagerID));
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLock.LockID));
            //MySqlCommand.Parameters.Add(new SqlParameter("@Status", MyLock.Status));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();
            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;//0:true;1:faile
            MySqlConnection.Close();

            return ReturnValue;

        }

        public string UpdateLockForSale(string LockID, int ManagerID)
        {

            RandomRegisterCode MyRandomRegisterCode = new RandomRegisterCode();
            string NewRegisterCode = MyRandomRegisterCode.CreateRegsisterCode();


            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();

            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;

            string MyUpdateCMDText = "UpdateLockForSale";

            SqlCommand MySqlCommand = new SqlCommand(MyUpdateCMDText, MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", ManagerID));
            MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", NewRegisterCode));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;


            int RowCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            MySqlConnection.Close();
            if (ReturnValue == 0)
            {
                return NewRegisterCode;
            }
            else
            {
                return null;
            }



        }



        //===================================================================
        public string FindMasterKey(string LockID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select MasterKey from Lock where LockID='" + LockID + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            string MasterKey=null;

            while (MySqlDataReader.Read())            {

                MasterKey=(string)MySqlDataReader["MasterKey"];              

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MasterKey;



        }

        public int UpdateMasterKey(string LockID,string NewMasterKey)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("UpdateMasterKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;       

           MySqlCommand.Parameters.Add(new SqlParameter("@MasterKey", NewMasterKey));
           MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
           MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
           MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
           MySqlConnection.Open();
           int RowCount = MySqlCommand.ExecuteNonQuery();
           int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;//0:true;1:faile
           MySqlConnection.Close();
           return ReturnValue;

        }

        public int ShareXUpdateMasterKey(string LockID, string NewMasterKey)
        {

             //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            SqlCommand MySqlCommand = new SqlCommand("UpdateMasterKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@MasterKey", NewMasterKey));
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
           
            //MySqlConnection.Open();

            int RowCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;//0:true;1:faile
              //MySqlConnection.Close();
            return ReturnValue;

        }

    }
}
