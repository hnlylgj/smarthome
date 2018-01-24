using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using SmartBusServiceLib;
using ALiYunOSSOTSAPILib;
using Aliyun.OpenServices.OpenStorageService;
using LGJAsynchSocketService.AsynchSQLServerIO;
using LGJAsynchSocketService.AsynchFileIO;
using LGJAsynchSocketService.LGJAynchWork;
namespace LGJAsynchSocketService.LockServerLib
{
    public class ImageFileReceiveProcess
    {
        //AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         LoginUser MyLoginUser;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;
         byte[] RecieveFileBuffers;
         uint RecieveFileLenght;

         string MyCompleteFileName;
         string MyAutoFileNameStr;
         long SnapID;
         int SnapTypeID;
         FileStream MyRecieveFileStream;
         BinaryWriter MyBinaryWriter;

         public ImageFileReceiveProcess(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            //新版
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = MeLoginUser.MyReadWriteSocketChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public ImageFileReceiveProcess(AsynchLockServerSocketService InAsynchLockServerSocketService, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
         {
             //旧版
             this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
             this.MyReadWriteChannel = InputReadWriteObject;
             this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
             this.MyRecieveCount = InputRecieveCount;
         }

         public void CompleteCommand()
         {
             if (this.MyLoginUser.LoopReadCount == 0)
             {
                 //第一次
                 if (MyReadWriteChannel.MyReadBuffers[14] == 0xFF)
                 {
                     //1.后续分块不包括消息头:A模式--我的定义
                     MyLoginUser.FileReadMode = 1;
                     CompleteCommandA();

                 }
                 else
                 {
                    //2;后续分块包括消息头：B模式--DIGI定义
                    MyLoginUser.FileReadMode = 2;
                     CompleteCommandB();

                 }

             }
             else
             {
                 //第二次
                 if (MyLoginUser.FileReadMode == 1)
                 {
                     CompleteCommandA();
                 }
                 if (MyLoginUser.FileReadMode == 2)
                 {
                     CompleteCommandB();
                 }
             }



         }
         public void CompleteCommandA()
         {

             if (this.MyLoginUser.WorkStatus == 0)
             {
                 //--第一次传输-------------
                 this.MyLoginUser.LoopReadCount = 1;
                 //--1.消息类别判断-------------------------------------------------------------------------------------------------------------
                 string HexMessageTypeIDStr;
                 byte[] RecieveFileByteUnit = this.MyReadBuffers;
                 uint FileContentIndex=0;
                
                 uint SelfTagMessageLenght;
          
                 //byte[] RecieveFileBuffers;
                 uint NextSaveIndex;
                 if (MyAsynchLockServerSocketService.DataFormateFlag)
                 {
                     HexMessageTypeIDStr = string.Format("{0:X2}", RecieveFileByteUnit[9]) + string.Format("{0:X2}", RecieveFileByteUnit[8]);
                 }
                 else
                 {
                     HexMessageTypeIDStr = string.Format("{0:X2}", RecieveFileByteUnit[8]) + string.Format("{0:X2}", RecieveFileByteUnit[9]);
                 }

                 if (HexMessageTypeIDStr == "3007")
                 {
                     FileContentIndex = 38;
                     SnapTypeID = 0;//收件

                 }
                 if (HexMessageTypeIDStr == "3009")
                 {
                     FileContentIndex = 40;
                     SnapTypeID = 1;//异常

                 }
                 if (HexMessageTypeIDStr == "200C")
                 {
                     FileContentIndex = 39;
                     SnapTypeID = 2;//监控

                 }

                 string RecieveFileLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[21]);
                 RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[20]);
                 RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[19]);
                 RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[18]);
                 SelfTagMessageLenght = Convert.ToUInt32(RecieveFileLenghtStr, 16);
                 RecieveFileLenght = SelfTagMessageLenght; //应该接收的图像文件总长度             

                 NextSaveIndex = (uint)this.MyRecieveCount - FileContentIndex;//第一次接收的文件字节数！
               
                 this.MyLoginUser.FileByteBuffer((int)RecieveFileLenght);

                 //--填充所接收的文件字节------------------------------------------------------
                 for (int i = 0; i < NextSaveIndex; i++)
                 {

                     this.MyLoginUser.MyByteBuffer[i] = RecieveFileByteUnit[FileContentIndex + i];

                 }
                 //------------------------------------------------------------------------------
                if (NextSaveIndex == RecieveFileLenght)//如一次传输完成;
                 {                   
                     //===回显基本信息===================================================    
                     string BaseMessageString = string.Format("标准文件传输[{0}]", NextSaveIndex);
                     //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                     string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);

                    MyAsynchLockServerSocketService.DisplayResultInfor(2, string.Format(MyTimeMarker + "[{0}]{1}", this.MyLoginUser.SocketInfor, BaseMessageString));
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker+"[{0}]一次传输完成接收图像流！", this.MyLoginUser.LockID));
                                       
                     
                     this.MyLoginUser.WorkStatus = NextSaveIndex;
                     this.MyLoginUser.SnapTypeID = SnapTypeID;

                     //SyncStartImageStreamSave();
                     
                     AsyncStartImageStreamSave();

                 }
                 else //如需多次传输完成
                 {
                     //===回显基本信息===================================================
                     string BaseMessageString = string.Format("扩展文件传输[{0}]", SelfTagMessageLenght);
                     //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                      string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                     MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]开始接收图像流分包[{1}][{2}]", this.MyLoginUser.LockID, this.MyLoginUser.LoopReadCount, NextSaveIndex));
                     MyAsynchLockServerSocketService.DisplayResultInfor(2, string.Format(MyTimeMarker + "[{0}]{1}", this.MyLoginUser.SocketInfor, BaseMessageString));
                     
                     this.MyLoginUser.WorkStatus = NextSaveIndex;
                     this.MyLoginUser.WorkCountSum = RecieveFileLenght;//保存图像文件总长度
                     this.MyLoginUser.SnapTypeID = SnapTypeID;
                     


                 }
                 

             }
             else
             {
                 //----第2...n次传输---------------------
                 this.MyLoginUser.LoopReadCount++;
                 //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                 string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]连续接收图像流分包[{1}][{2}]", this.MyLoginUser.LockID, this.MyLoginUser.LoopReadCount, MyRecieveCount));
              
                 for (uint i = 0; i < MyRecieveCount; i++)
                 {
                     this.MyLoginUser.MyByteBuffer[this.MyLoginUser.WorkStatus + i] = this.MyReadBuffers[i];

                 }
                 this.MyLoginUser.WorkStatus = this.MyLoginUser.WorkStatus + (uint)MyRecieveCount;
                 if (this.MyLoginUser.WorkStatus == this.MyLoginUser.WorkCountSum)
                 {
                     
                     //SyncStartImageStreamSave();
                     AsyncStartImageStreamSave();
                    
                 }
                 

             }
  
           
          

         }

         public void CompleteCommandB()
        {

            if (this.MyLoginUser.WorkStatus == 0)
            {
                //--第一次传输-------------------------------------------------------------------------
                this.MyLoginUser.LoopReadCount = 1;
                //--1.消息类别判断-------------------------------------------------------------------------------------------------------------
                string HexMessageTypeIDStr;
                byte[] RecieveFileByteUnit = this.MyReadBuffers;
                uint FileContentIndex = 0;
                uint PhysicalLockMsgCount;
                uint NextSaveIndex=0;
                if (MyAsynchLockServerSocketService.DataFormateFlag)
                {
                    HexMessageTypeIDStr = string.Format("{0:X2}", RecieveFileByteUnit[9]) + string.Format("{0:X2}", RecieveFileByteUnit[8]);
                }
                else
                {
                    HexMessageTypeIDStr = string.Format("{0:X2}", RecieveFileByteUnit[8]) + string.Format("{0:X2}", RecieveFileByteUnit[9]);
                }

                if (HexMessageTypeIDStr == "3007")
                {
                   
                    FileContentIndex = 38;
                    SnapTypeID = 0;

                }
                if (HexMessageTypeIDStr == "3009")
                {

                    FileContentIndex = 40;
                    SnapTypeID = 1;

                }
                if (HexMessageTypeIDStr == "200C")
                {

                    FileContentIndex = 39;
                    SnapTypeID = 2;

                }

                string RecieveMsgCountStr = string.Format("{0:X2}", RecieveFileByteUnit[11]);
                RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", RecieveFileByteUnit[10]);
                PhysicalLockMsgCount = Convert.ToUInt16(RecieveMsgCountStr, 16);//总包数
                

                NextSaveIndex = (uint)this.MyRecieveCount - FileContentIndex - 1;//第一次接收的文件字节数！去掉效验字节
                              
                if (PhysicalLockMsgCount == 1)//一次传输完成;
                {
                    this.MyLoginUser.FileByteBuffer((int)NextSaveIndex);//刚好分配内存空间
                    //--填充所接收的文件字节-----------------------------------------------------
                    for (int i = 0; i < NextSaveIndex; i++)
                    {

                        this.MyLoginUser.MyByteBuffer[i] = RecieveFileByteUnit[FileContentIndex + i];

                    }
                    string BaseMessageString = string.Format("标准文件传输[{0}]", NextSaveIndex);
                    //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                    string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                    MyAsynchLockServerSocketService.DisplayResultInfor(2, string.Format(MyTimeMarker + "[{0}]{1}", this.MyLoginUser.SocketInfor, BaseMessageString));
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]一次传输完成接收图像流！", this.MyLoginUser.LockID));

                    //SyncStartImageStreamSave();
                    AsyncStartImageStreamSave();

                }

                else //如需多次传输完成则保存起来以下一次累加
                {
                    //===回显基本信息===================================================
                    string BaseMessageString = string.Format("扩展文件传输包数[{0}]", PhysicalLockMsgCount);
                    //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                    string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]开始接收图像流分包[{1}][{2}]", this.MyLoginUser.LockID, this.MyLoginUser.LoopReadCount, NextSaveIndex));
                    MyAsynchLockServerSocketService.DisplayResultInfor(2, string.Format(MyTimeMarker + "[{0}]{1}", this.MyLoginUser.SocketInfor, BaseMessageString));

                    this.MyLoginUser.WorkStatus = NextSaveIndex;
                    this.MyLoginUser.WorkCountSum = PhysicalLockMsgCount;
                    this.MyLoginUser.SnapTypeID = SnapTypeID;
                    this.MyLoginUser.FileByteBuffer(65536);//固定分配内存空间
                    //--填充所接收的文件字节-----------------------------------------------------
                    for (int i = 0; i < NextSaveIndex; i++)
                    {

                        this.MyLoginUser.MyByteBuffer[i] = RecieveFileByteUnit[FileContentIndex + i];

                    }
                    //RecieveFileBuffers.CopyTo(this.MyLoginUser.MyByteBuffer, 0);


                }


            }
            else
            {
                //----第2...n次传输---------------------
                this.MyLoginUser.LoopReadCount++;
                uint FileContentIndex = 0;
                uint NextSaveIndex = 0;
                if(MyLoginUser.SnapTypeID==0)
                {
                    FileContentIndex = 38;
                    //MyRecieveCount = MyRecieveCount - 38 - 1;//去掉消息头与效验字节
                }
                if (MyLoginUser.SnapTypeID == 1)
                {
                    FileContentIndex = 40;
                    //MyRecieveCount = MyRecieveCount - 40 - 1;
                }
                if (MyLoginUser.SnapTypeID == 2)
                {
                    FileContentIndex = 39;
                    //MyRecieveCount = MyRecieveCount - 39 - 1;
                }
                NextSaveIndex = (uint)this.MyRecieveCount - FileContentIndex - 1;//第一次接收的文件字节数,去掉效验字节
               // string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]连续接收图像流分包[{1}][{2}]", this.MyLoginUser.LockID, this.MyLoginUser.LoopReadCount, NextSaveIndex));

                for (uint i = 0; i < MyRecieveCount; i++)
                {
                    this.MyLoginUser.MyByteBuffer[this.MyLoginUser.WorkStatus + i] = this.MyReadBuffers[FileContentIndex+i];

                }
                this.MyLoginUser.WorkStatus = this.MyLoginUser.WorkStatus + (uint)MyRecieveCount;
               
               if (this.MyLoginUser.WorkCountSum == this.MyLoginUser.LoopReadCount)//总包数控制循环计数
                {

                    //SyncStartImageStreamSave();
                    AsyncStartImageStreamSave();
                }


            }

          

        }


         private void SyncStartImageStreamSave()
         {
             MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();
             if (this.MyLoginUser.FileReadMode == 2)//B模式下多一步
             {
                 RecieveFileLenght = this.MyLoginUser.WorkStatus;
                 RecieveFileBuffers = new byte[RecieveFileLenght];
                 for (uint i = 0; i < RecieveFileLenght; i++)
                 {
                     RecieveFileBuffers[i] = this.MyLoginUser.MyByteBuffer[i];//B模式下：this.MyLoginUser.WorkStatus不一定等于this.MyLoginUser.MyByteBuffer的Lenght

                 }
             }
             else//A模式
             {
                 RecieveFileLenght = this.MyLoginUser.WorkStatus;
                 RecieveFileBuffers = this.MyLoginUser.MyByteBuffer;

             }
             //----------------------------------------------------------

              SaveToFile();
             //SaveImageToDB();
            
             SaveSnapParaToDB();

             if (MyAsynchLockServerSocketService.SaveSnapFlag == 0)
             {

                 SaveSnapImageToDB();
             }
             else
             {

                 SaveSnapImageToALiYunOSSEx();
             }


             if (SnapTypeID < 2)
             {
                 //SendEMailNotify();
                 //if (this.MyLoginUser.LockID == "88886666ABCD111")
                 SendEMailNotifyEx();
             }

             ResponseToMobile();
             ReplyMessageToLock();

             FinallyDisposed();




         }

         private void AsyncStartImageStreamSave()
         {
             MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();
             this.MyLoginUser.TempString = MyAutoFileNameStr;
             if (this.MyAsynchLockServerSocketService.IsSaveToFile)
             {
                 AsynchProcessFile MyAsynchProcessFile = new AsynchProcessFile();
                 MyAsynchProcessFile.SnapImageSaveFile(this.MyLoginUser, this.MyAsynchLockServerSocketService);//异步过滤操作文件

             }
             else
             {

                 LGJAsynchAccessDBase MyLGJAsynchAccessDBase = new LGJAsynchAccessDBase();
                 MyLGJAsynchAccessDBase.AsynchSaveSnapPara(this.MyLoginUser, this.MyAsynchLockServerSocketService);//异步过滤直接操作数据库与邮件
             }

            //AsynchWorkProcess MyAsynchWorkProcess = new AsynchWorkProcess();
            //MyAsynchWorkProcess.AsyncStartSendEMail(this.MyLoginUser, this.MyAsynchLockServerSocketService);//测试异步发送邮件过滤操作


            string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
            MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker+string.Format("[当前核心进程线程工作模式：异步并行]"));

           

             //SaveToFile();
             //SaveImageToDB();

             //SaveSnapParaToDB();

            /// if (MyAsynchLockServerSocketService.SaveSnapFlag == 0)
            // {

             //    SaveSnapImageToDB();
            /// }
            // else
             //{
//
             ///    SaveSnapImageToALiYunOSSEx();
             //}


             //if (SnapTypeID < 2)
             //{
                 //SendEMailNotify();
                 //if (this.MyLoginUser.LockID == "88886666ABCD111")
               //  SendEMailNotifyEx();
             //}

            // ResponseToMobile();
            
            /// ReplyMessageToLock();

            // FinallyDisposed();




         }
       
        private bool SaveToFile()
         {
             MyCompleteFileName = "C:\\LockSnapImage\\" + this.MyLoginUser.LockID + "_" + MyAutoFileNameStr + ".jpg";
             MyRecieveFileStream = new FileStream(MyCompleteFileName, FileMode.Create, FileAccess.Write);
             MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);
             try
             {
                 //MyBinaryWriter.Write(RecieveFileBuffers, 0, (int)RecieveFileLenght);
                 MyBinaryWriter.Write(RecieveFileBuffers);                   
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到Files成功！");
                 return true;   
             }
             catch (Exception AnyException)
             {

                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到Files失败！");
                 return false;
             }
            finally
             {
                MyBinaryWriter.Close();
                MyRecieveFileStream.Close(); 

              }
           


         }

         private void SaveSnapParaToDB()
         {

             SnapID = 0;
             SnapImage MySnapImage = new SnapImage();
             MySnapImage.LockID = this.MyLoginUser.LockID;
             MySnapImage.SnapTypeID = this.MyLoginUser.SnapTypeID;
             MySnapImage.SnapUUID = MyAutoFileNameStr;

             SnapManager MySnapManager = new SnapManager();
             int ReturnValue = MySnapManager.InsertSnap(MySnapImage, ref SnapID);
             if (ReturnValue == 0)
             {

                 string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存参数到数据库成功", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


             }
             else
             {

                 string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存参数到数据库失败", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
           
             }


         }

         private void SaveSnapImageToDB()
         {
             SnapManager MySnapManager = new SnapManager();
             int ReturnValue = MySnapManager.UpdateSnapImage(this.MyLoginUser.LockID, RecieveFileBuffers, this.SnapID);
             if (ReturnValue == 1)//1:默认为正确；
             {

                 string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到数据库成功", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


             }
             else
             {

                 string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到数据库失败", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
            
             }

         }

         private bool SaveSnapImageToALiYunOSSEx()
         {
             ALiYunOSSOTSAPI MyALiYunOSSOTSAPI;
             MyALiYunOSSOTSAPI = new ALiYunOSSOTSAPI();

             string MySaveToOSSFileName = this.MyLoginUser.LockID + "/" + MyAutoFileNameStr + ".jpg";
             MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;
             MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);

             bool SaveFlag = false;
             string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
             try
             {

                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.PrefixStr = this.MyLoginUser.LockID + "/";
                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.FileKeyName = MyAutoFileNameStr + ".jpg";

                 MyALiYunOSSOTSAPI.PrivateUploadFile3(ref MyMemoryStreamContent);

                 //MyMemoryStreamContent.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";


                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到OSS:[{2}][{3}]", this.MyLoginUser.LockID, this.MyLoginUser.SocketInfor, RecieveFileLenght, MyALiYunOSSOTSAPI.ResultMessageStr));

                 SaveFlag = true;


             }
             catch (Exception AnyException)
             {
                 //MyMemoryStreamContent.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]扩展保存数据到OSS:[{1}]", MyLockIDStr, MyALiYunOSSOTSAPI.ResultMessageStr));

                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到OSS:[{2}][{3}]", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, RecieveFileLenght, MyALiYunOSSOTSAPI.ResultMessageStr));

                 SaveFlag = false;

             }

             finally
             {

                 MyMemoryStreamContent.Close();

             }
             return SaveFlag;

         }

        private void SaveImageToDB()
         {
             //AddSnapImage]
             string LockID = this.MyLoginUser.LockID;
             int SnapTypeID = 0;
             string SnapUUID = MyAutoFileNameStr;
             SnapID = 0;

             int StartIndex = 22;
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

             string LockID = this.MyLoginUser.LockID;
             int SnapTypeID = 0;
             string SnapUUID = MyAutoFileNameStr;
             SnapID = 0;

             int StartIndex = 22;
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
                          /*--图像不存数据库-----------------------------------------------
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
                         //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到数据库成功");
                         string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                         this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]标准保存数据到数据库成功", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));

                     }
                     else
                     {
                         MySqlConnection.Close();
                            //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库失败");
                         //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到数据库失败");
                         string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                         this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]标准保存数据到数据库失败", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
                     }


         }
     
         private bool SaveToALiYunOSSOTS()
         {
             //string MyFileNameStr = Guid.NewGuid().ToString().ToUpper();
             int StartIndex;

            //http://hnlylgj.oss-cn-hangzhou.aliyuncs.com/89765432BCDA820/B507D6A4-BC67-4A0F-97F3-B3A00C115450.jpg

             string MySaveToOSSFileName = this.MyLoginUser.LockID + "/" + MyAutoFileNameStr + ".jpg";
             MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;     
             StartIndex = 22;
             MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
             //MemoryStream MyMemoryStreamEMail = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);
           

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
             int StartIndex = 22;
             MemoryStream MyMemoryStreamContent = new MemoryStream(MyReadBuffers, StartIndex, MyRecieveCount - StartIndex);

             string MySaveToOSSFileName = this.MyLoginUser.LockID + "/" + MyAutoFileNameStr + ".jpg";
             MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;

             bool SaveFlag = false;
             string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
             try
             {

                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.PrefixStr = this.MyLoginUser.LockID + "/";
                 MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.FileKeyName = MyAutoFileNameStr + ".jpg";

                 MyALiYunOSSOTSAPI.PrivateUploadFile3(ref MyMemoryStreamContent);
                  //MyMemoryStreamContent.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]标准保存数据到OSS:[{2}][{3}]", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyALiYunOSSOTSAPI.ResultMessageStr, MyRecieveCount - StartIndex));

                 
                 SaveFlag = true;

             }
             catch (Exception AnyException)
             {
                 //MyMemoryStreamContent.Close();
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                 //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                 //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}]标准保存数据到OSS:[{1}]", MyLockIDStr, MyALiYunOSSOTSAPI.ResultMessageStr));

                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]标准保存数据到OSS:[{2}][{3}]", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyALiYunOSSOTSAPI.ResultMessageStr, MyRecieveCount - StartIndex));

                 
                 SaveFlag = false;
             }

             finally
             {
                 MyMemoryStreamContent.Close();

             }

             return SaveFlag;
         }

         private bool SendEMailNotify()
         {
              int StartIndex=22;
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

         private bool SendEMailNotifyEx()
         {
              /*
            int StartIndex=22;
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
            */

             if (this.MyLoginUser.LockID == "88886666ABCD110")//Test
             {

                  //string SendFileNameStr = MyAutoFileNameStr + ".jpg";
                 //MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);
                 //LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc("hnlylgjx@qq.com", MyMemoryStreamContent, SendFileNameStr);
                 //MyMemoryStreamContent.Close();
                
                 LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc("hnlylgjx@qq.com", SnapID.ToString(), this.MyLoginUser.LockID);
               
                 MySendMailProc.StartSendMail();//同步发送邮件

                 if (MySendMailProc.SendFlagID)
                 {
                     string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                     this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]标准发送邮件通知成功！", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
                     //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "发送邮件通知成功！");
                 }
                 else
                 {
                     string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                     this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]标准发送邮件通知失败！", this.MyLoginUser.LockID, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
                     //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "发送邮件通知失败！");
                 }
                 return MySendMailProc.SendFlagID;
             }
             return false;




         }

         private void ReplyMessageToLock()
         {
             //如果主动上传发送消息响应到云锁，否则不回应  
             //if (HexMessageTypeIDStr != "3007") return;
             if (SnapTypeID == 2)
             {
                 return;
             }
             
             int MySendByteCount = 24;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;

             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {

                 HexSendMessageIDString = SnapTypeID == 0 ? "0830" : "0A30";
             }
             else
             {
                 HexSendMessageIDString = SnapTypeID == 0 ? "3008" : "300A";
             }

             //填充字节信息头
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



             MySendMessageBytes[2] = 1;

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

             //填充命令处理结果

             MySendMessageBytes[22] = 0;////ProcessID;// 0;成功;//-1：FF失败

             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes);

         }
       
         private void ResponseToMobileX()
         {
             string CommandMessageStr = "snapimage";
                          
             string SnapIDStr = SnapID.ToString(); 

             if (string.IsNullOrEmpty(MyCompleteFileName)) MyCompleteFileName = "SMTP://" + MyAutoFileNameStr + ".jpg"; ;
             CommandMessageStr = CommandMessageStr + "#" + this.MyLoginUser.MobileID + "-" + this.MyLoginUser.LockID + "#[" + SnapIDStr + "," + MyCompleteFileName + "]!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 250;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }
               
             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
             try
             {

                 this.NewReadWriteChannel = this.MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;

                 this.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }

                    


         }

         private void ResponseToMobile()
         {
             
             //如果200C是被动上传不会响应云锁
             string CommandMessageID = "remotesnap";
             if (SnapTypeID == 0)
             {
                 CommandMessageID = "mailsnap";
             }
             if (SnapTypeID == 1)
             {
                 CommandMessageID = "exceptsnap";
             }
             if (SnapTypeID == 2)
             {
                 CommandMessageID = "remotesnap";
             }
         

             string ResultStr;
             string SnapIDStr = SnapID.ToString();

             if (SnapTypeID < 2)//抓拍
             {

                 ResultStr = "[" + SnapIDStr + "," + MyCompleteFileName + "]";
             }
             else//远拍
             {
                 ResultStr = "true";
                 ResultStr = ResultStr + "[" + SnapIDStr + "," + MyCompleteFileName + "]";
             }

             string CommandMessageStr;
             CommandMessageStr = CommandMessageID + "#" + MyLoginUser.MobileID + "-" + MyLoginUser.LockID + "#" + ResultStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];

             //255:云锁响应移动端消息；250：云锁主动上传消息到移动端
             if (SnapTypeID < 2)
             {
                 MySendMessageBytes[2] = 250;
             }
             else
             {
                 MySendMessageBytes[2] = 255;
             }


             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }


             //----1.保存数据库备查-------------------------------------------------------------------------------------------------------------
             MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLoginUser.LockID, CommandMessageID, CommandMessageStr);

             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------

             try
             {
                 //LockServerLib.FindMobileChannel MyBindedMobileChannel = new LockServerLib.FindMobileChannel(MyMobileIDStr);
                 //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;

                 this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }



         }
        
         private void FinallyDisposed()
         {
                        
             this.MyLoginUser.WorkCountSum = 0;
             this.MyLoginUser.WorkStatus = 0;
             this.MyLoginUser.LoopReadCount = 0;
             this.MyLoginUser.MyByteBuffer = null;
             this.RecieveFileBuffers = null;

         }


    }
}
