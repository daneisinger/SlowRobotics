
using SlowRobotics.Agent.Types;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Add
    {

        public class PointToWorld : Behaviour<Vec3D>, IWorldBehaviour
        {
            public int frequency { get; set; }
            public IWorld world {get;set;}
            int c;

            public PointToWorld(int _priority, IWorld _world, int _frequency) :base(_priority)
            {
                world = _world;
                frequency = _frequency;
                c = 0;
            }

            public override void runOn(Vec3D a)
            {
                if (c++ % frequency == 0)world.addPoint(a,false); 
            }
        }

        public class Extend : ScaledBehaviour<Graph<Particle,Spring>>, IWorldBehaviour
        {
            public IWorld world { get; set; }

            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            public int frequency { get; set; }
            public int ctr;
            public float stiffness { get; set; }

            public Extend(int _priority, int _frequency, Vec3D _offset, float _stiffness,List<IBehaviour> _behaviours, IWorld _world) : base(_priority)
            {
                world = _world;
                offset = _offset;
                frequency = _frequency;
                ctr = 0;
                behaviours = _behaviours;
                stiffness = _stiffness;
            }

            public override void runOn(Graph<Particle,Spring> graph)
            {
                if (ctr++ % frequency == 0)
                {
                    //get head particle
                    Particle p = graph.parent;
                    Particle clone = new Particle(p.add(offset));
                    //point nodes to new particle
                    graph.replaceGeometry(p, clone);
                    //get the last node
                    INode<Particle> lastNode;
                    if(graph.getNodeAt(clone, out lastNode))
                    {
                        //make new connection if possible
                        Spring newConnection = new Spring(lastNode, new Node<Particle>(p));
                        graph.insert(newConnection);
                        
                    }
                    //make new agent
                    ParticleAgent a = new ParticleAgent(clone);
                    a.addBehaviours(behaviours);
                    //add to world -------------------------------------------TODO - use graph to store things instead
                    world.addAgent(a);
                    world.addPoint(clone, true);
                }
            }
        }
    }
}
