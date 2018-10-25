using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    public class Root
    {
        public Complex z { get; set; }
        public float angle { get; set; }
        public float bandwidth { get; set; }
        public float ratio { get; set; }

        public Root(Complex z, float sampleRate)
        {
            this.z = z;
            getAngle(sampleRate);
            getBandwidth(sampleRate);
            getRatio();
        }

        public static Root getRoot(Complex z, float sampleRate)
        {
            return new Root(z, sampleRate);
        }

        private void getRatio()
        {
            ratio = bandwidth / angle;
        }

        private void getBandwidth(float sampleRate)
        {
            float measure = (float)Math.Sqrt(Math.Pow(z.Re, 2) + Math.Pow(z.Im, 2));

            bandwidth = -0.5f * (sampleRate / (float)(2 * Math.PI)) * (float)Math.Log(measure, Math.E);

            
        }

        private void getAngle(float sampleRate)
        {
            angle = (float)Math.Atan2(z.Im, z.Re);

            angle *= (sampleRate) / (float)(2 * Math.PI);
        }
    }
}
