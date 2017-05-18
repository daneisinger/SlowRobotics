using Rhino.Geometry;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using SlowRobotics.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.GraphTools
{
    /// <summary>
    /// Contains a number of static utility methods for modifying and adding to graphs
    /// </summary>
    public static class GraphUtils
    {
        /// <summary>
        /// Interconnects all nodes in a graph. New springs are given "brace" tag.
        /// </summary>
        /// <param name="graph">Graph to interconnect</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        public static void interconnectNodes(Graph<SRParticle,Spring> graph, float stiffness)
        {
            foreach(SRParticle p in graph.Geometry)
            {
                foreach(SRParticle pp in graph.Geometry)
                {
                    if (pp!= p)
                    {
                        Spring brace = new Spring(p, pp);
                        brace.tag = "brace";
                        brace.s = stiffness;
                        graph.insert(brace);
                    }
                }
            }
        }

        /// <summary>
        /// Connects nodes in a graph by proximity.
        /// </summary>
        /// <param name="graph">Graph to add to</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        /// <param name="minDist">Minimum distance for connections</param>
        /// <param name="maxDist">Maximum distance for connections</param>
        /// <param name="tag">Tag for new springs</param>
        public static void createProximateSprings(Graph<SRParticle, Spring> graph, float stiffness, float minDist, float maxDist, string tag)
        {
            List<Vec3D> pts = graph.Geometry.ConvertAll(x => (Vec3D)x);
            AABB bb = AABB.getBoundingBox(pts);
            Plane3DOctree tree = new Plane3DOctree(bb.getMin(), bb.getExtent().magnitude()*2);
            tree.AddAll(pts);

            foreach (SRParticle p in graph.Geometry)
            {
                foreach (Vec3D v in tree.Search(p, maxDist))
                {
                    if (v.distanceTo(p) > minDist)
                    {
                        Spring s = new Spring(p, (SRParticle)v);
                        s.s = stiffness;
                        s.tag = tag;
                        graph.insert(s);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates over node collection and creates a spring for every n nodes. Works well for linear graph topologies.
        /// New springs are assigned the brace tag.
        /// </summary>
        /// <param name="graph">Graph to add to</param>
        /// <param name="n">number of nodes between connection nodes</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        public static void connectNthNodes(Graph<SRParticle,Spring> graph, int n, float stiffness)
        {
            for (int i = 0; i < graph.Nodes.Count - n - 1; i += n)
            {
                INode<SRParticle> a;
                INode<SRParticle> b;
                graph.getNodeAt(i, out a);
                graph.getNodeAt(i + n, out b);
                Spring s = new Spring(a, b);
                s.s = stiffness;
                s.tag = "brace";
                graph.insert(s);
            }
        }
        /// <summary>
        /// Iterates over edge collection and creates springs that span neighbours by iterating outwards from each
        /// edge by a specified degree. Degree 1 will create spans between neighbouring edges. Degree 2 creates spans between edges 
        /// that are 1 edge apart etc.
        /// </summary>
        /// <param name="graph">Graph to add to</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        /// <param name="lengthRatio">Scale factor for rest length of new springs</param>
        /// <param name="degree">Span degree</param>
        public static void spanSprings(Graph<SRParticle, Spring> graph, float stiffness, float lengthRatio, int degree)
        {
            List<Spring> bucket = new List<Spring>();

            for (int dim = 1; dim <= degree; dim++)
            {
                if (graph.Edges.Count > dim)
                {
                    for (int i = dim; i < graph.Edges.Count; i += dim)
                    {
                        Spring a = graph.Edges[i - dim];
                        Spring b = graph.Edges[i];
                        Spring s = new Spring(a.a, b.b);
                        s.s = stiffness;
                        float len = graph.Edges.GetRange(i - dim, dim + 1).Sum(spr => spr.l);
                        s.l = len * lengthRatio;
                        s.tag = "brace";
                        bucket.Add(s);
                    }
                }
            }
            foreach (Spring s in bucket) graph.insert(s);
        }

        /// <summary>
        /// Iterates over nodes in a graph and uses a star pathfinding to create springs to nearby nodes.
        /// </summary>
        /// <param name="graph">Graph to add to</param>
        /// <param name="n">Tries to create a spring to node n nodes away from current node</param>
        /// <param name="stiffness">Stiffness of new springs</param>
        public  static void connectNthNodesWithLength(Graph<SRParticle, Spring> graph, int n, float stiffness)
        {
            List<Spring> toAdd = new List<Spring>();

            for (int i = 0; i < graph.Nodes.Count - n; i += 1)
            {
                INode<SRParticle> a;
                INode<SRParticle> b;

                graph.getNodeAt(i, out a);
                graph.getNodeAt(i + n, out b);

                Spring s = new Spring(a, b);
                s.s = stiffness;

                AStar<SRParticle> astar = new AStar<SRParticle>(a, b);

                float l = 0;

                IEnumerable<INode<SRParticle>> nodes = astar.GetPath();
                List<IEdge<SRParticle>> springs = graph.getContainedEdges(nodes);

                foreach (Spring ss in springs)
                {
                    l += ss.l;
                }

                s.l = l;
                s.tag = "brace";
                toAdd.Add(s);

                foreach (INode<SRParticle> oldn in graph.Nodes)
                {
                    oldn.SetClosedList(false);
                    oldn.SetOpenList(false);
                    oldn.parent = null;
                    oldn.Cost = 0;
                }

            }

            foreach (Spring s in toAdd) graph.insert(s);
        }

        /// <summary>
        /// Merges nodes in a graph within a given tolerance.
        /// </summary>
        /// <param name="graph">Graph to modify </param>
        /// <param name="tolerance">Merge tolerance</param>
        public static void mergeNodes (Graph<SRParticle,Spring> graph, float tolerance)
        {
            Dictionary<string, SRParticle> bins = new Dictionary<string, SRParticle>(); //bins to keep track of duplicates within a threshold

            foreach (SRParticle p in graph.Geometry)
            {
                string key = makeSpatialKey(p, tolerance);
                SRParticle a;
                if (!bins.TryGetValue(key, out a)) bins.Add(key, p);
                if (a != null)
                {
                    graph.mergeInto(p, a);
                }
            }
        }

        public static string makeSpatialKey(Vec3D pt, float binSize)
        {
            return ("" +
                (Math.Round(pt.x / binSize) * binSize) + "," +
                (Math.Round(pt.y / binSize) * binSize) + "," +
                (Math.Round(pt.z / binSize) * binSize));
        }

    }
}
