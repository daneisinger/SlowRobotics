using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class LinkPair
    {
        public Link a, b;
        public float angle { get; set; }
        

        public LinkPair(Link _a, Link _b)
        {
            a = _a;
            b = _b;
            angle = angleBetweenSharedNode(a,b);
        }

        public float getCurrentAngle()
        {
            return a.angleBetween(b, true);
        }

        public static float angleBetweenSharedNode(Link _a, Link _b)
        {
            //atempt get shared node
            Node shared;
            if (getSharedNode(_a,_b, out shared))
            {
                Vec3D ab = _a.tryGetOther(shared).sub(shared);
                Vec3D abo = _b.tryGetOther(shared).sub(shared);
                
                //TODO need to avoid NAN

                return ab.angleBetween(abo, true);
            }
            return 0;
        }

        public bool hasLink(Link l)
        {
            if (a == l || b == l) return true;
            return false;
        }

        public bool replaceLink(Link toReplace, Link replaceWith)
        {
            if (a == toReplace)
            {
                a = replaceWith;
                return true;
            }else if (b == toReplace)
            {
                b = replaceWith;
                return true;
            }
            return false;
        }

        public static bool getSharedNode(Link _a, Link _b, out Node shared)
        {
            if(_a.a == _b.a || _a.a == _b.b)
            {
                shared = _a.a;
                return true;
            }
            if (_a.b == _b.a || _a.b == _b.b)
            {
                shared = _a.b;
                return true;
            }
            shared = null;
            return false;
        }

        public Vec3D bisectPair(Node shared)
        {
         //   float currentAngle = getCurrentAngle();

                Node nextOther = b.tryGetOther(shared);
                Node prevOther = a.tryGetOther(shared);

                 if (nextOther == prevOther) return new Vec3D();

                float currentAngle= prevOther.sub(shared).angleBetween(nextOther.sub(shared), true);
                // if (float.IsNaN(currentAngle)) currentAngle = (float)Math.PI;

                Vec3D ab = prevOther.add(nextOther).scale((float)0.5);
                float diff = (angle - currentAngle) / (float)Math.PI; //max is 1, min is -1
                Vec3D avg = ab.sub(shared).scale(diff);
                return avg;

        }

        //WIP

        public Quaternion aQuat { get; set; }
        public Quaternion bQuat { get; set; }

        public bool setQuats()
        {
            Node shared;
            if (getSharedNode(a,b,out shared))
            {
                aQuat = Quaternion.getAlignmentQuat(a.getDir(), shared.xx);
                bQuat = Quaternion.getAlignmentQuat(b.getDir(), shared.xx);
                return true;
            }
            return false; //no shared node
        }

        public void setQuats(Node shared)
        {

            aQuat = Quaternion.getAlignmentQuat(a.getDir(), shared.xx);
            bQuat = Quaternion.getAlignmentQuat(b.getDir(), shared.xx);

        }


        public Vec3D bisectQuats(Node shared, float interpolationFactor)
        {
            Quaternion aCurrent = Quaternion.getAlignmentQuat(a.getDir(), shared.xx);
            Quaternion bCurrent = Quaternion.getAlignmentQuat(b.getDir(), shared.xx);

            Quaternion aTransform = aCurrent.interpolateToSelf(aQuat, interpolationFactor);
            Quaternion bTransform = aCurrent.interpolateToSelf(bQuat, interpolationFactor);

            //get current quats
            //interpolate by difference
            //transform shared node
            Vec3D aF = shared.transformOrigin(aTransform.toMatrix4x4());
            Vec3D bF = shared.transformOrigin(bTransform.toMatrix4x4());

            return aF.addSelf(bF);
        }


    }
}
