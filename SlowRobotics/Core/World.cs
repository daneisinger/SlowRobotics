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
        void add(Agent a);
        void addDynamic(Plane3D p);
        void addStatic(Plane3D p);
        void remove(Agent a);
        void removeDynamic(Plane3D p);
        void removeStatic(Plane3D p);

        void run();

        List<Agent> getPop();

    }
}
