using Grasshopper.Kernel;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rhino.Geometry;
using SlowRobotics.Core;

namespace SlowRoboticsGH
{
    public class FieldElementParameter : GH_PersistentParam<GH_FieldElement>
    {
        public FieldElementParameter() : base("Field Element", "FieldElement", "This is a Field Element", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.fieldElement;
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

    public class FalloffParameter : GH_PersistentParam<GH_Falloff>
    {
        public FalloffParameter() : base("Falloff", "Falloff", "Falloff Strategy", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.falloff;
        public override System.Guid ComponentGuid => new Guid("{964ba95a-0cae-4ace-a21e-3bbe948d9bbf}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Falloff value)
        {
            value = new GH_Falloff();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Falloff> values)
        {
            values = new List<GH_Falloff>();
            return GH_GetterResult.success;
        }
    }

    public class FieldParameter : GH_PersistentParam<GH_Field>
    {
        public FieldParameter() : base("Field", "Field", "This is a Field", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.field;
        public override System.Guid ComponentGuid => new Guid("{450de04c-260e-416e-bc30-1d428bb1d104}");

        protected override GH_GetterResult Prompt_Singular(ref GH_Field value)
        {
            value = new GH_Field();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Field> values)
        {
            values = new List<GH_Field>();
            return GH_GetterResult.success;
        }
    }

    public class BehaviourParameter : GH_PersistentParam<GH_Behaviour>
    {
        public BehaviourParameter() : base("Behaviour", "Behaviour", "This is an Agent Behaviour", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.behaviour;
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

    public class BodyParameter : GH_PersistentParam<GH_Body>, IGH_PreviewObject
    {
        public BodyParameter() : base("Body", "Body", "This is a Body", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.body;
        public override System.Guid ComponentGuid => new Guid("{ebda6c99-eb20-4dac-9d01-1bb4b9717eee}");

        bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get { return true; }
        }

        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Body value)
        {
            value = new GH_Body();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Body> values)
        {
            values = new List<GH_Body>();
            return GH_GetterResult.success;
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
    }

    public class GraphParameter : GH_PersistentParam<GH_Graph>, IGH_PreviewObject
    {
        public GraphParameter() : base("Graph", "Graph", "This is a Graph", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.graph;
        public override System.Guid ComponentGuid => new Guid("{de92ea40-4472-4351-a1ec-eb948d096d8e}");

        bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get  { return true; }
        }

        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

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

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
    }

    public class SpringParameter : GH_PersistentParam<GH_Spring>, IGH_PreviewObject
    {
        public SpringParameter() : base("Spring", "Spring", "This is a Spring", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.spring;
        public override System.Guid ComponentGuid => new Guid("{69b8e253-7bf0-464f-af79-062550329983}");

        bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get { return true; }
        }

        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Spring value)
        {
            value = new GH_Spring();
            return GH_GetterResult.success;
        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_Spring> values)
        {
            values = new List<GH_Spring>();
            return GH_GetterResult.success;
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
    }

    public class Plane3DParameter : GH_PersistentParam<GH_Plane3D>, IGH_PreviewObject
    {
        public Plane3DParameter() : base("Plane3D", "Plane3D", "This is a Plane3D", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.plane3d;
        public override System.Guid ComponentGuid => new Guid("{0ec193e4-45a7-4029-adc6-8995aa45cc2a}");
        bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get { return true; }
        }

        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }
        protected override GH_GetterResult Prompt_Singular(ref GH_Plane3D value)
        {

            //value = new GH_Plane3D();
            return GH_GetterResult.cancel;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Plane3D> values)
        {
            //values = new List<GH_Plane3D>();
            return GH_GetterResult.cancel;
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
    }

    public class NStringParameter : GH_PersistentParam<GH_String>
    {
        public NStringParameter() : base("Nursery String", "NString", "This is a String", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources._string;
        public override System.Guid ComponentGuid => new Guid("{522f865a-4cbf-4b15-83f3-e23d44345d7a}");

        protected override GH_GetterResult Prompt_Singular(ref GH_String value)
        {

            Rhino.Input.Custom.GetString go = new Rhino.Input.Custom.GetString();
            go.SetCommandPrompt("Text");
            go.AcceptNothing(true);

            switch (go.Get())
            {
                case Rhino.Input.GetResult.String:
                    value = new GH_String(new SRString(go.GetLiteralString().ToString()));
                    return GH_GetterResult.success;

                case Rhino.Input.GetResult.Nothing:
                    return GH_GetterResult.accept;

                default:
                    return GH_GetterResult.cancel;
            }

        }
        protected override GH_GetterResult Prompt_Plural(ref List<GH_String> values)
        {
            values = new List<GH_String>();
            values.Add(new GH_String(new SRString("")));
            return GH_GetterResult.success;
        }
    }

    public class ParticleParameter : GH_PersistentParam<GH_Particle>, IGH_PreviewObject
    {
        public ParticleParameter() : base("Particle", "Particle", "This is a Particle", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.particle;
        public override System.Guid ComponentGuid => new Guid("{742c9e9f-e8a6-4fd8-a89f-ae3b590d0d4a}");

        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }

        bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        public bool IsPreviewCapable
        {
            get
            {
                return true;
            }
        }

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

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            Preview_DrawMeshes(args);
        }
    }

    public class AgentParameter : GH_PersistentParam<GH_Agent>
    {
        public AgentParameter() : base("Agent", "Agent", "This is an Agent", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.agent;
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

    public class AgentListParameter : GH_PersistentParam<GH_AgentList>
    {
        public AgentListParameter() : base("Agent List", "AgentList", "This is a List of Agents", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.agentList;
        public override System.Guid ComponentGuid => new Guid("{bf67f1ae-e5c0-4731-a606-b7f03e6d66fe}");
    
        protected override GH_GetterResult Prompt_Singular(ref GH_AgentList value)
        {

            value = new GH_AgentList();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_AgentList> values)
        {
            values = new List<GH_AgentList>();
            return GH_GetterResult.success;
        }
    }

    public class SRWrapperParameter : GH_PersistentParam<GH_SRWrapper>
    {
        public SRWrapperParameter() : base("Wrapper", "Wrapper", "Wraps an object and assigns a dictionary of properties", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.wrapper;
        public override System.Guid ComponentGuid => new Guid("{121e180b-d90e-4be2-ba2c-5d668166a0c7}");

        protected override GH_GetterResult Prompt_Singular(ref GH_SRWrapper value)
        {

            value = new GH_SRWrapper();
            return GH_GetterResult.success;

        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_SRWrapper> values)
        {
            values = new List<GH_SRWrapper>();
            return GH_GetterResult.success;
        }
    }

    /*

    //Voxels not ready for Nursery Release

    public class VoxelGridParameter : GH_PersistentParam<GH_VoxelGrid>
    {
        public VoxelGridParameter() : base("Voxel Grid", "VoxelGrid", "This is a Voxel Grid", "Nursery", "Parameters") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.voxelGrid;
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
    }*/
}
