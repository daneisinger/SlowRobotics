using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.NeighbourBehaviours
{
    public class SeparationBehaviour : NeighbourBehaviour
    {
        bool inXY;
        float max;
        float cutoff;
        float maxDist;

        public SeparationBehaviour(int _priority, float _maxDist, bool _inXY, float _max, float _cutoff) : base(_priority)
        {
            inXY = _inXY;
            max = _max;
            cutoff = _cutoff;
            maxDist = _maxDist;
        }

        override
        public void run(Agent a, Agent b)
        {

            Plane3D j = (Plane3D)b;
            if (j != a)
            {
                Vec3D toPlane3D = j.sub(a);
                ToxiPlane p = new ToxiPlane(a, a.zz);
                Vec3D op = p.getProjectedPoint(j);
                op.subSelf(a);
                float d = toPlane3D.magnitude();
                float ratio = d / maxDist;
                if (d < cutoff)
                {
                    op.invert();
                    ratio = 1 - (d / (cutoff));
                }
                else {
                    ratio = (d - cutoff) / (cutoff);
                }
                float f = interp.interpolate(0, max, ratio);
                op.scaleSelf(f);
                toPlane3D.scaleSelf(f);
                if (inXY)
                {
                    a.addForce(op);
                }
                else {
                    a.addForce(toPlane3D);
                }
            }
        }
        }

    }
