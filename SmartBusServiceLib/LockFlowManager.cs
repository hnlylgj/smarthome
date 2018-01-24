using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SmartBusServiceLib
{
    public class LockFlowManager
    {
        string ConnectionString;
        public bool ResultID;
        public LockFlowManager()
        {
           
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;
        
        
        }
        public LockFlowManager(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }

        public List<LockFlow> LoadAllLockFlow()
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

        public List<LockFlow> FindLockFlow(int ManagerID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from LockFlow where ManagerID=" + ManagerID.ToString();

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            LockFlow AnyLockFlow = null;
            List<LockFlow> MyAllLockFlow = new List<LockFlow>();
            while (MySqlDataReader.Read())
            {

                AnyLockFlow = new LockFlow();

                AnyLockFlow.FlowID = (int)MySqlDataReader["FlowID"];
                AnyLockFlow.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyLockFlow.OperatorID = (int)MySqlDataReader["OperatorID"];
                AnyLockFlow.LockID = (string)MySqlDataReader["LockID"];
                AnyLockFlow.Date = (DateTime)MySqlDataReader["Date"];

                MyAllLockFlow.Add(AnyLockFlow);  
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllLockFlow;



        }

        public List<LockFlow> FindLockFlow(int ManagerID, string InConnectionString)
        {
                     
            ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from LockFlow where ManagerID=" + ManagerID.ToString();
            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            LockFlow AnyLockFlow = null;
            List<LockFlow> MyAllLockFlow = new List<LockFlow>();
            while (MySqlDataReader.Read())
            {
                AnyLockFlow = new LockFlow();
                AnyLockFlow.FlowID = (int)MySqlDataReader["FlowID"];
                AnyLockFlow.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyLockFlow.OperatorID = (int)MySqlDataReader["OperatorID"];
                AnyLockFlow.LockID = (string)MySqlDataReader["LockID"];
                AnyLockFlow.Date = (DateTime)MySqlDataReader["Date"];

                MyAllLockFlow.Add(AnyLockFlow);
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllLockFlow;

        }

        public void UpdateLockFlow(LockFlow MyLockFlow)
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

        public void DeleteLockFlow(LockFlow MyLockFlow)
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

        public void InsertLockFlow(LockFlow MyLockFlow)
        {
           

                //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
                ResultID = false;

                SqlConnection MySqlConnection =  new SqlConnection(this.ConnectionString);
                MySqlConnection.Open();
                //SqlConnection MySqlConnection = HelpTool.MySqlConnection;
                //--1.--------------------------------------------------------
                int MyFlowID;
                using (SqlCommand GetMaxcmd = new SqlCommand())
                {
                    GetMaxcmd.CommandText = "select Max(FlowID) from LockFlow";//89765432BCDA820";
                    GetMaxcmd.Connection = MySqlConnection;
                    using (SqlDataReader MySqlDataReader = GetMaxcmd.ExecuteReader())
                    {
                        MySqlDataReader.Read();
                        MyFlowID = (int)MySqlDataReader.GetValue(0);


                    }
                }
                MyFlowID++;
                MyLockFlow.FlowID = MyFlowID;

                //--2.------------------------------------------------------------
                string MyCommandText = "insert into LockFlow (FlowID,ManagerID, OperatorID, LockID,Date) values (@FlowID,@ManagerID, @OperatorID, @LockID, @Date)";

                SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
                MySqlCommand.Parameters.Add(new SqlParameter("@FlowID", MyLockFlow.FlowID));
                MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", MyLockFlow.ManagerID));
                MySqlCommand.Parameters.Add(new SqlParameter("@OperatorID", MyLockFlow.OperatorID));
                MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockFlow.LockID));
                MySqlCommand.Parameters.Add(new SqlParameter("@Date", MyLockFlow.Date));

                MySqlCommand.ExecuteNonQuery();

                //---------------------------------------------------------------
                MySqlConnection.Close();
                ResultID = true;


                       

        }

        public void InsertLockFlowEx(LockFlow MyLockFlow)
        {


            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            ResultID = false;

            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
            MySqlConnection.Open();
            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;
            //--1.------------------------------------------------------------------------------------------------------------------------------
              /*
            int MyFlowID;
            using (SqlCommand GetMaxcmd = new SqlCommand())
            {
                GetMaxcmd.CommandText = "select Max(FlowID) from LockFlow";//89765432BCDA820";
                GetMaxcmd.Connection = MySqlConnection;
                using (SqlDataReader MySqlDataReader = GetMaxcmd.ExecuteReader())
                {
                    MySqlDataReader.Read();
                    MyFlowID = (int)MySqlDataReader.GetValue(0);


                }
            }
            MyFlowID++;
            */
            MyLockFlow.FlowID = 0;

            //--2.--------------------------------------------------------------------------------------------------------------------------------
            string MyCommandText = "insert into LockFlow (FlowID,ManagerID, OperatorID, LockID) values (@FlowID,@ManagerID, @OperatorID, @LockID)";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@FlowID", MyLockFlow.FlowID));
            MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", MyLockFlow.ManagerID));
            MySqlCommand.Parameters.Add(new SqlParameter("@OperatorID", MyLockFlow.OperatorID));
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockFlow.LockID));

           
            MySqlCommand.ExecuteNonQuery();

            MySqlConnection.Close();
            ResultID = true;




        }
    
    }
}
