using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class AlignXXToNearLinks : PlaneAgentBehaviour
    {

        public float searchRadius { get; set; }
        public float strength { get; set; }
        public bool useParent { get; set; }

        public AlignXXToNearLinks(int _priority, float _maxDist, float _strength, bool _useParent) : base(_priority)
        {
            searchRadius = _maxDist;
            useParent = _useParent;
            strength = _strength;
        }

        public override void test(PlaneAgent a, Plane3D p)
        {
            if (p is Node) test(a, (Node)p);
        }

        public void test(PlaneAgent a, Node p)
        {
            if (useParent || p.parent != a.parent)
            {
                foreach (Link l in p.getLinks())
                {
                    Vec3D ab = l.closestPt(a).sub(a);
                    float f = scaleBehaviour(ab, 0, searchRadius, strength, ExponentialInterpolation.Squared);
                    a.interpolateToXX(l.getDir(), f);
                }
            }
        }
    }
}
