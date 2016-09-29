using Rhino.Geometry;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.Visualisation
{
    public static class Draw
    {

        public static List<Line> Links(Node a)
        {
            List<Line> output = new List<Line>();
            a.getLinks().ForEach(l => {
                output.Add(new Line(l.a.x,l.a.y,l.a.z,l.b.x,l.b.y,l.b.z));
            });
            return output;
        }

        public static void Pairs(Node a, out List<Polyline> geom, out List<float> targetAngles)
        {
            List<Polyline> output = new List<Polyline>();
            List<float> angles = new List<float>();
            a.getPairs().ForEach(p => {

                Vec3D aa = p.a.tryGetOther(a);
                Vec3D ab = p.b.tryGetOther(a);
                angles.Add(p.angle);

                output.Add(new Polyline(new List<Point3d>() {
                    new Point3d(aa.x,aa.y,aa.z),
                    new Point3d(a.x,a.y,a.z),
                    new Point3d(ab.x,ab.y,ab.z)
                }));

            });
            geom = output;
            targetAngles = angles;

        }
    }
}
