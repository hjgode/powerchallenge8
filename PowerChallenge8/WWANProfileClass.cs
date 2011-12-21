using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Intermec.DeviceManagement.SmartSystem;

namespace PowerChallenge
{
    public static class WWANProfileClass
    {
        public static bool setWWANprofile()
        {
            bool bRet = false;
            ITCSSApi ss;
            ss = new ITCSSApi();
            string sXml = "";

            sXml = "";
            sXml += "<wap-provisioningdoc>";
            sXml += "<characteristic type=\"CM_GPRSEntries\">" + "\r\n";
            sXml += "   <characteristic type=\"My Connection\">" + "\r\n";
            sXml += "                 <parm name=\"DestId\" value=\"{436EF144-B4FB-4863-A041-8F905A62C572}\"/>" + "\r\n";
            sXml += "                 <parm name=\"Phone\" value=\"internet.t-d1.de\"/>" + "\r\n";
            sXml += "                 <parm name=\"UserName\" value=\"internet\"/>" + "\r\n";
            sXml += "                 <parm name=\"Password\" value=\"t-d1\"/>" + "\r\n";
            sXml += "                 <parm name=\"Enabled\" value=\"1\"/>" + "\r\n";
            sXml += "                 <characteristic type=\"DevSpecificCellular\">" + "\r\n";
            sXml += "                     <parm name=\"GPRSInfoAccessPointName\" value=\"internet.t-d1.de\"/>" + "\r\n";
            sXml += "                 </characteristic>" + "\r\n";
            sXml += "             </characteristic>" + "\r\n";
            sXml += "         </characteristic>" + "\r\n";
            sXml += "         </wap-provisioningdoc>" + "\r\n";

            uint uiRet = 0;
            StringBuilder sbRetData = new StringBuilder(1024);
            int iLen = 1024;
            //file processing???
            string AppPath;
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!AppPath.EndsWith(@"\"))
                AppPath += @"\";
            string xmlFile = AppPath + "wwanProfile.xml";

            if (System.IO.File.Exists(xmlFile))
                uiRet = ss.ConfigFromFile(xmlFile, xmlFile + ".out", sbRetData, ref iLen, 3000);
            else
                uiRet = ss.Set(sXml, sbRetData, ref iLen, 3000);

            if (uiRet != ITCSSErrors.E_SS_SUCCESS)
            {
                LoggingClass.addLog("setWWANprofile: Error setting Profile.");
                bRet = false;
            }
            else
                bRet = true;

            return bRet;
        }
    }
}
