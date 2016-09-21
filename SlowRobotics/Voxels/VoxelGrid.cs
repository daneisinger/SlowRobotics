using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class VoxelGrid
    {

        public int w, h, d;

        private readonly int _nxy, _n;
        private readonly double _nxInv, _nxyInv; 

        public float[] min;
        public float[] max;

        public VoxelGrid(int _w, int _h, int _d, float[] _min, float[] _max)
        {
            w = _w;
            h = _h;
            d = _d;
            min = _min;
            max = _max;

            _nxy = w * h;
            _n = _nxy * d;

            _nxInv = 1.0 / w;
            _nxyInv = 1.0 / _nxy;
        }

        public int Count
        {
            get { return _n; }
        }

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

        /// <summary>
        /// Map from world space to voxel space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int[] map(float x, float y, float z)
        {

            int[] mapped = new int[3];

            mapped[0] = (int)((x - min[0]) / ((max[0] - min[0]) / w));
            mapped[1] = (int)((y - min[1]) / ((max[1] - min[1]) / h));
            mapped[2] = (int)((z - min[2]) / ((max[2] - min[2]) / d));
            return mapped;
        }

        /// <summary>
        /// Map from voxel space to world space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float[] mapWorld(int x, int y, int z)
        {

            float[] mapped = new float[3];
            mapped[0] = (float)((x * ((max[0] - min[0]) / w) + min[0]));
            mapped[1] = (float)((y * ((max[1] - min[1]) / h) + min[1]));
            mapped[2] = (float)((z * ((max[2] - min[2]) / d) + min[2]));
            return mapped;
        }

        /// <summary>
        /// get the index at a coordinate in the grid
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int indexAt(int i, int j, int k)
        {
            int index = i + w * (j + h * k);
            return  (i >= 0 && i < w && j >= 0 && j < h && k >= 0 && k < d)? index :-1;
        }

        /// <summary>
        /// get grid position of index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void posAt(int index, out int i, out int j, out int k)
        {
            k = (int)(index * _nxyInv);
            i = index - k * _nxy; // store remainder in i
            j = (int)(i * _nxInv);
            i -= j * w;
        }


    }
}
