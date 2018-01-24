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
     public class HelpTool
    {
         public static string ConnectionString;
         public static SqlConnection MySqlConnection;
         public static string ServiceBusServerIP;
         public static string ServiceBusServerPort;
         static HelpTool()
         {
             //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
             ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             ConnectionString = CloudLockConnectString.ConnectionString;
             //-----------------------------------------------------------------------------------
             //LGJVMWin2003 
             //ConnectionString = "Data Source=LGJVMWin2003;Initial Catalog=CloudLock;Persist Security Info=True;User ID=sa;Password=123456";
             //MySqlConnection = new SqlConnection(ConnectionString);
             //MySqlConnection.Open();

             //1.打开配置文件,获取相应的appSettings配置节
             ServiceBusServerIP = ConfigurationManager.AppSettings["ServiceBusServerIP"];
             ServiceBusServerPort = ConfigurationManager.AppSettings["ServiceBusServerPort"];
         
         }

          public static void Close()
           {
               if (MySqlConnection != null)
               {
                   if (MySqlConnection.State==ConnectionState.Open  )
                    MySqlConnection.Close();


               }

            }


    }
}
