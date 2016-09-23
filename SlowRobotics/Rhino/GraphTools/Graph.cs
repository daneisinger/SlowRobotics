using Rhino.Geometry;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Rhino.GraphTools
{
    public class Graph
    {

        public static List<Line> getConnectionsAsLines(Node n)
        {
            List<Line> output = new List<Line>();
            n.getLinks().ForEach(
                l => output.Add(new Line(new Point3d(l.a.x, l.a.y, l.a.z), new Point3d(l.b.x, l.b.y, l.b.z)))
            );
            return output;
        }

        public static List<Node> mergeDuplicates (List<Point3d> pts, float distToMergeDuplicates)
        {
            Dictionary<string, Node> bins = new Dictionary<string, Node>(); //bins to keep track of duplicates within a threshold

            foreach (Point3d p in pts)
            {
                string key = makeSpatialKey(p, distToMergeDuplicates);
                Node a;
                //set a and b to nodes that already exist if possible
                if (!bins.TryGetValue(key, out a))
                {
                    a = new Node((float)p.X, (float)p.Y, (float)p.Z);
                    bins.Add(key, a);
                }
            }

            return bins.Values.ToList<Node>();
        }

        public static List<Node> buildFromLines(List<Line> lines, float distToMergeDuplicates)
        {
            Dictionary<string, Node> bins = new Dictionary<string, Node>(); //bins to keep track of duplicates within a threshold

            foreach(Line l in lines)
            {
                string startKey = makeSpatialKey(l.From, distToMergeDuplicates);
                string endKey = makeSpatialKey(l.To, distToMergeDuplicates);
                Node a, b;

                //set a and b to nodes that already exist if possible
                if (!bins.TryGetValue(startKey, out a))
                {
                    a = new Node((float)l.FromX, (float)l.FromY, (float)l.FromZ);
                    bins.Add(startKey, a);
                }
                if (!bins.TryGetValue(endKey, out b))
                {
                    b = new Node((float)l.ToX, (float)l.ToY, (float)l.ToZ);
                    bins.Add(endKey, b);
                }

                //add link to a and b
                Link connection = new Link(a, b);
                a.connect(connection);
                b.connect(connection);
            }

            return bins.Values.ToList<Node>();
        }

        public static string makeSpatialKey(Point3d pt, float binSize)
        {
            return(""+
                (Math.Round(pt.X / binSize) * binSize)+","+
                (Math.Round(pt.Y / binSize) * binSize)+"," +
                (Math.Round(pt.Z / binSize) * binSize));
        }
    }
}
