using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SlowRobotics.Agent;


using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.Rhino.IO;
using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using SlowRobotics.Voxels;
using System.Collections.Generic;

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

    public class GH_Field : GH_Goo<IField>
    {
        public GH_Field() { this.Value = null; }
        public GH_Field(GH_Field goo) { this.Value = goo.Value; }
        public GH_Field(IField native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Field(this);
        public override bool IsValid => true;
        public override string TypeName => "Field";
        public override string TypeDescription => "Field";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IField)
            {
                Value = source as IField;
                return true;
            }
            if (source is GH_Field)
            {
                Value = ((GH_Field)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_String : GH_Goo<SRString>
    {
        public GH_String() { this.Value = null; }
        public GH_String(GH_String goo) { this.Value = goo.Value; }
        public GH_String(SRString native) { this.Value = native; }
        //  public GH_Plane3D(Plane native) { this.Value = new Plane3D(new Vec3D()); }
        public override IGH_Goo Duplicate() => new GH_String(this);
        public override bool IsValid => true;
        public override string TypeName => "NString";
        public override string TypeDescription => "Nursery String";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        //todo - do casts

        public override bool CastFrom(object source)
        {
            if (source is GH_String)
            {
                Value = ((GH_String)source).Value;
                return true;
            }
            if (source is SRString)
            {
                Value = source as SRString;
                return true;
            }
            if (source is string)
            {
                string str;
                if(GH_Convert.ToString(source, out str, GH_Conversion.Both))
                {
                    Value = new SRString(str);
                    return true;
                }
                
                return false;
            }

            // last ditch effor to cast to string
            string tryCast;
            if(GH_Convert.ToString(source, out tryCast, GH_Conversion.Both))
            {
                Value = new SRString(tryCast);
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

    public class GH_Particle : GH_Goo<SRParticle>
    {
        public GH_Particle() { this.Value = null; }
        public GH_Particle(GH_Particle goo) { this.Value = goo.Value; }
        public GH_Particle(SRParticle native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Particle(this);
        public override bool IsValid => true;
        public override string TypeName => "Node";
        public override string TypeDescription => "Node";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is Plane3D)
            {
                Value = new SRParticle((Plane3D)source);
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new SRParticle(((GH_Plane3D)source).Value);
                return true;
            }

            if (source is GH_Plane)
            {
                SRParticle p = new SRParticle(IO.ToPlane3D(((GH_Plane)source).Value));
                Value = p;
                return true;
            }

            if(source is SRParticle)
            {
                Value = (SRParticle)source;
                return true;
            }
            return false;
        }

    }
    public class GH_Graph : GH_Goo<Graph<SRParticle, Spring>> { 

        public GH_Graph() { this.Value = null; }
        public GH_Graph(GH_Graph goo) { this.Value = goo.Value; }
        public GH_Graph(Graph<SRParticle, Spring> native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Graph(this);
        public override bool IsValid => true;
        public override string TypeName => "Spring Graph";
        public override string TypeDescription => "Spring Graph";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is Graph<SRParticle, Spring>)
            {
                Value = source as Graph<SRParticle, Spring>;
                return true;
            }
            if (source is GH_Graph)
            {
                Value = ((GH_Graph)source).Value;
                return true;
            }
            if (source is GH_Particle)
            {
                Graph<SRParticle, Spring> graph = new Graph<SRParticle, Spring>();
                SRParticle p = ((GH_Particle)source).Value;
                graph.insert(p);
                graph.parent = p;
                Value = graph;
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
                object o = ((IGH_Goo)source).ScriptVariable();
                Value = new AgentT<object>(o);
                return true;
            }
            
            if(source is GH_ObjectWrapper)
            {
                GH_ObjectWrapper wrapper = (GH_ObjectWrapper)source;

                Value = new AgentT<object>(wrapper.Value);
                return true;
            }

            Value = new AgentT<object>(source);
            return true;
           // return false;

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

    public class GH_AgentList : GH_Goo<AgentList>
    {
        public GH_AgentList() { this.Value = null; }
        public GH_AgentList(GH_AgentList goo) { this.Value = goo.Value; }
        public GH_AgentList(AgentList native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_AgentList(this);
        public override bool IsValid => true;
        public override string TypeName => "AgentList";
        public override string TypeDescription => "AgentList";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is AgentList)
            {
                Value = source as AgentList;
                return true;
            }

            if (source is GH_AgentList)
            {
                Value = ((GH_AgentList)source).Value;
                return true;
            }

            if (source is GH_Agent)
            {
                Value = new AgentList(((GH_Agent)source).Value);

                return true;
            }
            if (source is IEnumerable<GH_Agent>)
            {
                AgentList l = new AgentList();
                foreach(GH_Agent a in (IEnumerable<GH_Agent>)source)
                {
                    l.add(a.Value);
                }
                Value = l;

                return true;
            }
            if (source is IEnumerable<IAgent>)
            {
                AgentList l = new AgentList();
                l.addAll((IEnumerable<IAgent>)source);
                Value = l;

                return true;
            }
            if (source is IAgent)
            {
                Value = new AgentList((IAgent)source);
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
