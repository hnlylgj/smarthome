using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LGJAsynchSocketService;

namespace AsynchSocketServiceUILib
{
    public class CloudLockServiceUIForm : AsynchSocketSeverUIFormBaseFrame 
    {
        AsynchLockServerSocketService MyAsynchLockServerSocketService;
      
        public CloudLockServiceUIForm(): base()
        {
            MyAsynchLockServerSocketService = new LGJAsynchSocketService.AsynchLockServerSocketService();

            MyAsynchSocketServiceBaseFrame = MyAsynchLockServerSocketService;
            MyManagerSocketLoginUser = MyAsynchLockServerSocketService.MyManagerLoginLockUser;

            MyAsynchLockServerSocketService.MyRefreshNotifyCallBack = new AsynchLockServerSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyRefreshNotifyCallBack = new ManagerLoginLockUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);

            nSocketServiceTypeID = SocketServiceTypeID.lockServer;  
            //InitAsynchSocketSet();
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
            grdTableStyle.MappingName = MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList.GetType().Name;// MyManagerSocketLoginUser.MyLoginUserList.GetType().Name;
           

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

            grdColStyle3.HeaderText = "智能端ID";
            grdColStyle3.MappingName = "LockID";
            grdColStyle3.Alignment = HorizontalAlignment.Center;
            grdColStyle3.Width = 230;



            //---4------------------------------------------------------------------

            DataGridTextBoxColumn grdColStyle4 = new DataGridTextBoxColumn();

            grdColStyle4.HeaderText = "移动端ID";
            grdColStyle4.MappingName = "MobileID";
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

            
              ////MyLockServerAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchLockServerSocketService();
            //MyLockServerAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new AsynchLockServerSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            //MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyRefreshNotifyCallBack = new ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
            ///MyManagerSocketLoginUser = MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser;
        }

        protected override void InitMainGridUIStyles()
        {
            DataGridLoginUser.DataSource =MyAsynchLockServerSocketService.MyManagerLoginLockUser.MyLoginUserList;// MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList;
            FormatGridWithBothTableAndColumnStyles();
           
                       
            MessageNotifyIconText = "智能终端服务器:";
            this.Icon = (System.Drawing.Icon)(Properties.Resources.lgj2015Cotroller);

            this.Text = "智能终端服务器-->智能总线服务";
            this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.lgj2015Cotroller); ;// ((System.Drawing.Icon)(resources.GetObject("MessageNotifyIcon.Icon")));
            this.MessageNotifyIcon.Text = "智能终端服务器-->智能总线服务";

          
        }

        /*
        protected override void AsynchSocketSeverUIBaseFormClosed()
        {
            base.AsynchSocketSeverUIBaseFormClosed(); 
             //IsCloseOwerFormID = true;
            //MyAsynchSocketServiceBaseFrame.CloseAsynchSocketListerService();
            //if (MyOwerMainFormShowHandler != null) MyOwerMainFormShowHandler(nSocketServiceTypeID);
            MyAsynchLockServerSocketService.CloseLockService(); 


        }
       */

    }
}
