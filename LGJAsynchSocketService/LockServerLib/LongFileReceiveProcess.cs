using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace LGJAsynchSocketService.LockServerLib
{
    class LongFileReceiveProcess
    {
        [ThreadStatic()]
      public static uint DebugCount;
        [ThreadStatic()]
      public static uint LoopReadCount;
       [ThreadStatic()] 
      public static uint LoopReadFlag;
       [ThreadStatic()]
       public static uint NextSaveIndex;
       [ThreadStatic()]
      public static string MyLockIDStr;
       [ThreadStatic()] 
      public static byte[] StaticRecieveFileBytes ;

      static LongFileReceiveProcess()
       {
            LoopReadFlag = 0;
            LoopReadCount = 0;
            NextSaveIndex = 0;
            
        }

       public LongFileReceiveProcess(uint InitCount, byte[] RecieveFileByteUnit)
        {
            StaticRecieveFileBytes = new byte[InitCount];
            //-------------------------------------------------------------------------
            for (int i = 0; i < RecieveFileByteUnit.Length; i++)
            {
                StaticRecieveFileBytes [NextSaveIndex] = RecieveFileByteUnit[i];

            }
            NextSaveIndex = NextSaveIndex + (uint)RecieveFileByteUnit.Length;

 
        }

       public LongFileReceiveProcess(AsynchSocketServiceBaseFrame InAsynchSocketServiceBaseFrame, SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] RecieveFileByteUnit, uint MyRecieveCount)
       {
           if (LoopReadCount == 0)
           {
               string RecieveMessageLenghtStr = string.Format("{0:X2}", RecieveFileByteUnit[21]);
               RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[20]);
               RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[19]);
               RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", RecieveFileByteUnit[18]);

               UInt32 RecieveMessageLenght = Convert.ToUInt32(RecieveMessageLenghtStr, 16);
               UInt32 RecieveFileLenght = RecieveMessageLenght - 22;

               LoopReadCount++;

               StaticRecieveFileBytes = new byte[RecieveFileLenght];
               //byte[] RecieveFileFirstByteBuffer = new byte[MyRecieveCount-22];

               LoopReadFlag = (uint)(RecieveMessageLenght - MyRecieveCount);
               NextSaveIndex = MyRecieveCount - 22;

               //--填充所接收的文件字节----
               for (int i = 0; i < NextSaveIndex; i++)
               {

                   StaticRecieveFileBytes[0] = RecieveFileByteUnit[22 + i];

               }

               //AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);

               //---1.找智能锁本身通道的路由表记录-------------------------------------
               LockServerLib.FindLockChannel MyBindedLockChannel = new LockServerLib.FindLockChannel(InputSocketServiceReadWriteChannel);
               LoginUser MyLoginUser = InAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
               MyLockIDStr = MyLoginUser.LockID;
           }
           else
           {
               this.AddLongFilePackage(RecieveFileByteUnit,MyRecieveCount);
           }
           //MyMobileIDStr = MyLoginUser.MobileID; 

           //AddLongFilePackage(RecieveFileByteUnit, (int)RecieveFileLenght)

           /*
          LoopReadFlag = RecieveMessageLenght;

          if (RecieveMessageLenght <= MyRecieveCount)
          {

              StaticRecieveFileBytes = new byte[RecieveFileLenght];
              AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);
              StartUpLoadFileSave();

          }
          else
          {
              LoopReadCount++;
              StaticRecieveFileBytes = new byte[RecieveFileLenght];
              AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);

          }
          */



       }

       public static void StartLongFileReceive(AsynchSocketServiceBaseFrame InAsynchSocketServiceBaseFrame,SocketServiceReadWriteChannel InputSocketServiceReadWriteChannel, byte[] FirstRecieveFileByteUnit, uint MyRecieveCount)
       {
           string RecieveMessageLenghtStr = string.Format("{0:X2}", FirstRecieveFileByteUnit[21]);
           RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", FirstRecieveFileByteUnit[20]);
           RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", FirstRecieveFileByteUnit[19]);
           RecieveMessageLenghtStr = RecieveMessageLenghtStr + string.Format("{0:X2}", FirstRecieveFileByteUnit[18]);

           UInt32 RecieveMessageLenght = Convert.ToUInt32(RecieveMessageLenghtStr, 16);
           UInt32 RecieveFileLenght = RecieveMessageLenght - 22;
          
           LoopReadCount++;

           StaticRecieveFileBytes = new byte[RecieveFileLenght];
           //byte[] RecieveFileFirstByteBuffer = new byte[MyRecieveCount-22];

           LoopReadFlag =(uint) (RecieveMessageLenght - MyRecieveCount);
           NextSaveIndex = MyRecieveCount - 22;

           //--填充所接收的文件字节----
           for (int i = 0; i < NextSaveIndex; i++)
           {

               StaticRecieveFileBytes[0] = FirstRecieveFileByteUnit[22 + i];

           }

           //---1.找智能锁本身通道的路由表记录-------------------------------------
           LockServerLib.FindLockChannel MyBindedLockChannel = new LockServerLib.FindLockChannel(InputSocketServiceReadWriteChannel);
           LoginUser MyLoginUser = InAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList.Find(new Predicate<LoginUser>(MyBindedLockChannel.BindedLockChannelForSocket));
           MyLockIDStr = MyLoginUser.LockID;

              //MyMobileIDStr = MyLoginUser.MobileID; 

           //AddLongFilePackage(RecieveFileByteUnit, (int)RecieveFileLenght)
          
            /*
           LoopReadFlag = RecieveMessageLenght;

           if (RecieveMessageLenght <= MyRecieveCount)
           {

               StaticRecieveFileBytes = new byte[RecieveFileLenght];
               AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);
               StartUpLoadFileSave();

           }
           else
           {
               LoopReadCount++;
               StaticRecieveFileBytes = new byte[RecieveFileLenght];
               AddLongFilePackage(RecieveFileByteUnit, MyRecieveCount);

           }
           */



       }
     
       public  void AddLongFilePackage(byte[] RecieveFileByteUnit,uint MyRecieveCount)
        {

           LoopReadCount++;
           for (uint i = 0; i < MyRecieveCount; i++)
            {
                StaticRecieveFileBytes [NextSaveIndex+i] = RecieveFileByteUnit[i];

            }
            NextSaveIndex = NextSaveIndex + (uint)MyRecieveCount;

            LoopReadFlag = (uint)(LoopReadFlag - MyRecieveCount);

            if (LoopReadFlag == 0)
            {
                StartLongFileSave();
            }

            /*
            if (LoopReadFlag <= NextSaveIndex)
              {
                StartUpLoadFileSave();

             }
           */
       

        }
       
       public int StartLongFileSave()
         {
              /*
             string MySaveFileName;
             FileStream MyRecieveFileStream;
             BinaryWriter MyBinaryWriter;
             //int StartIndex;
             //MySaveFileName = Encoding.UTF8.GetString(StaticRecieveFileBytes , 6, (int)StaticRecieveFileBytes [5]);

             MyRecieveFileStream = new FileStream(MySaveFileName, FileMode.Create, FileAccess.Write);
             MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);
             StartIndex =6 + (int)StaticRecieveFileBytes [5];
             MyBinaryWriter.Write(StaticRecieveFileBytes , StartIndex, StaticRecieveFileBytes .Length - StartIndex);


             MyBinaryWriter.Close();
             MyRecieveFileStream.Close();
             UpLoadFileSaveEnd();
           */

           //========================
             string MyFileNameStr = Guid.NewGuid().ToString().ToUpper();
             //int StartIndex; 
             FileStream MyRecieveFileStream;
             BinaryWriter MyBinaryWriter;
             string MyCompleteFileName = "F:\\LockSnapImage\\" + MyLockIDStr + "\\" + MyFileNameStr + ".jpg";
             MyRecieveFileStream = new FileStream(MyCompleteFileName, FileMode.Create, FileAccess.Write);
             MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);
            
             MyBinaryWriter.Write(StaticRecieveFileBytes);
             MyBinaryWriter.Close();
             MyRecieveFileStream.Close();

             CloseLongFileSave();
             return 0;
         }
     
        public void CloseLongFileSave()
       {
           LoopReadCount = 0;
           LoopReadFlag = 0;
           NextSaveIndex = 0;
           MyLockIDStr = null;
           StaticRecieveFileBytes = null;

       }

        //--------------------------------------------------------------------------------
       public static int StartUpLoadFileSave(byte[] RecieveFileByteUnit)
         {

             int StartIndex;
             string MyFileName;
             FileStream MyRecieveFileStream;
             BinaryWriter MyBinaryWriter;
             MyFileName = Encoding.UTF8.GetString(RecieveFileByteUnit, 6, (int)RecieveFileByteUnit[5]);
             MyRecieveFileStream = new FileStream(MyFileName, FileMode.Create, FileAccess.Write);
             MyBinaryWriter = new BinaryWriter(MyRecieveFileStream);
             StartIndex = 6 + (int)RecieveFileByteUnit[5];
             MyBinaryWriter.Write(RecieveFileByteUnit, StartIndex, RecieveFileByteUnit.Length - StartIndex);
             MyBinaryWriter.Close();
             MyRecieveFileStream.Close();
             //CloseLongFileSave();
             return 0;
         }
      
      
    }
}
