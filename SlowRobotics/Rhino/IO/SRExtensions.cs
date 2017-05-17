using Rhino.Geometry;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.IO
{
    public static class SRExtensions
    {

        public static Plane ToPlane(this Plane3D p)
        {
            Point3d origin = new Point3d(p.x, p.y, p.z);
            Vector3d xx = new Vector3d(p.xx.x, p.xx.y, p.xx.z);
            Vector3d yy = new Vector3d(p.yy.x, p.yy.y, p.yy.z);
            return new Plane(origin, xx, yy);
        }

        public static Plane3D ToPlane3D(this Plane p)
        {
            return new Plane3D(new Vec3D((float)p.Origin.X, (float)p.Origin.Y, (float)p.Origin.Z),
                         new Vec3D((float)p.XAxis.X, (float)p.XAxis.Y, (float)p.XAxis.Z),
                         new Vec3D((float)p.YAxis.X, (float)p.YAxis.Y, (float)p.YAxis.Z));
        }

        public static Line toLine(this SRLinearParticle l)
        {
            return new Line(l.start.ToPoint3d(), l.end.ToPoint3d());
        }

        public static Line toLine(this Spring s)
        {
            return new Line(s.start.ToPoint3d(), s.end.ToPoint3d());
        }

        public static Line toLine(this ILine l)
        {
            return new Line(l.start.ToPoint3d(), l.end.ToPoint3d());
        }

        public static Vec3D ToVec3D(this Point3f p)
        {
            return new Vec3D(p.X, p.Y, p.Z);
        }

        public static Vec3D ToVec3D(this Vector3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static Vec3D ToVec3D(this Point3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static AABB ToAABB(this Box b)
        {
            Vec3D min = ToVec3D(b.BoundingBox.Min);
            Vec3D max = ToVec3D(b.BoundingBox.Max);
            return AABB.fromMinMax(min, max);
        }

        public static Point3d ToPoint3d(this Vec3D p)
        {
            return new Point3d(p.x, p.y, p.z);
        }

        public static Vector3d ToVector3d(this Vec3D p)
        {
            return new Vector3d(p.x, p.y, p.z);
        }
    }
}
