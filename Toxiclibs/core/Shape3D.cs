using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiclibs.core
{
    public interface Shape3D
    {

        /**
         * Checks if the point is within the given shape/volume.
         * 
         * @return true, if inside
         */
        bool containsPoint(ReadonlyVec3D p);
    }
}
