using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utils
{
    public class Timer
    {
        private string _timerName;
        private Thread _workerThread;
        private ManualResetEvent _stop = new ManualResetEvent(true);
        private ManualResetEvent _stopped = new ManualResetEvent(true);

        public int Interval { get; set; }
        public event EventHandler Tick;
        public bool Enabled
        {
            get
            {
                return !_stopped.WaitOne(0);
            }
        }

        public Timer() : this("")
        {
        }

        public Timer(string timerName)
        {
            _timerName = timerName;
        }

        public void Start()
        {
            lock (this)
            {
                if (!Enabled)
                {
                    _stop.Reset();
                    _stopped.Reset();
                    _workerThread = new Thread(ThreadProc);
                    _workerThread.Name = "Timer: " + _timerName;
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                _stop.Set();

                if (Thread.CurrentThread != _workerThread)
                    _stopped.WaitOne();
            }
        }

        public void TriggerNow()
        {
            _lastTime = Utils.Time.GetTime() - Interval;
        }
        public void Restart()
        {
            lock (this)
            {
                if (Enabled)
                {
                    _lastTime = Utils.Time.GetTime();
                }
                else
                {
                    Start();
                }
            }
        }

        double _lastTime = 0.0;
        public void ThreadProc()
        {
            _lastTime = Utils.Time.GetTime();

            while (_stop.WaitOne(10) == false)
            {
                double time = Utils.Time.GetTime();
                if (_lastTime + Interval < time)
                {
                    if (Tick != null)
                        Tick(this, new EventArgs());

                    time = Utils.Time.GetTime();
                    while (_lastTime + Interval < time)
                        _lastTime += Interval;
                }
            }
            _stopped.Set();
        }

    }
}
