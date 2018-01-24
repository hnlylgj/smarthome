using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using ALiYunOSSOTSAPILib;
using Aliyun.OpenServices.OpenStorageService;

namespace LGJAsynchSocketService.LockServerLib
{
     public class RemoteSnapParser
    {
         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;

         string MyCompleteFileName;
         string MyAutoFileNameStr;
         long SnapID;


         public RemoteSnapParser(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public RemoteSnapParser(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteChannel, int InRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteChannel;
            this.MyReadBuffers = InputReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = InRecieveCount;
        }

         public void RequestToLock()
         {

             //1.解析来自移动端的消息-----------------------------------------------------------------------------------------------------------------------
             string BaseMessageString = Encoding.UTF8.GetString(this.MyReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
             MyLockIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
             MyMobileIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);
             string ParaSnapStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf("#") + 1,1);
;
            
              //----2.驱动智能锁----------------------------   
             //--找智能锁通道--------------------------------------------------------------------------------
             FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr); //测试正确
             MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));

             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);
             
             
             //--再转发出去-------------
             if (MyLoginUser != null)
              {
                  this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
                  DriveToSmartLock(ParaSnapStr);
              }
              else
              {
                  //--否则原路或者走响应通道快速返回[0、1]-----------
                  NotFindLockResponseToMobile();


              }
                    

            
         }

         public void ResponseToMobile()
         {
             string CommandMessageStr = "remotesnap";
     
             //---1.找智能锁本身通道的路由表记录-------------------------------------
             //LoginUser MyLoginUser = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.FindLoginUserList(ref  this.MyReadWriteChannel.MyTCPClient);  
             //-------------------------------------
             // FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
             // LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
             
            
              MyLockIDStr = MyLoginUser.LockID;
              MyMobileIDStr = MyLoginUser.MobileID;

              Byte RecieveMessageResult= MyReadBuffers[22];//结果标志

              string SnapIDStr = SnapID.ToString(); 
              string ResultStr;
              if (RecieveMessageResult == 0)
              {
                  
                  //ImageFileSaveToFile();//1.

                  //SaveImageToDB();//2.
                  SaveImageToDBEx();

                  //SaveToALiYunOSSOTS();//3.
                  SaveToALiYunOSSOTSEx();//3.

                  //SendEMailNotify();//4.

                  ResultStr = "true";
                  if (string.IsNullOrEmpty(MyCompleteFileName)) MyCompleteFileName = "SMTP://" + MyAutoFileNameStr + ".jpg"; ;
                  ResultStr = ResultStr + "[" + SnapIDStr + "," + MyCompleteFileName + "]";
              }
              else
              {
                  ResultStr = "false";
                  ResultStr = ResultStr + "[]";
              }


              CommandMessageStr = CommandMessageStr + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];
                            

             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

                         
             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
            
             try
             {
                  
                 this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }



           
         }

         private void NotFindLockResponseToMobile()
         {
             string CommandMessageID = "remotesnap";
             string ResultStr = "false";
             ResultStr = ResultStr + "[11]";
             string CommandMessageStr;
             CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             //----1.保存数据库备查-------------------------------------------------------------------------------------------------------------
             //MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);

             try
             {
                 //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);//原路打回！

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误！");
             }

         }
                  
         private string GetDateTimeWeekIndex()
         {
             //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
             string DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
             string WeekIndexStr = DateTime.Now.ToString("dddd");
             string nReturn = null;
             switch (WeekIndexStr)
             {
                 case "星期一": nReturn = "1";
                     break;

                 case "星期二": nReturn = "2";
                     break;

                 case "星期三": nReturn = "3";
                     break;

                 case "星期四": nReturn = "4";
                     break;

                 case "星期五": nReturn = "5";
                     break;

                 case "星期六": nReturn = "6";
                     break;

                 case "星期日": nReturn = "7";
                     break;

                 default:
                     nReturn = "0";
                     break;

             }


             return DateTimeStr + nReturn;
         }

         private void DriveToSmartLock(string ParaSnapStr)
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 24;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0B20";
             }
             else
             {
                 HexSendMessageIDString = "200B";
             }


             //填充缓冲区信息头
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
                 MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
             }
             else
             {
                 MySendMessageBytes[0] = Convert.ToByte(HexSendLenghtString.Substring(0, 2), 16);
                 MySendMessageBytes[1] = Convert.ToByte(HexSendLenghtString.Substring(2, 2), 16);
             }

             MySendMessageBytes[2] = 2; //移动端请求

             MySendMessageBytes[8] = Convert.ToByte(HexSendMessageIDString.Substring(0, 2), 16);
             MySendMessageBytes[9] = Convert.ToByte(HexSendMessageIDString.Substring(2, 2), 16);



             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[10] = 1;
                 MySendMessageBytes[12] = 1;

             }
             else
             {
                 MySendMessageBytes[11] = 1;
                 MySendMessageBytes[13] = 1;
             }

              MySendMessageBytes[22] = 1;//抓拍标志



              MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private bool ImageFileSaveToFile()
         {
                
             
             MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();
             int StartIndex;
             FileStream MyRecieveFileStream;
             BinaryWriter MyBinaryWriter;
             MyCompleteFileName = "F:\\LockSnapImage\\" + MyLockIDStr + "\\" + MyAutoFileNameStr + ".jpg";
             MyRecieveFileStream = new FileStream(MyCompleteFileName, FileMode.Create, FileAccess.Write);
             MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);
             StartIndex = 39;
             MyBinaryWriter.Write(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);

             //SendEMailNotify(MyRecieveFileStream, MyAutoFileNameStr + ".jpg");//发邮件通知

             MyBinaryWriter.Close();
             MyRecieveFileStream.Close();
                  

             return true;



         }

         private void SaveImageToDB()
         {

             string LockID = MyLockIDStr;
             int SnapTypeID = 0;
             string SnapUUID = MyAutoFileNameStr;
             SnapID = 0;

             int StartIndex = 39;
             //MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
             byte[] MyImageData = new byte[MyRecieveCount - StartIndex];
             for (int i = 0; i < MyRecieveCount - StartIndex; i++)
             {

                 MyImageData[i] = MyReadBuffers[i + StartIndex];
             }

             ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             using (SqlConnection cn = new SqlConnection(CloudLockConnectString.ConnectionString))
             {
                 cn.ConnectionString = CloudLockConnectString.ConnectionString;
                 cn.Open();

                 using (SqlCommand GetMaxcmd = new SqlCommand())
                 {
                     GetMaxcmd.CommandText = "select Max(SnapID) from snap" + LockID;//89765432BCDA820";
                     GetMaxcmd.Connection = cn;
                     using (SqlDataReader dr = GetMaxcmd.ExecuteReader())
                     {
                         dr.Read();
                         SnapID = (long)dr.GetValue(0);


                     }
                 }
                 SnapID++;

                 using (SqlCommand Insertcmd = new SqlCommand())
                 {
                     // string MyCommandText = "insert into " + MySQLViewName + " (LockID,Number, Name, KeyStr, Date) values (@LockID,@Number, @Name, @KeyStr, @Date)";
                     Insertcmd.CommandText = "insert into snap" + LockID + " (LockID,SnapID, SnapTypeID, SnapUUID, Date,ViewFlag) values (@LockID,@SnapID, @SnapTypeID, @SnapUUID, @Date,@ViewFlag)";
                     SqlParameter parm = new SqlParameter();
                     parm.ParameterName = "@LockID";
                     parm.Value = LockID;
                     Insertcmd.Parameters.Add(parm);

                     parm = new SqlParameter();
                     parm.ParameterName = "@SnapID";
                     parm.Value = SnapID;
                     Insertcmd.Parameters.Add(parm);

                     parm = new SqlParameter();
                     parm.ParameterName = "@SnapTypeID";
                     parm.Value = SnapTypeID;//0,1,2
                     Insertcmd.Parameters.Add(parm);


                     parm = new SqlParameter();
                     parm.ParameterName = "@SnapUUID";
                     parm.Value = SnapUUID;
                     Insertcmd.Parameters.Add(parm);

                     parm = new SqlParameter();
                     parm.ParameterName = "@ViewFlag";
                     parm.Value = 1;
                     Insertcmd.Parameters.Add(parm);

                     parm = new SqlParameter();
                     parm.ParameterName = "@Date";
                     parm.Value = DateTime.Now;
                     Insertcmd.Parameters.Add(parm);

                     Insertcmd.Connection = cn;
                     Insertcmd.ExecuteNonQuery();
                 }


                 using (SqlCommand Updatecmd = new SqlCommand())
                 {
                     Updatecmd.CommandText = "Update snap" + LockID + " Set Image = @Image " + "WHERE SnapID = @SnapID";
                     SqlParameter parm = new SqlParameter();
                     parm.ParameterName = "@SnapID";
                     parm.Value = SnapID;
                     Updatecmd.Parameters.Add(parm);
                     parm = new SqlParameter();
                     parm.ParameterName = "@Image";
                     parm.Value = MyImageData;// MyMemoryStreamContent.GetBuffer();
                     Updatecmd.Parameters.Add(parm);

                     Updatecmd.Connection = cn;
                     Updatecmd.ExecuteNonQuery();
                 }




             }

             this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库成功");


         }

         private void SaveImageToDBEx()
         {

             string LockID = MyLockIDStr;
             int SnapTypeID = 0;
             string SnapUUID = MyAutoFileNameStr;
             SnapID = 0;

               //int StartIndex = 22;
             //MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
             //byte[] MyImageData = new byte[MyRecieveCount - StartIndex];
             //for (int i = 0; i < MyRecieveCount - StartIndex; i++)
             //{
             //
             //    MyImageData[i] = MyReadBuffers[i + StartIndex];
            // }

             int StartIndex = 39;
             //MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
             byte[] MyImageData = new byte[MyRecieveCount - StartIndex];
             for (int i = 0; i < MyRecieveCount - StartIndex; i++)
             {

                 MyImageData[i] = MyReadBuffers[i + StartIndex];
             }
                  /*
        using (SqlConnection cn = new SqlConnection(CloudLockConnectString.ConnectionString))
        {
            cn.ConnectionString = CloudLockConnectString.ConnectionString;
            cn.Open();

            using (SqlCommand GetMaxcmd = new SqlCommand())
            {
                GetMaxcmd.CommandText = "select Max(SnapID) from snap" + LockID;//89765432BCDA820";
                GetMaxcmd.Connection = cn;
                using (SqlDataReader dr = GetMaxcmd.ExecuteReader())
                {
                    dr.Read();
                    SnapID = (long)dr.GetValue(0);


                }
            }
           */

             //SnapID++;

             ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             SqlConnection MySqlConnection = new SqlConnection(CloudLockConnectString.ConnectionString);
             MySqlConnection.Open();

             SqlCommand MySqlCommand = new SqlCommand("AddSnapImage", MySqlConnection);
             MySqlCommand.CommandType = CommandType.StoredProcedure;

             SqlParameter parm = new SqlParameter();
             parm.ParameterName = "@LockID";
             parm.Value = LockID;
             MySqlCommand.Parameters.Add(parm);

             parm = new SqlParameter();
             parm.ParameterName = "@SnapTypeID";
             parm.Value = SnapTypeID;//0,1,2
             MySqlCommand.Parameters.Add(parm);


             parm = new SqlParameter();
             parm.ParameterName = "@SnapUUID";
             parm.Value = SnapUUID;
             MySqlCommand.Parameters.Add(parm);

             MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
             MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
             MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
             MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

             int MyCount = MySqlCommand.ExecuteNonQuery();
             int ReturnValue = (int)MySqlCommand.Parameters["@ReturnValue"].Value;
             SnapID = (long)MySqlCommand.Parameters["@ReturnSnapID"].Value;

             if (ReturnValue == 0)
             {
                 /*-----图像不存数据库----------------------------
                 SqlCommand MyUpdateImagelCommand = new SqlCommand();
                 MyUpdateImagelCommand.CommandText = "Update snap" + LockID + " Set Image = @Image " + "WHERE SnapID = @SnapID";

                 parm = new SqlParameter();
                 parm.ParameterName = "@SnapID";
                 parm.Value = SnapID;
                 MyUpdateImagelCommand.Parameters.Add(parm);
                 parm = new SqlParameter();
                 parm.ParameterName = "@Image";
                 parm.Value = MyImageData;// MyMemoryStreamContent.GetBuffer();
                 MyUpdateImagelCommand.Parameters.Add(parm);

                 MyUpdateImagelCommand.Connection = MySqlConnection;
                 MyUpdateImagelCommand.ExecuteNonQuery();
                 */
                 MySqlConnection.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库成功");
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到数据库成功");

             }
             else
             {
                 MySqlConnection.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库失败");
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到数据库失败");
             }


         }

         private bool SaveToALiYunOSSOTS()
         {
             //string MyFileNameStr = Guid.NewGuid().ToString().ToUpper();
             int StartIndex;
             //http://hnlylgj.oss-cn-hangzhou.aliyuncs.com/89765432BCDA820/B507D6A4-BC67-4A0F-97F3-B3A00C115450.jpg
             string MySaveToOSSFileName = MyLockIDStr + "/" + MyAutoFileNameStr + ".jpg";
             MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;
             StartIndex = 39;
             MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);

             string MyAccessID = "7je124e077a86vzr6zqtpazy";
             string MyAccessKey = "SQYaadscvGhFuTnm3U99JugIpQw=";
             string MyBbucketName = "hnlylgj";
             try
             {
                 ObjectMetadata MyObjectMetaData = new ObjectMetadata();
                 MyObjectMetaData.ContentType = "image/jpeg";

                 OssClient MyOSSClient = new OssClient(MyAccessID, MyAccessKey);

                 MyOSSClient.PutObject(MyBbucketName, MySaveToOSSFileName, MyMemoryStreamContent, MyObjectMetaData);

                 MyMemoryStreamContent.Close();
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS成功！");
                 return true;

             }
             catch (Exception AnyException)
             {
                 MyMemoryStreamContent.Close();
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS失败！");
                 return false;
             }

             

         }

         private bool SaveToALiYunOSSOTSEx()
         {
             ALiYunOSSOTSAPI MyALiYunOSSOTSAPI;
             MyALiYunOSSOTSAPI = new ALiYunOSSOTSAPI();
             int StartIndex = 39;
             MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
             string MySaveToOSSFileName = MyLockIDStr + "/" + MyAutoFileNameStr + ".jpg";
             MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;
          
             try
             {              
                                 
                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.PrefixStr = this.MyLockIDStr + "/";
                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.FileKeyName = MyAutoFileNameStr + ".jpg"; 

                 MyALiYunOSSOTSAPI.PrivateUploadFile3(ref MyMemoryStreamContent);
                 MyMemoryStreamContent.Close();
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 return true;

             }
             catch (Exception AnyException)
             {
                 MyMemoryStreamContent.Close();
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 return false;
             }



         }
       
         private bool SendEMailNotify()
         {
             int StartIndex = 39;
             string SendFileNameStr = MyAutoFileNameStr + ".jpg";
             MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);

             LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc("hnlylgjx@qq.com", MyMemoryStreamContent, SendFileNameStr);
             MySendMailProc.StartSendMail();
             MyMemoryStreamContent.Close();
             if (MySendMailProc.SendFlagID)
             {
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "发送邮件通知成功！");
             }
             else
             {
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "发送邮件通知失败！");
             }
             return MySendMailProc.SendFlagID;

         }

    
    }
}
