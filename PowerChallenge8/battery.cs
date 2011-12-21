#pragma warning disable 0649
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PowerChallenge
{
    public static class battery
    {
        public static int BatCapacity = 2200;
        /// <summary>
        /// Gets the battery life time percentage
        /// </summary>
        /// <returns>uint percentage</returns>
        public static int GetBatteryLifePercent()
        {
            int CurrBatStat = 0;
            BatteryStatus.SYSTEM_POWER_STATUS_EX2 BatStat = new BatteryStatus.SYSTEM_POWER_STATUS_EX2();
            if (BatteryStatus.getStatus() != null)  // GetSystemPowerStatusEx2(BatStat, (uint)Marshal.SizeOf(BatStat), true) == (uint)Marshal.SizeOf(BatStat))
            {
                //convert to hours and minutes: ie 13.50 => 13 hours and 30 Minutes
                double BatLife = (double)BatCapacity / (double)BatStat.BatteryCurrent;
                int hhBatLife = (int)BatLife;
                int mmBatLife = (int)(((BatLife - hhBatLife) * 100) * 0.6);

                CurrBatStat = BatStat.BatteryLifePercent;

            }

            return (CurrBatStat);
        }
        public static TimeSpan GetBatteryEstimatedTimeLeft()
        {
            TimeSpan ts;
            BatteryStatus.SYSTEM_POWER_STATUS_EX2 BatStat = new BatteryStatus.SYSTEM_POWER_STATUS_EX2();
            if (BatteryStatus.getStatus()!=null)  // GetSystemPowerStatusEx2(BatStat, (uint)Marshal.SizeOf(BatStat), true) == (uint)Marshal.SizeOf(BatStat))
            {
                //convert to hours and minutes: ie 13.50 => 13 hours and 30 Minutes
                double BatLife = (double)BatCapacity / (double)BatStat.BatteryCurrent;
                int hhBatLife = (int)BatLife;
                int mmBatLife = (int)(((BatLife - hhBatLife) * 100) * 0.6);
                ts = new TimeSpan(hhBatLife, mmBatLife, 0);

            }
            else
                ts = new TimeSpan(0, 0, 0);

            return (ts);

        }
        public static class BatteryStatus
        {

            [DllImport("coredll")]
            private static extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);


            public class SYSTEM_POWER_STATUS_EX2
            {
                [Flags]
                enum ACLineStats:byte
                {
                    AC_LINE_OFFLINE                 = 0x00,
                    AC_LINE_ONLINE                  = 0x01,
                    AC_LINE_BACKUP_POWER            = 0x02,
                    AC_LINE_UNKNOWN                 = 0xFF
                }
                [Flags]
                enum BattFlags:byte{
                    BATTERY_FLAG_HIGH               = 0x01,
                    BATTERY_FLAG_LOW                = 0x02,
                    BATTERY_FLAG_CRITICAL           = 0x04,
                    BATTERY_FLAG_CHARGING           = 0x08,
                    BATTERY_FLAG_NO_BATTERY         = 0x80,
                    BATTERY_FLAG_UNKNOWN            = 0xFF
                }
                [Flags]
                enum Chemistry:byte
                {
                    BATTERY_CHEMISTRY_ALKALINE     = 0x01,
                    BATTERY_CHEMISTRY_NICD         = 0x02,
                    BATTERY_CHEMISTRY_NIMH         = 0x03,
                    BATTERY_CHEMISTRY_LION         = 0x04,
                    BATTERY_CHEMISTRY_LIPOLY       = 0x05,
                    BATTERY_CHEMISTRY_ZINCAIR      = 0x06,
                    BATTERY_CHEMISTRY_UNKNOWN      = 0xFF
                }
                const byte  BATTERY_PERCENTAGE_UNKNOWN      = 0xFF;

                const uint BATTERY_LIFE_UNKNOWN = 0xFFFFFFFF;
                
                public byte ACLineStatus;
                public byte BatteryFlag;
                /// <summary>
                /// Percentage of full battery charge remaining. This member can be a value in the range 0 to 100, 
                /// or BATTERY_PERCENTAGE_UNKNOWN if the status is unknown. All other values are reserved.
                /// </summary>
                public byte BatteryLifePercent;
                public byte Reserved1;
                /// <summary>
                /// Number of seconds of battery life remaining, or BATTERY_LIFE_UNKNOWN if remaining seconds are unknown.
                /// </summary>
                public uint BatteryLifeTime;
                /// <summary>
                /// Number of seconds of battery life when at full charge, or BATTERY_LIFE_UNKNOWN if full lifetime is unknown.
                /// </summary>
                public uint BatteryFullLifeTime;
                public byte Reserved2;

                public byte BackupBatteryFlag;
                public byte BackupBatteryLifePercent;
                public byte Reserved3;
                public uint BackupBatteryLifeTime;
                public uint BackupBatteryFullLifeTime;
                /// <summary>
                /// Reports Reading of battery voltage in millivolts (0..65535 mV)
                /// </summary>
                public uint BatteryVoltage;
                /// <summary>
                /// Reports Instantaneous current drain (mA). 0..32767 for charge, 0 to -32768 for discharge
                /// </summary>
                public uint BatteryCurrent;
                /// <summary>
                /// Reports short term average of device current drain (mA). 0..32767 for charge, 0 to -32768 for discharge
                /// </summary>
                public uint BatteryAverageCurrent;
                /// <summary>
                /// Reports time constant (mS) of integration used in reporting BatteryAverageCurrent
                /// </summary>
                public uint BatteryAverageInterval;
                /// <summary>
                /// Reports long-term cumulative average DISCHARGE (mAH). Reset by charging or changing the batteries. 0 to 32767 mAH  
                /// </summary>
                public uint BatterymAHourConsumed;
                /// <summary>
                /// Reports Battery temp in 0.1 degree C (-3276.8 to 3276.7 degrees C)
                /// </summary>
                public uint BatteryTemperature;
                /// <summary>
                /// Reports Reading of backup battery voltage
                /// </summary>
                public uint BackupBatteryVoltage;
                /// <summary>
                /// See Chemistry defines above
                /// </summary>
                public byte BatteryChemistry;
                public override string ToString()
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Battery Status: ");
                    switch (ACLineStatus){
                        case (byte)ACLineStats.AC_LINE_ONLINE:
                            sb.Append("AC on, ");
                            break;
                        case (byte)ACLineStats.AC_LINE_OFFLINE:
                            sb.Append("AC off, ");
                            break;
                        case (byte)ACLineStats.AC_LINE_BACKUP_POWER:
                            sb.Append("AC backup, ");
                            break;
                        case (byte)ACLineStats.AC_LINE_UNKNOWN:
                            sb.Append("AC unknown, ");
                            break;
                        default:
                            sb.Append("AC undef, ");
                            break;
                    }
                    switch (BatteryFlag)
                    {
                        case (byte)BattFlags.BATTERY_FLAG_CHARGING:
                            sb.Append("charging, ");
                            break;
                        case (byte)BattFlags.BATTERY_FLAG_CRITICAL:
                            sb.Append("critical, ");
                            break;
                        case (byte)BattFlags.BATTERY_FLAG_HIGH:
                            sb.Append("high, ");
                            break;
                        case (byte)BattFlags.BATTERY_FLAG_LOW:
                            sb.Append("low, ");
                            break;
                        case (byte)BattFlags.BATTERY_FLAG_NO_BATTERY:
                            sb.Append("no batt, ");
                            break;
                        case (byte)BattFlags.BATTERY_FLAG_UNKNOWN:
                            sb.Append("batt. unknown, ");
                            break;
                        default:
                            sb.Append("batt. undef, ");
                            break;
                    }
                    if(BatteryLifePercent!=BATTERY_PERCENTAGE_UNKNOWN)
                        sb.Append("life%=" + BatteryLifePercent.ToString()+", ");
                    else
                        sb.Append("life%=undef, ");

                    if(BatteryLifeTime != BATTERY_LIFE_UNKNOWN)
                        sb.Append("lifetime=" + BatteryLifeTime.ToString()+", ");
                    else
                        sb.Append("lifetime=undef, ");

                    if (BatteryFullLifeTime != BATTERY_LIFE_UNKNOWN)
                        sb.Append("FullLifeTime=" + BatteryFullLifeTime.ToString() + ", ");
                    else
                        sb.Append("FullLifeTime=undef, ");

                    sb.Append ("BatteryVoltage="+ BatteryVoltage.ToString()+", ");
                    sb.Append ("BatteryCurrent="+BatteryCurrent.ToString()+", ");
                    sb.Append ("BatteryAverageCurrent="+BatteryAverageCurrent.ToString()+", ");
                    sb.Append ("BatteryAverageInterval="+BatteryAverageInterval.ToString()+", ");
                    sb.Append ("BatterymAHourConsumed="+BatterymAHourConsumed.ToString()+", ");
                    sb.Append ("BatteryTemperature="+BatteryTemperature.ToString()+", ");

                    sb.Append("BatteryChemistry=");
                    switch (BatteryChemistry)
                    {
                        case (byte)Chemistry.BATTERY_CHEMISTRY_ALKALINE:
                            sb.Append("Alkaline, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_LION:
                            sb.Append("Lion, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_LIPOLY:
                            sb.Append("Lipoly, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_NICD:
                            sb.Append("NICD, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_NIMH:
                            sb.Append("NIMH, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_UNKNOWN:
                            sb.Append("unknown, ");
                            break;
                        case (byte)Chemistry.BATTERY_CHEMISTRY_ZINCAIR:
                            sb.Append("ZINAIR, ");
                            break;
                        default:
                            sb.Append("undef, ");
                            break;
                    }
                    sb.Append ("BatteryChemistry=" + BatteryChemistry.ToString() + ", ");

                    return sb.ToString();
                }
            }
            public static SYSTEM_POWER_STATUS_EX2 getStatus()
            {
                BatteryStatus.SYSTEM_POWER_STATUS_EX2 BatStat = new SYSTEM_POWER_STATUS_EX2();
                if (BatteryStatus.GetSystemPowerStatusEx2(BatStat, (uint)Marshal.SizeOf(BatStat), true) == (uint)Marshal.SizeOf(BatStat))
                {
                    return BatStat;
                }
                else
                    return null;
            }
        }
    }
}
