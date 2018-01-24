using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LGJAsynchSocketService;


namespace AsynchSocketServiceUILib
 {
   
    public enum SocketServiceTypeID
    {
        lockServer, MobileAppServer, RequestIndexServer, ResponseIndexServer

    }
  
    public partial class AsynchSocketSeverUIFormBaseFrame : Form
    {

        //AsynchSocketServiceBaseFrame MyAsynchSocketService;
        //ManagerLoginLockUser MyManagerLoginLockUser;
        //private List<LoginUser> MyLoginLockUserList = new List<LoginUser>();
        
         protected LGJAsynchSocketService.ManagerSocketLoginUser MyManagerSocketLoginUser;       
         protected LGJAsynchSocketService.AsynchSocketServiceBaseFrame MyAsynchSocketServiceBaseFrame; 


        private delegate void DelegateRefreshdLoginUserCallback();
        private DelegateRefreshdLoginUserCallback MyRefreshDataGridCallBack;

        private delegate void DelegateRefreshPrimeNotifyIcon(string str);
        private DelegateRefreshPrimeNotifyIcon MyRefreshPrimeNotifyIconCallBack;

        protected delegate void DelegateRefreshRichTextBoxStatus(string str);
        protected DelegateRefreshRichTextBoxStatus MyAppendRichTextBoxStatus;

        //private delegate void DelegateAppendClientEndPoint(string str);
        //private DelegateAppendClientEndPoint MyAsynchAppendClientEndPoint;

        //private delegate void DelegateAppendClientLock(string str);
        //private DelegateAppendClientLock MyAsynchAppendClientLock;

        private delegate void delegateAddListBoxItemCallback(string str); 
        private delegateAddListBoxItemCallback MyListBoxSendCallback;
        private delegateAddListBoxItemCallback MyListBoxRecieveCallback;
        //------------------------------------------------------------------
        public delegate void OwerMainFormShowHandler(SocketServiceTypeID InSocketServiceTypeID);
        public OwerMainFormShowHandler MyOwerMainFormShowHandler;

        protected SocketServiceTypeID nSocketServiceTypeID;
        protected string MessageNotifyIconText;
        protected bool IsCloseOwerFormID;

        public int LocalIPAdressSelectIndex;
        //-------------------------------------------------------------------
        public AsynchSocketSeverUIFormBaseFrame()
        {
            InitializeComponent();

              //----------------------------------------------------------------------------------------------------
            //MyManagerLoginLockUser = new ManagerLoginLockUser();
            //MyManagerLoginLockUser.MyCoudLockSeverMainForm = this;
            //MyManagerLoginLockUser.Init();

                     
            // InitAsynchSocketSet();
                         

                 
             //MyAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchSocketServiceBaseFrame(MyManagerSocketLoginUser);
             //MyAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new LGJAsynchSocketService.AsynchSocketServiceBaseFrame.AsynchRefreshNotifyHandler(this.InvokeAllProcess);  
                       
             
             //MyAsynchSocketService = new AsynchSocketServiceBaseFrame(this, MyManagerLoginLockUser);
             //MyAsynchSocketService.MyCoudLockSeverMainForm = this;
             //MyAsynchSocketService.MyManagerLoginLockUser = MyManagerLoginLockUser;  
             //LocalHostIPListcomboBox.Items.AddRange(MyAsynchSocketService.LocalIPAddressArray);

             //LocalHostIPListcomboBox.Items.AddRange(MyAsynchSocketServiceBaseFrame.LocalIPAddressArray);

            MyRefreshDataGridCallBack = new DelegateRefreshdLoginUserCallback(this.AsynchLoginUserRefreshCallBack);
            MyRefreshPrimeNotifyIconCallBack = new DelegateRefreshPrimeNotifyIcon(this.RefreshPrimeNotifyIconCallBack);
            MyAppendRichTextBoxStatus = new DelegateRefreshRichTextBoxStatus(this.AppendRichTextBoxStatusCallBack);

            MyListBoxRecieveCallback = new delegateAddListBoxItemCallback(this.SetRecieveListBoxCallBack);
            MyListBoxSendCallback = new delegateAddListBoxItemCallback(this.SetSendListBoxCallBack);

            LocalIPAdressSelectIndex = -1;           


        }

        private void AsynchSocketSeverUIFormBaseFrame_Load(object sender, EventArgs e)
        {
            LocalHostIPListcomboBox.Items.AddRange(MyAsynchSocketServiceBaseFrame.LocalIPAddressArray);
           
            //DataGridLoginUser.DataSource = MyManagerSocketLoginUser.MyLoginLockUserList; //MyLoginLockUserList;
            //DataGridLoginUser.DataSource = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList; 
            //FormatGridWithBothTableAndColumnStyles();

            InitMainGridUIStyles();
            IsCloseOwerFormID = false;

            
          

               //----Xp兼容性差----------------------------------------------------
            /*
            if (MyAsynchSocketService.LocalIPAddressArray.Length>0)
            {
                //LocalHostIPListcomboBox.SelectedIndex = 0;
            }
            else
            {
                LocalHostIPListcomboBox.Items.Add("127.0.0.1");//脱网情况
                LocalHostIPListcomboBox.SelectedIndex = 1;
            }
             */
            //----------------------------------------------------------
        }

        private void AsynchSocketSeverUIFormBaseFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            ; ;
            
        }

        private void AsynchSocketSeverUIFormBaseFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            AsynchSocketSeverUIBaseFormClosed();

        }
              
        protected virtual void AsynchSocketSeverUIBaseFormClosed()
        {

             IsCloseOwerFormID = true;
            MyAsynchSocketServiceBaseFrame.CloseAsynchSocketListerService();
            if (MyOwerMainFormShowHandler != null) MyOwerMainFormShowHandler(nSocketServiceTypeID);
        }
       

        private void AsynchSocketSeverUIFormBaseFrame_Shown(object sender, EventArgs e)
        {
            if (nSocketServiceTypeID == SocketServiceTypeID.lockServer)
            {
                FormateCheckBox.Checked = true;
                FormateCheckBox.Enabled = false; 
            }
            else
            {
                FormateCheckBox.Enabled = false; 
            }

            //AutoRepairGroupBox.Enabled = false;

            MainTabControl.SelectTab(2);
            AlgorithmcomboBox.SelectedIndex = 0;

            if (LocalIPAdressSelectIndex > -1)
            {
                LocalHostIPListcomboBox.SelectedIndex = LocalIPAdressSelectIndex;
                buttonServerStart_Click(null, null);

            }
            //---------------------
            ShownFinallyInit();

        }
     
       protected virtual void  ShownFinallyInit()
       {
            //子类具体实现
            this.LocalHostIPListcomboBox.Enabled = false;

        }

       protected  void ShownInitUICtrl()
       {

            FileSavecheckBox.Enabled = false;
            EMailPushcheckBox.Enabled = false;
            NotePushcheckBox.Enabled = false;
            SMSPushcheckBox.Enabled = false;
         


        }

        protected virtual void FormatGridWithBothTableAndColumnStyles()
        {

            //子类实现
            /*
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
            */
        }


        private void InitAsynchSocketSet()
        {
            //MyManagerSocketLoginUser = new LGJAsynchSocketService.ManagerSocketLoginUser();
            //MyManagerSocketLoginUser.Init();//InvokeAllProcess(string MesssageStr,int MessageID)
            //MyManagerSocketLoginUser.MyRefreshNotifyCallBack = new LGJAsynchSocketService.ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);

             //nSocketServiceTypeID = SocketServiceTypeID.lockServer;
             //MyAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchSocketServiceBaseFrame(MyManagerSocketLoginUser);

             //MyAsynchSocketServiceBaseFrame = new LGJAsynchSocketService.AsynchSocketServiceBaseFrame();
             // MyAsynchSocketServiceBaseFrame.MyRefreshNotifyCallBack = new AsynchSocketServiceBaseFrame.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
             //MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyRefreshNotifyCallBack = new ManagerSocketLoginUser.AsynchRefreshNotifyHandler(this.InvokeAllProcess);
             //MyManagerSocketLoginUser =MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser;
        
        }
       
        protected virtual void InitMainGridUIStyles()
        {

            DataGridLoginUser.DataSource = MyAsynchSocketServiceBaseFrame.MyManagerSocketLoginUser.MyLoginUserList;
            FormatGridWithBothTableAndColumnStyles();


           MessageNotifyIconText = "云端服务器:";
           this.Icon = (System.Drawing.Icon)(Properties.Resources.MyLockServer);
           this.Text = "云端服务器-->智能总线服务";
           this.MessageNotifyIcon.Icon = (System.Drawing.Icon)(Properties.Resources.MyLockServer); ;// ((System.Drawing.Icon)(resources.GetObject("MessageNotifyIcon.Icon")));
           this.MessageNotifyIcon.Text = "云端服务器-->智能总线服务";
           


         
        
        
        }

        //---CallBack Start--------------------------------------------------------------------------------
      
        private void AsynchLoginUserRefreshCallBack()
        {
            //回调刷新显示登录锁用户信息(增加/心跳/注销) 

            DataGridLoginUser.TableStyles.Clear();
            FormatGridWithBothTableAndColumnStyles();
            DataGridLoginUser.Refresh();


        }

        private void RefreshPrimeNotifyIconCallBack(string InputNotifyMessageStr)
        {
            MessageNotifyIcon.Text = MessageNotifyIconText + InputNotifyMessageStr;
            MessageNotifyIcon.BalloonTipTitle = MessageNotifyIconText;
            MessageNotifyIcon.BalloonTipText = InputNotifyMessageStr;
            MessageNotifyIcon.ShowBalloonTip(100);


        }

        private void AppendRichTextBoxStatusCallBack(string str)
        {
            
             RichTextBoxStatus.AppendText(str+"\r\n"); 
           

        }

        private void SetSendListBoxCallBack(string MessageString)
        {
            listBoxSendData.Items.Add(MessageString);
            listBoxSendData.SelectedIndex = listBoxSendData.Items.Count - 1;
            listBoxSendData.ClearSelected();

            //string MessageID = MessageString.Substring(18, 2) + MessageString.Substring(16, 2);
            //ExtraReceiveRichTextBox.AppendText("[" + MessageID + "]->" + MessageString + "\r\n");
            ExtraReceiveRichTextBox.AppendText(MessageString + "\r\n"); 
        }
       
        private void SetRecieveListBoxCallBack(string str)
        {

            listBoxRecieveData.Items.Add(str);
            listBoxRecieveData.SelectedIndex = listBoxRecieveData.Items.Count - 1;
            listBoxRecieveData.ClearSelected();

           

        }

        //-----CallBack End-----------------------------------------------------------------------
        //----Invokeb Start-----------------------------------------------------------------------

        protected void InvokeAllProcess(string MesssageStr,int MessageID)
        {
            if (IsCloseOwerFormID == true) return;
            if (MessageID == 6)
            {
                System.Windows.Forms.MessageBox.Show("可能是服务侦听套接字错误不能运行将退出！");
                System.Windows.Forms.Application.Exit();
            }

            if (WatchCheckBox.Checked)
            {
                switch (MessageID)
                {

                    case 0:
                        if (FlowWatchcheckBox.Checked) 
                        RefreshPrimeNotifyIconCallBack(MesssageStr);
                        break;
                    case 1:
                        InvokeAppendRichTextBoxStatus(MesssageStr);
                        break;

                    case 2:
                        if(FlowWatchcheckBox.Checked) 
                        InvokeSetRecieveListBox(MesssageStr);
                        break;

                    case 3:
                        if (FlowWatchcheckBox.Checked) 
                        InvokeSetSendListBox(MesssageStr);
                        break;

                    case 4:
                        InvokeLoginUserRefresh();
                        break;
                    default:
                        break;

                }
            }
           
        }

        private void InvokeAppendRichTextBoxStatus(string str)
        {
            if (IsCloseOwerFormID == false)
            RichTextBoxStatus.BeginInvoke(MyAppendRichTextBoxStatus, str);


        }

        private void InvokeLoginUserRefresh()
        {
            //刷新显示登录锁用户信息(增加/心跳/注销) 

            DataGridLoginUser.BeginInvoke(this.MyRefreshDataGridCallBack);


        }

        private void InvokeSetSendListBox(string str)
        {
            listBoxSendData.BeginInvoke(this.MyListBoxSendCallback, str);  
        }

        private void InvokeSetRecieveListBox(string str)
        {

            listBoxRecieveData.BeginInvoke(this.MyListBoxRecieveCallback, str); 
           

        }

        //-----Invokeb End-------------------------------------------------------------------------
        private void buttonServerStart_Click(object sender, EventArgs e)
        {
                //if (LocalHostIPListcomboBox.Text == "") return;

                bool ReturnID=StartServerLister();
                if (ReturnID)
                {
                    buttonServerStart.Enabled = false;
                    buttonServerStop.Enabled = true;
                }

        }

        protected virtual bool StartServerLister()
        {
           
            if (MyAsynchSocketServiceBaseFrame.StartAsynchSocketListerService())
            {
                
                return true;
            }
            else
            {
                return false;

            }
        }

        protected virtual void StopServerLister()
        {

            MyAsynchSocketServiceBaseFrame.CloseAsynchSocketListerService();
            buttonServerStart.Enabled = true;
            buttonServerStop.Enabled = false;
        }

        private void buttonServerStop_Click(object sender, EventArgs e)
        {

            StopServerLister();
         
        }
        
        //-----------------------------------------------------------------------------------------
        private void LocalHostIPListcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //MyAsynchSocketServiceBaseFrame.SelectedLocalHostIPListIndex(LocalHostIPListcomboBox.SelectedIndex); 
        }
              
        private void SendDataButton_Click(object sender, EventArgs e)
        {

        }

        private void MainFormAutoTimer_Tick(object sender, EventArgs e)
        {
            //MyManagerLoginLockUser.TimerSmartAutoUpdateList();  
        }

        private void ThreadradioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FormateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MyAsynchSocketServiceBaseFrame.DataFormateFlag = FormateCheckBox.Checked; 
        }

        private void WatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void EMailPushcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.MyAsynchSocketServiceBaseFrame.IsEMailPush = EMailPushcheckBox.Checked;
        }

        private void NotePushcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.MyAsynchSocketServiceBaseFrame.IsNotePush = NotePushcheckBox.Checked;
        
        }

        private void SMSPushcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.MyAsynchSocketServiceBaseFrame.IsSMSPush = SMSPushcheckBox.Checked;
        }

        private void AlgorithmcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MyAsynchSocketServiceBaseFrame.AlgorithmFlag = AlgorithmcomboBox.SelectedIndex;
        }

        private void FileSavecheckBox_CheckedChanged(object sender, EventArgs e)
        {

            this.MyAsynchSocketServiceBaseFrame.IsSaveToFile = FileSavecheckBox.Checked;

        }

       

       


       

    }
}
