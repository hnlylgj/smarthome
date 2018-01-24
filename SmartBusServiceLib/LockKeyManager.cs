using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SmartBusServiceLib
{
    public class LockKeyManager
    {
        string ConnectionString;
        SqlConnection MySqlConnection;
        public LockKeyManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;

        }
        public LockKeyManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
        public LockKeyManager(SqlConnection InSqlConnection)
        {
            this.MySqlConnection = InSqlConnection;
        }

        public List<LockKey> LoadAllLockKey()
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

        public List<LockKey> LoadAllLockKey(string LockIDStr)
        {

            List<LockKey> MyAllLockKey = new List<LockKey>();
       //ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
         

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            //string commandText = "select * from "+MySQLViewName;    
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MySQLViewName = "key" + LockIDStr;
              
            string commandText = "select * from  " + MySQLViewName + "  where StateID=0 ORDER BY Number DESC";

            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {

                LockKey AnyLockKey = new LockKey();
                AnyLockKey.LockID = (string)MySqlDataReader["LockID"];

                AnyLockKey.LockKeyID = (int)MySqlDataReader["Number"];

                AnyLockKey.OwerName = (string)MySqlDataReader["Name"];

                AnyLockKey.CreateTime = (DateTime)MySqlDataReader["Date"];

                AnyLockKey.KeyString = (string)MySqlDataReader["KeyStr"];

                MyAllLockKey.Add(AnyLockKey);

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllLockKey;



        }

        public List<LockKey> LoadAllLockKey(string LockIDStr, string InConnectionString)
        {

            List<LockKey> MyAllLockKey = new List<LockKey>();
            ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            string MySQLViewName = "key" + LockIDStr;
            string commandText = "select * from  " + MySQLViewName + "  where StateID=0 ORDER BY Number DESC";

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

        }

        public LockKey FindOneLockKey(string LockID, int Number)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MySQLViewName = "key" + LockID;
            //string MyCommandText = "select LockID,Number, Name, Date, KeyDateStr from  " + MySQLViewName + "  where StateID=0 and Number=@Number ORDER BY Number DESC";
            string MyCommandText = "select LockID,Number, Name, Date, KeyDateStr from  " + MySQLViewName + "  where StateID=0 and Number=@Number";
            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", Number));

            MySqlConnection.Open();
            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();
            LockKey AnyLockKey = null;
            while (MySqlDataReader.Read())
            {
                AnyLockKey = new LockKey();
                AnyLockKey.LockID = (string)MySqlDataReader["LockID"];
                AnyLockKey.LockKeyID = (int)MySqlDataReader["Number"];
                AnyLockKey.KeyDateStr = (string)MySqlDataReader["KeyDateStr"];
                break;

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyLockKey;



        }

        public void UpdateLockKey(LockKey MyLockKey)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MyCommandText = "UPDATE LockKey set StateID=0 where LockID=@LockID and Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();


        }

        public void DeleteLockKey(LockKey MyLockKey)
        {

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //--切底删除-----------
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MySQLViewName = "key" + MyLockKey.LockID;
            string MyCommandText = "delete from  " + MySQLViewName + "  where Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();


            //---采用标志删除---------------------------------------------------------------
            /*
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string MySQLViewName = "key" + MyLockKey.LockID;
            //string MyCommandText = "UPDATE  "+MySQLViewName+"  set StateID=-1 where LockID=@LockID and Number=@Number";
            string MyCommandText = "UPDATE LockKey set StateID=-1 where LockID=@LockID and Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();
             * */


        }

        public void DeleteLockKeyEx(LockKey MyLockKey)
        {

            //---采用标志删除---------------------------------------------------------------
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string MySQLViewName = "key" + MyLockKey.LockID;
            //string MyCommandText = "UPDATE  "+MySQLViewName+"  set StateID=-1 where LockID=@LockID and Number=@Number";
            string MyCommandText = "UPDATE LockKey set StateID=-1 where LockID=@LockID and Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlConnection.Open();
            MySqlCommand.ExecuteNonQuery();
            MySqlConnection.Close();


        }

        public void ShareXDeleteLockKeyEx(LockKey MyLockKey)
        {

            //---采用标志删除---------------------------------------------------------------
            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            //MySqlConnection.Open();
            //string MySQLViewName = "key" + MyLockKey.LockID;
            //string MyCommandText = "UPDATE  "+MySQLViewName+"  set StateID=-1 where LockID=@LockID and Number=@Number";

            string MyCommandText = "UPDATE LockKey set StateID=-1 where LockID=@LockID and Number=@Number";
            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlCommand.ExecuteNonQuery();

            //MySqlConnection.Close();


        }

        public void InsertLockKey(LockKey MyLockKey)
        {

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MySQLViewName = "key" + MyLockKey.LockID;
            string MyCommandText = "insert into " + MySQLViewName + " (LockID,Number, Name, KeyStr, Date) values (@LockID,@Number, @Name, @KeyStr, @Date)";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@KeyStr", MyLockKey.KeyString));
            MySqlCommand.Parameters.Add(new SqlParameter("@Date", MyLockKey.GetCreateTime));


            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();

        }
        public int InsertLockKeyEx(LockKey MyLockKey)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("AddLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
            MySqlCommand.Parameters.Add(new SqlParameter("@KeyDateStr", MyLockKey.KeyDateStr));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnNumber", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnNumber"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            int ReturnNumber = (int)MySqlCommand.Parameters["@ReturnNumber"].Value;
            MySqlConnection.Close();
            return ReturnNumber;

        }

        public int ShareXInsertLockKeyEx(LockKey MyLockKey)
        {

            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("AddLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
            MySqlCommand.Parameters.Add(new SqlParameter("@KeyDateStr", MyLockKey.KeyDateStr));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnNumber", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnNumber"].Direction = ParameterDirection.Output;

            //MySqlConnection.Open();

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            int ReturnNumber = (int)MySqlCommand.Parameters["@ReturnNumber"].Value;
            //MySqlConnection.Close();
            return ReturnNumber;

        }

        public int SynchAddLockKey(LockKey MyLockKey)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("SynchAddLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
            MySqlCommand.Parameters.Add(new SqlParameter("@CreateDate", MyLockKey.CreateTime));
            MySqlCommand.Parameters.Add(new SqlParameter("@KeyDateStr", MyLockKey.KeyDateStr));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnNumber", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnNumber"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            int ReturnNumber = (int)MySqlCommand.Parameters["@ReturnNumber"].Value;
            MySqlConnection.Close();
            return ReturnValue;

        }

        public int ShareXSynchAddLockKey(LockKey MyLockKey)
        {

            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("SynchAddLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
            MySqlCommand.Parameters.Add(new SqlParameter("@CreateDate", MyLockKey.CreateTime));
            MySqlCommand.Parameters.Add(new SqlParameter("@KeyDateStr", MyLockKey.KeyDateStr));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnNumber", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnNumber"].Direction = ParameterDirection.Output;

            //MySqlConnection.Open();

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            int ReturnNumber = (int)MySqlCommand.Parameters["@ReturnNumber"].Value;
            //MySqlConnection.Close();
            return ReturnValue;

        }

        public int ClearLockKey(string LockID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("ClearLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();
            MySqlCommand.ExecuteNonQuery();
            int ReturnValue = 1;
            ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            MySqlConnection.Close();
            return ReturnValue;//0:成功，其他失败

            //---采用标志删除---------------------------------------------------------------
            /*
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string MySQLViewName = "key" + MyLockKey.LockID;
            //string MyCommandText = "UPDATE  "+MySQLViewName+"  set StateID=-1 where LockID=@LockID and Number=@Number";
            string MyCommandText = "UPDATE LockKey set StateID=-1 where LockID=@LockID and Number=@Number";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();
             * */


        }

        public int ShareXClearLockKey(string LockID)
        {

            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("ClearLockKey", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

            //MySqlConnection.Open();
            MySqlCommand.ExecuteNonQuery();
            int ReturnValue = 1;
            ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            //MySqlConnection.Close();
            return ReturnValue;//0:成功，其他失败




        }


    }
}
