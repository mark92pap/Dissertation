using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    class VowelFormants
    {
        public float f1;
        public float f2;
        public int vowel;
        public int spanishVowel;

        public float[,] freqRefs = new float[16, 2] { {240, 2400}, {235, 2100}, {390, 2300}, {370, 1900}, {610, 1900}, {585, 1710}, {850, 1610}, {820, 1530}, {750, 940}, {700, 760}, {600, 1170}, {500, 700}, {460, 1310}, {360, 640}, {300, 1390}, {250, 595} };

        public float[,] spanishFreqRefs = new float[5,2] { { 286, 2147}, {458, 1814}, {638, 1353}, {460, 1019}, {322, 992} };

        public VowelFormants(float f1, float f2)
        {
            this.f1 = f1;
            this.f2 = f2;
            vowel = getVowel();
            spanishVowel = getSpanishVowel();
        }

        public int getSpanishVowel()
        {
            int vowel = 0;

            float minDistance = float.MaxValue;

            for (int i = 0; i < 5; i++)
            {
                float distance = getSpanishDistance(i);

                var debug = string.Format("minDistance = {0}, Distance = {1}\n", minDistance, distance);
                var h = string.Format("f1 = {0},f2 = {1}\n", f1, f2);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    vowel = i;
                }
            }

            return vowel;
        }

        public float getSpanishDistance(int i)
        {
            float distance = 0;

            distance = (float)Math.Sqrt(Math.Pow((this.f1 - spanishFreqRefs[i, 0]), 2) + Math.Pow((this.f2 - spanishFreqRefs[i, 1]), 2));

            return distance;
        }

        public float[] getFreqs(int i)
        {
            float[] freqs = new float[2];

            freqs[0] = freqRefs[i, 0];
            freqs[1] = freqRefs[i, 1];

            return freqs;
        }

        public string spanishVowelRepresentation()
        {
            string rep = null;

            switch (spanishVowel)
            {
                case 0:
                    rep = "i";
                    break;
                case 1:
                    rep = "e";
                    break;
                case 2:
                    rep = "a";
                    break;
                case 3:
                    rep = "o";
                    break;
                case 4:
                    rep = "u";
                    break;
            }
            return rep;
        }

        public string vowelRepresentation()
        {
            string rep = null;

            switch (vowel)
            {
                case 0:
                    rep = "i";
                    break;
                case 1:
                    rep = "y";
                    break;
                case 2:
                    rep = "e";
                    break;
                case 3:
                    rep = "ø";
                    break;
                case 4:
                    rep = "ε";
                    break;
                case 5:
                    rep = "œ";
                    break;
                case 6:
                    rep = "a";
                    break;
                case 7:
                    rep = "ɶ";
                    break;
                case 8:
                    rep = "α";
                    break;
                case 9:
                    rep = "ɒ";
                    break;
                case 10:
                    rep = "ʌ";
                    break;
                case 11:
                    rep = "ɔ";
                    break;
                case 12:
                    rep = "ɤ";
                    break;
                case 13:
                    rep = "o";
                    break;
                case 14:
                    rep = "ɯ";
                    break;
                case 15:
                    rep = "u";
                    break;
            }

            return rep;

        }

        public int getVowel()
        {
            int vowel = 0;

            float minDistance = float.MaxValue;

            for (int i=0;i<16;i++)
            {
                float distance = getDistance(i);

                var debug = string.Format("minDistance = {0}, Distance = {1}\n", minDistance, distance);
                var h = string.Format("f1 = {0},f2 = {1}\n",f1,f2);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    vowel = i;
                }
            }

            return vowel;
        }

        public float getDistance(int i)
        {
            float distance = 0;

            distance = (float)Math.Sqrt(Math.Pow((this.f1-freqRefs[i,0]),2) + Math.Pow((this.f2-freqRefs[i,1]),2));
            
            return distance;
        }

    }
}
