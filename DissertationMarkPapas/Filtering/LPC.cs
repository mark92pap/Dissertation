using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace DissertationMarkPapas
{
    public class LPC
    {
        public int length { get; set; }
        public float sampleRate { get; set; }
        public float[] autocorrelation { get; set; } = null;
        public float[] coefficients { get; set; }
        public float[] fullCoefficients { get; set; }
        public Polynomial poly { get; set; }
        public Root[] roots { get; set; }
        public Root[] finalRoots { get; set; } = null;
        public int[] rootIndexer { get; set; }

        public LPC(int LPClength, RecordingData recording)  //CONSTRUCTOR
        {
            length = LPClength;
            sampleRate = recording.sampleRate;

            autocorrelation = new float[length + 1];

            coefficients = new float[length];

            fullCoefficients = new float[length + 1];

            getAutocorrelation(recording.preprocessedData);

            getLPC();

            getPolynomial();

            getRoots();

            getFinalRoots();

            fillRootIndexer();

        }

        public LPC(int LPClength, SynthesizedData synthesized)
        {
            length = LPClength;
            sampleRate = synthesized.previousCoeffs.sampleRate;

            autocorrelation = new float[length + 1];

            coefficients = new float[length];

            fullCoefficients = new float[length + 1];

            getAutocorrelation(synthesized.synthesizedData);

            getLPC();

            getPolynomial();

            getRoots();

            getFinalRoots();

            fillRootIndexer();
        }

        public LPC(LPC previousCoeffs, VowelUtteredPD vowel)
        {
            length = previousCoeffs.length;
            sampleRate = previousCoeffs.sampleRate;


            getNewRoots(previousCoeffs, vowel);

            getPolyFromRoots();
            getCoeffsFromPoly();
        }

        private void getNewRoots(LPC previousCoeffs, VowelUtteredPD vowel)
        {
            roots = previousCoeffs.roots;

            roots[previousCoeffs.rootIndexer[0]] = Root.getRoot(vowel.targetZ1, sampleRate);    //Z1
            roots[previousCoeffs.rootIndexer[1]] = Root.getRoot(vowel.targetZ2, sampleRate);    //Z2

            roots[previousCoeffs.rootIndexer[2]] = Root.getRoot(Complex.Conj(vowel.targetZ1), sampleRate);    //Z1 CONJ
            roots[previousCoeffs.rootIndexer[3]] = Root.getRoot(Complex.Conj(vowel.targetZ2), sampleRate);    //Z2 CONJ

        }

        private void fillRootIndexer()
        {

            Debug.WriteLine($"finalRoots.length = {finalRoots.Length}");

            rootIndexer = new int[4];

            if (finalRoots.Length<2)
            {
                return;
            }

            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].z == finalRoots[0].z)  //IS F1
                {
                    rootIndexer[0] = i;
                }
                else if (Complex.Conj(roots[i].z) == finalRoots[0].z) //IS CONJ(F1)
                {
                    rootIndexer[2] = i;
                }
                else if (roots[i].z == finalRoots[1].z) //IS F2
                {
                    rootIndexer[1] = i;
                }
                else if (Complex.Conj(roots[i].z) == finalRoots[1].z) //IS CONJ(F2)
                {
                    rootIndexer[3] = i;
                }
            }
        }

        private void getCoeffsFromPoly()
        {
            Complex[] coeffs = poly.Coefficients;
            coefficients = new float[coeffs.Length-1];
            fullCoefficients = new float[coeffs.Length];

            for (int i=0;i<coeffs.Length-1;i++)
            {
                coefficients[i] = (float)coeffs[i].Re;
                fullCoefficients[i] = (float)coeffs[i].Re;
            }

            fullCoefficients[coeffs.Length - 1] = (float)coeffs[coeffs.Length - 1].Re;

            Array.Reverse(fullCoefficients);
        }

        private void getPolyFromRoots()
        {
            Polynomial[] rootPoly = new Polynomial[roots.Length];

            for (int i = 0; i < roots.Length; i++)
            {
                Complex[] coeffs = new Complex[2];
                coeffs[0] = -roots[i].z;

                coeffs[1] = new Complex(1, 0);
                rootPoly[i] = new Polynomial(coeffs);

            }

            poly = rootPoly[0];

            for (int i = 1; i < rootPoly.Length; i++)
            {
                poly = poly * rootPoly[i];
            }
        }

        private void getPolynomial()
        {
            double[] coeffs = new double[coefficients.Length + 1];

            coeffs[0] = 1;

            for (int i=0;i<coefficients.Length;i++)
            {
                coeffs[i + 1] = -coefficients[i];
            }

            Array.Reverse(coeffs);  //1 must be on coeffs[maxIndex]

            poly = new Polynomial(coeffs);
        }

        private void getAutocorrelation(float[] arr)
        {
            int xLength = autocorrelation.Length;
            int aLength = arr.Length;

            float max = -1;

            for (int k = 0; k < xLength; k++)
            {
                autocorrelation[k] = 0;

                for (int m = 0; m < aLength - k; m++)   //last value is N-k-1
                {
                    autocorrelation[k] += arr[m] * arr[m + k];
                }

                if (autocorrelation[k] > max)
                {
                    max = autocorrelation[k];
                }
            }

            //normalize

            for (int i = 0; i < xLength; i++)
            {
                autocorrelation[i] = autocorrelation[i] / max;
            }
        }

        private void getLPC()
        {
            int xLength = autocorrelation.Length;
            int matrixLength = xLength - 1;

            float[,] autocMatrix = new float[matrixLength, matrixLength]; //need values R(0:lpcLength-1)
            float[] rVector = new float[matrixLength];

            for (int i = 0; i < matrixLength; i++)
            {
                autocMatrix[0, i] = autocorrelation[i];   //fill first row and column
                autocMatrix[i, 0] = autocorrelation[i];

                rVector[i] = autocorrelation[i + 1];

            }

            for (int i = 1; i < matrixLength; i++)
            {
                for (int j = 1; j < matrixLength; j++)
                {
                    autocMatrix[i, j] = autocMatrix[i - 1, j - 1]; //each other element is equal to the previous element in the diagonal
                }
            }  //FILLED

            float[] LPC = new float[matrixLength];

            getCoeffs(rVector, autocMatrix);  //done
        }

        private void getCoeffs(float[] autocorrelation, float[,] autocMatrix)
        {
            //calculate
            int rows = autocMatrix.GetLength(0);
            int cols = autocMatrix.GetLength(1);
            float[,] temp = new float[rows, cols];

            //COPY TO TEMP
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    temp[i, j] = autocMatrix[i, j];
                }

            }

            Matrix<float> A = Matrix<float>.Build.DenseOfArray(temp);

            Vector<float> V = Vector<float>.Build.DenseOfArray(autocorrelation);

            Vector<float> solution = A.Solve(V);

            V = solution;

            fullCoefficients[0] = 1;

            for (int i = 0; i < solution.Count; i++)
            {
                coefficients[i] = solution[i];
                fullCoefficients[i + 1] = -solution[i];
            }
        }

        private void getRoots()
        {

            double[,] A = new double[length, length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i == j + 1)
                    {
                        A[i, j] = 1;
                    }
                    else
                    {
                        A[i, j] = 0;
                    }
                }
            }

            for (int j = 0; j < length; j++)
            {
                A[0, j] = coefficients[j]; //is already normalized
            }

            Matrix<double> B = Matrix<double>.Build.DenseOfArray(A);

            MathNet.Numerics.LinearAlgebra.Factorization.Evd<double> eigen = B.Evd();

            Vector<System.Numerics.Complex> eigenvector = eigen.EigenValues;

            roots = new Root[eigenvector.Count];

            for (int i = 0; i < eigenvector.Count; i++)
            {
                Complex temp = new Complex(eigenvector[i].Real, eigenvector[i].Imaginary);
                roots[i] = new Root(temp,sampleRate);
            }

        }

        private void getFinalRoots()    //NEEDS WORK
        {
            float[] anglesCopy = new float[roots.Length];

            for (int i=0;i<roots.Length;i++)
            {
                anglesCopy[i] = roots[i].angle;
            }


            Array.Sort(anglesCopy, roots);  //roots and angles have the same INDEX and are sorted according to angles

            int rootCounter = 0;



            for (int i = 0; i < roots.Length; i++)
            {
                if ((roots[i].z.Im > 0) && (roots[i].ratio < 0.15) && (roots[i].angle < 3000))
                {
                    Debug.WriteLine($"OKAY \t \t angle[{i}] = {roots[i].angle} \t bandwidths[{i}]] = {roots[i].bandwidth} \t ratio[{i}] = {roots[i].ratio}");
                    rootCounter++;
                }

                else if ((roots[i].z.Im > 0) && (roots[i].angle < 3000))
                {
                    Debug.WriteLine($"NOT OKAY \t angle[{i}] = {roots[i].angle} \t bandwidths[{i}]] = {roots[i].bandwidth} \t ratio[{i}] = {roots[i].ratio}");
                }

            }

            Debug.WriteLine("\n");
            finalRoots = new Root[rootCounter];


            int indexer = 0;


            for (int i = 0; i < roots.Length; i++)
            {
                if ((roots[i].z.Im > 0) && (roots[i].ratio < 0.15) && (roots[i].angle < 3000))
                {
                    finalRoots[indexer] = roots[i];
                    indexer++;
                }
            }
        }
    }
}
