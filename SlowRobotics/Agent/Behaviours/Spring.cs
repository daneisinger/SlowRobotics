﻿using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Spring : ScaledBehaviour<Graph>
    {
        public float damping { get; set; }
        public bool verlet { get; set; }

        public Spring(int _priority, float _damping, bool _verlet) : base(_priority)
        {
            damping = _damping;
            verlet = _verlet;
        }

        public override void runOn(Graph a_lm)
        {
            if(a_lm != null) { 
                List<Link> springs = a_lm.getLinks();
                springs.AddRange(a_lm.getTertiaryLinks());
                foreach (Link l in springs)
                {
                    float d = (l.l - (l.getLength()));
                    Vec3D ab = l.b.sub(l.a).getNormalized();
                    if (l.a is Particle)
                    {
                        Particle p = (Particle)l.a;
                        p.addForce(ab.scale(-d * l.stiffness * damping * scaleFactor));//hookes law restorative force
                    }
                    if (l.b is Particle && verlet)
                    {
                        Particle p = (Particle)l.b;
                        p.addForce(ab.scale(d * l.stiffness * damping * scaleFactor));
                    }
                }
            }

        }

    }
}
