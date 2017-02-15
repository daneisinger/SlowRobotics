using Rhino;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.IO
{
    public static class IO
    {

        //TODO - clean up parenting thing, implemement fast proximity connections between nodes that aren't joined.

        public static List<AgentT<SRParticle>> ConvertCurveToGraph(Curve c, int res, float stiffness, ref Graph<SRParticle, Spring> lm)
        {
            double[] pts = c.DivideByCount(res, true);
            return ConvertCurveToGraph(c, pts, stiffness, ref lm);
        }

        public static List<AgentT<SRParticle>> ConvertCurveToGraph(Curve c, float stiffness, ref Graph<SRParticle, Spring> lm)
        {

            Interval dom = c.Domain;
            List<double> pts = new List<double>();
            double t = dom[0];
            Continuity cont = Continuity.C1_locus_continuous;
            while (true) {
                double result;
                if (!c.GetNextDiscontinuity(cont, t, dom[1], out result)) break;
                t = result;
                pts.Add(t);
            }

            return ConvertCurveToGraph(c, pts.ToArray(), stiffness, ref lm);
        }

        public static List<AgentT<SRParticle>> ConvertCurveToGraph(Curve c, double[] pts, float stiffness, ref Graph<SRParticle,Spring> lm)
        {
            Plane startPlane;
            c.FrameAt(0, out startPlane);
            SRParticle p1 = new SRParticle(ToPlane3D(startPlane));
            AgentT<SRParticle> a = new AgentT<SRParticle>(p1);
            lm.parent = p1;

            p1.parent = lm.parent; //parent the particle

            List<AgentT<SRParticle>> agents = new List<AgentT<SRParticle>>();
            agents.Add(a);
            

            INode<SRParticle> parent = null;

            for (int i = 0; i < pts.Length; i++)
            {
                Plane currentPlane;
                c.FrameAt(pts[i], out currentPlane);
                SRParticle p2 = new SRParticle(ToPlane3D(currentPlane));
                p2.parent = lm.parent; // parent the particle
                AgentT<SRParticle> b = new AgentT<SRParticle>(p2);
                

                agents.Add(b);
                Spring s = new Spring(a.getData(), b.getData());

                if (parent == null) parent = s.a;

                s.a.parent = parent;
                s.b.parent = parent;

                s.s = stiffness;
                lm.insert(s);
                a = b;
            }

            return agents;
        }
       
        public static List<IAgent> ConvertMeshToGraph(Mesh m, float stiffness, out Graph<SRParticle,Spring> lm)
        {

            
            List<AgentT<SRParticle>> agents = new List<AgentT<SRParticle>>();
            SRParticle p1 = new SRParticle(new Plane3D(ToVec3D(m.TopologyVertices[0])));
            AgentT<SRParticle> a = new AgentT<SRParticle>(p1);
            lm = new Graph<SRParticle,Spring>();
            lm.parent = p1;
            //not sure if I need particles parented
            agents.Add(a);

            for (int i = 1; i < m.TopologyVertices.Count; i++)
            {
                Point3f p = m.TopologyVertices[i];
                SRParticle p2 = new SRParticle(new Plane3D(ToVec3D(p)));
                AgentT<SRParticle> aa = new AgentT<SRParticle>(p2);
                agents.Add(aa);
            }

            for (int i = 0; i < m.TopologyEdges.Count; i++)
            {
                IndexPair p = m.TopologyEdges.GetTopologyVertices(i);
                Spring s = new Spring(agents[p.I].getData(), agents[p.J].getData());
                s.s = stiffness;
                lm.insert(s);
            }

            return agents.Select(x=>(IAgent)x).ToList();

        }

        public static List<Line> ToLines(Graph<SRParticle, Spring> g)
        {
            List<Line> output = new List<Line>();
            g.Edges.ForEach(
                l => output.Add(new Line(new Point3d(l.a.Geometry.x, l.a.Geometry.y, l.a.Geometry.z),
                new Point3d(l.b.Geometry.x, l.b.Geometry.y, l.b.Geometry.z)))
            );
            return output;
        }

        public static AgentT<SRParticle> ToPlaneAgent(Plane p)
        {
            SRParticle p1 = new SRParticle(ToPlane3D(p));
            return new AgentT<SRParticle>(p1);
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
