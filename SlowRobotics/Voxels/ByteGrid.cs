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

    }
}
