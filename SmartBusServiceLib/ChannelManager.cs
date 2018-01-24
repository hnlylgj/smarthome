using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SmartBusServiceLib
{
     public class ChannelManager
    {
        string ConnectionString;
        SqlConnection MySqlConnection;
        public ChannelManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;

        }
        public ChannelManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
        public ChannelManager(SqlConnection InSqlConnection)
        {
            this.MySqlConnection = InSqlConnection;
        }
        public List<Channel> LoadAllChannel()
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

        public Channel FindChannel(int CustomerID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;

            string MyCommandText = "select * from Channel where CustomerID=" + CustomerID.ToString();

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

         

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Channel AnyChannel = null;

            while (MySqlDataReader.Read())
            {

                AnyChannel = new Channel();

                AnyChannel.ChannelID = (int)MySqlDataReader["ChannelID"];
                AnyChannel.LockID = (string)MySqlDataReader["LockID"];
                AnyChannel.MobileID = (string)MySqlDataReader["MobileID"];
                AnyChannel.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
                AnyChannel.CustomerID = (int)MySqlDataReader["CustomerID"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyChannel;



        }

        public Channel FindChannel(int CustomerID, string InConnectionString)
        {

         
            ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Channel where CustomerID=" + CustomerID.ToString();

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Channel AnyChannel = null;

            while (MySqlDataReader.Read())
            {

                AnyChannel = new Channel();

                AnyChannel.ChannelID = (int)MySqlDataReader["ChannelID"];
                AnyChannel.LockID = (string)MySqlDataReader["LockID"];
                AnyChannel.MobileID = (string)MySqlDataReader["MobileID"];
                AnyChannel.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
                AnyChannel.CustomerID = (int)MySqlDataReader["CustomerID"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyChannel;



        }

        public Channel ShareXFindChannel(string InLockID)
        {
              //ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Channel where LockID='" + InLockID+"'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

               //MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Channel AnyChannel = null;

            while (MySqlDataReader.Read())
            {

                AnyChannel = new Channel();

                AnyChannel.ChannelID = (int)MySqlDataReader["ChannelID"];
                AnyChannel.LockID = (string)MySqlDataReader["LockID"];
                AnyChannel.MobileID = (string)MySqlDataReader["MobileID"];
                AnyChannel.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
                AnyChannel.CustomerID = (int)MySqlDataReader["CustomerID"];


            }

            MySqlDataReader.Close();

            //MySqlConnection.Close();

            return AnyChannel;



        }
        public Channel FindChannel(string InLockID)
        {
            //ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Channel where LockID='" + InLockID + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Channel AnyChannel = null;

            while (MySqlDataReader.Read())
            {

                AnyChannel = new Channel();

                AnyChannel.ChannelID = (int)MySqlDataReader["ChannelID"];
                AnyChannel.LockID = (string)MySqlDataReader["LockID"];
                AnyChannel.MobileID = (string)MySqlDataReader["MobileID"];
                AnyChannel.CreateTime = (DateTime)MySqlDataReader["CreateTime"];
                AnyChannel.CustomerID = (int)MySqlDataReader["CustomerID"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyChannel;



        }


         public void UpdateChannel(Channel MyChannel)
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

        public void DeleteChannel(Channel MyChannel)
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

        public void InsertChannel(Channel MyChannel)
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
