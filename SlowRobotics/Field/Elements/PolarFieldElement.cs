using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    /// <summary>
    /// Creates a vector field by finding cross products with a planes Z Axis and the vector to a sample point
    /// </summary>
    public class PolarFieldElement : PlaneFieldElement
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_location">Field plane</param>
        /// <param name="_weight">Field weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        public PolarFieldElement (Plane3D _location, float _weight, float _maxDist, float _attenuation) : base(_location, _weight, _maxDist, _attenuation) {}

        /// <summary>
        /// Integrates polar field tensor at sample point into FieldData
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
        public override void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);
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
