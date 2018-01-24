using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsyncSocketClientLib
{
    public class MobileServerClientAPI : LGJSocketClientAPIBase
    {

        public MobileServerClientAPI() : base()
        {


        }
        public MobileServerClientAPI(string IPStr, int PortID)
            : base(IPStr, PortID)
        {


        }

        //=====================================================================
        public void SendMessageString(string SendMessageStr)
        {
            try
            {
                SendIDFlag = 1;
                int nBuffersLenght;
                string MySendMessageString = SendMessageStr;
                byte[] MySendBaseMessageBytes = Encoding.UTF8.GetBytes(MySendMessageString);
                nBuffersLenght = MySendBaseMessageBytes.Length;// MySendMessageString.Length;
                byte[] MySendMessageBytes = new byte[nBuffersLenght + 3];

                //填充信息头
                MySendMessageBytes[2] = 255;

                //填充
                for (int i = 0; i < nBuffersLenght; i++)
                {

                    MySendMessageBytes[3 + i] = MySendBaseMessageBytes[i];

                }

                SynchSendCommand(MySendMessageBytes);


            }
            catch (Exception err)
            {
                SendIDFlag = 2;
            }
        }
                
        protected override void SendMessageForString(Byte[] InSendMessageBuffers)
        {
            //string MyTimeMarkerStr = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            //string MyTimeMarkerStr = string.Format("{0:HH:mm:ss}", DateTime.Now);// + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string MyTimeMarker = string.Format("{0:HH:mm:ss}", DateTime.Now) + ":" + string.Format("{0:D3}", DateTime.Now.Millisecond);
            string ALLRequstMessageString = Encoding.UTF8.GetString(InSendMessageBuffers, 3, InSendMessageBuffers.Length - 3);
            ReturnSendMessageInvoke(MyTimeMarker + "[" + MyTcpClient.Client.LocalEndPoint.ToString() + "->]" + ALLRequstMessageString);



        }

        //===============================================================



    }
}
