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
        bool dynamicOnly;
        /// <summary>
        /// Finds neighbours within a given radius
        /// </summary>
        /// <param name="_radius">search radius from current agent position</param>
        /// <param name="_priority"></param>
        public GetNeighboursBehaviour(float _radius, int _priority) : this(_radius, _priority, true) { }


        public GetNeighboursBehaviour(float _radius, int _priority, bool _dynamicOnly) : base(_priority)
        {
            radius = _radius;
            dynamicOnly = _dynamicOnly;
        }

        override
        public void run(Agent a)
        {
            a.neighbours = a.world.getDynamicPoints(a,radius);
            if(!dynamicOnly)a.neighbours.AddRange(a.world.getStaticPoints(a, radius));
        }
    }
}
