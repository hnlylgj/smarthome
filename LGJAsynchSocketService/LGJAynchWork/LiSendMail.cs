using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.LGJAynchWork
{
     public class LiAsyncSendMail
    {
        private delegate void AsynchSendMailHandler();
        private delegate bool AsynchSendMailHandlerEx();
        private AsynchSendMailHandler AsynchSendMailCallBak;
        private AsynchSendMailHandlerEx AsynchSendMailCallBakEx;
        //private IAsyncResult MyIAsyncResult;
        private SendMailProc MySendMailProc;

        public LiAsyncSendMail(SendMailProc MeSendMailProc)
        {
            this.MySendMailProc = MeSendMailProc;
            AsynchSendMailCallBak = new AsynchSendMailHandler(this.SendEMailNotify);
            AsynchSendMailCallBakEx = new AsynchSendMailHandlerEx(this.SendEMailNotifyEx);
            
        }
        public void SendEMailNotify()
        {
            MySendMailProc.StartSendMail();//真正的干活！
        }

        public bool SendEMailNotifyEx()
        {
            MySendMailProc.StartSendMail();//真正的干活！
            return MySendMailProc.SendFlagID;
        }
        public IAsyncResult BeginSendEMailNotify(AsyncCallback MyCallBack, Object MystateObject)        {

            //return AsynchSendMailCallBak.BeginInvoke(MyCallBack, MystateObject);
            return AsynchSendMailCallBakEx.BeginInvoke(MyCallBack, MystateObject);

        }
        public bool EndSendEMailNotify(IAsyncResult InAsyncResult)
        {

            return AsynchSendMailCallBakEx.EndInvoke(InAsyncResult);  
        }

    }
}
