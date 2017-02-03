using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class SpringBehaviour : ScaledBehaviour<Graph<SRParticle,Spring>>
    {
        public float damping { get; set; }
        public bool verlet { get; set; }

        public SpringBehaviour(int _priority, float _damping, bool _verlet) : base(_priority)
        {
            damping = _damping;
            verlet = _verlet;
        }

        public override void runOn(Graph<SRParticle, Spring> graph)
        {
            foreach (Spring l in graph.Edges)
            {
                float d = (l.l - (l.getLength()));
                Vec3D ab = l.getDir();

                l.a.Geometry.addForce(ab.scale(-d * l.s * damping * scaleFactor));
                if (verlet)l.b.Geometry.addForce(ab.scale(d * l.s * damping * scaleFactor));
            }
        }
    }
}
