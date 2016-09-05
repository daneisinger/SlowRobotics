using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public interface World
    {

        List<Vec3D> getDynamicPoints(Vec3D pos, float radius);
        List<Vec3D> getStaticPoints(Vec3D pos, float radius);
        void addDynamic(Node p);
        void addStatic(Node p);
        bool removeDynamic(Node p);
        bool removeStatic(Node p);

        void run();

        List<Node> getPop();

    }
}
