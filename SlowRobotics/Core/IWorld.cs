using SlowRobotics.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public interface IWorld
    {

        List<Vec3D> search(Vec3D pos, float radius, int type);

        void addPoint(Vec3D p, bool dynamic);
        bool removePoint(Vec3D p);
        List<Vec3D> getPoints();

        void addAgent(IAgent a);
        void removeAgent(IAgent a);
        List<IAgent> getPop();

        void run();
        void run(float damping);

    }
}
