using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlowRoboticsGH
{
    public class BehaviourParameter : GH_PersistentParam<GH_Behaviour>
    {
        public BehaviourParameter() : base("Behaviour parameter", "Behaviour", "This is an Agent Behaviour", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        //protected override System.Drawing.Bitmap Icon => Properties.Resources.iconParamProgram;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override System.Guid ComponentGuid => new Guid("{5633a9d5-2b09-49f0-8488-dba3c169a6af}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Behaviour value)
        {
            value = new GH_Behaviour();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Behaviour> values)
        {
            values = new List<GH_Behaviour>();
            return GH_GetterResult.success;
        }
    }

    public class LinkMeshParameter : GH_PersistentParam<GH_LinkMesh>
    {
        public LinkMeshParameter() : base("LinkMesh parameter", "LinkMesh", "This is a LinkMesh", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        //protected override System.Drawing.Bitmap Icon => Properties.Resources.iconParamProgram;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override System.Guid ComponentGuid => new Guid("{de92ea40-4472-4351-a1ec-eb948d096d8e}");

        protected override GH_GetterResult Prompt_Singular(ref GH_LinkMesh value)
        {
            value = new GH_LinkMesh();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_LinkMesh> values)
        {
            values = new List<GH_LinkMesh>();
            return GH_GetterResult.success;
        }
    }

    public class AgentParameter : GH_PersistentParam<GH_Agent>
    {
        public AgentParameter() : base("Agent parameter", "Agent", "This is an Agent", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        //protected override System.Drawing.Bitmap Icon => Properties.Resources.iconParamProgram;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override System.Guid ComponentGuid => new Guid("{646ef930-37c0-492f-8a0e-41f400cb222a}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Agent value)
        {

            value = new GH_Agent();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Agent> values)
        {
            values = new List<GH_Agent>();
            return GH_GetterResult.success;
        }
    }

    public class WorldParameter : GH_PersistentParam<GH_World>
    {
        public WorldParameter() : base("World parameter", "World", "This is a World", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        //protected override System.Drawing.Bitmap Icon => Properties.Resources.iconParamProgram;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override System.Guid ComponentGuid => new Guid("{220cd95c-6506-4313-87cc-ad2000e65592}");

        protected override GH_GetterResult Prompt_Singular(ref GH_World value)
        {

            Rhino.Input.Custom.GetNumber go = new Rhino.Input.Custom.GetNumber();
            go.SetCommandPrompt("World size");
            go.AcceptNothing(true);

            switch (go.Get())
            {
                case Rhino.Input.GetResult.Number:
                    value = new GH_World((float)go.Number());
                    return GH_GetterResult.success;

                case Rhino.Input.GetResult.Nothing:
                    return GH_GetterResult.accept;

                default:
                    return GH_GetterResult.cancel;
            }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_World> values)
        {
            values = new List<GH_World>();
            return GH_GetterResult.success;
        }
    }
}
