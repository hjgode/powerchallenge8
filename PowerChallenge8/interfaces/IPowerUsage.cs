using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace PowerChallenge.interfaces
{
    public class PowerUsageEventArgs{
        public enum status{
            stopped=0,
            running
        }
        string _sName;
        int _iProgress;
        status _eStatus;
        public PowerUsageEventArgs(string sName, int iProgress, status eStatus){
            _sName=sName;
            _iProgress=iProgress;
            _eStatus=eStatus;
        }
    }
    public delegate void PowerUsageEventHandler(object sender, PowerUsageEventArgs args);
	public interface IPowerUsage
	{
        bool startPowerUsage();
        bool stopPowerUsage();
        event PowerUsageEventHandler PowerUsageEvent;
        System.Threading.Thread _thread{get; set;}
	}

    //#######################
    // SAMPLE
    public class scanner : IPowerUsage
    {
        private bool threadStopped = true;
        private System.Threading.Thread thread;
        public System.Threading.Thread _thread
        {
            get { return thread; }
            set { thread = value; }
        }
        scanner()
        {
            ;
        }
        public bool startPowerUsage()
        {
            if (thread == null)
            {
                thread = new System.Threading.Thread(theThread);
                thread.Start();
            }
            return true;
        }
        private void theThread()
        {
            threadStopped = false;
            try
            {
                do
                {
                    Thread.Sleep(1000);
                    newEvent("Test", 50, PowerUsageEventArgs.status.running);
                } while (true);
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
            }
            onNewEvent(new PowerUsageEventArgs("Test", 50, PowerUsageEventArgs.status.stopped));
            threadStopped = true;
        }
        public bool stopPowerUsage()
        {
            if (thread != null)
            {
                thread.Abort();
                thread.Join(1000);
            }
            if (threadStopped)
                return true;
            else
                return false;
        }

        public event interfaces.PowerUsageEventHandler PowerUsageEvent;
        delegate void deleScannerPowerUsageEventHandler(string sName, int iProgress, PowerUsageEventArgs.status eStatus);
        
        private void newEvent(string sName, int iProgress,  PowerUsageEventArgs.status eStatus){
            onNewEvent(new PowerUsageEventArgs(sName, iProgress,eStatus)); //call event fire function
        }
        //called when new event is to fire
        protected virtual void onNewEvent(PowerUsageEventArgs args)
        {
            if (PowerUsageEvent != null) //check if there is any listener
            {
                //fire event
                PowerUsageEvent(this, args);
            }
        }
    }
}
