using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ComskipToCuttermaran
{
    public partial class SettingsThresholdUserControl : UserControl
    {
        public int Threshold { get; set; }
        public double SafetyPercent { get; set; }
        public int SafetyOffset { get; set; }
        public int DetectedThresholdPreSafety { get; set; }

        public string Title
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }
        public int MaxThreshold
        {
            get { return (int)numericUpDownManual.Maximum; }
            set { numericUpDownManual.Maximum = value; }
        }

        public bool _updating = false;

        public SettingsThresholdUserControl()
        {
            InitializeComponent();
        }

        private void SettingsThresholdUserControl_Load(object sender, EventArgs e)
        {
            _updating = true;
            if (Threshold >= 0)
            {
                radioButtonManual.Checked = true;
                numericUpDownManual.Value = Threshold;
            }
            else
            {
                radioButtonAutoDetect.Checked = true;
            }
            numericUpDownSafetyPercent.Value = (int)(SafetyPercent * 100);
            numericUpDownSafetyOffset.Value = SafetyOffset;
            UpdateDetectedThreshold();
            _updating = false;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating) return;

            if (radioButtonAutoDetect.Checked)
            {
                Threshold = -1;
                numericUpDownManual.Enabled = false;
            }
            else
            {
                Threshold = (int)numericUpDownManual.Value;
                numericUpDownManual.Enabled = true;
            }
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_updating) return;

            if (!radioButtonAutoDetect.Checked)
            {
                Threshold = (int)numericUpDownManual.Value;
            }
            SafetyPercent = (double)numericUpDownSafetyPercent.Value / 100.0;
            SafetyOffset = (int)numericUpDownSafetyOffset.Value;
            UpdateDetectedThreshold();
        }

        private void UpdateDetectedThreshold()
        {
            var threshold = DetectedThresholdPreSafety;
            threshold = (int)(threshold * (1.0 + SafetyPercent));
            threshold += SafetyOffset;
            textBoxValue.Text = threshold.ToString();
        }

    }
}
