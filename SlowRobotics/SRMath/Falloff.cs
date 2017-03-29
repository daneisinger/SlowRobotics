using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.SRMath
{
    public interface FalloffStrategy
    {
        Vec3D getForce(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator);
    }

    public class NoFalloffStrategy : FalloffStrategy
    {
        public Vec3D getForce(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float d = ab.magnitude();
            return (d > minDist && d < maxDist) ? ab.normalizeTo(maxForce) : new Vec3D();
        }

    }
    public class LinearFalloffStrategy : FalloffStrategy
    {
        public Vec3D getForce(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float d = ab.magnitude();
            float f = MathUtils.map(d, minDist, maxDist, 0, 1);
            return (d > minDist && d < maxDist) ? ab.normalizeTo(interpolator.interpolate(0, maxForce, f)) : new Vec3D();
        }
    }

    public class InverseFalloffStrategy : FalloffStrategy
    { 
        public Vec3D getForce(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float d = ab.magnitude();
            return (d > 0 && d > minDist && d < maxDist) ? ab.normalizeTo(maxForce * 1/d) : new Vec3D();
        }
    }
}
