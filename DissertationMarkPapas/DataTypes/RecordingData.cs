using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    public class RecordingData
    {
        private const float voicedThreshold = 1f;

        public float[] data { get; set; }

        public float sampleRate { get; set; }

        public int binLength { get; set; }

        public bool voiced { get; set; }

        public float[] preprocessedData { get; set; }

        public RecordingData(float[] samples, float rate, int binLength)   //CONSTRUCTOR
        {
            sampleRate = rate;

            this.binLength = binLength;

            data = new float[samples.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = samples[i];
            }

            isVoiced();

            preprocessing();
        }

        private void isVoiced()
        {
            //calculate ZCR

            double lengthInSeconds = (double)binLength / (double)sampleRate;    // samples/(samples/sec) = sec

            int numZC = 0;

            for (int i = 0; i < data.Length - 1; i++)
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
            
            double ratio = energy / numZC;


            if (ratio > voicedThreshold)
                voiced = true;
            else
                voiced = false;

        }

        private void preprocessing()
        {
            preprocessedData = data;

            for (int i = 0; i < preprocessedData.Length; i++)
            {
                float window = 0.54f - 0.46f * (float)Math.Cos((2 * Math.PI * i) / (preprocessedData.Length - 1));
                preprocessedData[i] = window * preprocessedData[i];
            }

            //PREEMPHASIS

            for (int i = 1; i < preprocessedData.Length; i++)
            {
                preprocessedData[i] = preprocessedData[i] - 0.63f * preprocessedData[i - 1];
            }
        }
    }
}
