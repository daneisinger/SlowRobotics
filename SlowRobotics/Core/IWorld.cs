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

        List<Vec3D> searchDynamic(Vec3D pos, float radius);
        List<Vec3D> searchStatic(Vec3D pos, float radius);
        List<Vec3D> search(Vec3D pos, float radius);
        List<Vec3D> getPoints();

        void addDynamic(Particle p);
        void addStatic(Node p);
        bool removeDynamic(Particle p);
        bool removeStatic(Node p);

        void run();
        void run(float damping);

        List<IAgent> getPop();

    }
}
