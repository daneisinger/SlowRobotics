using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class FilterClosest : ScaledBehaviour<Vec3D>
    {
        public FilterClosest(int priority) : base(priority)
        {
        }

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
        }
    }

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
