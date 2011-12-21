using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace PowerChallenge
{
    class WiFiClass:StopableThreadClass
    {
        public int iCount = 0;
        private TimeSpan _testTime;
        public TimeSpan testTime
        {
            get { return _testTime; }
            set { _testTime = value; }
        }
        private DateTime dtStart;
        public new void doWork()
        {
            _bIsRunning = true;
            LoggingClass.addLog("Starting WiFi Thread");
            //switch WLAN off
            if (Intermec.Utils.RadioDrivers.WIFI != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
            {
                LoggingClass.addLog("Power OFF WLAN");
                Intermec.Utils.RadioDrivers.WIFI = Intermec.Utils.RadioDrivers.RADIO_STATES.Off;
            }
            //ensure a valid WiFi connection is setup
            LoggingClass.addLog("Setting WLAN profile");
            WlanProfileClass.setWLANprofile();
            //switch WLAN on
            if (Intermec.Utils.RadioDrivers.WIFI != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
            {
                LoggingClass.addLog("Power ON WLAN");
                Intermec.Utils.RadioDrivers.WIFI = Intermec.Utils.RadioDrivers.RADIO_STATES.On;
            }

            try
            {
                dtStart = DateTime.Now;
                TimeSpan testDiff;
                do
                {
                    //end test after x time
                    testDiff=DateTime.Now - dtStart ;
                    if (testDiff > _testTime)
                        _bStopThread = true;

                    System.Diagnostics.Debug.WriteLine("Thread '" + this.name + "' running");
                    Thread.Sleep(10000);
                    //calc how many seconds are left
                    iCount = (_testTime - testDiff).Minutes * 60 + (_testTime - testDiff).Seconds; 
                } while (!_bStopThread);

                if (Intermec.Utils.RadioDrivers.WIFI != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
                {
                    Intermec.Utils.RadioDrivers.WIFI = Intermec.Utils.RadioDrivers.RADIO_STATES.Off;
                    LoggingClass.addLog("Power ON WLAN");
                }
            }
            catch (ThreadAbortException ex)
            {
                LoggingClass.addLog("ThreadAbortException '" + ex.Message + "' in " + name);
            }
            catch (Exception ex)
            {
                LoggingClass.addLog("Exception '" + ex.Message + "' in " + name);
            }
            LoggingClass.addLog("Leaving WiFi Thread");
            _bIsRunning = false;
        }
        public WiFiClass()
        {
#if DEBUG
            testTime = new TimeSpan(0, 5, 0); //5 minutes test
#else
            _testTime = new TimeSpan(1, 0, 0); //one hour test
#endif
            this.name = "WiFi Thread";
            this.Run();
        }
    }
}
