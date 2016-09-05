using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class LockByInertiaBehaviour : AgentBehaviour
    {
        float minInertia;
        int minAge;

        public LockByInertiaBehaviour(int _priority, float _minInertia, int _minAge) : base(_priority)
        {
            minInertia = _minInertia;
            minAge = _minAge;
        }

        override
        public void run(Agent a)
        {
            if (a.getInertia() * (a.getSpeed() * 4) < minInertia && a.age>minAge)
            {
                //POSSIBLE ISSUE HERE:
                Node staticNode = new Node(a);
                a.world.addStatic(staticNode);
                a.world.removeDynamic(a);

            }
        }

    }
}
