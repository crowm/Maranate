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

namespace ComskipToCuttermaran
{
    public partial class TimelineUserControl : UserControl
    {
        const int VIDEO_FRAME_COUNT = 7;

        bool _dirty = false;
        int _frameNumber = -1;
        double _secondsVisible = 10.0;
        List<MediaFile.Frame> _frames = new List<MediaFile.Frame>();

        object _dataLock = new object();
        object _drawLock = new object();
        Bitmap _backBuffer;
        Bitmap _backBufferCurrent;
        Size _backBufferSize;

        RectangleF _timelineRectangle = new RectangleF();
        RectangleF _graphRectangle = new RectangleF();

        public ComskipCsvProcessor CsvProcessor { get; set; }
        public MediaFile VideoMediaFile { get; set; }
        public int FrameNumber
        {
            get
            {
                return _frameNumber;
            }
            set
            {
                if (_frameNumber != value)
                {
                    _frameNumber = value;
                    SetDirty();
                }
            }
        }

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

        public delegate void SelectedFrameChangedCallback(int frameNumber);
        public event SelectedFrameChangedCallback SelectedFrameChanged;

        public TimelineUserControl()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
        }

        public void CloseFiles()
        {
            _backBufferThreadEnd = true;
            _backBufferThread.Join();
            _backBufferThreadEnd = false;

            CsvProcessor = null;
            if (VideoMediaFile != null)
            {
                VideoMediaFile.Dispose();
                VideoMediaFile = null;
            }

            foreach (var frame in _frames)
            {
                if (frame != null)
                    frame.Dispose();
            }
            _frames.Clear();

            StartBackBufferThread();
        }

        private Thread _backBufferThread;
        private bool _backBufferThreadEnd = false;
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
                while (!_backBufferThreadEnd)
                {
                    if (_dirty)
                    {
                        _dirty = false;

                        lock (_dataLock)
                        {
                            PrepareData();
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

        private void TimelineUserControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
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

        private void PrepareData()
        {
            if ((CsvProcessor == null) || (VideoMediaFile == null))
                return;

            int frameInterval = (int)((_secondsVisible / (double)VIDEO_FRAME_COUNT) / VideoMediaFile.FrameDuration);
            int firstFrame = _frameNumber - (VIDEO_FRAME_COUNT / 2) * frameInterval;
            int lastFrame = _frameNumber + (VIDEO_FRAME_COUNT / 2) * frameInterval;

            var oldFrames = _frames;
            foreach (var frame in oldFrames)
            {
                if (frame != null)
                    frame.Dispose();
            }

            var frameIndexes = new List<int>();
            for (int n = firstFrame; n <= lastFrame; n += frameInterval)
                frameIndexes.Add(n);
            _frames = VideoMediaFile.GetVideoFrames(frameIndexes);

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

                if ((CsvProcessor == null) || (VideoMediaFile == null))
                    return;

                float yOffset = 0.0f;

                // Draw Timeline
                DrawTimeline(g, ref yOffset);
                DrawSplitter(g, ref yOffset);

                var yOffsetCurrentMarker = yOffset;

                // Draw frames
                DrawFrames(g, ref yOffset);
                DrawSplitter(g, ref yOffset);

                // Draw Statistics
                DrawStatistics(g, ref yOffset);

                using (var pen = new Pen(Brushes.Gray))
                {
                    pen.DashStyle = DashStyle.Dot;
                    var x = (this.ClientSize.Width + 1) / 2;
                    g.DrawLine(pen, x, yOffsetCurrentMarker, x, this.ClientSize.Height);
                }
            }

            var oldBuffer = _backBufferCurrent;
            _backBufferCurrent = _backBuffer;
            _backBuffer = oldBuffer;
        }

        void DrawSplitter(Graphics g, ref float yOffset)
        {
            var splitterRect = new RectangleF(0.0f, yOffset, this.ClientSize.Width, 4.0f);
            using (var splitterBrush = new System.Drawing.Drawing2D.LinearGradientBrush(splitterRect, Color.FromArgb(200, 200, 200), Color.FromArgb(80, 80, 80), 90.0f))
            {
                g.FillRectangle(splitterBrush, splitterRect);
            }
            yOffset += splitterRect.Height;

        }

        void DrawTimeline(Graphics g, ref float yOffset)
        {
            float height = 30.0f;

            _timelineRectangle = new RectangleF(0.0f, yOffset, this.ClientSize.Width, height);

            ComskipCsvProcessor.Block currentBlock = null;
            foreach (var block in CsvProcessor.Data.Blocks.Reverse<ComskipCsvProcessor.Block>())
            {
                if ((currentBlock == null) && (block.StartFrameNumber <= FrameNumber))
                    currentBlock = block;
            }
            
            using (var font = new Font(this.Font.FontFamily, 10.0f, FontStyle.Bold))
            {
                // Fill Show blocks
                using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, Color.Gray, Color.Black))
                {
                    foreach (var block in CsvProcessor.Data.Blocks)
                    {
                        var isCommercial = ((block.IsCommercialOverride.HasValue) ? block.IsCommercialOverride.Value : block.IsCommercial);
                        if (!isCommercial)
                        {
                            float x1 = (block.StartFrameNumber / (float)VideoMediaFile.TotalFrames) * this.ClientSize.Width;
                            float x2 = (block.EndFrame.FrameNumber / (float)VideoMediaFile.TotalFrames) * this.ClientSize.Width;
                            var cutPointRect = new RectangleF(x1, yOffset, x2 - x1, height);
                            g.FillRectangle(brush, cutPointRect);
                        }
                    }
                }

                // Draw cut points
                foreach (var block in CsvProcessor.Data.Blocks)
                {
                    var brush = Brushes.Brown;
                    float x = (block.StartFrameNumber / (float)VideoMediaFile.TotalFrames) * this.ClientSize.Width;
                    var cutPointRect = new RectangleF(x, yOffset, 1.0f, height);
                    g.FillRectangle(brush, cutPointRect);

                    g.DrawString(block.BlockNumber.ToString(), font, brush, cutPointRect.Location);
                }

                // Draw current cut point
                if (currentBlock != null)
                {
                    var block = currentBlock;
                    var brush = Brushes.Orange;
                    float x = (block.StartFrameNumber / (float)VideoMediaFile.TotalFrames) * this.ClientSize.Width;
                    var cutPointRect = new RectangleF(x, yOffset, 1.0f, height);
                    g.FillRectangle(brush, cutPointRect);

                    g.DrawString(block.BlockNumber.ToString(), font, brush, cutPointRect.Location);
                }


                // Draw current position marker
                {
                    using (var path = new GraphicsPath())
                    {
                        int x = (int)((FrameNumber / (float)VideoMediaFile.TotalFrames) * this.ClientSize.Width) + 1;
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

        void DrawFrames(Graphics g, ref float yOffset)
        {
            if (_frames.Count == 0)
                return;

            float frameWidth = this.ClientSize.Width / (float)VIDEO_FRAME_COUNT;
            float frameHeight = frameWidth * 9 / 16;

            for (int frameIndex = 0; frameIndex < VIDEO_FRAME_COUNT; frameIndex++)
            {
                float x = frameIndex * frameWidth;
                var frame = _frames[frameIndex];
                var frameRect = new RectangleF(x, yOffset, frameWidth - 1.0f, frameHeight - 1.0f);
                if ((frame != null) && (frame.Image != null))
                {
                    g.DrawImage(frame.Image, frameRect);
                }
                else
                {
                    using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.Black))
                    {
                        g.FillRectangle(brush, frameRect);
                    }
                }
            }
            yOffset += frameHeight;
        }

        void DrawStatistics(Graphics g, ref float yOffset)
        {
            float height = 6.0f;

            _graphRectangle = new RectangleF(0.0f, yOffset, this.ClientSize.Width, this.ClientSize.Height - yOffset);

            int frameInterval = (int)((_secondsVisible / (double)VIDEO_FRAME_COUNT) / VideoMediaFile.FrameDuration);
            int firstFrameIndex = _frameNumber - (VIDEO_FRAME_COUNT / 2) * frameInterval;
            int lastFrameIndex = _frameNumber + (VIDEO_FRAME_COUNT / 2) * frameInterval;
            int frameCount = lastFrameIndex - firstFrameIndex;

            using (var centreFrame = VideoMediaFile.GetVideoFrame(_frameNumber))
            {
                using (var font = new Font(this.Font.FontFamily, 8.0f, FontStyle.Regular))
                {
                    int currSecond = -1;
                    for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; frameIndex++)
                    {
                        if ((frameIndex < 0) || (frameIndex >= VideoMediaFile.TotalFrames))
                            continue;

                        // Draw ticks and time
                        var seconds = ((frameIndex - _frameNumber) * VideoMediaFile.FrameDuration) + centreFrame.Seconds;

                        float dx = this.ClientSize.Width / (float)frameCount;
                        float x = (frameIndex - firstFrameIndex) * dx;

                        var tickHeight = height;
                        bool printTime = false;
                        if (seconds >= (currSecond + 1))
                        {
                            if ((currSecond != -1) || (frameIndex > firstFrameIndex))
                            {
                                tickHeight *= 2;
                                printTime = true;
                            }
                            currSecond = (int)Math.Floor(seconds);
                        }

                        var brush = Brushes.White;
                        var tickRect = new RectangleF(x, yOffset, 1.0f, tickHeight);
                        g.FillRectangle(brush, tickRect);

                        if (printTime)
                        {
                            tickRect.Offset(1.0f, height);
                            var secondsTimeSpan = TimeSpan.FromSeconds(seconds);
                            g.DrawString(secondsTimeSpan.ToString(@"h\:mm\:ss"), font, brush, tickRect.Location);
                        }

                    }
                }
            }

            yOffset += height;

            // Draw graphs
            DrawGraph(
                g,
                ref yOffset,
                CsvProcessor.Data.WholeFileAverages.AverageBrightness,
                ComskipCsvProcessor.ColumnId.Brightness,
                CsvProcessor.Data.WholeFileAverages.AverageBrightness * 2,
                CsvProcessor.Data.BrightnessThreshold,
                Pens.Red);

            DrawSplitter(g, ref yOffset);

            DrawGraph(
                g,
                ref yOffset,
                CsvProcessor.Data.WholeFileAverages.AverageUniform,
                ComskipCsvProcessor.ColumnId.Uniform,
                CsvProcessor.Data.WholeFileAverages.AverageUniform * 2,
                CsvProcessor.Data.UniformThreshold,
                Pens.Yellow);

            DrawSplitter(g, ref yOffset);

            DrawGraph(
                g,
                ref yOffset,
                CsvProcessor.Data.WholeFileAverages.AverageSound,
                ComskipCsvProcessor.ColumnId.Sound,
                CsvProcessor.Data.WholeFileAverages.AverageSound * 2,
                CsvProcessor.Data.SoundThreshold,
                Pens.Green);

            DrawSplitter(g, ref yOffset);
        }

        private void DrawGraph(Graphics g, ref float yOffset, double wholeFileAverage, ComskipCsvProcessor.ColumnId columnId, double maxValue, double threshold, Pen graphPen)
        {
            const int graphHeight = 64;

            int frameInterval = (int)((_secondsVisible / (double)VIDEO_FRAME_COUNT) / VideoMediaFile.FrameDuration);
            int firstFrameIndex = _frameNumber - (VIDEO_FRAME_COUNT / 2) * frameInterval;
            int lastFrameIndex = _frameNumber + (VIDEO_FRAME_COUNT / 2) * frameInterval;
            int frameCount = lastFrameIndex - firstFrameIndex;

            g.DrawLine(Pens.DarkSlateGray, 0.0f, yOffset + graphHeight, this.ClientSize.Width, yOffset + graphHeight);

            {
                var y = yOffset + (float)((maxValue - wholeFileAverage) / (maxValue / (float)graphHeight));
                if (y < yOffset)
                    y = yOffset;
                using (var pen = new Pen(Color.DarkSlateGray))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, 0.0f, y, this.ClientSize.Width, y);
                }
            }

            for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; frameIndex++)
            {
                if ((frameIndex < 0) || (frameIndex >= CsvProcessor.Data.Frames.Count))
                    continue;

                float dx = this.ClientSize.Width / (float)frameCount;
                float x = (frameIndex - firstFrameIndex) * dx;

                var frame = CsvProcessor.Data.Frames[frameIndex];
                if (frame != null)
                {
                    var value = frame.Values[columnId];
                    var y = yOffset + (float)((maxValue - value) / (maxValue / (float)graphHeight));
                    if (value <= threshold)
                    {
                        g.DrawLine(Pens.Wheat, x, y, x + dx, y);
                    }
                    else if (y < yOffset)
                    {
                        y = yOffset;
                        g.DrawLine(Pens.Wheat, x, y, x + dx, y);
                    }
                    else
                    {
                        g.DrawLine(graphPen, x, y, x + dx, y);
                    }
                }

            }

            yOffset += graphHeight;
        }

        private void TimelineUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (_timelineRectangle.Contains(e.X, e.Y))
            {
                var perc = (e.X - _timelineRectangle.X) / _timelineRectangle.Width;
                var frameNumber = VideoMediaFile.TotalFrames * perc;
                if (SelectedFrameChanged != null)
                    SelectedFrameChanged((int)frameNumber);
            }
            if (_graphRectangle.Contains(e.X, e.Y))
            {
                int frameInterval = (int)((_secondsVisible / (double)VIDEO_FRAME_COUNT) / VideoMediaFile.FrameDuration);
                int firstFrameIndex = _frameNumber - (VIDEO_FRAME_COUNT / 2) * frameInterval;
                int lastFrameIndex = _frameNumber + (VIDEO_FRAME_COUNT / 2) * frameInterval;
                int frameCount = lastFrameIndex - firstFrameIndex;

                var perc = (e.X - _graphRectangle.X) / _graphRectangle.Width;
                var frameNumber = frameCount * perc + firstFrameIndex;
                if (frameNumber < 0)
                    frameNumber = 0;
                if (frameNumber >= VideoMediaFile.TotalFrames)
                    frameNumber = VideoMediaFile.TotalFrames - 1;
                if (SelectedFrameChanged != null)
                    SelectedFrameChanged((int)frameNumber);
            }
        }

    }
}
