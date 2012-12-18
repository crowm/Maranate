using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace ComskipToCuttermaran
{
    class ComskipCsvProcessor
    {
        private string _inputName;
        private string _videoFilename;
        private string _audioFilename;
        private int _frameOffset;
        private int _frameRate;
        private Dictionary<ColumnId, int> _columnIds = new Dictionary<ColumnId, int>();

        #region "Settings"
        public Settings_info Settings = new Settings_info();
        public class Settings_info
        {
            public int BrightnessThreshold = -1;
            public double DetectBrightnessSafetyBufferPercent = 0.2;
            public int DetectBrightnessSafetyBufferOffset = 0;

            public int UniformThreshold = -1;
            public double DetectUniformSafetyBufferPercent = 0.0;
            public int DetectUniformSafetyBufferOffset = 20;

            public int SoundThreshold = -1;
            public double DetectSoundSafetyBufferPercent = 0.0;
            public int DetectSoundSafetyBufferOffset = 20;

            public double ScoreAboveAverageBrightnessThreshold = 0.0;
            public double ScoreAboveAverageBrightnessMultiplier = -1.5;
            public double ScoreBelowAverageBrightnessThreshold = 0.0;
            public double ScoreBelowAverageBrightnessMultiplier = 1.5;

            public double ScoreAboveAverageLogoThreshold = 0.0;
            public double ScoreAboveAverageLogoMultiplier = 1.0;
            public double ScoreBelowAverageLogoThreshold = 0.0;
            public double ScoreBelowAverageLogoMultiplier = -0.5;

            public double ScoreAboveAverageUniformThreshold = 0.0;
            public double ScoreAboveAverageUniformMultiplier = -0.5;
            public double ScoreBelowAverageUniformThreshold = 0.0;
            public double ScoreBelowAverageUniformMultiplier = 0.4;

            public double ScoreAboveAverageLogSoundThreshold = 0.1;
            public double ScoreAboveAverageLogSoundMultiplier = -1.0;
            public double ScoreBelowAverageLogSoundThreshold = -0.2;
            public double ScoreBelowAverageLogSoundMultiplier = -0.5;
        }
        #endregion

        #region "Data"
        public Data_info Data = new Data_info();
        public class Data_info
        {
            public List<Frame> Frames = new List<Frame>();
            public List<BlackFrameGroup> BlackFrameGroups = new List<BlackFrameGroup>();
            public List<Block> Blocks = new List<Block>();
            public Block WholeFileAverages = new Block();

            public int? BrightnessThreshold;
            public int? UniformThreshold;
            public int? SoundThreshold;

            public HistogramData BrightnessThresholdHistogram;
            public HistogramData UniformThresholdHistogram;
            public HistogramData SoundThresholdHistogram;
        }
        #endregion

        #region "Classes & Enum"
        public enum ColumnId
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
        public class Frame
        {
            public Dictionary<ColumnId, int> Values = new Dictionary<ColumnId, int>();

            public int FrameNumber { get { return Values[ColumnId.Frame]; } }
            public int Brightness { get { return Values[ColumnId.Brightness]; } }
            public int Scene_Change { get { return Values[ColumnId.Scene_Change]; } }
            public int Logo { get { return Values[ColumnId.Logo]; } }
            public int Uniform { get { return Values[ColumnId.Uniform]; } }
            public int Sound { get { return Values[ColumnId.Sound]; } }

            public double PTS;
            public string State;

            public bool? _invalid;
            public bool Invalid
            {
                get
                {
                    if (_invalid == null)
                    {
                        int brightness = this.Brightness;
                        int sound = this.Sound;
                        _invalid = ((brightness <= 0) || (sound < 0));
                    }
                    return _invalid.Value;
                }
            }

            public override string ToString()
            {
                return FrameNumber.ToString() + " (" + Brightness.ToString() + ")";
            }
        }

        public class BlackFrameGroup
        {
            public List<Frame> Frames = new List<Frame>();
            public Frame CutFrame;
        }

        public class Block
        {
            public Frame StartFrame;
            public Frame EndFrame;

            public double Length;
            public double Score;

            public double AverageBrightness;
            public double AverageUniform;
            public double AverageSceneChange;
            public double AverageSound;
            public double AverageLogSound;
            public double AverageLogo;
            public bool StrictLength;
            public bool IsCommercial;

            public double ScoreBrightness;
            public double ScoreLogo;
            public double ScoreUniform;
            public double ScoreLogSound;

            public override string ToString()
            {
                return ((StartFrame == null) ? "null" : StartFrame.FrameNumber.ToString())
                    + " - "
                    + ((EndFrame == null) ? "null" : EndFrame.FrameNumber.ToString())
                    + " : " + Length.ToString()
                    + " : " + Score.ToString();
            }
        }

        public class HistogramData
        {
            public Dictionary<int, int> Data = new Dictionary<int,int>();
            public int MaximumValue = 0;

            public int this[int key]
            {
                get
                {
                    int value = 0;
                    Data.TryGetValue(key, out value);
                    return value;
                }
            }

            public void Add(int value)
            {
                int count = 0;
                Data.TryGetValue(value, out count);
                count++;
                Data[value] = count;
                if (MaximumValue < count)
                    MaximumValue = count;
            }
        }
        #endregion

        public event Action<string> Log;

        public ComskipCsvProcessor(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            _inputName = inputName;
            _videoFilename = videoFilename;
            _audioFilename = audioFilename;
            _frameOffset = frameOffset;
        }

        public int Process()
        {
            int result = 0;
            Log("Processing Comskip csv file.");

            result = Load();
            if (result < 0)
                return result;

            // Detect thresholds
            if (Data.BrightnessThreshold == null)
            {
                if (Settings.BrightnessThreshold <= 0)
                {
                    Data.BrightnessThreshold = DetectThreshold(ColumnId.Brightness, out Data.BrightnessThresholdHistogram);
                    Data.BrightnessThreshold = (int)(Data.BrightnessThreshold * (1.0 + Settings.DetectBrightnessSafetyBufferPercent));
                    Data.BrightnessThreshold += Settings.DetectBrightnessSafetyBufferOffset;
                    Log("Adding safety buffer to threshold: " + Data.BrightnessThreshold.ToString());
                }
                else
                {
                    Data.BrightnessThreshold = Settings.BrightnessThreshold;
                    Log("Brightness threshold loaded: " + Data.BrightnessThreshold.ToString());
                }
            }

            if (Data.UniformThreshold == null)
            {
                if (Settings.UniformThreshold <= 0)
                {
                    Data.UniformThreshold = DetectThreshold(ColumnId.Uniform, out Data.UniformThresholdHistogram);
                    Data.UniformThreshold = (int)(Data.UniformThreshold * (1.0 + Settings.DetectUniformSafetyBufferPercent));
                    Data.UniformThreshold += Settings.DetectUniformSafetyBufferOffset;
                    Log("Adding safety buffer to threshold: " + Data.UniformThreshold.ToString());
                }
                else
                {
                    Data.UniformThreshold += Settings.UniformThreshold;
                    Log("Uniform threshold loaded: " + Data.UniformThreshold.ToString());
                }
            }

            if (Data.SoundThreshold == null)
            {
                if (Settings.SoundThreshold <= 0)
                {
                    Data.SoundThreshold = DetectThreshold(ColumnId.Sound, out Data.SoundThresholdHistogram);
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

            result = FindBlackFrames();

            return result;
        }

        private int Load()
        {
            using (var fileStream = File.Open(_inputName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                            ColumnId columnId = (ColumnId)Enum.Parse(typeof(ColumnId), parts[i], true);
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

                        var f = new Frame();
                        foreach (ColumnId id in Enum.GetValues(typeof(ColumnId)))
                        {
                            var part = parts[_columnIds[id]];
                            if (id == ColumnId.PTS)
                                f.PTS = double.Parse(part);
                            else if (id == ColumnId.Frame)
                                f.Values[id] = int.Parse(part) - 1; // Adjust frame numbers to start at 0 instead of 1.
                            else
                                f.Values[id] = int.Parse(part);
                        }
                        Data.Frames.Add(f);
                    }
                }
            }
            return 0;
        }

        private int FindBlackFrames()
        {
            {
                var blackFrameGroup = new BlackFrameGroup();

                foreach (var frame in Data.Frames)
                {
                    if (frame.Invalid)
                        continue;

                    int brightness = frame.Brightness;
                    int uniform = frame.Uniform;
                    int sound = frame.Sound;

                    if ((brightness <= Data.BrightnessThreshold) &&
                        (uniform <= Data.UniformThreshold) &&
                        (sound <= Data.SoundThreshold))
                    {
                        blackFrameGroup.Frames.Add(frame);
                    }
                    else if (blackFrameGroup.Frames.Count > 0)
                    {
                        Data.BlackFrameGroups.Add(blackFrameGroup);
                        blackFrameGroup = new BlackFrameGroup();
                    }
                }
                if (blackFrameGroup.Frames.Count > 0)
                {
                    Data.BlackFrameGroups.Add(blackFrameGroup);
                }
            }

            // Process the black frame groups to determine Cut Points.
            foreach (var blackFrameGroup in Data.BlackFrameGroups)
            {
                int cutFrameIndex = -1;
                int minBrightness = (from f in blackFrameGroup.Frames select f.Brightness).Min();
                int minSound = int.MaxValue;

                for (int frameIndex = 0; frameIndex < blackFrameGroup.Frames.Count; frameIndex++)
                {
                    var frame = blackFrameGroup.Frames[frameIndex];
                    int brightness = frame.Brightness - minBrightness;
                    if (brightness == 0)
                    {
                        int sound = frame.Sound;
                        if (sound < minSound)
                        {
                            minSound = sound;
                        }
                        cutFrameIndex = frameIndex;
                        frame.State = "-";
                    }
                    else
                    {
                        frame.State = "-".PadRight(brightness, '-');
                    }
                }

                for (int frameIndex = cutFrameIndex; frameIndex >= 0; frameIndex--)
                {
                    var frame = blackFrameGroup.Frames[frameIndex];
                    int brightness = frame.Brightness - minBrightness;
                    if (brightness == 0)
                    {
                        int sound = frame.Sound;
                        if (sound == minSound)
                        {
                            cutFrameIndex = frameIndex;
                            break;
                        }
                        frame.State = "- v";
                    }
                }


                blackFrameGroup.CutFrame = blackFrameGroup.Frames[cutFrameIndex];
                blackFrameGroup.CutFrame.State = "- *";
            }

            // log black frame details
            var blackFrameGroupCountText = Data.BlackFrameGroups.Count.ToString();
            var maxFrameTextLength = Math.Min(Data.Frames.Count.ToString().Length, "frame".Length);

            Log("");
            Log("Initial black frame groups: " + blackFrameGroupCountText);

            var line = new StringBuilder();
            line.Append("nr".PadLeft(blackFrameGroupCountText.Length));
            line.Append(" " + "Frame".PadLeft(maxFrameTextLength));
            line.Append(" Brightness");
            line.Append(" Uniform");
            line.Append(" Sound");
            line.Append("   State");
            Log(line.ToString());

            for (int blackFrameGroupIndex = 0; blackFrameGroupIndex < Data.BlackFrameGroups.Count; blackFrameGroupIndex++)
            {
                var blackFrameGroup = Data.BlackFrameGroups[blackFrameGroupIndex];

                foreach (var frame in blackFrameGroup.Frames)
                {
                    line = new StringBuilder();
                    line.Append(blackFrameGroupIndex.ToString().PadLeft(blackFrameGroupCountText.Length));
                    line.Append(" " + frame.FrameNumber.ToString().PadLeft(maxFrameTextLength));
                    line.Append(" " + frame.Brightness.ToString().PadLeft("Brightness".Length));
                    line.Append(" " + frame.Uniform.ToString().PadLeft("Uniform".Length));
                    line.Append(" " + frame.Sound.ToString().PadLeft("Sound".Length));
                    line.Append(" " + frame.State);
                    Log(line.ToString());
                }
                Log("-----------------------------");
            }

            // Create blocks
            Frame startFrame = Data.Frames[0];
            for (int blackFrameGroupIndex = 0; blackFrameGroupIndex <= Data.BlackFrameGroups.Count; blackFrameGroupIndex++)
            {
                Frame nextFrame = null;
                int endFrameNumber;
                if (blackFrameGroupIndex < Data.BlackFrameGroups.Count)
                {
                    var blackFrameGroup = Data.BlackFrameGroups[blackFrameGroupIndex];
                    nextFrame = blackFrameGroup.CutFrame;
                    endFrameNumber = nextFrame.FrameNumber - 1;
                }
                else
                {
                    endFrameNumber = Data.Frames.Count - 1;
                }

                if (endFrameNumber <= startFrame.FrameNumber)
                    continue;

                Frame endFrame = Data.Frames[endFrameNumber];

                var block = new Block();
                block.StartFrame = startFrame;
                block.EndFrame = endFrame;
                Data.Blocks.Add(block);

                startFrame = nextFrame;
            }

            // Calculate whole file averages
            Log("Whole file averages");
            Log("#    Start    End |  Bright      S/C   Logo    Uniform    Sound LogSound   Length");
            Data.WholeFileAverages = new Block();
            Data.WholeFileAverages.StartFrame = Data.Frames.First();
            Data.WholeFileAverages.EndFrame = Data.Frames.Last();
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

            Log("");
            Log("Writing cuttermaran file");
            {
                var cuttermaranFilename = Path.GetDirectoryName(_inputName) + "\\" + Path.GetFileNameWithoutExtension(_inputName) + ".cpf";
                using (var cuttermaranFile = new CuttermaranFileWriter(cuttermaranFilename, _videoFilename, _audioFilename, Data.Frames.Count))
                {
                    foreach (var block in Data.Blocks)
                    {
                        if (!block.IsCommercial)
                            cuttermaranFile.WriteCut(block.StartFrame.FrameNumber, block.EndFrame.FrameNumber);
                    }
                }
            }
            {
                var cuttermaranFilename = Path.GetDirectoryName(_inputName) + "\\" + Path.GetFileNameWithoutExtension(_inputName) + "_allblocks.cpf";
                using (var cuttermaranFile = new CuttermaranFileWriter(cuttermaranFilename, _videoFilename, _audioFilename, Data.Frames.Count))
                {
                    foreach (var block in Data.Blocks)
                    {
                        cuttermaranFile.WriteCut(block.StartFrame.FrameNumber, block.EndFrame.FrameNumber);
                    }
                }
            }

            return 0;
        }

        private int DetectThreshold(ColumnId columnId, out HistogramData histogram)
        {
            histogram = GetValleysHistogram(columnId);
            PrintHistogram(columnId.ToString() + " Valleys Histogram", histogram);

            for (int i = 1; i < 256; i++)
            {
                int current = histogram[i];
                int previous = histogram[i - 1];
                if (current < previous)
                {
                    i--;
                    Log(columnId.ToString() + " threshold detected: " + i.ToString());
                    return i;
                }
            }

            return 0;
        }

        private HistogramData GetValuesHistogram(ColumnId columnId)
        {
            var histogram = new HistogramData();
            foreach (var frame in Data.Frames)
            {
                if (frame.Invalid)
                    continue;

                int value = frame.Values[columnId];
                histogram.Add(value);
            }
            return histogram;
        }

        private HistogramData GetValleysHistogram(ColumnId columnId)
        {
            var histogram = new HistogramData();
            var histogramFrames = new Dictionary<int, List<int>>();
            int lastValue = 0;
            bool increasing = true;
            foreach (var frame in Data.Frames)
            {
                if (frame.Invalid)
                    continue;

                int value = frame.Values[columnId];

                if (value < lastValue)
                {
                    increasing = false;
                }
                else if (value > lastValue)
                {
                    if (!increasing)
                    {
                        histogram.Add(lastValue);

                        List<int> frames;
                        histogramFrames.TryGetValue(lastValue, out frames);
                        if (frames == null)
                            frames = new List<int>();
                        frames.Add(frame.FrameNumber);
                        histogramFrames[lastValue] = frames;
                    }
                    increasing = true;
                }
                lastValue = value;
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
            int startFrameIndex = Data.Frames.IndexOf(block.StartFrame);
            int endFrameIndex = Data.Frames.IndexOf(block.EndFrame);
            int frameCount = endFrameIndex - startFrameIndex + 1;

            long brightness = 0;
            long scene_change = 0;
            long logo = 0;
            long uniform = 0;
            long sound = 0;
            double logSound = 0;

            for (int frameIndex = startFrameIndex; frameIndex <= endFrameIndex; frameIndex++)
            {
                var frame = Data.Frames[frameIndex];

                brightness += frame.Brightness;
                scene_change += frame.Scene_Change;
                logo += frame.Logo;
                uniform += frame.Uniform;
                sound += frame.Sound;
                double logSoundValue = 0.0;
                if (frame.Sound >= 1.0)
                {
                    logSoundValue = Math.Log(frame.Sound, 2);
                }
                //double logSoundValue = frame.Sound * frame.Sound / 1000.0;
                logSound += logSoundValue;
            }

            block.AverageBrightness = brightness / (double)frameCount;
            block.AverageSceneChange = scene_change / (double)frameCount;
            block.AverageLogo = logo / (double)frameCount;
            block.AverageUniform = uniform / (double)frameCount;
            block.AverageSound = sound / (double)frameCount;
            block.AverageLogSound = logSound / (double)frameCount;

            block.Length = block.EndFrame.PTS - block.StartFrame.PTS;
        }

        private void PrintBlockAverages(Block block, string title)
        {
            var text = new StringBuilder();
            text.Append(title.PadRight(4));
            text.Append(block.StartFrame.FrameNumber.ToString().PadLeft(6) + " ");
            text.Append(block.EndFrame.FrameNumber.ToString().PadLeft(6) + " |");
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
