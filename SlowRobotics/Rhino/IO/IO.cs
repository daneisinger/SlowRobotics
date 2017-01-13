using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using SlowRobotics.Agent;
using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Agent.Types;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.IO
{
    public static class IO
    {
        public static List<ParticleAgent> ConvertCurveToLinkMesh(Curve c, int res, float stiffness, out Graph lm)
        {
            Plane startPlane;
            c.FrameAt(0, out startPlane);
            Core.Particle p1 = new Core.Particle(ToPlane3D(startPlane));
            ParticleAgent a = new ParticleAgent(p1);
            lm = new Graph(p1);
            p1.parent = lm.parent;

            List<ParticleAgent> agents = new List<ParticleAgent>();
            agents.Add(a);

            double[] pts = c.DivideByCount(res, true);

            for (int i = 1; i < pts.Length; i++)
            {
                Plane currentPlane;
                c.FrameAt(pts[i], out currentPlane);
                Core.Particle p2 = new Core.Particle(ToPlane3D(currentPlane));
                ParticleAgent b = new ParticleAgent(p2);
                p2.parent = lm.parent;

                agents.Add(b);
                lm.connectNodes(a.getData(), b.getData(), stiffness);
                a = b;
            }

            return agents;
        }

        public static List<IAgent> ConvertMeshToLinkMesh(Mesh m, float stiffness, out Graph lm)
        {
            List<ParticleAgent> agents = new List<ParticleAgent>();
            Core.Particle p1 = new Core.Particle(new Plane3D(ToVec3D(m.TopologyVertices[0])));
            ParticleAgent a = new ParticleAgent(p1);
            lm = new Graph(p1);
            p1.parent = lm.parent;
            agents.Add(a);

            for (int i = 1; i < m.TopologyVertices.Count; i++)
            {
                Point3f p = m.TopologyVertices[i];
                Core.Particle p2 = new Core.Particle(new Plane3D(ToVec3D(p)));
                ParticleAgent aa = new ParticleAgent(p2);
                p2.parent = lm.parent;
                agents.Add(aa);
            }

            for (int i = 0; i < m.TopologyEdges.Count; i++)
            {
                IndexPair p = m.TopologyEdges.GetTopologyVertices(i);
                lm.connectNodes(agents[p.I].getData(), agents[p.J].getData(), stiffness);
            }

            return agents.Select(x=>(IAgent)x).ToList();

        }

        public static ParticleAgent ToPlaneAgent(Plane p)
        {
            Core.Particle p1 = new Core.Particle(ToPlane3D(p));
            return new ParticleAgent(p1);
        }

        public static Plane3D ToPlane3D(Plane p)
        {
            return new Plane3D(new Vec3D((float)p.Origin.X, (float)p.Origin.Y, (float)p.Origin.Z),
              new Vec3D((float)p.XAxis.X, (float)p.XAxis.Y, (float)p.XAxis.Z),
              new Vec3D((float)p.YAxis.X, (float)p.YAxis.Y, (float)p.YAxis.Z));
        }

        public static Vec3D ToVec3D(Point3f p)
        {
            return new Vec3D(p.X,p.Y,p.Z);
        }

        public static Vec3D ToVec3D(Vector3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static Vec3D ToVec3D(Point3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static AABB ToAABB(Box b)
        {
            Vec3D min = ToVec3D(b.BoundingBox.Min);
            Vec3D max = ToVec3D(b.BoundingBox.Max);
            return AABB.fromMinMax(min,max);
        }

        public static Point3d ToPoint3d(Vec3D p)
        {
            return new Point3d(p.x, p.y, p.z);
        }
        public static Plane ToPlane(Plane3D p)
        {
            Point3d origin = new Point3d(p.x, p.y, p.z);
            Vector3d xx = new Vector3d(p.xx.x, p.xx.y, p.xx.z);
            Vector3d yy = new Vector3d(p.yy.x, p.yy.y, p.yy.z);

            return new Plane(origin, xx, yy);
        }
       
    }
}
