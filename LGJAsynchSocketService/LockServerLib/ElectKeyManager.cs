using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SmartBusServiceLib;
//using SmartLockGridViewLib;

namespace LGJAsynchSocketService.LockServerLib
{
    public class ElectKeyManager
    {
        LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount = 0;
         byte[] MyReadBuffers;
         int nCRUDflag=-1;

         string MyMobileIDStr;
         string MyLockIDStr;
        
         string DateTimeStr;
         string NameIDStr;
         string KeyNumberIDStr;
         int KeyIDNumber;
         string KeyIDStr;

        string BaseMessageString; 
        string ResultStr;

        public ElectKeyManager(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount, int CRUDflag)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
            this.nCRUDflag = CRUDflag;//0：ADDKEY，1：DELETEKEY 3：TEMPKEY
        }
         public ElectKeyManager(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteChannel, int InRecieveCount, int CRUDflag)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteChannel;
            this.MyReadBuffers = InputReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = InRecieveCount;
            this.nCRUDflag = CRUDflag;//0：ADDKEY，1：DELETEKEY 3：TEMPKEY
        }

         public void RequestToLock()
         {
            
              //2.解析来自移动端的消息-----------------------------------------------------------------------------------------------------------------------
             BaseMessageString = null;          
             BaseMessageString = Encoding.UTF8.GetString(this.MyReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
             
             MyLockIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
             MyMobileIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);

              //--找智能锁通道--------------------------------------------------------------------------------
             FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr); //测试正确
             MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));

             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);

             if (MyLoginUser == null)
             {
                 NotFindLockResponseToMobile();                 
                 return;
             }
             this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
             switch (this.nCRUDflag)
             {
                 //ADDKEY
                 case 0:

                     NameIDStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf("#") + 1);
                     //NameIDStr = NameIDStr.Substring(NameIDStr.IndexOf(",") + 1);
                     NameIDStr = NameIDStr.Substring(0, NameIDStr.IndexOf(","));
                     //KeyIDStr =  BaseMessageString.Substring(BaseMessageString.LastIndexOf(",") + 1,6);
                     this.NewReadWriteChannel.TempString = NameIDStr;//临时存储增加的姓名字符串汉字
                        //GetDateTimeWeekIndex();
                     //this.NewReadWriteChannel.TempNumber= AddKeySaveTODataCenterBaseEx();//从数据库获得编号
                     //--再转发出去-----------------------------------------------------------------
                     addkeyDriveToSmartLockExx();
                     
                       //if (this.NewReadWriteChannel.TempNumber > 0)
                     //{
                     //    addkeyDriveToSmartLockEx(this.NewReadWriteChannel.TempNumber);
                     //}
                     //else
                     //{
                     //    MyAsynchLockServerSocketService.DisplayResultInfor(1, "增加密钥数据库操作错误！");
                     //}
                     break;

                 //DELETEKEY
                 case 1:
                     KeyNumberIDStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf("#") + 1);
                     KeyNumberIDStr = KeyNumberIDStr.Replace("!", "");
                     this.NewReadWriteChannel.TempString = KeyNumberIDStr;//临时存储删除的密钥序号！
                     //GetDateTimeWeekIndex();
                     this.DateTimeStr = GetKeyDateStrFromODataBase();
                     if (this.DateTimeStr != null)
                     {
                         //--再转发出去----------------                
                         //deletekeyDriveToSmartLock(KeyNumberIDStr);  
                         deletekeyDriveToSmartLockEx(int.Parse(KeyNumberIDStr));
                     }
                     else
                     {
                        
                         NotFindLockResponseToMobile();//直接返回错误应答！数据库不存在或者已经删除
                         MyAsynchLockServerSocketService.DisplayResultInfor(1, "删除密钥数据库读取错误！");

                     }
                     break;
                 
                 //TEMPKEY
                 case 2:
                     KeyIDStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf("#") + 1);
                     KeyIDStr = KeyIDStr.Replace("!", "");
                     //--再转发出去----------------
                     tempkeyDriveToSmartLockEx(KeyIDStr);
                     break;
                 default:
                     break;


             }
             

            
         }

         public void ResponseToMobile()
         {
               
              MyMobileIDStr = MyLoginUser.MobileID;
              MyLockIDStr = MyLoginUser.LockID;

              string CommandMessageID = null;
              string CommandMessageStr = null;
              Byte RecieveMessageResult = MyReadBuffers[22]; //结果标志
              string ResultStr;
              if (RecieveMessageResult == 0)
              {
                  ResultStr = "true" ;
                 
              }
              else
              {
                  ResultStr = "false[10]";

              }

              byte[] MySendBaseMessageBytes=null;
              int nBuffersLenght=0;
              byte[] MySendMessageBytes = null; 
              byte nMessageFlag=255;
              switch (this.nCRUDflag)
              {
                    case 0:
                                                
                         NameIDStr = this.MyReadWriteChannel.TempString;
                         if (NameIDStr != null)//以防重复
                         {
                             this.MyReadWriteChannel.TempString = null;
                             //int KeyIDNumber = this.MyReadWriteChannel.TempNumber; 
                             //Byte KeyIDNumber = MyReadBuffers[23];//返回新增Key的序号
                             if (RecieveMessageResult == 0)
                             {
                                 string HexMessageKeyIDNumber = string.Format("{0:X2}", MyReadBuffers[25]) + string.Format("{0:X2}", MyReadBuffers[24]);
                                 KeyIDNumber = Convert.ToUInt16(HexMessageKeyIDNumber, 16);
                                 DateTimeStr = Encoding.ASCII.GetString(MyReadBuffers, 26, 15);
                                 //AddKeySaveTODataCenterBase((int)KeyIDNumber, NameIDStr);//保存到数据库中心
                                 //AddKeyUpdateTODataCenterBase(KeyIDNumber);
                                 AddKeySaveTODataCenterBaseEx();
                                 ResultStr = ResultStr + "[" + NameIDStr + "@" + KeyIDNumber.ToString() + "]";
                             }
                             else
                             {
                                 ResultStr = ResultStr + "[" + NameIDStr + "]";


                             }

                             CommandMessageID = "addkey";
                             CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                             MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                             nBuffersLenght = MySendBaseMessageBytes.Length;
                             //nMessageFlag = 255;

                         }
                         else
                         {
                             MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]增加密钥重复应答错误！",this.MyLockIDStr));
                             return; 
                         }

                        

                      break;

                   case 1:

                      if (this.MyReadWriteChannel.TempString != null)
                      {
                         
                          if (RecieveMessageResult == 0)
                          {
                              ResultStr = ResultStr + "[" + this.MyReadWriteChannel.TempString + "]";
                              int TempKeyIDNumber = int.Parse(this.MyReadWriteChannel.TempString);
                              DeleteKeySaveTODataCenterBaseEx(TempKeyIDNumber);//采用同步标志删除数据库中心记录

                          }
                          else
                          {
                              ResultStr = ResultStr + "[" + this.MyReadWriteChannel.TempString + "]";

                              MyAsynchLockServerSocketService.DisplayResultInfor(1, "删除密钥智能锁操作错误！");
                          }
                          CommandMessageID = "deletekey";
                          CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                          MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                          nBuffersLenght = CommandMessageStr.Length;
                           //MySendMessageBytes = new byte[nBuffersLenght + 3];  
                          this.MyReadWriteChannel.TempString = null;
                      }
                      else
                      {
                          MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]删除密钥重复应答错误！", this.MyLockIDStr));
                          return; 
                      }
                     

                      break;

                     case 2:
                      CommandMessageID = "tempkey";
                      CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                      MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                      nBuffersLenght = CommandMessageStr.Length;
                    
                    
                      break;


              }
            //---------------------------
              MySendMessageBytes = new byte[nBuffersLenght + 3];
              MySendMessageBytes[2] = nMessageFlag;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

                  
                 //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
             //FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MessageMobileStr);
             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;
            //--再转发出去-----------
             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
             //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
 


             //----1.保存数据库备查-------------------------------------------------------------------------------------------------------------
             //MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);


             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
             try
             {

                 //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                 int ReplyChannelIndex = MyLoginUser.ReplyChannelLoginID - 1;
                 if (ReplyChannelIndex < 2)
                 {
                     this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[ReplyChannelIndex].MyReadWriteSocketChannel;
                     MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                 }
                 else
                 {
                     //通道错误；
                     MyAsynchLockServerSocketService.DisplayResultInfor(1, "从智能端响应再转发移动端寻找通道错误：" + ReplyChannelIndex.ToString());
                 }


             }
             catch
             {
                  MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }
           
         }

         private void NotFindLockResponseToMobile()
         {
          
             ResultStr = "false[11]"; ;
             string CommandMessageStr=null;
             string CommandMessageID = null; 

             byte[] MySendBaseMessageBytes = null;
             int nBuffersLenght = 0;
             byte[] MySendMessageBytes = null;
            switch (this.nCRUDflag)
             {
                 case 0:
                     ResultStr = ResultStr + "[" + NameIDStr + "]";
                     CommandMessageID = "addkey";
                       //CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                     //MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                     //nBuffersLenght = MySendBaseMessageBytes.Length;
                    
                     break;

                 case 1:
                     KeyNumberIDStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf("#") + 1);
                     KeyNumberIDStr = KeyNumberIDStr.Replace("!", "");
                     ResultStr = ResultStr + "[" + KeyNumberIDStr + "]";      
                    CommandMessageID = "deletekey";
                    //CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                    //MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                    //nBuffersLenght = CommandMessageStr.Length;     

                     break;

                 case 2:
                     CommandMessageID = "tempkey";
                     //CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
                     //MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
                     //nBuffersLenght = CommandMessageStr.Length;

                     break;


             }
             CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + "!";
             MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);
             nBuffersLenght = MySendBaseMessageBytes.Length;
             MySendMessageBytes = new byte[nBuffersLenght + 3]; 
             MySendMessageBytes[2] = 255;
             MySendBaseMessageBytes.CopyTo(MySendMessageBytes, 3);
            
             //填充
             /*
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }
             */
             //----1.保存数据库备查-------------------------------------------------------------------------------------------------------------
             //MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);

             try
             {
                 this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误！");
             }

         }
        
         private void GetDateTimeWeekIndex()
         {
                //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
             //string DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
             DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
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

             DateTimeStr = DateTimeStr + nReturn;

             //return DateTimeStr + nReturn;
         }
        
         private string GetEndDateTimeWeekIndex()
         {
             //monday,tuesday,wednesday,thursday,friday,saturday,sunday  
             string DateTimeStr = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now.AddHours(6));//6个小时内有效
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

         private void addkeyDriveToSmartLock(string KeyStr)
         {
                          
             int MySendByteCount = 44;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0522";
             }
             else
             {
                 HexSendMessageIDString = "2205";
             }


             byte[] MyKeyStrBytes = Encoding.UTF8.GetBytes(KeyStr);
             //string MyDateTimeString = GetDateTimeWeekIndex();
             GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(DateTimeStr);

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

             //--填充字符密码字节----

             for (int i = 0; i < 6; i++)
             {

                 MySendMessageBytes[22 + i] = MyKeyStrBytes[i];

             }

             //--填充日期字节----
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[28 + i] = MyDateTimeBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private void addkeyDriveToSmartLockEx(int KeyNumberID)
         {
             //增加密钥加强版
             int MySendByteCount = 41;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0522";
             }
             else
             {
                 HexSendMessageIDString = "2205";
             }


             //string MyDateTimeString = GetDateTimeWeekIndex();
             GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(DateTimeStr);

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
                       
             //--填充字符密码字节----
             /*
             for (int i = 0; i < 6; i++)
             {

                 MySendMessageBytes[22 + i] = MyKeyStrBytes[i];

             }
             */

             MySendMessageBytes[22] = 1;//只增加一个用户

             //--用户序号
             string HexKeyNumberIDString = string.Format("{0:X4}", KeyNumberID);
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[23] = Convert.ToByte(HexKeyNumberIDString.Substring(2, 2), 16);
                 MySendMessageBytes[24] = Convert.ToByte(HexKeyNumberIDString.Substring(0, 2), 16);
             }
             else
             {
                 MySendMessageBytes[23] = Convert.ToByte(HexKeyNumberIDString.Substring(0, 2), 16);
                 MySendMessageBytes[24] = Convert.ToByte(HexKeyNumberIDString.Substring(2, 2), 16);
             }

             //--填充日期字节----
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[25 + i] = MyDateTimeBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private void addkeyDriveToSmartLockExx()
         {

             int MySendByteCount = 23;//加校验字节0x17

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0522";
             }
             else
             {
                 HexSendMessageIDString = "2205";
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

             

             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private void deletekeyDriveToSmartLock(string NumberStr)
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 24;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0720";
             }
             else
             {
                 HexSendMessageIDString = "2007";
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

             MySendMessageBytes[22] = Convert.ToByte(NumberStr);

             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private void deletekeyDriveToSmartLockEx(int KeyNumberID)
         {
             
             int MySendByteCount = 41;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0720";
             }
             else
             {
                 HexSendMessageIDString = "2007";
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

             MySendMessageBytes[22] = 1;//只删除一个用户

             //--用户序号
             string HexKeyNumberIDString = string.Format("{0:X4}", KeyNumberID);
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MySendMessageBytes[23] = Convert.ToByte(HexKeyNumberIDString.Substring(2, 2), 16);
                 MySendMessageBytes[24] = Convert.ToByte(HexKeyNumberIDString.Substring(0, 2), 16);
             }
             else
             {
                 MySendMessageBytes[23] = Convert.ToByte(HexKeyNumberIDString.Substring(0, 2), 16);
                 MySendMessageBytes[24] = Convert.ToByte(HexKeyNumberIDString.Substring(2, 2), 16);
             }


              //--填充日期字节----
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(DateTimeStr);
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[25 + i] = MyDateTimeBytes[i];

             }
            
          
             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }        
        
         private void tempkeyDriveToSmartLock(string tempKeyStr)
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 44;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0920";
             }
             else
             {
                 HexSendMessageIDString = "2009";
             }


             byte[] MyKeyStrBytes = Encoding.UTF8.GetBytes(tempKeyStr);
             //string MyDateTimeString = GetDateTimeWeekIndex();
             GetDateTimeWeekIndex();
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(DateTimeStr);

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

             //--填充字符密码字节----

             for (int i = 0; i < 6; i++)
             {

                 MySendMessageBytes[22 + i] = MyKeyStrBytes[i];

             }

             //--填充日期字节----
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[28 + i] = MyDateTimeBytes[i];

             }

             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

         }

         private void tempkeyDriveToSmartLockEx(string tempKeyStr)
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 75;//加校验字节29+45+1=75

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0920";
             }
             else
             {
                 HexSendMessageIDString = "2009";
             }

             //---密码生成！----------------------------------------------------
             //byte[] MyKeyStrBytes = Encoding.UTF8.GetBytes(tempKeyStr);
             //byte ntempKeyStr1 = byte.Parse(tempKeyStr.Substring(0, 1));
             //byte ntempKeyStr2 = byte.Parse(tempKeyStr.Substring(1, 1));
             //byte ntempKeyStr3 = byte.Parse(tempKeyStr.Substring(2, 1));
             //byte ntempKeyStr4 = byte.Parse(tempKeyStr.Substring(3, 1));
             //byte ntempKeyStr5 = byte.Parse(tempKeyStr.Substring(5, 1));

             byte[] MyKeyStrBytes=new byte[6];
             for (int i = 0; i < 6; i++)
             {

                 MyKeyStrBytes[i] = byte.Parse(tempKeyStr.Substring(i, 1));

             }
           


             //string MyDateTimeString = GetDateTimeWeekIndex();//生效时间
             GetDateTimeWeekIndex();//生效时间--创建时间
             byte[] MyDateTimeBytes = Encoding.UTF8.GetBytes(DateTimeStr);

             string MyEndDateTimeString = GetEndDateTimeWeekIndex();//失效时间
             byte[] MyEndDateTimeBytes = Encoding.UTF8.GetBytes(MyEndDateTimeString);




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

             //--填充临时字符密码字节----
             
             for (int i = 0; i < 6; i++)
             {

                 MySendMessageBytes[22 + i] = MyKeyStrBytes[i];

             }
            
             MyKeyStrBytes.CopyTo(MySendMessageBytes, 22); 

             MySendMessageBytes[28] = 1;//一次性密码

             //--填充起始时间点字节----
              /*
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[29 + i] = MyDateTimeBytes[i];

             }
              * */
          
             MyDateTimeBytes.CopyTo(MySendMessageBytes, 29); 
             //--填充结束时间点字节----
             /*
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[44 + i] = MyEndDateTimeBytes[i];

             }
             */
         
             MyEndDateTimeBytes.CopyTo(MySendMessageBytes, 44);  
             //--填充生成时间字节----
              /*
             for (int i = 0; i < 15; i++)
             {

                 MySendMessageBytes[59 + i] = MyDateTimeBytes[i];

             }
              * */

             MyDateTimeBytes.CopyTo(MySendMessageBytes, 59); 


             MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
           
              //----Test------------------------------------------------------------------------------------------
             /*
             string SendMessageString = null;
             for (int i = 0; i < MySendMessageBytes.Length; i++)
             {

                 SendMessageString = SendMessageString + string.Format("{0:X2}", MySendMessageBytes[i]);

             }
             
             MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, "2009", SendMessageString); 
            * */

         }

         private void AddKeySaveTODataCenterBase(int KeyIDNumber, string NameIDStr)
         {

             //插入数据库
             //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];

             LockKey AnyLockKey = new LockKey();

             AnyLockKey.LockID = MyLockIDStr;
             AnyLockKey.LockKeyID = KeyIDNumber;
             AnyLockKey.OwerName = NameIDStr;
             AnyLockKey.KeyString = "******";
             AnyLockKey.CreateTime = DateTime.Now;
             LockKeyManager MyLockKeyManager = new LockKeyManager();//CloudLockConnectString.ConnectionString
             MyLockKeyManager.InsertLockKey(AnyLockKey); 

         }

         private int AddKeySaveTODataCenterBaseEx()
         {

             //-----插入数据库----------------------------------
               //int KeyIDNumberID=0;
             //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             //LockKeyManager MyLockKeyManager = new LockKeyManager(CloudLockConnectString.ConnectionString);
            
             //LockKeyManager MyLockKeyManager = new LockKeyManager();
             LockKeyManager MyLockKeyManager = new LockKeyManager(); 

             LockKey AnyLockKey = new LockKey();
             AnyLockKey.LockID =this.MyLockIDStr;
             AnyLockKey.LockKeyID = this.KeyIDNumber;
             AnyLockKey.OwerName = this.NameIDStr;
             AnyLockKey.KeyDateStr = this.DateTimeStr;
             //return MyLockKeyManager.InsertLockKeyEx(AnyLockKey);
             return MyLockKeyManager.InsertLockKeyEx(AnyLockKey);
           
            


         }

         private string GetKeyDateStrFromODataBase()
         {

             LockKeyManager MyLockKeyManager = new LockKeyManager();
                  //LockKey AnyLockKey = new LockKey();
             //AnyLockKey.LockID = this.MyLockIDStr;
             //AnyLockKey..OwerName = this.NameIDStr;
             //AnyLockKey.KeyDateStr = this.DateTimeStr;
             LockKey AnyLockKey = MyLockKeyManager.FindOneLockKey(this.MyLockIDStr, int.Parse(KeyNumberIDStr));
             if (AnyLockKey != null)
             {
                 return MyLockKeyManager.FindOneLockKey(this.MyLockIDStr, int.Parse(KeyNumberIDStr)).KeyDateStr;
             }
             else
             {
                 return null;
             }
             
                 


         }

         private void AddKeyUpdateTODataCenterBase(int KeyIDNumber)
         {

             //更改数据库
             //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             //LockKeyManager MyLockKeyManager = new LockKeyManager(CloudLockConnectString.ConnectionString);

             LockKeyManager MyLockKeyManager = new LockKeyManager();
             LockKey AnyLockKey = new LockKey();

             AnyLockKey.LockID = MyLockIDStr;
             AnyLockKey.LockKeyID = KeyIDNumber;
             MyLockKeyManager.UpdateLockKey(AnyLockKey);


         }
        
         private void DeleteKeySaveTODataCenterBase(int KeyIDNumber)
         {

                //从数据库删除
                  //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             // LockKeyManager MyLockKeyManager = new LockKeyManager(CloudLockConnectString.ConnectionString);
             LockKeyManager MyLockKeyManager = new LockKeyManager();
             LockKey AnyLockKey = new LockKey();

             AnyLockKey.LockID = MyLockIDStr;
             AnyLockKey.LockKeyID = KeyIDNumber;
             MyLockKeyManager.DeleteLockKey(AnyLockKey);//--切底删除---

         }

         private void DeleteKeySaveTODataCenterBaseEx(int KeyIDNumber)
         {

               //从数据库删除
              //ConnectionStringSettings CloudLockConnectString = ConfigurationManager.ConnectionStrings["CloudLockConnectString"];
             //LockKeyManager MyLockKeyManager = new LockKeyManager(CloudLockConnectString.ConnectionString);
           
              //LockKeyManager MyLockKeyManager = new LockKeyManager();

             LockKeyManager MyLockKeyManager = new LockKeyManager(); 

             LockKey AnyLockKey = new LockKey();
             AnyLockKey.LockID = MyLockIDStr;
             AnyLockKey.LockKeyID = KeyIDNumber;

              //MyLockKeyManager.DeleteLockKeyEx(AnyLockKey);//--采用标志删除--

             MyLockKeyManager.DeleteLockKeyEx(AnyLockKey);//--采用标志删除--

         }
        
         protected void SendMessageForSaveDB(string InMessageID,byte[] InSendMessageBuffers)
         {
             //有问题！！
             string LockID = MyLockIDStr;// "1234567890ABCDE";
             string MessageID = InMessageID;// string.Format("{0:X2}", InSendMessageBuffers[9]) + string.Format("{0:X2}", InSendMessageBuffers[8]);
             string SendMessageString = null;
             for (int i = 0; i < InSendMessageBuffers.Length; i++)
             {

                 SendMessageString = SendMessageString + string.Format("{0:X2}", InSendMessageBuffers[i]);

             }
             MessageListManager MyMessageListManager = new MessageListManager();
             MyMessageListManager.InsertMessageSave(LockID, MessageID, SendMessageString);

         }
       

    }
}
