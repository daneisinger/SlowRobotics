using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class ZLock : AgentBehaviour
    {
        public float minZ
        {
            get; set;
        }

        public ZLock(int _priority, float _minZ) : base(_priority)
        {
            minZ = _minZ;
        }

        public override void run (PlaneAgent a)
        {
            if (a.z < minZ) a.setInertia(0);
        }
    }
}
