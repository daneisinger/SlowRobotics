using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class InertiaLock : ScaledAgentBehaviour
    {

        public float minInertia { get; set; }
        public float speedFactor { get; set; }
        public int minAge { get; set; }

        public InertiaLock(int _priority, float _minInertia, float _speedFactor, int _minAge) : base(_priority)
        {
            minInertia = _minInertia;
            speedFactor = _speedFactor;
            minAge = _minAge;
        }

        public override void run(PlaneAgent a)
        {
            if (a.getInertia() + (a.getSpeed() * speedFactor * scaleFactor) < minInertia && a.age > minAge)
            {
                a.setInertia(0);
            }
        }
    }
}
