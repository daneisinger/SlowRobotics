using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class FloatGrid : VoxelGridT<float>
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
            setAll(0);
        }

        /// <summary>
        /// Randomize grid values between 0 and max
        /// </summary>
        /// <param name="max"></param>
        public void setRandom(float max)
        {
            System.Random r = new System.Random();
            Function(() => (float)(r.NextDouble() * max));
        }


        public new void booleanSphere(int x, int y, int z, int rad, float val, Filter filter)
        {
            int radSqr = rad * rad;
            setRegion(x, y, z, val, rad, (int i, int j, int k, float dVal) =>
            {
                int distSqr = (i * i) + (j * j) + (k * k);
                float v = val * (1 - ((float)distSqr / radSqr));
                if (distSqr < radSqr && filter(val, getValue(x + i, y + j, z + k))) //sphere
                {
                    set(x + i, y + j, z + k, v);
                    return true;
                }
                return false;

            });
        }

        /// <summary>
        /// Average voxel values with its 6 axis aligned neighbours
        /// </summary>
        public void blur()
        {
            Function((int x, int y, int z) =>
            {
                float sum = 0;
                int ctr = 0;
                
                foreach(float v in getAxisNeighbours(x,y,z))
                {
                    if (v >= 0)
                    {
                        sum += v;
                        ctr++;
                    }
                }
                return sum/ctr;

            });
            }

        /// <summary>
        /// Blur all voxels
        /// </summary>
        public void scalarBlur()
        {
            Function((int x, int y, int z) => {
                float sum = 0;
                int ctr = 0;
                setRegion(x, y, z, 0, 1, (int i, int j, int k, float dVal) =>
                {
                    float val = getValue(i+x,j+y,k+z);
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
                    if (val >= 0)
                    {
                        sum += (val * scalar);
                        ctr += scalar;
                    }
                    return true;
                });
                return (float)(sum / ctr);
              });
        }

    }
}
