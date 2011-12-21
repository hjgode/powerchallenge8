using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.IO;
using System.Net;

namespace PowerChallenge
{
    /// <summary>
    /// start WWAN
    /// run a file transfer of 10K every 5 Minutes
    /// </summary>
    public class WWANClass : StopableThreadClass
    {
#if DEBUG
        private TimeSpan _testInterval= new TimeSpan(0,0,30);
#else
      private TimeSpan _testInterval= new TimeSpan(0,5,0);
#endif
        public TimeSpan testInterval
        {
            get { return _testInterval; }
            set { _testInterval = value; }
        }
        private string _sWWANfile="http://www.hjgode.de/temp/10kfile.hex";
        public string sWWANfile
        {
            get { return _sWWANfile; }
            set { _sWWANfile = value; }
        }

        private DateTime dtStart;
        public int iCount = 0;

        public WWANClass()
        {
            this.name = "WWAN thread";
            this.Run();
        }
        public new void doWork()
        {
            LoggingClass.addLog("Starting WWAN Thread");
            try
            {
                //switch WWAN on
                if (Intermec.Utils.RadioDrivers.Phone != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
                {
                    LoggingClass.addLog("Power ON WWAN");
                    Intermec.Utils.RadioDrivers.Phone = Intermec.Utils.RadioDrivers.RADIO_STATES.On;
                    Thread.Sleep(5000); //give the modem time to start
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception in WWAN doWork(): '" + ex.Message + "'");
            }
            _bIsRunning = true;
            //ensure a valid WiFi connection is setup
            LoggingClass.addLog("Setting WWAN profile");
            WWANProfileClass.setWWANprofile();
            try
            {
                dtStart = DateTime.Now;
                TimeSpan testDiff;
                do
                {
                    //end test after x time
                    testDiff = DateTime.Now - dtStart;
                    if (testDiff > _testInterval)
                    {
                        //do file transfer (blocking call)
                        LoggingClass.addLog("WWAN: Downloading file...");
                        DownloadFileAsync(@"\downloaded.hex", _sWWANfile);
                        //DownloadFile(@"\downloaded.hex", "http://www.hjgode.de/temp/10kfile.hex");
                        dtStart = DateTime.Now;
                        //LoggingClass.addLog("WWAN: Downloading file finished");
                    }
                    System.Diagnostics.Debug.WriteLine("Thread '" + this.name + "' running");
                    Thread.Sleep(10000);
                    //calc how many seconds are left
                    iCount = (_testInterval - testDiff).Minutes * 60 + (_testInterval - testDiff).Seconds;
                } while (!_bStopThread);
                if (Intermec.Utils.RadioDrivers.WIFI != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
                {
                    Intermec.Utils.RadioDrivers.WIFI = Intermec.Utils.RadioDrivers.RADIO_STATES.Off;
                    LoggingClass.addLog("Power OFF WWAN");
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
            //switch WWAN off
            if (Intermec.Utils.RadioDrivers.Phone != Intermec.Utils.RadioDrivers.RADIO_STATES.Unavailable)
            {
                LoggingClass.addLog("Power OFF WWAN");
                Intermec.Utils.RadioDrivers.Phone = Intermec.Utils.RadioDrivers.RADIO_STATES.Off;
                Thread.Sleep(5000); //give the modem time to stop
            }
            _bIsRunning = false;
            StopDownload();
            LoggingClass.addLog("Leaving WWAN Thread");
        }

#region async download
        private HttpWebRequest m_req;
        private HttpWebResponse m_resp;
        private MemoryStream tempStream;
        private FileStream m_fs;

        string m_localFile = @"\downloaded.hex";
        // Data buffer for stream operations
        private byte[] dataBuffer;
        private const int DataBlockSize = 1000;
        private int pbVal, maxVal;
        private void DownloadFileAsync(string localFile, string downloadUrl)
        {
            LoggingClass.addLog("WWAN: DownloadFileAsync");
            m_req = (HttpWebRequest)HttpWebRequest.Create(downloadUrl);
            tempStream = new MemoryStream();
            m_req.BeginGetResponse(new AsyncCallback(ResponseReceived), null);
        }
        private void StopDownload()
        {
            LoggingClass.addLog("WWAN: StopDownload");
            if(m_req!=null)
                m_req.Abort();
            if (m_fs != null)
            {
                m_fs.Close();
                m_fs = null;
            }
        }
        void ResponseReceived(IAsyncResult res)
        {
            LoggingClass.addLog("WWAN: ResponseReceived");
            try
            {
                m_resp = (HttpWebResponse)m_req.EndGetResponse(res);
            }
            catch (WebException ex)
            {
                LoggingClass.addLog("ResponseReceived WebException: " + ex.Message);
                return;
            }
            catch (ArgumentNullException ex)
            {
                LoggingClass.addLog("ResponseReceived ArgumentException: " + ex.Message);
                return;
            }
            catch (ArgumentException ex)
            {
                LoggingClass.addLog("ResponseReceived ArgumentException: " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                LoggingClass.addLog("ResponseReceived Exception: " + ex.Message);
                return;
            }
            // Allocate data buffer
            dataBuffer = new byte[DataBlockSize];
            // Set up progrees bar
            maxVal = (int)m_resp.ContentLength;
            //pbProgress.Invoke(new EventHandler(SetProgressMax));
            
            // Open file stream to save received data
            m_fs = new FileStream(m_localFile, FileMode.Create);
            tempStream = new MemoryStream();
            
            // Request the first chunk
            m_resp.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);
        }

        void OnDataRead(IAsyncResult res)
        {
            LoggingClass.addLog("WWAN: OnDataRead");
            // How many bytes did we get this time
            int nBytes = m_resp.GetResponseStream().EndRead(res);
            
            // Write buffer
            //m_fs.Write(dataBuffer, 0, nBytes);
            tempStream.Write(dataBuffer, 0, nBytes);

            // Update progress bar using Invoke()
            pbVal += nBytes;
            LoggingClass.addLog("WWAN: read " + pbVal.ToString()+"/"+maxVal.ToString());
            //pbProgress.Invoke(new EventHandler(UpdateProgressValue));
            // Are we done yet?
            if (nBytes > 0)
            {
                // No, keep reading
                m_resp.GetResponseStream().BeginRead(dataBuffer, 0, DataBlockSize, new AsyncCallback(OnDataRead), this);
            }
            else
            {
                // Yes, perform cleanup and update UI.
                m_fs.Write(tempStream.ToArray(), 0, tempStream.ToArray().Length - 1);

                tempStream.Close(); tempStream = null;
                m_fs.Close();
                m_fs = null;
                //this.Invoke(new EventHandler(this.AllDone));
            }
        }
#endregion
        public void DownloadFile(string localFile, string downloadUrl)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(downloadUrl);
            req.Method = "GET";
            req.Timeout = 10000; //set to timeout after 10 seconds
            HttpWebResponse resp;
            Stream respStream;
            //StreamReader rdr=null;
            BinaryReader rdr = null;
            StreamWriter wrtr=null; 
            try
            {
                resp = (HttpWebResponse)req.GetResponse();

                // Retrieve response stream and wrap in StreamReader
                respStream = resp.GetResponseStream();
                //rdr = new StreamReader(respStream);
                rdr = new BinaryReader(respStream);

                // Create the local file
                wrtr = new StreamWriter(localFile);
                

                // loop through response stream reading each line 
                //  and writing to the local file
                
                //string inLine = rdr.ReadLine(); //rdr.Read();
                byte[] inBuffer = new byte[1024];
                int iReadCount = rdr.Read(inBuffer, 0, inBuffer.Length);
                
                //while (inLine != null)
                while(iReadCount>0)
                {
                    //wrtr.WriteLine(inLine);
                    wrtr.Write(inBuffer);
                    //inLine = rdr.ReadLine();
                    iReadCount = rdr.Read(inBuffer, 0, inBuffer.Length);
                }

            }
            catch (ArgumentNullException ex)
            {
                LoggingClass.addLog("DownloadFile ArgumentException: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                LoggingClass.addLog("DownloadFile ArgumentException: " + ex.Message);
            }
            catch (Exception ex)
            {
                LoggingClass.addLog("DownloadFile Exception: " + ex.Message);
            }
            finally
            {
                if(rdr!=null)
                    rdr.Close();
                if(wrtr!=null)
                    wrtr.Close();
            }
        }
    }
}
