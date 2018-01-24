using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SmartBusServiceLib;
using LGJAsynchSocketService.AsynchSQLServerIO;

namespace LGJAsynchSocketService.LockServerLib
{
     public class LoginCommandParser
    {
         //AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         LoginUser MyLoginUser;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         //SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount;
         
         string MyLockIDStr;
         string MyFindMobileIDStr;
         public LoginCommandParser(AsynchLockServerSocketService InputAsynchSocketService, SocketServiceReadWriteChannel InputReadWriteChannel, int InputRecieveCount)
         {
             //this.MyAsynchSocketServiceBaseFrame = InputAsynchSocketServiceBaseFrame;
             this.MyAsynchLockServerSocketService = InputAsynchSocketService;
             this.MyReadWriteChannel = InputReadWriteChannel;
             this.MyRecieveCount = InputRecieveCount; 
         }

         public LoginCommandParser(AsynchLockServerSocketService InputAsynchSocketService, LoginUser MeLoginUser, int InputRecieveCount)
        {
            
            this.MyAsynchLockServerSocketService = InputAsynchSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel =MeLoginUser.MyReadWriteSocketChannel;
            this.MyRecieveCount = InputRecieveCount;
        }

        public void CompleteCommand()
        {

            AsynchCompleteCommand();//异步执行
            //SynchCompleteCommand();//同步执行

        }
         public void AsynchCompleteCommand()
         {
             //MyLockIDStr = Encoding.ASCII.GetString(MyReadWriteChannel.MyReadBuffers, 22, 15);             
             //MyFindMobileIDStr = CheckRegisterAsDBase(MyLockIDStr);

              LGJAsynchAccessDBase MyLGJAsynchAccessDBase = new LGJAsynchAccessDBase();
              this.MyLoginUser.ByteBufferCopy(this.MyRecieveCount);//数据复制
              //MyLGJAsynchAccessDBase.AsynchLoginAuth(MyLockIDStr,this.MyReadWriteChannel, this.MyAsynchLockServerSocketService);//异步操作数据库
              MyLGJAsynchAccessDBase.AsynchLoginAuthEx(this.MyLoginUser ,this.MyAsynchLockServerSocketService);//异步操作数据库
            string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);
            MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr+"[当前服务器核心进程线程工作模式]：异步并行"));
             
              //return;
               
            //============================================================================================
            /*
              bool FindResultID=GetChannel(MyLockIDStr);//同步操作数据库
              if (FindResultID)
             {
                 //----回显信息----------------------------------------------------------------------------------------
                 string MyRecieveDateTimeString = Encoding.ASCII.GetString(MyReadWriteChannel.MyReadBuffers, 39, 15); 
                 string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
    //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyLockIDStr + "--" + MyRecieveDateTimeString));
                 
                  MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyLockIDStr));
                 //--注册通道--------------------------------------------------------------------------------------------
                  MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForLoginEx(ref MyReadWriteChannel.MyTCPClient, MyLockIDStr, MyFindMobileIDStr); //更新路由注册表
                   
     //MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteChannel.MyTCPClient, MyLockIDName, 0); //更新路由注册表
                 //MyAsynchLockServerSocketService.MyManagerSocketLoginUser.CRUDLoginUserListForLogin(ref MyReadWriteChannel.MyTCPClient, MyLockIDStr, MyFindMobileIDStr); //更新路由注册表
                                
                 //发注册消息到移动端
                 OnLineResponseToMobile();
                 Byte TimeCompareID = TimeCompare(MyRecieveDateTimeString);//时间比较
                 ReplyLoginMessageToLock(0, TimeCompareID);//发送成功响应消息到云锁
             }
             else
             {
                 //--取消通道---------------------------------------------------
                   ReplyLoginMessageToLock(0xFF, 1); //发送失败响应消息到云锁
                    //System.Threading.Thread.Sleep(300);
                   //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}][{1}]因没有注册将被迫断开连接", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString(), MyLockIDStr));
                   //MyAsynchLockServerSocketService.MyManagerLoginLockUser.RemoveNotLoginUser(ref MyReadWriteChannel.MyTCPClient); 

                                   
             }
            */
         }


        public void SynchCompleteCommand()
        {
            string MyLockIDStr = Encoding.ASCII.GetString(MyReadWriteChannel.MyReadBuffers, 22, 15);
            string MyFindMobileIDStr = CheckRegisterAsDBase(MyLockIDStr);

            //LGJAsynchAccessDBase MyLGJAsynchAccessDBase = new LGJAsynchAccessDBase();
            //this.MyLoginUser.ByteBufferCopy(this.MyRecieveCount);//数据复制
                                                                 //MyLGJAsynchAccessDBase.AsynchLoginAuth(MyLockIDStr,this.MyReadWriteChannel, this.MyAsynchLockServerSocketService);//异步操作数据库
            //MyLGJAsynchAccessDBase.AsynchLoginAuthEx(this.MyLoginUser, this.MyAsynchLockServerSocketService);//异步操作数据库

           // MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("当前核心进程线程工作模式：异步并行"));

            //return;

         
            bool FindResultID = GetChannel(MyLockIDStr);//同步操作数据库
            if (FindResultID)
            {
                //----回显信息----------------------------------------------------------------------------------------
                string MyRecieveDateTimeString = Encoding.ASCII.GetString(MyReadWriteChannel.MyReadBuffers, 39, 15);
                string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
                //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyLockIDStr + "--" + MyRecieveDateTimeString));

                MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarkerStr + "[{0}]注册智能锁：{1}", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint, MyLockIDStr));
                //--注册通道--------------------------------------------------------------------------------------------
                MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForLoginEx(ref MyReadWriteChannel.MyTCPClient, MyLockIDStr, MyFindMobileIDStr); //更新路由注册表

                //MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.CRUDLoginUserList(ref MyReadWriteChannel.MyTCPClient, MyLockIDName, 0); //更新路由注册表
                //MyAsynchLockServerSocketService.MyManagerSocketLoginUser.CRUDLoginUserListForLogin(ref MyReadWriteChannel.MyTCPClient, MyLockIDStr, MyFindMobileIDStr); //更新路由注册表

                //发注册消息到移动端
                OnLineResponseToMobile();
                Byte TimeCompareID = TimeCompare(MyRecieveDateTimeString);//时间比较
                ReplyLoginMessageToLock(0, TimeCompareID);//发送成功响应消息到云锁
            }
            else
            {
                //--取消通道---------------------------------------------------
                ReplyLoginMessageToLock(0xFF, 1); //发送失败响应消息到云锁
                                                  //System.Threading.Thread.Sleep(300);
                                                  //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}][{1}]因没有注册将被迫断开连接", MyReadWriteChannel.MyTCPClient.Client.RemoteEndPoint.ToString(), MyLockIDStr));
                                                  //MyAsynchLockServerSocketService.MyManagerLoginLockUser.RemoveNotLoginUser(ref MyReadWriteChannel.MyTCPClient); 


            }

        }

        private string CheckRegisterAsDBase(string LockIDStr)
         {

             //-----Test数据:查找通道注册[数据库]-----------------------------------
             if (LockIDStr == "89765432BCDA820")//测试例子而已
             {
                 return "9876DDDD8989100";
             }
             if (LockIDStr == "89765432BCDA821")//测试例子而已
             {
                 return  "9876DDDD8989101";
             }

             if (LockIDStr == "89765432BCDA822")//测试例子而已
             {
                 return "9876DDDD8989102";
             }

             if (LockIDStr == "89765432BCDA823")//测试例子
             {
                 return "9876DDDD8989103";
             }

             if (LockIDStr == "89765432BCDA824")//测试例子
             {
                 return "9876DDDD8989104";
             }

             if (LockIDStr == "89765432BCDA825")//测试例子
             {
                 return "9876DDDD8989105";
             }

             if (LockIDStr == "89765432BCDA826")//测试例子
             {
                 return "9876DDDD8989106";
             }

             if (LockIDStr == "89765432BCDA827")//测试例子
             {
                 return "9876DDDD8989107";
             }
             if (LockIDStr == "89765432BCDA828")//测试例子
             {
                 return "9876DDDD8989108";
             }
               /*
             if (LockIDStr == "89765432BCDA829")//测试例子
             {
                 return "9876DDDD8989109";
             }
              * */
            //=====================================
             if (LockIDStr == "860238021873885")//测试例子
             {
                 return "9876DDDD8989109";
             }


              return null;
           

             //----------------------------------------------------------


         }

         private bool GetChannel(string LockIDStr)
         {
             //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             //LockManager MyLockManager = new LockManager(CloudLockConnectString.ConnectionString);

             LockManager MyLockManager = new LockManager();
             Lock MyLock = null;
             Channel MyChannel = null;
              //MyLock = MyLockManager.FindLock(LockIDStr);
             MyLock = MyLockManager.FindLock(LockIDStr);
             if (MyLock == null)
             {
                 return false;
             }
             else
             {

                 if (MyLock.Status == 3)
                 {

                     //ChannelManager MyChannelManager = new ChannelManager(CloudLockConnectString.ConnectionString);
                     //MyChannel = MyChannelManager.FindChannel(LockIDStr);
                     
                     ChannelManager MyChannelManager = new ChannelManager();
                     MyChannel = MyChannelManager.FindChannel(LockIDStr);

                     if (MyChannel == null)
                     {
                         return false;
                     }
                     else
                     {

                         MyFindMobileIDStr = MyChannel.MobileID; 
                         return true;

                     }
                     
                    
                 }
                 else
                 {
                     return false;
                 }

             }

           


             //----------------------------------------------------------


         }

         private void ReplyLoginMessageToLock(Byte  ProcessID,Byte TimeSynchID )
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
         //---------------------------------------

         private void OnLineResponseToMobile()
         {
             string CommandMessageStr = "locklogin";
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
           
             CommandMessageStr = CommandMessageStr + "#" + MyFindMobileIDStr + "-" + MyLockIDStr + "#[" +"openlock"+","+GetDateTimeWeekIndex()+"]!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];


             MySendMessageBytes[2] = 250;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }
               
            

             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[1通道和2号通道]--------------------------------------------------------------------------------
             try
             {
                 //LockServerLib.FindMobileChannel MyBindedMobileChannel = new LockServerLib.FindMobileChannel(MyFindMobileIDStr);
                 //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;
                 
                 
                 //this.NewReadWriteChannel = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
               
                 /*
                 if (MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].LockID == "***************")
                 {
                     this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                 }
                 else
                 {
                     MyAsynchLockServerSocketService.DisplayResultInfor(1, "移动服务器端响应通道标志错误[1]");
                 }
                 */
                 SocketServiceReadWriteChannel NewReadWriteChannel;
                 //进行广播！
                 NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
            
                 NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[1].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                    

               
              }
             catch
             {
                 //无通道记录；；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动服务器端响应通道错误[2]");
             }

            
            


         }
      
    }
}
