using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maranate.Statistics
{
    public interface IStatisticsProcessor
    {
        Data_info Data { get; }

        event Action<string> Log;

        int Process();
        int Reprocess(bool keepManualCommercialOverrides);

        int WriteCuttermaranFile();
    }

    #region "Classes"

    public class Data_info
    {
        public Dictionary<string, int> ColumnIds = new Dictionary<string, int>();
        public List<Field> Fields = new List<Field>();
        public List<BlackFieldGroup> BlackFieldGroups = new List<BlackFieldGroup>();
        public List<Block> Blocks = new List<Block>();
        public Block WholeFileAverages = new Block();

        public HashSet<int> ManualCutFields = new HashSet<int>();

        public int BrightnessThreshold;
        public int UniformThreshold;
        public int SoundThreshold;

        public int BrightnessThresholdPreSafety;
        public int UniformThresholdPreSafety;
        public int SoundThresholdPreSafety;

        public HistogramData BrightnessThresholdHistogram;
        public HistogramData UniformThresholdHistogram;
        public HistogramData SoundThresholdHistogram;
    }

    public class Field
    {
        private Dictionary<string, int> _columnIds = null;
        private Dictionary<int, double> _values = new Dictionary<int, double>();

        public int FieldNumber = -1;
        public double PTS = -1.0;
        public string State;
        public bool Invalid = false;

        public Field(Data_info container)
        {
            _columnIds = container.ColumnIds;
        }

        public void SetValue(string columnId, double value)
        {
            int columnIndex = 0;
            if (!_columnIds.TryGetValue(columnId, out columnIndex))
            {
                lock (_columnIds)
                {
                    if (!_columnIds.TryGetValue(columnId, out columnIndex))
                    {
                        columnIndex = _columnIds.Count;
                        _columnIds[columnId] = columnIndex;
                    }
                }
            }
            _values[columnIndex] = value;
        }

        public double? GetValue(string columnId)
        {
            int columnIndex = 0;
            if (!_columnIds.TryGetValue(columnId, out columnIndex))
                return null;
            
            double value = 0;
            if (!_values.TryGetValue(columnIndex, out value))
                return null;

            return value;
        }

        public Field Clone(Data_info container)
        {
            var f = new Field(container);
            foreach (var key in _values.Keys)
            {
                f._values[key] = _values[key];
            }
            f.FieldNumber = FieldNumber;
            f.PTS = PTS;
            f.State = State;
            f.Invalid = Invalid;
            return f;
        }
    }

    public class BlackFieldGroup
    {
        public List<Field> Fields = new List<Field>();
        public Field CutField;
        public bool IsManualCutPoint;
    }

    public class Block
    {
        public int BlockNumber;

        public Field StartField;
        public Field EndField;

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

        public bool? IsCommercialOverride;
        public bool IsManualCutPoint;

        public double ScoreBrightness;
        public double ScoreLogo;
        public double ScoreUniform;
        public double ScoreLogSound;

        public int StartFieldNumber { get { return StartField.FieldNumber; } }

        public override string ToString()
        {
            return ((StartField == null) ? "null" : StartField.FieldNumber.ToString())
                + " - "
                + ((EndField == null) ? "null" : EndField.FieldNumber.ToString())
                + " : " + Length.ToString()
                + " : " + Score.ToString();
        }
    }

    public class HistogramData
    {
        public Dictionary<double, int> Data = new Dictionary<double, int>();
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

        public void Add(double value)
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

}
