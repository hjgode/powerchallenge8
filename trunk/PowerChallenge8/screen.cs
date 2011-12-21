#define USE_INTERMEC
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.SharedSource.Utilities;
using System.Runtime.InteropServices;

namespace Intermec.Utils
{
	static class Display
	{
		private enum PowerState
		{
			PwrDeviceUnspecified = -1,
			FullOn = 0,
			LowPower = 1,
			StandBy = 2,
			Sleep = 3,
			Off = 4,
			PowerMax = 5
		};

        private enum SysPowerState
        {
            POWER_STATE_ON           = 0x00010000,        // on state
            POWER_STATE_OFF          = 0x00020000,        // no power, full off
            POWER_STATE_CRITICAL     = 0x00040000,        // critical off
            POWER_STATE_BOOT         = 0x00080000,        // boot state
            POWER_STATE_IDLE         = 0x00100000,        // idle state
            POWER_STATE_SUSPEND      = 0x00200000,        // suspend state
            POWER_STATE_UNATTENDED   = 0x00400000,        // Unattended state.
            POWER_STATE_RESET        = 0x00800000,        // reset state
            POWER_STATE_USERIDLE     = 0x01000000,        // user idle state
            POWER_STATE_PASSWORD     = 0x10000000,        // This state is password protected.        
        }
        private enum DeviceFlags
        {
            POWER_NAME = 0,
            POWER_FORCE
        }
		[DllImport("coredll.dll", SetLastError=true)]
		private static extern int DevicePowerNotify(string pdevice, PowerState pstate, int flags);
        [DllImport("coredll.dll", SetLastError = true)]
		private static extern int SetSystemPowerState(IntPtr hDev, SysPowerState ps, DeviceFlags df);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr SetPowerRequirement(string pvDevice, PowerState DeviceState, DeviceFlags DeviceFlags, IntPtr pvSystemState, long StateFlags);
        //pvSystemState
        //[in] If not set to null, indicates that the requirement should only be enforced for the named system power state.
        //StateFlags
        //[in] Unused. Set to zero.
        [DllImport("coredll.dll")]
		private static extern int ReleasePowerRequirement(IntPtr hPowerReq);
        private static IntPtr hPower = IntPtr.Zero;
        public static int requestFullPower()
        {
            //return(DevicePowerNotify("BKL1:", PowerState.FullOn, 1));
            if (hPower == IntPtr.Zero)
                hPower = SetPowerRequirement("BKL1:", PowerState.FullOn, DeviceFlags.POWER_FORCE | DeviceFlags.POWER_NAME, IntPtr.Zero, 0);
            if (hPower == IntPtr.Zero)
            {
                int iErr = Marshal.GetLastWin32Error();
                System.Diagnostics.Debug.WriteLine("SetPowerRequirement returned error=" + iErr.ToString());
            }
            //SetSystemPowerState(NULL, POWER_STATE_ON, POWER_FORCE);
            int iRes = SetSystemPowerState(IntPtr.Zero, SysPowerState.POWER_STATE_ON, DeviceFlags.POWER_FORCE);
            if (iRes != 0)
            {
                int iErr = Marshal.GetLastWin32Error();
                System.Diagnostics.Debug.WriteLine("SetSystemPowerState returned error=" + iErr.ToString());
            }
            if (hPower != IntPtr.Zero)
                return 0;
            else
                return -1;
        }
        public static int releaseFullPower()
        {
            int iRes = 0;
            if (hPower != IntPtr.Zero)
                iRes = ReleasePowerRequirement(hPower);
            else
                iRes = -99;
            return iRes;
        }
#if USE_INTERMEC
        [DllImport("itc50.dll")]
        static extern int ITCGetScreenBrightness(ref int pdwBrightness);
        [DllImport("itc50.dll")]
        static extern int ITCSetScreenBrightness(int dwBrightLevel);
        [DllImport("itc50.dll")]
        static extern int ITCSetScreenBrightnessAcDc(int dwPowerSelect, int dwBrightness);


        static public int GetBackLightLevel()
        {
            int iBacklight = 0;
            if (ITCGetScreenBrightness(ref iBacklight)>=0)
                return iBacklight;
            else
                return -1;
        }
        static public void SetBackLightLevel(int Level)
        {
            int iRes = 0;
            //iRes = ITCSetScreenBrightness(Level);//does not work with BDU
            iRes = ITCSetScreenBrightnessAcDc(0xDC, Level); //does not work with BDU
            iRes = ITCSetScreenBrightnessAcDc(0xAC, Level); //does not work with BDU

            string xml = "<Subsystem Name=\"Device Settings\">";
            xml += "<Group Name=\"Backlight\">";
            xml += string.Format("<Field Name=\"Backlight ambient\">{0}</Field> ", Level.ToString());
            xml += "</Group>";
            xml += "</Subsystem>";
            try
            {
                StringBuilder answer = new StringBuilder(1024);
                int retSize = 1024, iTimeout = 3000;
                iTimeout = 10;
                Intermec.DeviceManagement.SmartSystem.ITCSSApi ss = new Intermec.DeviceManagement.SmartSystem.ITCSSApi();
                ss = new Intermec.DeviceManagement.SmartSystem.ITCSSApi();
                uint iResult = ss.Set(xml, answer, ref retSize, iTimeout);
                //E_SSAPI_MISSING_REQUIRED_PARM			= 0xC16E002E;
                if (iResult == Intermec.DeviceManagement.SmartSystem.ITCSSErrors.E_SS_SUCCESS)
                    System.Diagnostics.Debug.WriteLine("ssAPI returned success=" + iResult.ToString());
                else
                    System.Diagnostics.Debug.WriteLine("ssAPI returned ERROR=" + iResult.ToString());
                System.Diagnostics.Debug.WriteLine("ssAPI Answer ='" + answer);
                if(Level==0)
                    Display.SwitchBackLight(false);
            }
            catch (SystemException sx)
            {
                System.Diagnostics.Debug.WriteLine("ssAPI caused Exception: " + sx.Message);
            }
        }
#else
		static public int GetBackLightLevel()
		{
            int Level = 5;
			IntPtr SettingsKey;
			const string SETTINGS_KEY_NAME = "ControlPanel\\Backlight";
			SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.CURRENT_USER), SETTINGS_KEY_NAME);
			if (SettingsKey != IntPtr.Zero)
			{
				Level = (int) Registry.GetValue(SettingsKey, "BattBrightness");
				Registry.CloseKey(SettingsKey);
			}
            return Level;
		}

		static public void SetBackLightLevel(int Level)
		{
			IntPtr SettingsKey;
			const string SETTINGS_KEY_NAME = "ControlPanel\\Backlight";
			SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.CURRENT_USER), SETTINGS_KEY_NAME);
			if (SettingsKey != IntPtr.Zero)
			{
				Registry.SetValue(SettingsKey, "BattBrightness", Level);
				Registry.CloseKey(SettingsKey);
			}
		}
#endif

        static public void enableBacklight(bool OnOff)
        {
            if(OnOff)
			    DevicePowerNotify("BKL1:", PowerState.FullOn, 1);
            else
                DevicePowerNotify("BKL1:", PowerState.Off, 1);
        }

		static public void SwitchBackLight(bool OnOff)
		{
			const string SETTINGS_KEY_NAME = "ControlPanel\\Backlight";
			IntPtr SettingsKey;

			if (OnOff == false)
			{
				 //Switch off the on tap backlight back on
				SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.CURRENT_USER), SETTINGS_KEY_NAME);
				if (SettingsKey != IntPtr.Zero)
				{
					Registry.SetValue(SettingsKey, "BattBacklightOnTap", 0);
					Registry.CloseKey(SettingsKey);
				}

				DevicePowerNotify("BKL1:", PowerState.Off, 1);
			}
			else
			{
				SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.CURRENT_USER), SETTINGS_KEY_NAME);
				if (SettingsKey != IntPtr.Zero)
				{
					Registry.SetValue(SettingsKey, "BattBacklightOnTap", 1);
					Registry.CloseKey(SettingsKey);
				}
				SettingsKey = Registry.OpenKey(Registry.GetRootKey(Registry.HKey.CURRENT_USER), SETTINGS_KEY_NAME);
				if (SettingsKey != IntPtr.Zero)
				{
					Registry.SetValue(SettingsKey, "BattAutodimEnabled", 0);
					Registry.CloseKey(SettingsKey);
				}

				DevicePowerNotify("BKL1:", PowerState.FullOn, 1);
			}


		}

	}
}
