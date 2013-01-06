namespace Maranate
{
    partial class SettingsThresholdUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radioButtonAutoDetect = new System.Windows.Forms.RadioButton();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownManual = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSafetyPercent = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSafetyOffset = new System.Windows.Forms.NumericUpDown();
            this.labelTitle = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownManual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSafetyPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSafetyOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonAutoDetect
            // 
            this.radioButtonAutoDetect.AutoSize = true;
            this.radioButtonAutoDetect.Location = new System.Drawing.Point(8, 42);
            this.radioButtonAutoDetect.Name = "radioButtonAutoDetect";
            this.radioButtonAutoDetect.Size = new System.Drawing.Size(82, 17);
            this.radioButtonAutoDetect.TabIndex = 0;
            this.radioButtonAutoDetect.TabStop = true;
            this.radioButtonAutoDetect.Text = "Auto Detect";
            this.radioButtonAutoDetect.UseVisualStyleBackColor = true;
            this.radioButtonAutoDetect.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(8, 19);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(60, 17);
            this.radioButtonManual.TabIndex = 0;
            this.radioButtonManual.TabStop = true;
            this.radioButtonManual.Text = "Manual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            this.radioButtonManual.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Safety %";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Safety +/-";
            // 
            // numericUpDownManual
            // 
            this.numericUpDownManual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownManual.Location = new System.Drawing.Point(91, 19);
            this.numericUpDownManual.Name = "numericUpDownManual";
            this.numericUpDownManual.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownManual.TabIndex = 1;
            this.numericUpDownManual.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownSafetyPercent
            // 
            this.numericUpDownSafetyPercent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSafetyPercent.Location = new System.Drawing.Point(91, 64);
            this.numericUpDownSafetyPercent.Name = "numericUpDownSafetyPercent";
            this.numericUpDownSafetyPercent.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownSafetyPercent.TabIndex = 1;
            this.numericUpDownSafetyPercent.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownSafetyOffset
            // 
            this.numericUpDownSafetyOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSafetyOffset.Location = new System.Drawing.Point(91, 90);
            this.numericUpDownSafetyOffset.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownSafetyOffset.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownSafetyOffset.Name = "numericUpDownSafetyOffset";
            this.numericUpDownSafetyOffset.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownSafetyOffset.TabIndex = 1;
            this.numericUpDownSafetyOffset.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(5, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(27, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Title";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Value";
            // 
            // textBoxValue
            // 
            this.textBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxValue.Location = new System.Drawing.Point(91, 116);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.ReadOnly = true;
            this.textBoxValue.Size = new System.Drawing.Size(65, 20);
            this.textBoxValue.TabIndex = 2;
            // 
            // SettingsThresholdUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.numericUpDownSafetyOffset);
            this.Controls.Add(this.radioButtonAutoDetect);
            this.Controls.Add(this.numericUpDownSafetyPercent);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.radioButtonManual);
            this.Controls.Add(this.numericUpDownManual);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "SettingsThresholdUserControl";
            this.Size = new System.Drawing.Size(159, 139);
            this.Load += new System.EventHandler(this.SettingsThresholdUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownManual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSafetyPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSafetyOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonAutoDetect;
        private System.Windows.Forms.RadioButton radioButtonManual;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownManual;
        private System.Windows.Forms.NumericUpDown numericUpDownSafetyPercent;
        private System.Windows.Forms.NumericUpDown numericUpDownSafetyOffset;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxValue;
    }
}
