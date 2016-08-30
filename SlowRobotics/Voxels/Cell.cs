using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class Cell
    {
        float val;
        public int x, y, z;

        public Cell(float v, int _x, int _y, int _z)
        {
            val = v;
            x = _x;
            y = _y;
            z = _z;
        }

        public void set(float v) { val = v; }
        public float get() { return val; }
    }
}
