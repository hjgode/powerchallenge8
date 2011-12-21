#define USE_INTERMEC
using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.WindowsMobile.SharedSource.Utilities;
using System.Runtime.InteropServices;

namespace PowerChallenge
{
    public static class Device
    {
        public const int ITC_DEVICE_UNKN = 0;
        public const int ITC_DEVICE_CK30 = 1;
        public const int ITC_DEVICE_CK31 = 2;
        public const int ITC_DEVICE_751G = 3;
        public const int ITC_DEVICE_CN2G = 4;
        public const int ITC_DEVICE_700_COLOR = 5;
        public const int ITC_DEVICE_700COLOR_PPC2003 = 6;
        public const int ITC_DEVICE_XP_CV60 = 7;
        public const int ITC_DEVICE_CV60_CE = 8;
        public const int ITC_DEVICE_CK60 = 9;
        public const int ITC_DEVICE_CN30 = 10;
        public const int ITC_DEVICE_CN2B = 11;
        public const int ITC_DEVICE_CN3 = 12;
        public const int ITC_DEVICE_CV30 = 13;
        public const int ITC_DEVICE_CK32 = 14;
        public const int ITC_DEVICE_CK3 = 15;

        public const int ITC_DEVICE_CN4  = 16;
        public const int ITC_DEVICE_CN50 = 17;
        public const int ITC_DEVICE_CS4 = 18;
        public const int ITC_DEVICE_CN70 = 20;  //Left small number space for any more Andromeda II devices for simplicity of grouping
        public const int ITC_DEVICE_CN70e = 21;
        public const int ITC_DEVICE_CK70 = 22;
        public const int ITC_DEVICE_CK71 = 23;

        //public const int ITC_DEVICE_CN31 = 20;
        public const int ITC_DEVICE_AII = 0xA2;
        public const int ITC_DEVICE_AIII = 0xA3;

	    [DllImport("itc50.dll")]
	    static extern Int32 ITCGetDeviceType();
        public static int GetDeviceType()
        {
            return ITCGetDeviceType();
        }
#if USE_INTERMEC

        public static string GetMFGCode()
        {
            /*
            <DevInfo Action="Set" Persist="true">
              <Subsystem Name="SS_Client">
                <Group Name="Identity">
                  <Field Name="HardwareVersion">CK70AA1KCU3W2100</Field> 
                </Group>
              </Subsystem>
            </DevInfo>
            */
            Intermec.DeviceManagement.SmartSystem.ITCSSApi ss = new Intermec.DeviceManagement.SmartSystem.ITCSSApi();
            string xml = "<Subsystem Name=\"SS_Client\">"+"\r\n";
            xml += "<Group Name=\"Identity\">" + "\r\n";
            xml+="<Field Name=\"HardwareVersion\">CK70AA1KCU3W2100</Field>" + "\r\n";; 
            xml+="</Group>" + "\r\n";;
            xml += "</Subsystem>" + "\r\n";;
            try
            {
                StringBuilder answer = new StringBuilder(1024);
                int retSize = 1024, iTimeout = 10000;
                iTimeout = 10;
                ss = new Intermec.DeviceManagement.SmartSystem.ITCSSApi();
                uint iResult = ss.Get(xml, answer, ref retSize, iTimeout);
                //E_SSAPI_MISSING_REQUIRED_PARM			= 0xC16E002E;
                if (iResult == Intermec.DeviceManagement.SmartSystem.ITCSSErrors.E_SS_SUCCESS)
                    System.Diagnostics.Debug.WriteLine("ssAPI returned success=" + iResult.ToString());
                else
                    System.Diagnostics.Debug.WriteLine("ssAPI returned ERROR=" + iResult.ToString());
                System.Diagnostics.Debug.WriteLine("ssAPI Answer ='" + answer);
                
                string sHardware = ssAPIhelper.getStrSetting(answer, "HardwareVersion");
                return sHardware;
            }
            catch (SystemException sx)
            {
                System.Diagnostics.Debug.WriteLine("ssAPI caused Exception: " + sx.Message);
                return "unknown";
            }

        }
#else
        public static string GetMFGCode()
        {
            IntPtr SettingsKey;
            const string SETTINGS_KEY_NAME = "Software\\Intermec\\System";
            string MFGCode = "";
            SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.LOCAL_MACHINE), SETTINGS_KEY_NAME);
            if (SettingsKey != IntPtr.Zero)
            {
                MFGCode = (string)Registry.GetValue(SettingsKey, "MFGCode");
                Registry.CloseKey(SettingsKey);
            }
            return MFGCode;

        }
#endif
        public static bool HasGPS()
        {
            // CN3ANC831G2*200 831 = GPS
            // CN3ANH830G2*200 830 = No GPS

            string s = GetMFGCode();
            if (s.Substring(8, 1) == "1" || s.StartsWith("CK7"))
                return true;
            else
                return false;
        }

        public enum MessageBeepType
        {
            Default = -1,
            Ok = 0x00000000,
            Error = 0x00000010,
            Question = 0x00000020,
            Warning = 0x00000030,
            Information = 0x00000040,
        }

        [DllImport("CoreDll.dll", SetLastError = true)]
        public static extern bool MessageBeep(
            MessageBeepType type
        );

        private static System.Threading.Timer t1;
        [DllImport("coredll.dll")]
        static extern void SystemIdleTimerReset();
        public static void KeepOn()
        {
            System.Threading.AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);
            // Create the delegate that invokes methods for the timer.
            System.Threading.TimerCallback timerDelegate = new System.Threading.TimerCallback (myTimerCallback);
            // start the timer
            t1 = new System.Threading.Timer(timerDelegate, autoEvent, 100, 1000);
        }
        public static void KeepOnStop()
        {
            t1.Dispose();
        }
        private static void myTimerCallback(Object state)
        {
            SystemIdleTimerReset();
        }
        public static int setIdleOFF()
        {
            int iRes = 0;
            Intermec.DeviceManagement.SmartSystem.ITCSSApi ss;
            string xml="";
	        //xml ="<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            //xml+="<DevInfo Action=\"Set\" Persist=\"true\">\n";
            xml +=" <Subsystem Name=\"Device Settings\">";
            //xml+="  <Group Name=\"Display\">";
            //xml+="   <Group Name=\"Backlight\">";
            //xml+="    <Field Name=\"Backlight level\">5</Field>";
            //xml+="    <Group Name=\"Battery Power\">";
            //xml+="     <Field Name=\"Backlight turns off after\">0</Field>";
            //xml+="     <Field Name=\"Backlight On Tap\">1</Field>";
            //xml+="    </Group>";
            //xml+="    <Group Name=\"External Power\">";
            //xml+="     <Field Name=\"Backlight turns off after\">0</Field>";
            //xml+="     <Field Name=\"Backlight On Tap\">1</Field>";
            //xml+="    </Group>";
            //xml+="   </Group>";
            //xml+="  </Group>";
            xml+="  <Group Name=\"Power Management\">";
            //xml+="   <Field Name=\"Power Profiles\">254</Field>";
            //xml+="   <Field Name=\"Power Button\">2</Field>";
            xml+="   <Group Name=\"Battery Power\">";
            xml+="    <Field Name=\"Device turns off after\">0</Field>";
            xml+="    <Field Name=\"Screen turns off after\">0</Field>";
            xml+="   </Group>";
            xml+="   <Group Name=\"External Power\">";
            xml+="    <Field Name=\"Device turns off after\">0</Field>";
            xml+="    <Field Name=\"Screen turns off after\">0</Field>";
            xml+="   </Group>";
            xml+="  </Group>";
            xml+=" </Subsystem>";
            //xml+="</DevInfo>";
            string AppPath;
            AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (!AppPath.EndsWith(@"\"))
                AppPath += @"\";
            string xmlFile = AppPath+"setIdleOff.xml";
            try
            {
                StringBuilder answer = new StringBuilder (1024);
                int retSize=1024, iTimeout=10000;
                iTimeout = 10;
                ss = new Intermec.DeviceManagement.SmartSystem.ITCSSApi();
                uint iResult=0;
                if (System.IO.File.Exists(xmlFile))
                    iResult = ss.ConfigFromFile(xmlFile, xmlFile + ".out", answer, ref retSize, iTimeout);
                else
                    iResult = ss.Set(xml, answer, ref retSize, iTimeout);
                //E_SSAPI_MISSING_REQUIRED_PARM			= 0xC16E002E;
                if (iResult == Intermec.DeviceManagement.SmartSystem.ITCSSErrors.E_SS_SUCCESS)
                {
                    iRes = 0;
                    System.Diagnostics.Debug.WriteLine("ssAPI returned success=" + iResult.ToString());
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ssAPI returned ERROR=" + iResult.ToString());
                    iRes = -1;
                }
                System.Diagnostics.Debug.WriteLine("ssAPI Answer ='" + answer );
            }
            catch (SystemException sx)
            {
                System.Diagnostics.Debug.WriteLine("ssAPI caused Exception: " + sx.Message);
            }
            return iRes;
        }
    }


    /*
        const long E_SS_SUCCESS	= 0x00000000;

        //   SSClient Errors (C16B0000-C16BFFFF)

        const long E_SSClient_OPEN_INPUT_FAILED = 0xC16B0001;
        const long E_SSClient_READ_INPUT_FAILED = 0xC16B0002;
        const long E_SSClient_PARSE_INPUT_FAILED = 0xC16B0003;
        const long E_SSClient_OPEN_OUTPUT_FAILED = 0xC16B0004;
        const long E_SSClient_WRITE_OUTPUT_FAILED = 0xC16B0005;
        const long E_SSClient_UNRECOGNIZED_XML_TAG = 0xC16B0006;
        const long E_SSClient_MISSING_ATTRIBUTE = 0xC16B0007;
        const long E_SSClient_TAG_OUT_OF_CONTEXT = 0xC16B0008;
        const long E_SSClient_BAD_NAME = 0xC16B0009;
        const long E_SSClient_BAD_VALUE = 0xC16B000A;
        const long E_SSClient_BAD_INSTANCE = 0xC16B000B;
        const long E_SSClient_VERSION_MISMATCH = 0xC16B000C;
        const long E_SSClient_SERVICE_IS_UNAVAILABLE = 0xC16B000D;
        const long E_SSClient_ERROR_IN_SUBELEMENT = 0xC16B000E;

        //   Legacy ICCE Errors (C16B0000-C16BFFFF). Replaced by SSClient Errors.

        const long E_ICCE_OPEN_INPUT_FAILED = 0xC16B0001;
        const long E_ICCE_READ_INPUT_FAILED = 0xC16B0002;
        const long E_ICCE_PARSE_INPUT_FAILED = 0xC16B0003;
        const long E_ICCE_OPEN_OUTPUT_FAILED = 0xC16B0004;
        const long E_ICCE_WRITE_OUTPUT_FAILED = 0xC16B0005;
        const long E_ICCE_UNRECOGNIZED_XML_TAG = 0xC16B0006;
        const long E_ICCE_MISSING_ATTRIBUTE = 0xC16B0007;
        const long E_ICCE_TAG_OUT_OF_CONTEXT = 0xC16B0008;
        const long E_ICCE_BAD_NAME = 0xC16B0009;
        const long E_ICCE_BAD_VALUE = 0xC16B000A;
        const long E_ICCE_BAD_INSTANCE = 0xC16B000B;
        const long E_ICCE_VERSION_MISMATCH = 0xC16B000C;
        const long E_ICCE_SERVICE_IS_UNAVAILABLE = 0xC16B000D;
        const long E_ICCE_ERROR_IN_SUBELEMENT = 0xC16B000E;

        //   ITCConfig/SS_API Errors (C16E0000-C16EFFFF)

        const long E_ITCCONFIG_OPERATION_FAILED		= 0xC16E0001;
        const long E_ITCCONFIG_OPEN_CONN_FAILED		= 0xC16E0002;
        const long E_ITCCONFIG_SEND_REQ_FAILED		= 0xC16E0003;
        const long E_ITCCONFIG_RCV_RESP_FAILED		= 0xC16E0004;
        const long E_ITCCONFIG_RCV_TIMEOUT		= 0xC16E0005;
        const long E_ITCCONFIG_RCV_BUFFER_TOO_SMALL 	= 0xC16E0006;
        const long E_ITCCONFIG_CONN_NOT_OPENED		= 0xC16E0007;
        const long E_ITCCONFIG_READ_FILE_FAILED		= 0xC16E0008;
        const long E_ITCCONFIG_CREATE_DOM_FAILED	= 0xC16E0009;
        const long E_ITCCONFIG_OPEN_FILE_FAILED		= 0xC16E000A;

        const long E_SSAPI_OPERATION_FAILED					= 0xC16E0021;
        const long E_SSAPI_OPEN_CONN_FAILED					= 0xC16E0022;
        const long E_SSAPI_SEND_REQ_FAILED					= 0xC16E0023;
        const long E_SSAPI_RCV_RESP_FAILED					= 0xC16E0024;
        const long E_SSAPI_RCV_TIMEOUT						= 0xC16E0025;
        const long E_SSAPI_RCV_BUFFER_TOO_SMALL 			= 0xC16E0026;
        const long E_SSAPI_CONN_NOT_OPENED					= 0xC16E0027;
        const long E_SSAPI_READ_FILE_FAILED					= 0xC16E0028;
        const long E_SSAPI_CREATE_DOM_FAILED				= 0xC16E0029;
        const long E_SSAPI_OPEN_FILE_FAILED					= 0xC16E002A;
        const long E_SSAPI_XML_MATCH_NOT_FOUND				= 0xC16E002B;
        const long E_SSAPI_NO_MORE_CONNECTION_ALLOWED 	= 0xC16E002C;
        const long E_SSAPI_SYS_RSC_ALLOC_FAILED 			= 0xC16E002D;
        const long E_SSAPI_MISSING_REQUIRED_PARM			= 0xC16E002E;
        const long E_SSAPI_INVALID_EVENT						= 0xC16E002F;
        const long E_SSAPI_TIMEOUT								= 0xC16E0030;
        const long E_SSAPI_MALFORMED_XML						= 0xC16E0031;
        const long E_SSAPI_INVALID_PARM						= 0xC16E0032;
        const long E_SSAPI_FUNCTION_UNAVAILABLE			= 0xC16E0033;
        const long E_SSAPI_MSG_ID_IN_USE                = 0xC16E0034;
        const long E_SSAPI_NOT_GROUP_MEMBER             = 0xC16E0035;

     */
}
