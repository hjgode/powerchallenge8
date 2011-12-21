namespace PowerChallenge
{
    partial class FormOptions
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
            this.mnuCancel = new System.Windows.Forms.MenuItem();
            this.mnuOK = new System.Windows.Forms.MenuItem();
            this.chkScanner = new System.Windows.Forms.CheckBox();
            this.chkWLAN = new System.Windows.Forms.CheckBox();
            this.chkWWAN = new System.Windows.Forms.CheckBox();
            this.chkCamera = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtScans = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWLAN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWWAN = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCamera = new System.Windows.Forms.TextBox();
            this.txtWWANfile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numBacklight = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.chkPowerWWAN = new System.Windows.Forms.CheckBox();
            this.chkPowerWLAN = new System.Windows.Forms.CheckBox();
            this.chkPowerBT = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkBackLight = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBacklight = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.mnuCancel);
            this.mainMenu1.MenuItems.Add(this.mnuOK);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Text = "Cancel";
            this.mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // mnuOK
            // 
            this.mnuOK.Text = "OK";
            this.mnuOK.Click += new System.EventHandler(this.mnuOK_Click);
            // 
            // chkScanner
            // 
            this.chkScanner.Checked = true;
            this.chkScanner.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScanner.Location = new System.Drawing.Point(13, 3);
            this.chkScanner.Name = "chkScanner";
            this.chkScanner.Size = new System.Drawing.Size(96, 23);
            this.chkScanner.TabIndex = 0;
            this.chkScanner.Text = "Scanner";
            this.chkScanner.CheckStateChanged += new System.EventHandler(this.chkScanner_CheckStateChanged);
            // 
            // chkWLAN
            // 
            this.chkWLAN.Checked = true;
            this.chkWLAN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWLAN.Location = new System.Drawing.Point(13, 32);
            this.chkWLAN.Name = "chkWLAN";
            this.chkWLAN.Size = new System.Drawing.Size(96, 23);
            this.chkWLAN.TabIndex = 0;
            this.chkWLAN.Text = "WLAN";
            this.chkWLAN.CheckStateChanged += new System.EventHandler(this.chkWLAN_CheckStateChanged);
            // 
            // chkWWAN
            // 
            this.chkWWAN.Checked = true;
            this.chkWWAN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWWAN.Location = new System.Drawing.Point(12, 57);
            this.chkWWAN.Name = "chkWWAN";
            this.chkWWAN.Size = new System.Drawing.Size(70, 23);
            this.chkWWAN.TabIndex = 0;
            this.chkWWAN.Text = "WWAN";
            this.chkWWAN.CheckStateChanged += new System.EventHandler(this.chkWWAN_CheckStateChanged);
            // 
            // chkCamera
            // 
            this.chkCamera.Checked = true;
            this.chkCamera.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCamera.Location = new System.Drawing.Point(12, 110);
            this.chkCamera.Name = "chkCamera";
            this.chkCamera.Size = new System.Drawing.Size(186, 23);
            this.chkCamera.TabIndex = 0;
            this.chkCamera.Text = "Camera (starts after scan)";
            this.chkCamera.CheckStateChanged += new System.EventHandler(this.chkCamera_CheckStateChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(134, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 21);
            this.label1.Text = "Scans:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtScans
            // 
            this.txtScans.Location = new System.Drawing.Point(189, 5);
            this.txtScans.Name = "txtScans";
            this.txtScans.Size = new System.Drawing.Size(39, 21);
            this.txtScans.TabIndex = 2;
            this.txtScans.Text = "500";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(88, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 21);
            this.label2.Text = "Duration (min):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtWLAN
            // 
            this.txtWLAN.Location = new System.Drawing.Point(189, 32);
            this.txtWLAN.Name = "txtWLAN";
            this.txtWLAN.Size = new System.Drawing.Size(39, 21);
            this.txtWLAN.TabIndex = 2;
            this.txtWLAN.Text = "60";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(88, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 21);
            this.label3.Text = "Interval (min):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtWWAN
            // 
            this.txtWWAN.Location = new System.Drawing.Point(189, 59);
            this.txtWWAN.Name = "txtWWAN";
            this.txtWWAN.Size = new System.Drawing.Size(39, 21);
            this.txtWWAN.TabIndex = 2;
            this.txtWWAN.Text = "5";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(134, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 21);
            this.label4.Text = "Count:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtCamera
            // 
            this.txtCamera.Location = new System.Drawing.Point(189, 136);
            this.txtCamera.Name = "txtCamera";
            this.txtCamera.Size = new System.Drawing.Size(39, 21);
            this.txtCamera.TabIndex = 2;
            this.txtCamera.Text = "30";
            // 
            // txtWWANfile
            // 
            this.txtWWANfile.Location = new System.Drawing.Point(51, 83);
            this.txtWWANfile.Name = "txtWWANfile";
            this.txtWWANfile.Size = new System.Drawing.Size(177, 21);
            this.txtWWANfile.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 21);
            this.label5.Text = "URL:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.numBacklight);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.chkPowerWWAN);
            this.panel1.Controls.Add(this.chkPowerWLAN);
            this.panel1.Controls.Add(this.chkPowerBT);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(0, 190);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(239, 77);
            // 
            // numBacklight
            // 
            this.numBacklight.Location = new System.Drawing.Point(189, 48);
            this.numBacklight.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numBacklight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBacklight.Name = "numBacklight";
            this.numBacklight.Size = new System.Drawing.Size(39, 22);
            this.numBacklight.TabIndex = 4;
            this.numBacklight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(108, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 14);
            this.label7.Text = "Backlight:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkPowerWWAN
            // 
            this.chkPowerWWAN.Location = new System.Drawing.Point(77, 23);
            this.chkPowerWWAN.Name = "chkPowerWWAN";
            this.chkPowerWWAN.Size = new System.Drawing.Size(68, 19);
            this.chkPowerWWAN.TabIndex = 1;
            this.chkPowerWWAN.Text = "WWAN";
            // 
            // chkPowerWLAN
            // 
            this.chkPowerWLAN.Location = new System.Drawing.Point(9, 48);
            this.chkPowerWLAN.Name = "chkPowerWLAN";
            this.chkPowerWLAN.Size = new System.Drawing.Size(62, 19);
            this.chkPowerWLAN.TabIndex = 1;
            this.chkPowerWLAN.Text = "WLAN";
            // 
            // chkPowerBT
            // 
            this.chkPowerBT.Location = new System.Drawing.Point(9, 23);
            this.chkPowerBT.Name = "chkPowerBT";
            this.chkPowerBT.Size = new System.Drawing.Size(62, 19);
            this.chkPowerBT.TabIndex = 1;
            this.chkPowerBT.Text = "BT";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 15);
            this.label6.Text = "Base Settings:";
            // 
            // chkBackLight
            // 
            this.chkBackLight.Checked = true;
            this.chkBackLight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBackLight.Location = new System.Drawing.Point(12, 161);
            this.chkBackLight.Name = "chkBackLight";
            this.chkBackLight.Size = new System.Drawing.Size(80, 23);
            this.chkBackLight.TabIndex = 0;
            this.chkBackLight.Text = "Backlight";
            this.chkBackLight.CheckStateChanged += new System.EventHandler(this.chkWWAN_CheckStateChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(88, 163);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 21);
            this.label8.Text = "ON Time (min):";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBacklight
            // 
            this.txtBacklight.Location = new System.Drawing.Point(189, 163);
            this.txtBacklight.Name = "txtBacklight";
            this.txtBacklight.Size = new System.Drawing.Size(39, 21);
            this.txtBacklight.TabIndex = 2;
            this.txtBacklight.Text = "6";
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtCamera);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWWANfile);
            this.Controls.Add(this.txtBacklight);
            this.Controls.Add(this.txtWWAN);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtWLAN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtScans);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkBackLight);
            this.Controls.Add(this.chkCamera);
            this.Controls.Add(this.chkWWAN);
            this.Controls.Add(this.chkWLAN);
            this.Controls.Add(this.chkScanner);
            this.Menu = this.mainMenu1;
            this.Name = "FormOptions";
            this.Text = "PowerChallenge8 Options";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem mnuCancel;
        private System.Windows.Forms.MenuItem mnuOK;
        private System.Windows.Forms.CheckBox chkScanner;
        private System.Windows.Forms.CheckBox chkWLAN;
        private System.Windows.Forms.CheckBox chkWWAN;
        private System.Windows.Forms.CheckBox chkCamera;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtScans;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWLAN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWWAN;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCamera;
        private System.Windows.Forms.TextBox txtWWANfile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numBacklight;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkPowerWWAN;
        private System.Windows.Forms.CheckBox chkPowerWLAN;
        private System.Windows.Forms.CheckBox chkPowerBT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkBackLight;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBacklight;
    }
}