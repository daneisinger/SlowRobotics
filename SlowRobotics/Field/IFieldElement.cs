using SlowRobotics.Core;
using SlowRobotics.Field.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    /// <summary>
    /// Field element interface
    /// </summary>
    public interface IFieldElement
    {
        Vec3D location { get; set; }
        void integrate(ref FieldData d, Vec3D loc);
        float getWeight(Vec3D loc);
    }
}
