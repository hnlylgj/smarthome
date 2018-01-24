using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBusServiceLib;

namespace LGJAsynchSocketService.LockServerLib
{
     public class GetOpenDoor
    {

        LoginUser MyLoginUser;
        AsynchLockServerSocketService MyAsynchLockServerSocketService;
        SocketServiceReadWriteChannel MyReadWriteChannel;
        SocketServiceReadWriteChannel NewReadWriteChannel;
        int MyRecieveCount;
        byte[] MyReadBuffers;

        string MyLockIDStr;
        string MyMobileIDStr;
        int OpenDoorCount;
        string OpenDoorListStr;


        public GetOpenDoor(AsynchLockServerSocketService InAsynchLockServerSocketService, LoginUser MeLoginUser, int MeRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
            this.MyLoginUser = MeLoginUser;
            this.MyReadWriteChannel = MeLoginUser.MyReadWriteSocketChannel;
            this.MyReadBuffers = this.MyReadWriteChannel.MyReadBuffers;
            this.MyRecieveCount = MeRecieveCount;
        }
        public GetOpenDoor(AsynchLockServerSocketService InAsynchLockServerSocketService, SocketServiceReadWriteChannel InputReadWriteChannel, int InRecieveCount)
        {
            this.MyAsynchLockServerSocketService = InAsynchLockServerSocketService;
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
          
            //--找智能锁通道------------------------------------------------------------------------------------------------------------------------------
            //this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.CRUDLoginUserListForMobileFindEx(MyLockIDStr, MyMobileIDStr);

            FindLockChannel MyBindedLockChannel = new FindLockChannel(MyLockIDStr); //测试正确
            MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForLockID));
                      
            
            //--再转发出去，驱动智能锁---------------------------------------------------------------------------------------------------------------------
            if (MyLoginUser != null)
            {
                this.NewReadWriteChannel = MyLoginUser.MyReadWriteSocketChannel;
                DriveToSmartLock();
            }
            else
            {
                //--否则原路或者走失败响应通道快速返回[0、1]
                FailResponseToMobile("[11]");
            }

           
        }

        public void ResponseToMobile()
        {
            string CommandMessageID = "getopen";
            //---1.找智能锁本身通道的路由表记录------------------------------------------------------------

            //LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerSocketLoginUser.FindLoginUserList(ref  this.MyReadWriteChannel.MyTCPClient);  
            //-------------------------------------
            //FindLockChannel MyBindedLockChannel = new FindLockChannel(this.MyReadWriteChannel);
            //LoginUser MyLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));


            //--1.数据解析保存到数据库----------------------------------------------------------------------
            MyLockIDStr = MyLoginUser.LockID;
            MyMobileIDStr = MyLoginUser.MobileID;

            Byte RecieveMessageResult = MyReadBuffers[22];//结果标志

            string ResultStr;
            if (RecieveMessageResult == 0)
            {
                ResultStr = "true";
            }
            else
            {
                ResultStr = "false";
                FailResponseToMobile("[10]");
                return;
            }

            OpenDoorCount = MyReadBuffers[23];
            if (OpenDoorCount == 0)
            {
                FailResponseToMobile("[10]");
                return;
            }
            SaveToDBase();
           
          //--2.消息推送到移动端-----------------------------------------------------------------

            string CommandMessageStr;
            CommandMessageStr = CommandMessageID + "#" + MyMobileIDStr + "-" + MyLockIDStr + "#" + ResultStr + OpenDoorListStr + "!";
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
            ///MyAsynchLockServerSocketService.SamrtLockResponseToSave(MyLockIDStr, CommandMessageID, CommandMessageStr);
            //----2.找移动端通道，如果是原型-2和正式版本需固定两个移动端请求消息通道和响应通道[0通道和1号通道]-----------------------------------

            /*
            try
            {
                this.NewReadWriteChannel = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList[0].MyReadWriteSocketChannel;

                MyAsynchLockServerSocketService.StartAsynchSendMessage(NewReadWriteChannel, MySendMessageBytes);

            }
            catch
            {

                MyAsynchLockServerSocketService.DisplayResultInfor(1, "转发移动端通道错误");
            }
             * */

            try
            {

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

        private void DriveToSmartLock()
        {

            //------校验字节------------------------------------------------
            int MySendByteCount = 23;//加校验字节

            byte[] MySendMessageBytes = new byte[MySendByteCount];

            string HexSendLenghtString = string.Format("{0:X4}", MySendByteCount);

            string HexSendMessageIDString;
            if (MyAsynchLockServerSocketService.DataFormateFlag)
            {
                HexSendMessageIDString = "0130";
            }
            else
            {
                HexSendMessageIDString = "3001";
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

        private void SaveToDBase()
        {
            string CreateDateStr;
            int KeyID;
            int Year;
            int Month ;
            int Date;
            int Hour;
            int Miminute;
            int Second;
            List<OpenDoor> MyAllOpenDoor = new List<OpenDoor>();
            OpenDoorListStr = "[" + OpenDoorCount.ToString()+":" ;
            for (int i = 0; i < OpenDoorCount; i++)
            {
                int Index = 24 + 17 * i;
                KeyID = this.MyReadBuffers[Index];
                CreateDateStr = null;
                CreateDateStr = Encoding.ASCII.GetString(this.MyReadBuffers, Index + 2, 14);
                Year = int.Parse(CreateDateStr.Substring(0, 4));
                Month = int.Parse(CreateDateStr.Substring(4, 2));
                Date = int.Parse(CreateDateStr.Substring(6, 2));
                Hour = int.Parse(CreateDateStr.Substring(8, 2));
                Miminute = int.Parse(CreateDateStr.Substring(10, 2));
                Second = int.Parse(CreateDateStr.Substring(12, 2)); 
               
                CreateDateStr = CreateDateStr.Insert(4, "-");
                CreateDateStr = CreateDateStr.Insert(7, "-");
                CreateDateStr = CreateDateStr.Insert(10, " ");
                CreateDateStr = CreateDateStr.Insert(13, ":");
                CreateDateStr = CreateDateStr.Insert(16, ":");
               
                OpenDoor MyOpenDoor = new OpenDoor();
                MyOpenDoor.LockID = MyLockIDStr;
                MyOpenDoor.KeyID = KeyID;
                MyOpenDoor.OpenDate = new DateTime(Year, Month, Date, Hour, Miminute, Second);  
                MyOpenDoor.OpenDateStr = CreateDateStr;

                MyAllOpenDoor.Add(MyOpenDoor);

                OpenDoorListStr = OpenDoorListStr + KeyID.ToString() + "," + CreateDateStr+";";

               

                

            }
            int LastIndex = OpenDoorListStr.LastIndexOf(";");
            OpenDoorListStr=OpenDoorListStr.Remove(LastIndex, 1);  
            OpenDoorListStr = OpenDoorListStr + "]"; 
                    
                 //OpenDoorManager MyOpenDoorManager = new OpenDoorManager();
            //MyOpenDoorManager.InsertOpenDoorEx(MyAllOpenDoor);

            OpenDoorManager MyOpenDoorManager = new OpenDoorManager();
            MyOpenDoorManager.InsertOpenDoorEx(MyAllOpenDoor);

          

        }

        private void FailResponseToMobile(string ExtStr)
        {
            string CommandMessageID = "getopen";
            string ResultStr = "false" + ExtStr ;
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
