using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    public interface IVowel
    {
        Root z1 { get; set; }

        Root z2 { get; set; }

        Vowel vowel { get; }
    }
}
