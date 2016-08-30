using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Link
    {
        public Plane3D a;
        public Plane3D b;
        public bool spr;
        public float l;
        public float stiffness;
        public float linkAngle;
        public int val;
        public bool end;

        public Link(Plane3D _a, Plane3D _b, bool _spr): this(_a, _b, _spr, _a.distanceTo(_b))
        {
           
        }

        public Link(Plane3D _a, Plane3D _b, bool _spr, float _l)
        {
            a = _a;
            b = _b;
            spr = _spr;
            l = _l;
            stiffness = 0.08f;
            linkAngle = (float)Math.PI;
            val = 0;
            end = false;
        }

        public void setAngle(float angle)
        {
            linkAngle = angle;
        }

        public void alignEnds()
        {
            a.interpolateToZZ(b.sub(a).normalize(), 0.1f);
            b.interpolateToZZ(b.sub(a).normalize(), 0.1f);
            a.interpolateToXX(b.xx, 0.1f);
        }

        public void alignWithLink(Link l, float sf)
        {
            a.interpolateToXX(l.a.xx, sf);
            b.interpolateToXX(l.b.xx, sf);
            //a.interpolateToXX(l.a.zz, 0.1f);
        }

        public void spring()
        {
            float d = (l - (a.distanceTo(b)));
            Vec3D ab = b.sub(a).getNormalized();
            a.addForce(ab.scale(-d * stiffness));
            b.addForce(ab.scale(d * stiffness));
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

        public void addForce(Vec3D force)
        {
            a.addForce(force);
            b.addForce(force);
        }

        public void update()
        {
            a.update(0.05f);
            b.update(0.05f);

        }
        public void setA(Plane3D _a)
        {
            a = _a;
        }

        public void setB(Plane3D _b)
        {
            b = _b;
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

        public void setVal(int v)
        {
            val = v;
        }

        public int getVal()
        {
            return val;
        }

        public void setEnd(bool toVal)
        {
            end = toVal;
        }
        public bool isEnd()
        {
            return end;

        }

    }
}
