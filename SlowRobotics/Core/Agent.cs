using SlowRobotics.Behaviours;
using SlowRobotics.Behaviours.NeighbourBehaviours;
using SlowRobotics.Behaviours.TrailBehaviours;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{

    public class Agent : Plane3D
    {

        public World world;
        public List<Vec3D> neighbours;
        public PriorityQueue<Behaviour> behaviours;
        public PriorityQueue<Behaviour> neighbourBehaviours;

        public Agent(Vec3D _o, Vec3D _x, Vec3D _y, bool _f, World _world) :base(_o, _x, _y)
        {
            f = _f;
            world = _world;
            init();
        }

        public Agent(Plane3D p, bool _f, World _world) :base(p)
        {
            f = _f;
            world = _world;
            init();
        }

        public new Agent copy()
        {
            Agent b = new Agent(this, f, world);
            b.behaviours = behaviours;
            b.neighbourBehaviours = neighbourBehaviours;
            return b;
        }

        private void init()
        {
            neighbours = new List<Vec3D>();
            behaviours = new PriorityQueue<Behaviour>();
            neighbourBehaviours = new PriorityQueue<Behaviour>();
        }

        
        public override void update()
        {
            //run base behaviours
            foreach (Behaviour b in behaviours.getData()) b.run(this);
            //run neighbour behaviours
            if (neighbours != null)
            {
                foreach (Agent n in neighbours)
                {
                    //run per neighbour behaviour
                    foreach (Behaviour b in neighbourBehaviours.getData()) b.run(this,n);
                }
            }

            update(0.94f);
        }

        public void addBehaviour(Behaviour b)
        {
            if (b is AgentBehaviour) behaviours.Enqueue(b);
            else if (b is NeighbourBehaviour) neighbourBehaviours.Enqueue(b);
        }

    }
}
