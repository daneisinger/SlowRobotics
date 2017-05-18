using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRobotics.Rhino.MeshTools
{
    /// <summary>
    /// Convenience class that represents a mesh face as a collection of points.
    /// </summary>
    public struct MFace
    {
        public Point3f v1, v2, v3, v4;
        public Color c;
        public bool quad;

        public MFace(Point3f _v1, Point3f _v2, Point3f _v3, Point3f _v4, Color _c)
        {
            v1 = _v1;
            v2 = _v2;
            v3 = _v3;
            v4 = _v4;
            quad = true;
            c = _c;
        }
        public MFace(Point3f _v1, Point3f _v2, Point3f _v3, Color _c)
        {
            v1 = _v1;
            v2 = _v2;
            v3 = _v3;
            v4 = Point3f.Unset;
            quad = false;
            c = _c;
        }

    }
}
