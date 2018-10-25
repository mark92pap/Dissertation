using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    class ClusteringFile
    {
        public string clusterFolder { get; set; }
        public string folderNow { get; set; }
        public string fileName { get; set; }

        public string completePath { get; set; }

        public ClusteringFile(string parentFolder)
        {
            clusterFolder = parentFolder;
        }

        public void setCurrentFolder(string folder)
        {
            folderNow = folder;
        }

        public void setFileName(string file)
        {
            fileName = file;
            computeCompletePath();
        }


        public void setFilename(int cat, int rec, int recSub, string[] vowels, int v)
        {
            string clusterFile = string.Empty;

            if (cat == 0)
            {
                clusterFile = "AVPEPUDEAC00";
            }
            else
            {
                clusterFile = "AVPEPUDEA00";
            }
            if (rec < 10)
            {
                clusterFile += "0" + rec + vowels[v] + recSub + ".wav";   //need to add one more 0
            }
            else
            {
                clusterFile += rec + vowels[v] + recSub + ".wav";
            }

            fileName = clusterFile;

            computeCompletePath();
        }

        private void computeCompletePath()
        {
            //Debug.WriteLine($"clusterFolder = {clusterFolder} \t folderNow = {folderNow} \t fileName = {fileName}");
            completePath = clusterFolder + folderNow + fileName;
        }
    }
}
