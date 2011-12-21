using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PowerChallenge
{
    public static class ActionsClass
    {
        public enum ActionTypes
        {
            idle = 0,
            scan1D,     //make 250 scans
            scan2D,     //make 250 scans
            photo,      //make 30 photos with flash
            wwanData,   //make 10K data transfer using wwan
            bluetooth,  //enable bluetooth
            wlanData,   //enable 1 minute WLAN data
            battStatus, //read batt status
        }
        public class myAction
        {
            public DateTime dtStart;
            public int batStart;
            public DateTime dtEnd;
            public int batEnd;
            public ActionTypes actionType;
            public long ticksDuration; //one tick is 100ns (10^-9), 1 second is 10.000.000 (10^+7) ticks
            public myAction()
            {
                dtEnd = DateTime.Now;
                dtStart = DateTime.Now;
                batStart = battery.GetBatteryLifePercent();
                batEnd = battery.GetBatteryLifePercent();
                actionType = ActionTypes.idle;
                ticksDuration = 0;
            }
        }
    }
}
