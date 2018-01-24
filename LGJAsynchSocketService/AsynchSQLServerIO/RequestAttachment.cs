using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace LGJAsynchSocketService.AsynchSQLServerIO
{
     public class RequestAttachment
    {
        private SqlConnection MySqlConnection;
        public SqlCommand MySqlCommand;
        public AsynchLockServerSocketService MyAsynchLockServerSocketService;
        
        public LoginUser MyLoginUser;
        public SocketServiceReadWriteChannel MyReadWriteChannel;
        public byte[] MyReadBuffer;
        //public string MyRequestKey;
        //public string LockID;


        public RequestAttachment(string InLockID,SocketServiceReadWriteChannel MeReadWriteChannel,AsynchLockServerSocketService MeAsynchLockServerSocketService)
        {
            this.MyReadWriteChannel=MeReadWriteChannel;
            this.MyAsynchLockServerSocketService=MeAsynchLockServerSocketService;
            //this.LockID=InLockID;
            //this.MyRequestKey=InRequestKey;
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            MySqlConnection.Open();
            //MySqlCommand = MySqlConnection.CreateCommand();
            MySqlCommand = new SqlCommand("AuthLockID", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;
            //MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyChannel.LockID));

        }

        public RequestAttachment(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
        {            
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffer = MeLoginUser.MyByteBuffer;
            this.MyAsynchLockServerSocketService = MeAsynchLockServerSocketService;
            //this.LockID = InLockID;
            //this.MyRequestKey=InRequestKey;
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            MySqlConnection.Open();
            //MySqlCommand = MySqlConnection.CreateCommand();
            MySqlCommand = new SqlCommand("AuthLockID", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;
            //MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyChannel.LockID));

        }

        public RequestAttachment(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService, string StoredProceName)
        {
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffer = MeLoginUser.MyByteBuffer;
            this.MyAsynchLockServerSocketService = MeAsynchLockServerSocketService;
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            MySqlConnection.Open();
            //MySqlCommand = MySqlConnection.CreateCommand();
            MySqlCommand = new SqlCommand(StoredProceName, MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;
           

        }

        public RequestAttachment(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService,int FlagID)
        {
            this.MyLoginUser = MeLoginUser;
            //this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            //this.MyReadBuffer = MeLoginUser.MyByteBuffer;
            this.MyAsynchLockServerSocketService = MeAsynchLockServerSocketService;
            ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
            MySqlConnection.Open();
            MySqlCommand = MySqlConnection.CreateCommand();
         


        }



    
}

}
