using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{

    //TODO - update for linkmesh
    public class CohereToNearestLink : PlaneAgentBehaviour
    {

        public float strength { get; set; }
        public float searchRadius { get; set; }
        public LinkMesh parent { get; set; }

        Vec3D a_l;
        Vec3D b_l;
        float minD;
        Link closestL;
        Link closestO;
        Vec3D targetA;
        Vec3D targetB;

        public CohereToNearestLink(int _priority, LinkMesh _parent, float _searchRadius, float _strength) : base(_priority)
        {
            strength = _strength;
            searchRadius = _searchRadius;
            parent = _parent;

            reset();
        }

        public void reset()
        {
            closestL = null;
            closestO = null;
            targetA = null;
            targetB = null;
            a_l = new Vec3D();
            b_l = new Vec3D();
            minD = 1000;
        }

        public override void test(PlaneAgent a, Plane3D p)
        {
            if (p is Node) test(a, (Node)p);   
        }

        //TODO - redo with linkmeshes

        public void test(PlaneAgent a, Node p)
        {
            foreach (Link l in a.getLinks())
            {
                foreach (Link ll in p.getLinks())
                {

                    Link.closestPtBetweenLinks(l, ll, ref a_l, ref b_l);
                    float d = a_l.distanceTo(b_l);

                    if (d < minD && d > 0)
                    {
                        targetA = a_l.copy();
                        targetB = b_l.copy();
                        minD = d;
                        closestL = l;
                        closestO = ll;
                    }
                }
            }
        }

        public override void run (PlaneAgent a)
        {
            if (targetA != null && minD > 0.2)
            {
                float frictioncof = scaleBehaviour(b_l.sub(a_l), 0.1f, searchRadius, -0.3f, ExponentialInterpolation.Squared);

                //TODO - fix inertia

                a.setInertia(1 + frictioncof);
                a.addForce(attract(a, targetB, 0.25f, searchRadius, strength, ExponentialInterpolation.Squared));

                if (minD < 0.5)
                {
                    if (targetA.distanceTo(closestL.a) > 2 && targetA.distanceTo(closestL.b) > 2) split(a, closestL, targetA);
                }
            }

            reset();
        }

        public void split(PlaneAgent a, Link l, Vec3D pt)
        {
            PlaneAgent aa = new PlaneAgent(new Plane3D(pt), a.world);
            aa.parent = a.parent;

            Node na = l.a;
            Node nb = l.b;

            Link a_s = new Link(aa, na);
            Link b_s = new Link(aa, nb);

            parent.replaceLink(l, a_s);
            parent.replaceLink(l, b_s);

            foreach (Behaviour b in a.behaviours.getData()) aa.addBehaviour(b);

            a.world.addDynamic(aa);
        }
    }
}
