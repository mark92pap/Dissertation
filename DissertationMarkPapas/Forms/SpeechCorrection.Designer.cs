namespace testBassGood
{
    partial class SpeechCorrection
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
            this.loadButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.pauseButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.timeOriginal = new System.Windows.Forms.Label();
            this.timerOriginal = new System.Windows.Forms.Timer(this.components);
            this.convertButton = new System.Windows.Forms.Button();
            this.playConvertedButton = new System.Windows.Forms.Button();
            this.pauseConvertedButton = new System.Windows.Forms.Button();
            this.stopConvertedButton = new System.Windows.Forms.Button();
            this.timerConverted = new System.Windows.Forms.Timer(this.components);
            this.timeConverted = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.volumeBar = new System.Windows.Forms.TrackBar();
            this.volumeText = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxSpectrum = new System.Windows.Forms.PictureBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxSpectrumCorrected = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConvertedButton = new System.Windows.Forms.Button();
            this.saveConvertedDialog = new System.Windows.Forms.SaveFileDialog();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.volumeLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxSpectrogram = new System.Windows.Forms.PictureBox();
            this.buttonSpectrumSpectrogram = new System.Windows.Forms.Button();
            this.pictureBoxSpectrogramConverted = new System.Windows.Forms.PictureBox();
            this.labelConversion = new System.Windows.Forms.Label();
            this.clusteringButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrumCorrected)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrogramConverted)).BeginInit();
            this.SuspendLayout();
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(19, 79);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "Open File";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playButton.Location = new System.Drawing.Point(1646, 98);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 1;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseButton.Location = new System.Drawing.Point(1646, 127);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 2;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stopButton.Location = new System.Drawing.Point(1646, 156);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // timeOriginal
            // 
            this.timeOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeOriginal.AutoSize = true;
            this.timeOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeOriginal.Location = new System.Drawing.Point(1529, 131);
            this.timeOriginal.Name = "timeOriginal";
            this.timeOriginal.Size = new System.Drawing.Size(93, 20);
            this.timeOriginal.TabIndex = 4;
            this.timeOriginal.Text = "00:00/00:00";
            // 
            // timerOriginal
            // 
            this.timerOriginal.Tick += new System.EventHandler(this.timerOriginal_Tick);
            // 
            // convertButton
            // 
            this.convertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.convertButton.Location = new System.Drawing.Point(35, 648);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(75, 23);
            this.convertButton.TabIndex = 5;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // playConvertedButton
            // 
            this.playConvertedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.playConvertedButton.Location = new System.Drawing.Point(1646, 619);
            this.playConvertedButton.Name = "playConvertedButton";
            this.playConvertedButton.Size = new System.Drawing.Size(75, 23);
            this.playConvertedButton.TabIndex = 6;
            this.playConvertedButton.Text = "Play";
            this.playConvertedButton.UseVisualStyleBackColor = true;
            this.playConvertedButton.Click += new System.EventHandler(this.playConvertedButton_Click);
            // 
            // pauseConvertedButton
            // 
            this.pauseConvertedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseConvertedButton.Location = new System.Drawing.Point(1646, 648);
            this.pauseConvertedButton.Name = "pauseConvertedButton";
            this.pauseConvertedButton.Size = new System.Drawing.Size(75, 23);
            this.pauseConvertedButton.TabIndex = 7;
            this.pauseConvertedButton.Text = "Pause";
            this.pauseConvertedButton.UseVisualStyleBackColor = true;
            this.pauseConvertedButton.Click += new System.EventHandler(this.pauseConvertedButton_Click);
            // 
            // stopConvertedButton
            // 
            this.stopConvertedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stopConvertedButton.Location = new System.Drawing.Point(1646, 680);
            this.stopConvertedButton.Name = "stopConvertedButton";
            this.stopConvertedButton.Size = new System.Drawing.Size(75, 23);
            this.stopConvertedButton.TabIndex = 8;
            this.stopConvertedButton.Text = "Stop";
            this.stopConvertedButton.UseVisualStyleBackColor = true;
            this.stopConvertedButton.Click += new System.EventHandler(this.stopConvertedButton_Click);
            // 
            // timerConverted
            // 
            this.timerConverted.Tick += new System.EventHandler(this.timerConverted_Tick);
            // 
            // timeConverted
            // 
            this.timeConverted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.timeConverted.AutoSize = true;
            this.timeConverted.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeConverted.Location = new System.Drawing.Point(1529, 648);
            this.timeConverted.Name = "timeConverted";
            this.timeConverted.Size = new System.Drawing.Size(93, 20);
            this.timeConverted.TabIndex = 9;
            this.timeConverted.Text = "00:00/00:00";
            // 
            // volumeBar
            // 
            this.volumeBar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.volumeBar.Location = new System.Drawing.Point(1582, 431);
            this.volumeBar.Maximum = 100;
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Size = new System.Drawing.Size(139, 45);
            this.volumeBar.TabIndex = 10;
            this.volumeBar.Value = 67;
            this.volumeBar.ValueChanged += new System.EventHandler(this.volumeBar_ValueChanged);
            // 
            // volumeText
            // 
            this.volumeText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.volumeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeText.Location = new System.Drawing.Point(1634, 492);
            this.volumeText.Name = "volumeText";
            this.volumeText.Size = new System.Drawing.Size(32, 26);
            this.volumeText.TabIndex = 11;
            this.volumeText.Text = "67";
            this.volumeText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.volumeText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.volumeText_KeyPress);
            this.volumeText.Leave += new System.EventHandler(this.volumeText_Leave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(138, 79);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1385, 166);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Location = new System.Drawing.Point(138, 589);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1385, 164);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            // 
            // pictureBoxSpectrum
            // 
            this.pictureBoxSpectrum.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBoxSpectrum.BackColor = System.Drawing.Color.Black;
            this.pictureBoxSpectrum.Location = new System.Drawing.Point(138, 251);
            this.pictureBoxSpectrum.Name = "pictureBoxSpectrum";
            this.pictureBoxSpectrum.Size = new System.Drawing.Size(1385, 225);
            this.pictureBoxSpectrum.TabIndex = 14;
            this.pictureBoxSpectrum.TabStop = false;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(35, 403);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 104);
            this.trackBar1.TabIndex = 15;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 358);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 42);
            this.label1.TabIndex = 16;
            this.label1.Text = "Choose Spectum Style";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxSpectrumCorrected
            // 
            this.pictureBoxSpectrumCorrected.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pictureBoxSpectrumCorrected.BackColor = System.Drawing.Color.Black;
            this.pictureBoxSpectrumCorrected.Location = new System.Drawing.Point(138, 779);
            this.pictureBoxSpectrumCorrected.Name = "pictureBoxSpectrumCorrected";
            this.pictureBoxSpectrumCorrected.Size = new System.Drawing.Size(1385, 254);
            this.pictureBoxSpectrumCorrected.TabIndex = 17;
            this.pictureBoxSpectrumCorrected.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1750, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startRecordingToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // startRecordingToolStripMenuItem
            // 
            this.startRecordingToolStripMenuItem.Name = "startRecordingToolStripMenuItem";
            this.startRecordingToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.startRecordingToolStripMenuItem.Text = "Start Recording";
            this.startRecordingToolStripMenuItem.Click += new System.EventHandler(this.startRecordingToolStripMenuItem_Click);
            // 
            // saveConvertedButton
            // 
            this.saveConvertedButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.saveConvertedButton.Enabled = false;
            this.saveConvertedButton.Location = new System.Drawing.Point(19, 700);
            this.saveConvertedButton.Name = "saveConvertedButton";
            this.saveConvertedButton.Size = new System.Drawing.Size(101, 23);
            this.saveConvertedButton.TabIndex = 19;
            this.saveConvertedButton.Text = "Save Converted";
            this.saveConvertedButton.UseVisualStyleBackColor = true;
            this.saveConvertedButton.Click += new System.EventHandler(this.saveConvertedButton_Click);
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileNameLabel.Location = new System.Drawing.Point(10, 180);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(100, 65);
            this.fileNameLabel.TabIndex = 20;
            // 
            // volumeLabel
            // 
            this.volumeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.volumeLabel.AutoSize = true;
            this.volumeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeLabel.Location = new System.Drawing.Point(1589, 399);
            this.volumeLabel.Name = "volumeLabel";
            this.volumeLabel.Size = new System.Drawing.Size(132, 20);
            this.volumeLabel.TabIndex = 21;
            this.volumeLabel.Text = "Volume Control";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(731, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 39);
            this.label2.TabIndex = 22;
            this.label2.Text = "ORIGINAL";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(709, 529);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 39);
            this.label3.TabIndex = 23;
            this.label3.Text = "CONVERTED";
            // 
            // pictureBoxSpectrogram
            // 
            this.pictureBoxSpectrogram.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBoxSpectrogram.Location = new System.Drawing.Point(138, 251);
            this.pictureBoxSpectrogram.Name = "pictureBoxSpectrogram";
            this.pictureBoxSpectrogram.Size = new System.Drawing.Size(1385, 225);
            this.pictureBoxSpectrogram.TabIndex = 24;
            this.pictureBoxSpectrogram.TabStop = false;
            this.pictureBoxSpectrogram.Visible = false;
            // 
            // buttonSpectrumSpectrogram
            // 
            this.buttonSpectrumSpectrogram.Location = new System.Drawing.Point(19, 298);
            this.buttonSpectrumSpectrogram.Name = "buttonSpectrumSpectrogram";
            this.buttonSpectrumSpectrogram.Size = new System.Drawing.Size(96, 40);
            this.buttonSpectrumSpectrogram.TabIndex = 25;
            this.buttonSpectrumSpectrogram.Text = "Change to Spectrogram";
            this.buttonSpectrumSpectrogram.UseVisualStyleBackColor = true;
            this.buttonSpectrumSpectrogram.Click += new System.EventHandler(this.buttonSpectrumSpectrogram_Click);
            // 
            // pictureBoxSpectrogramConverted
            // 
            this.pictureBoxSpectrogramConverted.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pictureBoxSpectrogramConverted.Location = new System.Drawing.Point(138, 779);
            this.pictureBoxSpectrogramConverted.Name = "pictureBoxSpectrogramConverted";
            this.pictureBoxSpectrogramConverted.Size = new System.Drawing.Size(1385, 254);
            this.pictureBoxSpectrogramConverted.TabIndex = 26;
            this.pictureBoxSpectrogramConverted.TabStop = false;
            this.pictureBoxSpectrogramConverted.Visible = false;
            // 
            // labelConversion
            // 
            this.labelConversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelConversion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConversion.Location = new System.Drawing.Point(15, 570);
            this.labelConversion.Name = "labelConversion";
            this.labelConversion.Size = new System.Drawing.Size(105, 72);
            this.labelConversion.TabIndex = 27;
            this.labelConversion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clusteringButton
            // 
            this.clusteringButton.Location = new System.Drawing.Point(1646, 545);
            this.clusteringButton.Name = "clusteringButton";
            this.clusteringButton.Size = new System.Drawing.Size(75, 23);
            this.clusteringButton.TabIndex = 28;
            this.clusteringButton.Text = "Clustering";
            this.clusteringButton.UseVisualStyleBackColor = true;
            this.clusteringButton.Click += new System.EventHandler(this.clusteringButton_Click);
            // 
            // SpeechCorrection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1750, 1045);
            this.Controls.Add(this.clusteringButton);
            this.Controls.Add(this.labelConversion);
            this.Controls.Add(this.pictureBoxSpectrogramConverted);
            this.Controls.Add(this.buttonSpectrumSpectrogram);
            this.Controls.Add(this.pictureBoxSpectrogram);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.volumeLabel);
            this.Controls.Add(this.fileNameLabel);
            this.Controls.Add(this.saveConvertedButton);
            this.Controls.Add(this.pictureBoxSpectrumCorrected);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.pictureBoxSpectrum);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.volumeText);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.timeConverted);
            this.Controls.Add(this.stopConvertedButton);
            this.Controls.Add(this.pauseConvertedButton);
            this.Controls.Add(this.playConvertedButton);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.timeOriginal);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SpeechCorrection";
            this.Text = "Speech Correction";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpeechCorrection_FormClosing);
            this.Load += new System.EventHandler(this.SpeechCorrection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrumCorrected)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSpectrogramConverted)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label timeOriginal;
        private System.Windows.Forms.Timer timerOriginal;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Button playConvertedButton;
        private System.Windows.Forms.Button pauseConvertedButton;
        private System.Windows.Forms.Button stopConvertedButton;
        private System.Windows.Forms.Timer timerConverted;
        private System.Windows.Forms.Label timeConverted;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TrackBar volumeBar;
        private System.Windows.Forms.TextBox volumeText;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBoxSpectrum;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxSpectrumCorrected;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startRecordingToolStripMenuItem;
        private System.Windows.Forms.Button saveConvertedButton;
        private System.Windows.Forms.SaveFileDialog saveConvertedDialog;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label volumeLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBoxSpectrogram;
        private System.Windows.Forms.Button buttonSpectrumSpectrogram;
        private System.Windows.Forms.PictureBox pictureBoxSpectrogramConverted;
        private System.Windows.Forms.Label labelConversion;
        private System.Windows.Forms.Button clusteringButton;
    }
}

