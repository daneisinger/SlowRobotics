using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Agent.Behaviours
{
    public class FilterClosest : ScaledBehaviour<SRParticle>
    {
        public FilterClosest(int priority) : base(priority)
        {
        }

        public override void run(IAgentT<object> agent)
        {
            var a = (AgentT<object>)agent;

            var particle = (SRParticle)a.getData();

            var neighbours = a.neighbours;

            if (neighbours.Count > 0)
            {

                double cd = Double.MaxValue;
                SRParticle closest = null;
                foreach (var nn in neighbours)
                {
                    var neighbour = (SRParticle)nn;

                    var dist = neighbour.distanceToSquared(particle);
                    if (dist < cd)
                    {
                        cd = dist;
                        closest = neighbour;
                    }
                }
                if (closest != null)
                {
                    a.neighbours.Clear();
                    a.neighbours.Add(closest);
                }
            }
        }
    }

    public class FilterParents : ScaledBehaviour<SRParticle>
    {
        public FilterParents(int priority) : base(priority)
        {
        }

        public override void run(IAgentT<object> agent)
        {
            var a = (AgentT<object>)agent;
            a.neighbours = a.neighbours.Where(n => ((SRParticle)n).parent != ((SRParticle)a.getData()).parent).ToList();
        }
    }
}
