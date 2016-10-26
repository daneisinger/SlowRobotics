using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class ByteGrid : VoxelGridT<Byte>
    {

        public ByteGrid(int _w, int _h, int _d, float[] _min, float[] _max) : base(_w,_h,_d,_min, _max)
        {
            setAll(0);
        }

        /// <summary>
        /// Average voxel values with its 6 axis aligned neighbours
        /// </summary>
        public override void blur()
        {
            Function((int x, int y, int z) =>
            {
                int sum = 0;
                int ctr = 0;

                foreach (byte v in getAxisNeighbours(x, y, z))
                {
                    if (v >= 0)
                    {
                        sum += v;
                        ctr++;
                    }
                }
                return (byte) (sum / ctr);

            });
        }
    }
}
