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
        /// <summary>
        /// Gets all points (up to a limit of maxPoints) within a given radius of a test point
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="radius"></param>
        /// <param name="maxPoints"></param>
        /// <returns></returns>
        IEnumerable<Vec3D> Search(Vec3D pt, float radius, int maxPoints);

        /// <summary>
        /// Add a point to the searchable collection
        /// </summary>
        /// <param name="pt"></param>
        void Add(Vec3D pt);

        /// <summary>
        /// Updates the collection with a new list of points
        /// </summary>
        /// <param name="pts"></param>
        void Update(IEnumerable<Vec3D> pts);
        
    }
}
