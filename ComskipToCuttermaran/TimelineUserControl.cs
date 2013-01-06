using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using Maranate.Statistics;

namespace Maranate
{
    public partial class TimelineUserControl : UserControl
    {
        const int VIDEO_FRAME_COUNT = 7;
        const int SPLITTER_HEIGHT = 4;

        bool _dirty = false;
        int _fieldNumber = -1;
        double _secondsVisible = 0.0;

        object _dataLock = new object();
        object _drawLock = new object();
        Bitmap _backBuffer;
        Bitmap _backBufferCurrent;
        Size _backBufferSize;
        int _backBufferFlips = 0;
        int _backBufferFlipsDrawn = 0;

        RectangleF _timelineRectangle = new RectangleF();

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IStatisticsProcessor StatisticsProcessor { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TotalFields { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FieldsPerSecond { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FieldNumber
        {
            get
            {
                return _fieldNumber;
            }
            set
            {
                if (_fieldNumber != value)
                {
                    _fieldNumber = value;
                    SetDirty();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double SecondsVisible
        {
            get
            {
                return _secondsVisible;
            }
            set
            {
                if (_secondsVisible != value)
                {
                    _secondsVisible = value;
                    SetDirty();
                }
            }
        }

        public delegate void SelectedFieldChangedCallback(int fieldNumber);
        public event SelectedFieldChangedCallback SelectedFieldChanged;

        public TimelineUserControl()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void CloseFiles()
        {
            _backBufferThreadEndList.Add(_backBufferThread.ManagedThreadId);

            StatisticsProcessor = null;

            StartBackBufferThread();
        }

        private Thread _backBufferThread;
        private bool _backBufferThreadEnd = false;
        private HashSet<int> _backBufferThreadEndList = new HashSet<int>();
        private void TimelineUserControl_Load(object sender, EventArgs e)
        {
            if (this.ParentForm == null)
                return;

            this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);

            StartBackBufferThread();
        }

        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backBufferThreadEnd = true;
            _backBufferThread.Join();
        }

        void StartBackBufferThread()
        {
            RunOnBackgroundThread("Draw timeline back buffer", ref _backBufferThread, () =>
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                var lastUpdate = stopwatch.ElapsedMilliseconds;

                var threadId = Thread.CurrentThread.ManagedThreadId;

                while (!_backBufferThreadEnd && !_backBufferThreadEndList.Contains(threadId))
                {
                    if (_dirty)
                    {
                        var updateTime = stopwatch.ElapsedMilliseconds;
                        if ((updateTime - lastUpdate) < 200)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        lastUpdate = updateTime;

                        _dirty = false;

                        lock (_dataLock)
                        {
                            PrepareBackBuffer();
                        }
                        this.Invoke(new Action(() =>
                        {
                            this.Invalidate();
                        }));
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }

                _backBufferThreadEndList.Remove(threadId);
            });
        }

        public void SetDirty()
        {
            _dirty = true;
        }

        protected void RunOnBackgroundThread(string threadName, ref Thread targetThread, Action target)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    target();
                }
                catch (Exception e)
                {
                    this.Invoke(new Action(() =>
                    {
                        var form = FindForm();
                        ErrorForm.Show(form, e.ToString(), form.Text + " - Unhandled exception");
                    }));
                }
            }));
            if (threadName != null)
                thread.Name = threadName;
            targetThread = thread;
            thread.Start();
        }

        void _updateTimer_Tick(object sender, EventArgs e)
        {
            if (_backBufferFlipsDrawn != _backBufferFlips)
            {
                _backBufferFlipsDrawn = _backBufferFlips;
                this.Invalidate();
            }
        }

        private void TimelineUserControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (this.DesignMode)
            {
                var rect = new RectangleF(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height - SPLITTER_HEIGHT);

                using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, Color.Gray, Color.Black))
                {
                    g.FillRectangle(brush, rect);
                }

                var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("Timeline", this.Font, Brushes.White, rect, format);

                float yOffset = this.ClientRectangle.Height - SPLITTER_HEIGHT;
                DrawSplitter(g, ref yOffset);
                
                return;
            }

            lock (_drawLock)
            {
                if (_backBufferCurrent == null)
                {
                    g.FillRectangle(Brushes.Black, this.ClientRectangle);
                }
                else
                {
                    g.DrawImageUnscaled(_backBufferCurrent, 0, 0);
                }
            }
        }

        private void PrepareBackBuffer()
        {
            bool recreateBuffer = ((_backBuffer == null) || (_backBufferSize.Width != this.ClientSize.Width) || (_backBufferSize.Height != this.ClientSize.Height));
            if (recreateBuffer)
            {
                if (_backBuffer != null)
                    _backBuffer.Dispose();
                _backBufferSize = new Size(this.ClientSize.Width, this.ClientSize.Height);
                _backBuffer = new Bitmap(_backBufferSize.Width, _backBufferSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            using (var g = Graphics.FromImage(_backBuffer))
            {
                g.FillRectangle(Brushes.Black, this.ClientRectangle);

                if ((StatisticsProcessor == null) || (TotalFields == 0))
                    return;

                float yOffset = 0.0f;

                // Draw Timeline
                DrawTimeline(g, ref yOffset);
                DrawSplitter(g, ref yOffset);
            }

            var oldBuffer = _backBufferCurrent;
            _backBufferCurrent = _backBuffer;
            _backBuffer = oldBuffer;

            _backBufferFlips++;
        }

        void DrawSplitter(Graphics g, ref float yOffset)
        {
            var splitterRect = new RectangleF(0.0f, yOffset, this.ClientSize.Width, SPLITTER_HEIGHT);
            using (var splitterBrush = new System.Drawing.Drawing2D.LinearGradientBrush(splitterRect, Color.FromArgb(200, 200, 200), Color.FromArgb(80, 80, 80), 90.0f))
            {
                g.FillRectangle(splitterBrush, splitterRect);
            }
            yOffset += splitterRect.Height;

        }

        void DrawTimeline(Graphics g, ref float yOffset)
        {
            float height = this.ClientRectangle.Height - SPLITTER_HEIGHT;

            _timelineRectangle = new RectangleF(0.0f, yOffset, this.ClientSize.Width, height);

            Maranate.Statistics.Block currentBlock = null;
            foreach (var block in StatisticsProcessor.Data.Blocks.Reverse<Maranate.Statistics.Block>())
            {
                if ((currentBlock == null) && (block.StartFieldNumber <= FieldNumber))
                    currentBlock = block;
            }
            
            using (var font = new Font(this.Font.FontFamily, 10.0f, FontStyle.Bold))
            {
                // Fill Show blocks
                using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, Color.Gray, Color.Black))
                {
                    foreach (var block in StatisticsProcessor.Data.Blocks)
                    {
                        var isCommercial = ((block.IsCommercialOverride.HasValue) ? block.IsCommercialOverride.Value : block.IsCommercial);
                        if (!isCommercial)
                        {
                            float x1 = (block.StartFieldNumber / (float)TotalFields) * this.ClientSize.Width;
                            float x2 = (block.EndField.FieldNumber / (float)TotalFields) * this.ClientSize.Width;
                            var cutPointRect = new RectangleF(x1, yOffset, x2 - x1, height);
                            g.FillRectangle(brush, cutPointRect);
                        }
                    }
                }

                // Draw cut points
                foreach (var block in StatisticsProcessor.Data.Blocks)
                {
                    var brush = Brushes.Brown;
                    float x = (block.StartFieldNumber / (float)TotalFields) * this.ClientSize.Width;
                    var cutPointRect = new RectangleF(x, yOffset, 1.0f, height);
                    g.FillRectangle(brush, cutPointRect);

                    g.DrawString(block.BlockNumber.ToString(), font, brush, cutPointRect.Location);
                }

                // Draw current cut point
                if (currentBlock != null)
                {
                    var block = currentBlock;
                    var brush = Brushes.Orange;
                    float x = (block.StartFieldNumber / (float)TotalFields) * this.ClientSize.Width;
                    var cutPointRect = new RectangleF(x, yOffset, 1.0f, height);
                    g.FillRectangle(brush, cutPointRect);

                    g.DrawString(block.BlockNumber.ToString(), font, brush, cutPointRect.Location);
                }


                // Draw visible markers
                if (_secondsVisible > 0.0)
                {
                    var totalSeconds = (TotalFields / (float)FieldsPerSecond);
                    var pixelsPerSecond = this.ClientSize.Width / totalSeconds;
                    var pixelsVisibleOffset = _secondsVisible * pixelsPerSecond / 2.0;
                    if (pixelsVisibleOffset > 10)
                    {
                        var pixelOffsetCurrent = (int)((FieldNumber / (float)TotalFields) * this.ClientSize.Width) + 1;
                        var pixelOffsetStart = (int)(pixelOffsetCurrent - pixelsVisibleOffset);
                        var pixelOffsetEnd = (int)(pixelOffsetCurrent + pixelsVisibleOffset);
                        int size = 8;
                        int bottom = (int)yOffset + (int)height - 0;

                        var brush = Brushes.Wheat;
                        using (var pen = new Pen(Color.FromArgb(255, 243, 222)))
                        {
                            g.DrawLine(pen, pixelOffsetStart, bottom - 1, pixelOffsetEnd, bottom - 1);

                            if (pixelOffsetStart >= 0)
                            {
                                using (var path = new GraphicsPath())
                                {
                                    int x = (int)pixelOffsetStart;
                                    var pt1 = new Point(x, bottom - size);
                                    var pt2 = new Point(x + size, bottom);
                                    var pt3 = new Point(x, bottom);
                                    var points = new Point[] { pt2, pt3, pt1, pt2 };

                                    path.AddLines(points);
                                    g.FillPath(brush, path);
                                    g.DrawPath(pen, path);
                                }
                            }

                            if (pixelOffsetEnd <= this.ClientSize.Width)
                            {
                                using (var path = new GraphicsPath())
                                {
                                    int x = (int)pixelOffsetEnd;
                                    var pt1 = new Point(x, bottom - size);
                                    var pt2 = new Point(x, bottom);
                                    var pt3 = new Point(x - size, bottom);
                                    var points = new Point[] { pt2, pt3, pt1, pt2 };

                                    path.AddLines(points);
                                    g.FillPath(brush, path);
                                    g.DrawPath(pen, path);
                                }
                            }
                        }
                    }
                }

                // Draw current position marker
                {
                    using (var path = new GraphicsPath())
                    {
                        int x = (int)((FieldNumber / (float)TotalFields) * this.ClientSize.Width) + 1;
                        int bottom = (int)yOffset + (int)height - 0;
                        int size = 8;
                        var pt1 = new Point(x, bottom - size);
                        var pt2 = new Point(x + size, bottom);
                        var pt3 = new Point(x - size, bottom);
                        var points = new Point[] { pt2, pt3, pt1, pt2 };

                        path.AddLines(points);
                        g.FillPath(Brushes.Wheat, path);
                        g.DrawPath(Pens.White, path);
                    }
                }

            }

            yOffset += height;
        }

        private void TimelineUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (_timelineRectangle.Contains(e.X, e.Y))
            {
                var perc = (e.X - _timelineRectangle.X) / _timelineRectangle.Width;
                var fieldNumber = TotalFields * perc;
                if (SelectedFieldChanged != null)
                    SelectedFieldChanged((int)fieldNumber);
            }
        }

    }
}
