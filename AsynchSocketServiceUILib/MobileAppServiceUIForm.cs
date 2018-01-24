using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
//using SmartLockVM.Properties;
using LGJAsynchSocketService;


namespace AsynchSocketServiceUILib
{

    public enum MobileAppTypeID
    {
        Windows, Android, iPhone

    }
    public class MobileAppServiceUIForm : AsynchSocketSeverUIFormBaseFrame 
    {

        MobileAppTypeID MyMobileAppTypeID;
        AsynchMobileAppSocketService MyAsynchMobileAppSocketService;

        public  MobileAppServiceUIForm() : base()
        {
            MyAsynchMobileAppSocketService = new AsynchMobileAppSocketService();
            MyAsynchSocketServiceBaseFrame = MyAsynchMobileAppSocketService;
            MyManagerSocketLoginUser = MyAsynchMobileAppSocketService.MyManagerLoginMobileUser;//.MyManagerSocketLoginUser;

            MyAsynchMobileAppSocketService.MySocketServiceTypeID = LGJAsynchSocketService.SocketServiceTypeID.MobileAppServer;
            MyAsynchMobileAppSocketService.MyRefreshNotifyCallBack = new AsynchMobileAppSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.MyRefreshNotifyCallBack = new ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            nSocketServiceTypeID = SocketServiceTypeID.MobileAppServer;

            MyMobileAppTypeID = MobileAppTypeID.Windows;
            
            //InitAsynchSocketSet();
        }
        public void SetTCPListerPort(int PortID)
        {
            //MyManagerSocketLoginUser.MyAsynchSendMessageCallback = new ManagerSocketLoginUser.AsynchSendMessageCallback(this.StartAsynchSendMessage);   
            MyAsynchMobileAppSocketService.SetPortID(PortID);
        }
        public void SetMainFormIcon(MobileAppTypeID InMyMobileAppTypeID)
        {

            MyMobileAppTypeID = InMyMobileAppTypeID;// MobileAppTypeID.Android;

         }
     
        protected override void FormatGridWithBothTableAndColumnStyles()
        {

            DataGridLoginUser.BackColor = Color.GhostWhite;
            DataGridLoginUser.BackgroundColor = Color.Lavender;
            DataGridLoginUser.BorderStyle = BorderStyle.None;
            DataGridLoginUser.CaptionBackColor = Color.RoyalBlue;
            DataGridLoginUser.CaptionFont = new Font("Tahoma", 13.0F, FontStyle.Bold);//10.0
            DataGridLoginUser.CaptionForeColor = Color.Bisque;
            DataGridLoginUser.CaptionText = "服务总线";// 
            DataGridLoginUser.Font = new Font("Tahoma", 15.0F); //12.0
            DataGridLoginUser.ParentRowsBackColor = Color.Lavender;
            DataGridLoginUser.ParentRowsForeColor = Color.MidnightBlue;


            DataGridTableStyle grdTableStyle = new DataGridTableStyle();

            grdTableStyle.AlternatingBackColor = Color.GhostWhite;
            grdTableStyle.BackColor = Color.GhostWhite;
            grdTableStyle.ForeColor = Color.MidnightBlue;
            grdTableStyle.GridLineColor = Color.RoyalBlue;
            grdTableStyle.HeaderBackColor = Color.MidnightBlue;
            grdTableStyle.HeaderFont = new Font("Tahoma", 15.0F, FontStyle.Bold);//12.0
            grdTableStyle.HeaderForeColor = Color.Lavender;
            grdTableStyle.SelectionBackColor = Color.Teal;
            grdTableStyle.SelectionForeColor = Color.PaleGreen;
            //grdTableStyle.MappingName = "ArrayList" ;
            //grdTableStyle.MappingName = ChargeDetailListEntityOBJ.GetType().Name;
            grdTableStyle.MappingName = MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.MyLoginUserList.GetType().Name;//MyManagerSocketLoginUser.MyLoginUserList.GetType().Name;

            grdTableStyle.PreferredColumnWidth = 125;
            grdTableStyle.PreferredRowHeight = 15;
            grdTableStyle.ReadOnly = true;


            //---1-------------------------------------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle1 = new DataGridTextBoxColumn();
            grdColStyle1.HeaderText = "通道编号";
            grdColStyle1.MappingName = "LoginID";
            grdColStyle1.Alignment = HorizontalAlignment.Center;
            grdColStyle1.Width = 150;


            //---2------------------------------------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle2 = new DataGridTextBoxColumn();
            grdColStyle2.HeaderText = "套接字";//
            grdColStyle2.MappingName = "SocketInfor";// "GetRemoteEndIP";
            grdColStyle2.Alignment = HorizontalAlignment.Center;
            grdColStyle2.Width = 260;


            //---3------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle3 = new DataGridTextBoxColumn();

            grdColStyle3.HeaderText = "移动端ID";
            grdColStyle3.MappingName = "MobileID";
            grdColStyle3.Alignment = HorizontalAlignment.Center;
            grdColStyle3.Width = 230;


            //---4------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle4 = new DataGridTextBoxColumn();

            grdColStyle4.HeaderText = "智能端ID";
            grdColStyle4.MappingName = "LockID";
            grdColStyle4.Alignment = HorizontalAlignment.Center;
            grdColStyle4.Width = 230;



          
            //---5------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle5 = new DataGridTextBoxColumn();

            grdColStyle5.HeaderText = "登录时间";
            grdColStyle5.MappingName = "GetLoginTime";
            grdColStyle5.Width = 220;
            grdColStyle5.Alignment = HorizontalAlignment.Center;

            //---6------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle6 = new DataGridTextBoxColumn();

            grdColStyle6.HeaderText = "活动时间";
            grdColStyle6.MappingName = "GetKeepTime";
            grdColStyle6.Alignment = HorizontalAlignment.Center;
            grdColStyle6.Width = 220;



            //------------------------------------------------------------------------------------------------------------------------------

            grdTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] { grdColStyle1, grdColStyle2, grdColStyle3, grdColStyle4, grdColStyle5, grdColStyle6 });


            DataGridLoginUser.TableStyles.Add(grdTableStyle);
        }

        /*
        private void InitAsynchSocketSet()
        {
             
              //nSocketServiceTypeID = SocketServiceTypeID.MobileAppServer;  
              //MyLockServerAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchMobileAppSocketService();
              //MyLockServerAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchMobileAppSocketService(this.MyManagerSocketLoginUser);

              //MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser = this.MyManagerSocketLoginUser;  
              //MyLockServerAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new LGJAsynchSocketService.AsynchMobileAppSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
              

              //MyMobileServerClientAPI = new MobileServerClientAPI();
              //MyLockServerAsynchSocketServiceBaseFrame.MyLGJSocketClientAPIBase = this.MyMobileServerClientAPI; 

              //MyMobileServerClientAPI.RemoteServerColseEvent += new LGJSocketClientAPIBase.RemoteServerColseHandler(this.RemoteServerColseEventCallBack);
              //MyMobileServerClientAPI.MyRecieveMessageCallback = new LGJSocketClientAPIBase.RecieveMessageCallback(MyLockServerAsynchSocketServiceBaseFrame.SocketClientResponseCallBack);
              //MyMobileServerClientAPI.ReturnSendMessageInvoke = new LGJSocketClientAPIBase.SendRecieveMessageCallback(MyLockServerAsynchSocketServiceBaseFrame.SocketClientRequestCallBack);   
             
                        
             //MyLockServerAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new AsynchMobileAppSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
             //MyAsynchMobileAppSocketService.MyManagerSocketLoginUser.MyRefreshNotifyCallBack = new ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            //MyManagerSocketLoginUser = MyAsynchMobileAppSocketService.MyManagerSocketLoginUser;
        }
        */


        protected override void ShownFinallyInit()
        {

            this.LocalHostIPListcomboBox.Enabled = false;
            this.ShownInitUICtrl();




        }
        protected override void InitMainGridUIStyles()
        {

            DataGridLoginUser.DataSource =MyAsynchMobileAppSocketService.MyManagerLoginMobileUser.MyLoginUserList;// MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList;
            FormatGridWithBothTableAndColumnStyles();
            if (this.MyMobileAppTypeID ==MobileAppTypeID.Android)
            {
                this.Icon = (System.Drawing.Icon)(Properties.Resources.lgj201501android);
                this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.lgj201501android);
            }
            else if (this.MyMobileAppTypeID == MobileAppTypeID.Windows)
            {
                this.Icon = (System.Drawing.Icon)(Properties.Resources.Cloud_Mobile_Device);
                this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.Cloud_Mobile_Device);
            }
            else
            {
                this.Icon = (System.Drawing.Icon)(Properties.Resources.Cloud_Mobile_Device);
                this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.Cloud_Mobile_Device);
            }
          

            this.Text = "移动端服务器-->智能总线服务";

            this.MessageNotifyIconText = "移动端服务器:";
           
            this.MessageNotifyIcon.Text = "移动端服务器-->智能总线服务";

           
            


        }

        /*
        protected override bool StartServerLister()
        {

            if (MyAsynchMobileAppSocketService.StartAsynchSocketListerService())
            {
                MyAsynchMobileAppSocketService.MyMobileServerClientAPI.SetServerIP(MyAsynchMobileAppSocketService.LocalListenerIPAddress.ToString(), 8900);  
               if (MyAsynchMobileAppSocketService.MyMobileServerClientAPI.OpenConnect() == 0)
                {
                    
                    InvokeAppendRichTextBoxStatus("**成功连接云锁服务器！**");
                }
                else
                {
                    
                    InvokeAppendRichTextBoxStatus("**连接云锁服务器失败！**");
                }
                return true;
            }
            else
            {
                return false;

            }
        }

        protected override void StopServerLister()
        {
            base.StopServerLister();
            MyAsynchMobileAppSocketService.MyMobileServerClientAPI.CloseConnect();
        }
        */
       
        /*
        protected override void AsynchSocketSeverUIBaseFormClosed()
        {

            IsCloseOwerFormID = true;
            MyAsynchMobileAppSocketService.CloseAsynchSocketListerService();
            if (MyOwerMainFormShowHandler != null) MyOwerMainFormShowHandler(nSocketServiceTypeID);

           /// MyAsynchMobileAppSocketService.MyMobileServerClientAPI.CloseConnect();
        }
        */

        private void RemoteServerColseEventCallBack(object sender, EventArgs e)
        {

            //if (IsCloseOwerFormID == true) return;//主窗已关闭
            //InvokeAppendRichTextBoxStatus("云锁服务器已关闭Socket服务！");
           
        }

       
    }
}
