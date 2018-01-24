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

namespace LGJAsynchSocketService.LockServerLib
{
   public class LongFileReceiveProc
    {
        public uint LoopReadCount;
        public uint RecieveMessageLenght;
        private uint SelfTagMessageLenght;
        private uint RecieveFileLenght;
        private uint FileContentIndex;
        private byte[] RecieveFileBuffers;
        private uint NextSaveIndex;
        public string MyLockIDStr;
        public string MyMobileIDStr;
        public string SocketInfor;


        //-----兼容物理锁-----------------------------------------------------
        public int PhysicalLockMsgCount;
        int PackgeIndex;

        private byte[] RecieveFileBuffersEx;

       //---------------------------------------------------------------------
        private string MyCompleteFileName;
        private string HexMessageTypeIDStr;
        private string MyAutoFileNameStr;

        long SnapID;
        int SnapTypeID;

        //AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
        public AsynchLockServerSocketService MyAsynchLockServerSocketService;
        public SocketServiceReadWriteChannel MyReadWriteChannel;
        public SocketServiceReadWriteChannel NewReadWriteChannel;

        #region ------构造函数-----------------------------------------------------------

        public LongFileReceiveProc()
        {
            this.LoopReadCount = 0;
            this.RecieveMessageLenght = 0;
            this.SelfTagMessageLenght = 0;
            this.NextSaveIndex = 0;
            this.RecieveFileLenght = 0; 
          
        }

        public LongFileReceiveProc(string InLockIDStr)
        {
            this.LoopReadCount = 0;
            this.RecieveMessageLenght = 0;
            this.SelfTagMessageLenght = 0;
            this.NextSaveIndex = 0;
            this.RecieveFileLenght = 0;
            this.MyLockIDStr = InLockIDStr;

        }

        public LongFileReceiveProc(string InSocketInfor, string InLockIDStr, string InMobileIDStr)
        {
            this.LoopReadCount = 0;
            this.RecieveMessageLenght = 0;
            this.SelfTagMessageLenght = 0;
            this.NextSaveIndex = 0;
            this.RecieveFileLenght = 0;
            this.MyLockIDStr = InLockIDStr;
            this.MyMobileIDStr = InMobileIDStr; 
            this.SocketInfor = InSocketInfor;

        }        

        #endregion ----------------------------------------------------------------------

        public void StartLongFileReceive(byte[] RecieveFileByteUnit, uint MyRecieveCount)
        {
            //--虚拟锁类型---------------------------------------------------------------------------------------------------------------------
            if (LoopReadCount == 1)  //第一次传输
            {
                //--0.应答确认-----------------------------------------------------------------------------------------------------------------
                ////MyAsynchLockServerSocketService.CloudServerAckToLock(MyReadWriteChannel); 
                //--1.消息类别判断-------------------------------------------------------------------------------------------------------------
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
                    //FileContentIndex = 22;
                    FileContentIndex = 38;//加上另外16个字节;
                    SnapTypeID = 0;

                }
                if (HexMessageTypeIDStr == "3009")
                {
                  
                    FileContentIndex = 40;//加上另外18个字节;
                    SnapTypeID = 1;

                }
                if (HexMessageTypeIDStr == "200C")
                {
                   
                    FileContentIndex = 39;//加上另外17个字节;
                    SnapTypeID = 2;

                }

               
                string RecieveFileLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[21]);
                RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[20]);
                RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[19]);
                RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[18]);
                SelfTagMessageLenght = Convert.ToUInt32(RecieveFileLenghtStr, 16);
                RecieveFileLenght = SelfTagMessageLenght;
            
                /*
                if (RecieveFileByteUnit[15] == 0xFF)
                {
                    //--虚拟锁标志！
                    string RecieveFileLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[21]);
                    RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[20]);
                    RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[19]);
                    RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[18]);
                    SelfTagMessageLenght = Convert.ToUInt32(RecieveFileLenghtStr, 16);
                    RecieveFileLenght = SelfTagMessageLenght;

                }
                else
                {
                    //---兼容物理锁----   
                    string RecieveMsgCountStr = string.Format("{0:X2}", RecieveFileByteUnit[11]);
                    RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", RecieveFileByteUnit[10]);
                    PhysicalLockMsgCount = Convert.ToInt16(RecieveMsgCountStr, 16);

                    string RecieveFileLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[1]);
                    RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[0]);
                    SelfTagMessageLenght = Convert.ToUInt32(RecieveFileLenghtStr, 16);
                    RecieveFileLenght = SelfTagMessageLenght - FileContentIndex;


                }
             */
                 
                //RecieveFileLenght = SelfTagMessageLenght - FileContentIndex;
                //RecieveFileLenght = SelfTagMessageLenght;


              
                RecieveFileBuffers = new byte[RecieveFileLenght];

                NextSaveIndex = MyRecieveCount - FileContentIndex;//第一次接收的文件字节数！
               
                //--填充所接收的文件字节-----------------------------------------------------
                for (int i = 0; i < NextSaveIndex; i++)
                {

                    RecieveFileBuffers[i] = RecieveFileByteUnit[FileContentIndex + i];

                }

                if (NextSaveIndex == RecieveFileLenght)//需一次传输完成;
                {
                    //===回显基本信息===================================================
                    MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileByteUnit, 2, (int)SelfTagMessageLenght);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]一次传输完成接收图像流！", this.MyLockIDStr));
                    StartImageStreamSave();

                }
                else //需多次传输完成
                {
                     //===回显基本信息===================================================
                    string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker+string.Format("[{0}]开始接收图像流分包[{1}][{2}]", this.MyLockIDStr, LoopReadCount,NextSaveIndex));
                    //MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileByteUnit, 2, 65537);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                    MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileByteUnit, 2, (int)SelfTagMessageLenght);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                   
                }
            
            }
            else   //第2...n次传输
            {
                //--应答确认-----------------------------------------------------------------------------------------------------------------
                //MyAsynchLockServerSocketService.CloudServerAckToLock(MyReadWriteChannel); 
                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]连续接收图像流分包[{1}][{2}]", this.MyLockIDStr, LoopReadCount, MyRecieveCount));
              
                this.AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);
            }
         

        }

        public void StartLongFileReceiveEx(byte[] RecieveFileByteUnit, uint MyRecieveCount)
        {
            //---兼容物理锁-----------------------------------------------------------------------------------------------------------------------
            
            if (LoopReadCount == 1)  //第一次传输
            {
                PackgeIndex = 1;
                //--0.应答确认-----------------------------------------------------------------------------------------------------------------
                string RecieveMsgIndexStr = string.Format("{0:X2}", RecieveFileByteUnit[12]);//取低位！
                //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", MyReadBuffers[12]);//高位省略
                int FirstPackgeIndex = Convert.ToInt16(RecieveMsgIndexStr, 16);
                MyAsynchLockServerSocketService.CloudServerAckToLockEx(MyReadWriteChannel, PackgeIndex);                
                //MyAsynchLockServerSocketService.CloudServerAckToLock(MyReadWriteChannel);  
                if (PackgeIndex != FirstPackgeIndex)
                {
                    LoopReadCount--;
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]重复发送图像流分包[{1}]错误(1)！", this.MyLockIDStr, FirstPackgeIndex));
                    return;
                }
                PackgeIndex++;
                string RecieveMsgCountStr = string.Format("{0:X2}", RecieveFileByteUnit[11]);
                RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", RecieveFileByteUnit[10]);
                PhysicalLockMsgCount = Convert.ToInt16(RecieveMsgCountStr, 16);

              
                //--1.--------------------------------------------------------------------------------------------------------------------------
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
                    //FileContentIndex = 22;
                    FileContentIndex = 38;//加上另外16个字节;
                    SnapTypeID = 0;

                }
                if (HexMessageTypeIDStr == "3009")
                {

                    FileContentIndex = 40;//加上另外18个字节;
                    SnapTypeID = 1;

                }
                if (HexMessageTypeIDStr == "200C")
                {

                    Byte RecieveMessageResult = RecieveFileByteUnit[22];//结果标志
                    if (RecieveMessageResult != 0)
                    {
                        FailResponseToMobile();
                        return;

                    }
                    else
                    {
                        FileContentIndex = 39;//加上另外17个字节;
                        SnapTypeID = 2;
                    }

                }
             
                //string RecieveMsgCountStr = string.Format("{0:X2}", RecieveFileByteUnit[11]);
                //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", RecieveFileByteUnit[10]);
                //PhysicalLockMsgCount = Convert.ToInt16(RecieveMsgCountStr, 16);


                 //string RecieveMsgNunberStr = string.Format("{0:X2}", RecieveFileByteUnit[13]);
                //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", RecieveFileByteUnit[12]);
                //int PhysicalLockMsgNunber = Convert.ToInt16(RecieveMsgCountStr, 16);

                //string RecieveFileLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[1]);
                //RecieveFileLenghtStr = RecieveFileLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[0]);
                //SelfTagMessageLenght = Convert.ToUInt32(RecieveFileLenghtStr, 16);
                //RecieveFileLenght = SelfTagMessageLenght - FileContentIndex;              

                //RecieveFileLenght = SelfTagMessageLenght - FileContentIndex;
                //RecieveFileLenght = SelfTagMessageLenght;
                
                RecieveFileBuffersEx = new byte[65536];//申请存贮空间
                NextSaveIndex = MyRecieveCount - FileContentIndex-1;//第一次接收的文件字节数！去掉效验字节


                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]开始接收共[{1}]包图像流分包[{2}][{3}]", this.MyLockIDStr, PhysicalLockMsgCount, FirstPackgeIndex, NextSaveIndex));

                //--填充所接收的文件字节-----------------------------------------------------
                for (int i = 0; i < NextSaveIndex; i++)
                {

                    RecieveFileBuffersEx[i] = RecieveFileByteUnit[FileContentIndex + i];

                }

                if (PhysicalLockMsgCount == (int)LoopReadCount)//一次传输完成;
                {
                    
                    StartImageStreamSaveEx();

                }
             
                //else //多次传输完成
                //{
               //===回显基本信息===================================================
                //MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileByteUnit, 2, 65537);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                //MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileByteUnit, 2, (int)SelfTagMessageLenght);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                //}

            }
            else   //第2...n次传输
            {
                //--应答确认-----------------------------------------------------------------------------------------------------------------
                 string NextRecieveMsgIndexStr = string.Format("{0:X2}", RecieveFileByteUnit[12]);//取低位！
                //RecieveMsgCountStr = RecieveMsgCountStr + string.Format("{0:X2}", MyReadBuffers[12]);//高位省略
                int NextPackgeIndex = Convert.ToInt16(NextRecieveMsgIndexStr, 16);
                if (PackgeIndex == NextPackgeIndex)
                {
                    string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, MyTimeMarker + string.Format("[{0}]连续接收共[{1}]包图像流分包[{2}][{3}]", this.MyLockIDStr, PhysicalLockMsgCount, NextPackgeIndex, MyRecieveCount-23));
                    //MyAsynchLockServerSocketService.CloudServerAckToLock(MyReadWriteChannel);
                    MyAsynchLockServerSocketService.CloudServerAckToLockEx(MyReadWriteChannel, PackgeIndex);
                    this.AddLongFilePackageEx(RecieveFileByteUnit, MyRecieveCount);
                    PackgeIndex++;
                   

                }
                else//
                {
                    LoopReadCount--;
                    MyAsynchLockServerSocketService.CloudServerAckToLockEx(MyReadWriteChannel, NextPackgeIndex);
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]重复发送图像流分包[{1}]错误(2)！", this.MyLockIDStr, NextPackgeIndex));   

                }

            }






        }

        private void AddLongFilePackage(byte[] RecieveFileByteUnit, uint MyRecieveCount)
        {
            
            for (uint i = 0; i < MyRecieveCount; i++)
            {
                RecieveFileBuffers[NextSaveIndex + i] = RecieveFileByteUnit[i];

            }
            NextSaveIndex = NextSaveIndex + (uint)MyRecieveCount;

            //if (RecieveMessageLenght == SelfTagMessageLenght)
            if (NextSaveIndex == RecieveFileLenght)
            {
                StartImageStreamSave();
                //StartLongFileSave();
            }

            

        }

        private void AddLongFilePackageEx(byte[] RecieveFileByteUnit, uint MyRecieveCount)
        {
            //---兼容物理锁-----------------------------------------------------------------------------------------------------------------------
            FileContentIndex = 22; //消息头[从第二个包开始]   
            uint ImageBufferLenght = MyRecieveCount - FileContentIndex - 1;
            for (uint i = 0; i < ImageBufferLenght; i++)
            {
                RecieveFileBuffersEx[NextSaveIndex + i] = RecieveFileByteUnit[FileContentIndex + i];

            }
            NextSaveIndex = NextSaveIndex + ImageBufferLenght;

            if (PhysicalLockMsgCount == (int)LoopReadCount)
            {
                StartImageStreamSaveEx();
                //StartLongFileSave();
            }



        }

        private void StartImageStreamSave()
        {
           
            MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();
          
             
            //SaveToFile();
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
                if(this.MyLockIDStr=="88886666ABCD111")SendEMailNotifyEx();
            }
            
            ResponseToMobile();
            ReplyLoginMessageToLock();

            
            FinallyDisposed();

        }

        private void StartImageStreamSaveEx()
        {
            RecieveFileBuffers = new byte[NextSaveIndex];
            RecieveFileLenght = (uint)NextSaveIndex;
            //--填充所接收的文件字节-----------------------------------------------------
            for (int i = 0; i < NextSaveIndex; i++)
            {

                RecieveFileBuffers[i] = RecieveFileBuffersEx[i];

            }
            //最后一次次传输完成
            //===回显基本信息===================================================
            MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, RecieveFileBuffers, 2, (int)(int)NextSaveIndex);//扩展大文件传输：智能锁通迅标志：2，传输字节数>65536 ：65537 
                       
            MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();

           
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
                if (this.MyLockIDStr == "88886666ABCD111") SendEMailNotifyEx();
            }
          

            ResponseToMobile();
            ReplyLoginMessageToLock();


            FinallyDisposed();

        }

        private int SaveToFile()
        {
            try
            {
                //========================
                //MyAutoFileNameStr = Guid.NewGuid().ToString().ToUpper();
                FileStream MyRecieveFileStream;
                BinaryWriter MyBinaryWriter;
                MyCompleteFileName = "F:\\LockSnapImage\\" + MyLockIDStr + "\\" + MyAutoFileNameStr + ".jpg";
                MyRecieveFileStream = new FileStream(MyCompleteFileName, FileMode.Create, FileAccess.Write);
                MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);

                MyBinaryWriter.Write(RecieveFileBuffers);

                MyBinaryWriter.Close();
                MyRecieveFileStream.Close();

                MyRecieveFileStream = null;
                MyBinaryWriter = null;
                //------------------------------------------------
                //SaveToALiYunOSSOTS();
                //SendEMailNotify();

                 //int nSnapID;
               // if (HexMessageTypeIDStr == "3007") { nSnapID = 0; } else { nSnapID = 1; }  //如果200C是被动上传不会响应云锁
                //ResponseToMobile(nSnapID);//发送消息响应到移动端


                //if (HexMessageTypeIDStr == "3007") ReplyLoginMessageToLock(0);//如果主动上传发送消息响应到云锁，否则不回应   

                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到Files成功！");


                //FinallyDisposed();



            }
            catch (Exception ExceptionInfor)
            {
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存数据到Files失败！");
            }




            return 0;
        }

        private void SaveImageToDB()
        {

            string LockID = MyLockIDStr;
            int SnapTypeID;
            string SnapUUID = MyAutoFileNameStr;
            SnapID=0;
            SnapTypeID = 0;
          
            //MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);
            
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
                        parm.Value = RecieveFileBuffers;// MyMemoryStreamContent.GetBuffer();
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
            //int SnapTypeID = 0;
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

            //MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);
                
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
               parm.Value = RecieveFileBuffers;// MyMemoryStreamContent.GetBuffer();
               MyUpdateImagelCommand.Parameters.Add(parm);

               MyUpdateImagelCommand.Connection = MySqlConnection;
               MyUpdateImagelCommand.ExecuteNonQuery();
               */
                MySqlConnection.Close();
                   //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库成功");
                //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到数据库成功");
                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库成功", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
               

            }
            else
            {
                MySqlConnection.Close();
                   //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "保存图像数据到数据库失败");
                //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到数据库失败");
                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存数据到数据库失败", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
            }


        }

        private void SaveSnapParaToDB()
        {
            
              //string LockID = MyLockIDStr;
            //string SnapUUID = MyAutoFileNameStr;
         
            SnapID = 0;

            SnapImage MySnapImage = new SnapImage();
            MySnapImage.LockID = this.MyLockIDStr;
            MySnapImage.SnapTypeID = this.SnapTypeID;
            MySnapImage.SnapUUID = MyAutoFileNameStr;

            SnapManager MySnapManager = new SnapManager();
            int ReturnValue=MySnapManager.InsertSnap(MySnapImage, ref SnapID);
            if (ReturnValue == 0)
            {
                  
                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存参数到数据库成功", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


            }
            else
            {
                  
                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存参数到数据库失败", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
            }


        }

        private void SaveSnapImageToDB()
        {
            SnapManager MySnapManager = new SnapManager();
            int ReturnValue = MySnapManager.UpdateSnapImage(this.MyLockIDStr, RecieveFileBuffers, this.SnapID);
            if (ReturnValue == 1)//1:默认为正确；
            {

                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到数据库成功", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));


            }
            else
            {

                string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到数据库失败", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
            }

        }

        private bool SaveSnapImageToALiYunOSS()
        {
             //string MyFileNameStr = Guid.NewGuid().ToString().ToUpper();
            //MyAutoFileNameStr;//
            //MyCompleteFileName = MyLockIDStr + "/" + MyFileNameStr + ".jpg";

            string MySaveToOSSFileName = MyLockIDStr + "/" + MyAutoFileNameStr + ".jpg";
            MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;     
             MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);


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

        private bool SaveSnapImageToALiYunOSSEx()
        {
            ALiYunOSSOTSAPI MyALiYunOSSOTSAPI;
            MyALiYunOSSOTSAPI = new ALiYunOSSOTSAPI();

            string MySaveToOSSFileName = MyLockIDStr + "/" + MyAutoFileNameStr + ".jpg";
            MyCompleteFileName = "http://" + "hnlylgj.oss-cn-hangzhou.aliyuncs.com/" + MySaveToOSSFileName;
            MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);

            bool SaveFlag=false;
            string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
            try
            {

                MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.PrefixStr = this.MyLockIDStr + "/";
                MyALiYunOSSOTSAPI.MyALiYunOSSOTSLogin.FileKeyName = MyAutoFileNameStr + ".jpg";

                MyALiYunOSSOTSAPI.PrivateUploadFile3(ref MyMemoryStreamContent);
               
                 //MyMemoryStreamContent.Close();
                //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                
              
                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到OSS:[{2}][{3}]", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, RecieveFileLenght, MyALiYunOSSOTSAPI.ResultMessageStr));

                SaveFlag = true;
               

            }
            catch (Exception AnyException)
            {
                //MyMemoryStreamContent.Close();
                //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展保存数据到OSS:" + MyALiYunOSSOTSAPI.ResultMessageStr);
                //string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}]扩展保存数据到OSS:[{1}]", MyLockIDStr, MyALiYunOSSOTSAPI.ResultMessageStr));

                this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "锁端[{0}][{1}]扩展保存图像到OSS:[{2}][{3}]", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, RecieveFileLenght, MyALiYunOSSOTSAPI.ResultMessageStr));

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
           string SendFileNameStr = MyAutoFileNameStr + ".jpg";
           MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);

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
            if (MyLockIDStr == "88886666ABCD110")//Test-秦皇汉武
            {

                  //string SendFileNameStr = MyAutoFileNameStr + ".jpg";
                // MemoryStream MyMemoryStreamContent = new MemoryStream(RecieveFileBuffers);
                //LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc("hnlylgjx@qq.com", MyMemoryStreamContent, SendFileNameStr);
                // MyMemoryStreamContent.Close();

                //LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc("hnlylgjx@qq.com", SnapID.ToString(), MyLockIDStr);
                LGJAsynchSocketService.SendMailProc MySendMailProc = new SendMailProc(SnapID.ToString(), MyLockIDStr);
                MySendMailProc.StartSendMail();

                if (MySendMailProc.SendFlagID)
                {
                    string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                    this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]扩展发送邮件通知成功！", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));

                    //this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展发送邮件通知成功！");
                }
                else
                {
                    string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();
                    this.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]扩展发送邮件通知失败！", MyLockIDStr, MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint));
                    this.MyAsynchLockServerSocketService.DisplayResultInfor(1, "扩展发送邮件通知失败！");
                }
                return MySendMailProc.SendFlagID;
            }
            return false;

        }

        private void FinallyDisposed()
        {
            this.LoopReadCount = 0;
            this.RecieveMessageLenght = 0;
            this.SelfTagMessageLenght = 0;
            this.RecieveFileLenght = 0;
            this.NextSaveIndex = 0;        
            //this.MyLockIDStr = null;
            this.MyCompleteFileName = null;
            //this.MyMobileIDStr = null; 
            this.RecieveFileBuffers = null;
            this.RecieveFileBuffersEx = null;
            this.PhysicalLockMsgCount = 0;
            this.PackgeIndex = 0;

        }

        private void ReplyLoginMessageToLock()
        {
            //如果主动上传发送消息响应到云锁，否则不回应  
            //if (HexMessageTypeIDStr != "3007") return;
            if (SnapTypeID == 2)
            {
                return;
            }
             //------校验字节------------------------------------------------
            int MySendByteCount = 24;//加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;

            if (MyAsynchLockServerSocketService.DataFormateFlag)
            {
                
                HexSendMessageIDString = SnapTypeID == 0? "0830":"0A30";
            }
            else
            {
                HexSendMessageIDString = SnapTypeID == 0 ? "3008" : "300A";
            }

             
            //string MyDateTimeString = GetDateTimeWeekIndex();
            //byte[] MyDateTimeBytes = Encoding.ASCII.GetBytes(MyDateTimeString);

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
       //---------------------------------------
        private void ResponseToMobile()
        {
            /*
            int nSnapID;
            if (HexMessageTypeIDStr == "3007") 
            {
                nSnapID = 0; 
            }
            if (HexMessageTypeIDStr == "3009")
            {
                nSnapID = 1;
            }
            else
            {
                nSnapID = 2; 
            }  
            */
            //如果200C是被动上传不会响应云锁
            string CommandMessageID = "remotesnap"; 
            if ( SnapTypeID ==0) 
            {
               CommandMessageID=  "mailsnap";
            }
            if (SnapTypeID == 1)
            {
                 CommandMessageID=  "exceptsnap";
            }
            if (SnapTypeID == 2)
            {
                CommandMessageID = "remotesnap";
            }
          
              //---1.找智能锁本身通道的路由表记录-------------------------------------
            //LockServerLib.FindLockChannel MyBindedLockChannel = new LockServerLib.FindLockChannel(this.MyReadWriteChannel);
            // LoginUser MyLoginUser = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
            //string MessageLockStr = MyLoginUser.LockID;
            //string MessageMobileStr = MyLoginUser.MobileID; // null;

            /*
            //-----查找移动端[数据库]-----------------------------------
            if (MessageLockStr == "89765432BCDA823")//测试例子而已
            {
                MessageMobileStr = "9876DDDD8989101";
            }
            else
            {
                return;
            }
             * */

             ////if (string.IsNullOrEmpty(MyCompleteFileName)) MyCompleteFileName = "SMTP://"+MyAutoFileNameStr + ".jpg"; ;  
           
            string ResultStr;
            string SnapIDStr = SnapID.ToString();



            if (SnapTypeID <2)//抓拍
            {

                ResultStr = "[" + SnapIDStr+","+MyCompleteFileName + "]"; 
            }
            else//远拍
            {
                ResultStr = "true";
                ResultStr = ResultStr + "[" + SnapIDStr + "," + MyCompleteFileName + "]";
            }

            string CommandMessageStr;
            CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
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
            MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);

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

        private void FailResponseToMobile()
        {
            string CommandMessageStr = "remotesnap";
            string SnapIDStr = SnapID.ToString();
            string ResultStr;
            ResultStr = "false";
            ResultStr = ResultStr + "[10]"; 
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
            //FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MyMobileIDStr);
            //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;

            //--再转发出去-----------
            //MyAsynchSocketServiceBaseFrame.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

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

        private string GetDateTimeWeekIndex()
        {
            //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
            //string Teststr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
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
    }
}
