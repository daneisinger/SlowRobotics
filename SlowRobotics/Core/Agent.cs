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

    public class Agent : Particle
    {

        public World world;
        public List<Vec3D> neighbours;
        public PriorityQueue<Behaviour> behaviours;
        public PriorityQueue<Behaviour> neighbourBehaviours;

        public Agent(Vec3D _o, Vec3D _x, Vec3D _y, bool _f, World _world) : this(new Plane3D(_o, _x, _y),_f,_world) { }
        public Agent(Plane3D p, bool _f, World _world) : this(new Node(p), _f, _world) { }

        public Agent(Node n, bool _f, World _world) : base(n)
        {
            f = _f;
            world = _world;
            init();

            foreach(Link l in getLinks())
            {
                l.replaceNode(n, this);
            }
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

        public override void step(float damping)
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

            update(damping);
        }

        public void addBehaviour(Behaviour b)
        {
            if (b is NeighbourBehaviour)
            {
                neighbourBehaviours.Enqueue(b);
            }else if (b is AgentBehaviour) behaviours.Enqueue(b);
        }

    }
}
