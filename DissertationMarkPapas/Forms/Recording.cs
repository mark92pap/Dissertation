#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;
#endregion Using

namespace testBassGood
{
    public partial class Recording : Form
    {
        #region Variables

        private RECORDPROC myRecProc;

        WaveWriter writer = null;

        int bytesWritten = 0;
        private byte[] recBuffer;
        int count = 1;
        bool stopped = true;

        int channel;
        int recHandle = 0;
        int playHandle = 0;

        bool first = true;

        #endregion Variables

        #region Load/Close

        public Recording()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Recording_Load(object sender, EventArgs e)
        {
            //Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, this.Handle);
            Bass.BASS_RecordInit(-1);
            myRecProc = new RECORDPROC(MyRecording);
        }

        private void Recording_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopped = false;
            if (writer!=null)
                writer.Dispose();
            Bass.BASS_StreamFree(playHandle);
            Bass.BASS_StreamFree(recHandle);
            if (File.Exists("temp.wav"))
                File.Delete("temp.wav");
        }

        #endregion Load/Close

        #region Recording

        private void button1_Click(object sender, EventArgs e)  //start
        {
            button3.Enabled = false;
            if (!first)
            {
                label1.Text = "00:00";
                count = 1;


                if (writer != null)
                    writer.Dispose();

                Bass.BASS_StreamFree(playHandle);
                Bass.BASS_StreamFree(recHandle);

                if (File.Exists("temp.wav"))
                    File.Delete("temp.wav");

                writer.Dispose();
                stopped = true;
            }
            label4.Text = "Wait!";
            button1.Enabled = false;
            button2.Enabled = true;
            recHandle = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_SAMPLE_FLOAT, myRecProc, IntPtr.Zero);
            if (recHandle == 0)
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString(), "Error");
            }
            writer = new WaveWriter("temp.wav", recHandle, 16, true);
            Bass.BASS_ChannelPlay(recHandle, false);


            first = false;

            timer1.Interval = 1000;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)  //stop
        {

            label1.Text = "00:00";
            stopped = false;
            if (writer != null)
                writer.Close();

            button2.Enabled = false;
            button3.Enabled = true;
            timer1.Stop();
            button4.Enabled = true;
            button1.Enabled = true;
            label4.Text = "";


            playHandle = Bass.BASS_StreamCreateFile("temp.wav", 0, 0, 0);

            channel = Bass.BASS_SampleGetChannel(playHandle, false);


            long length = Bass.BASS_ChannelGetLength(playHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(playHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            label2.Text = "00:00/" + time;
        }

        private void button3_Click(object sender, EventArgs e)  //save
        {
            saveFileDialog1.Filter = "Wave Format (*.wav)|*.wav";
            saveFileDialog1.DefaultExt = "wav";
            saveFileDialog1.AddExtension = true;
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                File.Copy("temp.wav", filename, true);
            }
        }

        private bool MyRecording(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (length>0 && buffer!=IntPtr.Zero)
            {
                if (recBuffer == null || recBuffer.Length<length)
                {
                    recBuffer = new byte[length];
                }
                System.Runtime.InteropServices.Marshal.Copy(buffer, recBuffer, 0, length);
                bytesWritten += length;
                if (writer!=null && stopped)
                    writer.Write(buffer, length);
            }
            return stopped;
        }

        private void timer1_Tick(object sender, EventArgs e)    //recording timer
        {

            count--;
            if (count == 0)
                label4.Text = "Recording in progress";

            long position = Bass.BASS_ChannelGetPosition(recHandle);

            double currentTime = Bass.BASS_ChannelBytes2Seconds(recHandle, position);

            TimeSpan current = TimeSpan.FromSeconds(currentTime);

            string currentSt = current.ToString(@"mm\:ss");

            label1.Text = currentSt;
        }

        #endregion Recording

        #region Playback

        private void timer2_Tick(object sender, EventArgs e)    //playback timer
        {
            if (Bass.BASS_ChannelIsActive(playHandle) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                button1.Enabled = true;
                Bass.BASS_ChannelSetPosition(playHandle, 0);
                button4.Enabled = true;
                button5.Enabled = false;
                button6.Enabled = false;
                timer2.Stop();

                long lengthf = Bass.BASS_ChannelGetLength(playHandle);

                double totalTimef = Bass.BASS_ChannelBytes2Seconds(playHandle, lengthf);

                TimeSpan tsf = TimeSpan.FromSeconds(totalTimef);

                string timef = tsf.ToString(@"mm\:ss");

                label2.Text = "00:00/" + timef;
                return;
            }
            button1.Enabled = false;

            long length = Bass.BASS_ChannelGetLength(playHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(playHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            long position = Bass.BASS_ChannelGetPosition(playHandle);

            double currentTime = Bass.BASS_ChannelBytes2Seconds(playHandle, position);

            TimeSpan current = TimeSpan.FromSeconds(currentTime);

            string currentSt = current.ToString(@"mm\:ss");

            label2.Text = currentSt + "/" + time;
        }

        private void button4_Click(object sender, EventArgs e)  //start Playback
        {
            button4.Enabled = false;
            button5.Enabled = true;
            button6.Enabled = true;
            Bass.BASS_ChannelPlay(playHandle, false);
            timer2.Start();
        }


        private void button5_Click(object sender, EventArgs e) //pause Playback
        {
            button1.Enabled = true;
            Bass.BASS_ChannelPause(playHandle);
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = true;
            timer2.Stop();
        }

        private void button6_Click(object sender, EventArgs e) //stop Playback
        {
            button1.Enabled = true;
            Bass.BASS_ChannelStop(playHandle);
            Bass.BASS_ChannelSetPosition(playHandle, 0);
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = false;
            timer2.Stop();

            long length = Bass.BASS_ChannelGetLength(playHandle);

            double totalTime = Bass.BASS_ChannelBytes2Seconds(playHandle, length);

            TimeSpan ts = TimeSpan.FromSeconds(totalTime);

            string time = ts.ToString(@"mm\:ss");

            label2.Text = "00:00/" + time;
        }

        #endregion Playback


    }
}
