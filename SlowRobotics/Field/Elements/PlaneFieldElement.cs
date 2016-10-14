using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class PlaneFieldElement : DistanceFieldElement
    {

        public WeightedPlane3D value { get; set; }

        public PlaneFieldElement(Plane3D _plane) : this(_plane, 1, 10000, 2) { } //default

        public PlaneFieldElement(Plane3D _plane, float _weight, float _maxDist, float _attenuation) : 
            this(new WeightedPlane3D(_plane, _weight),_weight,_maxDist,_attenuation){ }

        public PlaneFieldElement(WeightedPlane3D _plane, float _weight, float _maxDist, float _attenuation) :base(_plane.copy(), _weight,_maxDist,_attenuation)
        {
            value = _plane;
        }

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);
            if (d.hasPlaneData())
            {
                d.planeData.add(value, w);
            }
            else
            {
                d.planeData = new WeightedPlane3D(value, w);
            }
        }

    }
}
