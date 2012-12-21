using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils
{
    public class Time
    {
        private static long ticksPerSecond;
        private static long baseTicks;

        private Time() { }

        static Time()
        {
            QueryPerformanceFrequency(ref ticksPerSecond);
            QueryPerformanceCounter(ref baseTicks);
        }

        public static double GetTime()
        {
            long time = 0;
            QueryPerformanceCounter(ref time);
            double preciseTime = (time - baseTicks) * 1000.0 / (double)ticksPerSecond;
            return preciseTime;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(ref long PerformanceCount);
    }

    public class PerformanceTimer
    {
        List<double> _times;
        List<string> _names;

        public PerformanceTimer()
        {
#if DEBUG
            _times = new List<double>();
            _names = new List<string>();

            _times.Add(Time.GetTime());
            _names.Add("Start");
#endif
        }

        public void Add(string name)
        {
#if DEBUG
            _times.Add(Time.GetTime());
            _names.Add(name);
#endif
        }

        public string[] Times
        {
            get
            {
                List<string> result = new List<string>();
                for (int i = 1; i < _times.Count; i++)
                {
                    double time = _times[i] - _times[0];
                    result.Add(_names[i] + " : " + time.ToString("0.00") + "ms");
                }
                return result.ToArray();
            }
        }

        public string[] Deltas
        {
            get
            {
                List<string> result = new List<string>();
                for (int i = 1; i < _times.Count; i++)
                {
                    double time = _times[i] - _times[i - 1];
                    result.Add(_names[i] + " : " + time.ToString("0.00") + "ms");
                }
                return result.ToArray();
            }
        }

    }
}
