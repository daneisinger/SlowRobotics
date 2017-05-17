using Rhino.Geometry;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.SRMath;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    /// <summary>
    /// Creates a vector field from the closest point on a mesh
    /// </summary>
    public class MeshFieldElement :DistanceFieldElement
    {
        Mesh mesh;
        public float minDist;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_mesh">Mesh to generate field</param>
        /// <param name="_weight">Field weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        /// <param name="_minDist">Minimum field distance - the field weight interpolates to 0 when sample points are < minDist from the mesh </param>
        public MeshFieldElement(Mesh _mesh, float _weight, float _maxDist, float _attenuation, float _minDist) :base (_mesh.GetBoundingBox(false).Center.ToVec3D(),_weight,_maxDist,_attenuation)
        {
            mesh = _mesh;
            minDist = _minDist;
        }

        /// <summary>
        /// Integrates the mesh field tensor at a sample point into FieldData
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
        public override void integrate(ref FieldData d, Vec3D loc)
        {
            Vec3D p = mesh.ClosestPoint(loc.ToPoint3d()).ToVec3D();
            Vec3D ap = p.sub(loc);
            float dist = ap.magnitude();
            if (dist < maxDist)
            {
                //linear falloff when < minDist
                float f = (float) Math.Pow((dist< minDist)? MathUtils.map(dist, 0, minDist, 0, 1) : 1, attenuation);
                Vec3D scaledVal = ap.scale(weight*f);

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
}
