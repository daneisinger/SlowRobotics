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
        public float damping { get; set; }
        public bool verlet { get; set; }
        public float restLengthScale { get; set; }
        public SpringBehaviour(int _priority, float _damping, bool _verlet, float _restLengthScale) : base(_priority)
        {
            damping = _damping;
            verlet = _verlet;
            restLengthScale = _restLengthScale;
        }

        public override void runOn(Graph<SRParticle, Spring> graph)
        {
            foreach (Spring l in graph.Edges)
            {
                float d = ((l.l*restLengthScale) - (l.getLength()));
                Vec3D ab = l.getDir();

                l.a.Geometry.addForce(ab.scale(-d * l.s * damping * scaleFactor));
                if (verlet)l.b.Geometry.addForce(ab.scale(d * l.s * damping * scaleFactor));
            }
        }
    }
}
