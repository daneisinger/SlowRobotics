using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Types
{
    /// <summary>
    /// Demo implementation of a particle based agent class
    /// Note - override of step and delta functions to update 
    /// particle object and access delta variables
    /// </summary>
    public class ParticleAgent : AgentT<Particle>
    {
        public ParticleAgent(Particle _p) : base(_p){ }

        public override void step(float damping)
        {
            foreach (IBehaviour b in behaviours.getData())
            {
                b.run(this);
            }
            getData().step(damping); //updates the particle object
        }

        public override float getDeltaForStep()
        {
            return getData().getDeltaForStep();
        }
    }
}
