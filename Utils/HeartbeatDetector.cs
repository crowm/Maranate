using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.Threading;

#pragma warning disable 0618

namespace Utils
{
    public class HeartbeatDetector
    {
        private static Dictionary<int, HeartbeatDetector> _detectors = new Dictionary<int, HeartbeatDetector>();
        private static Thread _heartbeatThread = null;
        private static object _heartbeatThreadLock = new object();
        private static bool _heartbeatDetectorEnabled = true;
        private static ManualResetEvent _shutdownEvent = new ManualResetEvent(false);

        public static void Shutdown()
        {
            lock (_heartbeatThreadLock)
            {
                if (_heartbeatThread != null)
                {
                    _heartbeatDetectorEnabled = false;
                    _shutdownEvent.Set();
                    _heartbeatThread.Join();
                    _heartbeatThread = null;
                }
            }
        }

        private static void CheckHeartbeatThreadExists()
        {
            lock (_heartbeatThreadLock)
            {
                if (_heartbeatThread == null)
                {
                    _heartbeatThread = new Thread(HeartbeatThreadProc);
                    _heartbeatThread.Name = "HeartbeatDetector";
                    _heartbeatThread.IsBackground = true;
                    _heartbeatThread.Start();
                }
            }
        }

        private static HeartbeatDetector GetDetector()
        {
            HeartbeatDetector detector = null;
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            lock (_detectors)
            {
                if (_detectors.ContainsKey(threadId))
                {
                    detector = _detectors[threadId];
                }
                else
                {
                    detector = new HeartbeatDetector();
                    _detectors.Add(threadId, detector);
                }
            }
            return detector;
        }

        public static void Beat(int maxTimeToNextBeat)
        {
            if (_heartbeatDetectorEnabled == false)
                return;

            CheckHeartbeatThreadExists();
            HeartbeatDetector detector = GetDetector();
            lock (detector)
            {
                double currTime = Time.GetTime();

                if (detector._overdueLogged)
                {
                    int overdue = (int)detector.GetOverdue(currTime);
                    Log.MainLog.WriteDebug("Thread " + detector._thread.ManagedThreadId + " (" + detector._thread.Name + ") heartbeat overdue by " + overdue.ToString() + "ms.");
                    Log.MainLog.WriteDebug(detector._overdueStackTrace + Environment.NewLine + Environment.NewLine);
                }

                detector._lastBeatTime = currTime;
                detector._maxTimeToNextBeat = maxTimeToNextBeat;
                detector._overdueLogged = false;
                detector._suspended = false;
            }
        }

        public static void Close()
        {
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            lock (_detectors)
            {
                if (_detectors.ContainsKey(threadId))
                {
                    HeartbeatDetector detector = null;
                    detector = _detectors[threadId];
                    lock (detector)
                    {
                        _detectors.Remove(threadId);
                    }
                }
            }
        }

        public static void Suspend()
        {
            if (_heartbeatDetectorEnabled == false)
                return;

            CheckHeartbeatThreadExists();
            HeartbeatDetector detector = GetDetector();
            lock (detector)
            {
                detector._suspended = true;
            }
        }

        public static void Resume()
        {
            if (_heartbeatDetectorEnabled == false)
                return;

            CheckHeartbeatThreadExists();
            HeartbeatDetector detector = GetDetector();
            lock (detector)
            {
                detector._lastBeatTime = Time.GetTime();
                detector._suspended = false;
            }
        }

        private static void HeartbeatThreadProc()
        {
            while (true)
            {
                if (_shutdownEvent.WaitOne(10, false))
                    break;

                double currTime = Time.GetTime();

                List<HeartbeatDetector> overdueHeartbeats = new List<HeartbeatDetector>();

                lock (_detectors)
                {
                    List<int> deadThreads = new List<int>();
                    foreach (HeartbeatDetector detector in _detectors.Values)
                    {
                        if (detector._thread.IsAlive == false)
                            deadThreads.Add(detector._thread.ManagedThreadId);
                    }
                    foreach (int threadId in deadThreads)
                    {
                        _detectors.Remove(threadId);
                    }

                    foreach (HeartbeatDetector detector in _detectors.Values)
                    {
                        if ((detector._lastBeatTime > 0.0) &&
                            (detector._maxTimeToNextBeat > 0.0) &&
                            (detector._overdueLogged == false) &&
                            (detector._suspended == false))
                        {
                            int overdue = (int)detector.GetOverdue(currTime);
                            if (overdue > 0)
                            {
                                overdueHeartbeats.Add(detector);
                                detector._overdueLogged = true;
                            }
                        }
                    }
                }

                foreach (HeartbeatDetector detector in overdueHeartbeats)
                {
                    if (_shutdownEvent.WaitOne(0, false))
                        break;

                    Thread thread;
                    double overdue;
                    lock (detector)
                    {
                        overdue = detector.GetOverdue(currTime);
                        thread = detector._thread;

                        if (thread.IsAlive)
                        {
                            //Logging.Log.MainLog.WriteDebug("Thread " + thread.ManagedThreadId + " heartbeat overdue by " + ((int)overdue).ToString() + "ms.  ThreadName=" + thread.Name);

                            //Logging.Log.MainLog.WriteDebug(GetStackTrace(thread) + Environment.NewLine + Environment.NewLine);
                            detector._overdueStackTrace = GetStackTrace(thread);
                        }
                    }
                }


            }
        }

        private static string GetStackTrace(Thread thread)
        {
            StringBuilder text = new StringBuilder();

            try
            {
                System.Diagnostics.StackTrace stackTrace;

                if (thread != System.Threading.Thread.CurrentThread)
                {
                    thread.Suspend();

                    int retryCount = 0;
                    System.Threading.ThreadState state;
                    while (true)
                    {
                        state = System.Threading.ThreadState.Suspended;
                        if ((thread.ThreadState & state) == state)
                            break;

                        state = ThreadState.WaitSleepJoin | ThreadState.SuspendRequested;
                        if ((thread.ThreadState & state) == state)
                            break;

                        Thread.Sleep(10);
                        retryCount++;
                        if (retryCount > 50)
                            return "Failed to suspend thread: " + thread.ThreadState.ToString();
                    }
                    stackTrace = new System.Diagnostics.StackTrace(thread, true);
                }
                else
                {
                    stackTrace = new System.Diagnostics.StackTrace(1, true);
                }
                //string fullTrace = stackTrace.ToString();

                int currFrameID = 0;
                if (thread == System.Threading.Thread.CurrentThread)
                    currFrameID = 1;

                while (currFrameID < stackTrace.FrameCount)
                {
                    System.Diagnostics.StackFrame sf = stackTrace.GetFrame(currFrameID);

                    MethodBase method = sf.GetMethod();
                    string declaringType = "<unknown>";
                    if (method.DeclaringType != null)
                        declaringType = method.DeclaringType.FullName;

                    StringBuilder methodName = new StringBuilder(declaringType + "." + method.Name + "(");

                    ParameterInfo[] parameters = method.GetParameters();
                    foreach (ParameterInfo param in parameters)
                    {
                        if (!methodName.ToString().EndsWith("("))
                            methodName.Append(", ");
                        if (param.IsOptional)
                            methodName.Append("Optional ");

                        methodName.Append(param.ParameterType.Name);
                    }
                    methodName.Append(")");

                    text.Append("  " + methodName);

                    int lineNumber = sf.GetFileLineNumber();
                    if (lineNumber > 0)
                    {
                        text.Append(" : " + lineNumber.ToString());
                    }

                    text.Append(Environment.NewLine);

                    currFrameID += 1;
                }
            }
            catch (Exception e)
            {
                return "Exception getting stack trace. Thread: " + thread.Name + ", Error: " + e.Message;
            }
            finally
            {
                try
                {
                    thread.Resume();
                }
                catch
                {
                }
            }

            return text.ToString();
        }



        private Thread _thread;
        private double _lastBeatTime = 0;
        private long _maxTimeToNextBeat = 0;
        private bool _overdueLogged = false;
        private bool _suspended = false;
        private string _overdueStackTrace = "";

        private HeartbeatDetector()
        {
            _thread = System.Threading.Thread.CurrentThread;
        }

        private double GetOverdue(double currTime)
        {
            return currTime - (_lastBeatTime + _maxTimeToNextBeat);
        }

    }
}
