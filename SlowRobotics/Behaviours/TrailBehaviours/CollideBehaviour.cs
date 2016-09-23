using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class CollideBehaviour : AgentBehaviour
    {
        float searchRadius, repelScale, friction, stickThreshold;
        bool ignoreParent;

        /// <summary>
        /// Create behaviour with default values for repel scale (0.6) and friction (-0.02)
        /// </summary>
        /// <param name="_priority"></param>
        /// <param name="_searchRadius"></param>
        /// <param name="world"></param>
        public CollideBehaviour(int _priority, float _searchRadius):this(_priority,_searchRadius,0.6f,-0.02f, 0f,true) { }
        public CollideBehaviour(int _priority, float _searchRadius, float _repelScale, float _friction) : this(_priority, _searchRadius, 0.6f, -0.02f, 0f, true) { }
        /// <summary>
        /// Create collide behaviour
        /// </summary>
        /// <param name="_priority"></param>
        /// <param name="_searchRadius"> Radius of search sphere</param>
        /// <param name="_repelScale"> Strength of force pushing away from colliding points</param>
        /// <param name="_friction"> Stickiness of colliding points</param>
        /// <param name="_world"></param>
        public CollideBehaviour (int _priority, float _searchRadius, float _repelScale, float _friction, float _stickThreshold, bool _ignoreParent) : base(_priority)
        {

            searchRadius = _searchRadius;
            repelScale = _repelScale;
            friction = _friction;
            stickThreshold = _stickThreshold;
            ignoreParent = _ignoreParent;
        }

        override
        public void run(Agent a)
        {
                Vec3D f = new Vec3D();
                float frictioncof = 0;

                List<Vec3D> n = a.world.getDynamicPoints(a, searchRadius);
                n.AddRange(a.world.getStaticPoints(a,searchRadius));

                foreach (Node j in n)
                {
                    if (ignoreParent || j.parent != a.parent)
                    {
                        f.addSelf(repel(a, j, 0.1f, searchRadius + 1, repelScale, ExponentialInterpolation.Squared));
                        frictioncof += a.map(1 - (a.distanceTo(j) / searchRadius), 0, 1, 0, friction);
                    }
                }
                a.setInertia(1 + frictioncof);
                if(a.getInertia()>stickThreshold)a.addForce(f);

        }
    }
}
