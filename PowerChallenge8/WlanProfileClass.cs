using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Intermec.DeviceManagement.SmartSystem;

namespace PowerChallenge
{
    public static class WlanProfileClass
    {
        private const string sDisableWLANxml = "<Subsystem Name=\"Communications\"> \r\n<Group Name=\"802.11 Radio\">\r\n  <Field Name=\"Radio Enabled\">0</Field> \r\n  </Group>\r\n  </Subsystem>";

        public static bool setWLANprofile()
        {
            bool bRet = false;
            ITCSSApi ss;
            ss = new ITCSSApi();
            string sXml = "";

            //sXml += "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> \r\n";
            //sXml += " <DevInfo Action=\"Set\" Persist=\"true\">\r\n";
            sXml += " <Subsystem Name=\"Funk Security\">\r\n";
            sXml += " <Group Name=\"Profile\" Instance=\"Profile_1\">\r\n";
            sXml += "  	<Field Name=\"SSID\">SUPPORT</Field> \r\n";
            sXml += "  	<Field Name=\"8021x\">None</Field>\r\n";
            sXml += "	<Field Name=\"Encryption\">TKIP</Field>\r\n";
            sXml += "	<Field Name=\"PreSharedKey\" Encrypt=\"binary.base64\">dzcrvPwmAWjcJAOO75RQEQ==</Field> \r\n";
            sXml += "   <Field Name=\"PSMode\">Enabled(Fast PSP)</Field> \r\n";
            sXml += "</Group>\r\n";
            sXml += "</Subsystem>\r\n";
            sXml += "<Subsystem Name=\"IQueue\">\r\n";
            sXml += "  	<Field Name=\"Associated Server IP\">192.168.128.5</Field> \r\n";
            sXml += "</Subsystem>\r\n";
            //sXml += "</DevInfo>\r\n";

            uint uiRet = 0;
            StringBuilder sbRetData = new StringBuilder(1024);
            int iLen = 1024;
            string AppPath;
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!AppPath.EndsWith(@"\"))
                AppPath += @"\";
            string xmlFile = AppPath + "wlanProfile.xml";

            if (System.IO.File.Exists(xmlFile))
                uiRet = ss.ConfigFromFile(xmlFile, xmlFile + ".out", sbRetData, ref iLen, 3000);
            else
                uiRet = ss.Set(sXml, sbRetData, ref iLen, 3000);

            if (uiRet != ITCSSErrors.E_SS_SUCCESS)
            {
                LoggingClass.addLog("setWLANprofile: Error setting Profile.");
                bRet = false;
            }
            else
            {
                LoggingClass.addLog("setWLANprofile: Setting Profile OK.");
                bRet = true;
            }
            System.Diagnostics.Debug.WriteLine("setWLANprofile: ssAPI Answer ='" + sbRetData);
            return bRet;
        }
    }
}
