using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Rhino.GraphTools
{
    public class Dijkstra<T>
    {

        private Dictionary<INode<T>, INode<T>> previous;
        private List<INode<T>> stack;

        private IEnumerable<INode<T>> allNodes;

        public Dijkstra(IEnumerable<INode<T>> _allnodes)
        {
            allNodes = _allnodes;
        }

        public void reset()
        {
            previous = new Dictionary<INode<T>, INode<T>>();
            stack = new List<INode<T>>();

            foreach (INode<T> n in allNodes)
            {
                n.Cost = int.MaxValue;
                stack.Add(n); //copy node
                previous.Add(n, null);
            }
        }

        public void set_distances(IEnumerable<INode<T>> start, int maxSearch)
        {
            reset();
            PriorityQueue<INode<T>> nodes = new PriorityQueue<INode<T>>();

            foreach (INode<T> n in start)
            {
                n.Cost = 0;
                nodes.Enqueue(n);
            }

            int steps = 0;

            while (stack.Count() != 0)
            {
                steps++;
                
                if (steps>maxSearch || nodes.Count() == 0 ) break; //stop if reached max steps or no more nodes to search

                    INode<T> smallest = nodes.Take();
                    stack.Remove(smallest);

                    if (smallest.Cost == int.MaxValue)
                    {
                        break;
                    }

                    foreach (INode<T> neighbour in smallest.Neighbours)
                    {
                        var alt = smallest.Cost + 1; //distance cost always 1
                        if (alt < neighbour.Cost)
                        {
                            neighbour.Cost = alt;
                            previous[neighbour] = smallest;
                            nodes.Enqueue(neighbour);
                        }
                    }
            }
        }

        /*
        public List<INode<T>> shortest_path(INode<T> start, INode<T> finish)
        {

            reset();

            List<INode<T>> path = null;

            distances[start] = 0;

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<INode<T>>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (INode<T> neighbour in smallest.Neighbours)
                {
                    var alt = distances[smallest] + 1; //distance always same for now
                    if (alt < distances[neighbour])
                    {
                        distances[neighbour] = alt;
                        previous[neighbour] = smallest;
                    }
                }
            }

            return path;
        }
        */
    }
}
