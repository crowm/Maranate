using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ComskipToCuttermaran.Statistics;

namespace ComskipToCuttermaran
{
    public partial class MainForm : SavedLocationForm
    {
        FileDetect.Filenames _filenames;
        int _frameOffset;
        StatisticsProcessor _csvProcessor;
        MediaFile _videoFile;

        double[] _seekTimes = new double[] { 1, 10, 60 };

        int _currentFieldIndex = 0;

        public MainForm(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            _filenames = FileDetect.FillFilenames(inputName, videoFilename, audioFilename);

            _frameOffset = frameOffset;

            this.MouseWheel += new MouseEventHandler(MainForm_MouseWheel);

            InitializeComponent();

            tabControl.Dock = DockStyle.Fill;
            panelGraphs.Dock = DockStyle.Fill;

            objectListViewBlocks.RowFormatter = delegate(BrightIdeasSoftware.OLVListItem item)
            {
                var block = item.RowObject as ComskipToCuttermaran.Statistics.Block;
                if (block == null)
                    return;

                item.UseItemStyleForSubItems = true;

                var isCommercial = (block.IsCommercialOverride.HasValue ? block.IsCommercialOverride.Value : block.IsCommercial);
                if (block.IsCommercialOverride.HasValue)
                {
                    item.BackColor = Color.FromArgb(0xFF, 0xFE, 0xD6);
                }
                if (block.IsCommercial)
                {
                    item.ForeColor = Color.Maroon;
                }
                else
                {
                    item.ForeColor = Color.Navy;
                }

                //if (merged)
                //    item.ForeColor = Color.LightGray;
                //else if (checkBoxShowExcluded.Checked && !entry.Export)
                //    item.ForeColor = Color.LightGray;
                //else if (entry.SqlScriptEntry.ObjectType == SqlScriptEntry.ObjectTypes.Unknown)
                //    item.ForeColor = Color.White;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Drop)
                //    item.ForeColor = Color.Maroon;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Delete)
                //    item.ForeColor = Color.Maroon;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Create)
                //    item.ForeColor = Color.Navy;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Alter)
                //    item.ForeColor = Color.Indigo;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Recreate)
                //    item.ForeColor = Color.MidnightBlue;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Insert)
                //    item.ForeColor = Color.Purple;
                //else if (entry.SqlScriptEntry.CommandType == SqlScriptEntry.CommandTypes.Update)
                //    item.ForeColor = Color.SeaGreen;

            };

            //button3.Visible = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            splitContainer3.Enabled = false;

            var controls = (from Control c in tabPageAnalysis.Controls select c).ToList();
            for (int i = 0; i < controls.Count; i++)
            {
                var control = controls[i];
                var subControls = (from Control c in control.Controls select c).ToList();
                controls.AddRange(subControls);

                control.PreviewKeyDown += new PreviewKeyDownEventHandler(control_PreviewKeyDown);
            }

            base.AddSavedSplitContainer(splitContainer3);
            base.AddSavedSplitContainer(splitContainer4);
            base.AddSavedObjectListViewColumns(objectListViewBlocks);

            LoadSettings();

            timerLoading.Tick += new EventHandler(timerLoading_Tick);
            timerLoadingCount = 0;
            timerLoading.Start();
            labelLoading.Visible = true;

            RunOnBackgroundThread("Loading files", LoadFiles);
        }

        void LoadSettings()
        {
            var settings = Settings.Current.DetectionSettings;

            settingsThresholdUserControlBrightness.Threshold = settings.BrightnessThreshold;
            settingsThresholdUserControlBrightness.SafetyPercent = settings.DetectBrightnessSafetyBufferPercent;
            settingsThresholdUserControlBrightness.SafetyOffset = settings.DetectBrightnessSafetyBufferOffset;

            settingsThresholdUserControlUniform.Threshold = settings.UniformThreshold;
            settingsThresholdUserControlUniform.SafetyPercent = settings.DetectUniformSafetyBufferPercent;
            settingsThresholdUserControlUniform.SafetyOffset = settings.DetectUniformSafetyBufferOffset;

            settingsThresholdUserControlSound.Threshold = settings.SoundThreshold;
            settingsThresholdUserControlSound.SafetyPercent = settings.DetectSoundSafetyBufferPercent;
            settingsThresholdUserControlSound.SafetyOffset = settings.DetectSoundSafetyBufferOffset;

            if (_csvProcessor != null)
            {
                settingsThresholdUserControlBrightness.DetectedThresholdPreSafety = _csvProcessor.Data.BrightnessThresholdPreSafety;
                settingsThresholdUserControlUniform.DetectedThresholdPreSafety = _csvProcessor.Data.UniformThresholdPreSafety;
                settingsThresholdUserControlSound.DetectedThresholdPreSafety = _csvProcessor.Data.SoundThresholdPreSafety;
            }
        }

        void SaveSettings()
        {
            var settings = Settings.Current.DetectionSettings;

            settings.BrightnessThreshold = settingsThresholdUserControlBrightness.Threshold;
            settings.DetectBrightnessSafetyBufferPercent = settingsThresholdUserControlBrightness.SafetyPercent;
            settings.DetectBrightnessSafetyBufferOffset = settingsThresholdUserControlBrightness.SafetyOffset;

            settings.UniformThreshold = settingsThresholdUserControlUniform.Threshold;
            settings.DetectUniformSafetyBufferPercent = settingsThresholdUserControlUniform.SafetyPercent;
            settings.DetectUniformSafetyBufferOffset = settingsThresholdUserControlUniform.SafetyOffset;

            settings.SoundThreshold = settingsThresholdUserControlSound.Threshold;
            settings.DetectSoundSafetyBufferPercent = settingsThresholdUserControlSound.SafetyPercent;
            settings.DetectSoundSafetyBufferOffset = settingsThresholdUserControlSound.SafetyOffset;
        }

        void control_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.IsInputKey = true;
            }
        }

        int timerLoadingCount = 0;
        void timerLoading_Tick(object sender, EventArgs e)
        {
            timerLoadingCount++;
            var periods = "".PadRight((timerLoadingCount % 10), '.');
            labelLoading.Text = "Loading." + periods;
        }

        private void CloseFiles()
        {
            this.Invoke(new Action(() =>
                {
                    SetPreviewField(null);
                    objectListViewBlocks.SetObjects(null);
                    UpdateButtons();
                    timelineUserControl1.CloseFiles();
                    graphsUserControl1.CloseFiles();

                    _csvProcessor = null;

                    if (_videoFile != null)
                    {
                        _videoFile.Dispose();
                        _videoFile = null;
                    }
                }));
        }

        private void LoadFiles()
        {
            this.Invoke(new Action(() =>
                {
                    splitContainer3.Enabled = false;
                    CloseFiles();
                    timerLoading.Start();
                    labelLoading.Visible = true;
                }));

            _csvProcessor = new StatisticsProcessor(_filenames, _frameOffset);
            _csvProcessor.Settings = Settings.Current.DetectionSettings;
            _csvProcessor.Log += new Action<string>(_csvProcessor_Log);
            _csvProcessor.Process();

            LoadSettings();

            _videoFile = new MediaFile();
            _videoFile.Resolution = MediaFile.ResolutionOption.Full;
            _videoFile.OutputYData = true;
            _videoFile.OutputRGBImage = true;
            //_videoFile.OutputYImage = true;
            _videoFile.Open(_filenames.videoFilename);

            var frameField = _videoFile.GetVideoFrameField(_currentFieldIndex);
            //if (frameField.Image != null)
            //    frameField.Image.Save(@"D:\temp\image-" + frameField.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            this.Invoke(new Action(() =>
                {
                    timerLoading.Stop();
                    labelLoading.Visible = false;

                    objectListViewBlocks.SetObjects(_csvProcessor.Data.Blocks);
                    UpdateButtons();

                    timelineUserControl1.StatisticsProcessor = _csvProcessor;
                    timelineUserControl1.TotalFields = _videoFile.TotalFields;
                    timelineUserControl1.FieldsPerSecond = _videoFile.FieldsPerSecond;

                    var graphsVideoFile = _videoFile.Clone(MediaFile.ResolutionOption.Full);
                    graphsVideoFile.OutputYImage = false;
                    graphsVideoFile.OutputRGBImage = true;
                    graphsUserControl1.StatisticsProcessor = _csvProcessor;
                    graphsUserControl1.VideoMediaFile = graphsVideoFile;
                    graphsUserControl1.SetDirty();

                    timelineUserControl1.SecondsVisible = graphsUserControl1.SecondsVisible;

                    SetPreviewField(frameField);

                    splitContainer3.Enabled = true;
                }));
        }

        void _csvProcessor_Log(string obj)
        {
            this.Invoke(new Action(() =>
                {
                    richTextBoxMessages.AppendText(obj + Environment.NewLine);
                }));
        }

        private void SetPreviewField(MediaFile.FrameField field)
        {
            if (field != null)
            {
                if (edgeMapToolStripMenuItem.Checked)
                {
                    var yFloatData = field.YData.GetFloatData();
                    var edgeData = ImageProcessing.ImageProcessor.GenerateEdgeDetectedImage(yFloatData);
                    var edgeImage = edgeData.ToBitmap();

                    pictureBoxPreview.Image = edgeImage;
                }
                else if (edgeMapStdDev5SecsToolStripMenuItem.Checked)
                {
                }
                else
                {
                    pictureBoxPreview.Image = field.Image;
                }
                //if (field.Image != null)
                //    field.Image.Save(@"D:\temp\image-" + field.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                var seconds = TimeSpan.FromSeconds(field.Seconds);
                string frameType = "";

                if (field.AVFrame.pict_type != SharpFFmpeg.FFmpeg.AVPictureType.AV_PICTURE_TYPE_NONE)
                {
                    var pictType = field.AVFrame.pict_type.ToString();
                    frameType = "[" + pictType.Substring(pictType.LastIndexOf('_') + 1) + "] ";
                }

                labelCurrentPosition.Text = "Current position: " + seconds.ToString(@"h\:mm\:ss\.fff") + "  " + frameType + field.FieldNumber.ToString();
                timelineUserControl1.FieldNumber = field.FieldNumber;
                graphsUserControl1.FieldNumber = field.FieldNumber;

                _currentFieldIndex = field.FieldNumber;
            }
            else
            {
                pictureBoxPreview.Image = null;
                labelCurrentPosition.Text = "";
            }

            UpdateButtons();
        }

        private bool _isPlaying = false;
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;

            if (_isPlaying)
            {
                buttonPlay.Text = "Stop";
                RunOnBackgroundThread(() =>
                {
                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var firstField = _currentFieldIndex;

                    for (int i = firstField; i < _videoFile.TotalFields; i++)
                    {
                        if (!_isPlaying)
                            break;

                        var field = _videoFile.GetVideoFrameField(i);
                        this.Invoke(new Action(() =>
                        {
                            SetPreviewField(field);
                        }));

                        var ptsMilliseconds = (_currentFieldIndex - firstField) * _videoFile.FieldDuration * 1000.0;
                        var timeToWait = (int)(ptsMilliseconds - stopwatch.ElapsedMilliseconds);
                        if (timeToWait > 0)
                            System.Threading.Thread.Sleep(timeToWait);
                    }
                });
            }
            else
            {
                buttonPlay.Text = "Play";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object updateFieldLock = new object();
            MediaFile.FrameField updateField = null;
            long updateFieldElapsed = 0L;
            int updateFieldCount = 0;

            var updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 20;
            updateTimer.Tick += new EventHandler((sender2, e2) =>
            {
                MediaFile.FrameField field;
                double elapsed;
                int fieldNumber;
                lock (updateFieldLock)
                {
                    field = updateField;
                    elapsed = updateFieldElapsed;
                    fieldNumber = updateFieldCount;
                }
                if (field != null)
                {
                    //SetPreviewFrame(frame);
                    pictureBoxPreview.Image = field.Image;

                    if (elapsed > 0.0)
                    {
                        var fps = 1000.0 * fieldNumber / elapsed;
                        labelCurrentPosition.Text = fps.ToString("0.00");
                    }
                }
            });
            updateTimer.Start();

            RunOnBackgroundThread(() =>
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                for (int i = 0; (i < _videoFile.TotalFields); i += 1)
                {
                    var field = _videoFile.GetVideoFrameField(i);

                    lock (updateFieldLock)
                    {
                        updateField = field;
                        updateFieldElapsed = stopwatch.ElapsedMilliseconds;
                        updateFieldCount = i + 1;
                    }

                    if (updateFieldElapsed > 5000)
                        break;
                }

                this.Invoke(new Action(() =>
                {
                    updateTimer.Stop();
                    updateTimer.Dispose();
                }));
            });
        }

        private void buttonSeekToStart_Click(object sender, EventArgs e)
        {
            _currentFieldIndex = 0;
            var field = _videoFile.GetVideoFrameField(_currentFieldIndex);
            SetPreviewField(field);
        }

        private void buttonSeekToEnd_Click(object sender, EventArgs e)
        {
            _currentFieldIndex = _videoFile.TotalFields - 1;
            var field = _videoFile.GetVideoFrameField(_currentFieldIndex);
            SetPreviewField(field);
        }

        private void buttonSeekBackLarge_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(-1 * _seekTimes[2]);
        }

        private void buttonSeekForwardLarge_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(_seekTimes[2]);
        }

        private void buttonSeekBackMedium_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(-1 * _seekTimes[1]);
        }

        private void buttonSeekForwardMedium_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(_seekTimes[1]);
        }

        private void buttonSeekBackSmall_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(-1 * _seekTimes[0]);
        }

        private void buttonSeekForwardSmall_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(_seekTimes[0]);
        }

        private void buttonSeekToPreviousFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFieldIndex--;
                var field = _videoFile.GetVideoFrameField(_currentFieldIndex);
                SetPreviewField(field);
            }
        }

        private void buttonSeekToNextFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFieldIndex++;
                var field = _videoFile.GetVideoFrameField(_currentFieldIndex);
                SetPreviewField(field);
            }
        }

        private void SeekToField(int fieldNumber)
        {
            if (_videoFile != null)
            {
                var field = _videoFile.GetVideoFrameField(fieldNumber);
                SetPreviewField(field);
            }
        }

        private void SeekOffsetSeconds(double seconds)
        {
            if (_videoFile != null)
            {
                int fieldIndex = (int)(seconds / _videoFile.FieldDuration);
                fieldIndex += _currentFieldIndex;
                if (fieldIndex < 0)
                    fieldIndex = 0;
                if (fieldIndex >= _videoFile.TotalFields)
                    fieldIndex = _videoFile.TotalFields - 1;
                var field = _videoFile.GetVideoFrameField(fieldIndex);
                SetPreviewField(field);
            }
        }

        private void PerformSeek(Keys key)
        {
            if (key == Keys.Right)
            {
                if (_currentKeyModifiers == (Keys.Control | Keys.Shift))
                    buttonSeekForwardLarge_Click(this, new EventArgs());
                else if (_currentKeyModifiers == Keys.Control)
                    buttonSeekForwardMedium_Click(this, new EventArgs());
                else if (_currentKeyModifiers == Keys.Shift)
                    buttonSeekForwardSmall_Click(this, new EventArgs());
                else
                    buttonSeekToNextFrame_Click(this, new EventArgs());
            }
            if (key == Keys.Left)
            {
                if (_currentKeyModifiers == (Keys.Control | Keys.Shift))
                    buttonSeekBackLarge_Click(this, new EventArgs());
                else if (_currentKeyModifiers == Keys.Control)
                    buttonSeekBackMedium_Click(this, new EventArgs());
                else if (_currentKeyModifiers == Keys.Shift)
                    buttonSeekBackSmall_Click(this, new EventArgs());
                else
                    buttonSeekToPreviousFrame_Click(this, new EventArgs());
            }
        }

        private Keys _currentKeyModifiers;
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            _currentKeyModifiers = e.Modifiers;
            //Console.WriteLine("KeyDown        : " + e.KeyCode.ToString() + "    " + e.Modifiers.ToString());
            if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right))
            {
                PerformSeek(e.KeyCode);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                var index = objectListViewBlocks.SelectedIndex;
                if (index > 0)
                    index--;
                objectListViewBlocks.SelectedIndex = index;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                var index = objectListViewBlocks.SelectedIndex;
                if (index < objectListViewBlocks.Items.Count - 1)
                    index++;
                objectListViewBlocks.SelectedIndex = index;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Add)
            {
                scrollBarZoom.Value = Math.Min(scrollBarZoom.Value + 1, scrollBarZoom.Maximum);
            }
            if (e.KeyCode == Keys.Subtract)
            {
                scrollBarZoom.Value = Math.Max(scrollBarZoom.Value - 1, scrollBarZoom.Minimum);
            }
            if (e.KeyCode == Keys.Insert)
            {
                InsertCutPoint();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Space)
            {
                SetIsCommercial(null);
                e.Handled = true;
            }
        }

        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            _currentKeyModifiers = e.Modifiers;
        }

        void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                PerformSeek(((e.Delta < 0) ? Keys.Right : Keys.Left));
            }
        }


        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private ComskipToCuttermaran.Statistics.Block _objectListViewBlocksExpectedObject = null;
        private void objectListViewBlocks_SelectionChanged(object sender, EventArgs e)
        {
            var block = objectListViewBlocks.SelectedObject as ComskipToCuttermaran.Statistics.Block;
            if (_objectListViewBlocksExpectedObject != block)
            {
                _objectListViewBlocksExpectedObject = block;

                if (block != null)
                {
                    SeekToField(block.StartFieldNumber);
                }
            }
            UpdateButtons();
        }

        private void timelineUserControl1_SelectedFieldChanged(int fieldNumber)
        {
            if (scrollBarZoom.Value < 2)
                fieldNumber -= (fieldNumber % _videoFile.FieldsPerSecond);

            SeekToField(fieldNumber);
            foreach (var block in _csvProcessor.Data.Blocks)
            {
                if ((fieldNumber >= block.StartField.FieldNumber) && (fieldNumber <= block.EndField.FieldNumber))
                {
                    _objectListViewBlocksExpectedObject = block;
                    objectListViewBlocks.SelectedObject = block;
                }
            }
        }

        private void buttonReprocess_Click(object sender, EventArgs e)
        {
            Reprocess(false);
        }
        private void Reprocess(bool keepManualCommercialOverrides)
        {
            richTextBoxMessages.Clear();
            SaveSettings();
            _csvProcessor.Settings = Settings.Current.DetectionSettings;
            _csvProcessor.Reprocess(keepManualCommercialOverrides);
            LoadSettings();
            objectListViewBlocks.SetObjects(_csvProcessor.Data.Blocks);
            UpdateButtons();
            timelineUserControl1.SetDirty();
            graphsUserControl1.SetDirty();
        }

        private void buttonInsertCutPoint_Click(object sender, EventArgs e)
        {
            InsertCutPoint();
        }

        private void InsertCutPoint()
        {
            _csvProcessor.Data.ManualCutFields.Add(_currentFieldIndex);
            Reprocess(true);
        }

        private void buttonToggleIsCommercial_Click(object sender, EventArgs e)
        {
            SetIsCommercial(null);
        }

        private void SetIsCommercial(bool? isCommercial)
        {
            var block = objectListViewBlocks.SelectedObject as ComskipToCuttermaran.Statistics.Block;
            if (block != null)
            {
                if (isCommercial == null)
                {
                    isCommercial = !(block.IsCommercialOverride.HasValue ? block.IsCommercialOverride.Value : block.IsCommercial);
                }
                if (isCommercial == block.IsCommercial)
                    block.IsCommercialOverride = null;
                else
                    block.IsCommercialOverride = !block.IsCommercial;
                timelineUserControl1.SetDirty();
                graphsUserControl1.SetDirty();
                objectListViewBlocks.RefreshObject(block);
            }
        }

        private void UpdateButtons()
        {
            var block = objectListViewBlocks.SelectedObject as ComskipToCuttermaran.Statistics.Block;
            if (block != null)
            {
                buttonInsertCutPoint.Enabled = (_currentFieldIndex != block.StartFieldNumber);
                buttonToggleIsCommercial.Enabled = true;
            }
            else
            {
                buttonInsertCutPoint.Enabled = false;
                buttonToggleIsCommercial.Enabled = false;
            }

            var enableBack = (_videoFile != null) && (_currentFieldIndex != 0);
            buttonSeekToStart.Enabled = enableBack;
            buttonSeekBackLarge.Enabled = enableBack;
            buttonSeekBackMedium.Enabled = enableBack;
            buttonSeekBackSmall.Enabled = enableBack;
            buttonSeekToPreviousFrame.Enabled = enableBack;

            var enableFwd = (_videoFile != null) && (_currentFieldIndex < (_videoFile.TotalFields - 1));
            buttonSeekToEnd.Enabled = enableFwd;
            buttonSeekForwardLarge.Enabled = enableFwd;
            buttonSeekForwardMedium.Enabled = enableFwd;
            buttonSeekForwardSmall.Enabled = enableFwd;
            buttonSeekToNextFrame.Enabled = enableFwd;

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _csvProcessor.WriteCuttermaranFile();
            
            //TODO: Execute Cuttermaran

            //this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (_filenames.inputFilename != null)
                dialog.FileName = _filenames.inputFilename;
            dialog.Filter = "Csv Files (*.csv)|*.csv|All Files (*.*)|*.*";
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
                Open(dialog.FileName);
        }

        private void Open(string filename, bool loadFiles = true)
        {
            _filenames = FileDetect.FillFilenames(filename);

            if (loadFiles)
                RunOnBackgroundThread("Loading files", LoadFiles);
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBoxPreview.Image.Save(@"D:\temp\image.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void imageTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalImageToolStripMenuItem.Checked = false;
            edgeMapToolStripMenuItem.Checked = false;
            edgeMapStdDev5SecsToolStripMenuItem.Checked = false;
            var menuItem = sender as ToolStripMenuItem;
            menuItem.Checked = true;

            var field = _videoFile.GetVideoFrameField(_currentFieldIndex);
            SetPreviewField(field);
        }

        private void scrollBarZoom_ValueChanged(object sender, EventArgs e)
        {
            graphsUserControl1.ZoomLevel = scrollBarZoom.Value;
            timelineUserControl1.SecondsVisible = graphsUserControl1.SecondsVisible;

            if (scrollBarZoom.Value == 2)
            {
                _seekTimes = new double[] { 1, 10, 60 };
                buttonSeekBackLarge.ButtonText = "-60s";
                buttonSeekForwardLarge.ButtonText = "+60s";
                buttonSeekBackMedium.ButtonText = "-10s";
                buttonSeekForwardMedium.ButtonText = "+10s";
                buttonSeekBackSmall.ButtonText = "-1s";
                buttonSeekForwardSmall.ButtonText = "+1s";
            }
            else
            {
                _seekTimes = new double[] { 1, 60, 600 };
                buttonSeekBackLarge.ButtonText = "-10m";
                buttonSeekForwardLarge.ButtonText = "+10m";
                buttonSeekBackMedium.ButtonText = "-1m";
                buttonSeekForwardMedium.ButtonText = "+1m";
                buttonSeekBackSmall.ButtonText = "-1s";
                buttonSeekForwardSmall.ButtonText = "+1s";
            }
        }

    }
}
