using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PowerChallenge
{
    [Serializable]
    public class PowerSettings
    {
        private string _sSettingsFile = "\\powerchallenge8.xml";
        public string sSettingsFile
        {
            get { return _sSettingsFile; }
            
        }
        private bool _bWWANenabled = true;
        public bool bWWANenabled
        {
            get { return _bWWANenabled; }
            set { _bWWANenabled = value; }
        }
        private bool _bBKLenabled = true;
        public bool bBKLenabled
        {
            get { return _bBKLenabled; }
            set { _bBKLenabled = value; }
        }
        private bool _bWLANenabled = true;
        public bool bWLANenabled
        {
            get { return _bWLANenabled; }
            set { _bWLANenabled = value; }
        }
        private bool _bSCANNERenabled = true;
        public bool bSCANNERenabled
        {
            get { return _bSCANNERenabled; }
            set { _bSCANNERenabled = value; }
        }
        private bool _bCAMERAenabled = true;
        public bool bCAMERAenabled
        {
            get { return _bCAMERAenabled; }
            set { _bCAMERAenabled = value; }
        }

        private int _iScanCount = 500;
        public int iScanCount
        {
            get { return _iScanCount; }
            set { _iScanCount = value; }
        }

        private string _sWWANfile = "http://www.hjgode.de/temp/10kfile.hex";
        public string sWWANfile
        {
            get { return _sWWANfile; }
            set { _sWWANfile = value; }
        }

        private bool _enableBT = true;
        public bool enableBT
        {
            get { return _enableBT; }
            set { _enableBT = value; }
        }
        private bool _enableWLAN = true;
        public bool enableWLAN
        {
            get { return _enableWLAN; }
            set { _enableWLAN = value; }
        }
        private bool _enableWWAN = true;
        public bool enableWWAN
        {
            get { return _enableWWAN; }
            set { _enableWWAN = value; }
        }
        private bool _enableBKL = true;
        /// <summary>
        /// enable Backlight autocontrol
        /// </summary>
        public bool enableBKL
        {
            get { return _enableBKL; }
            set { _enableBKL = value; }
        }
        private int _iBacklight=1;
        public int iBacklight
        {
            get { return _iBacklight; }
            set
            {
                if (value <= 5 && value >= 1)
                    _iBacklight = value;
            }
        }
        private TimeSpan _iWlanDuration = TimeSpan.FromMinutes(60); //minutes !!
        /// <summary>
        /// minutes of WLAN usage
        /// </summary>
        public int iWlanDuration
        {
            get { 
                return (int)_iWlanDuration.TotalMinutes;// _iWlanDuration.Hours * 60 + _iWlanDuration.Minutes; 
            }
            set { _iWlanDuration = TimeSpan.FromMinutes(value); }
        }

        private TimeSpan _iWWANinterval = TimeSpan.FromMinutes(5); //minutes
        /// <summary>
        /// Minute interval of WWAN access
        /// </summary>
        public int iWWANinterval
        {
            get { return (int)_iWWANinterval.TotalMinutes; }
            set { _iWWANinterval = TimeSpan.FromMinutes(value); }
        }
        
        private TimeSpan _iBklOffinterval = TimeSpan.FromMinutes(55); //minutes
        /// <summary>
        /// Minute interval of BKL (Backlight) access
        /// </summary>
        public int iBklOffInterval
        {
            get { return (int)_iBklOffinterval.TotalMinutes; }
            set { _iBklOffinterval = TimeSpan.FromMinutes(value); }
        }
        private TimeSpan _iBklOninterval = TimeSpan.FromMinutes(5); //minutes
        /// <summary>
        /// Minute interval of BKL (Backlight) access
        /// </summary>
        public int iBklOnInterval
        {
            get { return (int)_iBklOninterval.TotalMinutes; }
            set { _iBklOninterval = TimeSpan.FromMinutes(value); }
        }

        private int _iCameraCount = 30;
        public int iCameraCount
        {
            get { return _iCameraCount; }
            set { _iCameraCount = value; }
        }
        public PowerSettings()
        {
            string AppPath;
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!AppPath.EndsWith(@"\"))
                AppPath += @"\";
            _sSettingsFile = AppPath + "powerchallenge8.xml";
        }

    }
}
