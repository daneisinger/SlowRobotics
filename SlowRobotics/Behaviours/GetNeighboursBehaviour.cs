using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;

namespace SlowRobotics.Behaviours
{
    public class GetNeighboursBehaviour : AgentBehaviour
    {

        float radius;
        /// <summary>
        /// Finds neighbours within a given radius
        /// </summary>
        /// <param name="_radius">search radius from current agent position</param>
        /// <param name="_priority"></param>
        public GetNeighboursBehaviour(float _radius, int _priority) : base(_priority)
        {
            radius = _radius;
        }

        override
        public void run(Agent a)
        {
            a.neighbours = a.world.getDynamicPoints(a,radius);
        }
    }
}
