using SlowRobotics.Field.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    /// <summary>
    /// Field interface
    /// </summary>
    public interface IField
    {
        List<IFieldElement> field { get; set; }
        AABB bounds { get; set; }
        FieldData evaluate(Vec3D pt);
        void updateBounds();
    }
}
