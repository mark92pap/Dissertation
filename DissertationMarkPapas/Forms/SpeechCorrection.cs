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

        #region Delegates


        delegate void StringArgReturningVoidDelegate(string text);

        delegate void volumeSetVoidDelegate();

        #endregion Delegates


        #region Variables

        #region Debug Variables

        int numVoiced = 0;

        int numUnvoiced = 0;

        int totalNumSamples = 0;

        #endregion Debug Variables

        #region HandleVariables

        int pushHandle = -1;

        int originalHandle = -1;    //original input

        int originalChannel = -1;   //original channel

        int decodeHandle = -1;      //handle for filtering

        int decodeChannel = -1;     //filtering channel

        int floatHandle = -1;       //handle for energy calculation

        int voicedHandle = 0;       //prob not needed

        int unvoicedHandle = 0;     //prob not needed

        int formantsHandle = 0;     //modified formants handle

        int f0Handle = 0;           //f0 generated part

        int lowPassHandle = 0;      //used for voiced/low pass part

        int highPassHandle = 0;     //used for unvoiced/high pass part

        #endregion HandleVariables

        #region Filter Variables

        int LPClength;

        int movAvgLength = 4;

        int cutOffFreq = 4000;

        int binLength = 661;

        float[] overlapData;

        float[] previousOverlapData;

        bool voiced;

        int windowSize = 8;

        float detThreshold = 1f;

        //FILTERS
        private DSPPROC passData;

        BASS_BFX_BQF lowPass = new BASS_BFX_BQF();

        BASS_BFX_BQF highPass = new BASS_BFX_BQF();

        #endregion Filter Variables

        #region Other Variables

        float sampleRate = 44100;
        int sampleRateInt = 44100;

        double max = 0, min = double.MaxValue;
        double median = 0;
        int count = 0;
        int position = 0; //used for spectrogram

        private Visuals vis = new Visuals();
        private Visuals visSpectro = new Visuals();
        private Visuals visSpectroConverted = new Visuals();

        WaveForm waveFormOriginal = null;

        WaveForm waveFormConverted = null;

        string fileName = string.Empty;

        int device = -1;

        private int specIdx = 0;       //Spectrum Type
        private int voicePrintIdx = 0;



        #endregion Other Variables

        #region Clustering Variables

        int clusteringHandle = -1;  //Handle for initial clustering
        int clusteringChannel = -1; //Channel for initial Clustering

        string[] vowels = new string[] { "a", "e", "i", "o", "u" };
        string[] categories = new string[] { "Control", "Patologicas" };
        
        float[,,] pdValues = new float[5, 150, 2]; //Vowel, Recording#, F#
        int recNum = -1;

        Complex[] rootsAngles = new Complex[2];

        //LISTS FOR GETTING VALUES FROM CSV

        List<float>[,] hcFreqs = new List<float>[5, 2]; //List(vowel,f#)
        List<float>[,] pdFreqs = new List<float>[5, 2];

        float[,] hcMeanFreqs = new float[5, 2];
        float[,] pdMeanFreqs = new float[5, 2];

        Complex[] targetPoles = new Complex[2];

        #endregion Clustering Variables

        #endregion Variables


        //TODO
        #region Conversion

        private void convert()
        {
            #region Initializations

            CSVCreator csv = new CSVCreator("correctionResults.csv");

            //StringBuilder csv = new StringBuilder();

            //debug csv

            decodeHandle = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_STREAM_DECODE);

            BASS_CHANNELINFO info;

            info = Bass.BASS_ChannelGetInfo(decodeHandle);

            floatHandle = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);   //used for voiced/unvoiced detection

            //pushHandle = Bass.BASS_StreamCreatePush(info.freq, info.chans, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, IntPtr.Zero);    //PUSH STREAM FOR ADDITION

            pushHandle = Bass.BASS_StreamCreatePush(44100, info.chans, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, IntPtr.Zero);

            //FX

            //create bins with binLength bits length ~ 10ms @44100kHz  with Bass.BASS_ChannelGetData()

            LPClength = 46;

            overlapData = new float[LPClength];
            previousOverlapData = new float[LPClength];

            for (int i = 0; i < overlapData.Length; i++)
            {
                previousOverlapData[i] = 0;
                overlapData[i] = 0;
            }

            float[] data = new float[binLength];

            int currentBin = 0;

            #endregion initializations

            while (Bass.BASS_ChannelGetData(floatHandle, data, (binLength * 4) | (int)BASSData.BASS_DATA_FLOAT) != -1)
            {

                totalNumSamples += binLength;
                currentBin++;

                Debug.WriteLine($"Current Bin = {currentBin}\n");
                
                RecordingData initialData = new RecordingData(data, sampleRate, binLength);

                if (initialData.voiced==false)    //if unvoiced just overlapp add
                {
                    numUnvoiced++;
                    overlapAddUnvoiced(data);
                    continue; 
                }

                LPC previousLPC = new LPC(LPClength, initialData);

                if (previousLPC.finalRoots.Length<2)
                {
                    continue;
                }

                VowelUtteredPD vowel = new VowelUtteredPD(previousLPC.finalRoots[0], previousLPC.finalRoots[1],pdFreqs,hcMeanFreqs);

                SynthesizedData output = new SynthesizedData(initialData, previousLPC, vowel);

                //output.addOverlap(overlapData);

                Bass.BASS_StreamPutData(pushHandle, output.synthesizedData, output.synthesizedData.Length * sizeof(float));

                overlapData = output.overlapData;

                LPC finalLPC = new LPC(LPClength, output);
                
                if (finalLPC.finalRoots.Length<2)
                {
                    continue;
                }


                csv.AppendLine(currentBin, previousLPC, finalLPC, vowel);
                
            }
            csv.writeFile();
        }
        

        #region Synthesis Functions

        float[] getPaddedSignal(float[] data, int padLength)
        {
            float[] padded = new float[data.Length + padLength]; ;


            for (int i=0;i<data.Length;i++)
            {
                padded[i] = data[i];
            }

            for (int i=data.Length;i<padded.Length;i++)
            {
                padded[i] = 0;
            }


            return padded;
        }

        Polynomial getNewPolynomial(Complex[] roots)
        {
            Polynomial[] rootPoly = new Polynomial[roots.Length];

            for (int i=0;i<roots.Length;i++)
            {
                Complex[] coeffs = new Complex[2];
                coeffs[0] = -roots[i];

                coeffs[1] = new Complex(1, 0);
                rootPoly[i] = new Polynomial(coeffs);

            }

            Polynomial newPoly = rootPoly[0];

            for (int i=1;i<rootPoly.Length;i++)
            {
                newPoly = newPoly * rootPoly[i];
            }

            return newPoly;
        }

        int checkVowel(float f1,float f2)
        {
            int vowel=-1;

            float[] distance = new float[5];

            for (int v=0;v<5;v++)
            {
                distance[v] = 0;

                int length = pdFreqs[v, 0].Count;

                //calculating mean distance

                for (int i=0; i<length; i++)
                {
                    distance[v] = distance[v] + (float)Math.Sqrt(Math.Pow(pdFreqs[v,0][i] - f1, 2) + Math.Pow(pdFreqs[v,1][i] - f2, 2))/length;
                }

                //distance[v] = (float)Math.Sqrt(Math.Pow(pdMeanFreqs[v,0]-f1, 2) + Math.Pow(pdMeanFreqs[v,1]-f2, 2));

            }

            //find minimum distance --> vowel uttered

            float min = float.MaxValue;

            for (int v =0; v<5; v++)
            {
                if (distance[v]<min)
                {
                    min = distance[v];
                    vowel = v;
                }
            }


            return vowel;
        }

        int[] getPoleIndexes(float[] angles, float[] anglesAll)
        {
            int[] poleIndexes = new int[4];


            for (int i=0;i<anglesAll.Length;i++)
            {
                //angles is already sorted

                if (angles[0]==anglesAll[i])    //if it corresponds to f1
                {
                    poleIndexes[0] = i;
                }

                if (angles[1]==anglesAll[i])    //if it corresponds to f2
                {
                    poleIndexes[1] = i;
                }

                if (angles[0] == -anglesAll[i])
                {
                    poleIndexes[2] = i;         //Conjugate of f1
                }

                if (angles[1] == -anglesAll[i])
                {
                    poleIndexes[3] = i;         //Conjugate of f2
                }

            }

            return poleIndexes;
        }

        Complex getTargetPole(Complex pole, float f)
        {
            double amp = Math.Sqrt(Math.Pow(pole.Re,2) + Math.Pow(pole.Im,2));

            double log = Math.Log(amp);

            log = Math.Abs(log);

            //double real = log * Math.Cos(2 * Math.PI * f/7350);

            //double im = log * Math.Sin(2 * Math.PI * f/7350);

            double real = amp * Math.Cos(2 * Math.PI * f / 44100);

            double im = amp * Math.Sin(2 * Math.PI * f/44100);

            Complex target = new Complex(real,im);

            return target;
        }

        void getTargetPoles(Complex root1,Complex root2, int vowel)
        {
            targetPoles[0] = getTargetPole(root1, hcMeanFreqs[vowel, 0]);

            targetPoles[1] = getTargetPole(root2, hcMeanFreqs[vowel, 1]);
        }

        #endregion Synthesis Functions


        #region Main Functions

        private float getF0(float[] array, int bin)  //needs change - HPS 
        {

            StringBuilder csv = new StringBuilder("magnitudes,hps1,hps2,hps3,hps4,Product,f0\n");  //used to extract csv file

            float f0 = 0;

            float[] copy = new float[array.Length];

            copyArray(copy, array, 1);

            hannWindow(copy);   //Windowed

            //TEST WITH BIGGER FFT
            Exocortex.DSP.ComplexF[] copyFFT = new Exocortex.DSP.ComplexF[2048];

            for (int i = 0; i < copy.Length; i++)
            {
                copyFFT[i].Re = copy[i];
                copyFFT[i].Im = 0;
            }

            for (int i = copy.Length; i < 2048; i++)
            {
                copyFFT[i].Re = 0;
                copyFFT[i].Im = 0;
            }


            //END OF TEST

            Exocortex.DSP.Fourier.FFT(copyFFT, Exocortex.DSP.FourierDirection.Forward); //got FOURIER

            //DOWNSAMPLE

            float[] magnitudes = new float[copyFFT.Length / 2];

            getMagnitudes(magnitudes, copyFFT);

            float[] hps1 = downsample(magnitudes, 2);
            float[] hps2 = downsample(magnitudes, 3);
            float[] hps3 = downsample(magnitudes, 4);
            float[] hps4 = downsample(magnitudes, 5);


            float[] products = new float[hps4.Length];

            for (int i = 0; i < products.Length; i++)
            {
                products[i] = magnitudes[i] * hps1[i] * hps2[i] * hps3[i] * hps4[i];
            }

            int maxIndex = getMaxIndex(products);

            Debug.WriteLine($"F0 index = {maxIndex}");

            f0 = freqFromIndex(maxIndex);

            //ZERO CROSSING ESTIMATE

            float[] absArray = new float[array.Length];

            float mean = 0;

            float[] movedArray = new float[array.Length];



            for (int i = 0; i < array.Length; i++)
            {
                absArray[i] = Math.Abs(array[i]);

            }


            absArray = filter(new float[3] { 0.00051895f, 0.001f, 0.00051895f }, new float[3] { 1f, -1.9345f, 0.9366f }, absArray);

            for (int i = 0; i < absArray.Length; i++)
            {
                mean += absArray[i];
            }

            mean = mean / absArray.Length;

            for (int i = 0; i < absArray.Length; i++)
            {
                absArray[i] -= mean;

                if (i != array.Length - 1)
                {
                    movedArray[i] = absArray[i + 1];
                }
                else
                {
                    movedArray[i] = 0;
                }
            }

            int zcr = 0;

            for (int i = 0; i < absArray.Length; i++)
            {
                if ((movedArray[i] > 0) && (absArray[i] < 0))
                {
                    zcr++;
                }
                if ((movedArray[i] < 0) && (absArray[i] > 0))
                {
                    zcr++;
                }
            }


            float f0zcr = 0.5f * sampleRate * zcr / absArray.Length;

            Debug.WriteLine($"F0 ZCR ESTIMATE = {f0zcr} \t zcr={zcr}");

            //end ZCR ESTIMATE


            #region FileHPS

            for (int i = 0; i < magnitudes.Length; i++)
            {
                if (i < hps4.Length)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", magnitudes[i], hps1[i], hps2[i], hps3[i], hps4[i], products[i], f0);
                    csv.AppendLine(newLine);
                    continue;
                }

                if (i < hps3.Length)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", magnitudes[i], hps1[i], hps2[i], hps3[i], 0, 0, f0);
                    csv.AppendLine(newLine);
                    continue;
                }

                if (i < hps2.Length)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", magnitudes[i], hps1[i], hps2[i], 0, 0, 0, f0);
                    csv.AppendLine(newLine);
                    continue;
                }

                if (i < hps1.Length)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", magnitudes[i], hps1[i], 0, 0, 0, 0, f0);
                    csv.AppendLine(newLine);
                    continue;
                }

                var newLine2 = string.Format("{0},{1},{2},{3},{4},{5},{6}", magnitudes[i], 0, 0, 0, 0, 0, f0);
                csv.AppendLine(newLine2);

            }

            if (bin == 2)
            {
                File.WriteAllText("files/magnitudes.csv", csv.ToString());
            }
            #endregion FileHPS

            return f0;

        }

        private int getMaxIndex(float[] array)
        {
            int maxIndex = 0;

            for (int i=1;i<array.Length;i++)
            {
                if (array[i]>array[maxIndex])
                {
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private float[] downsample(float[] array, int factor)
        {
            float[] sampled = new float[array.Length / factor];

            for (int i=0; i<sampled.Length;i++)
            {
                sampled[i] = array[i * factor];
            }

            return sampled;
        }

        private void isVoiced(float[] data)     //NEEDS CORRECTION
        {
            //calculate ZCR

            double lengthInSeconds = (double)binLength / (double)sampleRate;    // samples/(samples/sec) = sec

            int numZC = 0;

            for (int i=0;i<data.Length-1;i++)
            {
                float first = data[i];
                float second = data[i + 1];
                if ((first >= 0 && second < 0) || (first <= 0 && second > 0))
                    numZC++;
            }
            //GOT NUMBER OF ZERO CROSSINGS ---------- NOT RATE
            
            //DONE
            //Calculate Energy

            float energy = 0;

            Parallel.For(0, data.Length, i =>
            {
                float s = data[i];
                energy += s * s;    //WINDOW
            }
            );

            energy /= binLength;

            energy *= (float)Math.Pow(10, 5);   //????probs for debug

            //Done

            

            double ratio = energy / numZC;

            //Debug.WriteLine($"ZCR = {numZC} \t \t Energy = {energy} \t \t ratio = {ratio}");

            count++;
            median += energy;
            if (energy > max)
                max = energy;
            if (energy < min)
                min = energy;
            

            if (ratio > detThreshold)
                voiced = true;
            else
                voiced = false;

            voiced = true;
        }

        private void overlapAdd(float[] output)     //CORRECT?
        {
            for (int i = 0; i < previousOverlapData.Length; i++)
            {
                output[i] += previousOverlapData[i];
                previousOverlapData[i] = overlapData[i];            //update
                overlapData[i] = 0;
            }
        }

        private void overlapAddUnvoiced(float[] data)      //CORRECT?
        {

            for (int i = 0; i < previousOverlapData.Length; i++)
            {
                data[i] += previousOverlapData[i];
                previousOverlapData[i] = 0; //no overlap
                overlapData[i] = 0;
            }

            int c = Bass.BASS_StreamPutData(pushHandle, data, data.Length * sizeof(float));

            if (c == -1)
            {
                BASSError error = Bass.BASS_ErrorGetCode();

                MessageBox.Show(error.ToString(), "first error");
                return;
            }

        }

        private void getAutoCorrelation(float[] arr, float[] output,int bin)     //correct
        {
            
            int xLength = output.Length;
            int aLength = arr.Length;

            float max = -1;

            for (int k = 0; k < xLength; k++)
            {
                output[k] = 0;

                for (int m = 0; m < aLength - k; m++)   //last value is N-k-1
                {
                    output[k] += arr[m] * arr[m + k];
                }

                if (output[k]>max)
                {
                    max = output[k];
                }
            }

            //normalize

            for (int i=0;i<xLength;i++)
            {
                output[i] = output[i] / max;
            }


        }

        private int[] getFormants(float[] array)
        {
            int[] formantIndexes = new int[4];

            //Get Autocorrelation

            //float[] autocorrelation = new float

            //Compute LPC coeffs


            //Compute roots of coeffs


            //retain only roots with >=0 imag parts

            //convert to rad/s and compute bandwidths

            //formants freqs = freqs>90Hz && Bandwidth<400Hz



            return formantIndexes;
        }

        private void getMagnitudes(float[] mags, Exocortex.DSP.ComplexF[] fft)
        {
            for (int i=0;i<mags.Length;i++)
            {
                mags[i] = (float)Math.Pow(fft[i].Re, 2) + (float)Math.Pow(fft[i].Im, 2);
                mags[i] = (float)Math.Sqrt(mags[i]);
            }
        }


        #endregion Main Functions


        #region LPC-related

        private void getCoeffs(float[] autocorrelation, float[,] autocMatrix, float[] coeffs, int bin) //probs correct
        {
            //calculate
            int rows = autocMatrix.GetLength(0);
            int cols = autocMatrix.GetLength(1);
            float[,] temp = new float[rows, cols];

            //COPY TO TEMP
            for (int i=0; i< rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    temp[i, j] = autocMatrix[i, j];
                }

            }

            Matrix<float> A = Matrix<float>.Build.DenseOfArray(temp);

            Vector<float> V = Vector<float>.Build.DenseOfArray(autocorrelation);

            Vector<float> solution = A.Solve(V);

            V = solution;

            for (int i=0;i<coeffs.Length;i++)
            {
                coeffs[i] = solution[i];
            }
            

        }

        private float[] getLPC(float[] autocorrelation, int bin) //probs correct
        {
            int xLength = autocorrelation.Length;
            int matrixLength = xLength - 1;

            float[,] autocMatrix = new float[matrixLength, matrixLength]; //need values R(0:lpcLength-1)
            float[] rVector = new float[matrixLength];

            for (int i=0;i<matrixLength; i++)
            { 
                autocMatrix[0, i] = autocorrelation[i];   //fill first row and column
                autocMatrix[i, 0] = autocorrelation[i];

                rVector[i] = autocorrelation[i + 1];

            }

            for (int i=1;i<matrixLength;i++)
            {
                for (int j = 1; j < matrixLength; j++)
                {
                    autocMatrix[i, j] = autocMatrix[i - 1, j - 1]; //each other element is equal to the previous element in the diagonal
                }
            }  //FILLED

            float[] LPC = new float[matrixLength];

            getCoeffs(rVector, autocMatrix, LPC, bin);  //done

            return LPC;

        }

        Complex[] getFinalRoots(Complex[] roots)
        {


            float[] angles = getAngles(roots);


            Array.Sort(angles, roots);  //roots and angles have the same INDEX

            float[] bandwidths = getBandwidths(roots);

            float[] ratio = new float[bandwidths.Length];

            for (int i = 0; i < bandwidths.Length; i++)
            {
                ratio[i] = bandwidths[i] / angles[i];
            }

            int rootCounter = 0;


            
            for (int i = 0; i < roots.Length; i++)
            {
                if ((roots[i].Im > 0) && (ratio[i]<0.13) && (angles[i]<3000))
                {
                    Debug.WriteLine($"OKAY \t \t angle[{i}] = {angles[i]} \t bandwidths[{i}]] = {bandwidths[i]} \t ratio[{i}] = {ratio[i]}");
                    rootCounter++;
                }
                
                else if ((roots[i].Im>0) && (angles[i] < 3000))
                {
                    Debug.WriteLine($"NOT OKAY \t angle[{i}] = {angles[i]} \t bandwidths[{i}]] = {bandwidths[i]} \t ratio[{i}] = {ratio[i]}");
                }
                
            }

            Complex[] finalRoots = new Complex[rootCounter];


            int indexer = 0;


            for (int i = 0; i < roots.Length; i++)
            {
                if ((roots[i].Im > 0) && (ratio[i] < 0.13) && (angles[i] < 3000))
                {
                    finalRoots[indexer] = roots[i];
                    indexer++;
                }
            }




            return finalRoots;
        }

        Complex[] getLPCRoots(float[] LPC,int bin)
        {

            double[] polynomialCoeffs = new double[LPC.Length + 1];

            polynomialCoeffs[0] = 1;

            for (int i = 0; i < LPC.Length; i++)
            {
                polynomialCoeffs[i + 1] = -LPC[i];
            }


            Array.Reverse(polynomialCoeffs);    //Sumfwna me Matlab to 1 paei sti megaluteri dunami
            
            Polynomial p = new Polynomial(polynomialCoeffs);

            //Complex[] lpcRoots = p.Roots();

            Complex[] lpcRoots = Polynomial.Roots(p);

            return lpcRoots;
        }

        Complex[] getLPCRootsEigen(float[] LPC)     //MATLAB CODE ---> CORRECT
        {
            double[,] A = new double[LPC.Length, LPC.Length];

            for (int i=0;i<LPC.Length;i++)
            {
                for (int j=0;j<LPC.Length;j++)
                {
                    if (i==j+1)
                    {
                        A[i, j] = 1;
                    }
                    else
                    {
                        A[i, j] = 0;
                    }
                }
            }

            for (int j=0;j<LPC.Length;j++)
            {
                A[0,j] = (double)LPC[j]; //is already normalized
            }

            Matrix<double> B = Matrix<double>.Build.DenseOfArray(A);

            MathNet.Numerics.LinearAlgebra.Factorization.Evd<double> eigen = B.Evd();

            Vector<System.Numerics.Complex> eigenvector = eigen.EigenValues;

            Complex[] roots = new Complex[eigenvector.Count];

            for (int i=0;i<eigenvector.Count;i++)
            {

                roots[i] = new Complex();
                roots[i].Re = eigenvector[i].Real;
                roots[i].Im = eigenvector[i].Imaginary;

            }
            Debug.WriteLine($"Length Roots = {roots.Length}");
            return roots;
        }

        float[] getBandwidths(Complex[] roots)
        {
            float[] bandwidths = new float[roots.Length];

            for (int i = 0; i < bandwidths.Length; i++)
            {
                float measure = (float)Math.Sqrt(Math.Pow(roots[i].Re, 2) + Math.Pow(roots[i].Im, 2));
                bandwidths[i] = -0.5f * (sampleRate / (float)(2 * Math.PI)) * (float)Math.Log(measure,Math.E);
            }

            return bandwidths;
        }

        float getAngle(Complex root)
        {
            float angle = (float)Math.Atan2(root.Im, root.Re);

            angle *= (sampleRate / 6) / (float)(2 * Math.PI);

            return angle;
        }

        float[] getAngles(Complex[] roots)
        {
            
            float[] angles = new float[roots.Length];

            for (int i = 0; i < roots.Length; i++)
            {
                angles[i] = (float)Math.Atan2(roots[i].Im, roots[i].Re);

                angles[i] *= (sampleRate) / (float)(2 * Math.PI);
            }

            return angles;
        }



        #endregion LPC-related

        
        #region Filters & Basic Functions

        private void hannWindow(float[] array)
        {
            double[] hannDoubles = MathNet.Numerics.Window.Hann(array.Length);

            for (int i = 0; i < array.Length;i++)
            {
                array[i] = (float)hannDoubles[i] * array[i];
            }
        }

        private void movingAverage(float[] arr)
        {
            int length = arr.Length;

            float[] output = new float[length];

            for (int i = 0; i < length; i++)
            {
                int distance = Math.Min(i, arr.Length - i - 1);
                int indexer = Math.Min(distance, movAvgLength);
                float sum = 0;

                for (int j = -indexer; j < indexer + 1; j++)
                {
                    sum += arr[i + j];
                }

                output[i] = sum / (2 * indexer + 1);
            }

            Parallel.For(0, length, i =>
            {
                arr[i] = output[i];
            }
            );
        }

        private void highLowPassFilter(float[] output, ButterworthFilter.PassType type) //CORRECT BUT ADDS NOISE
        {
            ButterworthFilter filter = new ButterworthFilter(cutOffFreq, sampleRate, type, 0.1f);

            for (int i = 0; i < output.Length; i++)
            {
                filter.Update(output[i]);

                output[i] = filter.Value;

            }

        }

        private void energyModification(float[] data)   //TODO - ADDS NOISE     
        {
            double[] Hamming = MathNet.Numerics.Window.Hamming(windowSize);
            MathNet.Filtering.Median.OnlineMedianFilter medianFilter = new MathNet.Filtering.Median.OnlineMedianFilter(windowSize);

            double[] dataFiltered = new double[data.Length + overlapData.Length];

            for (int i = 0; i < data.Length; i++)
            {
                dataFiltered[i] = (double)data[i];
            }

            //convolution
            
            Parallel.For(0, data.Length, i =>
            {
                for (int j = 0; j < windowSize; j++)
                {
                    dataFiltered[i] = data[i] * Hamming[j];
                }
            });
            

            //TEST  (BYPASS)



            //END TEST


            //dataFiltered = medianFilter.ProcessSamples(dataFiltered);

            Parallel.For(0, data.Length, i =>
            {
                data[i] = (float)dataFiltered[i];
            });


            for (int i = data.Length; i < dataFiltered.Length; i++)
            {
                overlapData[i - data.Length] += (float)dataFiltered[i];
            }

        }

        private void initialize(float[] d, float[] d1, float[] d2, float[] d3, float[] d4, float[] d5)
        {
            Parallel.For(0, d.Length, i =>
            {
                d1[i] = d[i];
                d2[i] = d[i];
                d3[i] = d[i];
                d4[i] = d[i];
                d5[i] = d[i];
            }
            );
        }

        private void copyArray(float[] output, float[] source, float factor)
        {
            if (output.Length != source.Length)
            {
                MessageBox.Show("Wrong dimensions!\n");
                return;
            }

            Parallel.For(0, output.Length, i =>
            {
                output[i] = factor * source[i];
            }
            );
        }

        private void addition(float[] hp, float[] synth, float[] output)
        {
            Parallel.For(0, hp.Length, i =>
            {
                output[i] = 2f * hp[i] + synth[i];
                //output[i] *= 7f;
            }
                );
        }

        private float freqFromIndex(int index)
        {
            float freq = 0;

            float Fs = sampleRate;

            float N = 2048f / 2f;

            freq = index * Fs / N;

            return freq;
        }

        private float getMean(float[] arr)
        {
            float mean = 0;

            Parallel.For(0, arr.Length, i =>
            {
                mean += arr[i];
            }
            );
            mean /= arr.Length;
            return mean;
        }

        private void removeDC(float[] array)    //DONE
        {
            float mean = 0;
            Parallel.For(0, array.Length, i =>
            {
                mean += array[i];
            }
            );

            mean /= array.Length;

            Parallel.For(0, array.Length, i =>
            {
                array[i] -= mean;
            }
            );

        }


        #endregion Filters & Basic Function


        #endregion Conversion


    }
}
