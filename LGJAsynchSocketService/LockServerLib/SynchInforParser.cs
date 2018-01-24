using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBusServiceLib;
//using SmartLockGridViewLib;
namespace LGJAsynchSocketService.LockServerLib
{
    public class SynchInforParser
    {

        LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         int MyRecieveCount;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;
         string MyMessageTypeID;
         public SynchInforParser(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public SynchInforParser(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteObject, int InputRecieveCount)
        {
            
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteObject;
            this.MyReadBuffers = InputReadWriteObject.MyReadBuffers;
            this.MyRecieveCount = InputRecieveCount;
        }

         public void CompleteCommand()
         {
             
             MyLockIDStr = MyLoginUser.LockID;
             MyMobileIDStr = MyLoginUser.MobileID;

             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 MyMessageTypeID = string.Format("{0:X2}", MyReadBuffers[9]) + string.Format("{0:X2}", MyReadBuffers[8]);
             }
             else
             {
                 MyMessageTypeID = string.Format("{0:X2}", MyReadBuffers[8]) + string.Format("{0:X2}", MyReadBuffers[9]);
             }

              

             ResponseMessageToLock(0);//发送响应消息到智能锁


             switch (MyMessageTypeID)
             {
                 case "1011":
                     MasterKeySynchProc();
                     break;

                 case "1013":
                     AddKeySynchProc();
                     break;
                
                 case "1015":
                     DeleteKeySynchProc();
                     break;

                 case "1017":
                     AllInforSynchProc();
                     break;

                 case "2003":
                     MasterKeySynchProc();
                     break;

                 case "2007":
                     DeleteKeySynchProc();
                     break;

                 case "2105":
                     AddKeySynchProc();
                     break;

                  default:
                     break;




             }

            

         }
      
         private void ResponseMessageToLock(Byte ProcessResultID)
         {
                        
             int MySendByteCount = 22+1+1;//加校验字节=24
             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;

             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 //HexSendMessageIDString = "1810";

                 switch (MyMessageTypeID)
                 {
                     case "1011":
                         HexSendMessageIDString = "1210";
                         break;

                     case "1013":
                         HexSendMessageIDString = "1410";
                         break;

                     case "1015":
                         HexSendMessageIDString = "1610";
                         break;

                     case "1017":
                         HexSendMessageIDString = "1810";
                         break;

                     case "2003":
                         HexSendMessageIDString = "0420";
                         break;

                     case "2007":
                         HexSendMessageIDString = "0820";
                         break;
                     case "2105":
                         HexSendMessageIDString = "0621";
                         break;

                     default:
                         HexSendMessageIDString = "1810";
                         break;




                 }
             }
             else
             {
                 HexSendMessageIDString = "1018";//暂不执行
             }
             
             //填充字节信息头2个字节------------------------------------------------------------
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
             //原样打回所谓流水号-----------------------------------------------------------------
             for (int i = 0; i < 5; i++)
             {

                 MySendMessageBytes[i + 3] = this.MyReadBuffers[i+3];

             }

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
             MySendMessageBytes[22] = ProcessResultID;// 0;

             MyAsynchLockServerSocketService.StartAsynchSendMessage(MyReadWriteChannel, MySendMessageBytes);
               //----Test----------------------------------------------------------------------------------------
             /*
             string BaseMessageString = null;
             for (int i = 0; i < 24; i++)
             {

                 BaseMessageString = BaseMessageString + string.Format("{0:X2}", MySendMessageBytes[i]);

             }
             MyAsynchLockServerSocketService.SamrtLockMessageToSave(MyLockIDStr, "1018", BaseMessageString);
              */

         }

         private DateTime  GetCreateDateFromStr(string CreateDateStr)
         {             
             int Year;
             int Month;
             int Date;
             int Hour;
             int Miminute;
             int Second;
         
             Year = int.Parse(CreateDateStr.Substring(0, 4));
             Month = int.Parse(CreateDateStr.Substring(4, 2));
             Date = int.Parse(CreateDateStr.Substring(6, 2));
             Hour = int.Parse(CreateDateStr.Substring(8, 2));
             Miminute = int.Parse(CreateDateStr.Substring(10, 2));
             Second = int.Parse(CreateDateStr.Substring(12, 2));

             return new DateTime(Year, Month, Date, Hour, Miminute, Second);  

         }
       
         private void AllInforSynchProc()
         {

             string MasterKey=null;
             string CreateDateStr;
             int KeyCount;
             int KeyID;
             //---母码部分--------------------------------------------------------------------------------------------------------------
             for (int i = 0; i < 6; i++)
             {

                 MasterKey = MasterKey + this.MyReadBuffers[22+i].ToString();

             }
             CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, 28, 15); 
              //CreateDateStr = CreateDateStr.Insert(4, "-");
             //CreateDateStr = CreateDateStr.Insert(7, "-");
             //CreateDateStr = CreateDateStr.Insert(10, " ");
             //CreateDateStr = CreateDateStr.Insert(13, ":");
             //CreateDateStr = CreateDateStr.Insert(16, ":");
              
             MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]母码修改为[{1}]",MyLockIDStr, MasterKey));

              //LockManager MyLockManager = new LockManager();
             //MyLockManager.UpdateMasterKey(MyLockIDStr, MasterKey);//--保存数据库

             LockManager MyLockManager = new LockManager();
             MyLockManager.UpdateMasterKey(MyLockIDStr, MasterKey);//--保存新母码到数据库
 
             //--密钥部分-------------------------------------------------------------------------------------------------------------
             KeyCount = this.MyReadBuffers[43];
             //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("当前有效密钥数[{0}]", KeyCount)); 
             //--------------------------------------------------------------------------------------------------
             int ReturnCode;
             LockKeyManager MyLockKeyManager = new LockKeyManager(); 
             for (int i = 0; i < KeyCount; i++)
             {
                 int Index=44+17*i;
                 KeyID = this.MyReadBuffers[Index];
                 CreateDateStr = null;
                 CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, Index+2, 15);
                 
                 //CreateDateStr = CreateDateStr.Insert(4, "-");
                 //CreateDateStr = CreateDateStr.Insert(7, "-");
                // CreateDateStr = CreateDateStr.Insert(10, " ");
                // CreateDateStr = CreateDateStr.Insert(13, ":");
                // CreateDateStr = CreateDateStr.Insert(16, ":");
                // MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]号密钥创建日期[{1}]", KeyID, CreateDateStr)); 
                 //-------------------------------------------------------------------------------------------------

                  //LockKeyManager MyLockKeyManager = new LockKeyManager();
                 LockKey AnyLockKey = new LockKey();
                 AnyLockKey.LockID = this.MyLockIDStr;
                 AnyLockKey.LockKeyID = KeyID;
                 AnyLockKey.OwerName = "XXX"+string.Format("{0:D2}",KeyID);
                 AnyLockKey.CreateTime = this.GetCreateDateFromStr(CreateDateStr);  
                 AnyLockKey.KeyDateStr = CreateDateStr;
                 //ReturnCode=MyLockKeyManager.SynchAddLockKey(AnyLockKey);
                 ReturnCode = MyLockKeyManager.SynchAddLockKey(AnyLockKey);
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]保存[{1}]号密钥到数据库[{2}]", MyLockIDStr, KeyID, ReturnCode)); 


             }
            




         }

         private void MasterKeySynchProc()
         {

             string MasterKey = null;
             string CreateDateStr;
             //---母码部分--------------------------------------------------------------------------------------------------------------
             for (int i = 0; i < 6; i++)
             {

                 MasterKey = MasterKey + this.MyReadBuffers[22 + i].ToString();

             }
             CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, 28, 15);
           
             //CreateDateStr = CreateDateStr.Insert(4, "-");
             //CreateDateStr = CreateDateStr.Insert(7, "-");
             //CreateDateStr = CreateDateStr.Insert(10, " ");
             ///CreateDateStr = CreateDateStr.Insert(13, ":");
             //CreateDateStr = CreateDateStr.Insert(16, ":");
            
             MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]母码修改为[{1}]",MyLockIDStr, MasterKey));

              //LockManager MyLockManager = new LockManager();
             //MyLockManager.UpdateMasterKey(MyLockIDStr, MasterKey);//--保存数据库

             LockManager MyLockManager = new LockManager();
             MyLockManager.UpdateMasterKey(MyLockIDStr, MasterKey);//--保存数据库



         }

         private void AddKeySynchProc()
         {
             string CreateDateStr;
             int KeyCount;
             int KeyID;
    
             //--密钥部分-------------------------------------------------------------------------------------------------------------
             KeyCount = this.MyReadBuffers[22];
             //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("当前有效密钥数[{0}]", KeyCount));
             int ReturnCode;
             LockKeyManager MyLockKeyManager = new LockKeyManager(); 
             for (int i = 0; i < KeyCount; i++)
             {
                 int Index = 23 + 17 * i;
                 KeyID = this.MyReadBuffers[Index];
                 CreateDateStr = null;
                 CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, Index + 2, 15);
                
                  //CreateDateStr = CreateDateStr.Insert(4, "-");
                 //CreateDateStr = CreateDateStr.Insert(7, "-");
                 //CreateDateStr = CreateDateStr.Insert(10, " ");
                 //CreateDateStr = CreateDateStr.Insert(13, ":");
                 //CreateDateStr = CreateDateStr.Insert(16, ":");
                 //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]号密钥创建日期[{1}]", KeyID, CreateDateStr));

                  //LockKeyManager MyLockKeyManager = new LockKeyManager();
                 //LockKeyManager MyLockKeyManager = new LockKeyManager(this.MyAsynchLockServerSocketService.MySqlConnection); 
                
                 LockKey AnyLockKey = new LockKey();
                 AnyLockKey.LockID = this.MyLockIDStr;
                 AnyLockKey.LockKeyID = KeyID;
                 AnyLockKey.OwerName = "XXX" + string.Format("{0:D2}", KeyID);
                 AnyLockKey.CreateTime = this.GetCreateDateFromStr(CreateDateStr);
                 AnyLockKey.KeyDateStr = CreateDateStr;
                  //ReturnCode = MyLockKeyManager.SynchAddLockKey(AnyLockKey);
                 ReturnCode = MyLockKeyManager.SynchAddLockKey(AnyLockKey);

                 MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]保存[{1}]号密钥到数据库[{2}]", MyLockIDStr, KeyID, ReturnCode)); 

             }





         }

         private void DeleteKeySynchProc()
         {

             
             string CreateDateStr;
             int KeyCount;
             int KeyID;
                         
             //--密钥部分-------------------------------------------------------------------------------------------------------------
             KeyCount = this.MyReadBuffers[22];
              //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("当前有效密钥数[{0}]", KeyCount));
             //-----------------------------------------------------------------------------------------------------------------------
             LockKeyManager MyLockKeyManager = new LockKeyManager(); 
             for (int i = 0; i < KeyCount; i++)
             {
                 int Index = 23 + 17 * i;
                 KeyID = this.MyReadBuffers[Index];
                 CreateDateStr = null;
                 CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, Index + 2, 15);
                 //CreateDateStr = CreateDateStr.Insert(4, "-");
                 //CreateDateStr = CreateDateStr.Insert(7, "-");
                 //CreateDateStr = CreateDateStr.Insert(10, " ");
                 //CreateDateStr = CreateDateStr.Insert(13, ":");
                 //CreateDateStr = CreateDateStr.Insert(16, ":");
                 //MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]号密钥创建日期[{1}]", KeyID, CreateDateStr));

                    //LockKeyManager MyLockKeyManager = new LockKeyManager();
               
                 LockKey AnyLockKey = new LockKey();
                 AnyLockKey.LockID = MyLockIDStr;
                 AnyLockKey.LockKeyID = KeyID;

                 //MyLockKeyManager.DeleteLockKeyEx(AnyLockKey);//--采用标志删除--
                 MyLockKeyManager.DeleteLockKeyEx(AnyLockKey);//--采用标志删除--
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("[{0}]删除[{1}]号密钥从数据库",MyLockIDStr, KeyID)); 
             }





         }

    }
}
