using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utils
{
    public class ProgressCounter : IDisposable
    {
        private static Dictionary<int, ProgressCounter> _threads = new Dictionary<int, ProgressCounter>();

        public ProgressCounter Top
        {
            get
            {
                return GetTop(_workerThread);
            }
            private set
            {
                if (value == null)
                    _threads.Remove(_workerThread.ManagedThreadId);
                else
                    _threads[_workerThread.ManagedThreadId] = value;
            }
        }
        public static ProgressCounter GetTop(Thread thread)
        {
            int threadId = thread.ManagedThreadId;
            ProgressCounter counter;
            _threads.TryGetValue(threadId, out counter);
            return counter;
        }

        private Thread _workerThread = null;
        private ProgressCounter _childCounter = null;
        private ProgressCounter _parentCounter = null;

        private int _value = 0;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                ProcessProgressChanged();
            }
        }

        private int _incrementBy = 1;
        public int IncrementBy
        {
            get
            {
                return _incrementBy;
            }
            set
            {
                _incrementBy = value;
                if (_incrementBy <= 0)
                    _incrementBy = 1;
                ProcessProgressChanged();
            }
        }

        private int _maximum = 100;
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
                ProcessProgressChanged();
            }
        }

        private int _roundToNumberOfDigits = 3;
        public int RoundToNumberOfDigits
        {
            get
            {
                return _roundToNumberOfDigits;
            }
            set
            {
                _roundToNumberOfDigits = value;
            }
        }

        private string _status = "";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                ProcessStatusChanged();
            }
        }

        public ProgressCounter()
            : this(100)
        {
        }
        public ProgressCounter(int maximumValue, int initialValue = 0, int incrementBy = 1)
            : this(Thread.CurrentThread, maximumValue, initialValue, incrementBy)
        {
        }
        public ProgressCounter(Thread workerThread, int maximumValue, int initialValue = 0, int incrementBy = 1)
        {
            _workerThread = workerThread;

            _childProgressChangedHandler = new ProgressChangedHandler(_childCounter_ProgressChanged);
            _childStatusChangedHandler = new StatusChangedHandler(_childCounter_StatusChanged);
            _value = initialValue;
            _incrementBy = incrementBy;
            _maximum = maximumValue;

            ProgressCounter parentCounter = Top;
            if (parentCounter == null)
            {
                // Set top level counter
                Top = this;
            }
            else
            {
                // Add to stack
                while (parentCounter._childCounter != null)
                    parentCounter = parentCounter._childCounter;

                if (parentCounter != null)
                {
                    parentCounter.AddChild(this);
                }
            }
        }

        public bool Aborted { get; private set; }
        public bool IsDisposed { get; private set; }
        public event EventHandler Disposed;
        public event EventHandler Complete;
        public event EventHandler Cancelled;

        public void Dispose()
        {
            if (Aborted)
            {
                if (Cancelled != null)
                    Cancelled(this, new EventArgs());
            }
            else
            {
                if (Complete != null)
                    Complete(this, new EventArgs());
            }

            if (_parentCounter != null)
                _parentCounter.RemoveChild();
            if (Top == this)
                Top = null;

            IsDisposed = true;
            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

        public void Abort()
        {
            Aborted = true;
            _workerThread.Abort();
        }

        public bool Increment()
        {
            Interlocked.Add(ref _value, _incrementBy);
            ProcessProgressChanged();
            return true;
        }
        public void SetValue(int value)
        {
            _value = value;
            ProcessProgressChanged();
        }
        public void SetValue(int value, int nextValue)
        {
            _value = value;
            _incrementBy = nextValue - value;
            ProcessProgressChanged();
        }
        public void SetStatus(string status)
        {
            _status = status;
            ProcessStatusChanged();
        }

        public double GetProgress()
        {
            double progress = _value / (double)_maximum;

            if (_childCounter != null)
            {
                double range = _incrementBy;
                if (range <= 0.0)
                    range = 1.0;
                range /= _maximum;
                progress += (_childCounter.GetProgress() * range);
            }

            progress = Math.Round(progress, _roundToNumberOfDigits);

            if (progress > 1.0)
                return 1.0;
            if (progress < 0)
                return 0;
            return progress;
        }

        public List<string> GetStatusList()
        {
            var statusList = new List<string>();
            for (var child = this; child != null; child = child._childCounter)
            {
                if (!string.IsNullOrEmpty(child.Status))
                    statusList.Add(child.Status);
            }
            return statusList;
        }

        public void AddChild(ProgressCounter child)
        {
            if (_childCounter == child)
                return;
            RemoveChild();
            _childCounter = child;
            _childCounter.ProgressChanged += _childProgressChangedHandler;
            _childCounter.StatusChanged += _childStatusChangedHandler;
            _childCounter._parentCounter = this;
        }

        public void RemoveChild()
        {
            if (_childCounter != null)
            {
                _childCounter._parentCounter = null;
                _childCounter.ProgressChanged -= _childProgressChangedHandler;
                _childCounter.StatusChanged -= _childStatusChangedHandler;
                _childCounter = null;

                ProcessStatusChanged();
            }
        }

        private void _childCounter_ProgressChanged(double percent)
        {
            ProcessProgressChanged();
        }

        private void _childCounter_StatusChanged(List<string> status)
        {
            ProcessStatusChanged();
        }

        private double _lastPercent = -1.0;
        private void ProcessProgressChanged()
        {
            double progress = GetProgress();
            if (_lastPercent != progress)
            {
                _lastPercent = progress;
                if (ProgressChanged != null)
                    ProgressChanged(progress);
            }
        }

        private string _lastStatus = null;
        private void ProcessStatusChanged()
        {
            var statusList = GetStatusList();
            string status = string.Join("|", statusList.ToArray());
            if (_lastStatus != status)
            {
                _lastStatus = status;
                if (statusList.Count == 0)
                    statusList.Add("");
                if (StatusChanged != null)
                    StatusChanged(statusList);
            }
        }

        public delegate void ProgressChangedHandler(double percent);
        public event ProgressChangedHandler ProgressChanged;
        private ProgressChangedHandler _childProgressChangedHandler;

        public delegate void StatusChangedHandler(List<string> status);
        public event StatusChangedHandler StatusChanged;
        private StatusChangedHandler _childStatusChangedHandler;

    }

}
