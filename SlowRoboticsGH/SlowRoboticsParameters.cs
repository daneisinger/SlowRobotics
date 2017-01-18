using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlowRoboticsGH
{
    public class FieldElementParameter : GH_PersistentParam<GH_FieldElement>
    {
        public FieldElementParameter() : base("Field Element", "FieldElement", "This is a Field Element", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{457d48c9-7010-4631-bd12-f2b27e683c55}");

        protected override GH_GetterResult Prompt_Singular(ref GH_FieldElement value)
        {
            value = new GH_FieldElement();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_FieldElement> values)
        {
            values = new List<GH_FieldElement>();
            return GH_GetterResult.success;
        }
    }

    public class BehaviourParameter : GH_PersistentParam<GH_Behaviour>
    {
        public BehaviourParameter() : base("Behaviour", "Behaviour", "This is an Agent Behaviour", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
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

    public class GraphParameter : GH_PersistentParam<GH_Graph>
    {
        public GraphParameter() : base("Graph", "Graph", "This is a Graph", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{de92ea40-4472-4351-a1ec-eb948d096d8e}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Graph value)
        {
            value = new GH_Graph();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Graph> values)
        {
            values = new List<GH_Graph>();
            return GH_GetterResult.success;
        }
    }

    /*
    public class NodeParameter : GH_PersistentParam<GH_Node>
    {
        public NodeParameter() : base("Node", "Node", "This is a Node", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{3f999a67-553e-438d-9c10-8115d04e045e}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Node value)
        {

            value = new GH_Node();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Node> values)
        {
            values = new List<GH_Node>();
            return GH_GetterResult.success;
        }
    }
    */
    public class Plane3DParameter : GH_PersistentParam<GH_Plane3D>
    {
        public Plane3DParameter() : base("Plane3D", "Plane3D", "This is a Plane3D", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{0ec193e4-45a7-4029-adc6-8995aa45cc2a}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Plane3D value)
        {

            value = new GH_Plane3D();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Plane3D> values)
        {
            values = new List<GH_Plane3D>();
            return GH_GetterResult.success;
        }
    }

    public class ParticleParameter : GH_PersistentParam<GH_Particle>
    {
        public ParticleParameter() : base("Particle", "Particle", "This is a Particle", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{742c9e9f-e8a6-4fd8-a89f-ae3b590d0d4a}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Particle value)
        {

            value = new GH_Particle();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Particle> values)
        {
            values = new List<GH_Particle>();
            return GH_GetterResult.success;
        }
    }

    public class AgentParameter : GH_PersistentParam<GH_Agent>
    {
        public AgentParameter() : base("Agent", "Agent", "This is an Agent", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
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
        public WorldParameter() : base("World", "World", "This is a World", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
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

    public class VoxelGridParameter : GH_PersistentParam<GH_VoxelGrid>
    {
        public VoxelGridParameter() : base("Voxel Grid", "VoxelGrid", "This is a Voxel Grid", "SlowRobotics", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;
        public override System.Guid ComponentGuid => new Guid("{1fa71f8b-dd8c-4345-9a08-0e5f6c75d423}");

        protected override GH_GetterResult Prompt_Singular(ref GH_VoxelGrid value)
        {

            Rhino.Input.Custom.GetNumber go = new Rhino.Input.Custom.GetNumber();
            go.SetCommandPrompt("Grid size");
            go.AcceptNothing(true);

            switch (go.Get())
            {
                case Rhino.Input.GetResult.Number:
                    value = new GH_VoxelGrid((int)go.Number());
                    return GH_GetterResult.success;

                case Rhino.Input.GetResult.Nothing:
                    return GH_GetterResult.accept;

                default:
                    return GH_GetterResult.cancel;
            }
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_VoxelGrid> values)
        {
            values = new List<GH_VoxelGrid>();
            return GH_GetterResult.success;
        }
    }
}
