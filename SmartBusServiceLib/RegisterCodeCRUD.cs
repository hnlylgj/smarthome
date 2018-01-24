using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SmartBusServiceLib
{
    public class RegisterCodeCRUD
    {
         string ConnectionString;
        public bool ResultID;
        public RegisterCodeCRUD()
        {
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;
        
        }
        public RegisterCodeCRUD(string InConnectionString)
        {
            this.ConnectionString = InConnectionString;
        }

        public List<RegisterCode> LoadAllRegisterCode()
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

        public List<RegisterCode> FindRegisterCode(int ManagerID)
        {

            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from RegisterCode where ManagerID=" + ManagerID.ToString();

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            RegisterCode AnyRegisterCode = null;
            List<RegisterCode> MyAllRegisterCode = new List<RegisterCode>();
            while (MySqlDataReader.Read())
            {

                AnyRegisterCode = new RegisterCode();

                AnyRegisterCode.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyRegisterCode.RegisterCodeStr = (string)MySqlDataReader["RegisterCode"];
                AnyRegisterCode.LockID = (string)MySqlDataReader["LockID"];
                AnyRegisterCode.Date = (DateTime)MySqlDataReader["Date"];

                MyAllRegisterCode.Add(AnyRegisterCode);  
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllRegisterCode;



        }

        public List<RegisterCode> FindRegisterCode(int ManagerID, string InConnectionString)
        {
                     
            ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            string MyCommandText = "select * from RegisterCode where ManagerID=" + ManagerID.ToString();

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            RegisterCode AnyRegisterCode = null;
            List<RegisterCode> MyAllRegisterCode = new List<RegisterCode>();
            while (MySqlDataReader.Read())
            {

                AnyRegisterCode = new RegisterCode();

                AnyRegisterCode.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyRegisterCode.RegisterCodeStr = (string)MySqlDataReader["RegisterCode"];
                AnyRegisterCode.LockID = (string)MySqlDataReader["LockID"];
                AnyRegisterCode.Date = (DateTime)MySqlDataReader["Date"];

                MyAllRegisterCode.Add(AnyRegisterCode);
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllRegisterCode;

        }

        public RegisterCode FindRegisterCode(string LockID)
        {

            //--------------------------------------------------------------
            string NowDateStr = DateTime.Now.ToString();
            NowDateStr = NowDateStr.Substring(0, NowDateStr.IndexOf(" "));
            string AddSubNowDateStr = NowDateStr.Replace("-", "");
            string DateIDFlag = "";
            AddSubNowDateStr = "(Select DATEADD(DAY,-3,'" + AddSubNowDateStr + "')) ";
            DateIDFlag = " and Date>=" + AddSubNowDateStr;

            //ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
             //SqlConnection MySqlConnection = HelpTool.MySqlConnection;

            string MyCommandText = "select * from RegisterCode where LockID='" + LockID + "'" + DateIDFlag + " ORDER BY Date DESC";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);

         
            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            RegisterCode AnyRegisterCode = null;
            while (MySqlDataReader.Read())
            {
                AnyRegisterCode = new RegisterCode();
                AnyRegisterCode.ManagerID = (int)MySqlDataReader["ManagerID"];
                AnyRegisterCode.RegisterCodeStr = (string)MySqlDataReader["RegisterCode"];
                AnyRegisterCode.LockID = (string)MySqlDataReader["LockID"];
                AnyRegisterCode.Date = (DateTime)MySqlDataReader["Date"];
                break;
                
            }

            MySqlDataReader.Close();
            MySqlConnection.Close();
            return AnyRegisterCode;

        }
               
        public void UpdateRegisterCode(RegisterCode MyRegisterCode)
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

        public void DeleteRegisterCode(RegisterCode MyRegisterCode)
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

        public int InsertRegisterCode(RegisterCode MyRegisterCode)
        {          

                //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
                     
                SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
                MySqlConnection.Open();
                //SqlConnection MySqlConnection = HelpTool.MySqlConnection;
                //--1.--------------------------------------------------------
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
                MyLockFlow.FlowID = MyFlowID;
                */
                //--2.------------------------------------------------------------
                string MyCommandText = "insert into RegisterCode (ManagerID, LockID,RegisterCode,Date) values (@ManagerID, @LockID,@RegisterCode, @Date)";

                SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
                MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", MyRegisterCode.ManagerID));
                MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", MyRegisterCode.RegisterCodeStr));
                MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyRegisterCode.LockID));
                MySqlCommand.Parameters.Add(new SqlParameter("@Date", MyRegisterCode.Date));

               int RowCount= MySqlCommand.ExecuteNonQuery();

                //---------------------------------------------------------------
                MySqlConnection.Close();
                return RowCount;

                       

        }

        public int InsertRegisterCodeEx(RegisterCode MyRegisterCode)
        {

            //string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";

            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
         
            //--1.-----------------------------------------------------------------------------------------------
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
          MyLockFlow.FlowID = MyFlowID;
          */
            //--2.-----------------------------------------------------------------------------------------------
            RandomRegisterCode MyRandomRegisterCode = new RandomRegisterCode();
            MyRegisterCode.RegisterCodeStr = MyRandomRegisterCode.CreateRegsisterCode(); 

            string MyCommandText = "insert into RegisterCode (ManagerID, LockID,RegisterCode) values (@ManagerID, @LockID,@RegisterCode)";

            SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
            MySqlCommand.Parameters.Add(new SqlParameter("@ManagerID", MyRegisterCode.ManagerID));
            MySqlCommand.Parameters.Add(new SqlParameter("@RegisterCode", MyRegisterCode.RegisterCodeStr));
            MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyRegisterCode.LockID));
            //MySqlCommand.Parameters.Add(new SqlParameter("@Date", MyRegisterCode.Date));
           
            MySqlConnection.Open();
            int RowCount = MySqlCommand.ExecuteNonQuery();
            //------------------------------------------------------------------------------------------------------
            MySqlConnection.Close();
            return RowCount;



        }

    }
}
