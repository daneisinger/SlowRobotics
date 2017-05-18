using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
    /// <summary>
    /// Search interface 
    /// </summary>
    public interface ISearchable
    {
        IEnumerable<Vec3D> Search(Vec3D pt, float radius);
        void Add(Vec3D pt);
        void Update(IEnumerable<Vec3D> pts);
    }
}
