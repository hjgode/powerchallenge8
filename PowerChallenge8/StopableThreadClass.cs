using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PowerChallenge
{
    public abstract class StopableThreadClass:IDisposable
    {
        private static int iCounter=0;
        internal bool _bIsRunning=false;
        internal bool _bStopThread = false;

        Thread WorkerThread;
        public string name="Name";

        public StopableThreadClass(){
            name = "Thread " + iCounter.ToString();
            iCounter++;
            if (WorkerThread == null)
            {
                WorkerThread = new Thread(new ThreadStart(doWork));
                _bIsRunning = true;
            }
        }

        public void Run()
        {
            if (_bIsRunning == false)
                WorkerThread.Start();
        }

        public virtual void doWork()
        {
            _bIsRunning = true;
            try
            {
                do
                {
                    ;
                    Thread.Sleep(100);
                } while (!_bStopThread);
            }
            catch (ThreadAbortException ex)
            {
                System.Diagnostics.Debug.WriteLine("ThreadAbortException '" + ex.Message + "' in " + name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception '" + ex.Message + "' in " + name);
            }
        }

        public void Quit()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _bStopThread = true;
            WorkerThread.Abort();
            WorkerThread.Join(0);
            WorkerThread = null;
        }
        public void Dispose()
        {
            Cleanup();
        }
    }
}
