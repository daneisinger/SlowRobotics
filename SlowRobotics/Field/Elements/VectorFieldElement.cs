using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    /// <summary>
    /// Creates a vector field from a location and direction
    /// </summary>
    public class VectorFieldElement : DistanceFieldElement
    {
        public Vec3D value { get; set; }

        /// <summary>
        /// Default constructor with weight 1, maxDist 10000, attenuation 2
        /// </summary>
        /// <param name="_location">Field location</param>
        /// <param name="_vector">Field direction</param>
        public VectorFieldElement(Vec3D _location, Vec3D _vector) : this(_location, _vector, 1, 10000, 2) { } //default

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_location">Field location</param>
        /// <param name="_vector">Field direction</param>
        /// <param name="_weight">Field weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        public VectorFieldElement(Vec3D _location, Vec3D _vector, float _weight, float _maxDist, float _attenuation) :base(_location.copy(),_weight,_maxDist,_attenuation)
        {
            value = _vector;
        }

        /// <summary>
        /// Integrates vector field tensor at sample point into FieldData
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
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
