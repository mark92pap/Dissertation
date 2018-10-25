using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    class VowelUtteredHC : IVowel
    {
        

        public Vowel vowel { get; set; }
        public Root z1 { get; set; }
        public Root z2 { get; set; }

        public VowelUtteredHC(Root root1, Root root2, Vowel v)
        {
            z1 = root1;
            z2 = root2;
            vowel = v;
        }
    }
}
