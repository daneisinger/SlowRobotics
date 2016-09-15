using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class FloatGrid : VoxelGrid<float>
    {

        /// <summary>
        /// Constructs a float grid and initialises all values to 0
        /// </summary>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="_d"></param>
        /// <param name="_min"></param>
        /// <param name="_max"></param>
        public FloatGrid(int _w, int _h, int _d, float[] _min, float[] _max) : base(_w,_h,_d,_min, _max)
        {
            initGrid(0);
        }

        /// <summary>
        /// Initialise FloatGrid with random vals between 0 and max
        /// </summary>
        /// <param name="max">Max range for random value</param>
        public void initRandom(float max)
        {
            System.Random r = new System.Random();
            vals = new Voxel[w * h * d];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int index = i + w * (j + h * k);
                        float v = (float)Math.Round(r.NextDouble() * 0.6);
                        vals[index].Data = v * max;
                    }
                }
            }
        }

        /// <summary>
        /// Set voxel values within a specified radius from a point in world space
        /// </summary>
        /// <param name="x">X coordinate of point </param>
        /// <param name="y">Y coordinate of point </param>
        /// <param name="z">Z coordinate of point </param>
        /// <param name="val">new voxel value</param>
        /// <param name="rad">max distance of voxel from point in world space</param>
        public void setWithRadius(float x, float y, float z, float val, float rad)
        {
            int[] pt = map(x, y, z);
            int radV = (int)(rad / ((max[0] - min[0]) / w));
            int bs = radV * radV;
            for (int i = -radV; i <= radV; i++)
            {
                for (int j = -radV; j <= radV; j++)
                {
                    for (int k = -radV; k <= radV; k++)
                    {
                        int ls = (i * i) + (j * j) + (k * k);
                        if (ls < bs) //sphere
                        {
                            float v = val * (1 - ((float)ls / bs));
                            if (getValue(pt[0] + i, pt[1] + j, pt[2] + k) < v)
                            {
                                setGridValue(pt[0] + i, pt[1] + j, pt[2] + k, v);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Blur all voxels
        /// </summary>
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

        /// <summary>
        /// Blur the value of a voxel using neighbours
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
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
                            float val = vals[index].Data;
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
                vals[x + w * (y + h * z)].Data = weightedAverage;
            }
        }

    }
}
