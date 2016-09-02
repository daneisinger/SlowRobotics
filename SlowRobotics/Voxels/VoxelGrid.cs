using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    /// <summary>
    /// Generic voxel class
    /// </summary>
    /// <typeparam name="T">Voxel data type</typeparam>
    public abstract class VoxelGrid<T>
    {
        public Voxel[] vals;      // The array of bytes containing the pixels.
        public int w, h, d;
        public float[] min;
        public float[] max;

        // Nested class is also generic on T

        //TODO - overload operators to finish this up as a generic type

        public class Voxel
        {
            private T data;  //T as private member datatype

            public Voxel(T t)  //T used in non-generic constructor
            {
                data = t;
            }

            public T Data  //T as return type of property
            {
                get { return data; }
                set { data = value; }
            }

        }


        public VoxelGrid(int _w, int _h, int _d, float[] _min, float[] _max)
        {
            w = _w;
            h = _h;
            d = _d;
            min = _min;
            max = _max;
        }
        
        /// <summary>
        /// Sets values by referencing array
        /// </summary>
        /// <param name="_vals"></param>
        public void setVals(Voxel[] _vals)
        {
            vals = _vals;
        }

        /// <summary>
        /// Returns a shallow copy of all voxel objects
        /// </summary>
        /// <returns></returns>
        public Voxel[] copyVals()
        {
            Voxel[] data = new Voxel[w * h * d];
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

        /// <summary>
        /// Sets value of all voxels
        /// </summary>
        /// <param name="t">Value </param>
        public void initGrid(T t)
        {
            vals = new Voxel[w * h * d];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        int index = i + w * (j + h * k);
                        vals[index] = new Voxel(t);
                    }
                }
            }
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

            mapped[0] = (int)((x - min[0]) / ((max[0]-min[0])/w));
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
            mapped[1] = (float)((y  * ((max[1] - min[1]) / h) + min[1]));
            mapped[2] = (float)((z  * ((max[2] - min[2]) / d) + min[2]));
            return mapped;
        }

        /// <summary>
        /// Sets value of a voxel at specified point in world space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="val"></param>
        public void setValue(float x, float y, float z, T val)
        {
            int[] pt = map(x, y, z);
            setGridValue(pt[0], pt[1], pt[2], val);
        }
        
        /// <summary>
        /// Sets value of a voxel at a specified grid index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="val"></param>
        public void setGridValue(int x, int y, int z, T val)
        {
            if (x >= 0 && x < w && y >= 0 && y < h && z >= 0 && z < d)
            {
                int index = (x) + w * ((y) + h * (z));
                set(index, val);
            }
        }
        

        private void set(int index, T val)
        {
            //vals[index].set(val);
            vals[index].Data = val;
        }

        /// <summary>
        /// Returns value of voxel at specified grid index, default if out of bounds
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public T getValue(int x, int y, int z)
        {
           if (x > w-1 || x < 0 || y < 0 || y > h-1 || z < 0 || z > d-1)
            {
                return default(T);
            }
            else
            {
                int index = x + w * (y + h * z);
                return vals[index].Data;
            }
        }

        /// <summary>
        /// Returns the voxel at a specified index in the grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Voxel get(int x, int y, int z)
        {
            if (x > w - 1 || x < 0 || y < 0 || y > h - 1 || z < 0 || z > d - 1)
            {
                return null;
            }
            else
            {
                int index = x + w * (y + h * z);
                return vals[index];
            }
        }
    }
}