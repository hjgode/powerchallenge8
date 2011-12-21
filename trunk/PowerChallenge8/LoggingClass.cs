using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PowerChallenge
{
    public static class LoggingClass
    {
        #region logging
        [DllImport("coredll.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        private const string _sLogFile = @"\powerchallenge.Log.txt";
        delegate void SetTextCallback(string text);
        public static void addLog(string text)
        {
            DateTime dt = DateTime.Now;
            string timeString = dt.ToLongTimeString();
            text = timeString + ": " + text;
            fileLog(text + "\r\n");
            if(System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(text);
        }

        private static ulong getFreeDiskSpace()
        {
            ulong ulFreeBytes, ulTotalBytes, ulTotalFreeBytes;
            if (GetDiskFreeSpaceEx("\\", out ulFreeBytes, out ulTotalBytes, out ulTotalFreeBytes))
                return ulFreeBytes;
            else
                return 0;
        }

        public static bool newLog()
        {
            bool bRet = true;
            try
            {
                if (System.IO.File.Exists(_sLogFile))
                {
                    System.IO.File.Delete(_sLogFile);
                }
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }

        public static void fileLog(string s)
        {
            try
            {
                //cut log file size
                System.IO.FileInfo fi = new System.IO.FileInfo(_sLogFile);
                if (fi.Exists)
                {
                    if (fi.Length > 1000000)
                    {
                        if (getFreeDiskSpace() > 2200000)
                            System.IO.File.Copy(_sLogFile, getNewFile());
                        System.IO.File.Delete(_sLogFile);
                    }
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

        public static void clearLogFiles()
        {
            try
            {
                string[] sFiles = sLogFiles;
                foreach (string sFile in sFiles)
                {
                    System.IO.File.Delete(sFile);
                }
            }
            catch (Exception)
            {
            }
        }

        private static string[] sLogFiles
        {
            get
            {
                string sBaseName = System.IO.Path.GetFileNameWithoutExtension(_sLogFile);
                string[] filePaths = System.IO.Directory.GetFiles(@"\", sBaseName + ".*");
                return filePaths;
            }
        }
        public static string getNewFile()
        {
            string sRet = _sLogFile;
            try
            {
                if (!System.IO.File.Exists(_sLogFile))
                    return _sLogFile;
            }
            catch (Exception)
            {
                
            }
            string sBaseName = System.IO.Path.GetFileNameWithoutExtension(_sLogFile);
            string[] filePaths = System.IO.Directory.GetFiles(@"\", sBaseName + ".*");
            List<string> sExtensions = new List<string>();
            foreach (string s in filePaths)
            {
                /*
                \powerchallenge.Log.txt
                \powerchallenge.Log.txt.old 
                */
                System.Diagnostics.Debug.WriteLine(s);
                string sExt = System.IO.Path.GetExtension(s);
                if(!sExt.Equals(".txt",StringComparison.OrdinalIgnoreCase))
                    sExtensions.Add(sExt.Substring(1)); //extension is .xyz, remove the dot!
            }
            List<int> iExt = new List<int>();
            int iMax = 0;
            foreach (string sNr in sExtensions)
            {
                try
                {
                    iExt.Add(int.Parse(sNr));
                    if(int.Parse(sNr)>iMax)
                        iMax=int.Parse(sNr);
                }
                catch (Exception)
                {
                }
            }
            string newExtension = iMax++.ToString("000");
            string newFile = sBaseName + "." + newExtension;
            sRet = newFile;
            return sRet;
        }

        #endregion
    }
}
