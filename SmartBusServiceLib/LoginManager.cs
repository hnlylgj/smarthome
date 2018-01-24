using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SmartBusServiceLib
{
     public class LoginManager
    {
         string ConnectionString;
         public LoginManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            this.ConnectionString = HelpTool.ConnectionString;



        }
         public LoginManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }
         public List<LoginStatus> LoadAllLogin()
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

         public List<LoginStatus> FindLoginForCustomerID(int CustomerID)
         {

             SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

             string MyCommandText = "select TOP 2 * from Login where CustomerID=" + CustomerID.ToString() + "  ORDER BY CreateDate DESC";

             SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

             MySqlConnection.Open();

             SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

             List<LoginStatus> MyLoginStatusList = new List<LoginStatus>();

            LoginStatus AnyLoginStatus = null;

             while (MySqlDataReader.Read())
             {

                 AnyLoginStatus = new LoginStatus();

                 AnyLoginStatus.CustomerID = (int)MySqlDataReader["CustomerID"];
                 AnyLoginStatus.LoginFlag = (int)MySqlDataReader["LoginFlag"];
                 AnyLoginStatus.CreateDate = (DateTime)MySqlDataReader["CreateDate"];
                 MyLoginStatusList.Add(AnyLoginStatus);

             }

             MySqlDataReader.Close();

             MySqlConnection.Close();

             return MyLoginStatusList;



         }

         public LoginStatus FindLoginForCustomerIDEx(int CustomerID)
         {

             SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

             string MyCommandText = "select TOP 1 * from Login where CustomerID=" + CustomerID.ToString() + "  ORDER BY CreateDate DESC";

             SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

             MySqlConnection.Open();

             SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();          

             LoginStatus AnyLoginStatus = null;

             while (MySqlDataReader.Read())
             {

                 AnyLoginStatus = new LoginStatus();

                 AnyLoginStatus.CustomerID = (int)MySqlDataReader["CustomerID"];
                 AnyLoginStatus.LoginFlag = (int)MySqlDataReader["LoginFlag"];
                 AnyLoginStatus.CreateDate = (DateTime)MySqlDataReader["CreateDate"];
                

             }

             MySqlDataReader.Close();

             MySqlConnection.Close();

             return AnyLoginStatus;



         }

         public void InsertLoginStatus(LoginStatus MyLoginStatus)
         {
             
             SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
             MySqlConnection.Open();
             //-----------------------------------------------------------------------------------------
             string MyCommandText = "insert into Login (CustomerID, LoginFlag) values (@CustomerID, @LoginFlag)";

             SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
             MySqlCommand.Parameters.Add(new SqlParameter("@CustomerID", MyLoginStatus.CustomerID));
             MySqlCommand.Parameters.Add(new SqlParameter("@LoginFlag", MyLoginStatus.LoginFlag));
             MySqlCommand.ExecuteNonQuery();

             //---------------------------------------------------------------
             MySqlConnection.Close();
           

         }


    }
}
