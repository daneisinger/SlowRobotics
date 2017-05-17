using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Removes all but closest point in neighbour collection 
    /// </summary>
    public class FilterClosest : ScaledBehaviour<Vec3D>
    {
        public int num { get; set; }

        public FilterClosest(int priority, int _num) : base(priority)
        {
            num = _num;
        }

        public override void run(IAgent<object> a)
        {
            //check if a has neighbours
            if (a.hasNeighbours())
            {
                //check if a is Vec3D
                Vec3D pos = a.getData() as Vec3D;
                if (pos != null)
                {
                    //sort neighbour list by distance to a and then take first numClosest
                    a.neighbours.Sort(delegate (Vec3D x, Vec3D y)
                    {
                        return x.sub(pos).CompareTo(y.sub(pos));
                    });
                    a.neighbours = a.neighbours.Take(num).ToList();
                }
            }
        }

        /*
        //legacy function
        public override void run(IAgent<object> agent)
        {

            Vec3D pos = agent.getData() as Vec3D;
            if(pos!= null) { 

            var neighbours = agent.neighbours;

                if (neighbours.Count > 0)
                {

                    double cd = Double.MaxValue;
                    Vec3D closest = null;
                    foreach (Vec3D nn in neighbours)
                    {
                        var dist = nn.distanceToSquared(pos);
                        if (dist < cd)
                        {
                            cd = dist;
                            closest = nn;
                        }
                    }
                    if (closest != null)
                    {
                        agent.neighbours.Clear();
                        agent.neighbours.Add(closest);
                    }
                }
            }
        }*/
    }

    /// <summary>
    /// Removes all neighbouring points with the same parent object as the agent
    /// </summary>
    public class FilterParents : ScaledBehaviour<SRParticle>
    {
        public FilterParents(int priority) : base(priority)
        {
        }

        public override void run(IAgent<object> agent)
        {
            agent.neighbours = agent.neighbours.Where(n => ((SRParticle)n).parent != ((SRParticle)agent.getData()).parent).ToList();
        }
    }
}
