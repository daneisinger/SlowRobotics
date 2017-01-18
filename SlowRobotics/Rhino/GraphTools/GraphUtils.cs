using Rhino.Geometry;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.GraphTools
{
    public static class GraphUtils
    {
        /*
        public static List<Line> getConnectionsAsLines(LegacyNode n)
        {
            List<Line> output = new List<Line>();
            n.getLinks().ForEach(
                l => output.Add(new Line(new Point3d(l.a.x, l.a.y, l.a.z), new Point3d(l.b.x, l.b.y, l.b.z)))
            );
            return output;
        }

        public static List<LegacyNode> mergeDuplicatesByCoordinates(List<Point3d> pts, float distToMergeDuplicates)
        {
            List<LegacyNode> unique = new List<LegacyNode>();

            foreach(Point3d p in Point3d.SortAndCullPointList(pts, distToMergeDuplicates))
            {
                unique.Add(new LegacyNode((float)p.X, (float)p.Y, (float)p.Z));
            }
            return unique;
        }

        public static List<LegacyNode> mergeDuplicates (List<Point3d> pts, float distToMergeDuplicates)
        {
            Dictionary<string, LegacyNode> bins = new Dictionary<string, LegacyNode>(); //bins to keep track of duplicates within a threshold

            foreach (Point3d p in pts)
            {
                string key = makeSpatialKey(p, distToMergeDuplicates);
                LegacyNode a;
                //set a and b to nodes that already exist if possible
                if (!bins.TryGetValue(key, out a))
                {
                    a = new LegacyNode((float)p.X, (float)p.Y, (float)p.Z);
                    bins.Add(key, a);
                }
            }

            return bins.Values.ToList<LegacyNode>();
        }

        public static void LinkByProximity(IWorld world, int maxConnections, float minDist, float maxDist)
        {
            List<LegacyNode> graph = new List<LegacyNode>();
            foreach(LegacyNode n in world.getAgents())
            {

                List<Vec3D> neighbours = world.search(n, maxDist,0);
                neighbours.AddRange(world.search(n, maxDist,1));
                foreach (Vec3D v in neighbours){
                    if(v.distanceTo(n)> minDist)
                    {
                        //create link
                    }
                }
            }
        }
        

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


        public static string makeSpatialKey(Point3d pt, float binSize)
        {
            return(""+
                (Math.Round(pt.X / binSize) * binSize)+","+
                (Math.Round(pt.Y / binSize) * binSize)+"," +
                (Math.Round(pt.Z / binSize) * binSize));
        }
        */
    }
}
