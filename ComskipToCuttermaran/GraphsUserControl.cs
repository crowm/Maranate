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
using ComskipToCuttermaran.Statistics;

namespace ComskipToCuttermaran
{
    public partial class GraphsUserControl : UserControl
    {
        const int VIDEO_FRAME_COUNT = 7;

        const int ZOOM_LEVEL_CLOSE = 2;

        bool _dirty = false;
        int _fieldNumber = 0;
        double _secondsVisible = 14.0;
        int _zoomLevel = ZOOM_LEVEL_CLOSE;

        List<MediaFile.FrameField> _fields = new List<MediaFile.FrameField>();

        object _dataLock = new object();
        object _drawLock = new object();
        Bitmap _backBuffer;
        Bitmap _backBufferCurrent;
        int _backBufferFlips = 0;
        int _backBufferFlipsDrawn = 0;

        RectangleF _graphRectangle = new RectangleF();

        int _fieldInterval;
        int _firstFieldIndex;
        int _lastFieldIndex;
        int _fieldCount;
        float _field_dx;

        System.Windows.Forms.Timer _updateTimer;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IStatisticsProcessor StatisticsProcessor { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MediaFile VideoMediaFile { get; set; }

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
        public int ZoomLevel
        {
            get
            {
                return _zoomLevel;
            }
            set
            {
                if (_zoomLevel != value)
                {
                    _zoomLevel = value;
                    UpdateSecondsVisible();
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
        }

        public delegate void SelectedFieldChangedCallback(int fieldNumber);
        public event SelectedFieldChangedCallback SelectedFieldChanged;

        public GraphsUserControl()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            //this.SetStyle(ControlStyles.UserPaint, true);

            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 40;
            _updateTimer.Tick += new EventHandler(_updateTimer_Tick);
            _updateTimer.Start();
        }

        public void CloseFiles()
        {
            _backBufferThreadEndList.Add(_backBufferThread.ManagedThreadId);
            _backBufferThread.Join();

            StatisticsProcessor = null;
            if (VideoMediaFile != null)
            {
                VideoMediaFile.Dispose();
                VideoMediaFile = null;
            }

            var oldFields = _fields;
            _fields = new List<MediaFile.FrameField>();
            foreach (var field in oldFields)
            {
                if (field != null)
                    field.Dispose();
            }
            oldFields.Clear();

            StartBackBufferThread();
        }

        private Thread _backBufferThread;
        private bool _backBufferThreadEnd = false;
        private HashSet<int> _backBufferThreadEndList = new HashSet<int>();
        private void GraphsUserControl_Load(object sender, EventArgs e)
        {
            if (this.ParentForm == null)
                return;

            AdjustHeight();

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
                        if ((updateTime - lastUpdate) < 500)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        lastUpdate = updateTime;

                        _dirty = false;

                        lock (_dataLock)
                        {
                            PrepareData();
                            PrepareBackBuffer();
                        }
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
                var rect = new RectangleF(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);

                using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent10, Color.DimGray, Color.Black))
                {
                    g.FillRectangle(brush, rect);
                }

                var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("Graphs", this.Font, Brushes.White, rect, format);

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

        private void PrepareData()
        {
            if ((StatisticsProcessor == null) || (VideoMediaFile == null))
                return;

            _fieldInterval = (int)((_secondsVisible / (double)VIDEO_FRAME_COUNT) / VideoMediaFile.FieldDuration);
            _firstFieldIndex = _fieldNumber - (_fieldInterval * VIDEO_FRAME_COUNT / 2);
            _lastFieldIndex = _fieldNumber + (_fieldInterval * VIDEO_FRAME_COUNT / 2);
            _fieldCount = _lastFieldIndex - _firstFieldIndex;
            _field_dx = this.ClientSize.Width / (float)_fieldCount;

            var oldFields = _fields;
            foreach (var field in oldFields)
            {
                if (field != null)
                {
                    field.Dispose();
                }
            }

            var fieldIndexes = new List<int>();
            for (int n = _firstFieldIndex; n < _lastFieldIndex; n += _fieldInterval)
                fieldIndexes.Add(n + (int)Math.Ceiling(_fieldInterval / 2.0));
            _fields = VideoMediaFile.GetVideoFrameFields(fieldIndexes);
            if (_fields.Count > 3)
            {
                var field = _fields[3];
                if (field == null)
                    Console.WriteLine("!!!! Middle field is null");
            }

        }

        private void PrepareBackBuffer()
        {
            bool recreateBuffer = ((_backBuffer == null) || (_backBuffer.Width != this.ClientSize.Width) || (_backBuffer.Height != this.ClientSize.Height));
            if (recreateBuffer)
            {
                if (_backBuffer != null)
                    _backBuffer.Dispose();
                _backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            int height;
            DrawToImage(_backBuffer, out height);

            var oldBuffer = _backBufferCurrent;
            _backBufferCurrent = _backBuffer;
            _backBuffer = oldBuffer;

            _backBufferFlips++;
        }

        private void DrawToImage(Bitmap image, out int height)
        {
            using (var g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.Black, this.ClientRectangle);

                float yOffset = 0.0f;

                // Draw frames
                DrawFrames(g, ref yOffset);
                DrawSplitter(g, ref yOffset);

                if ((VideoMediaFile != null) && (StatisticsProcessor != null))
                {
                    // Draw current marker
                    var markerColor = Color.FromArgb(160, 160, 160);
                    using (var brush = new HatchBrush(HatchStyle.LightDownwardDiagonal, markerColor))
                    {
                        var x = (this.ClientSize.Width + 1) / 2.0f;
                        var width = (_field_dx >= 1.0f) ? _field_dx : 1.0f;
                        var rect = Rectangle.FromLTRB((int)x, (int)yOffset, (int)(x + width), this.ClientSize.Height);
                        g.FillRectangle(brush, rect);
                        using (var pen = new Pen(markerColor))
                        {
                            g.DrawRectangle(pen, rect);
                        }
                    }
                }

                // Draw Statistics
                DrawStatistics(g, ref yOffset);

                height = (int)yOffset;
            }
        }

        private void GraphsUserControl_Resize(object sender, EventArgs e)
        {
            AdjustHeight();
            UpdateSecondsVisible();
        }

        private int _lastCalculatedWidth = -1;
        private void AdjustHeight()
        {
            if ((_lastCalculatedWidth != this.Width) && !this.DesignMode)
            {
                _lastCalculatedWidth = this.Width;
                var image = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int height;
                DrawToImage(image, out height);

                this.Height = height;

                SetDirty();
            }
        }

        private void UpdateSecondsVisible()
        {
            if (_zoomLevel == ZOOM_LEVEL_CLOSE)
                _secondsVisible = 14.0;
            else
                _secondsVisible = this.ClientSize.Width;
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

        void DrawFrames(Graphics g, ref float yOffset)
        {
            float frameWidth = this.ClientSize.Width / (float)VIDEO_FRAME_COUNT;
            float frameHeight = frameWidth * 9 / 16;

            if (_fields.Count > 0)
            {
                for (int frameIndex = 0; frameIndex < VIDEO_FRAME_COUNT; frameIndex++)
                {
                    float x = frameIndex * frameWidth;
                    var field = _fields[frameIndex];
                    var frameRect = new RectangleF(x, yOffset, frameWidth - 1.0f, frameHeight - 1.0f);
                    if (field == null)
                    {
                        using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.Black))
                        {
                            g.FillRectangle(brush, frameRect);
                        }
                    }
                    else if (field.Image == null)
                    {
                        using (var brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard, Color.DarkGray, Color.Black))
                        {
                            g.FillRectangle(brush, frameRect);
                        }
                    }
                    else
                    {
                        g.DrawImage(field.Image, frameRect);
                    }
                }
            }

            yOffset += frameHeight;
        }

        void DrawStatistics(Graphics g, ref float yOffset)
        {
            float height = 6.0f;

            _graphRectangle = new RectangleF(0.0f, yOffset, this.ClientSize.Width, this.ClientSize.Height - yOffset);

            
            using (var font = new Font(this.Font.FontFamily, 8.0f, FontStyle.Regular))
            {
                if (VideoMediaFile != null)
                {
                    var centreField = _fields[VIDEO_FRAME_COUNT / 2];

                    if (_zoomLevel == ZOOM_LEVEL_CLOSE)
                    {
                        int currSecond = -1;
                        for (int fieldIndex = _firstFieldIndex; fieldIndex <= _lastFieldIndex; fieldIndex++)
                        {
                            if ((fieldIndex < 0) || (fieldIndex >= VideoMediaFile.TotalFields))
                                continue;

                            // Draw ticks and time
                            var seconds = ((fieldIndex - _fieldNumber) * VideoMediaFile.FieldDuration) + centreField.Seconds;

                            float x = (fieldIndex - _firstFieldIndex) * _field_dx;

                            var tickHeight = height;
                            bool printTime = false;
                            if (seconds >= (currSecond + 1))
                            {
                                if ((currSecond != -1) || (fieldIndex > _firstFieldIndex))
                                {
                                    tickHeight *= 2;
                                    printTime = true;
                                }
                                currSecond = (int)Math.Floor(seconds);
                            }

                            var brush = Brushes.White;

                            if (tickHeight == height)
                            {
                                if ((fieldIndex % VideoMediaFile.FieldsPerFrame) != 0)
                                {
                                    tickHeight /= 2;
                                    brush = Brushes.DarkGray;
                                }
                            }

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
                    else
                    {
                        var baseIndex = (this.ClientSize.Width / 2);
                        baseIndex -= (baseIndex / 60 + 1) * 60;

                        var baseSeconds = (int)centreField.Seconds + baseIndex - (this.ClientSize.Width / 2);

                        var brush = Brushes.White;

                        for (var index = 0; index < this.ClientSize.Width; index++)
                        {
                            var seconds = baseSeconds + index - baseIndex;

                            if (seconds < 0)
                                continue;

                            if ((seconds % 60) == 0)
                            {
                                var tickHeight = height * 2;
                                var tickRect = new RectangleF(index, yOffset, 1.0f, tickHeight);
                                g.FillRectangle(brush, tickRect);

                                tickRect.Offset(1.0f, height);
                                var secondsTimeSpan = TimeSpan.FromSeconds(seconds);
                                g.DrawString(secondsTimeSpan.ToString(@"h\:mm\:ss"), font, brush, tickRect.Location);
                            }
                            else if ((seconds % 30) == 0)
                            {
                                var tickHeight = height;
                                var tickRect = new RectangleF(index, yOffset, 1.0f, tickHeight);
                                g.FillRectangle(brush, tickRect);
                            }
                            else if ((seconds % 5) == 0)
                            {
                                var tickHeight = height / 2;
                                var tickRect = new RectangleF(index, yOffset, 1.0f, tickHeight);
                                g.FillRectangle(brush, tickRect);
                            }
                        }

                    }
                }

                yOffset += height;
                var textSize = g.MeasureString("0:00:00", font);
                yOffset += textSize.Height;
            }

            // Draw graphs
            var graphInfo = new GraphInfo();

            if (StatisticsProcessor != null)
            {
                graphInfo = new GraphInfo(
                    "Max Brightness",
                    StatisticsProcessor.Data.WholeFileAverages.AverageBrightness,
                    "Brightness_Max",
                    StatisticsProcessor.Data.WholeFileAverages.AverageBrightness * 2,
                    StatisticsProcessor.Data.BrightnessThreshold,
                    Pens.Maroon);
            }
            DrawGraph(g, ref yOffset, graphInfo);

            DrawSplitter(g, ref yOffset);

            if (StatisticsProcessor != null)
            {
                graphInfo = new GraphInfo(
                    "Avg Brightness",
                    StatisticsProcessor.Data.WholeFileAverages.AverageBrightness,
                    "Brightness_Avg",
                    StatisticsProcessor.Data.WholeFileAverages.AverageBrightness * 2,
                    StatisticsProcessor.Data.BrightnessThreshold,
                    Pens.Red);
            }
            DrawGraph(g, ref yOffset, graphInfo);

            DrawSplitter(g, ref yOffset);

            //if (StatisticsProcessor != null)
            //{
            //    graphInfo = new GraphInfo(
            //        "Comskip Brightness",
            //        StatisticsProcessor.Data.WholeFileAverages.AverageBrightness,
            //        "csv_Brightness",
            //        StatisticsProcessor.Data.WholeFileAverages.AverageBrightness * 2,
            //        StatisticsProcessor.Data.BrightnessThreshold,
            //        Pens.Red);
            //}
            //DrawGraph(g, ref yOffset, graphInfo);

            //DrawSplitter(g, ref yOffset);

            //if (StatisticsProcessor != null)
            //{
            //    graphInfo = new GraphInfo(
            //        "Comskip Uniform",
            //        StatisticsProcessor.Data.WholeFileAverages.AverageUniform,
            //        "csv_Uniform",
            //        StatisticsProcessor.Data.WholeFileAverages.AverageUniform * 2,
            //        StatisticsProcessor.Data.UniformThreshold,
            //        Pens.Purple);
            //}
            //DrawGraph(g, ref yOffset, graphInfo);

            //DrawSplitter(g, ref yOffset);

            if (StatisticsProcessor != null)
            {
                graphInfo = new GraphInfo(
                    "Brightness Standard Deviation",
                    0.0,
                    "Brightness_StdDev",
                    byte.MaxValue / 2,
                    0.0,
                    Pens.Purple);
            }
            DrawGraph(g, ref yOffset, graphInfo);


            DrawSplitter(g, ref yOffset);

            if (StatisticsProcessor != null)
            {
                graphInfo = new GraphInfo(
                    "Logo Difference",
                    0.0,
                    "Logo_Difference",
                    0.3f,
                    0.0,
                    Pens.Yellow);
            }
            DrawGraph(g, ref yOffset, graphInfo);

            DrawSplitter(g, ref yOffset);

            if (StatisticsProcessor != null)
            {
                graphInfo = new GraphInfo(
                    "Comskip Sound",
                    StatisticsProcessor.Data.WholeFileAverages.AverageSound,
                    "csv_Sound",
                    StatisticsProcessor.Data.WholeFileAverages.AverageSound * 2,
                    StatisticsProcessor.Data.SoundThreshold,
                    Pens.Green);
            }
            DrawGraph(g, ref yOffset, graphInfo);

            DrawSplitter(g, ref yOffset);
        }

        private class GraphInfo
        {
            public string Title = "";
            public double WholeFileAverage = 0.0;
            public string ColumnId = "";
            public double MaxValue = 255.0;
            public double Threshold = 0.0;
            public Pen GraphPen = null;

            public GraphInfo()
            {
            }
            public GraphInfo(string title, double wholeFileAverage, string columnId, double maxValue, double threshold, Pen graphPen)
            {
                Title = title;
                WholeFileAverage = wholeFileAverage;
                ColumnId = columnId;
                MaxValue = maxValue;
                Threshold = threshold;
                GraphPen = graphPen;
            }
        }

        private void DrawGraph(Graphics g, ref float yOffset, GraphInfo graphInfo)
        {
            DrawGraph(g, ref yOffset, graphInfo.Title, graphInfo.WholeFileAverage, graphInfo.ColumnId, graphInfo.MaxValue, graphInfo.Threshold, graphInfo.GraphPen);
        }
        private void DrawGraph(Graphics g, ref float yOffset, string title, double wholeFileAverage, string columnId, double maxValue, double threshold, Pen graphPen)
        {
            const int graphHeight = 64;

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

            if ((StatisticsProcessor == null) || (VideoMediaFile == null))
            {
                yOffset += graphHeight;
                return;
            }

            if (_zoomLevel == ZOOM_LEVEL_CLOSE)
            {
                for (int fieldIndex = _firstFieldIndex; fieldIndex <= _lastFieldIndex; fieldIndex++)
                {
                    if ((fieldIndex < 0) || (fieldIndex >= StatisticsProcessor.Data.Fields.Count))
                        continue;

                    float x = (fieldIndex - _firstFieldIndex) * _field_dx;

                    var frame = StatisticsProcessor.Data.Fields[fieldIndex];
                    if (frame != null)
                    {
                        var value = frame.GetValue(columnId);
                        if (value.HasValue)
                        {
                            var y = yOffset + (float)((maxValue - value) / (maxValue / (float)graphHeight));
                            if (value <= threshold)
                            {
                                g.DrawLine(Pens.Wheat, x, y, x + _field_dx, y);
                            }
                            else if (y < yOffset)
                            {
                                y = yOffset;
                                g.DrawLine(Pens.Wheat, x, y, x + _field_dx, y);
                            }
                            else
                            {
                                g.DrawLine(graphPen, x, y, x + _field_dx, y);
                            }
                        }
                    }
                }
            }
            else
            {
                var centreField = _fields[VIDEO_FRAME_COUNT / 2];

                var baseIndex = (this.ClientSize.Width / 2);
                baseIndex -= (baseIndex / 60 + 1) * 60;

                var baseSeconds = (int)centreField.Seconds + baseIndex - (this.ClientSize.Width / 2);

                var brush = Brushes.White;

                for (var x = 0; x < this.ClientSize.Width; x++)
                {
                    var seconds = baseSeconds + x - baseIndex;

                    if (seconds < 0)
                        continue;

                    var fieldIndexStart = seconds * VideoMediaFile.FieldsPerSecond;
                    var fieldIndexEnd = fieldIndexStart + VideoMediaFile.FieldsPerSecond;

                    var average = new StatisticsProcessor.AverageDouble();
                    average.RecordMaximum = true;
                    average.RecordMinimum = true;

                    for (int fieldIndex = fieldIndexStart; fieldIndex < fieldIndexEnd; fieldIndex++)
                    {
                        if (fieldIndex >= StatisticsProcessor.Data.Fields.Count)
                            break;
                         
                        var frame = StatisticsProcessor.Data.Fields[fieldIndex];
                        if (frame != null)
                        {
                            var value = frame.GetValue(columnId);
                            average.Add(value);
                        }
                    }

                    if (average.Count > 0)
                    {
                        var yAvg = yOffset + (float)((maxValue - average.Average) / (maxValue / (float)graphHeight));
                        var yMax = yOffset + (float)((maxValue - average.Maximum) / (maxValue / (float)graphHeight));
                        var yMin = yOffset + (float)((maxValue - average.Minimum) / (maxValue / (float)graphHeight));

                        if (yAvg < yOffset)
                            yAvg = yOffset;
                        if (yMax < yOffset)
                            yMax = yOffset;
                        if (yMin < yOffset)
                            yMin = yOffset;

                        var color = (int)(graphPen.Color.GetBrightness() * 192);
                        using (var rangePen = new Pen(Color.FromArgb(color, color, color)))
                        {
                            // Draw Min to Avg
                            var pen = rangePen;
                            if (yMin > yOffset)
                            {
                                if (average.Minimum <= threshold)
                                    pen = Pens.Wheat;
                                g.DrawLine(pen, x, yAvg, x, yMin + 1);
                            }

                            // Draw Avg to Max
                            pen = rangePen;
                            if (yAvg > yOffset)
                            {
                                g.DrawLine(pen, x, yMax, x, yAvg + 1);
                            }

                            // Draw Avg point
                            pen = graphPen;
                            if ((average.Average <= threshold) || (yAvg == yOffset))
                            {
                                pen = Pens.Wheat;
                            }
                            g.DrawLine(pen, x, yAvg, x, yAvg + 1);
                        }

                    }
                }                
            }

            // Draw title
            var textColor = Color.FromArgb(230, 230, 230);
            
            using (var textBrush = new SolidBrush(textColor))
            {
                float x = 5.0f;
                float y = yOffset + 5.0f;
                int decimalPlaces = (int)(4 - Math.Log10(maxValue));
                var doubleFormat = "0";
                if (decimalPlaces > 0)
                    doubleFormat += ".".PadRight(decimalPlaces + 1, '0');

                g.DrawString(title, this.Font, textBrush, x, y);
                y += this.Font.Height;
                g.DrawString("Average: " + wholeFileAverage.ToString(doubleFormat), this.Font, textBrush, x, y);
                y += this.Font.Height;
                g.DrawString("Threshold: " + threshold.ToString(), this.Font, textBrush, x, y);

                // Current value
                x = this.ClientSize.Width / 2.0f + _field_dx + 5.0f;
                y = yOffset + 5.0f + this.Font.Height;
                var frame = StatisticsProcessor.Data.Fields[_fieldNumber];
                if (frame != null)
                {
                    var value = frame.GetValue(columnId);
                    if (value.HasValue)
                    {
                        g.DrawString(value.ToString(), this.Font, textBrush, x, y);
                    }
                }
            }


            yOffset += graphHeight;
        }

        private void TimelineUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (_graphRectangle.Contains(e.X, e.Y))
            {
                var perc = (e.X - _graphRectangle.X) / _graphRectangle.Width;
                var fieldNumber = _fieldCount * perc + _firstFieldIndex;
                if (fieldNumber < 0)
                    fieldNumber = 0;
                if (fieldNumber >= VideoMediaFile.TotalFields)
                    fieldNumber = VideoMediaFile.TotalFields - 1;
                if (SelectedFieldChanged != null)
                    SelectedFieldChanged((int)fieldNumber);
            }
        }

    }
}
