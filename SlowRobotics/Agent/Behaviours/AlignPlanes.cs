using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class AlignPlanes : PlaneAgentBehaviour
    {
        public float maxDist { get; set; }
        public float orientToNeighbour { get; set; }

        public AlignPlanes(int _priority, float _maxDist, float _orientToNeighbour) : base(_priority)
        {
            maxDist = _maxDist;
            orientToNeighbour = _orientToNeighbour;
        }

       public override void test(PlaneAgent a, Plane3D p)
        {
           alignPlane(a, p, 0.1f, maxDist, orientToNeighbour, ExponentialInterpolation.Squared);
        }
    }
}
