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
    public class MessageListManager
    {
        string ConnectionString;  
        public MessageListManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            this.ConnectionString = HelpTool.ConnectionString;
        }
        public MessageListManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
      
        public string FindMessageStr(string LockID,string MessageID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

               //string MySQLViewName = "key" + LockID;
            //string MyCommandText = "select LockID,Number, Name, Date, KeyDateStr from  " + MySQLViewName + "  where StateID=0 and Number=@Number ORDER BY Number DESC";
            string MyCommandText = "select MessageStr from  MessageList  where  LockID=@LockID and MessageID=@MessageID";
            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@MessageID", MessageID));
            MySqlConnection.Open();
            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();
            string MessageStr = null;
            while (MySqlDataReader.Read())
            {
                MessageStr = (string)MySqlDataReader["MessageStr"];
                break;

            }
            MySqlDataReader.Close();

            if (MessageStr != null)
            {
                MyCommandText = "delete from MessageList  where LockID=@LockID and MessageID=@MessageID";
                MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
                MySqlCommand.Parameters.Clear(); 
                MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
                MySqlCommand.Parameters.Add(new SqlParameter("@MessageID", MessageID));
                MySqlCommand.ExecuteNonQuery();
            }

          
            MySqlConnection.Close();
            return MessageStr;



        }

        public void InsertMessageSave(string LockID, string MessageID, string MessageStr)
        {
            //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            //SqlConnection MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();

            SqlCommand MySqlCommand = new SqlCommand("AddMessage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@MessageID";
            parm.Value = MessageID;
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@MessageStr";
            parm.Value = MessageStr;
            MySqlCommand.Parameters.Add(parm);

            //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            //MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            //MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            int MyCount = MySqlCommand.ExecuteNonQuery();
            MySqlConnection.Close();

          




        }

        public void InsertMessageSaveEx(string LockID, string MessageID, string MessageStr)
        {
            
            //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            //SqlConnection MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);

            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
            
            MySqlConnection.Open();

            SqlCommand MySqlCommand = new SqlCommand("AddMessage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@MessageID";
            parm.Value = MessageID;
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@MessageStr";
            parm.Value = MessageStr;
            MySqlCommand.Parameters.Add(parm);

                //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            //MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            //MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            //MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            int MyCount = MySqlCommand.ExecuteNonQuery();
            MySqlConnection.Close();

           




        }

        public void DeleteMessage(string LockID, string MessageID)
        {

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //--切底删除-----------
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            string MyCommandText = "delete from MessageList  where LockID=@LockID and MessageID=@MessageID";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@MessageID", MessageID));

            MySqlConnection.Open();

            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();


           


        }

    }
}
