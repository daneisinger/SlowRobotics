using Rhino;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using SlowRobotics.SRMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.IO
{

    /// <summary>
    /// Utility methods for converting rhino geometry to Nursery types.
    /// </summary>
    public static class SRConvert
    {

        
        /// <summary>
        /// Convert a curve to a graph by dividing the curve into a list of points
        /// </summary>
        /// <param name="c">Curve to convert</param>
        /// <param name="res">Number of division points</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        /// <param name="g">Graph to contain new springs</param>
        /// <returns></returns>
        public static List<Agent<SRParticle>> CurveToGraph(Curve c, int res, float stiffness, ref Graph<SRParticle, Spring> g)
        {
            double[] pts = c.DivideByCount(res, true);
            return CurveToGraph(c, pts, stiffness, ref g);
        }

        /// <summary>
        /// Convert a curve to a graph by creating springs between curve discontinuities
        /// </summary>
        /// <param name="c">Curve to convert</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        /// <param name="g">Graph to contain new springs</param>
        /// <returns></returns>
        public static List<Agent<SRParticle>> CurveToGraph(Curve c, float stiffness, ref Graph<SRParticle, Spring> g)
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

            return CurveToGraph(c, pts.ToArray(), stiffness, ref g);
        }

        /// <summary>
        /// Convert curve to a graph by creating springs between points at a list of curve parameters
        /// </summary>
        /// <param name="c">Curve to convert</param>
        /// <param name="pts">Parameters on curve to create springs between</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        /// <param name="g">Graph to contain new springs</param>
        /// <returns></returns>
        public static List<Agent<SRParticle>> CurveToGraph(Curve c, double[] pts, float stiffness, ref Graph<SRParticle,Spring> g)
        {
            Plane startPlane;
            c.FrameAt(0, out startPlane);
            SRParticle p1 = new SRParticle(startPlane.ToPlane3D());
            Agent<SRParticle> a = new Agent<SRParticle>(p1);
            g.parent = p1;

            p1.parent = g.parent; //parent the particle

            List<Agent<SRParticle>> agents = new List<Agent<SRParticle>>();
            agents.Add(a);
            

            INode<SRParticle> parent = null;

            for (int i = 0; i < pts.Length; i++)
            {
                Plane currentPlane;
                c.FrameAt(pts[i], out currentPlane);
                SRParticle p2 = new SRParticle(currentPlane.ToPlane3D());
                p2.parent = g.parent; // parent the particle
                Agent<SRParticle> b = (i==pts.Length-1 && c.IsClosed)? agents[0]:new Agent<SRParticle>(p2);

                agents.Add(b);
                Spring s = new Spring(a.getData(), b.getData());

                if (parent == null) parent = s.a;

                s.a.parent = parent;
                s.b.parent = parent;

                s.s = stiffness;
                g.insert(s);
                a = b;
            }

            return agents;
        }
       
        /// <summary>
        /// Convert a mesh to a graph by creating nodes at vertices and springs along edges
        /// </summary>
        /// <param name="m">Mesh to convert</param>
        /// <param name="stiffness">Spring stiffness</param>
        /// <returns></returns>
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
                s.a.Tag = m.TopologyVertices.MeshVertexIndices(p.I)[0].ToString();
                s.b.Tag = m.TopologyVertices.MeshVertexIndices(p.J)[0].ToString(); //set tags to mesh vertex indexes
                s.s = stiffness;
                lm.insert(s);
            }
            return lm;
        }

        private class SRParticleComparer : IEqualityComparer<SRParticle>
        {
            public SRParticleComparer()
            {
            }

            public bool Equals(SRParticle x, SRParticle y)
            {
                return (x.x == y.x && x.y == y.y && x.z == y.z);
            }

            public int GetHashCode(SRParticle x)
            {
                string s = " " + x.x + " " + x.y+" "+ x.z;
                return s.GetHashCode();
            }
        }

        /// <summary>
        /// Converts a list of connected lines to graph edges.
        /// </summary>
        /// <param name="edges">Lines to convert to edges</param>
        /// <param name="stiffness">Stiffness of springs</param>
        /// <returns></returns>
        public static Graph<SRParticle, Spring> EdgesToGraph(List<Line> edges, float stiffness)
        {
            //construct a graph using custom comparer for location checks
            Graph<SRParticle, Spring> graph = new Graph<SRParticle, Spring>(new SRParticleComparer());
            //insert all unique points
            IParticle parent = null;
            foreach (Line l in edges)
            {
                Spring s = new Spring(l.toLine3D());
                if (parent == null) parent = s.a.Geometry;
                s.a.Geometry.parent = parent;
                s.b.Geometry.parent = parent;
                s.s = stiffness;
                graph.insert(s);
            }
            return graph;
        }

    }
}
