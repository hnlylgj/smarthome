using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.IO; 
namespace LGJAsynchSocketService
{
         
         
    public class SendMailProc
    {
        NetworkCredential MyNetworkCredential;
        string MySendFromStr;
        string EMailToStr;
        SmtpClient MySmtpClient;
        MailAddress MyFromMailAddress;
        MailAddress MyToMailAddress;
        MailMessage MyMessageContent;
        Attachment MyAttachment;

        //string AttachFileName;
        //FileStream MySendFileStream;
        //MemoryStream MyMemoryStreamContent;
        
        public bool SendFlagID;

        /*
        public SendMailProc(string EMailStr, string SnapFileName)
        {
            //不发附件，全部正文
            EMailToStr = EMailStr;

                //MySendFromStr = "hnlylgj@qq.com";
            //MySmtpClient = new SmtpClient("smtp.qq.com");
            MySendFromStr = "hnlylgj@163.com";
            MySmtpClient = new SmtpClient("smtp.163.com");
            
            MyFromMailAddress = new MailAddress(MySendFromStr, "云智能锁", Encoding.UTF8);
            //构造一个收件人地址对象
            MyToMailAddress = new MailAddress(EMailToStr, "八面玲珑", Encoding.UTF8);
            //构造一个Email的Message对象
            MyMessageContent = new MailMessage(MyFromMailAddress, MyToMailAddress);

            //AttachFileName = SnapFileName;
            //MyAttachment = new Attachment(AttachFileName);
            //MyMessageContent.Attachments.Add(MyAttachment);

            //添加邮件主题和内容
            MyMessageContent.Subject = "云智能锁通知";// txtSubject.Text;
            MyMessageContent.SubjectEncoding = Encoding.UTF8;
            MyMessageContent.Body = "Dear, Hello！Sanp a Picture!\r\n" + SnapFileName + "\r\n-★★云智能锁★★-";
            MyMessageContent.BodyEncoding = Encoding.UTF8;

            //设置邮件的信息
            MySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MySmtpClient.EnableSsl = false;
            MySmtpClient.UseDefaultCredentials = false;

            MyMessageContent.BodyEncoding = System.Text.Encoding.UTF8;
            MyMessageContent.IsBodyHtml = false;
                     

            //设置用户名和密码。
            string username = "hnlylgj";
            string passwd = "531202lgjx";
            MyNetworkCredential = new NetworkCredential(username, passwd);
            MySmtpClient.Credentials = MyNetworkCredential;


        }
        */
        public SendMailProc(string EMailStr, Stream SnapImageStream, string SnapFileName)
        {
            //发附件与正文
            EMailToStr = EMailStr;

             //MySendFromStr = "hnlylgj@qq.com";
            //MySmtpClient = new SmtpClient("smtp.qq.com");

            MySendFromStr = "hnlylgj@163.com";
            MySmtpClient = new SmtpClient("smtp.163.com");

            MyFromMailAddress = new MailAddress(MySendFromStr, "云智能锁", Encoding.UTF8);
            //构造一个收件人地址对象
            MyToMailAddress = new MailAddress(EMailToStr, "八面玲珑", Encoding.UTF8);
            //构造一个Email的Message对象
            MyMessageContent = new MailMessage(MyFromMailAddress, MyToMailAddress);

             //AttachFileName = "C:\\Users\\zhanggong\\Pictures\\Forclound2.jpg";
             //MySendFileStream = File.Open(AttachFileName, FileMode.Open);

            //ContentType MyContentType = new ContentType();
            //MyAttachment = new Attachment(MySendFileStream, "LGJ201408051425.jpg", "image/jpeg");

             MyAttachment = new Attachment(SnapImageStream, SnapFileName, "image/jpeg");
             MyMessageContent.Attachments.Add(MyAttachment);

            //添加邮件主题和内容
             MyMessageContent.Subject = "云智能锁通知";
            MyMessageContent.SubjectEncoding = Encoding.UTF8;
            MyMessageContent.Body = "Dear, Hello！Sanp a Picture!\r\n" + "★★云智能锁★★";
            MyMessageContent.BodyEncoding = Encoding.UTF8;

            //设置邮件的信息
            MySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MySmtpClient.EnableSsl = false;
            MySmtpClient.UseDefaultCredentials = false;

            MyMessageContent.BodyEncoding = System.Text.Encoding.UTF8;
            MyMessageContent.IsBodyHtml = false;


            //设置用户名和密码。
            string username = "hnlylgj";
            string passwd = "531202lgjx";
            MyNetworkCredential = new NetworkCredential(username, passwd);
            MySmtpClient.Credentials = MyNetworkCredential;


        }

        public SendMailProc(string EMailStr, string SnapID, string LockID)
        {
            //不发附件，全部正文

            EMailToStr = EMailStr;
            
            
            //MySendFromStr = "hnlylgj@qq.com";
            //MySmtpClient = new SmtpClient("smtp.qq.com");

            MySendFromStr = "hnlylgj@163.com";
            MySmtpClient = new SmtpClient("smtp.163.com");
            MySmtpClient.Timeout = 5000;

            MyFromMailAddress = new MailAddress(MySendFromStr, "云智能锁", Encoding.UTF8);
            //构造一个收件人地址对象
            MyToMailAddress = new MailAddress(EMailToStr, "八面玲珑", Encoding.UTF8);
            //构造一个Email的Message对象
            MyMessageContent = new MailMessage(MyFromMailAddress, MyToMailAddress);
            
            
            //AttachFileName = SnapFileName;
            //MyAttachment = new Attachment(AttachFileName);
            //MyMessageContent.Attachments.Add(MyAttachment);
            //string SnapID;            
            //string LockID;

            string HttpUrlStr = "http://121.42.45.167/RemoteSnapImage.aspx?SnapID=" + SnapID + "&LockID=" + LockID;

            //添加邮件主题和内容
            MyMessageContent.Subject = "动态消息";// txtSubject.Text;
            MyMessageContent.SubjectEncoding = Encoding.UTF8;
            MyMessageContent.Body = "" + HttpUrlStr + "\r\n";
            MyMessageContent.BodyEncoding = Encoding.UTF8;

            //设置邮件的信息
            MySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MySmtpClient.EnableSsl = false;
            MySmtpClient.UseDefaultCredentials = false;

            MyMessageContent.BodyEncoding = System.Text.Encoding.UTF8;
            MyMessageContent.IsBodyHtml = false;


            //设置用户名和密码。
            string username = "hnlylgj";
            string passwd = "531202lgjx";
            MyNetworkCredential = new NetworkCredential(username, passwd);
            MySmtpClient.Credentials = MyNetworkCredential;


        }

        public SendMailProc(string SnapID, string LockID)
        {
            //不发附件，全部正文-》EverNote笔记提醒功能
            EMailToStr = "hnlylgj.5f72bdc@m.evernote.com";//hnlylgj.5f72bdc@m.evernote.com

            //构造一个发件人地址对象
            MySendFromStr = "hnlylgj@163.com";
            MySmtpClient = new SmtpClient("smtp.163.com");
            MyFromMailAddress = new MailAddress(MySendFromStr, "云锁", Encoding.UTF8);

            //构造一个收件人地址对象
            MyToMailAddress = new MailAddress(EMailToStr, "八面玲珑", Encoding.UTF8);

            //构造一个Email的Message对象
            MyMessageContent = new MailMessage(MyFromMailAddress, MyToMailAddress);


            string HttpUrlStr = "http://localhost:8080/RemoteSnapImage.aspx?SnapID=" + SnapID + "&LockID=" + LockID;

            //添加邮件主题和内容
            MyMessageContent.Subject = "云锁抓拍!@智能家居#家居";
            MyMessageContent.SubjectEncoding = Encoding.UTF8;
            MyMessageContent.Body = "抓拍动态:" + HttpUrlStr;//+ "\r\n"
            MyMessageContent.BodyEncoding = Encoding.UTF8;

            //设置邮件的信息
            MySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MySmtpClient.EnableSsl = false;
            MySmtpClient.UseDefaultCredentials = false;

            MyMessageContent.BodyEncoding = System.Text.Encoding.UTF8;
            MyMessageContent.IsBodyHtml = false;


            //设置用户名和密码。
            string username = "hnlylgj";
            string passwd = "531202lgjx";
            MyNetworkCredential = new NetworkCredential(username, passwd);
            MySmtpClient.Credentials = MyNetworkCredential;


        }

        public void StartSendMail()
        {    
            
            try
            {
                //发送邮件
               
                MySmtpClient.Send(MyMessageContent);
                SendFlagID = true;
            }
            catch (Exception ex)
            {
                SendFlagID = false;
            }
            //if(MySendFileStream!=null) 
            // MySendFileStream.Close(); 
        }




    }
}
