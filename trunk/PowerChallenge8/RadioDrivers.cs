
/*
 RadioDrivers.cs
 A component of the Intermec Developer Library (IDL)

 Purpose:
 Switch on/off BT/WLAN/WWAN Radio's

 Author: Ronald van der Putten
 _______________________________________________________________________________
 Copyright Intermec (c) 2007-2008, All rights reserved.
 _______________________________________________________________________________

 */


using System;
using System.Runtime.InteropServices;

namespace Intermec.Utils
{
	static class RadioDrivers
	{

		private enum RADIODEVTYPE
		{
			RADIODEVICES_WIFI = 1,
			RADIODEVICES_PHONE,
			RADIODEVICES_BLUETOOTH,
		}

		// whether to save before or after changing state

		internal enum RADIO_STATES : int
		{
			Unavailable = -1,
			Off = 0,
			On
		}
		internal enum BT_STATES:int
		{
			Unavailable = -1,
			Off = 0,
			On,
			Discoverable
		}


		
		[StructLayout(LayoutKind.Sequential)]
		struct RadioDeviceStruct
		{
			public IntPtr pszDeviceName; 
			public IntPtr pszDisplayName;
			public int dwState;     
			public int dwDesired;   
			public RADIODEVTYPE DeviceType;  
			public IntPtr pNext;    
		}

		[DllImport("ossvcs.dll", EntryPoint = "#276")]
		private static extern int GetWirelessDevices(ref IntPtr pDevices, int dwFlags);

		[DllImport("ossvcs.dll", EntryPoint = "#280")]
		private static extern int FreeDevicesList(IntPtr pDevices);

		[DllImport("ossvcs.dll", EntryPoint = "#273")]
		private static extern int ChangeRadioState(IntPtr pDevices, int dwState, int sa);

		private const int OK = 0;
		
		[DllImport("WWANpower.dll", EntryPoint = "WWANpower")]
        private static extern int WWANpower(bool bEnable);

		private const int RADIODEVICES_DONT_SAVE = 0;
		private const int RADIODEVICES_PRE_SAVE = 1;
		private const int RADIODEVICES_POST_SAVE = 2;

		private static IntPtr pDevicesList = IntPtr.Zero;



		public static RADIO_STATES Phone
		{
			
			get
			{
				return ((RADIO_STATES)GetState(RADIODEVTYPE.RADIODEVICES_PHONE));
			}

			set
			{
                //workaround
                int iRes = 0;
                switch (value)
                {
                    case RADIO_STATES.Off:
                        iRes = WWANpower(false);
                        break;
                    case RADIO_STATES.On:
                        iRes = WWANpower(true);
                        break;
                    default:
                        iRes = -1; //unknown state
                        break;
                }
                System.Diagnostics.Debug.WriteLine("Switch RadioState returns: " + iRes.ToString());
                //not working on CK70
				//ChangeDeviceState(RADIODEVTYPE.RADIODEVICES_PHONE, (int)value);
			}
		}

		public static RADIO_STATES WIFI
		{
			get
			{
				return ((RADIO_STATES)GetState(RADIODEVTYPE.RADIODEVICES_WIFI));
			}

			set
			{
				ChangeDeviceState (RADIODEVTYPE.RADIODEVICES_WIFI, (int)value);

			}
		}
		public static BT_STATES BlueTooth
		{
			get
			{
				return ((BT_STATES)GetState(RADIODEVTYPE.RADIODEVICES_BLUETOOTH));
			}

			set
			{
				ChangeDeviceState (RADIODEVTYPE.RADIODEVICES_BLUETOOTH, (int)value);
			}

		}


		private static void ChangeDeviceState(RADIODEVTYPE RadioType, int state)
		{

			RadioDeviceStruct rds;

			IntPtr pDevice = GetDevice(RadioType);
			
			if (pDevice != IntPtr.Zero)
			{

				rds = (RadioDeviceStruct)Marshal.PtrToStructure(pDevice, typeof(RadioDeviceStruct));
				if (rds.dwState != state)
				{
					ChangeRadioState(pDevice, state, RADIODEVICES_PRE_SAVE);
				}
				Cleanup();
			}
		}

		private static int GetState(RADIODEVTYPE device)
		{
			RadioDeviceStruct rds;
			int State = -1;
			IntPtr pDevice = GetDevice(device);
			if (pDevice != IntPtr.Zero)
			{
				rds = (RadioDeviceStruct)Marshal.PtrToStructure(pDevice, typeof(RadioDeviceStruct));
				State = rds.dwState;
			}
			Cleanup();

			return State;
		}


		private static IntPtr GetDevice(RADIODEVTYPE DeviceType)
		{

			IntPtr pCurrent;

			
			
			try
			{
				pDevicesList = pCurrent = GetDeviceList();

				while (pCurrent != IntPtr.Zero)
				{
					RadioDeviceStruct rds = (RadioDeviceStruct)Marshal.PtrToStructure(pCurrent, typeof(RadioDeviceStruct));

					if (DeviceType == rds.DeviceType)
					{
						return pCurrent;
					}
					else
					{
						pCurrent =rds.pNext;
						
					}

				}

			}
			catch (Exception e)
			{
                System.Diagnostics.Debug.WriteLine("Exception in GetDevice: "+e.Message);
			}
			return IntPtr.Zero; 
		}




		private static IntPtr GetDeviceList()
        {
            IntPtr RadioDeviceList = new IntPtr();
            try
            {
				if (GetWirelessDevices(ref RadioDeviceList, 0) == OK)
                {
					if (RadioDeviceList != IntPtr.Zero)
						return RadioDeviceList;
					else
						return IntPtr.Zero;
                        
                }
                else
                    throw new Exception("Error while getting list of radio devices!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


		private static void Cleanup()
		{
			if (pDevicesList != IntPtr.Zero)
				FreeDevicesList(pDevicesList);

		}
        

	}

}
