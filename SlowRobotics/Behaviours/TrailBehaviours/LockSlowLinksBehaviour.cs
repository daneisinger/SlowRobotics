using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class LockSlowLinksBehaviour : TrailBehaviour
    {
        float minInertia;
        int minAge;
        World world;

        public LockSlowLinksBehaviour(int _priority, float _minInertia, int _minAge, World _world) : base(_priority)
        {
            minInertia = _minInertia;
            minAge = _minAge;
            world = _world;
        }

        override
        public void run(Agent a, int i)
        {
            Link l = a.trail[i];
            if (l.a.getInertia() * (l.a.getSpeed() * 4) < minInertia && l.a.age>minAge)
            {
                l.a.Lock();
                l.b.Lock();
                world.addStatic(l.a);
                world.removeDynamic(l.b);
                world.addStatic(l.b);
            }
        }

    }
}
