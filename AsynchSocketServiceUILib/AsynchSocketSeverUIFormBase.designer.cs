namespace AsynchSocketServiceUILib
{
    partial class AsynchSocketSeverUIFormBaseFrame
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsynchSocketSeverUIFormBaseFrame));
            this.DataGridLoginUser = new System.Windows.Forms.DataGrid();
            this.MessageNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.StateTabPage = new System.Windows.Forms.TabPage();
            this.SystemSetTabPage = new System.Windows.Forms.TabPage();
            this.DataFormategroupBox = new System.Windows.Forms.GroupBox();
            this.SMSPushcheckBox = new System.Windows.Forms.CheckBox();
            this.NotePushcheckBox = new System.Windows.Forms.CheckBox();
            this.EMailPushcheckBox = new System.Windows.Forms.CheckBox();
            this.FormateCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoRepairGroupBox = new System.Windows.Forms.GroupBox();
            this.FileSavecheckBox = new System.Windows.Forms.CheckBox();
            this.FlowWatchcheckBox = new System.Windows.Forms.CheckBox();
            this.WatchCheckBox = new System.Windows.Forms.CheckBox();
            this.SmartAutoUpdatecheckBox = new System.Windows.Forms.CheckBox();
            this.ThreadradioButton = new System.Windows.Forms.RadioButton();
            this.TimerradioButton = new System.Windows.Forms.RadioButton();
            this.ServerGroupBox = new System.Windows.Forms.GroupBox();
            this.SecuritySelect = new System.Windows.Forms.Label();
            this.AlgorithmcomboBox = new System.Windows.Forms.ComboBox();
            this.LocalHostIPListcomboBox = new System.Windows.Forms.ComboBox();
            this.buttonServerStop = new System.Windows.Forms.Button();
            this.buttonServerStart = new System.Windows.Forms.Button();
            this.SwatchTabPage = new System.Windows.Forms.TabPage();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.RichTextBoxStatus = new System.Windows.Forms.RichTextBox();
            this.groupBoxRecieve = new System.Windows.Forms.GroupBox();
            this.listBoxRecieveData = new System.Windows.Forms.ListBox();
            this.groupBoxSend = new System.Windows.Forms.GroupBox();
            this.listBoxSendData = new System.Windows.Forms.ListBox();
            this.TestTabPage = new System.Windows.Forms.TabPage();
            this.ExtraReceiveRichTextBox = new System.Windows.Forms.RichTextBox();
            this.MainFormAutoTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridLoginUser)).BeginInit();
            this.MainTabControl.SuspendLayout();
            this.StateTabPage.SuspendLayout();
            this.SystemSetTabPage.SuspendLayout();
            this.DataFormategroupBox.SuspendLayout();
            this.AutoRepairGroupBox.SuspendLayout();
            this.ServerGroupBox.SuspendLayout();
            this.SwatchTabPage.SuspendLayout();
            this.groupBoxStatus.SuspendLayout();
            this.groupBoxRecieve.SuspendLayout();
            this.groupBoxSend.SuspendLayout();
            this.TestTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridLoginUser
            // 
            this.DataGridLoginUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridLoginUser.CaptionText = "服务总线信息";
            this.DataGridLoginUser.DataMember = "";
            this.DataGridLoginUser.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.DataGridLoginUser.Location = new System.Drawing.Point(8, 6);
            this.DataGridLoginUser.Name = "DataGridLoginUser";
            this.DataGridLoginUser.Size = new System.Drawing.Size(998, 335);
            this.DataGridLoginUser.TabIndex = 12;
            // 
            // MessageNotifyIcon
            // 
            this.MessageNotifyIcon.Text = "第吉尔智能总线";
            this.MessageNotifyIcon.Visible = true;
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.StateTabPage);
            this.MainTabControl.Controls.Add(this.SystemSetTabPage);
            this.MainTabControl.Controls.Add(this.SwatchTabPage);
            this.MainTabControl.Controls.Add(this.TestTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(1022, 370);
            this.MainTabControl.TabIndex = 0;
            // 
            // StateTabPage
            // 
            this.StateTabPage.Controls.Add(this.DataGridLoginUser);
            this.StateTabPage.Location = new System.Drawing.Point(4, 22);
            this.StateTabPage.Name = "StateTabPage";
            this.StateTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.StateTabPage.Size = new System.Drawing.Size(1014, 344);
            this.StateTabPage.TabIndex = 0;
            this.StateTabPage.Text = "状态";
            this.StateTabPage.UseVisualStyleBackColor = true;
            // 
            // SystemSetTabPage
            // 
            this.SystemSetTabPage.Controls.Add(this.DataFormategroupBox);
            this.SystemSetTabPage.Controls.Add(this.AutoRepairGroupBox);
            this.SystemSetTabPage.Controls.Add(this.ServerGroupBox);
            this.SystemSetTabPage.Location = new System.Drawing.Point(4, 22);
            this.SystemSetTabPage.Name = "SystemSetTabPage";
            this.SystemSetTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.SystemSetTabPage.Size = new System.Drawing.Size(1014, 344);
            this.SystemSetTabPage.TabIndex = 1;
            this.SystemSetTabPage.Text = "设置";
            this.SystemSetTabPage.UseVisualStyleBackColor = true;
            // 
            // DataFormategroupBox
            // 
            this.DataFormategroupBox.Controls.Add(this.SMSPushcheckBox);
            this.DataFormategroupBox.Controls.Add(this.NotePushcheckBox);
            this.DataFormategroupBox.Controls.Add(this.EMailPushcheckBox);
            this.DataFormategroupBox.Controls.Add(this.FormateCheckBox);
            this.DataFormategroupBox.Location = new System.Drawing.Point(491, 17);
            this.DataFormategroupBox.Name = "DataFormategroupBox";
            this.DataFormategroupBox.Size = new System.Drawing.Size(160, 279);
            this.DataFormategroupBox.TabIndex = 21;
            this.DataFormategroupBox.TabStop = false;
            this.DataFormategroupBox.Text = "通讯设置";
            // 
            // SMSPushcheckBox
            // 
            this.SMSPushcheckBox.AutoSize = true;
            this.SMSPushcheckBox.Location = new System.Drawing.Point(36, 172);
            this.SMSPushcheckBox.Name = "SMSPushcheckBox";
            this.SMSPushcheckBox.Size = new System.Drawing.Size(72, 16);
            this.SMSPushcheckBox.TabIndex = 18;
            this.SMSPushcheckBox.Text = "短信推送";
            this.SMSPushcheckBox.UseVisualStyleBackColor = true;
            this.SMSPushcheckBox.CheckedChanged += new System.EventHandler(this.SMSPushcheckBox_CheckedChanged);
            // 
            // NotePushcheckBox
            // 
            this.NotePushcheckBox.AutoSize = true;
            this.NotePushcheckBox.Location = new System.Drawing.Point(36, 126);
            this.NotePushcheckBox.Name = "NotePushcheckBox";
            this.NotePushcheckBox.Size = new System.Drawing.Size(72, 16);
            this.NotePushcheckBox.TabIndex = 18;
            this.NotePushcheckBox.Text = "笔记推送";
            this.NotePushcheckBox.UseVisualStyleBackColor = true;
            this.NotePushcheckBox.CheckedChanged += new System.EventHandler(this.NotePushcheckBox_CheckedChanged);
            // 
            // EMailPushcheckBox
            // 
            this.EMailPushcheckBox.AutoSize = true;
            this.EMailPushcheckBox.Location = new System.Drawing.Point(36, 83);
            this.EMailPushcheckBox.Name = "EMailPushcheckBox";
            this.EMailPushcheckBox.Size = new System.Drawing.Size(72, 16);
            this.EMailPushcheckBox.TabIndex = 18;
            this.EMailPushcheckBox.Text = "邮件推送";
            this.EMailPushcheckBox.UseVisualStyleBackColor = true;
            this.EMailPushcheckBox.CheckedChanged += new System.EventHandler(this.EMailPushcheckBox_CheckedChanged);
            // 
            // FormateCheckBox
            // 
            this.FormateCheckBox.AutoSize = true;
            this.FormateCheckBox.Location = new System.Drawing.Point(36, 46);
            this.FormateCheckBox.Name = "FormateCheckBox";
            this.FormateCheckBox.Size = new System.Drawing.Size(72, 16);
            this.FormateCheckBox.TabIndex = 18;
            this.FormateCheckBox.Text = "小端格式";
            this.FormateCheckBox.UseVisualStyleBackColor = true;
            this.FormateCheckBox.CheckedChanged += new System.EventHandler(this.FormateCheckBox_CheckedChanged);
            // 
            // AutoRepairGroupBox
            // 
            this.AutoRepairGroupBox.Controls.Add(this.FileSavecheckBox);
            this.AutoRepairGroupBox.Controls.Add(this.FlowWatchcheckBox);
            this.AutoRepairGroupBox.Controls.Add(this.WatchCheckBox);
            this.AutoRepairGroupBox.Controls.Add(this.SmartAutoUpdatecheckBox);
            this.AutoRepairGroupBox.Controls.Add(this.ThreadradioButton);
            this.AutoRepairGroupBox.Controls.Add(this.TimerradioButton);
            this.AutoRepairGroupBox.Location = new System.Drawing.Point(226, 17);
            this.AutoRepairGroupBox.Name = "AutoRepairGroupBox";
            this.AutoRepairGroupBox.Size = new System.Drawing.Size(227, 279);
            this.AutoRepairGroupBox.TabIndex = 20;
            this.AutoRepairGroupBox.TabStop = false;
            this.AutoRepairGroupBox.Text = "智能总线维护方式";
            // 
            // FileSavecheckBox
            // 
            this.FileSavecheckBox.AutoSize = true;
            this.FileSavecheckBox.Location = new System.Drawing.Point(52, 205);
            this.FileSavecheckBox.Name = "FileSavecheckBox";
            this.FileSavecheckBox.Size = new System.Drawing.Size(108, 16);
            this.FileSavecheckBox.TabIndex = 18;
            this.FileSavecheckBox.Text = "保存图像到文件";
            this.FileSavecheckBox.UseVisualStyleBackColor = true;
            this.FileSavecheckBox.CheckedChanged += new System.EventHandler(this.FileSavecheckBox_CheckedChanged);
            // 
            // FlowWatchcheckBox
            // 
            this.FlowWatchcheckBox.AutoSize = true;
            this.FlowWatchcheckBox.Checked = true;
            this.FlowWatchcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FlowWatchcheckBox.Location = new System.Drawing.Point(52, 162);
            this.FlowWatchcheckBox.Name = "FlowWatchcheckBox";
            this.FlowWatchcheckBox.Size = new System.Drawing.Size(72, 16);
            this.FlowWatchcheckBox.TabIndex = 17;
            this.FlowWatchcheckBox.Text = "监控明细";
            this.FlowWatchcheckBox.UseVisualStyleBackColor = true;
            this.FlowWatchcheckBox.CheckedChanged += new System.EventHandler(this.WatchCheckBox_CheckedChanged);
            // 
            // WatchCheckBox
            // 
            this.WatchCheckBox.AutoSize = true;
            this.WatchCheckBox.Checked = true;
            this.WatchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WatchCheckBox.Location = new System.Drawing.Point(52, 112);
            this.WatchCheckBox.Name = "WatchCheckBox";
            this.WatchCheckBox.Size = new System.Drawing.Size(72, 16);
            this.WatchCheckBox.TabIndex = 17;
            this.WatchCheckBox.Text = "监控总线";
            this.WatchCheckBox.UseVisualStyleBackColor = true;
            this.WatchCheckBox.CheckedChanged += new System.EventHandler(this.WatchCheckBox_CheckedChanged);
            // 
            // SmartAutoUpdatecheckBox
            // 
            this.SmartAutoUpdatecheckBox.AutoSize = true;
            this.SmartAutoUpdatecheckBox.Location = new System.Drawing.Point(52, 61);
            this.SmartAutoUpdatecheckBox.Name = "SmartAutoUpdatecheckBox";
            this.SmartAutoUpdatecheckBox.Size = new System.Drawing.Size(96, 16);
            this.SmartAutoUpdatecheckBox.TabIndex = 17;
            this.SmartAutoUpdatecheckBox.Text = "启动自动注销";
            this.SmartAutoUpdatecheckBox.UseVisualStyleBackColor = true;
            // 
            // ThreadradioButton
            // 
            this.ThreadradioButton.AutoSize = true;
            this.ThreadradioButton.Location = new System.Drawing.Point(116, 22);
            this.ThreadradioButton.Name = "ThreadradioButton";
            this.ThreadradioButton.Size = new System.Drawing.Size(71, 16);
            this.ThreadradioButton.TabIndex = 18;
            this.ThreadradioButton.TabStop = true;
            this.ThreadradioButton.Text = "检测线程";
            this.ThreadradioButton.UseVisualStyleBackColor = true;
            this.ThreadradioButton.CheckedChanged += new System.EventHandler(this.ThreadradioButton_CheckedChanged);
            // 
            // TimerradioButton
            // 
            this.TimerradioButton.AutoSize = true;
            this.TimerradioButton.Location = new System.Drawing.Point(23, 22);
            this.TimerradioButton.Name = "TimerradioButton";
            this.TimerradioButton.Size = new System.Drawing.Size(59, 16);
            this.TimerradioButton.TabIndex = 18;
            this.TimerradioButton.TabStop = true;
            this.TimerradioButton.Text = "定时器";
            this.TimerradioButton.UseVisualStyleBackColor = true;
            // 
            // ServerGroupBox
            // 
            this.ServerGroupBox.Controls.Add(this.SecuritySelect);
            this.ServerGroupBox.Controls.Add(this.AlgorithmcomboBox);
            this.ServerGroupBox.Controls.Add(this.LocalHostIPListcomboBox);
            this.ServerGroupBox.Controls.Add(this.buttonServerStop);
            this.ServerGroupBox.Controls.Add(this.buttonServerStart);
            this.ServerGroupBox.Location = new System.Drawing.Point(8, 17);
            this.ServerGroupBox.Name = "ServerGroupBox";
            this.ServerGroupBox.Size = new System.Drawing.Size(200, 279);
            this.ServerGroupBox.TabIndex = 2;
            this.ServerGroupBox.TabStop = false;
            this.ServerGroupBox.Text = "服务器状态";
            // 
            // SecuritySelect
            // 
            this.SecuritySelect.AutoSize = true;
            this.SecuritySelect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SecuritySelect.Location = new System.Drawing.Point(37, 180);
            this.SecuritySelect.Name = "SecuritySelect";
            this.SecuritySelect.Size = new System.Drawing.Size(91, 14);
            this.SecuritySelect.TabIndex = 3;
            this.SecuritySelect.Text = "加密算法设置";
            // 
            // AlgorithmcomboBox
            // 
            this.AlgorithmcomboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AlgorithmcomboBox.FormattingEnabled = true;
            this.AlgorithmcomboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.AlgorithmcomboBox.Location = new System.Drawing.Point(31, 199);
            this.AlgorithmcomboBox.Name = "AlgorithmcomboBox";
            this.AlgorithmcomboBox.Size = new System.Drawing.Size(110, 24);
            this.AlgorithmcomboBox.TabIndex = 2;
            this.AlgorithmcomboBox.SelectedIndexChanged += new System.EventHandler(this.AlgorithmcomboBox_SelectedIndexChanged);
            // 
            // LocalHostIPListcomboBox
            // 
            this.LocalHostIPListcomboBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LocalHostIPListcomboBox.FormattingEnabled = true;
            this.LocalHostIPListcomboBox.Location = new System.Drawing.Point(6, 28);
            this.LocalHostIPListcomboBox.Name = "LocalHostIPListcomboBox";
            this.LocalHostIPListcomboBox.Size = new System.Drawing.Size(175, 24);
            this.LocalHostIPListcomboBox.TabIndex = 0;
            this.LocalHostIPListcomboBox.SelectedIndexChanged += new System.EventHandler(this.LocalHostIPListcomboBox_SelectedIndexChanged);
            // 
            // buttonServerStop
            // 
            this.buttonServerStop.Location = new System.Drawing.Point(48, 122);
            this.buttonServerStop.Name = "buttonServerStop";
            this.buttonServerStop.Size = new System.Drawing.Size(75, 23);
            this.buttonServerStop.TabIndex = 1;
            this.buttonServerStop.Text = "停止服务";
            this.buttonServerStop.UseVisualStyleBackColor = true;
            this.buttonServerStop.Click += new System.EventHandler(this.buttonServerStop_Click);
            // 
            // buttonServerStart
            // 
            this.buttonServerStart.AccessibleRole = System.Windows.Forms.AccessibleRole.RowHeader;
            this.buttonServerStart.Location = new System.Drawing.Point(48, 72);
            this.buttonServerStart.Name = "buttonServerStart";
            this.buttonServerStart.Size = new System.Drawing.Size(75, 23);
            this.buttonServerStart.TabIndex = 1;
            this.buttonServerStart.Text = "开始服务";
            this.buttonServerStart.UseVisualStyleBackColor = true;
            this.buttonServerStart.Click += new System.EventHandler(this.buttonServerStart_Click);
            // 
            // SwatchTabPage
            // 
            this.SwatchTabPage.Controls.Add(this.groupBoxStatus);
            this.SwatchTabPage.Controls.Add(this.groupBoxRecieve);
            this.SwatchTabPage.Controls.Add(this.groupBoxSend);
            this.SwatchTabPage.Location = new System.Drawing.Point(4, 22);
            this.SwatchTabPage.Name = "SwatchTabPage";
            this.SwatchTabPage.Size = new System.Drawing.Size(1014, 344);
            this.SwatchTabPage.TabIndex = 2;
            this.SwatchTabPage.Text = "监视";
            this.SwatchTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStatus.Controls.Add(this.RichTextBoxStatus);
            this.groupBoxStatus.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxStatus.Location = new System.Drawing.Point(500, 12);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(511, 321);
            this.groupBoxStatus.TabIndex = 19;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "状态输出";
            // 
            // RichTextBoxStatus
            // 
            this.RichTextBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextBoxStatus.Location = new System.Drawing.Point(3, 19);
            this.RichTextBoxStatus.Name = "RichTextBoxStatus";
            this.RichTextBoxStatus.Size = new System.Drawing.Size(505, 299);
            this.RichTextBoxStatus.TabIndex = 0;
            this.RichTextBoxStatus.Text = "";
            // 
            // groupBoxRecieve
            // 
            this.groupBoxRecieve.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRecieve.Controls.Add(this.listBoxRecieveData);
            this.groupBoxRecieve.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxRecieve.Location = new System.Drawing.Point(391, 12);
            this.groupBoxRecieve.Name = "groupBoxRecieve";
            this.groupBoxRecieve.Size = new System.Drawing.Size(103, 329);
            this.groupBoxRecieve.TabIndex = 18;
            this.groupBoxRecieve.TabStop = false;
            this.groupBoxRecieve.Text = "接收";
            // 
            // listBoxRecieveData
            // 
            this.listBoxRecieveData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxRecieveData.FormattingEnabled = true;
            this.listBoxRecieveData.HorizontalScrollbar = true;
            this.listBoxRecieveData.ItemHeight = 14;
            this.listBoxRecieveData.Location = new System.Drawing.Point(3, 19);
            this.listBoxRecieveData.Name = "listBoxRecieveData";
            this.listBoxRecieveData.ScrollAlwaysVisible = true;
            this.listBoxRecieveData.Size = new System.Drawing.Size(97, 307);
            this.listBoxRecieveData.TabIndex = 0;
            // 
            // groupBoxSend
            // 
            this.groupBoxSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxSend.Controls.Add(this.listBoxSendData);
            this.groupBoxSend.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxSend.Location = new System.Drawing.Point(3, 12);
            this.groupBoxSend.Name = "groupBoxSend";
            this.groupBoxSend.Size = new System.Drawing.Size(382, 329);
            this.groupBoxSend.TabIndex = 5;
            this.groupBoxSend.TabStop = false;
            this.groupBoxSend.Text = "发送";
            // 
            // listBoxSendData
            // 
            this.listBoxSendData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxSendData.FormattingEnabled = true;
            this.listBoxSendData.HorizontalScrollbar = true;
            this.listBoxSendData.ItemHeight = 14;
            this.listBoxSendData.Location = new System.Drawing.Point(3, 19);
            this.listBoxSendData.Name = "listBoxSendData";
            this.listBoxSendData.ScrollAlwaysVisible = true;
            this.listBoxSendData.Size = new System.Drawing.Size(373, 312);
            this.listBoxSendData.TabIndex = 0;
            // 
            // TestTabPage
            // 
            this.TestTabPage.Controls.Add(this.ExtraReceiveRichTextBox);
            this.TestTabPage.Location = new System.Drawing.Point(4, 22);
            this.TestTabPage.Name = "TestTabPage";
            this.TestTabPage.Size = new System.Drawing.Size(1014, 344);
            this.TestTabPage.TabIndex = 3;
            this.TestTabPage.Text = "附加";
            this.TestTabPage.UseVisualStyleBackColor = true;
            // 
            // ExtraReceiveRichTextBox
            // 
            this.ExtraReceiveRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtraReceiveRichTextBox.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExtraReceiveRichTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ExtraReceiveRichTextBox.Location = new System.Drawing.Point(8, 3);
            this.ExtraReceiveRichTextBox.Name = "ExtraReceiveRichTextBox";
            this.ExtraReceiveRichTextBox.Size = new System.Drawing.Size(998, 333);
            this.ExtraReceiveRichTextBox.TabIndex = 0;
            this.ExtraReceiveRichTextBox.Text = "";
            // 
            // MainFormAutoTimer
            // 
            this.MainFormAutoTimer.Interval = 1000;
            this.MainFormAutoTimer.Tick += new System.EventHandler(this.MainFormAutoTimer_Tick);
            // 
            // AsynchSocketSeverUIFormBaseFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 370);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AsynchSocketSeverUIFormBaseFrame";
            this.Text = "服务器-->智能总线服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AsynchSocketSeverUIFormBaseFrame_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AsynchSocketSeverUIFormBaseFrame_FormClosed);
            this.Load += new System.EventHandler(this.AsynchSocketSeverUIFormBaseFrame_Load);
            this.Shown += new System.EventHandler(this.AsynchSocketSeverUIFormBaseFrame_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridLoginUser)).EndInit();
            this.MainTabControl.ResumeLayout(false);
            this.StateTabPage.ResumeLayout(false);
            this.SystemSetTabPage.ResumeLayout(false);
            this.DataFormategroupBox.ResumeLayout(false);
            this.DataFormategroupBox.PerformLayout();
            this.AutoRepairGroupBox.ResumeLayout(false);
            this.AutoRepairGroupBox.PerformLayout();
            this.ServerGroupBox.ResumeLayout(false);
            this.ServerGroupBox.PerformLayout();
            this.SwatchTabPage.ResumeLayout(false);
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxRecieve.ResumeLayout(false);
            this.groupBoxSend.ResumeLayout(false);
            this.TestTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGrid DataGridLoginUser;
        protected System.Windows.Forms.NotifyIcon MessageNotifyIcon;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage StateTabPage;
        private System.Windows.Forms.TabPage SystemSetTabPage;
        private System.Windows.Forms.TabPage SwatchTabPage;
        private System.Windows.Forms.TabPage TestTabPage;
        private System.Windows.Forms.GroupBox ServerGroupBox;
        protected System.Windows.Forms.ComboBox LocalHostIPListcomboBox;
        private System.Windows.Forms.Button buttonServerStop;
        private System.Windows.Forms.Button buttonServerStart;
        private System.Windows.Forms.GroupBox AutoRepairGroupBox;
        private System.Windows.Forms.CheckBox WatchCheckBox;
        private System.Windows.Forms.CheckBox SmartAutoUpdatecheckBox;
        private System.Windows.Forms.RadioButton ThreadradioButton;
        private System.Windows.Forms.RadioButton TimerradioButton;
        private System.Windows.Forms.GroupBox groupBoxSend;
        private System.Windows.Forms.ListBox listBoxSendData;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.RichTextBox RichTextBoxStatus;
        private System.Windows.Forms.GroupBox groupBoxRecieve;
        private System.Windows.Forms.ListBox listBoxRecieveData;
        private System.Windows.Forms.Timer MainFormAutoTimer;
        private System.Windows.Forms.GroupBox DataFormategroupBox;
        private System.Windows.Forms.CheckBox FormateCheckBox;
        private System.Windows.Forms.CheckBox FlowWatchcheckBox;
        private System.Windows.Forms.RichTextBox ExtraReceiveRichTextBox;
        private System.Windows.Forms.CheckBox SMSPushcheckBox;
        private System.Windows.Forms.CheckBox NotePushcheckBox;
        private System.Windows.Forms.CheckBox EMailPushcheckBox;
        private System.Windows.Forms.Label SecuritySelect;
        protected System.Windows.Forms.ComboBox AlgorithmcomboBox;
        private System.Windows.Forms.CheckBox FileSavecheckBox;
    }
}

