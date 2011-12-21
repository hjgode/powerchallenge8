namespace PowerChallenge
{
    partial class PowerChallenge8
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.mnuTest = new System.Windows.Forms.MenuItem();
            this.mnuCamera = new System.Windows.Forms.MenuItem();
            this.mnuOptions = new System.Windows.Forms.MenuItem();
            this.mnuStart = new System.Windows.Forms.MenuItem();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScan = new System.Windows.Forms.TextBox();
            this.txtWiFi = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBattPercent = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtWWAN = new System.Windows.Forms.TextBox();
            this.lblCamera = new System.Windows.Forms.Label();
            this.txtCamera = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBkl = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuFile);
            this.mainMenu1.MenuItems.Add(this.mnuStart);
            // 
            // mnuFile
            // 
            this.mnuFile.MenuItems.Add(this.mnuExit);
            this.mnuFile.MenuItems.Add(this.mnuTest);
            this.mnuFile.MenuItems.Add(this.mnuCamera);
            this.mnuFile.MenuItems.Add(this.mnuOptions);
            this.mnuFile.Text = "File";
            // 
            // mnuExit
            // 
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuTest
            // 
            this.mnuTest.Text = "Test";
            this.mnuTest.Click += new System.EventHandler(this.mnuTest_Click);
            // 
            // mnuCamera
            // 
            this.mnuCamera.Text = "Camera";
            this.mnuCamera.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.Text = "Options";
            this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
            // 
            // mnuStart
            // 
            this.mnuStart.Text = "START";
            this.mnuStart.Click += new System.EventHandler(this.mnuStart_Click);
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.Location = new System.Drawing.Point(0, 191);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(239, 74);
            this.txtLog.TabIndex = 0;
            // 
            // txtInfo
            // 
            this.txtInfo.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.txtInfo.Location = new System.Drawing.Point(3, 101);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(234, 69);
            this.txtInfo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 18);
            this.label1.Text = "Scan:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 18);
            this.label2.Text = "WiFi:";
            // 
            // txtScan
            // 
            this.txtScan.Location = new System.Drawing.Point(53, 37);
            this.txtScan.Name = "txtScan";
            this.txtScan.ReadOnly = true;
            this.txtScan.Size = new System.Drawing.Size(37, 21);
            this.txtScan.TabIndex = 5;
            // 
            // txtWiFi
            // 
            this.txtWiFi.Location = new System.Drawing.Point(53, 61);
            this.txtWiFi.Name = "txtWiFi";
            this.txtWiFi.ReadOnly = true;
            this.txtWiFi.Size = new System.Drawing.Size(37, 21);
            this.txtWiFi.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 18);
            this.label3.Text = "Batt:";
            // 
            // txtBattPercent
            // 
            this.txtBattPercent.Location = new System.Drawing.Point(53, 13);
            this.txtBattPercent.Name = "txtBattPercent";
            this.txtBattPercent.ReadOnly = true;
            this.txtBattPercent.Size = new System.Drawing.Size(37, 21);
            this.txtBattPercent.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(105, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 18);
            this.label4.Text = "%";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(140, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 18);
            this.label5.Text = "WWAN:";
            // 
            // txtWWAN
            // 
            this.txtWWAN.Location = new System.Drawing.Point(201, 61);
            this.txtWWAN.Name = "txtWWAN";
            this.txtWWAN.ReadOnly = true;
            this.txtWWAN.Size = new System.Drawing.Size(36, 21);
            this.txtWWAN.TabIndex = 5;
            // 
            // lblCamera
            // 
            this.lblCamera.Location = new System.Drawing.Point(140, 40);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(55, 18);
            this.lblCamera.Text = "Camera:";
            // 
            // txtCamera
            // 
            this.txtCamera.Location = new System.Drawing.Point(201, 37);
            this.txtCamera.Name = "txtCamera";
            this.txtCamera.ReadOnly = true;
            this.txtCamera.Size = new System.Drawing.Size(36, 21);
            this.txtCamera.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(140, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 18);
            this.label6.Text = "Backl.:";
            // 
            // txtBkl
            // 
            this.txtBkl.Location = new System.Drawing.Point(201, 13);
            this.txtBkl.Name = "txtBkl";
            this.txtBkl.ReadOnly = true;
            this.txtBkl.Size = new System.Drawing.Size(36, 21);
            this.txtBkl.TabIndex = 5;
            // 
            // PowerChallenge8
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.txtBkl);
            this.Controls.Add(this.txtCamera);
            this.Controls.Add(this.txtWWAN);
            this.Controls.Add(this.txtWiFi);
            this.Controls.Add(this.txtBattPercent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtScan);
            this.Controls.Add(this.lblCamera);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.txtLog);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "PowerChallenge8";
            this.Text = "PowerChallenge";
            this.Closed += new System.EventHandler(this.PowerChallenge8_Closed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PowerChallenge8_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuFile;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.TextBox txtWiFi;
        private System.Windows.Forms.MenuItem mnuStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBattPercent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtWWAN;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.MenuItem mnuTest;
        private System.Windows.Forms.MenuItem mnuCamera;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.TextBox txtCamera;
        private System.Windows.Forms.MenuItem mnuOptions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBkl;
    }
}

