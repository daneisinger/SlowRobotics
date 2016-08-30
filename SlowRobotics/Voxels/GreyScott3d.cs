using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Voxels
{
    /*Implementation of the ReactP5 solver
    Copyright(c) 2011, 2014, Ioannis(Yiannis) Chatzikonstantinou, All rights reserved.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    */
    public class GreyScott3D
    {
        public int w, h, d;                 // Dimensions
        public int I, Ir, Id;               // Iterations: General, Reaction, Diffusion
        public float[][] S;                 // Substances
        public float[][] Sn;                // Next Substances
        public float[][] dRS;               // Substances diffusion rates
        public float[][] P;                 // parameter arrays
        public int[][] N;                   //neighbours

        public float f = 0.025f;
        public float k = 0.060f;
        public float du = 0.16f;
        public float dv = 0.08f;
        public int solverSteps =5;

        public GreyScott3D(int _w, int _h, int _d)
        {
            w = _w;
            h = _h;
            d = _d;
            S = createSubstancesArray(1, 0);
            Sn = createSubstancesArray(1, 0);
            P = createParametersArray(); //creating a default array
            P = setReactionParameters(0, f); //setting reaction params to globals
            P = setReactionParameters(1, k);
            dRS = createDiffusionArray();  //creating diffusion array
            dRS = setDiffusionParameters(0, du);  //setting diffusion params to globals
            dRS = setDiffusionParameters(1, dv);
            I = Ir = Id = 0;
            createNeighbourMap();
        }

        //updates the reactions
        public void step()
        {
            int parallelThreads = Environment.ProcessorCount;
            // int duration = w / parallelThreads;
            for (int i = 0; i < solverSteps; i++)
            {

                // int start = index * duration;
                Parallel.For(0, parallelThreads, t =>
                {
                    stepReaction(t, parallelThreads);
                    stepDiffusion(t, parallelThreads);
                    I++;
                    Ir++;
                    Id++;
                });
            }
        }

        //updates the cell values
        public void stepReaction(int offset, int step)
        {
            int i,j, k, p; //loop and placeholder variables
            float uVal, vVal, FVal, kVal;  //chemical and reaction rate variables
                                           //chemical - how much of each chemical is in the cell
                                           //reaction = how fast  the chemicals mix
            float[] bF = P[0]; //gets the F param array
            float[] bk = P[1]; //gets the K param array

 
            for(i=offset;i< w; i+=step) {
            for (j = 0; j < h; j++)
            {
                for (k = 0; k < d; k++)
                {
                    p = i + j * w + k * w * h;  //loops through all the cells gets the current one in the array
                    uVal = S[0][p];  //gets the current u chem value
                    vVal = S[1][p];  //gets the current v chem alue 
                    FVal = bF[p];  //gets the reaction rate - this can vary accross the simulation if you want, in this simulation its static
                    kVal = bk[p];  //ditto for k rate
                    S[0][p] = S[0][p] + FVal * (1.0f - uVal) - uVal * vVal * vVal; //greyscott reaction to update the chemical quantities
                    S[1][p] = S[1][p] - (FVal + kVal) * vVal + uVal * vVal * vVal;

                }
            }
        }
           
        }

        //updates the diffusion
        void stepDiffusion(int offset, int step)
        {
            int i, j, k;  //loop variables
            for (int n = 0; n < S.Length; n++)
            {

                for (i = offset; i < w; i+=step)
                {
                    for (j = 0; j < h; j++)
                    {
                        for (k = 0; k < d; k++)
                        {
                            int p = i + j * w + k * w * h;  //placeholder for the current cell
                            float sumN = S[n][N[0][p]] + S[n][N[1][p]]
                              + S[n][N[2][p]] + S[n][N[3][p]]
                              + S[n][N[4][p]] + S[n][N[5][p]];
                            float val = S[n][p] + dRS[n][p] * (sumN - 6 * S[n][p]);  //averages all the neighbour cell values
                            Sn[n][p] = val;
                        }
                    }
                    
                }
            }
            swap(); //this avoids a concurrent modification I think. Though I am not sure if it even really works.
        }

        // Swaps current matrix with next. Just a shortcut for convenience.
        void swap()
        {
            float[][] temp = S;
            S = Sn;
            Sn = temp;
        }

        // Creates a neighbor map for neighbor lookup acceleration.
        void createNeighbourMap()
        {
            N = new int[6][];
            for (int i = 0; i < N.Length; i++) N[i] = new int[w*h*d]; //.net initialise

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int p = i + j * w + k * w * h;  //old way - overlaps the edges - sphere topology
                                                        //          if (i == 0)     N[0][p] = p + (w-1);       else N[0][p] = p - 1;
                                                        //          if (i == w - 1) N[1][p] = p - (w-1);       else N[1][p] = p + 1;
                                                        //          if (j == 0)     N[2][p] = p + w*(h-1);     else N[2][p] = p - w;
                                                        //          if (j == h - 1) N[3][p] = p - w*(h-1);     else N[3][p] = p + w;
                                                        //          if (k == 0)     N[4][p] = p + w*h*(d-1);   else N[4][p] = p - w*h;
                                                        //          if (k == d - 1) N[5][p] = p - w*h*(d-1);   else N[5][p] = p + w*h;
                        if (i == 0) N[0][p] = p; else N[0][p] = p - 1;
                        if (i == w - 1) N[1][p] = p; else N[1][p] = p + 1;
                        if (j == 0) N[2][p] = p; else N[2][p] = p - w;
                        if (j == h - 1) N[3][p] = p; else N[3][p] = p + w;
                        if (k == 0) N[4][p] = p; else N[4][p] = p - w * h;
                        if (k == d - 1) N[5][p] = p; else N[5][p] = p + w * h;
                    }
                }
            }
        }

        //fill the arrays up with some values
        float[][] createSubstancesArray(float a, float b)
        {
            float[][] R = new float[2][];
            for (int i = 0; i < R.Length; i++) R[i] = new float[w * h * d];
            for (int i = 0; i < R[0].Length; i++)
            {
                R[0][i] = a;
                R[1][i] = b;
            }
            return R;
        }

        //create greyscott parameters - constant across the whole simulation
        float[][] createParametersArray()
        {
            float[][] R = new float[2][];
            for (int i = 0; i < R.Length; i++) R[i] = new float[w * h * d];
            for (int i = 0; i < R[0].Length; i++)
            {
                R[0][i] = f; //F
                R[1][i] = k; //k
            }
            return R;
        }
        //create initial chemical values
        float[][] createDiffusionArray()
        {
            float[][] R = new float[S.Length][];
            for (int i = 0; i < R.Length; i++) R[i] = new float[w * h * d];
            for (int i = 0; i < R[0].Length; i++)
            {
                for (int j = 0; j < R.Length; j++)
                {
                    R[j][i] = 0.01f;
                }
            }
            return R;
        }

        //specify a parameter value
        float[][] setDiffusionParameters(int index, float value)
        {
            float[][] R = new float[dRS.Length][];
            for (int i = 0; i < R.Length; i++) R[i] = new float[w * h * d];

            for (int i = 0; i < R[0].Length; i++)
            {
                for (int j = 0; j < R.Length; j++)
                {
                    if (j == index)
                    {
                        R[j][i] = value;
                    }
                    else {
                        R[j][i] = dRS[j][i];
                    }
                }
            }
            return R;
        }

        //specify a reaction value per cell
        float[][] setReactionParameters(int index, float value)
        {
            float[][] R = new float[P.Length][];
            for (int i = 0; i < R.Length; i++) R[i] = new float[w * h * d];
            for (int i = 0; i < R[0].Length; i++)
            {
                for (int j = 0; j < R.Length; j++)
                {
                    if (j == index)
                    {
                        R[j][i] = value;
                    }
                    else {
                        R[j][i] = P[j][i];
                    }
                }
            }
            return R;
        }

        public void setSolverSteps(int num)
        {
            solverSteps = num;
        }

        //gets the current chem values out for putting into the volumetricSpaceArray (for drawing and exporting)
        public float[][] getSubstances()
        {
            return S;
        }
        public float[][] getParameters()
        {
            return P;
        }
        public float[][] getDiffusionRates()
        {
            return dRS;
        }
    }

}
