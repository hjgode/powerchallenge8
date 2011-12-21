using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Polenter.Serialization;

namespace PowerChallenge
{
    public partial class FormOptions : Form
    {
        string sSettingsFile = "\\powerchallenge8.xml";

        public PowerSettings mySettings;
        SharpSerializer serializer = new SharpSerializer();
        public FormOptions()
        {
            InitializeComponent();
            mySettings = new PowerSettings();

            sSettingsFile=mySettings.sSettingsFile;

            if (System.IO.File.Exists(sSettingsFile))
            {
                mySettings = (PowerSettings)serializer.Deserialize(sSettingsFile);
            }
            chkCamera.Checked = mySettings.bCAMERAenabled;
            chkScanner.Checked = mySettings.bSCANNERenabled;
            chkWLAN.Checked = mySettings.bWLANenabled;
            chkWWAN.Checked = mySettings.bWWANenabled;
            chkBackLight.Checked = mySettings.bBKLenabled;

            txtCamera.Text = mySettings.iCameraCount.ToString();
            txtScans.Text = mySettings.iScanCount.ToString();
            txtWLAN.Text = mySettings.iWlanDuration.ToString();
            txtWWAN.Text = mySettings.iWWANinterval.ToString();
            txtWWANfile.Text = mySettings.sWWANfile;

            txtBacklight.Text = mySettings.iBklOnInterval.ToString();

            //base settings
            numBacklight.Value = mySettings.iBacklight;
            chkPowerBT.Checked = mySettings.enableBT;
            chkPowerWLAN.Checked = mySettings.enableWLAN;
            chkPowerWWAN.Checked = mySettings.enableWWAN;
        }

        private void mnuOK_Click(object sender, EventArgs e)
        {
            mySettings.bCAMERAenabled = chkCamera.Checked;
            mySettings.bSCANNERenabled = chkScanner.Checked;
            mySettings.bWLANenabled = chkWLAN.Checked;
            mySettings.bWWANenabled = chkWWAN.Checked;

            mySettings.enableBT = chkPowerBT.Checked;
            mySettings.enableWLAN = chkPowerWLAN.Checked;
            mySettings.enableWWAN = chkPowerWWAN.Checked;

            mySettings.enableBKL = chkBackLight.Checked;

            try
            {
                mySettings.iBacklight = (int)numBacklight.Value;
            }
            catch (Exception)
            {}

            try
            {
                mySettings.iCameraCount = int.Parse(txtCamera.Text);
            }
            catch (Exception)
            {}

            try
            {
                mySettings.iScanCount = int.Parse(txtScans.Text);
            }
            catch (Exception)
            {}
            try
            {
                mySettings.iWlanDuration = int.Parse(txtWLAN.Text);
            }
            catch (Exception)
            {}

            try
            {
                mySettings.iWWANinterval = int.Parse(txtWWAN.Text);
            }
            catch (Exception)
            {}

            try
            {
                mySettings.iBklOnInterval = int.Parse(txtBacklight.Text);
            }
            catch (Exception)
            {}

            mySettings.sWWANfile = txtWWANfile.Text;

            serializer.Serialize(mySettings, sSettingsFile);
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chkScanner_CheckStateChanged(object sender, EventArgs e)
        {
            txtScans.Enabled = chkScanner.Checked;
        }

        private void chkWLAN_CheckStateChanged(object sender, EventArgs e)
        {
            txtWLAN.Enabled = chkWLAN.Checked;
        }

        private void chkWWAN_CheckStateChanged(object sender, EventArgs e)
        {
            txtWWAN.Enabled = chkWWAN.Checked;
            txtWWANfile.Enabled = chkWWAN.Checked;
        }

        private void chkCamera_CheckStateChanged(object sender, EventArgs e)
        {
            txtCamera.Enabled = chkCamera.Checked;
        }
    }
}