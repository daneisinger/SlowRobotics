using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class VoxelGrid
    {
        public float[] vals;      // The array of bytes containing the pixels.
        public int w, h, d;
        public float[] min;
        public float[] max;

        public VoxelGrid(int _w, int _h, int _d, float[] _min, float[] _max)
        {
            w = _w;
            h = _h;
            d = _d;
            min = _min;
            max = _max;
            initGrid();
        }

        public void setVals(float[] _vals)
        {
            vals = _vals;
        }

        public float[] copyVals()
        {
            float[] data = new float[w * h * d];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int index = i + w * (j + h * k);
                        data[index] = vals[index];
                    }
                }
            }
            return data;
        }
        //Initialisation Functions

        public void initGrid()
        {
            vals = new float[w * h * d];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int index = i + w * (j + h * k);
                        //  vals[index] = new Cell(0, i, j, k);
                        vals[index] = 0;
                    }
                }
            }
        }

        public void initRandom()
        {
            System.Random r = new System.Random();
            vals = new float[w * h * d];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int index = i + w * (j + h * k);
                        float v = (float)Math.Round(r.NextDouble()*0.6);
                        //vals[index] = new Cell(v, i, j, k);
                        vals[index] = v*255;
                    }
                }
            }
        }
        //Utility functions


        //constrain a value to bounds

        private float constrain(float v, float min, float max)
        {
            if (v < min)
            {
                return min;
            }
            else if (v > max)
            {
                return max;
            }
            else return v;
        }

        //map a point to voxel space
        private int[] map(float x, float y, float z)
        {


            int[] mapped = new int[3];

            mapped[0] = (int)((x - min[0]) / ((max[0]-min[0])/w));
            mapped[1] = (int)((y - min[1]) / ((max[1] - min[1]) / h));
            mapped[2] = (int)((z - min[2]) / ((max[2] - min[2]) / d));
            return mapped;
        }

        //map a voxel to world space
        public float[] mapWorld(int x, int y, int z)
        {

            //TODO - FIX!
            float[] mapped = new float[3];

            mapped[0] = (float)((x * ((max[0] - min[0]) / w) + min[0]));
            mapped[1] = (float)((y  * ((max[1] - min[1]) / h) + min[1]));
            mapped[2] = (float)((z  * ((max[2] - min[2]) / d) + min[2]));
            return mapped;
        }

        //get and set values
        public void setWithRadius(float x, float y, float z, float val, float rad)
        {
            int[] pt = map(x, y, z);
            int radV = (int)(rad / ((max[0] - min[0]) / w));
            int bs = radV*radV;
            for (int i=-radV; i<= radV; i++)
            {
                for (int j = -radV; j<= radV; j++)
                {
                    for (int k = -radV; k<= radV; k++)
                    {
                       int ls = (i * i) + (j * j) + (k * k);
                        if (ls < bs) //sphere
                        {
                            float v = val * (1 - ((float)ls / bs));
                            if (getValue(pt[0] + i, pt[1] + j, pt[2] + k)<v) { 
                            setGridValue(pt[0] + i, pt[1] + j, pt[2] + k, v);
                             }
                        }
                    }
                }
            }
        }
        public void setValue(float x, float y, float z, float val)
        {
            int[] pt = map(x, y, z);
            setGridValue(pt[0], pt[1], pt[2], val);
        }
        /**
         * Constrains cell modification to within grid. See setValue(Vec3D)
         */
        public void setGridValue(int x, int y, int z, float val)
        {
            if (x >= 0 && x < w && y >= 0 && y < h && z >= 0 && z < d)
            {
                int index = (x) + w * ((y) + h * (z));
                set(index, val);
            }
        }
        /**
         * Sets the value of a cell at a specified index
         * @param index index of cell in the voxel array
         * @param val specified value
         */
        private void set(int index, float val)
        {
            //vals[index].set(val);
            vals[index] = val;
        }

        public Cell getCell(int x, int y, int z) 
        {
		try {
                if (x > w || x < 0 || y < 0 || y > h || z < 0 || z > d)
                {
                    throw new System.IndexOutOfRangeException();
                }
                int index = x + w * (y + h * z);
                //Cell val = vals[index];
                Cell val = new Cell(-1, x, y, z);
                return val;
            } catch (Exception e) {
                return new Cell(-1, x, y, z);
            }


        }

        /**
         * Gets the value of the cell at the specified position in the voxel grid
         * @param x
         * @param y
         * @param z
         * @return value of cell
         */
        public float getValue(int x, int y, int z)
        {
           if (x > w-1 || x < 0 || y < 0 || y > h-1 || z < 0 || z > d-1)
            {
                return -1;
            }
            else
            {
                int index = x + w * (y + h * z);
                //return getCell(x, y, z).get();
                return vals[index];
            }
        }

        public void blur()
        {
            for (int z = 0; z < d; z += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    for (int x = 0; x < w; x += 1)
                    {
                        blurVoxel(x, y, z);
                    }
                }
            }
        }

        public void blurVoxel(int x, int y, int z)
        {
            if ((x > 1) && (x < w - 2) &&
                    (y > 1) && (y < h - 2) &&
                    (z > 1) && (z < d - 2))
            {
                float sum = 0;
                for (int k = -1; k <= 1; k++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            int index = (i + x) + w * ((j + y) + h * (k + z));
                            float val = vals[index];
                            int scalar = 1;
                            if (k == 0)
                            {
                                if (j * i == 0)
                                {
                                    scalar = 2;
                                }
                                if (j == 0 && i == 0)
                                {
                                    scalar = 4;
                                }
                            }
                            else if (j == 0 && i == 0)
                            {
                                scalar = 2;
                            }

                            sum += (val * scalar);
                        }
                    }
                }
                int weightedAverage = (int)(sum / 36);
                vals[x + w * (y + h * z)]= weightedAverage;
            }
        }

    }
}