using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LGJAsynchSocketService.LGJAynchWork;
namespace LGJAsynchSocketService.AsynchSQLServerIO
{

    /*
       实现了智能锁端两个重点功能异步数据库操作：1.登录验证，2.抓拍图像保存数据库   

        --李庚君--
        2015-07-31
    */
    public class LGJAsynchAccessDBase
    {
       public delegate void AsynchNotifyHandler(string MessageStr, int MessageID);
       public AsynchNotifyHandler ResultAsynchNotifyHandler;       
        
       public LGJAsynchAccessDBase()
       {
           
       }
       public void AsynchTest()
      {

         //开发测试
         string MyCommandText = "WaitFor Delay '00:00:1' Select @@Version";
         RequestAttachment MyRequestAttachment = new RequestAttachment(MyCommandText,null,null);

         AsyncCallback MyAsyncProcessResult = new AsyncCallback(ProcessResult);
         MyRequestAttachment.MySqlCommand.BeginExecuteReader(MyAsyncProcessResult, MyRequestAttachment);


      }
       public void AsynchLoginAuth(string LockID, SocketServiceReadWriteChannel MeReadWriteChannel, AsynchLockServerSocketService MeAsynchLockServerSocketService)
      {
         
          //第一个版本
          RequestAttachment MyRequestAttachment = new RequestAttachment(LockID, MeReadWriteChannel, MeAsynchLockServerSocketService);
         
         // string MyCommandText = "select MobileID from Channel where LockID='" + LockID + "'";
         // MyRequestAttachment.MySqlCommand.CommandText = MyCommandText;

         MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
         MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
         MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
         MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnMobileID", SqlDbType.NChar, 15));
         MyRequestAttachment.MySqlCommand.Parameters["@ReturnMobileID"].Direction = ParameterDirection.Output;

         //MyRequestAttachment.MySqlCommand.BeginExecuteReader(new AsyncCallback(ProcessResult), MyRequestAttachment);//查询语句
         MyRequestAttachment.MySqlCommand.BeginExecuteNonQuery(new AsyncCallback(ProcessResult3), MyRequestAttachment); //存储过程！

      }

       public void AsynchLoginAuthEx(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
       {
            //增强版本
           string LockID = Encoding.ASCII.GetString(MeLoginUser.MyByteBuffer, 22, 15);
           RequestAttachment MyRequestAttachment = new RequestAttachment(MeLoginUser, MeAsynchLockServerSocketService, "AuthLockID");

           // string MyCommandText = "select MobileID from Channel where LockID='" + LockID + "'";
           // MyRequestAttachment.MySqlCommand.CommandText = MyCommandText;

           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@LockID", LockID));
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
           MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnMobileID", SqlDbType.NChar, 15));
           MyRequestAttachment.MySqlCommand.Parameters["@ReturnMobileID"].Direction = ParameterDirection.Output;

            //MyRequestAttachment.MySqlCommand.BeginExecuteReader(new AsyncCallback(ProcessResult), MyRequestAttachment);//查询语句

            AsyncCallback MyLoginAuthResult = new AsyncCallback(LoginAuthResult);
            MyRequestAttachment.MySqlCommand.BeginExecuteNonQuery(MyLoginAuthResult, MyRequestAttachment); //存储过程！

       }
       
       public void AsynchSaveSnapPara(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
       {
           string SnapUUID = MeLoginUser.TempString;
           RequestAttachment MyRequestAttachment = new RequestAttachment(MeLoginUser, MeAsynchLockServerSocketService, "AddSnapImage");

           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@LockID", MeLoginUser.LockID));
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@SnapTypeID", MeLoginUser.SnapTypeID));
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@SnapUUID", SnapUUID));

           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int));
           MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@ReturnSnapID", SqlDbType.BigInt));
           MyRequestAttachment.MySqlCommand.Parameters["@ReturnSnapID"].Direction = ParameterDirection.Output;

            AsyncCallback MySaveSnapParaResult = new AsyncCallback(SaveSnapParaResult);
           MyRequestAttachment.MySqlCommand.BeginExecuteNonQuery(MySaveSnapParaResult, MyRequestAttachment); //存储过程！

           // MyRequestAttachment.MySqlCommand.EndExecuteNonQuery();

       }

       private void AsynchSaveSnapImage(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
       {
           string LockID = MeLoginUser.LockID;
           long SnapID = MeLoginUser.SnapID;
           byte[] SnapImageByte;

           if (MeLoginUser.FileReadMode == 2)//B模式下:多一步
           {
               SnapImageByte = new byte[MeLoginUser.WorkStatus];
               for (uint i = 0; i < MeLoginUser.WorkStatus; i++)
               {
                   SnapImageByte[i] = MeLoginUser.MyByteBuffer[i];//B模式下：this.MyLoginUser.WorkStatus不一定等于this.MyLoginUser.MyByteBuffer的Lenght

               }
           }
           else
           {
               SnapImageByte = MeLoginUser.MyByteBuffer;//A模式下：相等
           }
             

           RequestAttachment MyRequestAttachment = new RequestAttachment(MeLoginUser, MeAsynchLockServerSocketService, 0);//采用查询语句构造
            //-----1.视图版--------------
            string MyCommandText = "Update snap" + LockID + " Set Image = @Image " + "WHERE SnapID = @SnapID";
            //-----2.表格版---------------
            //string MyCommandText = "Update snap" + " Set Image = @Image " + "WHERE SnapID = @SnapID and LockID='"+ LockID+"'";

            MyRequestAttachment.MySqlCommand.CommandText = MyCommandText;

           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@SnapID", SnapID));
           MyRequestAttachment.MySqlCommand.Parameters.Add(new SqlParameter("@Image", SnapImageByte));

            AsyncCallback MyAsyncSaveSnapImageResult = new AsyncCallback(SaveSnapImageResult);

           MyRequestAttachment.MySqlCommand.BeginExecuteNonQuery(MyAsyncSaveSnapImageResult, MyRequestAttachment);

          
       }
    
       private void ProcessResult(IAsyncResult InMyAsyncResult)
       {
           //查询语句版本
          RequestAttachment MyRequestAttachment = (RequestAttachment)InMyAsyncResult.AsyncState;
          string MobileID = null;
          int ReturnValue = 1;

          using (MyRequestAttachment.MySqlCommand.Connection)//保证关闭连接
           {
               using (MyRequestAttachment.MySqlCommand)
               {                   
                   
                   SqlDataReader MySqlDataReader = MyRequestAttachment.MySqlCommand.EndExecuteReader(InMyAsyncResult);
                   if (MySqlDataReader.Read())
                   {
                     
                       MobileID = (string)MySqlDataReader["MobileID"];
                      

                   }
               }
           }

           //----------------------------------------------------------------------------------------------------------------
              if (ResultAsynchNotifyHandler != null) ResultAsynchNotifyHandler(MobileID, ReturnValue);
              if (MobileID != null)
              {
                 
                  //----回显信息----------------------------------------------------------------------------------------
                  string MyRecieveDateTimeString = Encoding.ASCII.GetString(MyRequestAttachment.MyReadWriteChannel.MyReadBuffers, 39, 15);
                  string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                  //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyLockIDStr + "--" + MyRecieveDateTimeString));

                  MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyRequestAttachment.MyLoginUser.SocketInfor, MyRequestAttachment.MyLoginUser.LockID));
                  //--注册通道--------------------------------------------------------------------------------------------
                  MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForLoginEx(ref MyRequestAttachment.MyReadWriteChannel.MyTCPClient, MyRequestAttachment.MyLoginUser.LockID, MobileID); //更新路由注册表

                  //发注册消息到移动端
                  OnLineResponseToMobile(MyRequestAttachment.MyLoginUser.LockID, MobileID, MyRequestAttachment);
                  Byte TimeCompareID = TimeCompare(MyRecieveDateTimeString);//时间比较
                  //发送成功响应消息到云锁
                  ReplyLoginMessageToLock(0, TimeCompareID, MyRequestAttachment);
              
              }
            else

           {

               //--取消通道---------------------------------------------------
               ReplyLoginMessageToLock(0xFF, 1, MyRequestAttachment); //发送失败响应消息到云锁

           }




       }

       private void ProcessResult3(IAsyncResult InMyAsyncResult)
       {
           //--存储过程+消息队列版本[有问题test]-----------------------------------------------------------------------------------------------
           string MobileID = null;
           int ReturnValue = 1;
           //-----1.---------------------------------------------------------------------------------------------------------------
           RequestAttachment MyRequestAttachment = (RequestAttachment)InMyAsyncResult.AsyncState;
           using (MyRequestAttachment.MySqlCommand.Connection)//保证关闭连接
           {
               using (MyRequestAttachment.MySqlCommand)
               {

                   MyRequestAttachment.MySqlCommand.EndExecuteNonQuery(InMyAsyncResult);
                   ReturnValue = (int)MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Value;
                   if (ReturnValue == 0) MobileID = (string)MyRequestAttachment.MySqlCommand.Parameters["@ReturnMobileID"].Value;
                   if (ResultAsynchNotifyHandler != null) ResultAsynchNotifyHandler(MobileID, ReturnValue);

               }
           }
           if (ResultAsynchNotifyHandler != null) ResultAsynchNotifyHandler(MobileID, ReturnValue);

           /*
           //----2.------------------------------------------------------------------------------------------------------------
           if (MobileID != null)
           {
               string MyAttachmentInfor = MyRequestAttachment.LockID + "," + MobileID;
               MyRequestAttachment.MyAsynchLockServerSocketService.MyMessageEntityManager.AddMessageEntity(new MessageQueue.LGJMessageEntity(("login", 0, MyRequestAttachment.MyReadWriteChannel, MyAttachmentInfor));
               

           }
           else
           {

             MyRequestAttachment.MyAsynchLockServerSocketService.MyMessageEntityManager.AddMessageEntity(new MessageQueue.LGJMessageEntity("login", 1, MyRequestAttachment.MyReadWriteChannel));

           }
           */

       }

        private void LoginAuthResult(IAsyncResult InMyAsyncResult)
       {
           //登录验证：存储过程版本
           string MobileID = null;
           int ReturnValue = 1;
           //-----1.---------------------------------------------------------------------------------------------------------------
           RequestAttachment MyRequestAttachment = (RequestAttachment)InMyAsyncResult.AsyncState;
           using (MyRequestAttachment.MySqlCommand.Connection)//保证关闭连接
           {
                using (MyRequestAttachment.MySqlCommand)
                {

                    MyRequestAttachment.MySqlCommand.EndExecuteNonQuery(InMyAsyncResult);
                    ReturnValue = (int)MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Value;
                    if (ReturnValue == 0) MobileID = (string)MyRequestAttachment.MySqlCommand.Parameters["@ReturnMobileID"].Value;

                }
           }
           //if (ResultAsynchNotifyHandler != null) ResultAsynchNotifyHandler(MobileID, ReturnValue);
           //----2.------------------------------------------------------------------------------------------------------------
          
           if (MobileID != null)
           {
                //成功验证
               //----回显信息----------------------------------------------------------------------------------------
               string LockID = Encoding.ASCII.GetString(MyRequestAttachment.MyLoginUser.MyByteBuffer, 22, 15);
               //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
               string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);
                MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyRequestAttachment.MyLoginUser.SocketInfor,LockID));
              
               //--注册通道--------------------------------------------------------------------------------------------
               MyRequestAttachment.MyLoginUser.LockID = LockID;
               MyRequestAttachment.MyLoginUser.MobileID = MobileID;
               MyRequestAttachment.MyLoginUser.ChannelStatus=0;

               MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(4,"");//刷新主界面


               //发智能锁上线消息到移动端
               OnLineResponseToMobile(LockID, MobileID, MyRequestAttachment);

               string MyRecieveDateTimeString = Encoding.ASCII.GetString(MyRequestAttachment.MyLoginUser.MyByteBuffer, 39, 15);               
               Byte TimeCompareID = TimeCompare(MyRecieveDateTimeString);//时间比较

               //发送成功响应消息到智能端
               ReplyLoginMessageToLock(0, TimeCompareID, MyRequestAttachment);

           }
           else
           {
               //失败验证
               //----发送失败响应消息到智能端-------------------------------------
               ReplyLoginMessageToLock(0xFF, 1, MyRequestAttachment); 
               //----取消通道-----------------------------------------------------
               //System.Threading.Thread.Sleep(500);
               //MyRequestAttachment.MyLoginUser.MyReadWriteSocketChannel.MyTCPClient.Client.Close();
               //MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList.Remove(MyRequestAttachment.MyLoginUser);

           }

       }

       private void SaveSnapParaResult(IAsyncResult InMySnapParaResult)
       {
           //存储过程版本
           long SnapID = 0;
           int ReturnValue = 1;
           //-----1.---------------------------------------------------------------------------------------------------------------
           RequestAttachment MyRequestAttachment = (RequestAttachment)InMySnapParaResult.AsyncState;
          
           using (MyRequestAttachment.MySqlCommand.Connection)//保证关闭连接
           {
               using (MyRequestAttachment.MySqlCommand)
               {

                   MyRequestAttachment.MySqlCommand.EndExecuteNonQuery(InMySnapParaResult);
                   ReturnValue = (int)MyRequestAttachment.MySqlCommand.Parameters["@ReturnValue"].Value;
                   if (ReturnValue == 0) SnapID = (long)MyRequestAttachment.MySqlCommand.Parameters["@ReturnSnapID"].Value;
                   

               }
           }
           //----2.------------------------------------------------------------------------------------------------------------

           if (ReturnValue ==0)
           {
               //----回显信息----------------------------------------------------------------------------------------
               //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);// + "[" + DateTime.Now.Ticks.ToString() + "]";

                MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}][{1}][{2}]异步保存参数到数据库成功", MyRequestAttachment.MyLoginUser.LockID, MyRequestAttachment.MyLoginUser.SocketInfor,SnapID));
                MyRequestAttachment.MyLoginUser.SnapID = SnapID;
               //----开始执行下一步保存图像---------------------------------------------------------------------------
               AsynchSaveSnapImage(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService);




           }
           else
           {
                //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";


                MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}][{1}]异步保存参数到数据库失败", MyRequestAttachment.MyLoginUser.LockID, MyRequestAttachment.MyLoginUser.SocketInfor));
          
               //--发送失败响应消息到云锁-------------------------------------
               SnapImageReplyMessageToLock(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService, 0xFF);
             
           }

       }

       private void SaveSnapImageResult(IAsyncResult InMySnapParaResult)
       {
           
           int ReturnValue = -1;//1:默认值为正确；
           //-----1.---------------------------------------------------------------------------------------------------------------
           RequestAttachment MyRequestAttachment = (RequestAttachment)InMySnapParaResult.AsyncState;
           using (MyRequestAttachment.MySqlCommand.Connection)//保证关闭连接
           {
               using (MyRequestAttachment.MySqlCommand)
               {

                  ReturnValue=MyRequestAttachment.MySqlCommand.EndExecuteNonQuery(InMySnapParaResult);
                   

               }
           }
          //----2.------------------------------------------------------------------------------------------------------------

           if (ReturnValue ==1)
           {
                //----回显信息----------------------------------------------------------------------------------------
                //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);
                MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}][{1}]异步保存图像到数据库成功", MyRequestAttachment.MyLoginUser.LockID, MyRequestAttachment.MyLoginUser.SocketInfor));

              //--发送成功响应消息到智能云锁-------------------------------------
               SnapImageReplyMessageToLock(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService, 0x0);

               //--发送成功响应消息移动端--------------------------------
               SnapImageResponseToMobile(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService);

               //--第三步：发邮件--------------------------------------------
               if (MyRequestAttachment.MyLoginUser.LockID == "88886666ABCD110" && MyRequestAttachment.MyAsynchLockServerSocketService.IsEMailPush==true)//Test
               {
                   AsynchWorkProcess MyAsynchWorkProcess = new AsynchWorkProcess();
                   MyAsynchWorkProcess.AsyncStartSendEMail(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService);//异步发送邮件操作
                  
                   //MyRequestAttachment.MyLoginUser.ClearSet();
               
               }
               else
               {
                   MyRequestAttachment.MyLoginUser.ClearSet();

               }

              

           }
           else
           {
               //string MyTimeMarker = string.Format("{0:MM-dd HH:mm:ss}", DateTime.Now) + ":" + DateTime.Now.Millisecond.ToString();// + "[" + DateTime.Now.Ticks.ToString() + "]";
                string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now);// + "[" + DateTime.Now.Ticks.ToString() + "]";

                MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "[{0}][{1}]异步保存图像到数据库失败", MyRequestAttachment.MyLoginUser.LockID, MyRequestAttachment.MyLoginUser.SocketInfor));
            
               //--发送失败响应消息到云锁-------------------------------------
               SnapImageReplyMessageToLock(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService, 0xFF);
             
           }

       }
       
       private void SnapImageReplyMessageToLock(LoginUser MeLoginUser, AsynchLockServerSocketService MyAsynchLockServerSocketService, byte ResultID)
       {
           //如果主动上传发送消息响应到云锁，否则不回应  
           //if (HexMessageTypeIDStr != "3007") return;
           int SnapTypeID = MeLoginUser.SnapTypeID;
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

           MySendMessageBytes[22] = ResultID;// 0;成功;//-1：FF失败

           MyAsynchLockServerSocketService.StartAsynchSendMessage(MeLoginUser.MyReadWriteSocketChannel, MySendMessageBytes);

       }

        private void SnapImageResponseToMobile(LoginUser MeLoginUser, AsynchLockServerSocketService MyAsynchLockServerSocketService)
        {

            //如果200C是被动上传不会响应云锁
            int SnapTypeID = MeLoginUser.SnapTypeID;

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
            string SnapIDStr = MeLoginUser.SnapID.ToString();
            string UUIDStr = MeLoginUser.TempString;// "";

             int ScaleSize=1;//缩放倍数
            if(MeLoginUser.WorkCountSum>200000)
            {
                ScaleSize =( (int)MeLoginUser.WorkCountSum / 200000);
                ScaleSize++;

                //if (ScaleSize > 5) ScaleSize = ScaleSize * 2;

            }

            string ScaleSizeFlag = ScaleSize.ToString();// "0";
             

            if (SnapTypeID < 2)//抓拍
            {

                ResultStr = "[" + SnapIDStr + ", " + UUIDStr + "," + ScaleSizeFlag + "]";
            }
            else//远拍
            {
                ResultStr = "true";
                ResultStr = ResultStr + "[" + SnapIDStr + "," + UUIDStr + "," + ScaleSizeFlag + "]";
            }

            string CommandMessageStr;
            CommandMessageStr = CommandMessageID + "#" + MeLoginUser.MobileID + "-" + MeLoginUser.LockID + "#" + ResultStr + "!";
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

            //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------

            try
            {

                SocketServiceReadWriteChannel NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                /*
                SocketServiceReadWriteChannel NewReadWriteChannel;
                if (MeLoginUser.ReplyChannelLoginID<1)
                {
                   //进行广播！
                    NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                    MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                    NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
                    MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                }
                else
                {

                    int ReplyChannelIndex = MeLoginUser.ReplyChannelLoginID - 1;
                    if (ReplyChannelIndex < 2)
                    {
                        NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[ReplyChannelIndex].MyReadWriteSocketChannel;
                        MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                    }
                    else
                    {
                        //通道错误；
                        MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应再转发移动端寻找通道错误：" + ReplyChannelIndex.ToString());
                    }
                }
                */
              

            }
            catch
            {
                //通道错误；
                MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
            }



        }
       
       private void ReplyLoginMessageToLock(Byte ProcessID, Byte TimeSynchID, RequestAttachment MyRequestAttachment)
         {
            
             int MySendByteCount = 40;//加校验字节
            
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyRequestAttachment.MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0210";
             }
             else
             {
                HexSendMessageIDString = "1002";
             }
           

             string MyDateTimeString = GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.ASCII.GetBytes(MyDateTimeString);

             //填充字节信息头
             if (MyRequestAttachment.MyAsynchLockServerSocketService.DataFormateFlag)
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



             if (MyRequestAttachment.MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[10] = 1;
                 MySendMessageBytes[12] = 1;
                 
             }
             else
             {
                 MySendMessageBytes[11] = 1;
                 MySendMessageBytes[13] = 1;
             }

            
         
             MySendMessageBytes[22] = ProcessID;// 0;//-1：0xFF失败
             MySendMessageBytes[23] = TimeSynchID;//1;

             //填充日期
           /*
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[24 + i] = MyDateTimeBytes[i];

             }
           */
            
           //MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessage(MyRequestAttachment.MyReadWriteChannel, MySendMessageBytes);

           MyRequestAttachment.MyLoginUser.IsCloseSocket = true;//设置关闭标志
           MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessageEx(MyRequestAttachment.MyLoginUser, MySendMessageBytes); 
           
       
       
       
       }
         
       private Byte TimeCompare(string  DateTimeStr)
         {

              return 1;
            

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
       
       private void OnLineResponseToMobile(string LockID,string MobileID, RequestAttachment MyRequestAttachment)
         {
             string CommandMessageStr = "locklogin";
              
             CommandMessageStr = CommandMessageStr + "#" + MobileID + "-" + LockID + "#[" +"openlock"+","+GetDateTimeWeekIndex()+"]!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 250;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

               //--找移动端通道，如果是原型-2和正式版本需固定1/2个移动端通道【请求和响应】[1通道和2号通道]--------------------------------------------------------------------------------
             try
             {            
                 
                SocketServiceReadWriteChannel NewReadWriteChannel = MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
              
                /*
                 SocketServiceReadWriteChannel NewReadWriteChannel;
                 if (MyRequestAttachment.MyLoginUser.ReplyChannelLoginID < 1)
                 {
                     //进行广播！
                     NewReadWriteChannel = MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                     MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                    //NewReadWriteChannel = MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
                     //MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                 }
                 else
                 {

                     int ReplyChannelIndex = MyRequestAttachment.MyLoginUser.ReplyChannelLoginID - 1;
                     if (ReplyChannelIndex < 1)//2
                     {
                         NewReadWriteChannel = MyRequestAttachment.MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[ReplyChannelIndex].MyReadWriteSocketChannel;
                         MyRequestAttachment.MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                     }
                     else
                     {
                         //通道错误；
                         MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应[Login]再转发移动端寻找通道错误：" + ReplyChannelIndex.ToString());
                     }
                 }
          
               */
             
             }
             catch
             {
                 //无通道记录；
                    MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动服务器端响应通道错误[2]");
             }

            
            


         }
      
    



    }

}
