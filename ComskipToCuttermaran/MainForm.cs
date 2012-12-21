using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ComskipToCuttermaran
{
    public partial class MainForm : SavedLocationForm
    {
        string _inputName;
        string _videoFilename;
        string _audioFilename;
        int _frameOffset;
        ComskipCsvProcessor _csvProcessor;
        MediaFile _videoFile;

        int _currentFrameIndex = 0;

        public MainForm(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            _inputName = inputName;
            _videoFilename = videoFilename;
            _audioFilename = audioFilename;
            _frameOffset = frameOffset;

            this.MouseWheel += new MouseEventHandler(MainForm_MouseWheel);

            InitializeComponent();

            tabControl.Dock = DockStyle.Fill;

            objectListViewBlocks.RowFormatter = delegate(BrightIdeasSoftware.OLVListItem item)
            {
                var block = item.RowObject as ComskipCsvProcessor.Block;
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
                    SetPreviewFrame(null);
                    objectListViewBlocks.SetObjects(null);
                    UpdateButtons();
                    timelineUserControl1.CloseFiles();

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

            _csvProcessor = new ComskipCsvProcessor(_inputName, _videoFilename, _audioFilename, _frameOffset);
            _csvProcessor.Settings = Settings.Current.DetectionSettings;
            _csvProcessor.Log += new Action<string>(_csvProcessor_Log);
            _csvProcessor.Process();

            LoadSettings();

            _videoFile = new MediaFile();
            _videoFile.Resolution = MediaFile.ResolutionOption.Full;
            _videoFile.OutputRGBImage = true;
            _videoFile.Open(_videoFilename);

            var frame = _videoFile.GetVideoFrame(_currentFrameIndex);
            //if (frame.Image != null)
            //    frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

            this.Invoke(new Action(() =>
                {
                    timerLoading.Stop();
                    labelLoading.Visible = false;

                    objectListViewBlocks.SetObjects(_csvProcessor.Data.Blocks);
                    UpdateButtons();

                    timelineUserControl1.CsvProcessor = _csvProcessor;
                    timelineUserControl1.VideoMediaFile = _videoFile.Clone(MediaFile.ResolutionOption.Quarter);
                    timelineUserControl1.SetDirty();

                    SetPreviewFrame(frame);

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

        private void SetPreviewFrame(MediaFile.Frame frame)
        {
            if (frame != null)
            {
                pictureBoxPreview.Image = frame.Image;
                //if (frame.Image != null)
                //    frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                var seconds = TimeSpan.FromSeconds(frame.Seconds);
                string frameType = "";

                if (frame.AVFrame.pict_type != SharpFFmpeg.FFmpeg.AVPictureType.AV_PICTURE_TYPE_NONE)
                {
                    var pictType = frame.AVFrame.pict_type.ToString();
                    frameType = "[" + pictType.Substring(pictType.LastIndexOf('_') + 1) + "] ";
                }

                labelPreviewFrame.Text = "Current position: " + seconds.ToString(@"h\:mm\:ss\.fff") + "  " + frameType + frame.FrameNumber.ToString();
                timelineUserControl1.FrameNumber = frame.FrameNumber;

                _currentFrameIndex = frame.FrameNumber;
            }
            else
            {
                pictureBoxPreview.Image = null;
                labelPreviewFrame.Text = "";
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
                    var firstFrame = _currentFrameIndex;

                    for (int i = firstFrame; i < _videoFile.TotalFrames; i++)
                    {
                        if (!_isPlaying)
                            break;

                        var frame = _videoFile.GetVideoFrame(i);
                        this.Invoke(new Action(() =>
                        {
                            SetPreviewFrame(frame);
                        }));

                        var ptsMilliseconds = (_currentFrameIndex - firstFrame) * _videoFile.FrameDuration * 1000.0;
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
            RunOnBackgroundThread(() =>
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                for (int i = _videoFile.TotalFrames - 5; i < _videoFile.TotalFrames; i += 1)
                {
                    var frames = _videoFile.GetVideoFrames(i, i);
                    _currentFrameIndex = frames[0].FrameNumber;
                    if (_currentFrameIndex == _videoFile.TotalFrames - 1)
                    {
                        this.Invoke(new Action(() =>
                        {
                            SetPreviewFrame(frames[0]);

                            labelPreviewFrame.Text = stopwatch.Elapsed.ToString();
                        }));
                    }
                    else
                    {
                        frames[0].Dispose();
                    }
                    //System.Threading.Thread.Sleep(10);
                }
            });
        }

        private void buttonSeekToStart_Click(object sender, EventArgs e)
        {
            _currentFrameIndex = 0;
            var frame = _videoFile.GetVideoFrame(_currentFrameIndex);
            SetPreviewFrame(frame);
        }

        private void buttonSeekToEnd_Click(object sender, EventArgs e)
        {
            _currentFrameIndex = _videoFile.TotalFrames - 1;
            var frame = _videoFile.GetVideoFrame(_currentFrameIndex);
            SetPreviewFrame(frame);
        }

        private void buttonSeekBack60s_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(-60.0);
        }

        private void buttonSeekForward60s_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(+60.0);
        }

        private void buttonSeekBack10s_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(-10.0);
        }

        private void buttonSeekForward10s_Click(object sender, EventArgs e)
        {
            SeekOffsetSeconds(+10.0);
        }

        private void buttonSeekToPreviousKeyFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFrameIndex--;
                var frame = _videoFile.GetVideoFrame(_currentFrameIndex, MediaFile.SeekModes.PreviousKeyFrame);
                SetPreviewFrame(frame);
            }
        }

        private void buttonSeekToNextKeyFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFrameIndex++;
                var frame = _videoFile.GetVideoFrame(_currentFrameIndex, MediaFile.SeekModes.NextKeyFrame);
                SetPreviewFrame(frame);
            }
        }

        private void buttonSeekToPreviousFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFrameIndex--;
                var frame = _videoFile.GetVideoFrame(_currentFrameIndex);
                SetPreviewFrame(frame);
            }
        }

        private void buttonSeekToNextFrame_Click(object sender, EventArgs e)
        {
            if (_videoFile != null)
            {
                _currentFrameIndex++;
                var frame = _videoFile.GetVideoFrame(_currentFrameIndex);
                SetPreviewFrame(frame);
            }
        }

        private void SeekToFrame(int frameNumber)
        {
            if (_videoFile != null)
            {
                var frame = _videoFile.GetVideoFrame(frameNumber);
                SetPreviewFrame(frame);
            }
        }

        private void SeekOffsetSeconds(double seconds)
        {
            if (_videoFile != null)
            {
                int frameIndex = (int)(seconds / _videoFile.FrameDuration);
                frameIndex += _currentFrameIndex;
                if (frameIndex < 0)
                    frameIndex = 0;
                if (frameIndex >= _videoFile.TotalFrames)
                    frameIndex = _videoFile.TotalFrames - 1;
                var frame = _videoFile.GetVideoFrame(frameIndex);
                SetPreviewFrame(frame);
            }
        }

        private void PerformSeek(Keys key)
        {
            if (key == Keys.Right)
            {
                if (_currentKeyModifiers == (Keys.Control | Keys.Shift))
                    SeekOffsetSeconds(+60.0);
                else if (_currentKeyModifiers == Keys.Control)
                    SeekOffsetSeconds(+10.0);
                else if (_currentKeyModifiers == Keys.Shift)
                    buttonSeekToNextKeyFrame_Click(this, new EventArgs());
                else
                    buttonSeekToNextFrame_Click(this, new EventArgs());
            }
            if (key == Keys.Left)
            {
                if (_currentKeyModifiers == (Keys.Control | Keys.Shift))
                    SeekOffsetSeconds(-60.0);
                else if (_currentKeyModifiers == Keys.Control)
                    SeekOffsetSeconds(-10.0);
                else if (_currentKeyModifiers == Keys.Shift)
                    buttonSeekToPreviousKeyFrame_Click(this, new EventArgs());
                else
                    buttonSeekToPreviousFrame_Click(this, new EventArgs());
            }
        }

        private Keys _currentKeyModifiers;
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            _currentKeyModifiers = e.Modifiers;
            Console.WriteLine("KeyDown        : " + e.KeyCode.ToString() + "    " + e.Modifiers.ToString());
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

        private ComskipCsvProcessor.Block _objectListViewBlocksExpectedObject = null;
        private void objectListViewBlocks_SelectionChanged(object sender, EventArgs e)
        {
            var block = objectListViewBlocks.SelectedObject as ComskipCsvProcessor.Block;
            if (_objectListViewBlocksExpectedObject != block)
            {
                _objectListViewBlocksExpectedObject = block;

                if (block != null)
                {
                    SeekToFrame(block.StartFrameNumber);
                }
            }
            UpdateButtons();
        }

        private void timelineUserControl1_SelectedFrameChanged(int frameNumber)
        {
            SeekToFrame(frameNumber);
            foreach (var block in _csvProcessor.Data.Blocks)
            {
                if ((frameNumber >= block.StartFrame.FrameNumber) && (frameNumber <= block.EndFrame.FrameNumber))
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
        }

        private void buttonInsertCutPoint_Click(object sender, EventArgs e)
        {
            InsertCutPoint();
        }

        private void InsertCutPoint()
        {
            _csvProcessor.Data.ManualCutFrames.Add(_currentFrameIndex);
            Reprocess(true);
        }

        private void buttonToggleIsCommercial_Click(object sender, EventArgs e)
        {
            SetIsCommercial(null);
        }

        private void SetIsCommercial(bool? isCommercial)
        {
            var block = objectListViewBlocks.SelectedObject as ComskipCsvProcessor.Block;
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
                objectListViewBlocks.RefreshObject(block);
            }
        }

        private void UpdateButtons()
        {
            var block = objectListViewBlocks.SelectedObject as ComskipCsvProcessor.Block;
            if (block != null)
            {
                buttonInsertCutPoint.Enabled = (_currentFrameIndex != block.StartFrameNumber);
                buttonToggleIsCommercial.Enabled = true;
            }
            else
            {
                buttonInsertCutPoint.Enabled = false;
                buttonToggleIsCommercial.Enabled = false;
            }

            var enableBack = (_videoFile != null) && (_currentFrameIndex != 0);
            buttonSeekToStart.Enabled = enableBack;
            buttonSeekBack60s.Enabled = enableBack;
            buttonSeekBack10s.Enabled = enableBack;
            buttonSeekToPreviousKeyFrame.Enabled = enableBack;
            buttonSeekToPreviousFrame.Enabled = enableBack;

            var enableFwd = (_videoFile != null) && (_currentFrameIndex < (_videoFile.TotalFrames - 1));
            buttonSeekToEnd.Enabled = enableFwd;
            buttonSeekForward60s.Enabled = enableFwd;
            buttonSeekForward10s.Enabled = enableFwd;
            buttonSeekToNextKeyFrame.Enabled = enableFwd;
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
            if (_inputName != null)
                dialog.FileName = _inputName;
            dialog.Filter = "Csv Files (*.csv)|*.csv|All Files (*.*)|*.*";
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
                Open(dialog.FileName, null, null, 0);
        }

        private void Open(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            if (videoFilename == null)
            {
                videoFilename = System.IO.Path.GetDirectoryName(inputName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(inputName) + ".m2v";
            }
            if (audioFilename == null)
            {
                audioFilename = System.IO.Path.GetDirectoryName(videoFilename) + "\\" + System.IO.Path.GetFileNameWithoutExtension(inputName) + ".mp2";
                if (!System.IO.File.Exists(audioFilename))
                {
                    audioFilename = System.IO.Path.GetDirectoryName(videoFilename) + "\\" + System.IO.Path.GetFileNameWithoutExtension(inputName) + ".ac3";
                }
            }

            _inputName = inputName;
            _videoFilename = videoFilename;
            _audioFilename = audioFilename;
            _frameOffset = frameOffset;

            RunOnBackgroundThread("Loading files", LoadFiles);
        }

    }
}
