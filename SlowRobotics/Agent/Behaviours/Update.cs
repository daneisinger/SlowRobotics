using SlowRobotics.Core;
using SlowRobotics.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class IntegrateBehaviour : ScaledBehaviour<IParticle>
    {
        public float damping { get; set; }

        public IntegrateBehaviour(int _priority, float _damping) : base(_priority)
        {
            damping = _damping;
            lateUpdate = true;
        }

        public override void runOn(IParticle p)
        {
            p.step(damping * scaleFactor);
        }
    }

    public class RebuildTree : Behaviour<ISearchable>
    {

        public AgentList pop;

        public RebuildTree(int _p, AgentList _pop) : base(_p)
        {
            pop = _pop;
            //lateUpdate = true;
        }

        public override void runOn(ISearchable tree)
        {

            List<Vec3D> pts = new List<Vec3D>();
            if (pop != null)
            {
                foreach (IAgent p in pop.getAgents())
                {
                    IAgentT<object> a = (IAgentT<object>)p;
                    if (a != null)
                    {
                        Vec3D v = a.getData() as Vec3D;
                        if (v != null) pts.Add(v);
                    }
                }
                tree.Update(pts);
            }
        }
    }
}
