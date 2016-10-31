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
    public class PlaneAgent : Particle, IGraphAgent, IParticleAgent, IStateAgent
    {
        public List<Vec3D> neighbours { get; set; }
        public PriorityQueue<IBehaviour> behaviours { get; set; }

        public PlaneAgent(Vec3D _o, Vec3D _x, Vec3D _y) : this(new Plane3D(_o, _x, _y)) { }
        public PlaneAgent(Plane3D p) : this(new Node(p)) { }

        public PlaneAgent(Node n) : base(n)
        {

            init();
            foreach (Link l in getLinks())
            {
                l.replaceNode(n, this);
            }
        }

        public new IGraphAgent copy()
        {
            PlaneAgent b = new PlaneAgent(this);
            b.behaviours = behaviours;
            return b;
        }

        private void init()
        {
            neighbours = new List<Vec3D>();
            behaviours = new PriorityQueue<IBehaviour>();
        }

        public override void step(float damping)
        {
            foreach (IBehaviour b in behaviours.getData()) b.run(this);
            update(damping);
        }

        public void addBehaviour(IBehaviour b)
        {
            behaviours.Enqueue(b);
        }

        public void addBehaviours(List<IBehaviour> newBehaviours)
        {
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public void addNeighbours(List<Vec3D> _neighbours)
        {
            neighbours.AddRange(_neighbours);
        }

        public bool hasNeighbours()
        {
            return neighbours.Count > 0;
        }

        public Particle getParticle()
        {
            return this;
        }
    }
}
