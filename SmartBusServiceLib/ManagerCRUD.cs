using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SmartBusServiceLib
{
     public class ManagerCRUD
    {
        string ConnectionString;      
        public ManagerCRUD()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";

            ConnectionString = HelpTool.ConnectionString;

        }
        public ManagerCRUD(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }

        public List<Manager> LoadAllManager()
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

        public Manager FindManager(string InLoginName)
        {
             //HelpTool.MySqlConnection;
            SqlConnection MySqlConnection =new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Manager where LoginName='" + InLoginName + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Manager AnyManager = null;

            while (MySqlDataReader.Read())
            {

                AnyManager = new Manager();

                AnyManager.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyManager.Name = (string)MySqlDataReader["Name"];
                AnyManager.LoginName = (string)MySqlDataReader["LoginName"];
                AnyManager.PassWord = (string)MySqlDataReader["PassWord"];
                AnyManager.RightType = (int)MySqlDataReader["RightType"];
                AnyManager.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
               

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyManager;



        }

        public Manager FindManager(string InLoginName, string InConnectionString)
        {

         
            ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Manager where LoginName='" + InLoginName + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Manager AnyManager = null;

            while (MySqlDataReader.Read())
            {

                AnyManager = new Manager();

                AnyManager.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyManager.Name = (string)MySqlDataReader["Name"];
                AnyManager.LoginName = (string)MySqlDataReader["LoginName"];
                AnyManager.PassWord = (string)MySqlDataReader["PassWord"];
                AnyManager.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
                AnyManager.RightType = (int)MySqlDataReader["RightType"];

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyManager;

        }

        public void UpdateManager(Manager MyManager)
        {

            /*
            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();

            SqlCommand updatecmd = new SqlCommand("UPDATE Products set ProductName=@ProductName,CategoryID=@CategoryID,Price=@Price,InStore=@InStore,Description=@Description where ProductID=@ProductID", conn);

            updatecmd.Parameters.Add(new SqlParameter("@ProductName", pro.ProductName));

            updatecmd.Parameters.Add(new SqlParameter("CategoryID", pro.CategoryID));

            updatecmd.Parameters.Add(new SqlParameter("@Price", pro.Price));

            updatecmd.Parameters.Add(new SqlParameter("@InStore", pro.InStore));

            updatecmd.Parameters.Add(new SqlParameter("@Description", pro.Description));

            updatecmd.Parameters.Add(new SqlParameter("@ProductID", pro.ProductID));

            conn.Open();

            updatecmd.ExecuteNonQuery();

            conn.Close();
           */

        }

        public void DeleteManager(Manager MyManager)
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

        public void InserManager(Manager MyManager)
        {
             /*
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
            */

        }
        
    }
}
