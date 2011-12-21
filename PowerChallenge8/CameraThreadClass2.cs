using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Wimo.Common.Device;

namespace PowerChallenge
{
    class CameraThreadClass2:StopableThreadClass
    {
        Camera cc2;
#if DEBUG
        public int iPhotoCountMax = 3;
#else
        public int iPhotoCountMax = 30;
#endif
        public int iCount = 0;
        public System.Windows.Forms.Form _fOwner = null;
        private int iLoopCount=0;

        public CameraThreadClass2()
        {
            this._fOwner = null;
            this.name = "Camera2 Thread";
            //this.Run();
        }
        public CameraThreadClass2(System.Windows.Forms.Form fOwner)
        {
            this._fOwner = fOwner;
            this.name = "Camera2 Thread";
            //this.Run();
        }

        public new bool _bStopThread = false;
        public new bool _bIsRunning = false;

        public new void doWork()
        {
            _bIsRunning = true;
            LoggingClass.addLog("Starting Camera2 Thread");

            try
            {
                iLoopCount = 0;
                do
                {
                    do
                    {
                        if (this._bStopThread)
                            break; 
                        System.Diagnostics.Debug.WriteLine("Thread '" + this.name + "' running");
                        //end test after x time
                        iCount = iPhotoCountMax - iLoopCount;
                        System.Diagnostics.Debug.WriteLine("Photo count = " + iLoopCount.ToString());
                        LoggingClass.addLog("Photo count = " + iLoopCount.ToString());

                        if (iLoopCount > iPhotoCountMax)
                        {
                            System.Diagnostics.Debug.WriteLine("Maximum photo count reached");
                            _bStopThread = true;
                            throw new Exception("Maximum photo count reached");
                        }


                        LoggingClass.addLog("Trying to do a Photo...");
                        cc2 = new Camera();
                        CameraResolution resCamera = Camera.GetLowestCameraResolution();
                        if (resCamera != null)
                            cc2.Open(resCamera);
                        else
                            cc2.Open();
                        System.Diagnostics.Debug.WriteLine("Camera opened.");
                        cc2.TakePicture();
                        int maxWait = 10, iWait = 0; //wait max 10 seconds
                        do
                        {
                            System.Diagnostics.Debug.WriteLine("Waiting for Camera picture taken...");
                            Thread.Sleep(1000);
                            iWait++;
                        } while (cc2.IsTakingPicture && iWait != maxWait);
                        System.Diagnostics.Debug.WriteLine("Camera picture wait done.");

                        if (cc2.IsOpenned)
                            cc2.Close();
                        LoggingClass.addLog("Photo done");
                        Thread.Sleep(1000);
                        iLoopCount++;
                    } while (iLoopCount <= iPhotoCountMax);
                }
                while (!_bStopThread);
            }
            catch (ThreadAbortException ex)
            {
                LoggingClass.addLog("ThreadAbortException '" + ex.Message + "' in " + name);
            }
            catch (Exception ex)
            {
                LoggingClass.addLog("Exception '" + ex.Message + "' in " + name);
            }
            LoggingClass.addLog("Leaving Camera2 Thread");
            _bIsRunning = false;
        }

    }
}
