using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

//using Wimo.Common.Device;
using Intermec.Multimedia;

namespace PowerChallenge
{
    class CameraThreadClass3:StopableThreadClass
    {
#if DEBUG
        public int iPhotoCountMax = 3;
#else
        public int iPhotoCountMax = 30;
#endif
        public int iCount = 0;
        private int iLoopCount=0;

        public CameraThreadClass3()
        {
            this.name = "Camera3 Thread";
            //this.Run();
        }

        public new bool _bStopThread = false;
        public new bool _bIsRunning = false;

        Intermec.Multimedia.Camera Cam;
        Camera.Resolution CurrentResolution;
        private string _snapshotDir = "\\My Documents\\My Pictures";

        private int initCamera()
        {
            // setup camera
            try
            {
                    Cam = new Camera(Camera.ImageResolutionType.Lowest); //this.pictureBox1, Camera.ImageResolutionType.Lowest);
            }
            catch (Intermec.Multimedia.CameraException ex)
            {
                System.Diagnostics.Debug.WriteLine("initCamera failed: " + ex.Message);
                return -1;        
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("initCamera failed: " + ex.Message);
                return -2;        
            }

            Cam.Streaming = true;

            // full sreen mode?
                Cam.PictureBoxUpdate = Camera.PictureBoxUpdateType.None;// Camera.PictureBoxUpdateType.FullScreen;

            // default location for snapshots
            try
            {
                Cam.SnapshotFile.Directory = _snapshotDir;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("initCamera failed: " + ex.Message);
                return -3;
            }

            // default snapshot filename
            Cam.SnapshotFile.Filename = "SnapShot";

            // use incremental numbering in filename
            Cam.SnapshotFile.FilenamePadding = Camera.FilenamePaddingType.IncrementalCounter;

            // by default, make JPG snapshots with 80 quality
            Cam.SnapshotFile.ImageFormatType = Camera.ImageType.JPG;
            Cam.SnapshotFile.JPGQuality = 80;


            // if there is a compass on the system, use it
            // (Requires te SensorCab to be installed)
            Cam.ImprintCompassPos = Camera.ImprintCompassPosType.Disabled;// Camera.ImprintCompassPosType.UpperRight;

            // hook snapshot event
                Cam.SnapshotEvent += new SnapshotEventHandler(Cam_SnapshotEvent);

            //hook motion detect event
            //Cam.MotionDetectionEvent += new MotionDetectionEventHandler(Cam_MotionDetectionEvent);
            //Cam.MotionDetection.Visualize = true;
            //Cam.MotionDetection.GridDifference = 10;

            // imprint the word 'caption' in the upperleft corner
            Cam.ImprintCaptionPos = Camera.ImprintCaptionPosType.UpperLeft;
            Cam.ImprintCaptionString = "PowerChallenge8";

            Cam.ImprintDateTimePos = Camera.ImprintDateTimePosType.LowerLeft;

            Cam.DisplayCameraInfo = false;

            // if whitebalance is available from the camera, use it
            //this.miWhiteBalance.Enabled = Cam.Features.WhiteBalance.Available;

            // if flash is available from the camera, set it up
            if (Cam.Features.Flash.Available == true)
            {
                Cam.Features.Flash.PresetValue = Camera.Feature.FlashFeature.FlashPresets.On;
                //this.miFlash.Enabled = true;
                //if (Cam.Features.Flash.PresetValue == Camera.Feature.FlashFeature.FlashPresets.On)
                //{
                //    this.miFlash.Checked = true;
                //}
                //else
                //{
                //    this.miFlash.Checked = false;
                //}
            }
            else
            {
                //this.miFlash.Enabled = false;
            }

            // hook the KeyDown event for snapshots
            //this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
            //this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            //this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            //setup the resolutions.
            //MenuItem[] resolution_menu = new MenuItem[Cam.AvailableImageResolutions.Length];
            Camera.Resolution[] AvalRes = new Camera.Resolution[Cam.AvailableImageResolutions.Length];
            Camera.Resolution minRes = Cam.CurrentViewfinderResolution;
            int mc = 0;
            foreach (Camera.Resolution res in Cam.AvailableImageResolutions)
            {
                AvalRes[mc] = res;
                //resolution_menu[mc] = new MenuItem();
                //resolution_menu[mc].Text = res.Width.ToString() + "*" + res.Height.ToString();
                //resolution_menu[mc].Click += new EventHandler(Resolution_Click);
                if (AvalRes[mc].Width < minRes.Width)
                    minRes = AvalRes[mc];
                //this.mSnapshot.MenuItems.Add(resolution_menu[mc]);
                //System.Diagnostics.Debug.WriteLine(string.Format("Res {0}: width={1}, height={2}, BPP={3}", mc + 1, AvalRes[mc].Width, AvalRes[mc].Height, AvalRes[mc].BPP));

                mc++;
            }
            CurrentResolution = minRes;
            //resolution_menu[0].Checked = true;

            //SetcheckWhiteBalance();

            Camera.Resolution mytype = minRes;// Cam.CurrentViewfinderResolution;
            //Cam.Streaming = this.mStreaming.Checked;

            //MenuSave = this.Menu;
            //this.Menu = null;
            //this.WindowState = FormWindowState.Maximized;
            return 0;
        }
        bool IsTakingPicture = false;
        //never called from background thread

        //---------------------------------------------------
        // Received snapshot event
        //---------------------------------------------------
        void Cam_SnapshotEvent(object sender, Camera.SnapshotArgs sna)
        {
            if (sna.Status != Camera.SnapshotStatus.Ok)
            {
                System.Diagnostics.Debug.WriteLine("Snapshot failed");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Snapshot done to:'" + sna.Filename+"'");
            }
            IsTakingPicture = false;
        }

        //---------------------------------------------------
        // Workaround as snapshot event is not fired
        //---------------------------------------------------
        /// <summary>
        /// Call once before Camera.SnapShot and then repeat
        /// to get info, if file count changed
        /// </summary>
        private int _lastFileCount;
        private bool isDirChanged(){
            string[] s;
            try
            {
               s = System.IO.Directory.GetFiles(_snapshotDir, "*.jpg");
            }
            catch (Exception)
            {
                System.IO.Directory.CreateDirectory(_snapshotDir);
                s = System.IO.Directory.GetFiles(_snapshotDir, "*.jpg");
            }
            int fileCount = s.Length;
            if (fileCount != _lastFileCount)
            {
                _lastFileCount = fileCount;
                return true;
            }
            else
                return false;
        }

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
                        int iError = initCamera();
                        if (iError == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("initCam OK.");
                            IsTakingPicture = true;
                            System.Diagnostics.Debug.WriteLine("Trying SnapShot()...");
                            isDirChanged(); //init _lastFileCount before Capture
                            Cam.Snapshot(CurrentResolution);

                            int maxWait = 10, iWait = 0; //wait max 10 seconds
                            do
                            {
                                System.Diagnostics.Debug.WriteLine("Waiting for Camera picture taken...");
                                Thread.Sleep(1000);
                                iWait++;
                            } while (IsTakingPicture && iWait != maxWait && !isDirChanged());
                            Cam.Streaming = false;
                            System.Diagnostics.Debug.WriteLine("Camera picture wait done.");
                            Cam.Dispose();
                            IsTakingPicture = false;
                            Cam = null;
                            GC.Collect();
                            LoggingClass.addLog("Photo done");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("initCam failed. Error=" + iError.ToString());
                            LoggingClass.addLog("initCam failed. Error=" + iError.ToString());
                        }
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
            finally
            {
                if (Cam != null)
                {
                    Cam.Streaming = false;
                    Cam.Dispose();
                    Cam = null;
                    GC.Collect();
                }
            }
            LoggingClass.addLog("Leaving camera3 Thread");
            _bIsRunning = false;
        }

    }
}
