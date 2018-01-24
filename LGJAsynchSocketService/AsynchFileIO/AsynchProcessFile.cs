using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGJAsynchSocketService.AsynchSQLServerIO;
namespace LGJAsynchSocketService.AsynchFileIO
{
    public class AsynchProcessFile
    {
         public void SnapImageSaveFile(LoginUser MeLoginUser, AsynchLockServerSocketService MeAsynchLockServerSocketService)
        {
            string MyAutoFileNameStr = MeLoginUser.TempString;// Guid.NewGuid().ToString().ToUpper();
            string MyCompleteFileName = "C:\\LockSnapImage\\" + MeLoginUser.LockID + "_" + MyAutoFileNameStr + ".jpg";
            RequestAttachment MyRequestAttachment = new RequestAttachment(MeLoginUser, MeAsynchLockServerSocketService, MyCompleteFileName, 0);
            AsyncCallback MyAsyncCallback=new AsyncCallback(this.SnapImageSaveResult);
          
           MyRequestAttachment.MyFileStream.BeginWrite(MeLoginUser.MyByteBuffer, 0, (int)MeLoginUser.WorkStatus, MyAsyncCallback, MyRequestAttachment);
           MyRequestAttachment.MyFileStream.Flush();
     
         
         }
         private void SnapImageSaveResult(IAsyncResult InAsyncResul)
         {
            
             RequestAttachment MyRequestAttachment = (RequestAttachment)InAsyncResul.AsyncState;
             try
             {
                 MyRequestAttachment.MyFileStream.EndWrite(InAsyncResul);
                 MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, string.Format("异步保存图像数据到文件[{0}]成功！", MyRequestAttachment.MyLoginUser.LockID+"_"+MyRequestAttachment.MyLoginUser.TempString));

                 //------下一步操作----------------------
                 LGJAsynchAccessDBase MyLGJAsynchAccessDBase = new LGJAsynchAccessDBase();
                 MyLGJAsynchAccessDBase.AsynchSaveSnapPara(MyRequestAttachment.MyLoginUser, MyRequestAttachment.MyAsynchLockServerSocketService);//异步操作数据库

             }
             catch
             {
                 MyRequestAttachment.MyAsynchLockServerSocketService.DisplayResultInfor(1, "异步保存图像数据到Files失败！");
             }
             finally
             {
                 MyRequestAttachment.MyFileStream.Close();
             }
            

         }
    }
}
