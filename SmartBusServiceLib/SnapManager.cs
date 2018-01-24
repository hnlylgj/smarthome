using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SmartBusServiceLib
{
    [System.ComponentModel.DataObjectAttribute]
    public class SnapManager
    {

        string ConnectionString;
        SqlConnection MySqlConnection;
        public SnapManager()
        {
              //ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            //ConnectionString = "Data Source=cloudlock.sqlserver.rds.aliyuncs.com,3433;Initial Catalog=cloudlock;Persist Security Info=True;User ID=lgj;Password=531202";
            ConnectionString = HelpTool.ConnectionString;
        }
        public SnapManager(string InConnectionString)
        {
            ConnectionString = InConnectionString;
        }
        public SnapManager(SqlConnection InSqlConnection)
        {
            this.MySqlConnection = InSqlConnection;
        }

        public List<SnapImage> LoadAllSnap()
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

        public List<SnapImage> LoadAllSnap(string LockIDStr, string InConnectionString)
        {

            List<SnapImage> MyAllSnapImage = new List<SnapImage>();
            if (InConnectionString != null)
                ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            string MySQLViewName = "snap" + LockIDStr;
            string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,CreateTime from  " + MySQLViewName + "  where SnapID > 0 and Image is not null ORDER BY SnapID DESC";

            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {

                SnapImage AnySnapImage = new SnapImage();

                AnySnapImage.SnapID = (long)MySqlDataReader["SnapID"];

                AnySnapImage.SnapTypeID = (int)MySqlDataReader["SnapTypeID"];

                AnySnapImage.SnapUUID = (string)MySqlDataReader["SnapUUID"];

                AnySnapImage.ViewFlag = (int)MySqlDataReader["ViewFlag"];

                AnySnapImage.CreateTime = (DateTime)MySqlDataReader["CreateTime"];

                MyAllSnapImage.Add(AnySnapImage);

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllSnapImage;



        }

        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
        public List<SnapImage> LoadAllSnap(string LockIDStr, string InConnectionString, string SnapTypeID, string TimeRange)
        {

            List<SnapImage> MyAllSnapImage = new List<SnapImage>();
            if (InConnectionString != null)
                ConnectionString = InConnectionString;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            string MySQLViewName = "snap" + LockIDStr;

            //---1.-------------------------------------------
            string SnapTypeIDFlag = "";
            if (SnapTypeID == "0")
            {
                SnapTypeIDFlag = "and SnapTypeID > -1";
            }
            if (SnapTypeID == "1")
            {
                SnapTypeIDFlag = "and SnapTypeID= 0";
            }

            if (SnapTypeID == "2")
            {
                SnapTypeIDFlag = "and SnapTypeID=1";
            }
            if (SnapTypeID == "3")
            {
                SnapTypeIDFlag = "and SnapTypeID=2";
            }
            //--2.-------------------------------------------------
            string DateIDFlag = "";
            string NowDateStr = DateTime.Now.ToString();
            NowDateStr = NowDateStr.Substring(0, NowDateStr.IndexOf(" "));
            string AddSubNowDateStr = NowDateStr.Replace("-", "");

            //Select DATEADD(DAY,1,'20130101')            
            if (TimeRange == "0")//当天
            {
                DateIDFlag = " and CreateTime>='" + NowDateStr + "'";
            }

            if (TimeRange == "1")//7天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-7,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;
            }
            if (TimeRange == "2")//10天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-10,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;
            }
            if (TimeRange == "3")//30天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-30,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;
            }
            if (TimeRange == "4")//2*30天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-60,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;
            }
            if (TimeRange == "5")//3*30天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-90,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;
            }
            if (TimeRange == "6")//6*30天之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-180,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;

            }
            if (TimeRange == "7")//一年之内
            {
                AddSubNowDateStr = "(Select DATEADD(DAY,-365,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;

            }
            if (TimeRange == "8")//三年之内
            {
                //string TempDateStr = "-1095";
                //AddSubNowDateStr = "(Select DATEADD(DAY,"+TempDateStr+",'" + AddSubNowDateStr + "')) ";
                AddSubNowDateStr = "(Select DATEADD(DAY,-1095,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;

            }

            if (TimeRange == "9")//五年之内
            {
                //string TempDateStr = "-1825";
                //AddSubNowDateStr = "(Select DATEADD(DAY," + TempDateStr + ",'" + AddSubNowDateStr + "')) ";
                AddSubNowDateStr = "(Select DATEADD(DAY,-1825,'" + AddSubNowDateStr + "')) ";
                DateIDFlag = " and CreateTime>=" + AddSubNowDateStr;

            }
            if (TimeRange == "10")//全部
            {
                DateIDFlag = "";
            }

            // string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,Date from  " + MySQLViewName + "  where SnapID > 0 and Image is not null ORDER BY SnapID DESC";
            // string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,Date from  " + MySQLViewName + "  where SnapID > 0 and Image is not null "+SnapTypeIDFlag+ DateIDFlag+" ORDER BY SnapID DESC";

            string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,CreateTime from  " + MySQLViewName + "  where SnapID > 0 " + SnapTypeIDFlag + DateIDFlag + " ORDER BY SnapID DESC";

            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {

                SnapImage AnySnapImage = new SnapImage();

                AnySnapImage.SnapID = (long)MySqlDataReader["SnapID"];

                AnySnapImage.SnapTypeID = (int)MySqlDataReader["SnapTypeID"];

                AnySnapImage.SnapUUID = (string)MySqlDataReader["SnapUUID"];

                AnySnapImage.ViewFlag = (int)MySqlDataReader["ViewFlag"];

                AnySnapImage.CreateTime = (DateTime)MySqlDataReader["CreateTime"];

                MyAllSnapImage.Add(AnySnapImage);

            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return MyAllSnapImage;



        }

        public SnapImage FindSnapForSnapID(string LockIDStr, string SnapID, string InConnectionString)
        {

            //List<SnapImage> MyAllSnapImage = new List<SnapImage>();
            if (InConnectionString != null) ConnectionString = InConnectionString; ;
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            //string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,Date from  " + MySQLViewName + "  where SnapID="+SnapID+ " and Image is not null ";
            string MySQLViewName = "snap" + LockIDStr;
            string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,CreateTime from  " + MySQLViewName + "  where SnapID=" + SnapID;
            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();
            SnapImage AnySnapImage = null;

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {

                AnySnapImage = new SnapImage();

                AnySnapImage.SnapID = (long)MySqlDataReader["SnapID"];

                AnySnapImage.SnapTypeID = (int)MySqlDataReader["SnapTypeID"];

                AnySnapImage.SnapUUID = (string)MySqlDataReader["SnapUUID"];

                AnySnapImage.ViewFlag = (int)MySqlDataReader["ViewFlag"];

                AnySnapImage.CreateTime = (DateTime)MySqlDataReader["CreateTime"];

                //MyAllSnapImage.Add(AnySnapImage);
                break;
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return AnySnapImage;



        }

        public int TotalDynamicForLockID(string LockIDStr, string InConnectionString)
        {

            //List<SnapImage> MyAllSnapImage = new List<SnapImage>();
            if (InConnectionString != null)
                ConnectionString = InConnectionString;// "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);

            //string commandText = "select * from LockKey where LockID='"+LockIDStr+"'";
            string MySQLViewName = "snap" + LockIDStr;
            //string commandText = "select SnapID,SnapTypeID,SnapUUID,ViewFlag,Date from  " + MySQLViewName + "  where SnapID="+SnapID+ " and Image is not null ";
            string commandText = "select MyCount=count(*) from  " + MySQLViewName + "  where ViewFlag=1";
            SqlCommand MySqlCommand = new SqlCommand(commandText, MySqlConnection);

            MySqlConnection.Open();
            int TotalDynamic = 0;

            SqlDataReader MySqlDataReader = MySqlCommand.ExecuteReader();

            while (MySqlDataReader.Read())
            {



                TotalDynamic = (int)MySqlDataReader["MyCount"];




                break;
            }

            MySqlDataReader.Close();

            MySqlConnection.Close();

            return TotalDynamic;



        }

        public byte[] DownImageFromDB(long SnapID, string LockID, int ScaleFlag = 0)
        {

             //string LockID = MyLockIDStr;
          
            string SqlCommandStr;
            byte[] MyImageBytes = null;
            string MySQLViewName = "snap" + LockID;
      
            //SqlCommandStr = "Select Image from snap" + LockID + " where SnapID=@SnapID";
            //" + MySQLViewName + "
          
            SqlCommandStr = "Select Image from " + MySQLViewName + " where SnapID=@SnapID";

            //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
            //using (SqlConnection cn = new SqlConnection(CloudLockConnectString.ConnectionString))
            //{
            //SqlConnection MySqlConnection = HelpTool.MySqlConnection;

            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
            MySqlConnection.Open();
            using (SqlCommand cmd = new SqlCommand(SqlCommandStr, MySqlConnection))
            {

                SqlParameter parm = cmd.CreateParameter();
                parm.DbType = System.Data.DbType.Int64;
                parm.Value = SnapID;
                parm.ParameterName = "@SnapID";
                cmd.Parameters.Add(parm);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    if (dr.GetValue(0)!=System.DBNull.Value)
                    MyImageBytes = (byte[])dr.GetValue(0);
                }
            }

            MySqlConnection.Close();
            if (MyImageBytes == null || MyImageBytes.Length == 0) return null;

            if (ScaleFlag == 0)
            {
                //原图大小
                return MyImageBytes;
            }
            else
            {
                //缩略图
                MemoryStream tempStream;
                Bitmap MyBitMap;

                tempStream = new MemoryStream(MyImageBytes, 0, MyImageBytes.Length);
                MyBitMap = new Bitmap(tempStream);
                MyBitMap = new Bitmap(MyBitMap, MyBitMap.Height / 16, MyBitMap.Width / 18);
                //MyBitMap = new Bitmap(MyBitMap, 300,300);
                byte[] MyMiniImageBytes;
                MemoryStream MinitempStream = new MemoryStream();

                MyBitMap.Save(MinitempStream, ImageFormat.Jpeg);
                int MiniImageLenght = MinitempStream.Capacity;
                MyMiniImageBytes = new byte[MiniImageLenght];
                MinitempStream.GetBuffer().CopyTo(MyMiniImageBytes, 0);

                tempStream.Close();
                MinitempStream.Close();
                return MyMiniImageBytes;

               

            }

            //}
            //if (MyImageBytes == null || MyImageBytes.Length == 0) return null;




            //MemoryStream tempmStream;
            //tempmStream = new MemoryStream(MyImageBytes, 0, MyImageBytes.Length);
            //bmp = new Bitmap(tempStream);
            //ImagepictureBox.Image = Image.FromStream(tempmStream);

            //ResultlistBox.Items.Add("读取图像成功！");
            //if you want to resize the photos...
            //bmp = new Bitmap(bmp, bmp.Height / 2, bmp.Width / 2);
            //Response.ContentType = "image/gif";
            //bmp.Save(Response.OutputStream, ImageFormat.Gif);
            //Response.End();

        }

        public Bitmap DownImageFromDBEx(long SnapID, string LockID, int ScaleFlag = 0)
        {
            string SqlCommandStr;
            byte[] MyImageBytes;
            string MySQLViewName = "snap" + LockID;
            SqlCommandStr = "Select Image from " + MySQLViewName + " where SnapID=@SnapID";


            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
            MySqlConnection.Open();
            using (SqlCommand cmd = new SqlCommand(SqlCommandStr, MySqlConnection))
            {

                SqlParameter parm = cmd.CreateParameter();
                parm.DbType = System.Data.DbType.Int64;
                parm.Value = SnapID;
                parm.ParameterName = "@SnapID";
                cmd.Parameters.Add(parm);
                //cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    MyImageBytes = (byte[])dr.GetValue(0);
                }
            }

            MySqlConnection.Close();
            if (MyImageBytes == null || MyImageBytes.Length == 0) return null;

            MemoryStream tempStream;
            Bitmap MyBitMap;
            tempStream = new MemoryStream(MyImageBytes, 0, MyImageBytes.Length);
            MyBitMap = new Bitmap(tempStream);
            if (ScaleFlag == 0)
            {
                //原图大小
                tempStream.Close();
              
                return MyBitMap;
            }
            else
            {
                //缩略图       

                 MyBitMap = new Bitmap(MyBitMap, MyBitMap.Height / 16, MyBitMap.Width / 18);
                //MyBitMap = new Bitmap(MyBitMap, 300, 300);
                tempStream.Close();
                return MyBitMap;

            }


        }

        public void UpdateListImageFromDB(string LockID, ref List<SnapImage> ForSnapList, int ScaleFlag = 0)
        {
            string SqlCommandStr;
            string MySQLViewName = "snap" + LockID;       


            SqlConnection MySqlConnection = new SqlConnection(this.ConnectionString);
            MySqlConnection.Open();

            int LoopCount=ForSnapList.Count;
            while (LoopCount > 0)
            {
                long SnapID = ForSnapList[LoopCount - 1].SnapID;
                SqlCommandStr = "Select Image from " + MySQLViewName + " where SnapID=@SnapID";
                using (SqlCommand cmd = new SqlCommand(SqlCommandStr, MySqlConnection))
                {

                    SqlParameter parm = cmd.CreateParameter();
                    parm.DbType = System.Data.DbType.Int64;
                    parm.Value = SnapID;
                    parm.ParameterName = "@SnapID";
                    cmd.Parameters.Add(parm);
                   
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        ForSnapList[LoopCount - 1].ImageByteBuffer = (byte[])dr.GetValue(0);
                    }
                }

                LoopCount--;
            }

            
            MySqlConnection.Close();
          
            if (ScaleFlag == 1)
            {
                //------图像变换--------------------------------
                LoopCount = ForSnapList.Count;
                while (LoopCount > 0)
                {
                    MemoryStream tempStream;
                    Bitmap MyBitMap;
                    tempStream = new MemoryStream(ForSnapList[LoopCount - 1].ImageByteBuffer);
                    MyBitMap = new Bitmap(tempStream);
                    //--缩略图---
                    MyBitMap = new Bitmap(MyBitMap, MyBitMap.Height / 16, MyBitMap.Width / 18);

                    byte[] MyMiniImageBytes;
                    MemoryStream MinitempStream = new MemoryStream();
                    MyBitMap.Save(MinitempStream, ImageFormat.Jpeg);
                    int MiniImageLenght = MinitempStream.Capacity;
                    MyMiniImageBytes = new byte[MiniImageLenght];
                    MinitempStream.GetBuffer().CopyTo(MyMiniImageBytes, 0);
                    ForSnapList[LoopCount - 1].ImageByteBuffer = MyMiniImageBytes;

                    tempStream.Close();
                    MinitempStream.Close();

                    LoopCount--;

                }

            }
          
            

          

       


        }

        public void UpdateSnap(SnapImage MySnapImage)
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

        public void DeleteSnap(SnapImage MySnapImage)
        {
            /*
            string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
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

        public void InsertSnap(SnapImage MySnapImage)
        {
           
            /*          
          string ConnectionString = "Server=VMXPSP3_LGJ;database=CloudLock;uid=sa;pwd=123456";
          SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
          string MySQLViewName = "key" + MyLockKey.LockID;
          string MyCommandText = "insert into "+MySQLViewName+" (LockID,Number, Name, KeyStr, Date) values (@LockID,@Number, @Name, @KeyStr, @Date)";
           
          SqlCommand MySqlCommand = new SqlCommand(MyCommandText, MySqlConnection);
          MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MyLockKey.LockID));
          MySqlCommand.Parameters.Add(new SqlParameter("@Number", MyLockKey.LockKeyID));
          MySqlCommand.Parameters.Add(new SqlParameter("@Name", MyLockKey.OwerName));
          MySqlCommand.Parameters.Add(new SqlParameter("@KeyStr", MyLockKey.KeyString));
          MySqlCommand.Parameters.Add(new SqlParameter("@Date", MyLockKey.GetCreateTime));


          MySqlConnection.Open();

          MySqlCommand.ExecuteNonQuery();

          MySqlConnection.Close();
            * */

        }

        public int InsertSnap(SnapImage MySnapImage, ref long SnapID)
        {
            
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            SqlCommand MySqlCommand = new SqlCommand("AddSnapImage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlConnection.Open();

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = MySnapImage.LockID;// LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@SnapTypeID";
            parm.Value = MySnapImage.SnapTypeID;// SnapTypeID;//0,1,2
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@SnapUUID";
            parm.Value = MySnapImage.SnapUUID;// SnapUUID;
            MySqlCommand.Parameters.Add(parm);

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            SnapID = (long)MySqlCommand.Parameters["@ReturnSnapID"].Value;

            MySqlConnection.Close();
            return ReturnValue;
        }
   
        public int ShareXInsertSnap(SnapImage MySnapImage, ref long SnapID)
        {
           //测试有问题
            SqlCommand MySqlCommand = new SqlCommand("AddSnapImage", MySqlConnection);
            MySqlCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter parm = new SqlParameter();
            parm.ParameterName = "@LockID";
            parm.Value = MySnapImage.LockID;// LockID;
            MySqlCommand.Parameters.Add(parm);

            parm = new SqlParameter();
            parm.ParameterName = "@SnapTypeID";
            parm.Value = MySnapImage.SnapTypeID;// SnapTypeID;//0,1,2
            MySqlCommand.Parameters.Add(parm);


            parm = new SqlParameter();
            parm.ParameterName = "@SnapUUID";
            parm.Value = MySnapImage.SnapUUID;// SnapUUID;
            MySqlCommand.Parameters.Add(parm);

            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
            MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
            MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            int MyCount = MySqlCommand.ExecuteNonQuery();
            int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
            SnapID = (long)MySqlCommand.Parameters["@ReturnSnapID"].Value;

            return ReturnValue;
        }

        public int UpdateSnapImage(string LockID, byte[] SnapImageByte, long SnapID)
        {
            SqlConnection MySqlConnection = new SqlConnection(ConnectionString);
            MySqlConnection.Open();
            SqlCommand MyUpdateImagelCommand = new SqlCommand();
            MyUpdateImagelCommand.CommandText = "Update snap" + LockID + " Set Image = @Image " + "WHERE SnapID = @SnapID";

            SqlParameter parm;
            parm = new SqlParameter();
            parm.ParameterName = "@SnapID";
            parm.Value = SnapID;
            MyUpdateImagelCommand.Parameters.Add(parm);
            parm = new SqlParameter();
            parm.ParameterName = "@Image";
            parm.Value = SnapImageByte;// MyMemoryStreamContent.GetBuffer();
            MyUpdateImagelCommand.Parameters.Add(parm);

            MyUpdateImagelCommand.Connection = MySqlConnection;
            int ReturnFlag = MyUpdateImagelCommand.ExecuteNonQuery();

            MySqlConnection.Close();
            return ReturnFlag;

        }
        
        public int ShareXUpdateSnapImage(string LockID, byte[] SnapImageByte, long SnapID)
        {
            //测试有问题
            SqlCommand MyUpdateImagelCommand = new SqlCommand();
            MyUpdateImagelCommand.CommandText = "Update snap" + LockID + " Set Image = @Image " + "WHERE SnapID = @SnapID";

            SqlParameter parm;
            parm = new SqlParameter();
            parm.ParameterName = "@SnapID";
            parm.Value = SnapID;
            MyUpdateImagelCommand.Parameters.Add(parm);
            parm = new SqlParameter();
            parm.ParameterName = "@Image";
            parm.Value = SnapImageByte;// MyMemoryStreamContent.GetBuffer();
            MyUpdateImagelCommand.Parameters.Add(parm);

            MyUpdateImagelCommand.Connection = MySqlConnection;
            return MyUpdateImagelCommand.ExecuteNonQuery();

        }

    }
}
