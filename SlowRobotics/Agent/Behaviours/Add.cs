
using SlowRobotics.Core;
using SlowRobotics.Spatial;
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

        public class PointToWorld : Behaviour<Vec3D>
        {
            public int frequency { get; set; }
            public ISearchable pts {get;set;}
            int c;

            public PointToWorld(int _priority, ISearchable _pts, int _frequency) :base(_priority)
            {
                pts = _pts;
                frequency = _frequency;
                c = 0;
            }

            public override void runOn(Vec3D a)
            {
                if (c++ % frequency == 0)pts.Add(a); 
            }
        }

        public class Extend : ScaledBehaviour<Graph<SRParticle,Spring>>
        {
            public ISearchable pts { get; set; }
            public AgentList pop { get; set; }
            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            public int frequency { get; set; }
            public int ctr;
            public float stiffness { get; set; }

            public Extend(int _priority, int _frequency, Vec3D _offset, float _stiffness,List<IBehaviour> _behaviours, AgentList _pop, ISearchable _pts) : base(_priority)
            {
                pop = _pop;
                pts = _pts;
                offset = _offset;
                frequency = _frequency;
                ctr = 0;
                behaviours = _behaviours;
                stiffness = _stiffness;
            }

            public override void runOn(Graph<SRParticle,Spring> graph)
            {
                if (ctr++ % frequency == 0)
                {
                    //get head particle
                    SRParticle p = graph.parent;
                    SRParticle clone = new SRParticle(p.add(offset));
                    //point nodes to new particle
                    graph.replaceGeometry(p, clone);
                    //get the last node
                    INode<SRParticle> lastNode;
                    if(graph.getNodeAt(clone, out lastNode))
                    {
                        //make new connection if possible
                        Spring newConnection = new Spring(lastNode, new Node<SRParticle>(p));
                        graph.insert(newConnection);
                        
                    }
                    //make new agent
                    AgentT<SRParticle> a = new AgentT<SRParticle>(clone);
                    a.addBehaviours(behaviours);
                    //add to world -------------------------------------------TODO - use graph to store things instead
                    pop.add(a, clone);
                }
            }
        }
    }
}
