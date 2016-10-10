using Grasshopper.Kernel.Types;
using SlowRobotics.Agent;
using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRoboticsGH
{
    public class GH_Behaviour : GH_Goo<Behaviour>
    {
        public GH_Behaviour() { this.Value = null; }
        public GH_Behaviour(GH_Behaviour goo) { this.Value = goo.Value; }
        public GH_Behaviour(Behaviour native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_Behaviour(this);
        public override bool IsValid => true;
        public override string TypeName => "Behaviour";
        public override string TypeDescription => "Behaviour";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is Behaviour)
            {
                Value = source as Behaviour;
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

    public class GH_LinkMesh : GH_Goo<LinkMesh>
    {
        public GH_LinkMesh() { this.Value = null; }
        public GH_LinkMesh(GH_LinkMesh goo) { this.Value = goo.Value; }
        public GH_LinkMesh(LinkMesh native) { this.Value = native; }
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
            return false;
        }

    }

    public class GH_Agent : GH_Goo<Agent>
    {
        public GH_Agent() { this.Value = null; }
        public GH_Agent(GH_Agent goo) { this.Value = goo.Value; }
        public GH_Agent(Agent native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Agent(this);
        public override bool IsValid => true;
        public override string TypeName => "Agent";
        public override string TypeDescription => "Agent";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is Agent)
            {
                Value = source as Agent;
                return true;
            }
            if (source is GH_Agent)
            {
                Value = ((GH_Agent)source).Value;
                return true;
            }
            return false;
        }

    }

    public class GH_World : GH_Goo<World>
    {
        public GH_World() { this.Value = null; }
        public GH_World(GH_World goo) { this.Value = goo.Value; }
        public GH_World(World native) { this.Value = native; }
        public GH_World(float extents) { this.Value = new SimpleWorld(extents); }

        public override IGH_Goo Duplicate() => new GH_World(this);
        public override bool IsValid => true;
        public override string TypeName => "World";
        public override string TypeDescription => "World";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is World)
            {
                Value = source as World;
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
}
