#region using
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;
using Un4seen;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Enc;
using MathNet.Filtering;
//using NAudio.Wave;
//using Exocortex.DSP;
using Exocortex;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using DissertationMarkPapas;
#endregion using

namespace testBassGood
{
    public partial class SpeechCorrection : Form
    {
        #region FormInit/Close
        
        public SpeechCorrection()
        {
            InitializeComponent();
            importCsv();
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void SpeechCorrection_Load(object sender, EventArgs e)
        {
            Bass.BASS_Init(device, sampleRateInt, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_MONO, IntPtr.Zero);

            playButton.Enabled = false;
            pauseButton.Enabled = false;
            stopButton.Enabled = false;
            playConvertedButton.Enabled = false;
            pauseConvertedButton.Enabled = false;
            stopConvertedButton.Enabled = false;
            convertButton.Enabled = false;
            BassFx.LoadMe();
            BassEnc.LoadMe();

            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
        }

        private void SpeechCorrection_FormClosing(object sender, FormClosingEventArgs e)
        {
            Bass.BASS_Stop();
            Bass.BASS_Free();
            if (File.Exists("converted.wav"))
                File.Delete("converted.wav");
        }



        #endregion FormInit/Close
        
        #region ButtonClicks

        private void convertButton_Click(object sender, EventArgs e)
        {
            labelConversion.Text = "Converting . . .";
            //Do conversion async
            Task taskA = Task.Factory.StartNew(() => convert()).ContinueWith(taskB => writeTempFile());

        }
        
        private void loadButton_Click(object sender, EventArgs e)
        {
            fileNameLabel.Text = "Loading . . .";
            openFile.Filter = "Wave File(*.wav) | *.wav";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                loadFile();

            }
            else
            {
                fileNameLabel.Text = "No file loaded";
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (originalHandle == -1)
            {
                MessageBox.Show("Open a file first!", "No file loaded");
                return;
            }

            if (decodeHandle != -1 && Bass.BASS_ChannelIsActive(decodeHandle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                MessageBox.Show("Converted is currently playing!", "Playback in progress");
                return;
            }

            Bass.BASS_ChannelPlay(originalHandle, false);

            pauseButton.Enabled = true;
            stopButton.Enabled = true;
            playButton.Enabled = false;
            timerOriginal.Interval = 1;
            timerOriginal.Start();

        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelPause(originalHandle);
            pauseButton.Enabled = false;
            playButton.Enabled = true;
            stopButton.Enabled = true;
            timerOriginal.Stop();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Bass.BASS_ChannelStop(originalHandle);
            Bass.BASS_ChannelSetPosition(originalHandle, 0);
            playButton.Enabled = true;
            pauseButton.Enabled = false;
            stopButton.Enabled = false;
            timerOriginal.Stop();

            long length = Bass.BASS_ChannelGetLength(originalHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(originalHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            timeOriginal.Text = "00:00/" + time;

            DrawWavePosition(0, length);
        }

        private void playConvertedButton_Click(object sender, EventArgs e)
        {

            if (Bass.BASS_ChannelIsActive(originalHandle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                MessageBox.Show("Original is currently playing", "Playback in progress");
                return;
            }

            Bass.BASS_ChannelPlay(decodeHandle, false);

            pauseConvertedButton.Enabled = true;
            stopConvertedButton.Enabled = true;
            playConvertedButton.Enabled = false;
            timerConverted.Interval = 1;
            timerConverted.Start();
        }

        private void pauseConvertedButton_Click(object sender, EventArgs e)
        {

            Bass.BASS_ChannelPause(decodeHandle);
            pauseConvertedButton.Enabled = false;
            playConvertedButton.Enabled = true;
            stopConvertedButton.Enabled = true;
            timerConverted.Stop();
        }

        private void stopConvertedButton_Click(object sender, EventArgs e)
        {

            Bass.BASS_ChannelStop(decodeHandle);
            Bass.BASS_ChannelSetPosition(decodeHandle, 0);
            playConvertedButton.Enabled = true;
            pauseConvertedButton.Enabled = false;
            stopConvertedButton.Enabled = false;
            timerConverted.Stop();

            long length = Bass.BASS_ChannelGetLength(decodeHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(decodeHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            timeConverted.Text = "00:00/" + time;

            DrawWavePositionConverted(0, length);
        }
        
        private void buttonSpectrumSpectrogram_Click(object sender, EventArgs e)
        {
            if (pictureBoxSpectrogram.Visible == true)
            {
                pictureBoxSpectrogram.Visible = false;
                pictureBoxSpectrum.Visible = true;
                buttonSpectrumSpectrogram.Text = "Change to Spectrogram";
            }
            else
            {
                pictureBoxSpectrogram.Visible = true;
                pictureBoxSpectrum.Visible = false;
                buttonSpectrumSpectrogram.Text = "Change to Spectrum";
            }

            if (pictureBoxSpectrogramConverted.Visible == true)
            {
                pictureBoxSpectrogramConverted.Visible = false;
                pictureBoxSpectrumCorrected.Visible = true;
                buttonSpectrumSpectrogram.Text = "Change to Spectrogram";
            }
            else
            {
                pictureBoxSpectrogramConverted.Visible = true;
                pictureBoxSpectrumCorrected.Visible = false;
                buttonSpectrumSpectrogram.Text = "Change to Spectrum";
            }

        }

        private void startRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recording recordingForm = new Recording();
            recordingForm.Show();
        }

        private void saveConvertedButton_Click(object sender, EventArgs e)  //CORRECT
        {
            saveConvertedDialog.Filter = "Wave file (*.wav)|*.wav";
            saveConvertedDialog.DefaultExt = "wav";
            saveConvertedDialog.AddExtension = true;
            DialogResult result = saveConvertedDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filename = saveConvertedDialog.FileName;
                File.Copy("converted.wav", filename, true);
            }
        }

        private void clusteringButton_Click(object sender, EventArgs e)
        {
            labelConversion.Text = "Clustering";

            Debug.WriteLine("CLUSTERINGGGGGGGGGGGGGGGGGG");
            //Task TaskA = Task.Factory.StartNew(() => clustering());

            Task TaskA = Task.Factory.StartNew(() => testClustering());
        }


        #endregion ButtonClicks

        #region Waveforms


        private void MyWaveFormCallback(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished)
        {
            if (finished)
            {
                long cuein = 0;
                long cueout = 0;
                waveFormOriginal.GetCuePoints(ref cuein, ref cueout, -25.0, -42.0, -1, -1);
                //waveFormOriginal.AddMarker("CUE", cuein);
                //waveFormOriginal.AddMarker("END", cueout);
            }
            DrawWave();
        }

        private void DrawSpectrogram()
        {
            long len = Bass.BASS_ChannelGetLength(originalHandle);
            double time = Bass.BASS_ChannelBytes2Seconds(originalHandle, len);
            int stepsPerSecond = 1000;

            int steps = (int)Math.Floor(stepsPerSecond * time);

            Bass.BASS_ChannelPlay(originalHandle, false);

            Bass.BASS_ChannelSetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);

            Bitmap img = new Bitmap(pictureBoxSpectrogram.Width, pictureBoxSpectrogram.Height);
            Graphics g = Graphics.FromImage(img);

            for (int i = 0; i < steps; i++)
            {
                Bass.BASS_ChannelSetPosition(originalHandle, 1.0 * i / stepsPerSecond);
                visSpectro.CreateSpectrum3DVoicePrint(originalHandle, g, pictureBoxSpectrogram.Bounds, Color.Cyan, Color.Green, i, false, true);

            }

            Bass.BASS_ChannelStop(originalHandle);
            Bass.BASS_ChannelSetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
            Bass.BASS_ChannelSetPosition(originalHandle, 0);

            pictureBoxSpectrogram.Image = img;

            this.position++;
            if (this.position >= pictureBoxSpectrogram.Width)
            {
                this.position = 0;
            }

        }

        private void DrawSpectrogramConverted()
        {
            long len = Bass.BASS_ChannelGetLength(decodeHandle);
            double time = Bass.BASS_ChannelBytes2Seconds(decodeHandle, len);
            int stepsPerSecond = 1000;

            int steps = (int)Math.Floor(stepsPerSecond * time);

            Bass.BASS_ChannelPlay(decodeHandle, false);

            Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, 0);

            Bitmap img = new Bitmap(pictureBoxSpectrogramConverted.Width, pictureBoxSpectrogramConverted.Height);
            Graphics g = Graphics.FromImage(img);

            for (int i = 0; i < steps; i++)
            {
                Bass.BASS_ChannelSetPosition(originalHandle, 1.0 * i / stepsPerSecond);
                visSpectroConverted.CreateSpectrum3DVoicePrint(decodeHandle, g, pictureBoxSpectrogramConverted.Bounds, Color.Cyan, Color.Green, i, false, true);

            }

            Bass.BASS_ChannelStop(decodeHandle);
            Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
            Bass.BASS_ChannelSetPosition(decodeHandle, 0);

            pictureBoxSpectrogramConverted.Image = img;

            this.position++;
            if (this.position >= pictureBoxSpectrogram.Width)
            {
                this.position = 0;
            }

        }

        private void DrawWave()
        {
            if (waveFormOriginal != null)
            {
                this.pictureBox1.BackgroundImage = waveFormOriginal.CreateBitmap(pictureBox1.Width, pictureBox1.Height, -1, -1, true);
            }
            else
            {
                this.pictureBox1.BackgroundImage = null;
            }
        }

        private void DrawWavePosition(long pos, long len)
        {
            if (len == 0 || pos < 0)
            {
                pictureBox1.Image = null;
                return;
            }

            Bitmap bitmap = null;
            Graphics g = null;
            Pen p = null;
            double bpp = 0;

            try
            {
                bpp = len / (double)pictureBox1.Width;

                p = new Pen(Color.Red);
                bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(bitmap);
                g.Clear(Color.Black);
                int x = (int)Math.Round(pos / bpp);
                g.DrawLine(p, x, 0, x, pictureBox1.Height - 1);
                bitmap.MakeTransparent(Color.Black);
            }
            catch
            {
                bitmap = null;
            }
            finally
            {
                if (p != null)
                    p.Dispose();
                if (g != null)
                    g.Dispose();
            }
            pictureBox1.Image = bitmap;
        }

        private void MyWaveFormCallbackConverted(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished)
        {
            if (finished)
            {
                long cuein = 0;
                long cueout = 0;
                waveFormConverted.GetCuePoints(ref cuein, ref cueout, -25.0, -42.0, -1, -1);
                //waveFormConverted.AddMarker("CUE", cuein);
                //waveFormConverted.AddMarker("END", cueout);
            }
            DrawWaveConverted();
        }

        private void DrawWaveConverted()
        {

            if (waveFormConverted != null)
            {
                this.pictureBox2.BackgroundImage = waveFormConverted.CreateBitmap(pictureBox1.Width, pictureBox1.Height, -1, -1, true);
            }
            else
            {
                this.pictureBox2.BackgroundImage = null;
            }
        }

        private void DrawWavePositionConverted(long pos, long len)
        {
            if (len == 0 || pos < 0)
            {
                pictureBox2.Image = null;
                return;
            }

            Bitmap bitmap = null;
            Graphics g = null;
            Pen p = null;
            double bpp = 0;

            try
            {
                bpp = len / (double)pictureBox2.Width;

                p = new Pen(Color.Red);
                bitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
                g = Graphics.FromImage(bitmap);
                g.Clear(Color.Black);
                int x = (int)Math.Round(pos / bpp);
                g.DrawLine(p, x, 0, x, pictureBox2.Height - 1);
                bitmap.MakeTransparent(Color.Black);
            }
            catch
            {
                bitmap = null;
            }
            finally
            {
                if (p != null)
                    p.Dispose();
                if (g != null)
                    g.Dispose();
            }
            pictureBox2.Image = bitmap;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) //WaveForm Original
        {
            if (waveFormOriginal == null)
                return;

            long pos = waveFormOriginal.GetBytePositionFromX(e.X, pictureBox1.Width, -1, -1);
            Bass.BASS_ChannelSetPosition(originalHandle, pos);
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e) //WaveForm Converted
        {
            if (waveFormConverted == null)
                return;

            long pos = waveFormConverted.GetBytePositionFromX(e.X, pictureBox2.Width, -1, -1);
            Bass.BASS_ChannelSetPosition(decodeHandle, pos);
        }

        private void DrawSpectrum()
        {
            switch (specIdx)
            {
                // normal spectrum (width = resolution)
                case 0:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrum(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Lime, Color.Red, Color.Black, false, false, false);
                    break;
                // normal spectrum (full resolution)
                case 1:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrum(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.SteelBlue, Color.Pink, Color.Black, false, true, true);
                    break;
                // line spectrum (width = resolution)
                case 2:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumLine(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Lime, Color.Red, Color.Black, 2, 2, false, false, false);
                    break;
                // line spectrum (full resolution)
                case 3:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumLine(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.SteelBlue, Color.Pink, Color.Black, 16, 4, false, true, true);
                    break;
                // ellipse spectrum (width = resolution)
                case 4:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumEllipse(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Lime, Color.Red, Color.Black, 1, 2, false, false, false);
                    break;
                // ellipse spectrum (full resolution)
                case 5:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumEllipse(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.SteelBlue, Color.Pink, Color.Black, 2, 4, false, true, true);
                    break;
                // dot spectrum (width = resolution)
                case 6:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumDot(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Lime, Color.Red, Color.Black, 1, 0, false, false, false);
                    break;
                // dot spectrum (full resolution)
                case 7:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumDot(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.SteelBlue, Color.Pink, Color.Black, 2, 1, false, false, true);
                    break;
                // peak spectrum (width = resolution)
                case 8:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumLinePeak(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.SeaGreen, Color.LightGreen, Color.Orange, Color.Black, 2, 1, 2, 10, false, false, false);
                    break;
                // peak spectrum (full resolution)
                case 9:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumLinePeak(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.GreenYellow, Color.RoyalBlue, Color.DarkOrange, Color.Black, 23, 5, 3, 5, false, true, true);
                    break;
                // wave spectrum (width = resolution)
                case 10:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumWave(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Yellow, Color.Orange, Color.Black, 1, false, false, false);
                    break;
                // dancing beans spectrum (width = resolution)
                case 11:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumBean(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Chocolate, Color.DarkGoldenrod, Color.Black, 4, false, false, true);
                    break;
                // dancing text spectrum (width = resolution)
                case 12:
                    this.pictureBoxSpectrum.Image = vis.CreateSpectrumText(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.White, Color.Tomato, Color.Black, "BASS .NET IS GREAT PIECE! UN4SEEN ROCKS...", false, false, true);
                    break;
                // frequency detection
                case 13:
                    float amp = vis.DetectFrequency(originalHandle, 10, 500, true);
                    if (amp > 0.3)
                        this.pictureBoxSpectrum.BackColor = Color.Red;
                    else
                        this.pictureBoxSpectrum.BackColor = Color.Black;
                    break;
                // 3D voice print
                case 14:
                    // we need to draw directly directly on the picture box...
                    // normally you would encapsulate this in your own custom control
                    Graphics g = Graphics.FromHwnd(this.pictureBoxSpectrum.Handle);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    vis.CreateSpectrum3DVoicePrint(originalHandle, g, new Rectangle(0, 0, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height), Color.Black, Color.White, voicePrintIdx, false, false);
                    g.Dispose();
                    // next call will be at the next pos
                    voicePrintIdx++;
                    if (voicePrintIdx > this.pictureBoxSpectrum.Width - 1)
                        voicePrintIdx = 0;
                    break;
                // WaveForm
                case 15:
                    this.pictureBoxSpectrum.Image = vis.CreateWaveForm(originalHandle, this.pictureBoxSpectrum.Width, this.pictureBoxSpectrum.Height, Color.Green, Color.Red, Color.Gray, Color.Black, 1, true, false, true);
                    break;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            specIdx = trackBar1.Value;
        }

        private void DrawSpectrumCorrected()
        {

            switch (specIdx)
            {
                // normal spectrum (width = resolution)
                case 0:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrum(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Lime, Color.Red, Color.Black, false, false, false);
                    break;
                // normal spectrum (full resolution)
                case 1:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrum(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.SteelBlue, Color.Pink, Color.Black, false, true, true);
                    break;
                // line spectrum (width = resolution)
                case 2:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumLine(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Lime, Color.Red, Color.Black, 2, 2, false, false, false);
                    break;
                // line spectrum (full resolution)
                case 3:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumLine(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.SteelBlue, Color.Pink, Color.Black, 16, 4, false, true, true);
                    break;
                // ellipse spectrum (width = resolution)
                case 4:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumEllipse(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Lime, Color.Red, Color.Black, 1, 2, false, false, false);
                    break;
                // ellipse spectrum (full resolution)
                case 5:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumEllipse(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.SteelBlue, Color.Pink, Color.Black, 2, 4, false, true, true);
                    break;
                // dot spectrum (width = resolution)
                case 6:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumDot(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Lime, Color.Red, Color.Black, 1, 0, false, false, false);
                    break;
                // dot spectrum (full resolution)
                case 7:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumDot(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.SteelBlue, Color.Pink, Color.Black, 2, 1, false, false, true);
                    break;
                // peak spectrum (width = resolution)
                case 8:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumLinePeak(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.SeaGreen, Color.LightGreen, Color.Orange, Color.Black, 2, 1, 2, 10, false, false, false);
                    break;
                // peak spectrum (full resolution)
                case 9:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumLinePeak(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.GreenYellow, Color.RoyalBlue, Color.DarkOrange, Color.Black, 23, 5, 3, 5, false, true, true);
                    break;
                // wave spectrum (width = resolution)
                case 10:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumWave(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Yellow, Color.Orange, Color.Black, 1, false, false, false);
                    break;
                // dancing beans spectrum (width = resolution)
                case 11:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumBean(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Chocolate, Color.DarkGoldenrod, Color.Black, 4, false, false, true);
                    break;
                // dancing text spectrum (width = resolution)
                case 12:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateSpectrumText(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.White, Color.Tomato, Color.Black, "BASS .NET IS GREAT PIECE! UN4SEEN ROCKS...", false, false, true);
                    break;
                // frequency detection
                case 13:
                    float amp = vis.DetectFrequency(decodeHandle, 10, 500, true);
                    if (amp > 0.3)
                        this.pictureBoxSpectrumCorrected.BackColor = Color.Red;
                    else
                        this.pictureBoxSpectrumCorrected.BackColor = Color.Black;
                    break;
                // 3D voice print
                case 14:
                    // we need to draw directly directly on the picture box...
                    // normally you would encapsulate this in your own custom control
                    Graphics g = Graphics.FromHwnd(this.pictureBoxSpectrumCorrected.Handle);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    vis.CreateSpectrum3DVoicePrint(decodeHandle, g, new Rectangle(0, 0, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height), Color.Black, Color.White, voicePrintIdx, false, false);
                    g.Dispose();
                    // next call will be at the next pos
                    voicePrintIdx++;
                    if (voicePrintIdx > this.pictureBoxSpectrumCorrected.Width - 1)
                        voicePrintIdx = 0;
                    break;
                // WaveForm
                case 15:
                    this.pictureBoxSpectrumCorrected.Image = vis.CreateWaveForm(decodeHandle, this.pictureBoxSpectrumCorrected.Width, this.pictureBoxSpectrumCorrected.Height, Color.Green, Color.Red, Color.Gray, Color.Black, 1, true, false, true);
                    break;
            }
        }

        private void getWaveform(long length)
        {
            waveFormOriginal = new WaveForm(fileName, new WAVEFORMPROC(MyWaveFormCallback), this);

            waveFormOriginal.FrameResolution = 0.01f;
            waveFormOriginal.CallbackFrequency = 2000;
            waveFormOriginal.ColorBackground = Color.WhiteSmoke;
            waveFormOriginal.ColorLeft = Color.Gainsboro;
            waveFormOriginal.ColorLeftEnvelope = Color.Gray;
            waveFormOriginal.ColorRight = Color.LightGray;
            waveFormOriginal.ColorRightEnvelope = Color.DimGray;
            waveFormOriginal.ColorMarker = Color.DarkBlue;
            waveFormOriginal.DrawWaveForm = WaveForm.WAVEFORMDRAWTYPE.Stereo;
            waveFormOriginal.DrawMarker = WaveForm.MARKERDRAWTYPE.Line | WaveForm.MARKERDRAWTYPE.Name | WaveForm.MARKERDRAWTYPE.NamePositionAlternate;
            waveFormOriginal.MarkerLength = 0.75f;

            waveFormOriginal.RenderStart(true, BASSFlag.BASS_DEFAULT);

            DrawWavePosition(0, length);
        }

        private void getWaveFormConverted(long length)
        {
            waveFormConverted = new WaveForm("files/converted.wav", new WAVEFORMPROC(MyWaveFormCallbackConverted), this);

            waveFormConverted.FrameResolution = 0.01f;
            waveFormConverted.CallbackFrequency = 2000;
            waveFormConverted.ColorBackground = Color.WhiteSmoke;
            waveFormConverted.ColorLeft = Color.Gainsboro;
            waveFormConverted.ColorLeftEnvelope = Color.Gray;
            waveFormConverted.ColorRight = Color.LightGray;
            waveFormConverted.ColorRightEnvelope = Color.DimGray;
            waveFormConverted.ColorMarker = Color.DarkBlue;
            waveFormConverted.DrawWaveForm = WaveForm.WAVEFORMDRAWTYPE.Stereo;
            waveFormConverted.DrawMarker = WaveForm.MARKERDRAWTYPE.Line | WaveForm.MARKERDRAWTYPE.Name | WaveForm.MARKERDRAWTYPE.NamePositionAlternate;
            waveFormConverted.MarkerLength = 0.75f;

            waveFormConverted.RenderStart(true, BASSFlag.BASS_DEFAULT);

            DrawWavePositionConverted(0, length);

            //Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
        }

        private void setVolumeConverted()
        {
            if (volumeBar.InvokeRequired)
            {
                volumeSetVoidDelegate d = new volumeSetVoidDelegate(setVolumeConverted);
                this.Invoke(d, null);
            }
            else
            {
                Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
            }
        }

        #endregion Waveforms
        
        #region Timers


        private void timerOriginal_Tick(object sender, EventArgs e)
        {

            if (Bass.BASS_ChannelIsActive(originalHandle) == BASSActive.BASS_ACTIVE_STOPPED)    //playback finished
            {
                Bass.BASS_ChannelSetPosition(originalHandle, 0);

                playButton.Enabled = true;

                pauseButton.Enabled = false;

                stopButton.Enabled = false;

                long lengthF = Bass.BASS_ChannelGetLength(originalHandle);

                double totalTimeF = Bass.BASS_ChannelBytes2Seconds(originalHandle, lengthF);

                TimeSpan tsF = TimeSpan.FromSeconds(totalTimeF);

                string timeF = tsF.ToString(@"mm\:ss");

                timeOriginal.Text = "00:00/" + timeF;

                DrawWavePosition(0, lengthF);

                timerOriginal.Stop();

                this.position = 0;

                return;

            }

            long length = Bass.BASS_ChannelGetLength(originalHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(originalHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            long position = Bass.BASS_ChannelGetPosition(originalHandle);

            double currentTime = Bass.BASS_ChannelBytes2Seconds(originalHandle, position);

            TimeSpan current = TimeSpan.FromSeconds(currentTime);

            string currentSt = current.ToString(@"mm\:ss");

            timeOriginal.Text = currentSt + "/" + time;

            DrawWavePosition(position, length);

            DrawSpectrum();
        }

        private void timerConverted_Tick(object sender, EventArgs e)
        {
            if (Bass.BASS_ChannelIsActive(decodeHandle) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                Bass.BASS_ChannelSetPosition(decodeHandle, 0);

                playConvertedButton.Enabled = true;

                pauseConvertedButton.Enabled = false;

                stopConvertedButton.Enabled = false;

                long lengthF = Bass.BASS_ChannelGetLength(decodeHandle);

                double totalTimeF = Bass.BASS_ChannelBytes2Seconds(decodeHandle, lengthF);

                TimeSpan tsF = TimeSpan.FromSeconds(totalTimeF);

                string timeF = tsF.ToString(@"mm\:ss");

                timeConverted.Text = "00:00/" + timeF;

                DrawWavePositionConverted(0, lengthF);

                timerConverted.Stop();

                return;

            }

            long length = Bass.BASS_ChannelGetLength(decodeHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(decodeHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            long position = Bass.BASS_ChannelGetPosition(decodeHandle);

            double currentTime = Bass.BASS_ChannelBytes2Seconds(decodeHandle, position);

            TimeSpan current = TimeSpan.FromSeconds(currentTime);

            string currentSt = current.ToString(@"mm\:ss");

            timeConverted.Text = currentSt + "/" + time;

            DrawWavePositionConverted(position, length);

            DrawSpectrumCorrected();
        }


        #endregion Timers

        #region Imports/Writes


        private void loadFile()
        {
            fileName = openFile.FileName;

            originalHandle = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_SAMPLE_MONO);

            int[] chans = new int[2];
            Bass.BASS_SampleGetChannels(originalHandle, chans);

            originalChannel = Bass.BASS_SampleGetChannel(originalHandle, false);

            playButton.Enabled = true;

            Bass.BASS_ChannelGetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_FREQ, ref sampleRate);

            long length = Bass.BASS_ChannelGetLength(originalHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(originalHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            timeOriginal.Text = "00:00/" + time;
            convertButton.Enabled = true;

            getWaveform(length);

            //DrawSpectrogram();

            Bass.BASS_ChannelSetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);

            fileNameLabel.Text = Path.GetFileName(fileName);
        }

        private void loadCorrected()            //CORRECT
        {
            Bass.BASS_StreamFree(decodeHandle);

            decodeHandle = Bass.BASS_StreamCreateFile("files/converted.wav", 0, 0, 0);

            decodeChannel = Bass.BASS_SampleGetChannel(decodeHandle, false);

            long length = Bass.BASS_ChannelGetLength(decodeHandle);


            double totalTime = Bass.BASS_ChannelBytes2Seconds(decodeHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            //timeConverted.Text = "00:00/" + time;

            //playConvertedButton.Enabled = true;

            //saveConvertedButton.Enabled = true;

            getWaveFormConverted(length);

            //DrawSpectrogramConverted();

            setButtons("00:00/" + time);

            System.Media.SystemSounds.Hand.Play();
        }


        private void importCsv()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    hcFreqs[i, j] = new List<float>();
                    pdFreqs[i, j] = new List<float>();

                    hcMeanFreqs[i, j] = 0;
                    pdMeanFreqs[i, j] = 0;
                }
            }


            var path = "files/allFreqs.csv";

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    int cat = int.Parse(values[0]);

                    int vow = int.Parse(values[1]);

                    float f1 = (float)double.Parse(values[4]);
                    float f2 = (float)double.Parse(values[5]);

                    if (cat == 0)
                    {

                        hcFreqs[vow, 0].Add(f1);
                        hcFreqs[vow, 1].Add(f2);

                        hcMeanFreqs[vow, 0] += f1;
                        hcMeanFreqs[vow, 1] += f2;

                    }
                    else
                    {

                        pdFreqs[vow, 0].Add(f1);
                        pdFreqs[vow, 1].Add(f2);

                        pdMeanFreqs[vow, 0] += f1;
                        pdMeanFreqs[vow, 1] += f2;

                    }
                }
            }
            Debug.WriteLine($"hcFreqs.Length = {hcFreqs[1, 0].Count}");

            for (int i = 0; i < 5; i++)
            {

                hcMeanFreqs[i, 0] /= hcFreqs[i, 0].Count;
                hcMeanFreqs[i, 1] /= hcFreqs[i, 1].Count;

                pdMeanFreqs[i, 0] /= pdFreqs[i, 0].Count;
                pdMeanFreqs[i, 1] /= pdFreqs[i, 1].Count;

                Debug.WriteLine($"hcMeanFreqs[{i},0] = {hcMeanFreqs[i, 0]} \t hcMeanFreqs[{i},1] = {hcMeanFreqs[i, 1]}");

                Debug.WriteLine($"pdMeanFreqs[{i},0] = {pdMeanFreqs[i, 0]} \t pdMeanFreqs[{i},1] = {pdMeanFreqs[i, 1]}");


            }
        }

        private void writeTempFile()            //CORRECT
        {
            EncoderWAV enc = new EncoderWAV(pushHandle);
            enc.WAV_BitsPerSample = 16;
            enc.OutputFile = "files/converted.wav";
            enc.InputFile = null;
            enc.Start(null, IntPtr.Zero, false);

            byte[] buffer = new byte[65536];
            while (Bass.BASS_ChannelIsActive(pushHandle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                //getting sample data automatically feeds the encoder
                int len = Bass.BASS_ChannelGetData(pushHandle, buffer, buffer.Length);

                if (len == 0) break;

            }

            enc.Stop();

            loadCorrected();
        }


        #endregion Imports/Writes
        
        #region Other & Other UI Interraction


        private void setButtons(string text)
        {
            if (timeConverted.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(setButtons);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                timeConverted.Text = text;

                playConvertedButton.Enabled = true;

                saveConvertedButton.Enabled = true;

                labelConversion.Text = "Succesfully Converted!";
            }
        }

        private void volumeBar_ValueChanged(object sender, EventArgs e)
        {
            Bass.BASS_ChannelSetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
            volumeText.Text = volumeBar.Value.ToString();

            Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
        }

        private void volumeText_Leave(object sender, EventArgs e)
        {
            int volume;
            bool isInt = int.TryParse(volumeText.Text, out volume);
            if (volume < 0 || volume > 100)
            {
                volumeText.Text = volumeBar.Value.ToString();
            }
            else
            {
                volumeBar.Value = volume;
                Bass.BASS_ChannelSetAttribute(originalHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);

                Bass.BASS_ChannelSetAttribute(decodeHandle, BASSAttribute.BASS_ATTRIB_VOL, volumeBar.Value / 100f);
            }
        }

        private void volumeText_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        #endregion Other & Other UI Interraction


    }
}
