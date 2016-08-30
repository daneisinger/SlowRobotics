using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class CollideTrailBehaviour : TrailBehaviour
    {
        World world;
        float searchRadius, repelScale, friction;

        /// <summary>
        /// Create behaviour with default values for repel scale (0.6) and friction (-0.02)
        /// </summary>
        /// <param name="_priority"></param>
        /// <param name="_searchRadius"></param>
        /// <param name="world"></param>
        public CollideTrailBehaviour(int _priority, float _searchRadius, World world):this(_priority,_searchRadius,0.6f,-0.02f,  world)
        {

        }
        /// <summary>
        /// Create collide behaviour
        /// </summary>
        /// <param name="_priority"></param>
        /// <param name="_searchRadius"> Radius of search sphere</param>
        /// <param name="_repelScale"> Strength of force pushing away from colliding points</param>
        /// <param name="_friction"> Stickiness of colliding points</param>
        /// <param name="_world"></param>
        public CollideTrailBehaviour (int _priority, float _searchRadius, float _repelScale, float _friction, World _world) : base(_priority)
        {
            world = _world;
            searchRadius = _searchRadius;
            repelScale = _repelScale;
            friction = _friction;
        }

        override
        public void run(Agent a, int i)
        {
            Link l = a.trail[i];
            if (!l.a.locked())
            {
                Vec3D f = new Vec3D();
                float frictioncof = 0;

                List<Vec3D> n = world.getDynamicPoints(l.a, searchRadius);
                n.AddRange(world.getStaticPoints(l.a,searchRadius));
                foreach (Plane3D j in n)
                {
                    f.addSelf(repel(l.a, j, repelScale, searchRadius + 1));
                    frictioncof += a.map(1 - (l.a.distanceTo(j) / searchRadius), 0, 1, 0, friction);
                }
                //mult by age
               // frictioncof *= (1 + (l.a.age / 700f));
                l.a.setInertia(1 + frictioncof);
                l.a.addForce(f);
                if (l.isEnd())
                {
                    l.b.setInertia(1 + frictioncof);
                }
            }
        }

        private Vec3D repel(Vec3D a, Vec3D j, float scale, float max)
        {
            Vec3D toPlane3D = j.sub(a);
            float d = toPlane3D.magnitude();
            if (d < max && d > 0.1)
            {
                float ratio = 1 - (d / (max));
                float f = interp.interpolate(0, scale, ratio);
                toPlane3D.invert();
                return toPlane3D.scale(f);
            }
            else {
                return new Vec3D();
            }
        }
    }
}
