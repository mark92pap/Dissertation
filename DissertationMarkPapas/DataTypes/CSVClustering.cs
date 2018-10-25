using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    public class CSVClustering
    {
        public StringBuilder csv { get; set; }
        public string fileName { get; set; }

        public CSVClustering(string file)
        {
            fileName = file;
            csv = new StringBuilder();
        }

        public void appendRecording(int recNum, int numBins, int cat, int v, float[,] f)
        {
            for (int binNum = 0; binNum < numBins; binNum++)
            {
                var line = string.Format("{0},{1},{2},{3},{4},{5}", cat, v, recNum, binNum, f[binNum, 0], f[binNum, 1]);
                csv.AppendLine(line);
            }
        }

        public void finalize()
        {
            File.WriteAllText(fileName, csv.ToString());
        }
    }
}
