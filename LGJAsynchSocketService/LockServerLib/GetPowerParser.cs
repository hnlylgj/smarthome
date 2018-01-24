using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGJAsynchSocketService.LockServerLib
{
    public class GetPowerParser
    {
         LoginUser MyLoginUser;
         AsynchLockServerSocketService MyAsynchLockServerSocketService;
         SocketServiceReadWriteChannel MyReadWriteChannel;
         SocketServiceReadWriteChannel NewReadWriteChannel;
         int MyRecieveCount = 0;
         byte[] MyReadBuffers;

         string MyLockIDStr;
         string MyMobileIDStr;

         public GetPowerParser(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
         public GetPowerParser(AsynchLockServerSocketService InputAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputReadWriteChannel, int InRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InputAsynchSocketServiceBaseFrame;
            this.MyReadWriteChannel = InputReadWriteChannel;
            this.MyReadBuffers = InputReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = InRecieveCount;
        }

         public void RequestToLock()
         {

              //解析来自移动端的消息-----------------------------------------------------------------------------------------------------------------------
             string BaseMessageString = Encoding.UTF8.GetString(this.MyReadWriteChannel.MyReadBuffers, 3, MyRecieveCount - 3);
         
             MyLockIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("#") + 1, 15);
             MyMobileIDStr = BaseMessageString.Substring(BaseMessageString.IndexOf("-") + 1, 15);
             string ParaKeyStr = BaseMessageString.Substring(BaseMessageString.LastIndexOf(",") + 1, 6);
               
              //----2.驱动智能锁----------------------------   
              //--找智能锁通道------------------------------

             FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr); //测试正确
             MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));

             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);
            
             //--再转发出去-------------
             if ( MyLoginUser != null)
             {
                 this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
                 DriveToSmartLock();
             }
             else
             {
                 //--否则原路或者走响应通道快速返回[0、1]
                 NotFindLockResponseToMobile();
             }
           
            

             //return ;
         }

         public void ResponseToMobile()
         {
             
             string CommandMessageID = "getpower";
             //---1.找智能锁本身通道的路由表记录-------------------------------------
               //LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.FindLoginUserList(ref  this.MyReadWriteChannel.MyTCPClient);  
             //-------------------------------------
              
              //FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
              //LoginUser MyLoginUser=MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
              
                 //string MessageMobileStr = MyLoginUser.MobileID;
             //string MessageLockStr =MyLoginUser.LockID;

              MyLockIDStr = MyLoginUser.LockID;
              MyMobileIDStr = MyLoginUser.MobileID;

              Byte RecieveMessageResult= MyReadBuffers[22];//结果标志
           
              string ResultStr;
              if (RecieveMessageResult == 0)
              {
                  ResultStr = "true";
              }
              else
              {
                  ResultStr = "false[10]";
              }

              string PowerInforStr;
              if (RecieveMessageResult == 0)
              {
                  Byte RecieveMessagePowerUnit = MyReadBuffers[23];//电量单位
                  string PowerUnitStr = RecieveMessagePowerUnit.ToString();

                  string PowerCountStr;
                  if (MyAsynchLockServerSocketService.DataFormateFlag)
                  {
                      PowerCountStr = string.Format("{0:X2}", MyReadBuffers[25]) + string.Format("{0:X2}", MyReadBuffers[24]);
                  }
                  else
                  {
                      PowerCountStr = string.Format("{0:X2}", MyReadBuffers[24]) + string.Format("{0:X2}", MyReadBuffers[25]);
                  }

                  UInt32 nPowerCount = Convert.ToUInt32(PowerCountStr, 16);
                  string nPowerCountStr = nPowerCount.ToString();
                  string PowerWarnStr;
                  if (MyReadBuffers[26] == 0)
                  {
                      PowerWarnStr = "0";//正常
                  }
                  else
                  {
                      PowerWarnStr = "1";//告警
                  }

                  PowerInforStr="["+nPowerCountStr+"$"+PowerUnitStr+","+PowerWarnStr+"]";
                 
              }
              else
              {

                   PowerInforStr = "";

              }
              string CommandMessageStr;
              CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + PowerInforStr + "!";
             byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(CommandMessageStr);

             int nBuffersLenght = CommandMessageStr.Length;
             byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];
                            

             MySendMessageBytes[2] = 255;

             //填充
             for (int i = 0; i < nBuffersLenght; i++)
             {

                 MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

             }

             //----1.智能锁响应消息保存数据库备查-------------------------------------------------------------------------------------------------------------
             //  MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);


             //--2.找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]-----------------------------------
              
             //--找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]--------------------------------------------------------------------------------
             //FindMobileChannel MyBindedMobileChannel = new FindMobileChannel(MessageMobileStr);
             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedMobileChannel.BindedMobileChannel)).MyReadWriteSocketChannel;
             //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;        
              //--再转发出去-----------
             //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
             try
             {
                 
                 this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

                /*
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
                 */

             }
             catch
             {
                 
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
             }
           
         }
      
         private void DriveToSmartLock()
         {

             //------校验字节------------------------------------------------
             int MySendByteCount = 23;//加校验字节

             byte[] MySendMessageBytes = new byte[MySendByteCount];

             string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

             string HexSendMessageIDString;
             if (MyAsynchLockServerSocketService.DataFormateFlag)
             {
                 HexSendMessageIDString = "0330";
             }
             else
             {
                 HexSendMessageIDString = "3003";
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

         private void NotFindLockResponseToMobile()
         {
             string CommandMessageID = "getpower";
             string ResultStr = "false[11]"; ;
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
             MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);

             try
             {
                 //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.MyLoginUserList[0].MyReadWriteSocketChannel;
                 //MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);
                 MyAsynchLockServerSocketService.StartAsynchSendMessage(this.MyReadWriteChannel, MySendMessageBytes);

             }
             catch
             {
                 //通道错误；
                 MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误！");
             }

         }

    }
}
