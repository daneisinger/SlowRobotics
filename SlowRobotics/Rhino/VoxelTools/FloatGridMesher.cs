using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using Grasshopper;
using Rhino;
using Rhino.Geometry;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SlowRobotics.Voxels;
using SlowRobotics.Rhino.MeshTools;

namespace SlowRobotics.Rhino.VoxelTools
{
    public class FloatGridMesher
    {

        //buffers
        public ConcurrentBag<MFace> buffer;
        public VoxelGridT<float> v;

        public FloatGridMesher(VoxelGridT<float> _voxelData)
        {
            v = _voxelData;
        }
        public Mesh buildMesh()
        {
            return Mesher.buildMesh(buffer);
        }

        //TODO - fix issue with non uniform grids

        public void run(float cutoff)
        {

            int res = 1;
            buffer = new ConcurrentBag<MFace>();
            //loop through voxel data and draw faces on threshold
            Parallel.For(0, v.d, z =>
            {

                for (int y = v.h; y >= 0; y--)
                {
                    for (int x = 0; x <v.w; x++)
                    {
                        //see if there are neighbouring values if not create the faces
                        float val = v.getValue(x, y, z);
                        if (val > cutoff)
                        {
                            //posBuffer.Add(new Point3f(x, y, z));

                            if (v.getValue((x - res), y, z) <= cutoff)
                            {
                                addFace(val, x, y, z, x, y, z + res, x, y + res, z + res, x, y + res, z, 0);
                            }
                            if (v.getValue((x + res), y, z) <= cutoff)
                            {
                                addFace(val, x + res, y, z, x + res, y, z + res, x + res, y + res, z + res, x + res, y + res, z, 1);
                            }
                            if (v.getValue(x, (y - res), z) <= cutoff)
                            {
                                addFace(val, x, y, z, x, y, z + res, x + res, y, z + res, x + res, y, z, 1);
                            }
                            if (v.getValue(x, (y + res), z) <= cutoff)
                            {
                                addFace(val, x, y + res, z, x, y + res, z + res, x + res, y + res, z + res, x + res, y + res, z, 0);
                            }
                            if (v.getValue(x, y, (z - res)) <= cutoff)
                            {
                                addFace(val, x, y, z, x, y + res, z, x + res, y + res, z, x + res, y, z, 0);
                            }
                            if (v.getValue(x, y, (z + res)) <= cutoff)
                            {
                                addFace(val, x, y, z + res, x, y + res, z + res, x + res, y + res, z + res, x + res, y, z + res, 1);
                            }
                        }
                    }
                }
            });
        }

        private void addFace(float vertexColours, float ax, float ay, float az, float bx, float by, float bz, float cx, float cy, float cz, float dx, float dy, float dz, int clockwise)
        {
            //first tri
            int col = Math.Max(Math.Min((int)vertexColours, 255), 0);
            Point3f a = new Point3f(ax, ay, az);
            Point3f b = new Point3f(bx, by, bz);
            Point3f d = new Point3f(dx, dy, dz);
            Point3f c = new Point3f(cx, cy, cz);
            if (clockwise == 0)
            {
                buffer.Add(new MFace(a, b, d, Color.FromArgb(col, col, col, col)));
                buffer.Add(new MFace(d, b, c, Color.FromArgb(col, col, col, col)));
            }
            else
            {
                buffer.Add(new MFace(d, b, a, Color.FromArgb(col, col, col, col)));
                buffer.Add(new MFace(c, b, d, Color.FromArgb(col, col, col, col)));
            }


        }

    }
}
