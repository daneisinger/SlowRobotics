using Rhino.Geometry;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class MeshFieldElement :DistanceFieldElement
    {
        Mesh mesh;
        public float minDist;

        public MeshFieldElement(Mesh _mesh, float _weight, float _maxDist, float _attenuation, float _minDist) :base (IO.ToVec3D(_mesh.GetBoundingBox(false).Center),_weight,_maxDist,_attenuation)
        {
            mesh = _mesh;
            minDist = _minDist;
        }

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            Vec3D p = IO.ToVec3D(mesh.ClosestPoint(IO.ToPoint3d(loc)));
            float dist = p.distanceTo(loc);
            if (dist > minDist)
            {
                //not using get weight function to avoid dual closest point checks
                dist = SRMath.constrain(p.distanceTo(loc), 1, maxDist);
                float w = ((dist < maxDist) ? (weight * (1 / (float)Math.Pow(dist, attenuation))) : 0);

                Vec3D scaledVal = p.sub(loc).normalizeTo(w);

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
