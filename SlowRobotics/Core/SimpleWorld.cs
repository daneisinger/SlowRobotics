using SlowRobotics.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SimpleWorld : IWorld
    {

        List<IAgent> pop; //things with behaviours to run

        public Plane3DOctree dynamicTree; //particles
        public Plane3DOctree staticTree; //nodes
        float bounds;

        public SimpleWorld(float _bounds)
        {
            pop = new List<IAgent>();
            bounds = _bounds;
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            staticTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
        }

        
        public List<IAgent> getPop()
        {
            return pop;
        }

        /// <summary>
        /// Add a plane to the octree
        /// </summary>
        /// <param name="p"></param>
        public void addDynamic(Particle p)
        {
            if(p is IAgent)pop.Add((IAgent)p);
            dynamicTree.addPoint(p);
        }
        /// <summary>
        /// Remove a plane from the octree
        /// </summary>
        /// <param name="p"></param>
        public bool removeDynamic(Particle p)
        {
            if (p is IAgent) pop.Remove((IAgent)p);
            return dynamicTree.remove(p);
        }

        public List<Vec3D> getPoints()
        {
            List<Vec3D> total = new List<Vec3D>();
            List<Vec3D> pts = dynamicTree.getPoints();
            List<Vec3D> spts = staticTree.getPoints();
            if (pts != null) total.AddRange(pts);
            if (spts != null) total.AddRange(spts);
            return total;

        }

        public List<Vec3D> searchDynamic(Vec3D pos, float radius)
        {
            List<Vec3D> dynamicPts = dynamicTree.getPointsWithinSphere(pos, radius);
            if (dynamicPts == null)
            {
                return new List<Vec3D>();
            }
            else return dynamicPts;
        }
        public List<Vec3D> searchStatic(Vec3D pos, float radius)
        {
            List<Vec3D> staticPts = staticTree.getPointsWithinSphere(pos, radius);
            if (staticPts == null)
            {
                return new List<Vec3D>();
            }
            else return staticPts;
        }
        public List<Vec3D> search(Vec3D pos, float radius)
        {
            List<Vec3D> dpts = searchDynamic(pos, radius);
            List<Vec3D> spts = searchStatic(pos, radius);
            dpts.AddRange(spts);
            return dpts;
        }
        public void addStatic(Node p)
        {
            if (p is IAgent) pop.Add((IAgent)p); 
            staticTree.addPoint(p);
        }

        public bool removeStatic(Node p)
        {
            if (p is IAgent) pop.Remove((IAgent)p);
            return staticTree.remove(p);
        }

        public void run()
        {
            run(0.94f);
        }

        public void run(float damping)
        {
            Random r = new Random();
            int steps = (int) (1 / damping);
            for (int i=0;i< steps; i++)
            {
                foreach (IAgent a in pop.OrderBy(n => r.Next())) a.step(damping);
            }
            
            //rebuild octrees
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            foreach (IAgent a in pop)
            {
                if(a is Particle)dynamicTree.addPoint((Particle)a);
            }
        }
    }
}
