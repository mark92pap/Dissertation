using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    class CSVCreator
    {
        public StringBuilder text { get; set; }

        public string filename { get; set; }

        public CSVCreator(string filename)
        {
            this.filename = filename;
            text = new StringBuilder();
        }

        public void AppendLine(int currentBin, LPC previousLPC, LPC finalLPC, VowelUtteredPD vowel)
        {
            if (double.IsNaN(finalLPC.finalRoots[0].z.Re))
            {
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", currentBin, previousLPC.finalRoots[0].angle, previousLPC.finalRoots[1].angle, vowel.vowel, "NaN", "NaN");
                text.AppendLine(newLine);
            }                                                       //BIN,          preF1,                          preF2,                          //preVowel, afterF1, afterF2
            else
            {
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", currentBin, previousLPC.finalRoots[0].angle, previousLPC.finalRoots[1].angle, vowel.vowel, finalLPC.finalRoots[0].angle, finalLPC.finalRoots[1].angle);
                text.AppendLine(newLine);



                Debug.WriteLine($"Previous F1 = {previousLPC.finalRoots[0].angle} \t Previous F2 = {previousLPC.finalRoots[1].angle}");
                Debug.WriteLine($"New F1 = {finalLPC.finalRoots[0].angle} \t New F2 = {finalLPC.finalRoots[1].angle}");
                Debug.WriteLine($"Vowel = {vowel.vowel}\n");
            }
        }

        public void writeFile()
        {
            Debug.WriteLine(filename);
            string path = "files/" + filename;
            Debug.WriteLine(path);
            System.IO.File.WriteAllText(path, text.ToString());
        }
    }
}
