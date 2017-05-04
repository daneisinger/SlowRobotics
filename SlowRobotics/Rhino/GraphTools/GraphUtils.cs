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
    public static class GraphUtils
    {

        public static void createProximateSprings(Graph<SRParticle, Spring> graph, float stiffness, float minDist, float maxDist, string tag)
        {
            List<Vec3D> pts = graph.Geometry.ConvertAll(x => (Vec3D)x);
            AABB bb = AABB.getBoundingBox(pts);
            Plane3DOctree tree = new Plane3DOctree(bb.getMin(), bb.getExtent().magnitude()*2);
            tree.AddAll(pts);

            foreach (SRParticle p in graph.Geometry)
            {
                List<Vec3D> neighbours = tree.Search(p, maxDist);

                foreach (Vec3D v in neighbours)
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
                graph.insert(s);
            }
        }

        public static void spanSprings(Graph<SRParticle, Spring> graph, float stiffness, float lengthRatio, int dimension)
        {
            List<Spring> bucket = new List<Spring>();

            for (int dim = 1; dim <= dimension; dim++)
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
