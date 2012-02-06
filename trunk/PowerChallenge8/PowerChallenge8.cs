#define USE_INTERMEC
#define USE_CAMERA
#define USE_WWAN
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Intermec.DataCollection;
using Intermec.Communication;
using Intermec.Multimedia;

using System.Threading;

using Intermec.Utils;

using Polenter.Serialization;

//using CameraTest;

namespace PowerChallenge
{
    public partial class PowerChallenge8 : Form
    {
        SharpSerializer serializer = new SharpSerializer();
        PowerSettings mySettings = new PowerSettings();
        //threads and functions
        List<Thread> startedThreads = new List<Thread>();
        //threads
        ScannerClass2 sc2;
        WiFiClass wifi;
        //camera??? 30 photos with flash
#if USE_CAMERA
        //CameraThreadClass2 camera;
        CameraThreadClass3 camera;
#endif
        //
        //WWAN 10K all 5 minutes
#if USE_WWAN
        WWANClass wwan;
#endif
        //backlight thread
        BKLClass bkl;

        //store and restore settings
        
        RadioDrivers.BT_STATES bt_state;
        RadioDrivers.RADIO_STATES wifi_state;
#if USE_WWAN
        RadioDrivers.RADIO_STATES phone_state;
#endif
        int BacklightLevel;

        Battery MyBattery;

        private System.Windows.Forms.Timer timerUpdate;

        private System.Windows.Forms.Timer timerThreadState;

        private bool bIsRunning = false;

        private bool _bStopThreads = false;

        private int battPercent = 0;

        DateTime startTime;

        public PowerChallenge8()
        {
            InitializeComponent();
            if (System.IO.File.Exists(mySettings.sSettingsFile))
            {
                mySettings = (PowerSettings)serializer.Deserialize(mySettings.sSettingsFile);
            }
            
            Display.enableBacklight(true);
            Display.requestFullPower();

            BacklightLevel = Display.GetBackLightLevel();
            System.Diagnostics.Debug.WriteLine("DeviceType is " + Device.GetDeviceType().ToString());
            addLog("DeviceType is " + Device.GetDeviceType().ToString());
            System.Diagnostics.Debug.WriteLine("MFGCode is " + Device.GetMFGCode());
            addLog("MFGCode is " + Device.GetMFGCode());
            MyBattery = new Battery();

            timerUpdate = new System.Windows.Forms.Timer();
            timerUpdate.Interval=2000;
            timerUpdate.Tick += new EventHandler(timerUpdate_Tick);

            timerThreadState = new System.Windows.Forms.Timer();
            timerThreadState.Interval = 3000;
            timerThreadState.Tick += new EventHandler(timerThreadState_Tick);

            startTime = DateTime.Now;

#if !USE_CAMERA
            txtCamera.Visible=false;
            lblCamera.Visible=false;
            mnuCamera.Enabled = false;
            mnuTest.Enabled = false;
#endif
        }

        void timerThreadState_Tick(object sender, EventArgs e)
        {
            // the scanner
            try
            {
                if (sc2 != null)
                {
                    if (sc2._bIsRunning)
                    {
                        txtScan.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        txtScan.BackColor = Color.LightPink;
                    }
                }
                else
                    txtScan.BackColor = Color.LightPink;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in timerThreadState_Tick for sc2._bIsRunning: " + ex.Message);
            }

            //WiFi
            try
            {
                if (wifi != null)
                {
                    if (wifi._bIsRunning)
                        txtWiFi.BackColor = Color.LightGreen;
                    else
                        txtWiFi.BackColor = Color.LightPink;
                }
                else
                    txtWiFi.BackColor = Color.LightPink;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in timerThreadState_Tick for wifi._bIsRunning: " + ex.Message);
            }
#if USE_WWAN
            //WWAN
            try
            {
                if (wwan != null)
                {
                    if (wwan._bIsRunning)
                        txtWWAN.BackColor = Color.LightGreen;
                    else
                        txtWWAN.BackColor = Color.LightPink;
                }
                else
                    txtWWAN.BackColor = Color.LightPink;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in timerThreadState_Tick for wwan._bIsRunning: " + ex.Message);
            }
#endif
            try
            {
                if (bkl != null)
                {
                    if (bkl._bIsRunning)
                        txtBkl.BackColor = Color.LightGreen;
                    else
                        txtBkl.BackColor = Color.LightPink;
                }
                else
                    txtBkl.BackColor = Color.LightPink;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in timerThreadState_Tick for bkl._bIsRunning: " + ex.Message);
            }
#if USE_CAMERA
            try
            {
                if (camera != null)
                {
                    if (camera._bIsRunning)
                        txtCamera.BackColor = Color.LightGreen;
                    else
                        txtCamera.BackColor = Color.LightPink;
                }
                else
                    txtCamera.BackColor = Color.LightPink;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in timerThreadState_Tick for wwan._bIsRunning: " + ex.Message);
            }
#endif
        }

        private bool stopCamera = false;

        int iTimerCount = 0;
        const int iUpdateInterval = 1;
        void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (_bStopThreads)
                return;
            //start camera after scanner has finished
            if (!stopCamera)
            {
                if (txtScan.Text == "0")
                {
                    if (camera != null)
                    {
                        if (!camera._bIsRunning)
                        {
                            stopCamera = true;
                            startCameraThread();
                        }
                    }
                    else
                    {
                        stopCamera = true;
                        startCameraThread();
                    }
                }
            }
            if (sc2 != null)
            {
                if (iTimerCount == iUpdateInterval)
                    addInfo("ScanAttempts=" + sc2.iCount.ToString());
                txtScan.Text = sc2.iCount.ToString();

            }

            if (wifi != null)
            {
                if (iTimerCount == iUpdateInterval)
                    addInfo("WiFiAttempts=" + wifi.iCount.ToString());
                txtWiFi.Text = wifi.iCount.ToString();
            }
#if USE_WWAN
            if (wwan != null)
            {
                if (iTimerCount == iUpdateInterval)
                    addInfo("WWAN thread: " + wwan.iCount.ToString());
                txtWWAN.Text = wwan.iCount.ToString();
            }
#endif
            if (bkl != null)
            {
                if (iTimerCount == iUpdateInterval)
                    addInfo("BKL thread: " + bkl.iCount.ToString());
                txtBkl.Text = bkl.iCount.ToString();
            }
#if USE_CAMERA
            if (camera != null)
            {
                if (iTimerCount == iUpdateInterval)
                    addInfo("Camera thread: " + camera.iCount.ToString());
                txtCamera.Text = camera.iCount.ToString();
            }
            else
                txtCamera.BackColor = Color.LightPink;
#endif
            txtBattPercent.Text = MyBattery.LifePercent.ToString();
            iTimerCount++;
            //only log batt value every minute and if changed!
            if (iTimerCount > iUpdateInterval && MyBattery.LifePercent!=battPercent)
            {
                iTimerCount = 0;
                battPercent = MyBattery.LifePercent;    //only update Batt percent if changed from previous value!
                LoggingClass.addLog("Batt percent=" + txtBattPercent.Text);
            }

        }
        
        delegate void SetInfoTextCallback(string text);
        private void addInfo(string text)
        {
            if (this.txtInfo.InvokeRequired)
            {
                SetInfoTextCallback d = new SetInfoTextCallback(addInfo);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtInfo.Text.Length > 2000)
                    txtInfo.Text = "";
                txtInfo.Text += text + "\r\n";
                txtInfo.SelectionLength = text.Length;
                txtInfo.SelectionStart = txtInfo.Text.Length - text.Length;
                txtInfo.ScrollToCaret();
            }
        }
        #region logging
        private string _sLogFile = @"\powerchallenge.Log.txt";
        delegate void SetTextCallback(string text);
        private void addLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                DateTime dt = DateTime.Now;
                long elapsedTicks = dt.Ticks - startTime.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

                string timeString = dt.ToShortDateString() + " " + dt.ToShortTimeString() + "\t" + elapsedSpan.Minutes + "\t";
                text = timeString + text;
                fileLog(text + "\r\n");

                if (txtLog.Text.Length > 2000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = text.Length;
                txtLog.SelectionStart = txtLog.Text.Length - text.Length;
                txtLog.ScrollToCaret();
            }
        }
        private void fileLog(string s)
        {
            try
            {
                //cut log file size
                System.IO.FileInfo fi = new System.IO.FileInfo(_sLogFile);
                if (fi.Exists)
                {
                    if (fi.Length > 1000000)
                        System.IO.File.Delete(_sLogFile);
                }

                System.IO.StreamWriter sw = new System.IO.StreamWriter(_sLogFile, true);
                sw.WriteLine(s);
                sw.Flush();
                sw.Close();

            }
            catch (Exception)
            {

            }
        }
        #endregion


        private void setupBase()
        {
            addLog("setting up base");
            // switch all off
            if (mySettings.enableBT)
            {
                RadioDrivers.BlueTooth = RadioDrivers.BT_STATES.On;
                addLog("- BT ON");
            }
            else
            {
                addLog("- BT OFF");
                RadioDrivers.BlueTooth = RadioDrivers.BT_STATES.Off;
            }
#if USE_WWAN
            if (mySettings.bWWANenabled)
            {
                RadioDrivers.Phone = RadioDrivers.RADIO_STATES.On;
                addLog("- WWAN ON");
            }
            else
            {
                addLog("- WWAN OFF");
                RadioDrivers.Phone = RadioDrivers.RADIO_STATES.Off;
            }
#endif
            if (mySettings.bWLANenabled)
            {
                RadioDrivers.WIFI = RadioDrivers.RADIO_STATES.On;
                addLog("- WLAN ON");
            }
            else
            {
                addLog("- WLAN OFF");
                RadioDrivers.WIFI = RadioDrivers.RADIO_STATES.Off;
            }

            Display.SetBackLightLevel(mySettings.iBacklight);
            addLog("- BKL = " + mySettings.iBacklight.ToString());

            addLog("disable BKL idle");
            Display.requestFullPower();
            addLog("disable auto powerdown");
            Device.setIdleOFF();
            //Display.SwitchBackLight(false);
        }
        private void saveSettings()
        {
            addLog("saving settings");
            // preserve the Radio states
            bt_state = RadioDrivers.BlueTooth;
            wifi_state = RadioDrivers.WIFI;
#if USE_WWAN
            phone_state = RadioDrivers.Phone;
#endif
        }
        private void restoreSettings()
        {
            addLog("restoring settings");
            // restore
            Display.SwitchBackLight(true);
            Display.SetBackLightLevel(BacklightLevel);
            RadioDrivers.BlueTooth = bt_state;
            RadioDrivers.WIFI = wifi_state;
#if USE_WWAN
            RadioDrivers.Phone = phone_state;
#endif
            Display.releaseFullPower();
        }

        private void SleepBatteryUpdateCycle()
        {
            System.Threading.Thread.Sleep(MyBattery.PowerUpdateCycle);
        }

        private void stopThreads()
        {
            timerUpdate.Enabled = false;
            timerThreadState.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            addLog("stopping threads");
            if (startedThreads != null && startedThreads.Count>0)
            {
                foreach (Thread th in startedThreads)
                {
                    addLog("Calling '" + th.Name + "' to abort...");

                    th.Abort();
                    //startedThreads.Remove(th);
                    //th.Join(); //block on running thread
                }
            }
            //stop scannerThread
            //if(sc!=null){
            //    while (!sc._threadStopped)
            //    {
            //        System.Threading.Thread.Sleep(10);
            //    }
            //}
            Cursor.Current = Cursors.Default;
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            if (bIsRunning)
            {
                mnuStart_Click(sender, e);
            }
            
            restoreSettings();
            stopThreads();
            Cursor.Current = Cursors.Default;
            this.Close();
        }

        private void mnuStart_Click(object sender, EventArgs e)
        {
            if (!bIsRunning)
            {
                mnuOptions.Enabled = false;
                mnuStart.Text = "STOP";
                Application.DoEvents();
                bIsRunning = true;
                addLog("Test started");
                saveSettings();
                setupBase();

                addLog("Starting Battlog");
                
                startTime = DateTime.Now;

                // let it all come to a rest..
                addLog("Sleep...");
                SleepBatteryUpdateCycle();

                //start the tests...
                //sc = new ScannerClass();
                //System.Threading.Thread scThread = new System.Threading.Thread(sc.scanAction1D);                
                //startedThreads.Add(scThread);
                //scThread.Start();

                if (mySettings.bSCANNERenabled)
                {
                    addLog("Starting Scanner thread...");
                    sc2 = new ScannerClass2();
                    sc2.iScanAttempts = mySettings.iScanCount;

                    System.Threading.Thread scThread2 = new Thread(sc2.doWork);
                    scThread2.Name = sc2.name;
                    startedThreads.Add(scThread2);
                    scThread2.IsBackground = false;
                    scThread2.Start();
                    addLog("Scanner thread started");
                }
                if (mySettings.bWLANenabled)
                {
                    addLog("Starting Wifi thread...");
                    wifi = new WiFiClass();
                    wifi.testTime = TimeSpan.FromMinutes( mySettings.iWlanDuration);

                    System.Threading.Thread wifiThread = new Thread(wifi.doWork);
                    wifiThread.Name = wifi.name;
                    startedThreads.Add(wifiThread);
                    wifiThread.IsBackground = false;
                    wifiThread.Start();
                    addLog("Wifi thread started");
                }
#if USE_WWAN
                if (mySettings.bWWANenabled)
                {
                    addLog("Starting WWAN thread...");
                    wwan = new WWANClass();
                    wwan.testInterval = TimeSpan.FromMinutes(mySettings.iWWANinterval);
                    wwan.sWWANfile = mySettings.sWWANfile;

                    System.Threading.Thread wwanThread = new Thread(wwan.doWork);
                    wwanThread.Name = wwan.name;
                    startedThreads.Add(wwanThread);
                    wwanThread.IsBackground = false;
                    wwanThread.Start();
                    addLog("WWAN thread started");
                }
#endif
                if (mySettings.bBKLenabled)
                {
                    addLog("Starting Backlight thread...");
                    bkl = new BKLClass();
#if DEBUG                    
                    bkl.OnInterval = TimeSpan.FromMinutes(1);
                    bkl.OffInterval = TimeSpan.FromMinutes(1);
#else
                    bkl.OnInterval = TimeSpan.FromMinutes(mySettings.iBklOnInterval);
                    bkl.OffInterval = TimeSpan.FromMinutes(60 - bkl.OnInterval.Minutes);// mySettings.iBklOffInterval);
#endif
                    bkl.iBacklightDefault = mySettings.iBacklight;

                    System.Threading.Thread bklThread = new Thread(bkl.doWork);
                    bklThread.Name = bkl.name;
                    startedThreads.Add(bklThread);
                    bklThread.IsBackground = false;
                    bklThread.Start();
                    addLog("Backlight thread started");
                }

                timerUpdate.Enabled = true;
                timerThreadState.Enabled = true;
            }//if not _bIsRunning
            else
            {
                timerUpdate.Enabled = false;
                timerThreadState.Enabled = false;

                mnuStart.Text = "Start";
                bIsRunning = false;
                addLog("Test stopped");
                addLog("Battlog stopping");
                _bStopThreads = true;
                //stop the test...
                stopThreads();
                mnuOptions.Enabled = true;
                restoreSettings();
                addLog("Settings restored");
            }
        }

        private void startCameraThread()
        {
#if USE_CAMERA
            if (mySettings.bCAMERAenabled)
            {
                addLog("Starting Camera thread...");
                camera = new CameraThreadClass3();
                System.Threading.Thread cameraThread = new Thread(camera.doWork);
                cameraThread.Name = camera.name;
                startedThreads.Add(cameraThread);
                cameraThread.IsBackground = false;
                cameraThread.Start();
                addLog("Camera thread started");
            }
#endif
        }
        //never called from background thread
        public void Cam_SnapshotEvent(object sender, Camera.SnapshotArgs sna)
        {
            if (sna.Status != Camera.SnapshotStatus.Ok)
            {
                System.Diagnostics.Debug.WriteLine("Snapshot failed");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Snapshot done to:'" + sna.Filename + "'");
            }
        }
        private void mnuTest_Click(object sender, EventArgs e)
        {
            //string test = LoggingClass.getNewFile();
            Display.SwitchBackLight(false);
            //Display.enableBacklight(false);
            System.Threading.Thread.Sleep(2000);
            Display.SwitchBackLight(true);
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            CameraThreadClass3 camera = new CameraThreadClass3();
            System.Threading.Thread camThread = new Thread(camera.doWork);
            camThread.Start();
            System.Threading.Thread.Sleep(20000);
            camThread.Abort();            
        }

        private void mnuOptions_Click(object sender, EventArgs e)
        {
            FormOptions dlg = new FormOptions();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                mySettings = dlg.mySettings;
            }
            TimeSpan ts = TimeSpan.FromMinutes( mySettings.iWlanDuration);
            ts = TimeSpan.FromMinutes(mySettings.iWWANinterval);
            dlg.Dispose();
        }

        private void PowerChallenge8_Closed(object sender, EventArgs e)
        {
            Intermec.Utils.Display.SwitchBackLight(true);
        }

        private void PowerChallenge8_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter || e.KeyCode==Keys.Escape)
                Intermec.Utils.Display.SwitchBackLight(true);
        }
    }
}