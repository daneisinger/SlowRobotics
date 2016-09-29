using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class LinkPair
    {
        public Link a, b;
        public float angle { get; set; }

        public LinkPair(Link _a, Link _b)
        {
            a = _a;
            b = _b;
            angle = a.angleBetween(b, true);
        }

        public float getCurrentAngle()
        {
            return a.angleBetween(b, true);
        }

        public bool hasLink(Link l)
        {
            if (a == l || b == l) return true;
            return false;
        }

        public bool getSharedNode(out Node shared)
        {
            if(a.a == b.a || a.a == b.b)
            {
                shared = a.a;
                return true;
            }
            if (a.b == b.a || a.b == b.b)
            {
                shared = a.b;
                return true;
            }
            shared = null;
            return false;
        }

        public Vec3D bisectPair(Node shared)
        {
            float currentAngle = getCurrentAngle();

                Node nextOther = b.tryGetOther(shared);
                Node prevOther = a.tryGetOther(shared);

                Vec3D ab = prevOther.add(nextOther).scale((float)0.5);
                float diff = (angle - currentAngle) / (float)Math.PI; //max is 1, min is -1
                Vec3D avg = ab.sub(shared).scale(diff);
                return avg;

        }


    }
}
