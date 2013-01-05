using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ComskipToCuttermaran.Statistics
{
    public class StatisticsProcessor : Statistics.IStatisticsProcessor
    {
        private FileDetect.Filenames _filenames;
        private int _frameOffset;

        private string _cuttermaranFilename;

        private int _frameRate;
        private Dictionary<CsvColumnId, int> _columnIds = new Dictionary<CsvColumnId, int>();

        #region "Settings"
        public Settings_info Settings = new Settings_info();

        public string CuttermaranFilename
        {
            get { return _cuttermaranFilename; }
            set { _cuttermaranFilename = value; }
        }
        #endregion

        #region "Data"
        public Data_info _data = new Data_info();
        public Data_info Data
        {
            get { return _data; }
        }
        #endregion

        public enum CsvColumnId
        {
            Frame = 0,
            Brightness = 1,
            Scene_Change = 2,
            Logo = 3,
            Uniform = 4,
            Sound = 5,
            MinY = 6,
            MaxY = 7,
            AR_Ratio = 8,
            GoodEdge = 9,
            IsBlack = 10,
            Cutscene = 11,
            MinX = 12,
            MaxX = 13,
            HasBright = 14,
            DimCount = 15,
            PTS = 16
        }

        public event Action<string> Log;

        public StatisticsProcessor(FileDetect.Filenames filenames, int frameOffset)
        {
            _filenames = filenames;
            _frameOffset = frameOffset;

            _cuttermaranFilename = Path.GetDirectoryName(_filenames.inputFilename) + "\\" + Path.GetFileNameWithoutExtension(_filenames.inputFilename) + ".cpf";
        }

        public int Process()
        {
            int result = 0;
            Log("Processing Comskip csv file.");

            result = Load();
            if (result < 0)
                return result;

            // Detect thresholds
            DetectThresholds();

            result = FindBlackFields();

            return result;
        }

        public int Reprocess(bool keepManualCommercialOverrides)
        {
            int result = 0;
            Log("Reprocessing.");

            var oldData = Data;
            var newData = new Data_info();
            newData.Fields = oldData.Fields;
            newData.ManualCutFields = oldData.ManualCutFields;
            _data = newData;

            DetectThresholds();

            result = FindBlackFields();

            if (keepManualCommercialOverrides)
            {
                // Copy manual commercial overrides from old blocks to new blocks.
                if ((oldData.Blocks.Count > 0) && (newData.Blocks.Count > 0))
                {
                    int oldIndex = 0;
                    int newIndex = 0;
                    Block oldBlock = null;
                    Block newBlock = null;
                    while ((oldIndex < oldData.Blocks.Count) && (newIndex < newData.Blocks.Count))
                    {
                        var oldBlockNext = oldData.Blocks[oldIndex];
                        var newBlockNext = newData.Blocks[newIndex];

                        if (oldBlockNext.StartFieldNumber < newBlockNext.StartFieldNumber)
                        {
                            oldIndex++;
                        }
                        else if (oldBlockNext.StartFieldNumber > newBlockNext.StartFieldNumber)
                        {
                            newIndex++;
                        }
                        else
                        {
                            if ((oldBlock != null) && (newBlock != null))
                            {
                                newBlock.IsCommercialOverride = oldBlock.IsCommercialOverride;
                            }

                            oldBlock = oldBlockNext;
                            newBlock = newBlockNext;
                            oldIndex++;
                            newIndex++;
                            continue;
                        }
                        oldBlock = null;
                        newBlock = null;
                    }
                }
            }

            return result;
        }

        private int Load()
        {
            int ret = 0;

            if (!string.IsNullOrEmpty(_filenames.csvFilename))
            {
                if (System.IO.File.Exists(_filenames.csvFilename))
                {
                    ret = LoadCsv();
                    if (ret < 0)
                        return ret;
                }
            }

            ret = LoadVideo();
            if (ret < 0)
                return ret;

            //ret = LoadAudio();
            //if (ret < 0)
            //    return ret;

            return ret;
        }

        private int LoadVideo()
        {
            {
                var _videoFile = new MediaFile();
                _videoFile.Resolution = MediaFile.ResolutionOption.Full;
                _videoFile.OutputRGBImage = false;
                _videoFile.OutputYImage = false;
                _videoFile.OutputYData = true;
                _videoFile.Open(_filenames.videoFilename);

                var fieldsPerSecond = _videoFile.FieldsPerSecond;

                // Fill with maps of 127s
                const int EDGE_MAP_AVERAGE_COUNT = 100;
                var edgeMapList = new LinkedList<ImageProcessing.YDataFloat>();

                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                Task taskLogoDetect = null;

                int fieldNumber = 0;
                int fieldNumberEdgeMapAvg = EDGE_MAP_AVERAGE_COUNT / -2;
                int lastPrintSeconds = 0;

                //return 0;

                // Get Logo detection base frame
                ImageProcessing.YDataFloat logoDetectionFrame;
                {
                    // Skip the first 5% and last 15% of the recording

                    var firstFrame = (int)(_videoFile.TotalFields * Settings.LogoDetectSearch_StartPosition_Percentage / 100);
                    firstFrame -= (firstFrame % _videoFile.FieldsPerFrame);
                    var lastFrame = (int)(_videoFile.TotalFields * Settings.LogoDetectSearch_EndPosition_Percentage / 100);
                    lastFrame -= (firstFrame % _videoFile.FieldsPerFrame);

                    const int LOGO_DETECTION_FRAMES_TO_AVERAGE = 250;
                    var frameInterval = (lastFrame - firstFrame) / LOGO_DETECTION_FRAMES_TO_AVERAGE;
                    frameInterval -= (frameInterval % _videoFile.FieldsPerFrame);
                    if (frameInterval < 2)
                        frameInterval = 2;
                    
                    for (fieldNumber = firstFrame; fieldNumber < lastFrame; fieldNumber += frameInterval)
                    {
                        var frameField = _videoFile.GetVideoFrameField(fieldNumber, MediaFile.SeekModes.Accurate);
                        var yData = frameField.YData;

                        if (taskLogoDetect != null)
                            taskLogoDetect.Wait();

                        taskLogoDetect = Task.Factory.StartNew(() =>
                            {
                                var yDataFloat = yData.GetFloatData();
                                var edgeData = ImageProcessing.ImageProcessor.GenerateEdgeDetectedImage(yDataFloat);
                                edgeMapList.AddLast(edgeData);
                                //edgeData.GetBitmap().Save(@"D:\temp\edgeData-" + fieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            });

                        //long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        //int elapsedSeconds = (int)(elapsedMilliseconds / 1000);
                        //if (elapsedSeconds != lastPrintSeconds)
                        //{
                        //    lastPrintSeconds = elapsedSeconds;
                        //    var fps = (fieldNumber / frameInterval) * 1000.0f / (float)elapsedMilliseconds;
                        //    Console.WriteLine("Frame: " + fieldNumber.ToString() + "  FPS: " + fps.ToString("0.00"));
                        //}
                    }
                    if (taskLogoDetect != null)
                        taskLogoDetect.Wait();

                    logoDetectionFrame = ImageProcessing.ImageProcessor.GenerateEdgeDetectionAverage(edgeMapList);
                    //logoDetectionFrame.GetBitmap().Save(@"D:\temp\logoFrame.png", System.Drawing.Imaging.ImageFormat.Png);

                    Console.WriteLine("Logo Detection time taken: " + stopwatch.Elapsed.ToString());
                }


                stopwatch.Restart();
                fieldNumber = 0;
                var emptyEdgeMap = ImageProcessing.YDataFloat.NewData(_videoFile.Width, _videoFile.Height / _videoFile.FieldsPerFrame, (float)byte.MaxValue, byte.MaxValue / 2.0f);
                edgeMapList.Clear();
                while (edgeMapList.Count < EDGE_MAP_AVERAGE_COUNT)
                    edgeMapList.AddLast(emptyEdgeMap);

                //while (fieldNumberEdgeMapAvg < _videoFile.TotalFields)
                while (fieldNumberEdgeMapAvg < 10000)
                {
                    if (fieldNumber < _videoFile.TotalFields)
                    {
                        var frameField = _videoFile.GetVideoFrameField(fieldNumber);
                        var yData = frameField.YData;
                        //yData.GetBitmap().Save(@"D:\temp\yData.png", System.Drawing.Imaging.ImageFormat.Png);

                        Field f = null;
                        while (Data.Fields.Count < fieldNumber)
                        {
                            f = new Field();
                            f.Invalid = true;
                            Data.Fields.Add(f);
                        }
                        if (Data.Fields.Count > fieldNumber)
                        {
                            f = Data.Fields[fieldNumber];
                        }
                        else
                        {
                            f = new Field();
                            f.PTS = frameField.PTS;
                            Data.Fields.Add(f);
                        }

                        // --------------
                        //Average and Max
                        float average = 0.0f;
                        float maximum = 0.0f;
                        ImageProcessing.ImageProcessor.CalculateAverageAndMaximumCPU(yData, out average, out maximum);
                        f.SetValue("Brightness_Avg", average);
                        f.SetValue("Brightness_Max", maximum);

                        // --------------
                        // Std Dev
                        float stdDev = 0.0f;
                        ImageProcessing.ImageProcessor.CalculateStandardDeviationCPU(yData, average, out stdDev);
                        f.SetValue("Brightness_StdDev", stdDev);


                        // --------------
                        // Edge detection
                        if ((fieldNumber % fieldsPerSecond) == 0)
                        {
                            var yDataFloat = yData.GetFloatData();
                            //yDataFloat.GetBitmap().Save(@"D:\temp\yDataFloat.png", System.Drawing.Imaging.ImageFormat.Png);

                            var edgeData = ImageProcessing.ImageProcessor.GenerateEdgeDetectedImage(yDataFloat);
                            //var edgeData = ImageProcessing.ImageProcessor.GenerateEdgeDetectedImageAccel(ImageProcessing.ImageProcessor.AcceleratorTarget.DX9, yDataFloat);

                            //edgeData.GetBitmap().Save(@"D:\temp\imageParallel.png", System.Drawing.Imaging.ImageFormat.Png);

                            edgeMapList.AddLast(edgeData);
                            while (edgeMapList.Count > EDGE_MAP_AVERAGE_COUNT)
                                edgeMapList.RemoveFirst();
                        }

                        frameField.Dispose();
                    }
                    else
                    {
                        if ((fieldNumber % fieldsPerSecond) == 0)
                        {
                            edgeMapList.AddLast(emptyEdgeMap);
                            while (edgeMapList.Count > EDGE_MAP_AVERAGE_COUNT)
                                edgeMapList.RemoveFirst();
                        }
                    }

                    if ((fieldNumberEdgeMapAvg >= 0) && (fieldNumberEdgeMapAvg < _videoFile.TotalFields))
                    {
                        if ((fieldNumberEdgeMapAvg % fieldsPerSecond) == 0)
                        {
                            if (taskLogoDetect != null)
                                taskLogoDetect.Wait();

                            //Console.Write("<");
                            var edgeMapListCopy = edgeMapList.ToList();
                            var fieldNumberEdgeMapAvgCopy = fieldNumberEdgeMapAvg;
                            taskLogoDetect = Task.Factory.StartNew(() =>
                                {
                                    var f = Data.Fields[fieldNumberEdgeMapAvgCopy];

                                    var edgeMapAvg = ImageProcessing.ImageProcessor.GenerateEdgeDetectionAverage(edgeMapListCopy);
                                    //var edgeMapAvg = ImageProcessing.ImageProcessor.GenerateEdgeDetectionAverageAccel(ImageProcessing.ImageProcessor.AcceleratorTarget.Multicore, edgeMapList);
                                    //var edgeMapAvg = ImageProcessing.ImageProcessor.GenerateEdgeDetectionAverageAccel(ImageProcessing.ImageProcessor.AcceleratorTarget.DX9, edgeMapList);
                                    //edgeMapAvg.GetBitmap().Save(@"D:\temp\edgeAvg-" + fieldNumberEdgeMapAvgCopy.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                                    float logoDifference;
                                    ImageProcessing.ImageProcessor.CalculateLogoDifference(edgeMapAvg, logoDetectionFrame, out logoDifference);
                                    f.SetValue("Logo_Difference", logoDifference);
                                });
                        }
                    }

                    fieldNumber++;
                    fieldNumberEdgeMapAvg++;

                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    int elapsedSeconds = (int)(elapsedMilliseconds / 1000);
                    if (elapsedSeconds != lastPrintSeconds)
                    {
                        lastPrintSeconds = elapsedSeconds;
                        var fps = fieldNumber * 1000.0f / (float)elapsedMilliseconds;
                        Console.WriteLine("Frame: " + fieldNumber.ToString() + "  FPS: " + fps.ToString("0.00"));
                    }
                }

                if (taskLogoDetect != null)
                    taskLogoDetect.Wait();

                Console.WriteLine("Video processing time taken: " + stopwatch.Elapsed.ToString());
            }

            return 0;
        }


        private int LoadCsv()
        {
            using (var fileStream = File.Open(_filenames.csvFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var file = new StreamReader(fileStream))
                {
                    string line = file.ReadLine();
                    if (line == null)
                    {
                        Log("Missing header");
                        return -1;
                    }

                    string[] parts = line.Split(',');
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        try
                        {
                            CsvColumnId columnId = (CsvColumnId)Enum.Parse(typeof(CsvColumnId), parts[i], true);
                            _columnIds.Add(columnId, i);
                        }
                        catch
                        {
                        }
                    }
                    _frameRate = int.Parse(line.Substring(line.LastIndexOf(',') + 1));

                    while ((line = file.ReadLine()) != null)
                    {
                        parts = line.Split(',');

                        var f = new Field();
                        foreach (CsvColumnId id in Enum.GetValues(typeof(CsvColumnId)))
                        {
                            var part = parts[_columnIds[id]];
                            if (id == CsvColumnId.PTS)
                                f.PTS = double.Parse(part);
                            else if (id == CsvColumnId.Frame)
                            {
                                int frameNumber = int.Parse(part) - 1;
                                f.SetValue("csv_" + id.ToString(), frameNumber); // Adjust frame numbers to start at 0 instead of 1.
                                f.FieldNumber = frameNumber * 2;
                            }
                            else
                                f.SetValue("csv_" + id.ToString(), int.Parse(part));
                        }

                        double? brightness = f.GetValue("csv_Brightness");
                        double? sound = f.GetValue("csv_Sound");
                        f.Invalid = ((brightness.HasValue && (brightness <= 0)) || (sound.HasValue && (sound < 0)));

                        while (Data.Fields.Count < f.FieldNumber)
                            Data.Fields.Add(null);
                        Data.Fields.Add(f);

                        // Add a copy since the file contains frames, not fields
                        f = f.Clone();
                        f.FieldNumber += 1;

                        Data.Fields.Add(f);
                    }
                }
            }
            return 0;
        }

        private void DetectThresholds()
        {
            if (Settings.BrightnessThreshold <= 0)
            {
                Data.BrightnessThreshold = DetectThreshold("csv_Brightness", out Data.BrightnessThresholdHistogram);
                Data.BrightnessThresholdPreSafety = Data.BrightnessThreshold;
                Data.BrightnessThreshold = (int)(Data.BrightnessThreshold * (1.0 + Settings.DetectBrightnessSafetyBufferPercent));
                Data.BrightnessThreshold += Settings.DetectBrightnessSafetyBufferOffset;
                Log("Adding safety buffer to threshold: " + Data.BrightnessThreshold.ToString());
            }
            else
            {
                Data.BrightnessThreshold = Settings.BrightnessThreshold;
                Log("Brightness threshold loaded: " + Data.BrightnessThreshold.ToString());
            }

            if (Settings.UniformThreshold <= 0)
            {
                Data.UniformThreshold = DetectThreshold("csv_Uniform", out Data.UniformThresholdHistogram);
                Data.UniformThresholdPreSafety = Data.UniformThreshold;
                Data.UniformThreshold = (int)(Data.UniformThreshold * (1.0 + Settings.DetectUniformSafetyBufferPercent));
                Data.UniformThreshold += Settings.DetectUniformSafetyBufferOffset;
                Log("Adding safety buffer to threshold: " + Data.UniformThreshold.ToString());
            }
            else
            {
                Data.UniformThreshold += Settings.UniformThreshold;
                Log("Uniform threshold loaded: " + Data.UniformThreshold.ToString());
            }

            if (Settings.SoundThreshold <= 0)
            {
                Data.SoundThreshold = DetectThreshold("csv_Sound", out Data.SoundThresholdHistogram);
                Data.SoundThresholdPreSafety = Data.SoundThreshold;
                Data.SoundThreshold = (int)(Data.SoundThreshold * (1.0 + Settings.DetectSoundSafetyBufferPercent));
                Data.SoundThreshold += Settings.DetectSoundSafetyBufferOffset;
                Log("Adding safety buffer to threshold: " + Data.SoundThreshold.ToString());
            }
            else
            {
                Data.SoundThreshold += Settings.SoundThreshold;
                Log("Sound threshold loaded: " + Data.SoundThreshold.ToString());
            }

        }

        private int FindBlackFields()
        {
            {
                var blackFieldGroup = new BlackFieldGroup();

                foreach (var field in Data.Fields)
                {
                    if (Data.ManualCutFields.Contains(field.FieldNumber))
                    {
                        if (blackFieldGroup.Fields.Count > 0)
                        {
                            Data.BlackFieldGroups.Add(blackFieldGroup);
                            blackFieldGroup = new BlackFieldGroup();
                        }
                        blackFieldGroup.Fields.Add(field);
                        blackFieldGroup.IsManualCutPoint = true;
                        Data.BlackFieldGroups.Add(blackFieldGroup);
                        blackFieldGroup = new BlackFieldGroup();
                        continue;
                    }

                    if (field.Invalid)
                        continue;

                    double? brightness = field.GetValue("csv_Brightness");
                    double? uniform = field.GetValue("csv_Uniform");
                    double? sound = field.GetValue("csv_Sound");

                    if (!brightness.HasValue || !uniform.HasValue || !sound.HasValue)
                        continue;

                    if ((brightness <= Data.BrightnessThreshold) &&
                        (uniform <= Data.UniformThreshold) &&
                        (sound <= Data.SoundThreshold))
                    {
                        blackFieldGroup.Fields.Add(field);
                    }
                    else if (blackFieldGroup.Fields.Count > 0)
                    {
                        Data.BlackFieldGroups.Add(blackFieldGroup);
                        blackFieldGroup = new BlackFieldGroup();
                    }
                }
                if (blackFieldGroup.Fields.Count > 0)
                {
                    Data.BlackFieldGroups.Add(blackFieldGroup);
                }
            }

            // Process the black field groups to determine Cut Points.
            foreach (var blackFieldGroup in Data.BlackFieldGroups)
            {
                int cutFieldIndex = -1;
                double minBrightness = (from f in blackFieldGroup.Fields
                                        let bright = f.GetValue("csv_Brightness")
                                        where bright.HasValue
                                        select bright.Value).Min();
                double minSound = int.MaxValue;

                for (int fieldIndex = 0; fieldIndex < blackFieldGroup.Fields.Count; fieldIndex++)
                {
                    var field = blackFieldGroup.Fields[fieldIndex];
                    double brightness = field.GetValue("csv_Brightness").Value - minBrightness;
                    if (brightness == 0)
                    {
                        double sound = field.GetValue("csv_Sound").Value;
                        if (sound < minSound)
                        {
                            minSound = sound;
                        }
                        cutFieldIndex = fieldIndex;
                        field.State = "-";
                    }
                    else
                    {
                        field.State = "-".PadRight((int)brightness, '-');
                    }
                }

                for (int fieldIndex = cutFieldIndex; fieldIndex >= 0; fieldIndex--)
                {
                    var field = blackFieldGroup.Fields[fieldIndex];
                    double brightness = field.GetValue("csv_Brightness").Value - minBrightness;
                    if (brightness == 0)
                    {
                        double sound = field.GetValue("csv_Sound").Value;
                        if (sound == minSound)
                        {
                            cutFieldIndex = fieldIndex;
                            break;
                        }
                        field.State = "- v";
                    }
                }


                blackFieldGroup.CutField = blackFieldGroup.Fields[cutFieldIndex];
                blackFieldGroup.CutField.State = "- *";
            }

            // log black field details
            var blackFieldGroupCountText = Data.BlackFieldGroups.Count.ToString();
            var maxFieldTextLength = Math.Min(Data.Fields.Count.ToString().Length, "field".Length);

            Log("");
            Log("Initial black field groups: " + blackFieldGroupCountText);

            var line = new StringBuilder();
            line.Append("nr".PadLeft(blackFieldGroupCountText.Length));
            line.Append(" " + "Field".PadLeft(maxFieldTextLength));
            line.Append(" Brightness");
            line.Append(" Uniform");
            line.Append(" Sound");
            line.Append("   State");
            Log(line.ToString());

            for (int blackFieldGroupIndex = 0; blackFieldGroupIndex < Data.BlackFieldGroups.Count; blackFieldGroupIndex++)
            {
                var blackFieldGroup = Data.BlackFieldGroups[blackFieldGroupIndex];

                foreach (var field in blackFieldGroup.Fields)
                {
                    double brightness = field.GetValue("csv_Brightness").Value;
                    double uniform = field.GetValue("csv_Uniform").Value;
                    double sound = field.GetValue("csv_Sound").Value;

                    line = new StringBuilder();
                    line.Append(blackFieldGroupIndex.ToString().PadLeft(blackFieldGroupCountText.Length));
                    line.Append(" " + field.FieldNumber.ToString().PadLeft(maxFieldTextLength));
                    line.Append(" " + brightness.ToString("0.00").PadLeft("Brightness".Length));
                    line.Append(" " + uniform.ToString("0.00").PadLeft("Uniform".Length));
                    line.Append(" " + sound.ToString("0.00").PadLeft("Sound".Length));
                    line.Append(" " + field.State);
                    Log(line.ToString());
                }
                Log("-----------------------------");
            }

            // Create blocks
            Field startField = Data.Fields[0];
            bool startFieldIsManualCutPoint = false;
            for (int blackFieldGroupIndex = 0; blackFieldGroupIndex <= Data.BlackFieldGroups.Count; blackFieldGroupIndex++)
            {
                Field nextField = null;
                int endFieldNumber;
                bool isManualCutPoint = false;
                if (blackFieldGroupIndex < Data.BlackFieldGroups.Count)
                {
                    var blackFieldGroup = Data.BlackFieldGroups[blackFieldGroupIndex];
                    nextField = blackFieldGroup.CutField;
                    endFieldNumber = nextField.FieldNumber - 1;
                    isManualCutPoint = blackFieldGroup.IsManualCutPoint;
                }
                else
                {
                    endFieldNumber = Data.Fields.Count - 1;
                }

                if (endFieldNumber <= startField.FieldNumber)
                    continue;

                Field endField = Data.Fields[endFieldNumber];

                var block = new Block();
                block.BlockNumber = Data.Blocks.Count;
                block.StartField = startField;
                block.EndField = endField;
                block.IsManualCutPoint = startFieldIsManualCutPoint;
                Data.Blocks.Add(block);

                startField = nextField;
                startFieldIsManualCutPoint = isManualCutPoint;
            }

            // Calculate whole file averages
            Log("Whole file averages");
            Log("#    Start    End |  Bright      S/C   Logo    Uniform    Sound LogSound   Length");
            Data.WholeFileAverages = new Block();
            Data.WholeFileAverages.StartField = Data.Fields[0];
            Data.WholeFileAverages.EndField = Data.Fields[Data.Fields.Count - 1];
            CalculateBlockAverages(Data.WholeFileAverages);
            PrintBlockAverages(Data.WholeFileAverages, "");

            // Calculate block averages
            Log("");
            Log("Block averages");
            for (int blockIndex = 0; blockIndex < Data.Blocks.Count; blockIndex++)
            {
                var block = Data.Blocks[blockIndex];
                CalculateBlockAverages(block);
                PrintBlockAverages(block, blockIndex.ToString());
            }


            // Scoring blocks
            Log("");
            Log("Scoring blocks");
            Log("#          Bright     Logo  Uniform LogSound      Score  #");
            for (int blockIndex = 0; blockIndex < Data.Blocks.Count; blockIndex++)
            {
                var block = Data.Blocks[blockIndex];

                block.ScoreBrightness = (block.AverageBrightness / Data.WholeFileAverages.AverageBrightness) - 1.0;
                var aboveAverageBrightness = Math.Max(0.0, block.ScoreBrightness - Settings.ScoreAboveAverageBrightnessThreshold) * Settings.ScoreAboveAverageBrightnessMultiplier;
                var belowAverageBrightness = Math.Max(0.0, Settings.ScoreBelowAverageBrightnessThreshold - block.ScoreBrightness) * Settings.ScoreBelowAverageBrightnessMultiplier;
                block.ScoreBrightness = aboveAverageBrightness + belowAverageBrightness;

                block.ScoreLogo = (block.AverageLogo) - 0.5;
                var aboveAverageLogo = Math.Max(0.0, block.ScoreLogo - Settings.ScoreAboveAverageLogoThreshold) * Settings.ScoreAboveAverageLogoMultiplier;
                var belowAverageLogo = Math.Max(0.0, Settings.ScoreBelowAverageLogoThreshold - block.ScoreLogo) * Settings.ScoreBelowAverageLogoMultiplier;
                block.ScoreLogo = aboveAverageLogo + belowAverageLogo;

                block.ScoreUniform = (block.AverageUniform / Data.WholeFileAverages.AverageUniform) - 1.0;
                var aboveAverageUniform = Math.Max(0.0, block.ScoreUniform - Settings.ScoreAboveAverageUniformThreshold) * Settings.ScoreAboveAverageUniformMultiplier;
                var belowAverageUniform = Math.Max(0.0, Settings.ScoreBelowAverageUniformThreshold - block.ScoreUniform) * Settings.ScoreBelowAverageUniformMultiplier;
                block.ScoreUniform = aboveAverageUniform + belowAverageUniform;

                block.ScoreLogSound = (block.AverageLogSound / Data.WholeFileAverages.AverageLogSound) - 1.0;
                var aboveAverageLogSound = Math.Max(0.0, block.ScoreLogSound - Settings.ScoreAboveAverageLogSoundThreshold) * Settings.ScoreAboveAverageLogSoundMultiplier;
                var belowAverageLogSound = Math.Max(0.0, Settings.ScoreBelowAverageLogSoundThreshold - block.ScoreLogSound) * Settings.ScoreBelowAverageLogSoundMultiplier;
                block.ScoreLogSound = aboveAverageLogSound + belowAverageLogSound;

                block.Score = block.ScoreBrightness + block.ScoreLogo + block.ScoreUniform + block.ScoreLogSound;

                block.IsCommercial = false;
                if (block.Score < 0.0)
                    block.IsCommercial = true;

                var text = new StringBuilder();
                text.Append(blockIndex.ToString().PadRight(8) + " ");
                text.Append(block.ScoreBrightness.ToString("+0.00;-0.00").PadLeft(8) + " ");
                text.Append(block.ScoreLogo.ToString("+0.00;-0.00").PadLeft(8) + " ");
                text.Append(block.ScoreUniform.ToString("+0.00;-0.00").PadLeft(8) + " ");
                text.Append(block.ScoreLogSound.ToString("+0.00;-0.00").PadLeft(8) + " = ");
                text.Append(block.Score.ToString("+0.00;-0.00").PadLeft(8) + "  ");
                text.Append(blockIndex.ToString());
                Log(text.ToString());
            }


            // TODO: Merge adjacent blocks

            return 0;
        }

        public int WriteCuttermaranFile()
        {
            Log("");
            Log("Writing cuttermaran file");
            {
                using (var cuttermaranFile = new CuttermaranFileWriter(_cuttermaranFilename, _filenames.videoFilename, _filenames.audioFilename, Data.Fields.Count))
                {
                    foreach (var block in Data.Blocks)
                    {
                        if (!block.IsCommercial)
                            cuttermaranFile.WriteCut(block.StartField.FieldNumber, block.EndField.FieldNumber);
                    }
                }
            }
            {
                var cuttermaranFilename = Path.GetDirectoryName(_filenames.inputFilename) + "\\" + Path.GetFileNameWithoutExtension(_filenames.inputFilename) + "_allblocks.cpf";
                using (var cuttermaranFile = new CuttermaranFileWriter(cuttermaranFilename, _filenames.videoFilename, _filenames.audioFilename, Data.Fields.Count))
                {
                    foreach (var block in Data.Blocks)
                    {
                        cuttermaranFile.WriteCut(block.StartField.FieldNumber, block.EndField.FieldNumber);
                    }
                }
            }

            return 0;
        }

        private int DetectThreshold(string columnId, out HistogramData histogram)
        {
            histogram = GetValleysHistogram(columnId);
            PrintHistogram(columnId + " Valleys Histogram", histogram);

            for (int i = 1; i < 256; i++)
            {
                int current = histogram[i];
                int previous = histogram[i - 1];
                if (current < previous)
                {
                    i--;
                    Log(columnId + " threshold detected: " + i.ToString());
                    return i;
                }
            }

            return 0;
        }

        private HistogramData GetValuesHistogram(string columnId)
        {
            var histogram = new HistogramData();
            foreach (var field in Data.Fields)
            {
                if (field.Invalid)
                    continue;

                double? value = field.GetValue(columnId);
                if (!value.HasValue)
                    continue;

                histogram.Add(value.Value);
            }
            return histogram;
        }

        private HistogramData GetValleysHistogram(string columnId)
        {
            var histogram = new HistogramData();
            var histogramFields = new Dictionary<double, List<int>>();
            double lastValue = 0;
            bool increasing = true;
            foreach (var field in Data.Fields)
            {
                if (field.Invalid)
                    continue;

                double? value = field.GetValue(columnId);
                if (!value.HasValue)
                    continue;

                if (value < lastValue)
                {
                    increasing = false;
                }
                else if (value > lastValue)
                {
                    if (!increasing)
                    {
                        histogram.Add(lastValue);

                        List<int> fields;
                        histogramFields.TryGetValue(lastValue, out fields);
                        if (fields == null)
                            fields = new List<int>();
                        fields.Add(field.FieldNumber);
                        histogramFields[lastValue] = fields;
                    }
                    increasing = true;
                }
                lastValue = value.Value;
            }
            return histogram;
        }

        private void PrintHistogram(string title, HistogramData histogram)
        {
            Log("");
            Log(title);
            int firstI = -1;
            for (int i = 0; i < 100; i++)
            {
                int count = histogram[i];
                if ((firstI == -1) && (count > 0))
                    firstI = i;
                if (i > firstI + 20)
                    break;
                int percentage = 100 * count / histogram.MaximumValue;
                Log(i.ToString().PadLeft(3) + ", " + count.ToString().PadLeft(histogram.MaximumValue.ToString().Length) + " : " + "".PadRight(percentage, '*'));
            }
        }

        private void CalculateBlockAverages(Block block)
        {
            int startFieldIndex = Data.Fields.IndexOf(block.StartField);
            int endFieldIndex = Data.Fields.IndexOf(block.EndField);
            int fieldCount = endFieldIndex - startFieldIndex + 1;

            double brightness = 0;
            double scene_change = 0;
            double logo = 0;
            double uniform = 0;
            double sound = 0;
            double logSound = 0;

            for (int fieldIndex = startFieldIndex; fieldIndex <= endFieldIndex; fieldIndex++)
            {
                var field = Data.Fields[fieldIndex];

                brightness += field.GetValue("csv_Brightness").Value;
                scene_change += field.GetValue("csv_Scene_Change").Value;
                logo += field.GetValue("csv_Logo").Value;
                uniform += field.GetValue("csv_Uniform").Value;
                double fieldSound = field.GetValue("csv_Sound").Value;
                sound += fieldSound;
                double logSoundValue = 0.0;
                if (fieldSound >= 1.0)
                {
                    logSoundValue = Math.Log(fieldSound, 2);
                }
                //double logSoundValue = field.Sound * field.Sound / 1000.0;
                logSound += logSoundValue;
            }

            block.AverageBrightness = brightness / (double)fieldCount;
            block.AverageSceneChange = scene_change / (double)fieldCount;
            block.AverageLogo = logo / (double)fieldCount;
            block.AverageUniform = uniform / (double)fieldCount;
            block.AverageSound = sound / (double)fieldCount;
            block.AverageLogSound = logSound / (double)fieldCount;

            block.Length = block.EndField.PTS - block.StartField.PTS;
        }

        private void PrintBlockAverages(Block block, string title)
        {
            var text = new StringBuilder();
            text.Append(title.PadRight(4));
            text.Append(block.StartField.FieldNumber.ToString().PadLeft(6) + " ");
            text.Append(block.EndField.FieldNumber.ToString().PadLeft(6) + " |");
            text.Append(block.AverageBrightness.ToString("0.00").PadLeft(8) + " ");
            text.Append(block.AverageSceneChange.ToString("0.00").PadLeft(8) + " ");
            text.Append(block.AverageLogo.ToString("0.00").PadLeft(6) + " ");
            text.Append(block.AverageUniform.ToString("0.00").PadLeft(10) + " ");
            text.Append(block.AverageSound.ToString("0.00").PadLeft(8) + " ");
            text.Append(block.AverageLogSound.ToString("0.00").PadLeft(8) + " ");
            text.Append(block.Length.ToString("0.00").PadLeft(8) + " ");
            Log(text.ToString());
        }

    }
}
