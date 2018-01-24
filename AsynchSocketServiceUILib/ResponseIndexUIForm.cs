using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LGJAsynchSocketService;

namespace AsynchSocketServiceUILib
{
     public class ResponseIndexUIForm : AsynchSocketSeverUIFormBaseFrame 
    {
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
             grdTableStyle.MappingName = MyManagerSocketLoginUser.MyLoginUserList.GetType().Name;

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

             /*
             //---3------------------------------------------------------------------

             DataGridTextBoxColumn grdColStyle3 = new DataGridTextBoxColumn();

             grdColStyle3.HeaderText = "移动端ID";
             grdColStyle3.MappingName = "MobileID";
             grdColStyle3.Alignment = HorizontalAlignment.Center;
             grdColStyle3.Width = 230;


             //---4------------------------------------------------------------------

             DataGridTextBoxColumn grdColStyle4 = new DataGridTextBoxColumn();

             grdColStyle4.HeaderText = "智能锁ID";
             grdColStyle4.MappingName = "LockID";
             grdColStyle4.Alignment = HorizontalAlignment.Center;
             grdColStyle4.Width = 230;
             */



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

             grdTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] { grdColStyle1, grdColStyle2, /*grdColStyle3, grdColStyle4, */grdColStyle5, grdColStyle6 });


             DataGridLoginUser.TableStyles.Add(grdTableStyle);
         }

         protected override void InitMainGridUIStyles()
         {

             this.Icon = (System.Drawing.Icon)(Properties.Resources.lgjResponseIndex);
             this.Text = "第吉尔响应索引服务器-->智能总线服务";
             this.MessageNotifyIconText = "第吉尔响应索引服务器:";
             this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.lgjResponseIndex);
             this.MessageNotifyIcon.Text = "第吉尔响应索引服务器-->智能总线服务";
         }

         private void InitAsynchSocketSet()
         {
                //nSocketServiceTypeID = SocketServiceTypeID.ResponseIndexServer;  
             //MyLockServerAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchResponseIndexSocketService();
             //MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser = this.MyManagerSocketLoginUser; 
             //MyLockServerAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new LGJAsynchSocketService.AsynchResponseIndexSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);

              //MyLockServerAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchResponseIndexSocketService();
              //MyLockServerAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new AsynchResponseIndexSocketService.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
              //MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyRefreshNotifyCallBack = new ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
              //MyManagerSocketLoginUser = MyLockServerAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser;
        
         }
        
    }
}
