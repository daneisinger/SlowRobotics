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
    public class MeshFieldElement :DistanceFieldElement
    {
        Mesh mesh;
        public float minDist;

        public MeshFieldElement(Mesh _mesh, float _weight, float _maxDist, float _attenuation, float _minDist) :base (_mesh.GetBoundingBox(false).Center.ToVec3D(),_weight,_maxDist,_attenuation)
        {
            mesh = _mesh;
            minDist = _minDist;
        }

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            Vec3D p = mesh.ClosestPoint(loc.ToPoint3d()).ToVec3D();
            Vec3D ap = p.sub(loc);
            float dist = ap.magnitude();
            if (dist < maxDist)
            {
                //not using get weight function to avoid dual closest point checks
                //dist = MathUtils.constrain(p.distanceTo(loc), minDist, maxDist);
                //float w = ((dist < maxDist) ? (weight * (1 / (float)Math.Pow(dist, attenuation))) : 0);

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
