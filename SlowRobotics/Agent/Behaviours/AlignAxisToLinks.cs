using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class AlignAxisToLinks : ScaledAgentBehaviour
    {

        public float strength { get; set; }
        public int axis { get; set; }

        public AlignAxisToLinks(int _priority, float _strength, int _axis) : base(_priority)
        {
            strength = _strength;
            axis = _axis;
        }

        public override void run(PlaneAgent a)
        {
            foreach (Link l in a.getLinks())
            {
                switch (axis)
                {
                    case 0:
                        a.interpolateToXX(l.getDir(), strength*scaleFactor);
                        break;
                    case 1:
                        a.interpolateToYY(l.getDir(), strength * scaleFactor);
                        break;
                    case 2:
                        a.interpolateToZZ(l.getDir(), strength * scaleFactor);
                        break;
                }
            }
        }

    }
}
