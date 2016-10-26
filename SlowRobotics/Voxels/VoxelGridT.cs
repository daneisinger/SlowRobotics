using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Voxels
{

    //TODO - need to make this more generic to work with GH. 
    //could have a voxel class?
    //or could accept a list of data?
    //need an interface to figure out what the base functions are that are needed
    //(e.g. testing for values when reading/writing, averageing values etc)

    /// <summary>
    /// Generic voxel class
    /// </summary>
    /// <typeparam name="T">Voxel data type</typeparam>
    /// 
    public abstract class VoxelGridT<T> : VoxelGrid, IVoxelGrid where T : struct, IComparable
    {
        private Voxel[] vals;      // The array of data

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

            //TODO - rethink these generics. seems like spatial slur's generic functions are more flexible
            public static Voxel operator +(Voxel i, Voxel j)
            {
                return new Voxel(Sum(i.data, j.data));
            }

            private static T Sum(T a, T b)
            {
                return (dynamic)a + (dynamic)b;
            }

            public static Voxel operator -(Voxel i, Voxel j)
            {
                return new Voxel(Sub(i.data, j.data));
            }

            private static T Sub(T a, T b)
            {
                return (dynamic)a - (dynamic)b;
            }

            public static Voxel operator *(Voxel i, Voxel j)
            {
                return new Voxel(Mult(i.data, j.data));
            }

            private static T Mult(T a, T b)
            {
                return (dynamic)a * (dynamic)b;
            }

        }

        public VoxelGridT(int _w, int _h, int _d, float[] _min, float[] _max) : base(_w, _h, _d, _min, _max)
        {
            vals = new Voxel[w * h * d];
        }

        #region Generic Functions

        //TODO - Make these all parallel

        /// <summary>
        /// set all voxel values to some function
        /// </summary>
        /// <param name="func"></param>
        public void Function(Func<T> func)
        {
            /*
            Parallel.For(0, count(), i =>
            {
                vals[i] = new Voxel(func());
            });*/
            for(int i = 0; i < Count; i++) 
            {
                vals[i] = new Voxel(func());
            }
        }

        /// <summary>
        /// set all voxel values to some function of the voxel index
        /// </summary>
        /// <param name="func"></param>
        public void Function(Func<int, T> func)
        {
            /*
            Parallel.For(0, count(), i =>
             {
                 vals[i] = new Voxel(func(i));
             });
             */
            for (int i = 0; i < Count; i++)
            {
                vals[i] = new Voxel(func(i));
            }
        }

        /// <summary>
        /// set all voxel values to some function of the voxel grid position 
        /// </summary>
        /// <param name="func"></param>
        public void Function(Func<int, int, int, T> func)
        {
            /*
             for (int x = 0; x < w; x++)
             { 
             //Parallel.For(0, w, x => {
                     for (int y = 0; y < h; y++)
                     {
                         for (int z = 0; z < d; z++)
                         {
                             vals[indexAt(x,y,z)]=new Voxel(func(x, y, z));
                         }
                     }
             }*/
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                int i, j, k;
                for (int index = range.Item1; index < range.Item2; index++)
                {
                    posAt(index, out i, out j, out k);
                    vals[index] = new Voxel(func(i,j,k));
                }
            });
        }

        /// <summary>
        /// Runs a function on voxels within a region
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="val"></param>
        /// <param name="rad"></param>
        /// <param name="setterFunction"></param>
        public void setRegion(int x, int y, int z, T val, int rad, Func<int, int, int, T, bool> setterFunction)
        {
            for (int i = -rad; i <= rad; i++)
            {
                for (int j = -rad; j <=rad; j++)
                {
                    for (int k = -rad; k <=rad; k++)
                    {
                        setterFunction(i, j, k, val);
                    }
                }
            }
        }

        public virtual void blur() { }
        #endregion


        #region Get and Set Voxel Regions


        public delegate bool Filter(T lhs, T rhs);

        public static bool Greater(T lhs, T rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool Less(T lhs, T rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool Equal(T lhs, T rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        /*
        public void booleanSphere(float x, float y, float z, float rad, T val, Filter filter)
        {
            int[] pt = map(x, y, z);
            int gridRadius = (int)(rad / ((max[0] - min[0]) / w));
            setSolidSphere(pt[0], pt[1], pt[2], gridRadius, val, filter);
        }*/

        public void drawSphere(int x, int y, int z, int rad, T val)
        {
            int radSqr = rad * rad;
            setRegion(x, y, z, val, rad, (int i, int j, int k, T dVal) =>
            {
                int distSqr = (i * i) + (j * j) + (k * k);
                int index = indexAt(x + i, y + j, z + k);
                if (distSqr < radSqr) //sphere
                {
                    set(index, dVal);
                    return true;
                }
                return false;

            });
        }
        /// <summary>
        /// Sets voxels within a radius of a point to some value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rad"></param>
        /// <param name="val"></param>
        public void booleanSphere(int x, int y, int z, int rad, T val, Filter filter)
        {
            int radSqr = rad * rad;
            setRegion(x, y, z, val, rad, (int i, int j, int k, T dVal) =>
              {
                  int distSqr = (i * i) + (j * j) + (k * k);
                  int index = indexAt(x + i, y + j, z + k);
                  if (distSqr < radSqr && filter(val, getValue(index))) //sphere
                {
                      set(index, dVal);
                      return true;
                  }
                  return false;

              });
        }

        public List<Voxel> getRegion(Vec3D min, Vec3D max)
        {
            List<Voxel> n = new List<Voxel>();
            for (int x = (int)min.x; x <= (int)max.x; x++)
            {
                for (int y = (int)min.y; y <= (int)max.y; y++)
                {
                    for (int z = (int)min.z; z<= (int)max.z; z++)
                    {
                        n.Add(get(x, y, z));
                    }
                }
            }
            return n;
        }

        public List<Voxel> getRegion(int[] pt, int rad)
        {
            return getRegion(pt[0], pt[1], pt[2], rad);
        }

        /// <summary>
        /// Gets a list of voxels within a region
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rad"></param>
        /// <returns></returns>
        public List<Voxel> getRegion(int x, int y, int z, int rad)
        {
            List<Voxel> n = new List<Voxel>();
            for (int i = x - rad; i < x + rad; i++)
            {
                for (int j = y - rad; j < y + rad; j++)
                {
                    for (int k = z - rad; k < z + rad; k++)
                    {
                        n.Add(get(i, j, k));
                    }
                }
            }
            return n;
        }


        public Voxel[] getAxisNeighbourVoxels(int x, int y, int z)
        {
            return new Voxel[] {
                    get(x + 1, y, z), get(x - 1, y, z),
                    get(x, y + 1, z), get(x, y - 1, z),
                    get(x, y, z + 1), get(x, y, z - 1)
                };
        }


        public T[] getAxisNeighbours(int x, int y, int z)
        {
            return new T[] {
                    getValue(x + 1, y, z), getValue(x - 1, y, z),
                    getValue(x, y + 1, z), getValue(x, y - 1, z),
                    getValue(x, y, z + 1), getValue(x, y, z - 1)
                };
        }

        /// <summary>
        /// Returns a shallow copy of all voxel objects
        /// </summary>
        /// <returns></returns>
        public Voxel[] duplicate()
        {
            Voxel[] data = new Voxel[w * h * d];
            Parallel.For(0, Count, i =>
            {
                data[i] = vals[i];
            });
            return data;
        }

        /// <summary>
        /// Sets values to T
        /// </summary>
        /// <param name="t">Value </param>
        public void setAll(T t)
        {
            Function(() => t);
        }

        /// <summary>
        /// Sets values by referencing array
        /// </summary>
        /// <param name="_vals"></param>
        public void setAll(Voxel[] _vals)
        {
            vals = _vals;
        }
        #endregion

        #region Get and set Voxels

        /// <summary>
        /// Sets value of a voxel at specified point in world space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="val"></param>
        public void setNearest(float x, float y, float z, T val)
        {
            int[] pt = map(x, y, z);
            set(pt[0], pt[1], pt[2], val);
        }
        /// <summary>
        /// Sets value of a voxel at a specified grid index
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="val"></param>
        public void set(int x, int y, int z, T val)
        {
                int index = indexAt(x, y, z);
                set(index, val);
        }


        public void set(int index, T val)
        {
            if (index>=0 && index <= Count) vals[index] = new Voxel(val);
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
            return getValue(indexAt(x, y, z));
        }

        private T getValue(int index)
        {
            Voxel v = get(index);
            return (v != null) ? v.Data : default(T);
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
            int index = indexAt(x, y, z);
            return get(index);
        }

        /// <summary>
        /// Gets voxel at index if it exists
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Voxel get(int index)
        {
            return (index >= 0) ? vals[index] : null;
        }
        #endregion

    }
}