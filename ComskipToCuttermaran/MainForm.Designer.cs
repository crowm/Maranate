namespace ComskipToCuttermaran
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.buttonSeekToStart = new CSharpControls.VistaButton();
            this.buttonPlay = new CSharpControls.VistaButton();
            this.objectListViewBlocks = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnBlockNumber = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnFrameNumber = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnScore = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAverageBright = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1AverageUniform = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAverageSceneChange = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAverageSound = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAverageLogo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnStrictLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.labelPreviewFrame = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.buttonSeekToEnd = new CSharpControls.VistaButton();
            this.buttonSeekToNextKeyFrame = new CSharpControls.VistaButton();
            this.buttonSeekToPreviousKeyFrame = new CSharpControls.VistaButton();
            this.buttonSeekToNextFrame = new CSharpControls.VistaButton();
            this.buttonSeekBack60s = new CSharpControls.VistaButton();
            this.buttonSeekToPreviousFrame = new CSharpControls.VistaButton();
            this.buttonSeekForward60s = new CSharpControls.VistaButton();
            this.buttonSeekForward10s = new CSharpControls.VistaButton();
            this.buttonSave = new CSharpControls.VistaButton();
            this.buttonToggleIsCommercial = new CSharpControls.VistaButton();
            this.buttonInsertCutPoint = new CSharpControls.VistaButton();
            this.buttonSeekBack10s = new CSharpControls.VistaButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.timelineUserControl1 = new ComskipToCuttermaran.TimelineUserControl();
            this.labelLoading = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageAnalysis = new System.Windows.Forms.TabPage();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.buttonReprocess = new CSharpControls.VistaButton();
            this.settingsThresholdUserControlSound = new ComskipToCuttermaran.SettingsThresholdUserControl();
            this.settingsThresholdUserControlUniform = new ComskipToCuttermaran.SettingsThresholdUserControl();
            this.settingsThresholdUserControlBrightness = new ComskipToCuttermaran.SettingsThresholdUserControl();
            this.tabPageMessages = new System.Windows.Forms.TabPage();
            this.richTextBoxMessages = new System.Windows.Forms.RichTextBox();
            this.timerLoading = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewBlocks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageAnalysis.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.tabPageMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPreview.BackColor = System.Drawing.Color.Black;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPreview.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(401, 321);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // buttonSeekToStart
            // 
            this.buttonSeekToStart.AllowDefaultButtonBorder = true;
            this.buttonSeekToStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToStart.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToStart.ButtonText = "|<";
            this.buttonSeekToStart.CornerRadius = 2;
            this.buttonSeekToStart.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToStart.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToStart.Location = new System.Drawing.Point(334, 2);
            this.buttonSeekToStart.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSeekToStart.Name = "buttonSeekToStart";
            this.buttonSeekToStart.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToStart.TabIndex = 2;
            this.toolTip.SetToolTip(this.buttonSeekToStart, "Move to start of file");
            this.buttonSeekToStart.Click += new System.EventHandler(this.buttonSeekToStart_Click);
            this.buttonSeekToStart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonPlay
            // 
            this.buttonPlay.AllowDefaultButtonBorder = true;
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.BackColor = System.Drawing.Color.Transparent;
            this.buttonPlay.ButtonText = "Play";
            this.buttonPlay.CornerRadius = 2;
            this.buttonPlay.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonPlay.Location = new System.Drawing.Point(363, 297);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(42, 23);
            this.buttonPlay.TabIndex = 12;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // objectListViewBlocks
            // 
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnBlockNumber);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnFrameNumber);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnLength);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnScore);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnAverageBright);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumn1AverageUniform);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnAverageSceneChange);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnAverageSound);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnAverageLogo);
            this.objectListViewBlocks.AllColumns.Add(this.olvColumnStrictLength);
            this.objectListViewBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListViewBlocks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBlockNumber,
            this.olvColumnFrameNumber,
            this.olvColumnLength,
            this.olvColumnScore,
            this.olvColumnAverageBright,
            this.olvColumn1AverageUniform,
            this.olvColumnAverageSceneChange,
            this.olvColumnAverageSound,
            this.olvColumnAverageLogo,
            this.olvColumnStrictLength});
            this.objectListViewBlocks.FullRowSelect = true;
            this.objectListViewBlocks.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.objectListViewBlocks.HideSelection = false;
            this.objectListViewBlocks.Location = new System.Drawing.Point(0, 0);
            this.objectListViewBlocks.MultiSelect = false;
            this.objectListViewBlocks.Name = "objectListViewBlocks";
            this.objectListViewBlocks.ShowGroups = false;
            this.objectListViewBlocks.Size = new System.Drawing.Size(329, 303);
            this.objectListViewBlocks.TabIndex = 0;
            this.objectListViewBlocks.UseCompatibleStateImageBehavior = false;
            this.objectListViewBlocks.View = System.Windows.Forms.View.Details;
            this.objectListViewBlocks.SelectionChanged += new System.EventHandler(this.objectListViewBlocks_SelectionChanged);
            // 
            // olvColumnBlockNumber
            // 
            this.olvColumnBlockNumber.AspectName = "BlockNumber";
            this.olvColumnBlockNumber.CellPadding = null;
            this.olvColumnBlockNumber.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnBlockNumber.Text = "#";
            this.olvColumnBlockNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnBlockNumber.Width = 30;
            // 
            // olvColumnFrameNumber
            // 
            this.olvColumnFrameNumber.AspectName = "StartFrameNumber";
            this.olvColumnFrameNumber.CellPadding = null;
            this.olvColumnFrameNumber.Text = "Frame";
            this.olvColumnFrameNumber.Width = 50;
            // 
            // olvColumnLength
            // 
            this.olvColumnLength.AspectName = "Length";
            this.olvColumnLength.AspectToStringFormat = "{0:0.000}";
            this.olvColumnLength.CellPadding = null;
            this.olvColumnLength.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnLength.Text = "Length";
            this.olvColumnLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // olvColumnScore
            // 
            this.olvColumnScore.AspectName = "Score";
            this.olvColumnScore.AspectToStringFormat = "{0:0.000}";
            this.olvColumnScore.CellPadding = null;
            this.olvColumnScore.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnScore.Text = "Score";
            this.olvColumnScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // olvColumnAverageBright
            // 
            this.olvColumnAverageBright.AspectName = "AverageBrightness";
            this.olvColumnAverageBright.AspectToStringFormat = "{0:0.00}";
            this.olvColumnAverageBright.CellPadding = null;
            this.olvColumnAverageBright.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageBright.Text = "Brightness";
            this.olvColumnAverageBright.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // olvColumn1AverageUniform
            // 
            this.olvColumn1AverageUniform.AspectName = "AverageUniform";
            this.olvColumn1AverageUniform.AspectToStringFormat = "{0:0.00}";
            this.olvColumn1AverageUniform.CellPadding = null;
            this.olvColumn1AverageUniform.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn1AverageUniform.Text = "Uniform";
            this.olvColumn1AverageUniform.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // olvColumnAverageSceneChange
            // 
            this.olvColumnAverageSceneChange.AspectName = "AverageSceneChange";
            this.olvColumnAverageSceneChange.AspectToStringFormat = "{0:0.00}";
            this.olvColumnAverageSceneChange.CellPadding = null;
            this.olvColumnAverageSceneChange.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageSceneChange.Text = "S/C";
            this.olvColumnAverageSceneChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageSceneChange.Width = 35;
            // 
            // olvColumnAverageSound
            // 
            this.olvColumnAverageSound.AspectName = "AverageLogSound";
            this.olvColumnAverageSound.AspectToStringFormat = "{0:0.000}";
            this.olvColumnAverageSound.CellPadding = null;
            this.olvColumnAverageSound.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageSound.Text = "Sound";
            this.olvColumnAverageSound.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageSound.Width = 50;
            // 
            // olvColumnAverageLogo
            // 
            this.olvColumnAverageLogo.AspectName = "AverageLogo";
            this.olvColumnAverageLogo.AspectToStringFormat = "{0:0.000}";
            this.olvColumnAverageLogo.CellPadding = null;
            this.olvColumnAverageLogo.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageLogo.Text = "Logo";
            this.olvColumnAverageLogo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumnAverageLogo.Width = 50;
            // 
            // olvColumnStrictLength
            // 
            this.olvColumnStrictLength.AspectName = "StrictLength";
            this.olvColumnStrictLength.CellPadding = null;
            this.olvColumnStrictLength.Text = "Strict";
            // 
            // labelPreviewFrame
            // 
            this.labelPreviewFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelPreviewFrame.AutoSize = true;
            this.labelPreviewFrame.Location = new System.Drawing.Point(4, 306);
            this.labelPreviewFrame.Name = "labelPreviewFrame";
            this.labelPreviewFrame.Size = new System.Drawing.Size(38, 13);
            this.labelPreviewFrame.TabIndex = 1;
            this.labelPreviewFrame.Text = "Frame";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.timelineUserControl1);
            this.splitContainer3.Size = new System.Drawing.Size(821, 628);
            this.splitContainer3.SplitterDistance = 325;
            this.splitContainer3.TabIndex = 5;
            // 
            // splitContainer4
            // 
            this.splitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.objectListViewBlocks);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToEnd);
            this.splitContainer4.Panel1.Controls.Add(this.labelPreviewFrame);
            this.splitContainer4.Panel1.Controls.Add(this.buttonPlay);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToStart);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToNextKeyFrame);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToPreviousKeyFrame);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToNextFrame);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekBack60s);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekToPreviousFrame);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekForward60s);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekForward10s);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSave);
            this.splitContainer4.Panel1.Controls.Add(this.buttonToggleIsCommercial);
            this.splitContainer4.Panel1.Controls.Add(this.buttonInsertCutPoint);
            this.splitContainer4.Panel1.Controls.Add(this.buttonSeekBack10s);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.panel2);
            this.splitContainer4.Size = new System.Drawing.Size(821, 325);
            this.splitContainer4.SplitterDistance = 412;
            this.splitContainer4.TabIndex = 0;
            // 
            // buttonSeekToEnd
            // 
            this.buttonSeekToEnd.AllowDefaultButtonBorder = true;
            this.buttonSeekToEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToEnd.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToEnd.ButtonText = ">|";
            this.buttonSeekToEnd.CornerRadius = 2;
            this.buttonSeekToEnd.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToEnd.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToEnd.Location = new System.Drawing.Point(370, 2);
            this.buttonSeekToEnd.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.buttonSeekToEnd.Name = "buttonSeekToEnd";
            this.buttonSeekToEnd.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToEnd.TabIndex = 11;
            this.buttonSeekToEnd.Click += new System.EventHandler(this.buttonSeekToEnd_Click);
            this.buttonSeekToEnd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekToNextKeyFrame
            // 
            this.buttonSeekToNextKeyFrame.AllowDefaultButtonBorder = true;
            this.buttonSeekToNextKeyFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToNextKeyFrame.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToNextKeyFrame.ButtonText = ">>";
            this.buttonSeekToNextKeyFrame.CornerRadius = 2;
            this.buttonSeekToNextKeyFrame.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToNextKeyFrame.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToNextKeyFrame.Location = new System.Drawing.Point(370, 83);
            this.buttonSeekToNextKeyFrame.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.buttonSeekToNextKeyFrame.Name = "buttonSeekToNextKeyFrame";
            this.buttonSeekToNextKeyFrame.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToNextKeyFrame.TabIndex = 8;
            this.toolTip.SetToolTip(this.buttonSeekToNextKeyFrame, "Move to the next key frame (Shift+Right)");
            this.buttonSeekToNextKeyFrame.Click += new System.EventHandler(this.buttonSeekToNextKeyFrame_Click);
            this.buttonSeekToNextKeyFrame.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekToPreviousKeyFrame
            // 
            this.buttonSeekToPreviousKeyFrame.AllowDefaultButtonBorder = true;
            this.buttonSeekToPreviousKeyFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToPreviousKeyFrame.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToPreviousKeyFrame.ButtonText = "<<";
            this.buttonSeekToPreviousKeyFrame.CornerRadius = 2;
            this.buttonSeekToPreviousKeyFrame.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToPreviousKeyFrame.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToPreviousKeyFrame.Location = new System.Drawing.Point(334, 83);
            this.buttonSeekToPreviousKeyFrame.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSeekToPreviousKeyFrame.Name = "buttonSeekToPreviousKeyFrame";
            this.buttonSeekToPreviousKeyFrame.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToPreviousKeyFrame.TabIndex = 5;
            this.toolTip.SetToolTip(this.buttonSeekToPreviousKeyFrame, "Move to the previous key frame (Shift+Left)");
            this.buttonSeekToPreviousKeyFrame.Click += new System.EventHandler(this.buttonSeekToPreviousKeyFrame_Click);
            this.buttonSeekToPreviousKeyFrame.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekToNextFrame
            // 
            this.buttonSeekToNextFrame.AllowDefaultButtonBorder = true;
            this.buttonSeekToNextFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToNextFrame.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToNextFrame.ButtonText = ">";
            this.buttonSeekToNextFrame.CornerRadius = 2;
            this.buttonSeekToNextFrame.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToNextFrame.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToNextFrame.Location = new System.Drawing.Point(370, 110);
            this.buttonSeekToNextFrame.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.buttonSeekToNextFrame.Name = "buttonSeekToNextFrame";
            this.buttonSeekToNextFrame.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToNextFrame.TabIndex = 7;
            this.toolTip.SetToolTip(this.buttonSeekToNextFrame, "Move to the next frame (Right)");
            this.buttonSeekToNextFrame.Click += new System.EventHandler(this.buttonSeekToNextFrame_Click);
            this.buttonSeekToNextFrame.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekBack60s
            // 
            this.buttonSeekBack60s.AllowDefaultButtonBorder = true;
            this.buttonSeekBack60s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekBack60s.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekBack60s.ButtonText = "-60s";
            this.buttonSeekBack60s.CornerRadius = 2;
            this.buttonSeekBack60s.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekBack60s.Location = new System.Drawing.Point(334, 29);
            this.buttonSeekBack60s.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSeekBack60s.Name = "buttonSeekBack60s";
            this.buttonSeekBack60s.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekBack60s.TabIndex = 3;
            this.toolTip.SetToolTip(this.buttonSeekBack60s, "Move backward 60 seconds (Ctrl+Shift+Left)");
            this.buttonSeekBack60s.Click += new System.EventHandler(this.buttonSeekBack60s_Click);
            this.buttonSeekBack60s.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekToPreviousFrame
            // 
            this.buttonSeekToPreviousFrame.AllowDefaultButtonBorder = true;
            this.buttonSeekToPreviousFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekToPreviousFrame.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekToPreviousFrame.ButtonText = "<";
            this.buttonSeekToPreviousFrame.CornerRadius = 2;
            this.buttonSeekToPreviousFrame.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekToPreviousFrame.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSeekToPreviousFrame.Location = new System.Drawing.Point(334, 110);
            this.buttonSeekToPreviousFrame.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSeekToPreviousFrame.Name = "buttonSeekToPreviousFrame";
            this.buttonSeekToPreviousFrame.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekToPreviousFrame.TabIndex = 6;
            this.toolTip.SetToolTip(this.buttonSeekToPreviousFrame, "Move to the previous frame (Left)");
            this.buttonSeekToPreviousFrame.Click += new System.EventHandler(this.buttonSeekToPreviousFrame_Click);
            this.buttonSeekToPreviousFrame.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekForward60s
            // 
            this.buttonSeekForward60s.AllowDefaultButtonBorder = true;
            this.buttonSeekForward60s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekForward60s.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekForward60s.ButtonText = "+60s";
            this.buttonSeekForward60s.CornerRadius = 2;
            this.buttonSeekForward60s.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekForward60s.Location = new System.Drawing.Point(370, 29);
            this.buttonSeekForward60s.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.buttonSeekForward60s.Name = "buttonSeekForward60s";
            this.buttonSeekForward60s.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekForward60s.TabIndex = 10;
            this.toolTip.SetToolTip(this.buttonSeekForward60s, "Move forward 60 seconds (Ctrl+Shift+Right)");
            this.buttonSeekForward60s.Click += new System.EventHandler(this.buttonSeekForward60s_Click);
            this.buttonSeekForward60s.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekForward10s
            // 
            this.buttonSeekForward10s.AllowDefaultButtonBorder = true;
            this.buttonSeekForward10s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekForward10s.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekForward10s.ButtonText = "+10s";
            this.buttonSeekForward10s.CornerRadius = 2;
            this.buttonSeekForward10s.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekForward10s.Location = new System.Drawing.Point(369, 56);
            this.buttonSeekForward10s.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.buttonSeekForward10s.Name = "buttonSeekForward10s";
            this.buttonSeekForward10s.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekForward10s.TabIndex = 9;
            this.toolTip.SetToolTip(this.buttonSeekForward10s, "Move forward 10 seconds (Ctrl+Right)");
            this.buttonSeekForward10s.Click += new System.EventHandler(this.buttonSeekForward10s_Click);
            this.buttonSeekForward10s.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSave
            // 
            this.buttonSave.AllowDefaultButtonBorder = true;
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.BackColor = System.Drawing.Color.Transparent;
            this.buttonSave.ButtonColor = System.Drawing.Color.Maroon;
            this.buttonSave.ButtonText = "Save";
            this.buttonSave.CornerRadius = 2;
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSave.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(334, 236);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(71, 35);
            this.buttonSave.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonSave, "Save the Cuttermaran file");
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            this.buttonSave.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonToggleIsCommercial
            // 
            this.buttonToggleIsCommercial.AllowDefaultButtonBorder = true;
            this.buttonToggleIsCommercial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonToggleIsCommercial.BackColor = System.Drawing.Color.Transparent;
            this.buttonToggleIsCommercial.ButtonText = "Toggle\nCommercial";
            this.buttonToggleIsCommercial.CornerRadius = 2;
            this.buttonToggleIsCommercial.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonToggleIsCommercial.Location = new System.Drawing.Point(334, 187);
            this.buttonToggleIsCommercial.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonToggleIsCommercial.Name = "buttonToggleIsCommercial";
            this.buttonToggleIsCommercial.Size = new System.Drawing.Size(71, 35);
            this.buttonToggleIsCommercial.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonToggleIsCommercial, "Toggle the commercial state of the current block");
            this.buttonToggleIsCommercial.Click += new System.EventHandler(this.buttonToggleIsCommercial_Click);
            this.buttonToggleIsCommercial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonInsertCutPoint
            // 
            this.buttonInsertCutPoint.AllowDefaultButtonBorder = true;
            this.buttonInsertCutPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInsertCutPoint.BackColor = System.Drawing.Color.Transparent;
            this.buttonInsertCutPoint.ButtonText = "Insert\ncut point";
            this.buttonInsertCutPoint.CornerRadius = 2;
            this.buttonInsertCutPoint.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonInsertCutPoint.Location = new System.Drawing.Point(334, 148);
            this.buttonInsertCutPoint.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonInsertCutPoint.Name = "buttonInsertCutPoint";
            this.buttonInsertCutPoint.Size = new System.Drawing.Size(71, 35);
            this.buttonInsertCutPoint.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonInsertCutPoint, "Insert a cut point at the current position");
            this.buttonInsertCutPoint.Click += new System.EventHandler(this.buttonInsertCutPoint_Click);
            this.buttonInsertCutPoint.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // buttonSeekBack10s
            // 
            this.buttonSeekBack10s.AllowDefaultButtonBorder = true;
            this.buttonSeekBack10s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekBack10s.BackColor = System.Drawing.Color.Transparent;
            this.buttonSeekBack10s.ButtonText = "-10s";
            this.buttonSeekBack10s.CornerRadius = 2;
            this.buttonSeekBack10s.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSeekBack10s.Location = new System.Drawing.Point(334, 56);
            this.buttonSeekBack10s.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.buttonSeekBack10s.Name = "buttonSeekBack10s";
            this.buttonSeekBack10s.Size = new System.Drawing.Size(36, 23);
            this.buttonSeekBack10s.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonSeekBack10s, "Move backward 10 seconds (Ctrl+Left)");
            this.buttonSeekBack10s.Click += new System.EventHandler(this.buttonSeekBack10s_Click);
            this.buttonSeekBack10s.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.pictureBoxPreview);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(401, 321);
            this.panel2.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(324, 297);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "Test Speed";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // timelineUserControl1
            // 
            this.timelineUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timelineUserControl1.BackColor = System.Drawing.Color.Black;
            this.timelineUserControl1.CsvProcessor = null;
            this.timelineUserControl1.FrameNumber = -1;
            this.timelineUserControl1.Location = new System.Drawing.Point(0, 0);
            this.timelineUserControl1.Name = "timelineUserControl1";
            this.timelineUserControl1.SecondsVisible = 10D;
            this.timelineUserControl1.Size = new System.Drawing.Size(817, 297);
            this.timelineUserControl1.TabIndex = 0;
            this.timelineUserControl1.VideoMediaFile = null;
            this.timelineUserControl1.SelectedFrameChanged += new ComskipToCuttermaran.TimelineUserControl.SelectedFrameChangedCallback(this.timelineUserControl1_SelectedFrameChanged);
            // 
            // labelLoading
            // 
            this.labelLoading.AutoSize = true;
            this.labelLoading.BackColor = System.Drawing.Color.Transparent;
            this.labelLoading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLoading.ForeColor = System.Drawing.Color.Black;
            this.labelLoading.Location = new System.Drawing.Point(51, 105);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(69, 21);
            this.labelLoading.TabIndex = 2;
            this.labelLoading.Text = "Loading.";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(831, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 60000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // tabControl
            // 
            this.tabControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl.Controls.Add(this.tabPageAnalysis);
            this.tabControl.Controls.Add(this.tabPageSettings);
            this.tabControl.Controls.Add(this.tabPageMessages);
            this.tabControl.Location = new System.Drawing.Point(2, 38);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(829, 654);
            this.tabControl.TabIndex = 1;
            // 
            // tabPageAnalysis
            // 
            this.tabPageAnalysis.Controls.Add(this.splitContainer3);
            this.tabPageAnalysis.Location = new System.Drawing.Point(4, 4);
            this.tabPageAnalysis.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageAnalysis.Name = "tabPageAnalysis";
            this.tabPageAnalysis.Size = new System.Drawing.Size(821, 628);
            this.tabPageAnalysis.TabIndex = 0;
            this.tabPageAnalysis.Text = "Analysis";
            this.tabPageAnalysis.UseVisualStyleBackColor = true;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.groupBoxSettings);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 4);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(821, 628);
            this.tabPageSettings.TabIndex = 2;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSettings.Controls.Add(this.buttonReprocess);
            this.groupBoxSettings.Controls.Add(this.settingsThresholdUserControlSound);
            this.groupBoxSettings.Controls.Add(this.settingsThresholdUserControlUniform);
            this.groupBoxSettings.Controls.Add(this.settingsThresholdUserControlBrightness);
            this.groupBoxSettings.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(807, 208);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // buttonReprocess
            // 
            this.buttonReprocess.AllowDefaultButtonBorder = true;
            this.buttonReprocess.BackColor = System.Drawing.Color.Transparent;
            this.buttonReprocess.ButtonText = "Reprocess";
            this.buttonReprocess.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonReprocess.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReprocess.Location = new System.Drawing.Point(8, 169);
            this.buttonReprocess.Name = "buttonReprocess";
            this.buttonReprocess.Size = new System.Drawing.Size(100, 32);
            this.buttonReprocess.TabIndex = 2;
            this.buttonReprocess.Click += new System.EventHandler(this.buttonReprocess_Click);
            // 
            // settingsThresholdUserControlSound
            // 
            this.settingsThresholdUserControlSound.DetectedThresholdPreSafety = 0;
            this.settingsThresholdUserControlSound.Location = new System.Drawing.Point(336, 21);
            this.settingsThresholdUserControlSound.MaxThreshold = 100000;
            this.settingsThresholdUserControlSound.Name = "settingsThresholdUserControlSound";
            this.settingsThresholdUserControlSound.SafetyOffset = 0;
            this.settingsThresholdUserControlSound.SafetyPercent = 0D;
            this.settingsThresholdUserControlSound.Size = new System.Drawing.Size(159, 142);
            this.settingsThresholdUserControlSound.TabIndex = 1;
            this.settingsThresholdUserControlSound.Threshold = 0;
            this.settingsThresholdUserControlSound.Title = "Sound";
            // 
            // settingsThresholdUserControlUniform
            // 
            this.settingsThresholdUserControlUniform.DetectedThresholdPreSafety = 0;
            this.settingsThresholdUserControlUniform.Location = new System.Drawing.Point(171, 21);
            this.settingsThresholdUserControlUniform.MaxThreshold = 100000;
            this.settingsThresholdUserControlUniform.Name = "settingsThresholdUserControlUniform";
            this.settingsThresholdUserControlUniform.SafetyOffset = 0;
            this.settingsThresholdUserControlUniform.SafetyPercent = 0D;
            this.settingsThresholdUserControlUniform.Size = new System.Drawing.Size(159, 142);
            this.settingsThresholdUserControlUniform.TabIndex = 1;
            this.settingsThresholdUserControlUniform.Threshold = 0;
            this.settingsThresholdUserControlUniform.Title = "Uniform";
            // 
            // settingsThresholdUserControlBrightness
            // 
            this.settingsThresholdUserControlBrightness.DetectedThresholdPreSafety = 0;
            this.settingsThresholdUserControlBrightness.Location = new System.Drawing.Point(6, 21);
            this.settingsThresholdUserControlBrightness.MaxThreshold = 256;
            this.settingsThresholdUserControlBrightness.Name = "settingsThresholdUserControlBrightness";
            this.settingsThresholdUserControlBrightness.SafetyOffset = 0;
            this.settingsThresholdUserControlBrightness.SafetyPercent = 0D;
            this.settingsThresholdUserControlBrightness.Size = new System.Drawing.Size(159, 142);
            this.settingsThresholdUserControlBrightness.TabIndex = 0;
            this.settingsThresholdUserControlBrightness.Threshold = 0;
            this.settingsThresholdUserControlBrightness.Title = "Brightness";
            // 
            // tabPageMessages
            // 
            this.tabPageMessages.Controls.Add(this.richTextBoxMessages);
            this.tabPageMessages.Location = new System.Drawing.Point(4, 4);
            this.tabPageMessages.Name = "tabPageMessages";
            this.tabPageMessages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMessages.Size = new System.Drawing.Size(821, 628);
            this.tabPageMessages.TabIndex = 1;
            this.tabPageMessages.Text = "Messages";
            this.tabPageMessages.UseVisualStyleBackColor = true;
            // 
            // richTextBoxMessages
            // 
            this.richTextBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxMessages.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxMessages.Name = "richTextBoxMessages";
            this.richTextBoxMessages.Size = new System.Drawing.Size(818, 625);
            this.richTextBoxMessages.TabIndex = 0;
            this.richTextBoxMessages.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 694);
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainForm_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectListViewBlocks)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageAnalysis.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            this.tabPageMessages.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private CSharpControls.VistaButton buttonSeekToStart;
        private CSharpControls.VistaButton buttonPlay;
        private BrightIdeasSoftware.ObjectListView objectListViewBlocks;
        private System.Windows.Forms.Label labelPreviewFrame;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private TimelineUserControl timelineUserControl1;
        private System.Windows.Forms.Button button3;
        private CSharpControls.VistaButton buttonSeekToEnd;
        private CSharpControls.VistaButton buttonSeekToNextKeyFrame;
        private CSharpControls.VistaButton buttonSeekToNextFrame;
        private CSharpControls.VistaButton buttonSeekToPreviousFrame;
        private CSharpControls.VistaButton buttonSeekToPreviousKeyFrame;
        private CSharpControls.VistaButton buttonSeekForward10s;
        private CSharpControls.VistaButton buttonSeekBack10s;
        private CSharpControls.VistaButton buttonSeekForward60s;
        private CSharpControls.VistaButton buttonSeekBack60s;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageAnalysis;
        private System.Windows.Forms.TabPage tabPageMessages;
        private System.Windows.Forms.RichTextBox richTextBoxMessages;
        private BrightIdeasSoftware.OLVColumn olvColumnFrameNumber;
        private BrightIdeasSoftware.OLVColumn olvColumnLength;
        private BrightIdeasSoftware.OLVColumn olvColumnScore;
        private BrightIdeasSoftware.OLVColumn olvColumnAverageBright;
        private BrightIdeasSoftware.OLVColumn olvColumn1AverageUniform;
        private BrightIdeasSoftware.OLVColumn olvColumnAverageSceneChange;
        private BrightIdeasSoftware.OLVColumn olvColumnAverageSound;
        private BrightIdeasSoftware.OLVColumn olvColumnAverageLogo;
        private BrightIdeasSoftware.OLVColumn olvColumnStrictLength;
        private System.Windows.Forms.Timer timerLoading;
        private System.Windows.Forms.Label labelLoading;
        private BrightIdeasSoftware.OLVColumn olvColumnBlockNumber;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private SettingsThresholdUserControl settingsThresholdUserControlBrightness;
        private SettingsThresholdUserControl settingsThresholdUserControlSound;
        private SettingsThresholdUserControl settingsThresholdUserControlUniform;
        private CSharpControls.VistaButton buttonReprocess;
        private CSharpControls.VistaButton buttonInsertCutPoint;
        private CSharpControls.VistaButton buttonToggleIsCommercial;
        private CSharpControls.VistaButton buttonSave;
    }
}