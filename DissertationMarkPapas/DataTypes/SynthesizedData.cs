using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    public class SynthesizedData
    {
        public RecordingData originalData { get; set; }
        public LPC previousCoeffs { get; set; }
        public LPC newCoeffs { get; set; }
        public int padLength { get; set; }
        public float[] synthesizedData { get; set; }
        public float[] overlapData { get; set; }

        public SynthesizedData(RecordingData data, LPC preCoeffs, VowelUtteredPD vowel)
        {
            originalData = data;
            previousCoeffs = preCoeffs;
            getNewCoeffs(previousCoeffs, vowel);
            padLength = newCoeffs.length ;

            synthesizedData = new float[originalData.data.Length];
            overlapData = new float[newCoeffs.length];

            synthesize();

        }

        public void addOverlap(float[] overlap)
        {
            for (int i=0;i<overlap.Length;i++)
            {
                synthesizedData[i] += overlap[i];
            }
        }

        private void synthesize()
        {
            float[] padded = getPaddedSignal(originalData.preprocessedData, padLength);

            float[] e = filter(previousCoeffs.fullCoefficients, new float[1] { 1 }, padded);

            float[] yPadded = filter(new float[1] { 1 }, newCoeffs.fullCoefficients, e);

            for (int i=0;i<synthesizedData.Length;i++)
            {
                synthesizedData[i] = yPadded[i];
            }
            

            for (int i = 0; i < overlapData.Length; i++)
            {
                overlapData[i] = yPadded[i + synthesizedData.Length];
            }
        }

        private void getNewCoeffs(LPC previousCoeffs, VowelUtteredPD vowel)
        {
            newCoeffs = new LPC(previousCoeffs, vowel);
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

        private float getMin(float a, float b)
        {
            if (a <= b)
                return a;
            else
                return b;
        }

        private float[] getPaddedSignal(float[] data, int padLength)
        {
            float[] padded = new float[data.Length + padLength]; ;


            for (int i = 0; i < data.Length; i++)
            {
                padded[i] = data[i];
            }

            for (int i = data.Length; i < padded.Length; i++)
            {
                padded[i] = 0;
            }


            return padded;
        }
    }
}
