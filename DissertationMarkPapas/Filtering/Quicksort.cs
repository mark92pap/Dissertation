using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationMarkPapas
{
    class Quicksort
    {

        public static void QuickSortMethod(float[] fInput)
        {

            QuickSortNow(fInput, 0, fInput.Length - 1);
            /*
            for (int i = 0; i < fInput.Length; i++)
            {
                Console.Write(fInput[i] + " ");
            }

            Console.ReadLine();
            */
        }

        public static void QuickSortNow(float[] fInput, int start, int end)
        {
            if (start < end)
            {
                int pivot = Partition(fInput, start, end);
                QuickSortNow(fInput, start, pivot - 1);
                QuickSortNow(fInput, pivot + 1, end);
            }
        }

        public static int Partition(float[] fInput, int start, int end)
        {
            float pivot = fInput[end];
            int pIndex = start;

            for (int i = start; i < end; i++)
            {
                if (fInput[i] <= pivot)
                {
                    float temp = fInput[i];
                    fInput[i] = fInput[pIndex];
                    fInput[pIndex] = temp;
                    pIndex++;
                }
            }

            float anotherTemp = fInput[pIndex];
            fInput[pIndex] = fInput[end];
            fInput[end] = anotherTemp;
            return pIndex;
        }
    }

}

