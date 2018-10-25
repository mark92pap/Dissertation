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

namespace testBassGood
{
    public partial class SpeechCorrection : Form
    {

        #region Clustering


        private void testClustering()
        {

            ClusteringFile file = new ClusteringFile("E:/Stuff/ProgrammingStuff/Dissertation/DiplwmatikiDownloads/PC-GITA db/Train and development subsets/vowels/");

            StringBuilder csvAll = new StringBuilder();

            CSVClustering csv = new CSVClustering("files/allFreqsNewClasses.csv");


            for (int cat = 0; cat < 2; cat++)
            {
                Debug.WriteLine($"cat = {cat + 1}");

                for (int v = 0; v < 5; v++)
                {
                    Debug.WriteLine($"Vowel = {v + 1}");

                    file.setCurrentFolder(categories[cat] + "/" + vowels[v].ToUpper() + "/");
                    recNum = -1;
                    for (int i = 1; i < 60; i++)
                    {

                        for (int j = 1; j < 4; j++)
                        {

                            #region FileNameInit

                            file.setFilename(cat, i, j, vowels, v);

                            if (!File.Exists(file.completePath))
                            {
                                Debug.WriteLine("FILE DOESN'T EXIST");
                                break;  //Τhe only case it doesn't exist is if i is wrong
                            }

                            recNum++;

                            #endregion FileNameInit

                            Debug.WriteLine("Recording Number = " + recNum);

                            Bass.BASS_StreamFree(clusteringHandle);

                            clusteringHandle = Bass.BASS_StreamCreateFile(file.completePath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

                            clusteringChannel = Bass.BASS_SampleGetChannel(decodeHandle, false);

                            float[,] f = clusterFormants2();     //PROCEEEEEEEEEEEEEEEEEEEEEEEEEEEEESS

                            csv.appendRecording(recNum, f.GetLength(0), cat, v, f);
                        }
                    }

                }

            }
            csv.finalize();
            

        }

        private void clustering()
        {


            #region Init
            //LOAD ALL RECORDINGS OF VOWELS AND CREATE CLUSTERS

            string clusterFolder = "D:/Stuff/diplwmatiki/DiplwmatikiDownloads/PC-GITA db/Train and development subsets/vowels";

            string folderNow = clusterFolder + "/Control/A";

            StringBuilder csvAll = new StringBuilder();

            #endregion Init


            for (int cat = 0; cat < 2; cat++)
            {
                Debug.WriteLine($"cat = {cat + 1}");

                for (int v = 0; v < 5; v++)
                {
                    Debug.WriteLine($"Vowel = {v + 1}");

                    folderNow = clusterFolder + "/" + categories[cat] + "/" + vowels[v].ToUpper();
                    recNum = -1;
                    for (int i = 1; i < 60; i++)
                    {

                        for (int j = 1; j < 4; j++)
                        {
                            //CODE FOR EACH RECORDING IN HERE

                            #region FileNameInit

                            string clusterFile = folderNow;

                            if (cat == 0)
                            {
                                clusterFile = clusterFile + "/AVPEPUDEAC00";
                            }
                            else
                            {
                                clusterFile = clusterFile + "/AVPEPUDEA00";
                            }
                            if (i < 10)
                            {
                                clusterFile += "0" + i + vowels[v] + j + ".wav";   //need to add one more 0
                            }
                            else
                            {
                                clusterFile += i + vowels[v] + j + ".wav";
                            }

                            //CODE FOR EACH FILE

                            if (!File.Exists(clusterFile))  //if file doesn't exist go on with the next one
                            {
                                break;  //Τhe only case it doesn't exist is if i is wrong
                            }
                            //FILE EXISTS --> OPEN FILE

                            recNum++;

                            #endregion FileNameInit

                            Debug.WriteLine("Recording Number = " + recNum);

                            Bass.BASS_StreamFree(clusteringHandle);

                            clusteringHandle = Bass.BASS_StreamCreateFile(clusterFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

                            clusteringChannel = Bass.BASS_SampleGetChannel(decodeHandle, false);

                            float[,] f = clusterFormants();     //PROCEEEEEEEEEEEEEEEEEEEEEEEEEEEEESS

                            for (int binNum = 0; binNum < f.GetLength(0); binNum++)
                            {
                                var line = string.Format("{0},{1},{2},{3},{4},{5}", cat, v, recNum, binNum, f[binNum, 0], f[binNum, 1]);
                                csvAll.AppendLine(line);
                            }
                        }
                    }

                }

            }

            File.WriteAllText("files/allFreqs.csv", csvAll.ToString());

        }

        private float[,] clusterFormants2()
        {
            //RECORDING ON CLUSTERING HANDLE
            //SWEEP RECORDING AND FIND MEAN OF f1, f2
            float[] data = new float[binLength];

            List<float> F1 = new List<float>(); //LIST CONTAINING F1 VALUES FOR ALL BINS
            List<float> F2 = new List<float>(); //LIST CONTAINING F2 VALUES FOR ALL BINS

            while (Bass.BASS_ChannelGetData(clusteringHandle, data, (binLength * 4) | (int)BASSData.BASS_DATA_FLOAT) != -1)
            {

                LPClength = 46;

                RecordingData samples = new RecordingData(data, sampleRate, binLength);

                LPC lpcSamples = new LPC(LPClength, samples);

                if (lpcSamples.finalRoots.Length<2) //less than two roots, don't write anything
                {
                    continue;
                }

                F1.Add(lpcSamples.finalRoots[0].angle);
                F2.Add(lpcSamples.finalRoots[1].angle);

            }

            float[,] f = new float[F1.Count, 2];

            for (int i = 0; i < F1.Count; i++)
            {
                f[i, 0] = F1[i];
                f[i, 1] = F2[i];
            }

            return f;
        }

        private float[,] clusterFormants()    //return f1,f2
        {
            //RECORDING ON CLUSTERING HANDLE
            //SWEEP RECORDING AND FIND MEAN OF f1, f2
            float[] data = new float[binLength];

            float[] f = new float[2];

            int binNumber = -1;

            List<float> F1 = new List<float>(); //LIST CONTAINING F1 VALUES FOR ALL BINS
            List<float> F2 = new List<float>(); //LIST CONTAINING F2 VALUES FOR ALL BINS

            while (Bass.BASS_ChannelGetData(clusteringHandle, data, (binLength * 4) | (int)BASSData.BASS_DATA_FLOAT) != -1)
            {

                binNumber++;

                LPClength = 46;

                float[] downsampled = dataPreprocessing(data);  //downsampled by 6

                float[] autocorrelation = new float[LPClength + 1];

                getAutoCorrelation(downsampled, autocorrelation, 1);

                getFrequencies(autocorrelation, f);

                F1.Add(f[0]);
                F2.Add(f[1]);

                Debug.WriteLine($"Bin Number = {binNumber + 1} \n");
                Debug.WriteLine($"F1 = {f[0]} \t F2 = {f[1]}\n");

            }

            float[,] f2 = new float[F1.Count, 2];

            for (int i = 0; i < F1.Count; i++)
            {
                f2[i, 0] = F1[i];
                f2[i, 1] = F2[i];
            }

            return f2;
        }

        private void getFrequencies(float[] autocorrelation, float[] f)
        {
            float[] LPC = new float[LPClength];

            LPC = getLPC(autocorrelation, 1);

            Complex[] roots = getLPCRootsEigen(LPC);

            Complex[] rootsFinal = getFinalRoots(roots);    //keep only the ones with imag part > 0 and compute their angles && bandwidths


            float[] angles = getAngles(rootsFinal);         //Hz --> Correct


            Quicksort.QuickSortMethod(angles);

            float[] bandwidths = getBandwidths(rootsFinal);


            if (angles.Length > 1)
            {
                f[0] = angles[0];
                f[1] = angles[1];
            }
            else
            {
                f[0] = 700;
                f[1] = 700;
            }
        }

        private float[] dataPreprocessing(float[] data)
        {

            //TEST FOR DOWNSAMPLED

            //float[] temp = downsample(data, 6);

            float[] temp = data;

            for (int i = 0; i < temp.Length; i++)
            {
                float window = 0.54f - 0.46f * (float)Math.Cos((2 * Math.PI * i) / (temp.Length - 1));
                temp[i] = window * temp[i];
            }

            //PREEMPHASIS

            for (int i = 1; i < temp.Length; i++)
            {
                temp[i] = temp[i] - 0.63f * temp[i - 1];
            }

            return temp;
        }

        private float getMin(float a, float b)
        {
            if (a <= b)
                return a;
            else
                return b;
        }

        private float[] filter(float[] num, float[] den, float[] data)  //CORRECT
        {

            float[] ret = new float[data.Length];

            ret[0] = data[0];

            for (int n = 1; n < ret.Length; n++)
            {

                float temp1 = 0;
                float temp2 = 0;

                float min1 = getMin(n, num.Length - 1);
                float min2 = getMin(n, den.Length - 1);

                for (int i = 0; i < min1 + 1; i++)
                {

                    if (min1 == 0)
                        break;

                    temp1 += num[i] * data[n - i];


                }

                if (min2 > 0)
                {
                    temp2 = data[n];

                }


                for (int i = 1; i < min2 + 1; i++)
                {
                    temp2 -= den[i] * ret[n - i];
                }

                ret[n] = temp1 + temp2;

            }


            return ret;
        }

        #endregion Clustering

    }
}
