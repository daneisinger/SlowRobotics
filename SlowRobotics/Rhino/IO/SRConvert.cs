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
    public static class SRConvert
    {

        //TODO - clean up parenting thing, implemement fast proximity connections between nodes that aren't joined.

        public static List<Agent<SRParticle>> CurveToGraph(Curve c, int res, float stiffness, ref Graph<SRParticle, Spring> lm)
        {
            double[] pts = c.DivideByCount(res, true);
            return CurveToGraph(c, pts, stiffness, ref lm);
        }

        public static List<Agent<SRParticle>> CurveToGraph(Curve c, float stiffness, ref Graph<SRParticle, Spring> lm)
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

            return CurveToGraph(c, pts.ToArray(), stiffness, ref lm);
        }

        public static List<Agent<SRParticle>> CurveToGraph(Curve c, double[] pts, float stiffness, ref Graph<SRParticle,Spring> lm)
        {
            Plane startPlane;
            c.FrameAt(0, out startPlane);
            SRParticle p1 = new SRParticle(startPlane.ToPlane3D());
            Agent<SRParticle> a = new Agent<SRParticle>(p1);
            lm.parent = p1;

            p1.parent = lm.parent; //parent the particle

            List<Agent<SRParticle>> agents = new List<Agent<SRParticle>>();
            agents.Add(a);
            

            INode<SRParticle> parent = null;

            for (int i = 0; i < pts.Length; i++)
            {
                Plane currentPlane;
                c.FrameAt(pts[i], out currentPlane);
                SRParticle p2 = new SRParticle(currentPlane.ToPlane3D());
                p2.parent = lm.parent; // parent the particle
                Agent<SRParticle> b = new Agent<SRParticle>(p2);
                

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
       
        public static Graph<SRParticle, Spring> MeshToGraph(Mesh m, float stiffness)
        {

            List<SRParticle> particles = new List<SRParticle>();
            SRParticle p1 = new SRParticle(new Plane3D(m.TopologyVertices[0].ToVec3D()));
            Graph<SRParticle, Spring>  lm = new Graph<SRParticle,Spring>();
            lm.parent = p1;
            particles.Add(p1);

            for (int i = 1; i < m.TopologyVertices.Count; i++)
            {
                Point3f p = m.TopologyVertices[i];
                SRParticle p2 = new SRParticle(new Plane3D(p.ToVec3D()));
                particles.Add(p2);
            }

            for (int i = 0; i < m.TopologyEdges.Count; i++)
            {
                IndexPair p = m.TopologyEdges.GetTopologyVertices(i);
                Spring s = new Spring(particles[p.I], particles[p.J]);
                s.a.Index = m.TopologyVertices.MeshVertexIndices(p.I)[0];
                s.b.Index = m.TopologyVertices.MeshVertexIndices(p.J)[0]; //set indexes to original vertices
                s.s = stiffness;
                lm.insert(s);
            }
            return lm;
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

        public static Agent<SRParticle> ToPlaneAgent(Plane p)
        {
            SRParticle p1 = new SRParticle(p.ToPlane3D());
            return new Agent<SRParticle>(p1);
        }

    }
}
