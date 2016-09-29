using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.NeighbourBehaviours
{
    public class AlignNeighbourLinksBehaviour : AgentBehaviour
    {

        float searchRadius, strength;
        bool ignoreParent;

        public AlignNeighbourLinksBehaviour(int _priority, float _searchRadius, float _strength, bool _ignoreParent) : base(_priority)
        {
            searchRadius = _searchRadius;
            ignoreParent = _ignoreParent;
            strength = _strength;
        }

        override
        public void run(Agent a)
        {
            List<Vec3D> n = a.world.getDynamicPoints(a, searchRadius);
            n.AddRange(a.world.getStaticPoints(a, searchRadius));

            foreach (Node j in n)
            {
                if (ignoreParent || j.parent != a.parent)
                {
                    foreach (Link l in j.getLinks())
                    {
                        Vec3D ab = l.closestPt(a).sub(a);
                        float f = scaleBehaviour(ab, 0, searchRadius, strength, ExponentialInterpolation.Squared); 
                        a.interpolateToXX(l.getDir(), f);
                    }
                }
            }
        }
    }
}
