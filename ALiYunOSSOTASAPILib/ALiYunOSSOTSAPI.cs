using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Aliyun.OpenServices.OpenStorageService;

namespace ALiYunOSSOTSAPILib
{
    public class ALiYunOSSOTSAPI
    {
        public ALiYunOSSOTSLogin MyALiYunOSSOTSLogin;
        public string ResultMessageStr;

        public ALiYunOSSOTSAPI()
        {
            MyALiYunOSSOTSLogin = new ALiYunOSSOTSLogin();
            ResultMessageStr = "10XXX";
        }
       
        public ALiYunOSSOTSAPI(ALiYunOSSOTSLogin InALiYunOSSOTSLogin)
        {
            MyALiYunOSSOTSLogin = InALiYunOSSOTSLogin;
        }

        public void PublicDownloadFile(string URL, string LocalFileName, ProgressBar UIViewProg)
        {
            try
            {
                HttpWebRequest MyHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                HttpWebResponse MyHttpWebResponse = (HttpWebResponse)MyHttpWebRequest.GetResponse();
                long MyReceiveBytesCount = MyHttpWebResponse.ContentLength;
                if (UIViewProg != null)
                {
                    UIViewProg.Maximum = (int)MyReceiveBytesCount;
                }
                Stream MyWebResponseStream = MyHttpWebResponse.GetResponseStream();
                Stream MySaveFileStream = new FileStream(LocalFileName, FileMode.Create);

                long MyDownloadedByteLenght = 0;
                byte[] MyOneReadBuffer = new byte[1024];
                int MyOneReadSize = MyWebResponseStream.Read(MyOneReadBuffer, 0, (int)MyOneReadBuffer.Length);
                while (MyOneReadSize > 0)
                {
                    MyDownloadedByteLenght = MyOneReadSize + MyDownloadedByteLenght;
                    Application.DoEvents();
                    MySaveFileStream.Write(MyOneReadBuffer, 0, MyOneReadSize);
                    if (UIViewProg != null)
                    {
                        UIViewProg.Value = (int)MyDownloadedByteLenght;
                    }
                    MyOneReadSize = MyWebResponseStream.Read(MyOneReadBuffer, 0, (int)MyOneReadBuffer.Length);
                }
                MySaveFileStream.Close();
                MyWebResponseStream.Close();

                ResultMessageStr = "文件公读下载成功！";// throw;
            }
            catch (System.Exception InforExcep)
            {
                ResultMessageStr = InforExcep.Message ; //throw;
            }
        }

        public void PublicDownloadFile2(string URL, ref MemoryStream InMemoryStreamContent, ProgressBar UIViewProg)
        {
            try
            {
                HttpWebRequest MyHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
                HttpWebResponse MyHttpWebResponse = (HttpWebResponse)MyHttpWebRequest.GetResponse();
                long MyReceiveBytesCount = MyHttpWebResponse.ContentLength;
                if (UIViewProg != null)
                {
                    UIViewProg.Maximum = (int)MyReceiveBytesCount;
                }

                Stream MyWebResponseStream = MyHttpWebResponse.GetResponseStream();
                long MyDownloadedByteLenght = 0;
                byte[] MyPreBuffer = new byte[1024];
                int MyReadHeadSize = MyWebResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                while (MyReadHeadSize > 0)
                {
                    MyDownloadedByteLenght = MyReadHeadSize + MyDownloadedByteLenght;
                    Application.DoEvents();
                    InMemoryStreamContent.Write(MyPreBuffer, 0, MyReadHeadSize);
                    if (UIViewProg != null)
                    {
                        UIViewProg.Value = (int)MyDownloadedByteLenght;
                    }
                    MyReadHeadSize = MyWebResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                }

                MyWebResponseStream.Close();
                ResultMessageStr = "文件公读下载成功!";
            }
            catch (Exception InforExcep)
            {
                 ResultMessageStr = InforExcep.Message; //throw;
            }
        }

        public void PrivateDownloadFile(string LocalFileFullName, ProgressBar UIViewProg)
        {
            try
            {
                OssClient MyOSSClient = new OssClient(MyALiYunOSSOTSLogin.MyAccessID, MyALiYunOSSOTSLogin.MyAccessKey);
                OssObject MyOssObject = MyOSSClient.GetObject(MyALiYunOSSOTSLogin.MyBucketName, MyALiYunOSSOTSLogin.PrefixStr + MyALiYunOSSOTSLogin.FileKeyName);

                string MyFileLenghtStr = MyOssObject.Metadata.UserMetadata["FileLength"];
                long MyReceiveBytesCount = long.Parse(MyFileLenghtStr);//MyOssObject.Content.Length;
                if (UIViewProg != null)
                {
                    UIViewProg.Maximum = (int)MyReceiveBytesCount;//220 * 10000;//(int)MyReceiveBytesCount;
                }
                Stream MyResponseStream = MyOssObject.Content;
                Stream MySaveFileStream = new FileStream(LocalFileFullName, FileMode.Create);

                long MyDownloadedByteLenght = 0;
                byte[] MyPreBuffer = new byte[1024];
                int MyReadHeadSize = MyResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                while (MyReadHeadSize > 0)
                {
                    MyDownloadedByteLenght = MyReadHeadSize + MyDownloadedByteLenght;
                    Application.DoEvents();
                    MySaveFileStream.Write(MyPreBuffer, 0, MyReadHeadSize);
                    if (UIViewProg != null)
                    {
                        UIViewProg.Value = (int)MyDownloadedByteLenght;
                    }
                    MyReadHeadSize = MyResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                }

                UIViewProg.Value = UIViewProg.Maximum;
                MySaveFileStream.Close();
                MyResponseStream.Close();
                ResultMessageStr = "文件私读下载成功！";
            }
            catch (Exception InforExcep)
            {
                 ResultMessageStr = "文件私读下载错误：" + InforExcep.Message; //throw;
            }
        }

        public void PrivateDownloadFile2(ref MemoryStream InMemoryStreamContent, ProgressBar UIViewProg)
        {
            try
            {
                OssClient MyOSSClient = new OssClient(MyALiYunOSSOTSLogin.MyAccessID, MyALiYunOSSOTSLogin.MyAccessKey);
                OssObject MyOssObject = MyOSSClient.GetObject(MyALiYunOSSOTSLogin.MyBucketName, MyALiYunOSSOTSLogin.PrefixStr + MyALiYunOSSOTSLogin.FileKeyName);

                string MyFileLenghtStr = MyOssObject.Metadata.UserMetadata["FileLength"];
                long MyReceiveBytesCount = long.Parse(MyFileLenghtStr);// MyOssObject.Content.Length;
                if (UIViewProg != null)
                {
                    UIViewProg.Maximum = (int)MyReceiveBytesCount;//(int)MyReceiveBytesCount;
                }
                Stream MyResponseStream = MyOssObject.Content;

                long MyDownloadedByteLenght = 0;
                byte[] MyPreBuffer = new byte[1024];
                int MyReadHeadSize = MyResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                while (MyReadHeadSize > 0)
                {
                    MyDownloadedByteLenght = MyReadHeadSize + MyDownloadedByteLenght;
                    Application.DoEvents();
                    InMemoryStreamContent.Write(MyPreBuffer, 0, MyReadHeadSize);
                    if (UIViewProg != null)
                    {
                        UIViewProg.Value = (int)MyDownloadedByteLenght;
                    }
                    MyReadHeadSize = MyResponseStream.Read(MyPreBuffer, 0, (int)MyPreBuffer.Length);
                }

                UIViewProg.Value = UIViewProg.Maximum;
                MyResponseStream.Close();
                 ResultMessageStr = "文件私读下载成功！";
            }
            catch (Exception InforExcep)
            {
                ResultMessageStr = "文件私读下载错误：" + InforExcep.Message; //throw;
            }
        }

        public void PrivateUploadFile(string UploadFileFullName, ProgressBar UIViewProg)
        {

            try
            {

                string UploadFileKeyName = MyALiYunOSSOTSLogin.PrefixStr + UploadFileFullName.Substring(UploadFileFullName.LastIndexOf("\\") + 1);
                MyALiYunOSSOTSLogin.FileKeyName = UploadFileFullName.Substring(UploadFileFullName.LastIndexOf("\\") + 1);
                FileStream MySendFileStream = File.Open(UploadFileFullName, FileMode.Open);
                long MySendBytesCount = MySendFileStream.Length;

                ObjectMetadata MyObjectMetadata = new ObjectMetadata();
                // 设定自定义的metadata。
                MyObjectMetadata.UserMetadata.Add("FileLength", MySendBytesCount.ToString());
                MyObjectMetadata.UserMetadata.Add("CreateDate", DateTime.Now.ToString());

                MyObjectMetadata.ContentType = "image/jpeg";

                OssClient MyOSSClient = new OssClient(MyALiYunOSSOTSLogin.MyAccessID, MyALiYunOSSOTSLogin.MyAccessKey);

                MyOSSClient.PutObject(MyALiYunOSSOTSLogin.MyBucketName, UploadFileKeyName, MySendFileStream, MyObjectMetadata);

                MySendFileStream.Close();

                  /*
                using (var MySendFileStream = File.Open(UpLoadFileName, FileMode.Open))
                {
                    MyOSSClient.PutObject(InALiYunOSSOTSLogin.MyBucketName, ToUploadFileFullName, MySendFileStream, MyObjectMetadata);


                }
                */

                 ResultMessageStr = "私有上传文件成功！";

            }
            catch (Exception AnyException)
            {

                ResultMessageStr = "私有上传文件失败！";
            }


        }

        public void PrivateUploadFile2(ref Stream InStreamContent, ProgressBar UIViewProg)
        {

            try
            {
                ObjectMetadata MyObjectMetadata = new ObjectMetadata();
                // 设定自定义的metadata。
                MyObjectMetadata.UserMetadata.Add("FileLength", InStreamContent.Length.ToString());
                MyObjectMetadata.UserMetadata.Add("CreateDate", DateTime.Now.ToString());

                MyObjectMetadata.ContentType = "image/jpeg";

                string ToUploadFileFullName = MyALiYunOSSOTSLogin.PrefixStr + MyALiYunOSSOTSLogin.FileKeyName;// UpLoadFileKeyName;

                OssClient MyOSSClient = new OssClient(MyALiYunOSSOTSLogin.MyAccessID, MyALiYunOSSOTSLogin.MyAccessKey);
                MyOSSClient.PutObject(MyALiYunOSSOTSLogin.MyBucketName, ToUploadFileFullName, InStreamContent, MyObjectMetadata);
                ResultMessageStr = "上传文件成功！";

            }
            catch (Exception AnyException)
            {

                ResultMessageStr = "上传文件失败！";
            }


        }

        public void PrivateUploadFile3(ref MemoryStream InStreamContent)
        {

            try
            {
                ObjectMetadata MyObjectMetadata = new ObjectMetadata();
                // 设定自定义的metadata。
                MyObjectMetadata.UserMetadata.Add("FileLength", InStreamContent.Length.ToString());
                MyObjectMetadata.UserMetadata.Add("CreateDate", DateTime.Now.ToString());

                MyObjectMetadata.ContentType = "image/jpeg";

                string ToUploadFileKeyName = MyALiYunOSSOTSLogin.PrefixStr + MyALiYunOSSOTSLogin.FileKeyName;// UpLoadFileKeyName;

                OssClient MyOSSClient = new OssClient(MyALiYunOSSOTSLogin.MyAccessID, MyALiYunOSSOTSLogin.MyAccessKey);
                MyOSSClient.PutObject(MyALiYunOSSOTSLogin.MyBucketName, ToUploadFileKeyName, InStreamContent, MyObjectMetadata);
                ResultMessageStr = "上传文件成功！";

            }
            catch (Exception AnyException)
            {

                ResultMessageStr = "上传文件失败！";
            }


        }



    }
}
