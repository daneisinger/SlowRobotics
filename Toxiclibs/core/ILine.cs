using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxiclibs.core
{
    public interface ILine
    {
        Vec3D start { get; set; }
        Vec3D end { get; set; }

        Vec3D closestPoint(ReadonlyVec3D testPoint);
        Vec3D closestPoint(ILine other);
        Vec3D pointAt(float param);
    }
}
