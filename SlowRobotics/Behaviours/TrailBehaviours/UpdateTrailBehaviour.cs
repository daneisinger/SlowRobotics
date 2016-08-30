using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class UpdateTrailBehaviour : TrailBehaviour
    {
        float damping;
        World world;

        public UpdateTrailBehaviour(int _priority, float _damping, World _world) : base(_priority)
        {
            damping = _damping;
            world = _world;
        }

        override
        public void run(Agent a, int i)
        {
            Link l = a.trail[i];
            if (l.spr) l.spring();
            //update first particle of current link
            if (!l.a.locked())
            {
                world.removeDynamic(l.a);
                l.a.update(damping);
                world.addDynamic(l.a);
            }
            
            //check to update end of link
            if (l.isEnd() && !l.b.locked() && l.b != a)
            {
                world.removeDynamic(l.b);
                l.b.update(damping);
                world.addDynamic(l.b);
            
            }
        }
    }
}
