using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class AlignAxisToVelocity : ScaledAgentBehaviour
    {

        public float strength { get; set; }
        public int axis { get; set; }

        public AlignAxisToVelocity(int _priority, float _strength, int _axis) : base(_priority)
            {
                strength = _strength;
                axis = _axis;
            }

        public override void run(PlaneAgent a)
        {
            switch (axis) {
                case 0:
                    a.interpolateToXX(a.getVel(), strength * scaleFactor);
                    break;
                case 1:
                    a.interpolateToYY(a.getVel(), strength * scaleFactor);
                    break;
                case 2:
                    a.interpolateToZZ(a.getVel(), strength * scaleFactor);
                    break;
            }
        }

     }
}
