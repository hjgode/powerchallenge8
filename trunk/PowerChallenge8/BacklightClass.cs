using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;

using Intermec.Utils;

namespace PowerChallenge
{
    /// <summary>
    /// start Backlight test
    /// turn bkl ON for a period
    /// </summary>
    public class BKLClass : StopableThreadClass
    {
#if DEBUG
        private TimeSpan _OnInterval = new TimeSpan(0, 1, 0);
#else
      private TimeSpan _OnInterval = new TimeSpan(0,5,0);
#endif

        public TimeSpan OnInterval
        {
            get { return _OnInterval; }
            set {   
                    _OnInterval = value;
                }
        }

#if DEBUG
        private TimeSpan _OffInterval = new TimeSpan(0, 1, 0);
#else
      private TimeSpan _OffInterval = new TimeSpan(0,5,0);
#endif

        public TimeSpan OffInterval
        {
            get { return _OffInterval; }
            set {   
                    _OffInterval = value;
                }
        }

        private int _iBacklightDefault = 1;
        public int iBacklightDefault
        {
            set { _iBacklightDefault = value; }
            get { return _iBacklightDefault; }
        }

        
        public int iCount = 0;

        public BKLClass()
        {
            this.name = "Backlight thread";
            this._iBacklightDefault = Display.GetBackLightLevel();
            
            //this.Run();
        }
        public new void doWork()
        {
            int iMinuteCounter = 0;
            LoggingClass.addLog("Starting Backlight Thread");
            try
            {
                //switch Backlight to base level
                Display.SetBackLightLevel(_iBacklightDefault);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in Backlight doWork(): '" + ex.Message + "'");
            }
            _bIsRunning = true;
            //toggle Backlight for xx minutes to OFF and then back ON
            try
            {
                do
                {
                    do
                    {
                        //bkl ON
                        if (toggleBacklight(true))
                            LoggingClass.addLog("BKL: switched ON");
                        System.Diagnostics.Debug.WriteLine("Thread '" + this.name + "' running. BKL on");
                        Thread.Sleep(60000); //sleep a minute
                        iMinuteCounter++; //
                        //calc how many seconds are left
                        iCount = _OnInterval.Minutes - iMinuteCounter + 1;
                    } while (iMinuteCounter < _OnInterval.Minutes && !_bStopThread); //ie run for 5 minutes

                    //reset counter
                    iMinuteCounter = 0;

                    do
                    {
                        //bkl OFF
                        if (toggleBacklight(false))
                            LoggingClass.addLog("BKL: switched OFF");
                        System.Diagnostics.Debug.WriteLine("Thread '" + this.name + "' running. BKL off");
                        Thread.Sleep(60000); //sleep a minute
                        iMinuteCounter++; //
                        //calc how many seconds are left
                        iCount = _OffInterval.Minutes - iMinuteCounter + 1;
                    } while (iMinuteCounter < _OffInterval.Minutes && !_bStopThread); //ie run for 55 minutes

                } while (!_bStopThread);
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
                //switch BKL ON
                toggleBacklight(true);
                //Display.SetBackLightLevel(_iBacklightDefault);
                _bIsRunning = false;
            }
            LoggingClass.addLog("Leaving Backlight Thread");
        }

        private bool _BacklightState = true;
        /// <summary>
        /// set new backlightstate and set Backlight ON/OFF
        /// </summary>
        /// <param name="newBacklightState"></param>
        /// <returns>true, if backlight sate changed
        /// false if </returns>
        private bool toggleBacklight(bool newBacklightState)
        {
            if (newBacklightState == _BacklightState)
                return false;
            else
            {
                _BacklightState = newBacklightState;
                if (_BacklightState)
                {
                    Display.SetBackLightLevel(_iBacklightDefault);
                    Display.SwitchBackLight(true);
                }
                else
                {
                    Display.SwitchBackLight(false);
                    Display.SetBackLightLevel(0);
                }
                return true;
            }
        }
    }
}
