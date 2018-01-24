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
    public class CustomerManager
    {

        string ConnectionString;      
        public CustomerManager()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;
       
        
        }
        public CustomerManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }

        public List<Customer> LoadAllCustomer()
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

        public Customer FindCustomer(string InLoginName)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
           
            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;

            string MyCommandText = "select * from Customer where LoginName='" + InLoginName + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

         

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Customer AnyCustomer = null;

            while (MySqlDataReader.Read())
            {

                AnyCustomer = new Customer();

                AnyCustomer.CustomerID = (int)MySqlDataReader["CustomerID"];
                AnyCustomer.CustomerName = (string)MySqlDataReader["CustomerName"];
                AnyCustomer.LoginName = (string)MySqlDataReader["LoginName"];
                AnyCustomer.Password = (string)MySqlDataReader["Password"];
                AnyCustomer.MobileID = (string)MySqlDataReader["MobileID"];
                AnyCustomer.CreateTime = (DateTime)MySqlDataReader["CreateTime"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyCustomer;



        }

        public Customer FindCustomer(string InLoginName, string InConnectionString)
        {

         
            ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Customer where LoginName='" + InLoginName + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Customer AnyCustomer=null;

            while (MySqlDataReader.Read())
            {

                AnyCustomer = new Customer();

                AnyCustomer.CustomerID = (int)MySqlDataReader["CustomerID"];
                AnyCustomer.CustomerName = (string)MySqlDataReader["CustomerName"];
                AnyCustomer.LoginName = (string)MySqlDataReader["LoginName"];         
                AnyCustomer.Password = (string)MySqlDataReader["Password "];
                AnyCustomer.MobileID = (string)MySqlDataReader["MobileID"];
                AnyCustomer.CreateTime = (DateTime)MySqlDataReader["CreateTime"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyCustomer;

        }

        public Customer FindCustomerEx(string MobileID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from Customer where MobileID='" + MobileID + "'";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            Customer AnyCustomer = null;

            while (MySqlDataReader.Read())
            {

                AnyCustomer = new Customer();

                AnyCustomer.CustomerID = (int)MySqlDataReader["CustomerID"];
                AnyCustomer.CustomerName = (string)MySqlDataReader["CustomerName"];
                AnyCustomer.LoginName = (string)MySqlDataReader["LoginName"];
                AnyCustomer.Password = (string)MySqlDataReader["Password"];
                AnyCustomer.MobileID = (string)MySqlDataReader["MobileID"];
                AnyCustomer.CreateTime = (DateTime)MySqlDataReader["CreateTime"];


            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnyCustomer;



        }

        public void UpdateCustomer(Customer MyCustomer)
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

        public void DeleteCustomer(Customer MyCustomer)
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

        public void InsertCustomer(Customer MyCustomer)
        {
             
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
             //string MySQLViewName = "key" + MyLockKey.LockID;
            //string MyCommandText = "insert into Customer (LockID,Number, Name, KeyStr, Date) values (@LockID,@Number, @Name, @KeyStr, @Date)";

            SqlCommand MySqlCommand = new SqlCommand("AddCustomer",MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;
            MySqlCommand.Parameters.Add(new SqlParameter("@CustomerName", MyCustomer.CustomerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@LoginName", MyCustomer.LoginName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Password", MyCustomer.Password));
            MySqlCommand.Parameters.Add(new SqlParameter("@PersonID", MyCustomer.PersonID));
            MySqlCommand.Parameters.Add(new SqlParameter("@TeleCode", MyCustomer.TeleCode));
            MySqlCommand.Parameters.Add(new SqlParameter("@EMail", MyCustomer.EMail));
            MySqlCommand.Parameters.Add(new SqlParameter("@Address", MyCustomer.Address));
          
            MySqlConnection.Open();
            int MyCount=MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            MySqlConnection.Close();
            

        }
      
        public void InsertCustomerEx(Customer MyCustomer, Channel MyChannel)
        {

            CreateMobileID MyCreateMobileID = new CreateMobileID();
 
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            SqlCommand MySqlCommand = new SqlCommand("AddCustomerEx", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyChannel.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", MyChannel.RegisterCodeStr));
            MySqlCommand.Parameters.Add(new SqlParameter("@MobileID", MyCreateMobileID.GetMobileID()));

            MySqlCommand.Parameters.Add(new SqlParameter("@CustomerName", MyCustomer.CustomerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@LoginName", MyCustomer.LoginName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Password", MyCustomer.Password));
            MySqlCommand.Parameters.Add(new SqlParameter("@PersonID", MyCustomer.PersonID));
            MySqlCommand.Parameters.Add(new SqlParameter("@TeleCode", MyCustomer.TeleCode));
            MySqlCommand.Parameters.Add(new SqlParameter("@EMail", MyCustomer.EMail));
            MySqlCommand.Parameters.Add(new SqlParameter("@Address", MyCustomer.Address));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();
            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            MySqlConnection.Close();


        }
        public void InsertCustomerExx(Customer MyCustomer, Channel MyChannel)
        {

            CreateMobileID MyCreateMobileID = new CreateMobileID();

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            SqlCommand MySqlCommand = new SqlCommand("AddCustomerExxxx", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyChannel.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", MyChannel.RegisterCodeStr));
            MySqlCommand.Parameters.Add(new SqlParameter("@MobileID", MyCreateMobileID.GetMobileID()));

            MySqlCommand.Parameters.Add(new SqlParameter("@CustomerName", MyCustomer.CustomerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@LoginName", MyCustomer.LoginName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Password", MyCustomer.Password));
            MySqlCommand.Parameters.Add(new SqlParameter("@PersonID", MyCustomer.PersonID));
            MySqlCommand.Parameters.Add(new SqlParameter("@TeleCode", MyCustomer.TeleCode));
            MySqlCommand.Parameters.Add(new SqlParameter("@EMail", MyCustomer.EMail));
            MySqlCommand.Parameters.Add(new SqlParameter("@Address", MyCustomer.Address));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnMobileID", SqlDbType.NChar,15));
            MySqlCommand.Parameters["@ReturnMobileID"].Direction = ParameterDirection.Output;

            MySqlConnection.Open();
            int MyCount = MySqlCommand.ExecuteNonQuery();
            //int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            //string MobileID = (string)MySqlCommand.Parameters["@ReturnMobileID"].Value;
            MySqlConnection.Close();


        }
        public string InsertCustomerExxx(Customer MyCustomer, Channel MyChannel)
        {

            CreateMobileID MyCreateMobileID = new CreateMobileID();
            string NewMobileID = MyCreateMobileID.GetMobileID();
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            


            SqlCommand MySqlCommand = new SqlCommand("AddCustomerExxxx", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyChannel.LockID));
            MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", MyChannel.RegisterCodeStr));
            MySqlCommand.Parameters.Add(new SqlParameter("@MobileID", NewMobileID));

            MySqlCommand.Parameters.Add(new SqlParameter("@CustomerName", MyCustomer.CustomerName));
            MySqlCommand.Parameters.Add(new SqlParameter("@LoginName", MyCustomer.LoginName));
            MySqlCommand.Parameters.Add(new SqlParameter("@Password", MyCustomer.Password));
            MySqlCommand.Parameters.Add(new SqlParameter("@PersonID", MyCustomer.PersonID));
            MySqlCommand.Parameters.Add(new SqlParameter("@TeleCode", MyCustomer.TeleCode));
            MySqlCommand.Parameters.Add(new SqlParameter("@EMail", MyCustomer.EMail));
            MySqlCommand.Parameters.Add(new SqlParameter("@Address", MyCustomer.Address));

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnMobileID", SqlDbType.NChar, 15));
            MySqlCommand.Parameters["@ReturnMobileID"].Direction = ParameterDirection.Output;

         
            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            
            //MySqlConnection.Close();
         
            if (ReturnValue == 0)
            {
                CreateCustomerView(MyChannel.LockID, MySqlConnection);
                MySqlConnection.Close();
                return NewMobileID;
            }
            else
            {
               MySqlConnection.Close();
                return null;
            }
         


        }

        private void CreateCustomerView(string MyLockID, SqlConnection MySqlConnection)
        {
            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            //MySqlConnection.Open();          
           
            SqlCommand MySqlCommand = new SqlCommand();
            MySqlCommand.Connection = MySqlConnection;
            string MyCommandText;
            string MySQLViewName;

            MySQLViewName = "key" + MyLockID;
            MyCommandText = "create view " + MySQLViewName + " as select * from LockKey where LockID = '" + MyLockID + "' ";
            MySqlCommand.CommandText = MyCommandText;
            MySqlCommand.ExecuteNonQuery();

            MySQLViewName = "open" + MyLockID;
            MyCommandText = "create view " + MySQLViewName + " as select * from OpenDoor where LockID = '" + MyLockID + "' ";
            MySqlCommand.CommandText = MyCommandText;
            MySqlCommand.ExecuteNonQuery();

            MySQLViewName = "snap" + MyLockID;
            MyCommandText = "create view " + MySQLViewName + " as select * from SnapImage where LockID = '" + MyLockID + "' ";
            MySqlCommand.Connection = MySqlConnection;
            MySqlCommand.CommandText = MyCommandText;
            MySqlCommand.ExecuteNonQuery();





   //MySqlConnection.Close();



        }
     
        private void CreateCustomerOSSDir(string MyLockID)
        {

            ;


        }



    }
}
