using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class AddToTrailBehaviour : AgentBehaviour
    {

        Vec3D offset;
        int frequency;
        int ctr;

        public AddToTrailBehaviour(int _priority, int _frequency, Vec3D _offset) : base(_priority)
        {
            offset = _offset;
            frequency = _frequency;
            ctr = 0;
        }

        override
        public void run(Agent a)
        {
            if (ctr % frequency == 0)
            {
                Plane3D p = new Plane3D(a.add(offset));
                addToTrail(a, p);
            }
            ctr++;

        }

        public static void addToTrail(Agent a, Plane3D p)
        {
            Link lastLink = a.trail.Last<Link>();
            if (lastLink.b == a)
            {
                lastLink.setB(new Plane3D(p));
                lastLink.updateLength();
            }
            else {
                Link newLink = new Link(lastLink.b, new Plane3D(p), true);
                newLink.setEnd(true);
                a.trail.Add(newLink);
                lastLink.setEnd(false);
            }
        }
    }
}
