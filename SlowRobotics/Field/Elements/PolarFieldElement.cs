using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class PolarFieldElement : PlaneFieldElement
    {

        public PolarFieldElement (Plane3D _location, float _weight, float _maxDist, float _attenuation) : base(_location, _weight, _maxDist, _attenuation) {}

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);
            //d.numberData += w;

            Vec3D ab = location.sub(loc).normalize();
            Vec3D scaledVal = ab.cross(value.zz).scale(w);

            if (d.hasVectorData())
            {
                d.vectorData.addSelf(scaledVal);
            }
            else {
                d.vectorData = scaledVal;
            }
        }
    }
}
