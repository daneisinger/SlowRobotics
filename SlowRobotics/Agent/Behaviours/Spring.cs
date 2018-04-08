using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Verlet spring behaviour that iterates over all springs (edges) in a graph
    /// and applies forces to the spring particles to try to maintain a given spring length.
    /// </summary>
    public class SpringBehaviour : ScaledBehaviour<Graph<SRParticle,Spring>>
    {
        public float restLengthScale { get; set; }
        public SpringBehaviour(int _priority, float _restLengthScale) : base(_priority)
        {
            restLengthScale = _restLengthScale;
        }

        public override void runOn(Graph<SRParticle, Spring> graph)
        {
            foreach (Spring l in graph.Edges)
            {
                Vec3D delta = l.a.Geometry.sub(l.b.Geometry);
                float distance = delta.magnitude();
                if (distance > 0)
                {
                    delta /= distance;
                }
                else {
                    delta.set(0f, 0f, 0f);
                }
                float intensity = -(distance - l.l) * l.s;
                delta *= intensity;

                l.a.Geometry.addForce(delta);
                l.b.Geometry.addForce(delta.invert());
            }
        }

    }
}
