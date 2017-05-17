using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    /// <summary>
    /// Creates a vector field from the weighted axis of a plane
    /// </summary>
    public class PlaneFieldElement : DistanceFieldElement
    {

        public WeightedPlane3D value { get; set; }

        /// <summary>
        /// Default constructor with weight 1, maxDist 10000 and attenuation 2
        /// </summary>
        /// <param name="_plane">Field plane</param>
        public PlaneFieldElement(Plane3D _plane) : this(_plane, 1, 10000, 2) { } //default

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_plane">Field plane</param>
        /// <param name="_weight">Field weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        public PlaneFieldElement(Plane3D _plane, float _weight, float _maxDist, float _attenuation) : 
            this(new WeightedPlane3D(_plane, _weight),_weight,_maxDist,_attenuation){ }

        /// <summary>
        /// Creates a field from a WeightedPlane 
        /// </summary>
        /// <param name="_plane"></param>
        /// <param name="_weight"></param>
        /// <param name="_maxDist"></param>
        /// <param name="_attenuation"></param>
        public PlaneFieldElement(WeightedPlane3D _plane, float _weight, float _maxDist, float _attenuation) :base(_plane.copy(), _weight,_maxDist,_attenuation)
        {
            value = _plane;
        }

        /// <summary>
        /// Integrates plane field tensor at a sample point into FieldData
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
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
