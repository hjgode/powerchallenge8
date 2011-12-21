using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NativeSync;

namespace NativeSync
{
    public static class ScanEvent
    {
        // Names for scan events
        private const string ITC_SCAN_STATE = "StateLeftScan"; // 	StateCenterScan
        private const string ITC_SCAN_DELTA = "DeltaLeftScan"; // 	DeltaCenterScan
        public static void fireScanner()
        {
            SystemEvent hScanDeltaEvent = new SystemEvent(ITC_SCAN_DELTA, false, false);
            SystemEvent hScanStateEvent = new SystemEvent(ITC_SCAN_STATE, false, false);
            hScanStateEvent.SetEvent();
            hScanDeltaEvent.SetEvent();
            System.Threading.Thread.Sleep(100);
            hScanStateEvent.ResetEvent();
            hScanDeltaEvent.SetEvent();
        }
    }
}
