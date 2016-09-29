using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.NeighbourBehaviours
{
    public class OrientToNeighbourBehaviour : NeighbourBehaviour
    {
        float maxDist;
        float orientToNeighbour;

        public OrientToNeighbourBehaviour(int _priority, float _maxDist, float _orientToNeighbour) : base(_priority)
        {
            maxDist = _maxDist;
            orientToNeighbour = _orientToNeighbour;
        }

        override
       public void run(Agent a, Agent b)
        {

            Plane3D j = (Plane3D)b;
            if (j != a)
            {
                alignPlane(a, j, 0.1f, maxDist, orientToNeighbour, ExponentialInterpolation.Squared);
            }
        }
    }
}
