using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Drawing;

namespace Wimo.Common.Device
{
    /// <summary>
    /// Specifies a camera resolution
    /// </summary>
    public class CameraResolution
    {
        int id;
        /// <summary>
        /// Id of the resolution
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }


        int width;
        /// <summary>
        /// Width component of the resolution
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        int height;
        /// <summary>
        /// Height component of the resolution
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
    }

    /// <summary>
    /// Class that represents a camera on the device
    /// </summary>
    public class Camera
    {
        IntPtr hCamera = IntPtr.Zero;

        /// <summary>
        /// Is the camera currently opened and being used?
        /// </summary>
        public bool IsOpenned
        {
            get { return hCamera != IntPtr.Zero; }
        }

        bool isTakingPicture = false;
        /// <summary>
        /// Is the camera taking pictures?
        /// </summary>
        public bool IsTakingPicture
        {
            get { return isTakingPicture; }
        }

        /// <summary>
        /// Opens the camera on the device and prepares for it to be used.  
        /// </summary>
        public void Open()
        {
            OpenCamera(0, ref hCamera);
        }

        /// <summary>
        /// Opens the camera on device and prepares for it to be used using the 
        /// specified resolution.
        /// </summary>
        /// <param name="cameraResolution">Resolution to set the camera to</param>
        public void Open(CameraResolution cameraResolution)
        {
            OpenCamera(cameraResolution.Id, ref hCamera);
        }

        /// <summary>
        /// Close the camera object.  This will free up the native resources
        /// </summary>
        public void Close()
        {
            // make sure we have a handle to the native camera object
            // before we try to really close it
            if (hCamera != IntPtr.Zero)
            {
                CloseCamera(hCamera);
            }
        }

        /// <summary>
        /// Takes a snapshot from the camera and returns it as a Bitmap
        /// </summary>
        /// <remarks>This implementation is slow.  When I have time, I will
        /// look into writing my own custom DirectShow filter so that we don't have
        /// to save the image to flash before we get it via the API.  Right now this uses the stock
        /// ImageSink Filter.</remarks>
        /// <returns>Bitmap representing the snapshot taken by the camera</returns>
        public Bitmap TakePicture()
        {
            isTakingPicture = true;
            Bitmap bmp = null;

            try
            {
                // we don't want a really tight loop, so we will sleep 100 milliseconds if we don't 
                // find the snapshot file.
                int sleepTime = 100;

                // if 30 seconds goes by, just give up.
                int maxTimeOut = 30000;

                // for now i save the snapshop to a file.  It would be best 
                // to switch this to a more "temporary" file.  
                string path = "\\wimocamera.jpg";
                if (File.Exists(path))
                {
                    File.Copy(path, path + ".jpg", true);
                    File.Delete(path);
                }

                // this is async, so we have to poll for the camera.jpg
                TakePicture(hCamera, path);

                int timer = 0;
                FileInfo info = new FileInfo(path);

                // we've told the native camera object to take a snapshot, but it 
                // takes time to save the picture to the flash.  So we will poll 
                // for the file to be completed.  
                while (bmp == null &&
                    timer < maxTimeOut)
                {
                    // We sleep right away because there is almost no way that the file will have been
                    // written by this time. 
                    Thread.Sleep(sleepTime);
                    timer += sleepTime;

                    // if we found our file
                    if (File.Exists(path))
                    {
                        try
                        {
                            // let's load it from the flash
                            bmp = new Bitmap(path);
                        }
                        catch (IOException)
                        {
                            // the file is there, but is still opened for write.  This means it is
                            // currently being written... we'll just ignore this and wait another round for the file
                            // to be available.
                        }
                        catch (ObjectDisposedException)
                        {
                            // the file is there, but is still opened for write.  This means it is
                            // currently being written... we'll just ignore this and wait another round for the file
                            // to be available.
                        }
                    }
                }

                // if it exists, we'll cleanup after ourselves
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
                // fail
                bmp = null;
            }

            isTakingPicture = false;
            return bmp;
        }

        //HGO
        public static CameraResolution GetLowestCameraResolution()
        {
            CameraResolution lowest = new CameraResolution();
            lowest.Width = 0;
            List<CameraResolution> list = new List<CameraResolution>();
            list = GetAvailableResolutions();
            int lastWidth = 10000;
            foreach (CameraResolution cRes in list)
            {
                if (cRes.Width < lastWidth)
                {
                    lastWidth = cRes.Width;
                    lowest = cRes;
                }
            }
            if (lowest.Width == 0)
                return null;//soemthing really went wrong
            return lowest;
        }
        //HGO end
        /// <summary>
        /// Returns a list of all of the possible camera resolutions supported
        /// by the device's camera.  
        /// <remarks>You can call this first to get the supported resolutions and then
        /// call the Camera.Open method to open the camera to the resolution you'd like.</remarks>
        /// </summary>
        /// <returns>List of CameraResolutions</returns>
        public static List<CameraResolution> GetAvailableResolutions()
        {
            List<CameraResolution> list = new List<CameraResolution>();
            int size = 0;
            IntPtr ptr;
            GetCameraResolutions(IntPtr.Zero, ref size);
            ptr = Marshal.AllocHGlobal(size);

            GetCameraResolutions(ptr, ref size);

            // the pointer is an array of 2 ints.  so the size / 8 is the number
            // of resolutions supported
            for (int index = 0; index < size / 8; index++)
            {
                CameraResolution res = new CameraResolution();
                res.Width = Marshal.ReadInt32(ptr, index * 8);
                res.Height = -Marshal.ReadInt32(ptr, index * 8 + 4);
                res.Id = index;
                list.Add(res);
            }

            Marshal.FreeHGlobal(ptr);
            return list;
        }

        // These are the pInvokes into the native library that does all
        // of the real work


        [DllImport("WiMoNative.dll")]
        private static extern int OpenCamera(int id, ref IntPtr hCamera);

        [DllImport("WiMoNative.dll")]
        private static extern int CloseCamera(IntPtr hCamera);

        [DllImport("WiMoNative.dll")]
        private static extern int TakePicture(IntPtr hCamera, string fileName);

        [DllImport("WiMoNative.dll")]
        private static extern int GetCameraResolutions(IntPtr pResolutions, ref int iResolutionsSize);
    }
}
