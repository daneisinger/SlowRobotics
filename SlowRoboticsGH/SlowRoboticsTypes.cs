using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Agent.Types;
using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Voxels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
    public class GH_Behaviour : GH_Goo<IBehaviour>
    {
        public GH_Behaviour() { this.Value = null; }
        public GH_Behaviour(GH_Behaviour goo) { this.Value = goo.Value; }
        public GH_Behaviour(IBehaviour native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_Behaviour(this);
        public override bool IsValid => true;
        public override string TypeName => "Behaviour";
        public override string TypeDescription => "Behaviour";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is IBehaviour)
            {
                Value = source as IBehaviour;
                return true;
            }
            if (source is GH_Behaviour)
            {
                Value = ((GH_Behaviour)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_FieldElement : GH_Goo<IFieldElement>
    {
        public GH_FieldElement() { this.Value = null; }
        public GH_FieldElement(GH_FieldElement goo) { this.Value = goo.Value; }
        public GH_FieldElement(IFieldElement native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_FieldElement(this);
        public override bool IsValid => true;
        public override string TypeName => "FieldElement";
        public override string TypeDescription => "FieldElement";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IFieldElement)
            {
                Value = source as IFieldElement;
                return true;
            }
            if (source is GH_FieldElement)
            {
                Value = ((GH_FieldElement)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_Plane3D : GH_Goo<Plane3D>
    {
        public GH_Plane3D() { this.Value = null; }
        public GH_Plane3D(GH_Plane3D goo) { this.Value = goo.Value; }
        public GH_Plane3D(Plane3D native) { this.Value = native; }
      //  public GH_Plane3D(Plane native) { this.Value = new Plane3D(new Vec3D()); }
        public override IGH_Goo Duplicate() => new GH_Plane3D(this);
        public override bool IsValid => true;
        public override string TypeName => "Plane3D";
        public override string TypeDescription => "Plane3D";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        //todo - do casts

        public override bool CastFrom(object source)
        {
            if (source is GH_Plane)
            {
                Value = IO.ToPlane3D(((GH_Plane)source).Value);
                return true;
            }
            if (source is Plane3D)
            {
                Value = source as Plane3D;
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = ((GH_Plane3D)source).Value;
                return true;
            }
            return false;
        }

        //test castTo override
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(Plane3D))
            {
                target = (Q)(object)Value;
                return true;
            }
            return base.CastTo<Q>(ref target);
        }
    }

    public class GH_Node : GH_Goo<Node>
    {
        public GH_Node() { this.Value = null; }
        public GH_Node(GH_Node goo) { this.Value = goo.Value; }
        public GH_Node(Node native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Node(this);
        public override bool IsValid => true;
        public override string TypeName => "Node";
        public override string TypeDescription => "Node";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {

            if (source is GH_Plane)
            {
                Value = new Node(IO.ToPlane3D(((GH_Plane)source).Value));
                return true;
            }

            if (source is Plane3D)
            {
                Value = new Node((Plane3D)source);
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new Node(((GH_Plane3D)source).Value);
                return true;
            }
            if (source is Node)
            {
                Value = source as Node;
                return true;
            }
            if (source is GH_Node)
            {
                Value = ((GH_Node)source).Value;
                return true;
            }
            return false;
        }

    }

    public class GH_Particle : GH_Goo<SlowRobotics.Core.Particle>
    {
        public GH_Particle() { this.Value = null; }
        public GH_Particle(GH_Particle goo) { this.Value = goo.Value; }
        public GH_Particle(SlowRobotics.Core.Particle native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Particle(this);
        public override bool IsValid => true;
        public override string TypeName => "Node";
        public override string TypeDescription => "Node";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is Node)
            {
                Value = new SlowRobotics.Core.Particle((Node)source);
                return true;
            }
            if (source is GH_Node)
            {
                Value = new SlowRobotics.Core.Particle(((GH_Node)source).Value);
                return true;
            }

           
            if (source is Plane3D)
            {
                Value = new SlowRobotics.Core.Particle(new Node((Plane3D)source));
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new SlowRobotics.Core.Particle(new Node(((GH_Plane3D)source).Value));
                return true;
            }

            if (source is GH_Plane)
            {
                SlowRobotics.Core.Particle p = new SlowRobotics.Core.Particle(new Node(IO.ToPlane3D(((GH_Plane)source).Value)));
                Value = p;
                return true;
            }

            return false;
        }

    }
    public class GH_Graph : GH_Goo<Graph>
    {
        public GH_Graph() { this.Value = null; }
        public GH_Graph(GH_Graph goo) { this.Value = goo.Value; }
        public GH_Graph(Graph native) { this.Value = native; }
        public GH_Graph(Node parent) { this.Value = new Graph(parent); }

        public override IGH_Goo Duplicate() => new GH_Graph(this);
        public override bool IsValid => true;
        public override string TypeName => "LinkMesh";
        public override string TypeDescription => "LinkMesh";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is Graph)
            {
                Value = source as Graph;
                return true;
            }
            if (source is GH_Graph)
            {
                Value = ((GH_Graph)source).Value;
                return true;
            }
            if (source is GH_Node)
            {
                Value = new Graph(((GH_Node)source).Value);
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new Graph(new Node(((GH_Plane3D)source).Value));
                return true;
            }
            return false;
        }

    }

    public class GH_Agent : GH_Goo<IAgent>
    {
        public GH_Agent() { this.Value = null; }
        public GH_Agent(GH_Agent goo) { this.Value = goo.Value; }
        public GH_Agent(IAgent native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Agent(this);
        public override bool IsValid => true;
        public override string TypeName => "Agent";
        public override string TypeDescription => "Agent";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IAgent)
            {
                Value = source as IAgent;
                return true;
            }
           
            if (source is GH_Agent)
            {
                Value = ((GH_Agent)source).Value;
                return true;
            }

            if (source is IGH_Goo)
            {
                Value = new AgentT<object>(((IGH_Goo)source).ScriptVariable());
                return true;
            }

            // Value = new AgentT<object>(source);
            // return true;
            return false;

            /*
            if (source is SlowRobotics.Core.Particle)
            {
                Value = new AgentT<Node>((Node)source);
                return true;
            }

            if (source is Node)
            {
                Value = new AgentT<Node>((Node)source);
                return true;
            }
            if (source is GH_Node)
            {
                Value = new AgentT<Node>(((GH_Node)source).Value);
                return true;
            }

            if (source is GH_Plane)
            {
                Value = new ParticleAgent(new SlowRobotics.Core.Particle(IO.ToPlane3D(((GH_Plane)source).Value)));
                return true;
            }

            if (source is Plane3D)
            {
                Value = new ParticleAgent(new SlowRobotics.Core.Particle(source as Plane3D));
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new ParticleAgent(new SlowRobotics.Core.Particle(((GH_Plane3D)source).Value));
                return true;
            }*/

        }

    }

    public class GH_World : GH_Goo<IWorld>
    {
        public GH_World() { this.Value = null; }
        public GH_World(GH_World goo) { this.Value = goo.Value; }
        public GH_World(IWorld native) { this.Value = native; }
        public GH_World(float extents) { this.Value = new World(extents); }
        
        public override IGH_Goo Duplicate() => new GH_World(this);
        public override bool IsValid => true;
        public override string TypeName => "World";
        public override string TypeDescription => "World";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;


        public override bool CastFrom(object source)
        {
            if (typeof(IWorld).IsAssignableFrom(source.GetType()))
            {
                Value = (IWorld)source;
                return true;
            }
            
            if (source is IWorld)
            {
                Value = source as IWorld;
                return true;
            }
            if (source is GH_World)
            {
                Value = ((GH_World)source).Value;
                return true;
            }
            return false;
        }

    }

    public class GH_VoxelGrid : GH_Goo<IVoxelGrid> 
    {
        public GH_VoxelGrid() { this.Value = null; }
        public GH_VoxelGrid(GH_VoxelGrid goo) { this.Value = goo.Value; }
        public GH_VoxelGrid(IVoxelGrid native) { this.Value = native; }
        public GH_VoxelGrid(int num) { this.Value = new FloatGrid(num, num, num, new float[] { 0, 0, 0 }, new float []{num,num,num }); }

        public override IGH_Goo Duplicate() => new GH_VoxelGrid(this);
        public override bool IsValid => true;
        public override string TypeName => "VoxelGrid";
        public override string TypeDescription => "VoxelGrid";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is IVoxelGrid)
            {
                Value = source as IVoxelGrid;
                return true;
            }
            if (source is GH_VoxelGrid)
            {
                Value = ((GH_VoxelGrid)source).Value;
                return true;
            }
            return false;
        }

    }
}
