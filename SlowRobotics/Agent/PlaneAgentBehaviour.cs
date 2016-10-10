using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    public class PlaneAgentBehaviour : Behaviour
    {


        public int priority;
        /// <summary>
        /// Empty constructor with default priority of 1
        /// </summary>
        public PlaneAgentBehaviour() : this(1)
        {

        }
        /// <summary>
        /// Create new behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Behaviour priority, higher runs first</param>
        public PlaneAgentBehaviour(int _priority)
        {
            priority = _priority;
        }

        public int CompareTo(Behaviour other)
        {
            if (other.getPriority() > priority) return -1;
            if (other.getPriority() < priority) return 1;
            return 0;
        }

        public int getPriority()
        {
            return priority;
        }
        
        /// <summary>
        /// Cast to plane agent
        /// </summary>
        /// <param name="a"></param>
        public virtual void run(Agent a)
        {
            if (a is PlaneAgent) run((PlaneAgent)a);
        }

        public virtual void test(Agent a, Plane3D p)
        {
            if (a is PlaneAgent) test((PlaneAgent)a, p);
        }

        /// <summary>
        /// Function for behaviour to run. Override this function in new behaviours
        /// </summary>
        /// <param name="a">Current agent</param>
        public virtual void run(PlaneAgent a) {}

        public virtual void test(PlaneAgent a, Plane3D p) { }


        public float scaleBehaviour(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float dist = ab.magnitude();
            float sf = 0;
            if (dist > minDist && dist < maxDist)
            {
                float f = (dist - minDist) / (maxDist - minDist);
                sf = interpolator.interpolate(0, maxForce, f);
            }
            return sf;
        }

        public Vec3D attract(Vec3D a, Vec3D b, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            Vec3D ab = b.sub(a);
            float f = maxForce - scaleBehaviour(ab, minDist, maxDist, maxForce, interpolator);
            return (f!=0) ? ab.normalizeTo(f) : new Vec3D();
        }

        public Vec3D repel(Vec3D a, Vec3D b, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            Vec3D ab = b.sub(a);
            float f = -(maxForce - scaleBehaviour(ab, minDist, maxDist, maxForce, interpolator));
            return (f!=0) ? ab.normalizeTo(f) :new Vec3D();
        }

        public Vec3D alignVectors(Vec3D aPos, Vec3D bPos, Vec3D aDir, Vec3D bDir, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            Vec3D ab = bPos.sub(aPos);
            float sf = maxForce - scaleBehaviour(ab, minDist, maxDist, maxForce, interpolator); //invert
            return aDir.interpolateTo(bDir, sf);
        }

        public void alignPlane(Plane3D toAlign, Plane3D b, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            Vec3D ab = b.sub(toAlign);
            float sf = maxForce - scaleBehaviour(ab, minDist, maxDist, maxForce, interpolator); //invert
            if (sf > 0) toAlign.interpolateToPlane3D(b, sf);
        }
    }
}
