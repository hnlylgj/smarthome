using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Threading;
namespace LGJAsynchSocketService.MessageQueue
{
    public class ProcessMessageEntity<TMessageEntity, TMessageEntityManager>
        where TMessageEntity : ILGJMessageEntity
        where TMessageEntityManager : IMessageEntityManager<TMessageEntity>
    {


        private TMessageEntityManager MyMessageEntityManager;
        public delegate void AsynchNotifyHandler(string MessageStr);
        public AsynchNotifyHandler ResultAsynchNotifyHandler;

        AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
        //bool IsExit = false;
        public static void Start(TMessageEntityManager MeMessageEntityManager, AsynchSocketServiceBaseFrame MeAsynchSocketServiceBaseFrame)
        {
            //			ProcessDocuments<T, U> pd;
            //			pd = new ProcessDocuments<T, U>(dm);
            //			Thread t1 = new Thread(new ThreadStart(pd.Run));
            //			t1.Start();

            // new Thread(new ThreadStart(new ProcessDocuments<T, U>(dm).Run)).Start();

            new Thread(new ProcessMessageEntity<TMessageEntity, TMessageEntityManager>(MeMessageEntityManager, MeAsynchSocketServiceBaseFrame).Run).Start();
        }

        protected ProcessMessageEntity(TMessageEntityManager MeMessageEntityManager, AsynchSocketServiceBaseFrame MeAsynchSocketServiceBaseFrame)
        {
            MyMessageEntityManager = MeMessageEntityManager;
            MyAsynchSocketServiceBaseFrame = MeAsynchSocketServiceBaseFrame;
            //ResultAsynchNotifyHandler = new AsynchNotifyHandler(MyQueneMainForm.AsynchCallBackHandler2);

        }


        protected void Run()
        {
            //while (true)
            while (MyAsynchSocketServiceBaseFrame.IsExit == false)
            {
                if (MyMessageEntityManager.IsMessageEntityAvailable)
                {
                    TMessageEntity MyMessageEntity = MyMessageEntityManager.GetMessageEntity();
                    FunDispatcher(MyMessageEntity);


                }
                Thread.Sleep(20);

            }
            if (MyMessageEntityManager.IsMessageEntityAvailable)
            {
                TMessageEntity MyMessageEntity = MyMessageEntityManager.GetMessageEntity();
               FunDispatcher(MyMessageEntity);

            }
        }
        private void FunDispatcher(TMessageEntity MyMessageEntity)
        {

            if (MyMessageEntity.Title == "create")
            {

                //MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.CRUDLoginUserList(MyMessageEntity.ReadWriteSocketChannel, 0);
            }

            if (MyMessageEntity.Title == "delete")
            {

                AsynchLockServerSocketService MyAsynchLockServerSocketService = (AsynchLockServerSocketService)MyAsynchSocketServiceBaseFrame;
                MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForDelete(ref MyMessageEntity.ReadWriteSocketChannel.MyTCPClient);
            }
            if (MyMessageEntity.Title == "select")
            {

                AsynchLockServerSocketService MyAsynchLockServerSocketService = (AsynchLockServerSocketService)MyAsynchSocketServiceBaseFrame;

                PreprocessMessage(MyMessageEntity, MyAsynchLockServerSocketService);
            
            }

            
            if (MyMessageEntity.Title == "login")
            {

                AsynchLockServerSocketService MyAsynchLockServerSocketService = (AsynchLockServerSocketService)MyAsynchSocketServiceBaseFrame;
                if (MyMessageEntity.ResultFlag == 0)
                {
                    string LockID = MyMessageEntity.AttachmentInfor.Substring(0, 15);
                    string MobileID = MyMessageEntity.AttachmentInfor.Substring(MyMessageEntity.AttachmentInfor.IndexOf(",") + 1);

                    //----回显信息----------------------------------------------------------------------------------------                  
                    string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                    MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyMessageEntity.ReadWriteSocketChannel.MyTCPClient.Client.RemoteEndPoint, LockID));
                    //--注册通道--------------------------------------------------------------------------------------------
                    MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForLoginEx(ref MyMessageEntity.ReadWriteSocketChannel.MyTCPClient, LockID, MobileID); //更新路由注册表
                    //发智能锁上线消息到移动端
                    OnLineResponseToMobile(MyAsynchLockServerSocketService, LockID, MobileID);
                    string MyRecieveDateTimeString = Encoding.ASCII.GetString(MyMessageEntity.ReadWriteSocketChannel.MyReadBuffers, 39, 15);
                    //时间比较 
                    Byte TimeCompareID = TimeCompare(MyRecieveDateTimeString);
                    //发送成功响应消息到云锁
                    ReplyLoginMessageToLock(0, TimeCompareID, MyAsynchLockServerSocketService, MyMessageEntity.ReadWriteSocketChannel);
                }

                else
                {
                    //--发送失败响应消息到云锁,并且取消通道-------------------------------------------------------------------------------                   
                    ReplyLoginMessageToLock(0xFF, 1, MyAsynchLockServerSocketService, MyMessageEntity.ReadWriteSocketChannel);
                    System.Threading.Thread.Sleep(200);//稍微等待
                    MyAsynchLockServerSocketService.MyManagerLoginLockUser.RemoveNotLoginUser(ref MyMessageEntity.ReadWriteSocketChannel.MyTCPClient);

                }
           



            }
        }

        private void PreprocessMessage(TMessageEntity MyMessageEntity, AsynchLockServerSocketService MyAsynchLockServerSocketService)
        {
           SocketServiceReadWriteChannel MyReadWriteChannel=MyMessageEntity.ReadWriteSocketChannel;
           byte MessageTypeFirstFlag = MyMessageEntity.MyByteBuffer[2];
           byte[] MyReadBuffers = MyMessageEntity.MyByteBuffer;
           int MyRecieveCount=MyMessageEntity.MyRecieveCount;
           if (MessageTypeFirstFlag < 250)
           {
               //锁端
               string HexSendMessageIDString;
               if (MyAsynchLockServerSocketService.DataFormateFlag)
               {
                   HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
               }
               else
               {
                   HexSendMessageIDString = string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
               }
               //-----------------------------------------------------------------

               if (MyAsynchLockServerSocketService.MyManagerLoginLockUser.AuthChannel(ref MyReadWriteChannel) == 1)
               {
                   //无认证
                   if (HexSendMessageIDString=="1001")
                   MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, MyReadBuffers, (int)MessageTypeFirstFlag, MyRecieveCount);//只有【Login-命令】能传输再处理
                   

               }
               else
               {
                   //已认证
                   MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, MyReadBuffers, (int)MessageTypeFirstFlag, MyRecieveCount);//所有命令能传输再处理
                    

               }
           }
           else
           {
               //移动端-------------------------
               MyAsynchLockServerSocketService.CommandDefineDispatch(MyReadWriteChannel, MyReadBuffers, (int)MessageTypeFirstFlag, MyRecieveCount);//所有命令能传输再处理
                    



           }
          


        }
        
         private void OnLineResponseToMobile(AsynchLockServerSocketService MyAsynchLockServerSocketService,string LockID,string MobileID)
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
               
            

             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
             try
             {
                 //LockServerLib.FindMobileChannel MyBindedMobileChannel = new LockServerLib.FindMobileChannel(MyFindMobileIDStr);
                 //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;
                 
                 
                 //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 if (MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].LockID == "***************")
                 {
                     SocketServiceReadWriteChannel NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                 }
                 else
                 {
                     MyAsynchLockServerSocketService.DisplayResultInfor(1, "移动服务器端响应通道标志错误[1]");
                 }
               
              }
             catch
             {
                 //无通道记录；；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动服务器端响应通道错误[2]");
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

         private void ReplyLoginMessageToLock(Byte  ProcessID,Byte TimeSynchID, AsynchLockServerSocketService MyAsynchLockServerSocketService,SocketServiceReadWriteChannel MyReadWriteChannel)
         {
            
             //------校验字节------------------------------------------------
             int MySendByteCount = 40;//加校验字节
            
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
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
         
             MySendMessageBytes[22] = ProcessID;// 0;//-1：FF失败
             MySendMessageBytes[23] = TimeSynchID;//1;

             //填充日期
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[24 + i] = MyDateTimeBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes); 
           
         }
         
         private Byte TimeCompare(string  DateTimeStr)
         {

              return 1;
            

         }
        
       
        }


    }

