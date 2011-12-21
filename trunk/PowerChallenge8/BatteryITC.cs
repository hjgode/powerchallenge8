#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;



namespace Intermec.Utils
{
	public class Battery
	{
		private System.Threading.Thread MyPowerThread;

		private double MeasuredAve = 0;
		private uint _PowerUpdateCycle = 5000;

		SYSTEM_POWER_STATUS_EX2 BSPS;

		[DllImport("coredll")]
		private static extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

		//[StructLayout(LayoutKind.Sequential)]
		private class SYSTEM_POWER_STATUS_EX2
		{
			public byte ACLineStatus;
			public byte BatteryFlag;
			public byte BatteryLifePercent;
			public byte Reserved1;
			public uint BatteryLifeTime;
			public uint BatteryFullLifeTime;
			public byte Reserved2;
			public byte BackupBatteryFlag;
			public byte BackupBatteryLifePercent;
			public byte Reserved3;
			public uint BackupBatteryLifeTime;
			public uint BackupBatteryFullLifeTime;
			public uint BatteryVoltage;
			public uint BatteryCurrent;
			public uint BatteryAverageCurrent;
			public uint BatteryAverageInterval;
			public uint BatterymAHourConsumed;
			public uint BatteryTemperature;
			public uint BackupBatteryVoltage;
			public byte BatteryChemistry;
		}

		


		private void AvgPowerThread()
		{
			double Sample_mw;

			double pwravg = 0;
			int  counter = 0;


			while (counter < 2)
			{
				Sample_mw = this.PowerMw;
				
				if (Sample_mw > 0)
				{
					pwravg+=Sample_mw;
					counter++;
				}
			
				System.Threading.Thread.Sleep((int)_PowerUpdateCycle);				

			}

			MeasuredAve = pwravg / counter;

			System.Diagnostics.Debug.WriteLine("power: " + MeasuredAve.ToString() + " Samples: " + counter.ToString());
		}



		public int PowerUpdateCycle
		{
			get
			{
				return (int)_PowerUpdateCycle;
			}
		}


		public Battery()
		{
			BSPS = new Battery.SYSTEM_POWER_STATUS_EX2();

			if (Battery.GetSystemPowerStatusEx2(BSPS, (uint)Marshal.SizeOf(BSPS), true) != 0)
			{
				// add 5% to the update time
				_PowerUpdateCycle = (uint)((double)BSPS.BatteryAverageInterval*1.05);
			}
		}



		public uint StartMeasuring()
		{
			MyPowerThread = new System.Threading.Thread(AvgPowerThread);
			System.Threading.Thread.Sleep((int)this._PowerUpdateCycle);
			MyPowerThread.Start();
			MyPowerThread.Join();
			return (uint)MeasuredAve;
		}

		private double PowerMw
		{
			get
			{
				double powermw = 0;
				
				if (Battery.GetSystemPowerStatusEx2(BSPS,(uint)Marshal.SizeOf(BSPS),true) != 0)
				{
					powermw = BSPS.BatteryVoltage * BSPS.BatteryCurrent / 1000;
					_PowerUpdateCycle = BSPS.BatteryAverageInterval;
				}
			
				return powermw;
			}

		}
        public int LifePercent
        {
            get
            {
				int percent = 0;
				
				if (Battery.GetSystemPowerStatusEx2(BSPS,(uint)Marshal.SizeOf(BSPS),true) != 0)
				{
                    percent = BSPS.BatteryLifePercent;
				}
				return percent;
            }
        }

	}
}
