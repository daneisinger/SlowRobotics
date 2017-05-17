using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;
using SlowRobotics.SRMath;

namespace SlowRobotics.Field.Elements
{
    /// <summary>
    /// Creates a vector field from a point
    /// </summary>
    public class DistanceFieldElement : IFieldElement
    {

        public Vec3D location { get; set; }
        public float weight { get; set; }
        public float maxDist { get; set; }
        public float attenuation { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_location">Field point location</param>
        /// <param name="_weight">Field Weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        public DistanceFieldElement(Vec3D _location, float _weight, float _maxDist, float _attenuation)
        {
            location = _location;
            weight = _weight;
            maxDist = _maxDist;
            attenuation = _attenuation;
        }

        /// <summary>
        /// Gets the field weight at a given point
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public virtual float getWeight(Vec3D loc)
        {
            float d = MathUtils.constrain(location.distanceTo(loc), 1, maxDist);
            return ((d < maxDist) ? (weight * (1 / (float)Math.Pow(d, attenuation))) : 0);
        }

        /// <summary>
        /// Integrates the distance field tensor at a given point into FieldData
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
        public virtual void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);
            d.numberData += w;

            Vec3D scaledVal = location.sub(loc).scale(w);
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
