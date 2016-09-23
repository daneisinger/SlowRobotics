using Rhino.Geometry;
using SlowRobotics.Rhino.MeshTools;
using SlowRobotics.Voxels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Rhino.VoxelTools
{

    //TODO - set this up with inheretance, too much overlap

    public class ColourGridMesher
    {
        //buffers
        public ConcurrentBag<MFace> buffer;
        public ColourGrid v;

        public ColourGridMesher(ColourGrid _voxelData)
        {
            v = _voxelData;
        }
        public Mesh buildMesh()
        {
            return Mesher.buildMesh(buffer);
        }

        //TODO - fix issue with non uniform grids

        public void run(Color targetColor, float tolerance, int inset)
        {

            int res = 1;
            buffer = new ConcurrentBag<MFace>();
            //loop through voxel data and draw faces on threshold
            Parallel.For(inset, v.d-inset, z =>
            {

                for (int y = v.h-inset-1; y >= inset; y--)
                {
                    for (int x = inset; x < v.w-inset; x++)
                    {
                        //see if there are neighbouring values if not create the faces
                        ColourVoxel val = v.getValue(x, y, z);

                        if (v.compareColours(targetColor, val, tolerance))
                        {
                            //posBuffer.Add(new Point3f(x, y, z));

                            if (!v.compareColours(targetColor, v.getValue((x - res), y, z), tolerance))
                            {
                                addFace(val, x, y, z, x, y, z + res, x, y + res, z + res, x, y + res, z, 0);
                            }
                            if (!v.compareColours(targetColor, v.getValue((x + res), y, z), tolerance))
                            {
                                addFace(val, x + res, y, z, x + res, y, z + res, x + res, y + res, z + res, x + res, y + res, z, 1);
                            }
                            if (!v.compareColours(targetColor, v.getValue(x, (y - res), z), tolerance))
                            {
                                addFace(val, x, y, z, x, y, z + res, x + res, y, z + res, x + res, y, z, 1);
                            }
                            if (!v.compareColours(targetColor, v.getValue(x, (y + res), z), tolerance))
                            {
                                addFace(val, x, y + res, z, x, y + res, z + res, x + res, y + res, z + res, x + res, y + res, z, 0);
                            }
                            if (!v.compareColours(targetColor, v.getValue(x, y, (z - res)), tolerance))
                            {
                                addFace(val, x, y, z, x, y + res, z, x + res, y + res, z, x + res, y, z, 0);
                            }
                            if (!v.compareColours(targetColor, v.getValue(x, y, (z + res)), tolerance))
                            {
                                addFace(val, x, y, z + res, x, y + res, z + res, x + res, y + res, z + res, x + res, y, z + res, 1);
                            }
                        }
                    }
                }
            });
        }

        private void addFace(ColourVoxel val, float ax, float ay, float az, float bx, float by, float bz, float cx, float cy, float cz, float dx, float dy, float dz, int clockwise)
        {
            //first tri
            Point3f a = new Point3f(ax, ay, az);
            Point3f b = new Point3f(bx, by, bz);
            Point3f d = new Point3f(dx, dy, dz);
            Point3f c = new Point3f(cx, cy, cz);
            if (clockwise == 0)
            {
                buffer.Add(new MFace(a, b, d, Color.FromArgb(val.R, val.G, val.B)));
                buffer.Add(new MFace(d, b, c, Color.FromArgb(val.R, val.G, val.B)));
            }
            else
            {
                buffer.Add(new MFace(d, b, a, Color.FromArgb(val.R, val.G, val.B)));
                buffer.Add(new MFace(c, b, d, Color.FromArgb(val.R, val.G, val.B)));
            }


        }

    }
}
