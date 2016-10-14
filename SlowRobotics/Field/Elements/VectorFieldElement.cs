using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class VectorFieldElement : DistanceFieldElement
    {
        public Vec3D value { get; set; }

        public VectorFieldElement(Vec3D _location, Vec3D _vector) : this(_location, _vector, 1, 10000, 2) { } //default

        public VectorFieldElement(Vec3D _location, Vec3D _vector, float _weight, float _maxDist, float _attenuation) :base(_location.copy(),_weight,_maxDist,_attenuation)
        {
            value = _vector;
        }

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);
            Vec3D scaledVal = value.scale(w);
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
