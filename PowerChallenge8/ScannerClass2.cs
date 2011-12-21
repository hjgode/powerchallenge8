using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

using Intermec.DataCollection;

using System.Threading;

using NativeSync;

namespace PowerChallenge
{
    class ScannerClass2:StopableThreadClass
    {
        private int _iSuccessScanCount = 0;
#if DEBUG
        private int _iScanAttempts = 10;
#else
        private int _iScanAttempts = 500;
#endif
        public int iScanAttempts
        {
            get { return _iScanAttempts; }
            set { _iScanAttempts = value; }
        }

        public int iCount = 0;

        private Control _control;

        //internal bool _bIsRunning = false;
        //internal bool _bStopThread = false;
        //public string name = "Name";

        public override void doWork()
        {
            _bIsRunning = true;
            _iSuccessScanCount = 0;
            BarcodeReader bcr = null;
            _control = new Control();
            try
            {
                bcr = new BarcodeReader(_control, "default");
                bcr.BarcodeRead += new BarcodeReadEventHandler(bcr_BarcodeRead);
                bcr.ThreadedRead(true);
                //img = new Imager();
                //Imager.PresetMode oldMode = img.Preset;
                //img.Preset = Imager.PresetMode.Standard_1D_Only;
                int iLoop = 1;
                do
                {
                    LoggingClass.addLog("Thread '" + this.name + "' started");

                    //the scanner loop
                    do
                    {
                        if (this._bStopThread)
                            break;
                        iCount = _iScanAttempts - iLoop;
                        if (iCount % 50 == 0)
                            LoggingClass.addLog("Thread '" + this.name + "' running. Attempt/Success=" + iLoop.ToString() + "/" + _iSuccessScanCount.ToString());
                        NativeSync.ScanEvent.fireScanner();

                        //fire the scanner for 100ms, if there is a good scan an event will be fired
                        //bcr.ScannerOn = true;

                        //battery.BatteryStatus.SYSTEM_POWER_STATUS_EX2 batStat = new battery.BatteryStatus.SYSTEM_POWER_STATUS_EX2();
                        //batStat = battery.BatteryStatus.getStatus();
                        System.Threading.Thread.Sleep(5);
                        //addInfo(batStat.ToString());

                        //bcr.ScannerOn = false;

                        System.Threading.Thread.Sleep(500);
                        iLoop++;
                    } while (iLoop <= _iScanAttempts);
                    LoggingClass.addLog("Thread '" + this.name + "' ending. Reached max scanAttempts");
                    _bStopThread = true;
                } while (!_bStopThread);
            }
            catch (BarcodeReaderException ex)
            {
                LoggingClass.addLog("scanAction1D Exception: " + ex.Message);
            }
            catch (ImagerException ex)
            {
                LoggingClass.addLog("scanAction1D Exception: " + ex.Message);
            }
            catch (ThreadAbortException ex)
            {
                LoggingClass.addLog("ThreadAbortException '" + ex.Message + "' in " + name);
            }
            catch (Exception ex)
            {
                LoggingClass.addLog("Exception '" + ex.Message + "' in " + name);
            }
            finally
            {
                //img.Preset = oldMode;
                //img.Dispose();

                //ensure scanner is OFF
                try
                {
                    NativeSync.ScanEvent.fireScanner();
                }
                catch (Exception)
                {
                }
                if (bcr != null)
                {
                    bcr.ThreadedRead(false);
                    bcr.BarcodeRead -= bcr_BarcodeRead;
                    bcr.Dispose();
                    bcr = null;
                }
            }
            this._bIsRunning = false;
        }
        void bcr_BarcodeRead(object sender, BarcodeReadEventArgs bre)
        {
            _iSuccessScanCount++;
        }

        public ScannerClass2()
        {
            this.name = "Scanner thread";
        }


    }
}
