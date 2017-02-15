using Rhino.Geometry;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
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
            tree.addAll(pts);

            foreach (SRParticle p in graph.Geometry)
            {
                List<Vec3D> neighbours = tree.getPointsWithinSphere(p, maxDist);

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

        /*
        public static List<LegacyNode> mergeDuplicatesByCoordinates(List<Point3d> pts, float distToMergeDuplicates)
        {
            List<LegacyNode> unique = new List<LegacyNode>();

            foreach(Point3d p in Point3d.SortAndCullPointList(pts, distToMergeDuplicates))
            {
                unique.Add(new LegacyNode((float)p.X, (float)p.Y, (float)p.Z));
            }
            return unique;
        }
        */

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


        /*
        public static List<LegacyNode> buildFromLines(List<Line> lines, float distToMergeDuplicates)
        {
            Dictionary<string, LegacyNode> bins = new Dictionary<string, LegacyNode>(); //bins to keep track of duplicates within a threshold

            foreach(Line l in lines)
            {
                string startKey = makeSpatialKey(l.From, distToMergeDuplicates);
                string endKey = makeSpatialKey(l.To, distToMergeDuplicates);
                LegacyNode a, b;

                //set a and b to nodes that already exist if possible
                if (!bins.TryGetValue(startKey, out a))
                {
                    a = new LegacyNode((float)l.FromX, (float)l.FromY, (float)l.FromZ);
                    bins.Add(startKey, a);
                }
                if (!bins.TryGetValue(endKey, out b))
                {
                    b = new LegacyNode((float)l.ToX, (float)l.ToY, (float)l.ToZ);
                    bins.Add(endKey, b);
                }

                //add link to a and b
                LegacyLink connection = new LegacyLink(a, b);
                a.connect(connection);
                b.connect(connection);
            }

            return bins.Values.ToList<LegacyNode>();
        }


        public static void marchNodes(LegacyNode current, ref List<LegacyNode> used)
        {

            List<LegacyNode> childNodes = current.getConnectedNodes().Except(used).ToList();
            used.AddRange(childNodes);

            foreach (LegacyNode n in childNodes)
            {
                marchNodes(n, ref used);
            }

        }

        public static void parentUnique(List<LegacyNode> nodes)
        {
            List<LegacyNode> remaining = new List<LegacyNode>(nodes);
            List<LegacyNode> used = new List<LegacyNode>();

            LegacyNode current = remaining[0];
            used.Add(current);
            marchNodes(current, ref used);
            
            foreach (LegacyNode n in used)
                {
                     n.parent = current;
                     remaining.Remove(n);
                }
            
          if (remaining.Count<nodes.Count() && remaining.Count > 1) parentUnique(remaining);
            
        }
        */



    }


   
}
