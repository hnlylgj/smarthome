using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.LGJAynchWork
{
    internal class AsynchWorkProcess
    {
        public void AsyncStartSendEMail(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
        {            
            
            string MyAutoFileNameStr = MeLoginUser.TempString;// Guid.NewGuid().ToString().ToUpper();
            string MyCompleteFileName = "C:\\LockSnapImage\\" + MeLoginUser.LockID + "_" + MyAutoFileNameStr + ".jpg";
            long SnapID = MeLoginUser.SnapID;
            string EMailToStr = "hnlylgjx@qq.com"; //--"hnlylgj.5f72bdc@m.evernote.com";//专发笔记提醒
            SendMailProc MySendMailProc = new SendMailProc(EMailToStr, SnapID.ToString(),MeLoginUser.LockID);
            LiAsyncSendMail MyLiAsyncSendMail = new LiAsyncSendMail(MySendMailProc);
            RequestAttatchment MyRequestAttatchment = new RequestAttatchment(MeLoginUser, MeAsynchLockServerSocketService, MyLiAsyncSendMail);

            AsyncCallback MyAsyncCallback = new AsyncCallback(this.SendEMailResult);
            MyLiAsyncSendMail.BeginSendEMailNotify(MyAsyncCallback, MyRequestAttatchment);

        }
        private void SendEMailResult(IAsyncResult InAsyncResul)
        {
            RequestAttatchment MyRequestAttatchment = ( RequestAttatchment)InAsyncResul.AsyncState;
             try
             {
                 bool ResultID=MyRequestAttatchment.MyAsyncLiSendMail.EndSendEMailNotify(InAsyncResul);
                 if(ResultID)
                 {
                     string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                     MyRequestAttatchment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]异步发送邮件通知成功！", MyRequestAttatchment.MyLoginUser.LockID, MyRequestAttatchment.MyLoginUser.SnapID));
                     //------下一步操作----------------------                 
                     //MyRequestAttatchment.MyLoginUser.ClearSet();//清理工作

                 }
                 else
                 {
                     string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                     MyRequestAttatchment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]异步发送邮件通知失败1！", MyRequestAttatchment.MyLoginUser.LockID, MyRequestAttatchment.MyLoginUser.SnapID));
                

                 }
               
             
             }
             catch
             {
                 string MyTimeMarker = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                 MyRequestAttatchment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format(MyTimeMarker + "客户[{0}][{1}]异步发送邮件通知失败2！", MyRequestAttatchment.MyLoginUser.LockID, MyRequestAttatchment.MyLoginUser.SnapID));
                
             }
             finally
             {
                 MyRequestAttatchment.MyLoginUser.ClearSet();//清理工作
             }
            
        }





    }
}
