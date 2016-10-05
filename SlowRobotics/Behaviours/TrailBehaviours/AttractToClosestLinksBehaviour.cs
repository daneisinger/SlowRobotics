using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class AttractToClosestLinksBehaviour : AgentBehaviour
    {

        float strength, searchRadius;

        public AttractToClosestLinksBehaviour(int _priority, float _searchRadius, float _strength) : base(_priority)
        {
            strength = _strength;
            searchRadius = _searchRadius;
        }

        override
        public void run(Agent a)
        {
            Vec3D target = null;
            Vec3D a_l = new Vec3D();
            Vec3D b_l = new Vec3D();
            float minD = 1000;

            foreach (Link l in a.getLinks())
            {
                List<Vec3D> n = a.world.getDynamicPoints(a, searchRadius);
                n.AddRange(a.world.getStaticPoints(a, searchRadius));

                foreach (Node j in n)
                {
                    if (j.parent!=a.parent)
                    {
                        foreach (Link ll in j.getLinks())
                        {

                            Link.closestPtBetweenLinks(l, ll, ref a_l, ref b_l);
                            float d = a_l.distanceTo(b_l);
                            if (d < minD)
                            {
                                target = b_l.copy();
                                minD = d;
                            }
                        }
                    }
                }
            }
            if (target != null) a.addForce(attract(a, target, 0, searchRadius, strength, ExponentialInterpolation.Squared));
        }

    }
}
