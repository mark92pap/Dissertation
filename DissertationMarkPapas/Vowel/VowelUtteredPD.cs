using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DissertationMarkPapas
{
    public class VowelUtteredPD : IVowel
    {
        public Root z1 { get; set; }
        public Root z2 { get; set; }
        public float targetF1 { get; private set; }
        public float targetF2 { get; private set; }

        public Complex targetZ1 { get; set; }
        public Complex targetZ2 { get; set; }


        public Vowel vowel { get; private set; }

        public VowelUtteredPD(Root root1, Root root2, List<float>[,] pdFreqs, float[,] hcMeanFreqs)
        {
            z1 = root1;
            z2 = root2;

            getVowel(pdFreqs, hcMeanFreqs);

            getTargetFormants();
        }
        
        public void getVowel(List<float>[,] pdFreqs, float[,] hcMeanFreqs)
        {
            int v = -1;

            float[] distance = new float[5];

            for (int i = 0; i < 5; i++)
            {
                distance[i] = 0;

                int length = pdFreqs[i, 0].Count;

                //calculating mean distance

                for (int j = 0; j < length; j++)
                {
                    distance[i] = distance[i] + (float)Math.Sqrt(Math.Pow(pdFreqs[i, 0][j] - z1.angle, 2) + Math.Pow(pdFreqs[i, 1][j] - z2.angle, 2)) / length;
                }

            }

            //find minimum distance --> vowel uttered

            float min = float.MaxValue;

            for (int i = 0; i < 5; i++)
            {
                if (distance[i] < min)
                {
                    min = distance[i];
                    v = i;
                }
            }

            targetF1 = hcMeanFreqs[v, 0];
            targetF2 = hcMeanFreqs[v, 1];

            switch (v)
            {
                case 0:
                    vowel = Vowel.a;
                    break;
                case 1:
                    vowel = Vowel.e;
                    break;
                case 2:
                    vowel = Vowel.i;
                    break;
                case 3:
                    vowel = Vowel.o;
                    break;
                case 4:
                    vowel = Vowel.u;
                    break;
                default:
                    vowel = Vowel.none;
                    break;
            }

        }

        private void getTargetFormants()
        {
            //Frequencies and Poles
            targetZ1 = getTargetPole(z1.z, targetF1);
            targetZ2 = getTargetPole(z2.z, targetF2);

            //setPoleWidth();
        }

        private void setPoleWidth()
        {
            double amp1 = Math.Sqrt(Math.Pow(targetZ1.Re, 2) + Math.Pow(targetZ1.Im, 2));

            double amp2 = Math.Sqrt(Math.Pow(targetZ2.Re, 2) + Math.Pow(targetZ2.Im, 2));

            double factor = 0.95;

            targetZ1.Re = targetZ1.Re * factor / amp1;
            targetZ1.Im = targetZ1.Im * factor / amp1;

            targetZ2.Re = targetZ2.Re * factor / amp2;
            targetZ2.Im = targetZ2.Im * factor / amp2;

        }

        private Complex getTargetPole(Complex pole, float f)
        {
            double amp = Math.Sqrt(Math.Pow(pole.Re, 2) + Math.Pow(pole.Im, 2));

            double log = Math.Log(amp);

            log = Math.Abs(log);

            //log = 0.99;

            //double real = log * Math.Cos(2 * Math.PI * f/7350);

            //double im = log * Math.Sin(2 * Math.PI * f/7350);

            double real = amp * Math.Cos(2 * Math.PI * f / 44100);

            double im = amp * Math.Sin(2 * Math.PI * f / 44100);

            double factor = 1;

            Complex target = new Complex(factor*real, factor*im);

            return target;
        }
    }
}