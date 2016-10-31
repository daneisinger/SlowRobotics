using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Agent.Behaviours;
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
    public class GH_LinkMesh : GH_Goo<LinkMesh>
    {
        public GH_LinkMesh() { this.Value = null; }
        public GH_LinkMesh(GH_LinkMesh goo) { this.Value = goo.Value; }
        public GH_LinkMesh(LinkMesh native) { this.Value = native; }
        public GH_LinkMesh(Node parent) { this.Value = new LinkMesh(parent); }

        public override IGH_Goo Duplicate() => new GH_LinkMesh(this);
        public override bool IsValid => true;
        public override string TypeName => "LinkMesh";
        public override string TypeDescription => "LinkMesh";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is LinkMesh)
            {
                Value = source as LinkMesh;
                return true;
            }
            if (source is GH_LinkMesh)
            {
                Value = ((GH_LinkMesh)source).Value;
                return true;
            }
            if(source is GH_Agent)
            {
                Value = new LinkMesh((Node)((GH_Agent)source).Value);
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new LinkMesh(new Node(((GH_Plane3D)source).Value));
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
            if (source is Node)
            {
                Value = new PlaneAgent((Node)source);
                return true;
            }
            if (source is GH_Node)
            {
                Value = new PlaneAgent(((GH_Node)source).Value);
                return true;
            }
            if (source is GH_Agent)
            {
                Value = ((GH_Agent)source).Value;
                return true;
            }
            if (source is GH_Plane)
            {
                Value = new PlaneAgent(IO.ToPlane3D(((GH_Plane)source).Value));
                return true;
            }
            if (source is Plane3D)
            {
                Value = new PlaneAgent(source as Plane3D);
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = new PlaneAgent(((GH_Plane3D)source).Value);
                return true;
            }
            return false;
        }

    }

    public class GH_World : GH_Goo<IWorld>
    {
        public GH_World() { this.Value = null; }
        public GH_World(GH_World goo) { this.Value = goo.Value; }
        public GH_World(IWorld native) { this.Value = native; }
        public GH_World(float extents) { this.Value = new SimpleWorld(extents); }
        
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
