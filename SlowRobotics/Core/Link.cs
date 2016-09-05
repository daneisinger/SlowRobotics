using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Link
    {
        public Node a;
        public Node b;
        public float l;
        public float stiffness { get; set; }
        public float linkAngle { get; set; }

        public Link(Node _a, Node _b, float _l)
        {
            a = _a;
            b = _b;
            l = _l;
            stiffness = 0.08f;
            linkAngle = (float)Math.PI;
        }

        public Link(Node _a, Node _b)
        {
            a = _a;
            b = _b;
            updateLength();
            stiffness = 0.08f;
            linkAngle = (float)Math.PI;
        }
        
        public bool replaceNode(Node oldN, Node newN)
        {
            if(oldN== a)
            {
                a = newN;
                return true;
            }else if(oldN== b)
            {
                b = newN;
                return true;
            }
            return false;
        }

        public float angleBetween(Link other, bool flip)
        {
            Vec3D ab = a.sub(b);
            Vec3D abo = other.a.sub(other.b);
            if (flip) abo.invert();
            return ab.angleBetween(abo, true);
        }

        public Vec3D closestPt(Vec3D p)
        {
            Vec3D dir = b.sub(a);
            float t = p.sub(a).dot(dir) / dir.magSquared();
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return a.add(dir.scaleSelf(t));
        }

        public void updateLength()
        {
            l = a.distanceTo(b);
        }

        public Vec3D getDir()
        {
            return b.sub(a).normalize();
        }

        public float getLength()
        {
            return a.distanceTo(b);
        }

        public Node tryGetOther(Node test)
        {
            if (a == test) return b;
            return a;
        }
    }
}
