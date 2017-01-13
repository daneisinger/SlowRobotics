using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SlowRobotics.Core;
using SlowRobotics.Rhino.GraphTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.Visualisation
{
    public static class Draw
    {

        public static List<GH_ObjectWrapper> LinkedPlanes(IWorld world)
        {
            Dictionary<Node, List<Node>> map = new Dictionary<Node, List<Node>>();

            world.getPoints().ForEach(v => {
                Node node = (Node)v;
                if (map.ContainsKey(node))
                {
                    map[node].Add(node);
                }
                else {
                    map.Add(node, new List<Node>() { node });
                }

            });
            List<GH_ObjectWrapper> nodes = new List<GH_ObjectWrapper>();
            foreach (Node n in map.Keys)
            {
                //get an end pt
                Node start = null;

                foreach (Node forStart in map[n]) if (forStart.getLinks().Count == 1) start = forStart;

                if (start != null)
                {

                    List<Node> used = new List<Node>() { start };
                    GraphTools.GraphUtils.marchNodes(start, ref used);
                    nodes.Add(new GH_ObjectWrapper(used.ConvertAll(x => NodeToPlane(x))));
                }
            }
            return nodes;
        }

        public static Plane NodeToPlane(Node nn)
        {
            Point3d pt = new Point3d(nn.x, nn.y, nn.z);
            Vector3d xx = new Vector3d(nn.xx.x, nn.xx.y, nn.xx.z);
            Vector3d yy = new Vector3d(nn.yy.x, nn.yy.y, nn.yy.z);
            return new Plane(pt, xx, yy);
        }

        public static List<Line> Links(Node a)
        {
            List<Line> output = new List<Line>();
            a.getLinks().ForEach(l => {
                output.Add(new Line(l.a.x,l.a.y,l.a.z,l.b.x,l.b.y,l.b.z));
            });
            return output;
        }
    }
}
