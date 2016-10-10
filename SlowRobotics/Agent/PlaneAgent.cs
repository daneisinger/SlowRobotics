using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    public class PlaneAgent : Particle, Agent
    {

        public World world { get; set; }
        public List<Vec3D> neighbours;
        public PriorityQueue<Behaviour> behaviours {get; set; }

        public PlaneAgent(Vec3D _o, Vec3D _x, Vec3D _y,  World _world) : this(new Plane3D(_o, _x, _y), _world) { }
        public PlaneAgent(Plane3D p,  World _world) : this(new Node(p),_world) { }

        public PlaneAgent(Node n,  World _world) : base(n)
        {
            world = _world;
            init();

            foreach (Link l in getLinks())
            {
                l.replaceNode(n, this);
            }
        }

        public new PlaneAgent copy()
        {
            PlaneAgent b = new PlaneAgent(this, world);
            b.behaviours = behaviours;
            return b;
        }

        private void init()
        {
            neighbours = new List<Vec3D>();
            behaviours = new PriorityQueue<Behaviour>();
        }

        public void Lock()
        {
            Node staticNode = new Node(this);
            world.addStatic(staticNode);
            world.removeDynamic(this);
        }

        public override void step(float damping)
        {
            foreach (Behaviour b in behaviours.getData()) b.run(this);
            if (getInertia() == 0)
            {
                Lock();
            }
            else
            {
                update(damping);
            }
        }

        public void addBehaviour(Behaviour b)
        {
            behaviours.Enqueue(b);
        }

        public List<Behaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        public void setNeighbours(List<Vec3D> _neighbours)
        {
            neighbours = _neighbours;
        }

        public void addNeighbours(List<Vec3D> _neighbours)
        {
            neighbours.AddRange(_neighbours);
        }

        public bool hasNeighbours()
        {
            return neighbours.Count > 0;
        }

    }
}
