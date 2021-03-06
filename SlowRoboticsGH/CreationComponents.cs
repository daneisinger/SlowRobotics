﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using Grasshopper.Kernel.Types;
using SlowRobotics.Field;
using SlowRobotics.Field.Elements;
using SlowRobotics.SRGraph;
using SlowRobotics.Spatial;
using SlowRobotics.Rhino.GraphTools;
using SlowRobotics.Utils;
using Grasshopper.Kernel.Parameters;

namespace SlowRoboticsGH
{
    public class CreateWrapperComponent : GH_Component, IGH_VariableParameterComponent
    {
        public CreateWrapperComponent() : base("Create Wrapper", "Wrap", "Wraps an object and assigns a properties dictionary - used for creating basic 'classes' without needing to compile a dll", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{b2d03049-b5fe-4d37-9461-536ef75c7b50}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateWrapper;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Object to wrap", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new SRWrapperParameter(), "Wrapper", "W", "Wrapped object and dictionary", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper w = null;
            Dictionary<string, object> dic = new Dictionary<string, object>();

            // if (!DA.GetData(0, ref w)) { return; }
            for (int i = 0; i < Params.Input.Count; i++)
            {
                if (i == 0)
                {
                    if (!DA.GetData(i, ref w)) { return; };
                }
                else {
                    //temp object
                    object t = null;
                    if (DA.GetData(i, ref t))
                    {
                        dic.Add(Params.Input[i].Name, t);
                    }
                }
            }

            SRWrapper s = new SRWrapper(w.Value, dic);

            DA.SetData(0, new GH_SRWrapper(s));
        }

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            //add parameters to input
            if (side == GH_ParameterSide.Input)
            {
                return true;
            }
            return false;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            //leave two inputs
            if (side == GH_ParameterSide.Input)
            {
                if (Params.Input.Count > 1)
                    return true;
            }
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {

            Param_GenericObject param = new Param_GenericObject();
            param.Name = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            param.NickName = param.Name;
            param.Description = "Param" + (Params.Output.Count + 1);
            param.SetPersistentData(0.0);
            param.Access = GH_ParamAccess.item;
            return param;
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            //Params.UnregisterInputParameter(Params.Input[index + 1]);
            return true;
        }


        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            ExpireSolution(true);
        }

    }

    public class CreatePopulationComponent : GH_Component
    {
        public CreatePopulationComponent() : base("Create Population", "CreatePop", "Creates an agent list for simulation", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{de35d78e-dc9a-4e2a-ae4c-708ee7ec823c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreatePopulation;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentParameter(), "Agents", "P", "Agents to add to population", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(), "Agents", "P", "Population", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Agent> pop = new List<GH_Agent>();

            if (!DA.GetDataList(0, pop)) { return; }

            AgentList list = new AgentList();
            foreach (GH_Agent a in pop) list.add(a.Value);
            list.populate();

            DA.SetData(0, list);
        }
    }

    public class CreateBodyComponent : GH_Component
    {
        public CreateBodyComponent() : base("Create Body", "CreateBody", "Creates a body from a collection of particles and properties - you can create bodies with default properties using the Body parameter", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{05d0cfe0-545b-4ebd-9088-00b2bab42b5a}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateBody;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles in body", GH_ParamAccess.list);
            pManager.AddTextParameter("Tag", "T", "Additional data attached to the particle", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Rigid", "R", "Toggle between rigid and soft body", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BodyParameter(), "Body", "B", "Body", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Particle> particles = new List<GH_Particle>();
            string tag = "";
            bool rigid = true;

            if (!DA.GetDataList(0, particles)) { return; }
            if (!DA.GetData(1, ref tag)) { return; }
            if (!DA.GetData(2, ref rigid)) { return; }

            SRBody b = new SRBody(particles.ConvertAll(p=>p.Value), rigid);
            b.tag = tag;

            DA.SetData(0, new GH_Body(b));
        }
    }

    public class CreateParticleComponent : GH_Component
    {
        public CreateParticleComponent() : base("Create Particle", "CreateParticle", "Creates a particle (or linearparticle) from plane and properties - you can create particles with default properties using the particle parameter", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c878e7c2-c607-47fc-aa32-520e7f0bbdde}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateParticles;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Location and orientation of particle", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Mass", "M", "Mass of the particle", GH_ParamAccess.item,1);
            pManager.AddTextParameter("Tag", "T", "Additional data attached to the particle", GH_ParamAccess.item, "");
            pManager.AddNumberParameter("Length", "L", "Extent of the particle z axis for simulating line interactions", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Radius", "R", "Radius of particle sphere", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particle", "P", "Particle", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane p = Plane.Unset;
            double mass = 1;
            string tag = "";
            double length = 1;
            double radius = 0;

            if (!DA.GetData(0, ref p)) { return; }
            if (!DA.GetData(1, ref mass)) { return; }
            if (!DA.GetData(2, ref tag)) { return; }
            if (!DA.GetData(3, ref length)) { return; }
            if (!DA.GetData(4, ref radius)) { return; }

            IParticle particle = null;

            if (length == 0)
            {
                SRParticle pp = new SRParticle(p.ToPlane3D());
                particle = pp;
            }
            else
            {
                SRLinearParticle lp = new SRLinearParticle(p.ToPlane3D());
                lp.length = length;
                particle = lp;
            }

            particle.mass = (float)mass;
            particle.tag = tag;
            particle.radius = (float)radius;
            DA.SetData(0, new GH_Particle(particle));
        }
    }

    public class ModifyParticleComponent : GH_Component
    {
        public ModifyParticleComponent() : base("Modify Particle", "ModParticle", "Modifies particle properties", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{90fab20f-251d-41e0-a918-f67032ffee00}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateParticles;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particle","P","Particle to modify", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mass", "M", "Mass of the particle", GH_ParamAccess.item);
            pManager.AddTextParameter("Tag", "T", "Additional data attached to the particle", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Fix", "F", "Fix the particle", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius of particle sphere", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particle", "P", "Particle", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Particle p = null;
            double mass = 1;
            string tag = "";
            bool f = false;
            double radius = 0;

            if (!DA.GetData(0, ref p)) { return; }
            IParticle particle = p.Value;

            //update if data set
            if (DA.GetData(1, ref mass)) { particle.mass = (float) mass; }
            if (DA.GetData(2, ref tag)) { particle.tag = tag; }
            if (DA.GetData(3, ref f)) { particle.f = f; }
            if (DA.GetData(4, ref radius)) { particle.radius = (float)radius; }
            
            DA.SetData(0, new GH_Particle(particle));
        }
    }

    public class CreateKDTreeComponent : GH_Component
    {
        public CreateKDTreeComponent() : base("Create KDTree", "KDTree", "Creates a KDTree", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{76fcc476-f4d6-4f09-a31b-f9d3365989d3}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateKDTree;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Plane3DParameter(), "Plane", "P", "Location and orientation of initial points in tree", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Tree", "T", "The KDTree", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Plane3D> planes = new List<GH_Plane3D>();
            Plane3DKDTree tree = new Plane3DKDTree();
            if (DA.GetDataList(0, planes))
            {
                foreach(GH_Plane3D p in planes)
                {
                    tree.Add(p.Value);
                }
            }
            DA.SetData(0, tree);
        }
    }

    public class CreatePointCollectionComponent : GH_Component
    {
        public CreatePointCollectionComponent() : base("Create PointCollection", "PointCollection", "Creates a PointCollection for brute force search", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{fbdd53df-5ada-48c4-8928-91fedd9c2e77}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreatePointCollection;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Plane3DParameter(), "Plane", "P", "Location and orientation of initial points in collection", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Collection", "C", "The PointCollection", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Plane3D> planes = new List<GH_Plane3D>();
            PointCollection collection = new PointCollection();
            if (DA.GetDataList(0, planes))
            {
                foreach (GH_Plane3D p in planes)
                {
                    collection.Add(p.Value);
                }
            }
            DA.SetData(0, collection);
        }
    }

    public class MeshToGraph : GH_Component
    {
        public MeshToGraph() : base("Convert Mesh to Graph", "MeshToGraph", "Converts mesh edges and vertices to a graph", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{899106fc-bc8b-405c-bab3-21d3063b9ef9}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertMeshToGraph;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh To Convert", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item,0.08f);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(),"Graph", "G", "Graph", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            Mesh m = null;

            if (!DA.GetData(0, ref m)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SRParticle, Spring> g = SRConvert.MeshToGraph(m, (float)stiffness);

            DA.SetData(0, g);
        }
    }

    public class EdgesToGraph : GH_Component
    {
        public EdgesToGraph() : base("Convert Edges to Graph", "EdgesToGraph", "Converts a collection of lines into a graph", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{afebfe39-e552-4d9e-8168-398c41b5a976}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertEdgeToGraph;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Edges", "E", "Edges To Convert", GH_ParamAccess.list);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item, 0.08f);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            List<Line> m = new List<Line>();

            if (!DA.GetDataList(0, m)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SRParticle, Spring> g = SRConvert.EdgesToGraph(m, (float)stiffness);

            DA.SetData(0, g);
        }
    }

    public class InterconnectNodesComponent : GH_Component
    {
        public InterconnectNodesComponent() : base("Interconnect Nodes", "Interconnect", "Interconnect all nodes in a link mesh", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{a1cac11b-f74a-4546-befd-88a304ff3eeb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.InterconnectNodes;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to contain Spings", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item, 0.15);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph", "G", "Graph", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph _graph = null;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SRParticle, Spring> graph = _graph.Value;
            GraphUtils.interconnectNodes(graph, (float)stiffness);

            DA.SetData(0, graph);
        }
    }

    public class ConnectByProximityComponent : GH_Component
    {
        public ConnectByProximityComponent() : base("Connect by proximity", "ConnectProximity", "Create brace springs between proximate nodes in a graph", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{ff9181ca-01c3-4743-ac72-362777938324}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertByProximity;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to brace", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item, 0.08);
            pManager.AddNumberParameter("Minimum Distance", "Mn", "Minimum connection distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum connection distance", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Scale", "Rs", "Rest Length Scale", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph _graph = null;
            double minD = 0;
            double maxD = 10;
            double rls = 1;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }
            if (!DA.GetData(2, ref minD)) { return; }
            if (!DA.GetData(3, ref maxD)) { return; }
            if (!DA.GetData(4, ref rls)) { return; }
            Graph<SRParticle, Spring> g = _graph.Value;

            GraphUtils.createProximateSprings(g, (float)stiffness, (float)minD, (float)maxD, "brace", (float) rls);

            DA.SetData(0, g);
        }
    }

    public class SpanGraphEdgesComponent : GH_Component
    {
        public SpanGraphEdgesComponent() : base("Span Graph Edges", "SpanEdges", "Creates bracing springs by spanning edges in a linear graph", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{9310194c-fbb8-4a29-9ae4-c58733cd75b0}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SpanGraphEdges;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to span", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item,0.15);
            pManager.AddIntegerParameter("Degree", "D", "Creates bracing that connect d number of springs from a given node", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Scale", "Rs", "Rest Length Scale", GH_ParamAccess.item, 0.15);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            GH_Graph _graph = null;
            double stiffness = 0.1;
            int degree = 1;
            double rls = 1;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }
            if (!DA.GetData(2, ref degree)) { return; }
            if (!DA.GetData(3, ref rls)) { return; }

            Graph<SRParticle, Spring> g = _graph.Value;
            GraphUtils.spanSprings(g, (float) stiffness, (float) rls, degree);

            DA.SetData(0, g);
        }
    }

    public class ConvertCurveToGraphComponent : GH_Component
    {
        public ConvertCurveToGraphComponent() : base("Convert Curve To Graph", "CurveToGraph", "Create graph by dividing a curve", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{67691d26-05ad-4680-a28d-666f0b83e939}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertCurveToGraph;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Res", "R", "Number of division points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item,0.08f);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph", GH_ParamAccess.item);
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles", GH_ParamAccess.list);
        }

        public Graph<SRParticle, Spring> graph = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double stiffness = 0.1;
            int res = 0;

            if (!DA.GetData(0, ref curve)) { return; }
            DA.GetData(1, ref res);
            if (!DA.GetData(2, ref stiffness)) { return; }

            graph = new Graph<SRParticle, Spring>();

            if (res == 0)
            {
                SRConvert.CurveToGraph(curve, (float) stiffness, ref graph);
            }
            else
            {
                SRConvert.CurveToGraph(curve, res, (float)stiffness, ref graph);
            }

            DA.SetData(0, graph);
            DA.SetDataList(1, graph.Geometry);
        }
    }

    public class CreatePlaneFieldElementComponent : GH_Component
    {
        public CreatePlaneFieldElementComponent() : base("Create Plane Field Element", "CreatePlaneElement", "Create plane field elements", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9d565fc7-b92d-4464-b18c-80022e8f6e1d}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.planeField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "G", "Field Plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane p = Plane.Unset;
            double weights = 1;
            double distances = 50;
            double attens = 2;

            if (!DA.GetData(0, ref p)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }

                    IFieldElement pts = new PlaneFieldElement(
                        p.ToPlane3D(),
                        (float)weights,
                        (float)distances,
                        (float)attens);

            DA.SetData(0, pts);

        }
    }

    public class CreateDistanceFieldElementComponent : GH_Component
    {
        public CreateDistanceFieldElementComponent() : base("Create Distance Field Element", "CreateDistanceElement", "Create distance field elements (attracion forces)", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6522aa26-68a2-4e16-8ce4-16766d662245}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.distanceField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Locations", "L", "Point defining origin of field", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d pp = Point3d.Unset;
            double weights = 1;
            double distances = 50;
            double attens = 2;

            if (!DA.GetData(0, ref pp)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }

                   IFieldElement pts = new DistanceFieldElement(
                        new Toxiclibs.core.Vec3D((float)pp.X, (float)pp.Y, (float)pp.Z),
                        (float)weights,
                        (float)distances,
                        (float)attens);

            DA.SetData(0, pts);

        }
    }

    public class CreateMeshFieldElementComponent : GH_Component
    {
        public CreateMeshFieldElementComponent() : base("Create Mesh Field Element", "CreateMeshElement", "Create mesh field elements", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{df15c4b0-10b3-4aa1-baac-ce85323bf87f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.meshField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh defining field", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Distance for field element - weight reduces below this distance", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element - linear falloff above min distance", GH_ParamAccess.item,2);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh geometry = null;
            double weights = 1;
            double distances = 50;
            double minDist = 1;
            double attens = 2;

            if (!DA.GetData(0, ref geometry)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref minDist)) { return; }
            if (!DA.GetData(4, ref attens)) { return; }

                    IFieldElement pts = new MeshFieldElement(
                        geometry,
                        (float)weights,
                        (float)distances,
                        (float)attens, (float) minDist);
            DA.SetData(0, pts);
        }
    }

    public class CreateBitmapFieldElementComponent : GH_Component
    {
        public CreateBitmapFieldElementComponent() : base("Create Bitmap Field Element", "CreateBitmapElement", "Create 2D bitmap field elements", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6cc8ba1f-c21b-4bdf-9ea7-98d98c14bb8c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.bitmapField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Image File", "F", "Location of image to load as bitmap", GH_ParamAccess.item);
            pManager.AddPointParameter("Locations", "L", "Point defining 0,0 corner of bitmap", GH_ParamAccess.item);
            pManager.AddNumberParameter("Scale", "S", "Scale the image", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string files = "";
            Point3d locs = Point3d.Unset;
            double scales = 1;
            double weights = 1;
            double distances = 50;
            double attens = 2;

            if (!DA.GetData(0, ref files)) { return; }
            if (!DA.GetData(1, ref locs)) { return; }
            if (!DA.GetData(2, ref scales)) { return; }
            if (!DA.GetData(3, ref weights)) { return; }
            if (!DA.GetData(3, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }

            IFieldElement pts = null;
                Bitmap bmp = new Bitmap(files);
                if (bmp != null)
                {
                    pts = new BitmapFieldElement(
                        bmp,
                        locs.ToVec3D(),
                        (float)scales,
                        (float)weights,
                        (float)distances,
                        (float)attens);
                }
            DA.SetData(0, pts);
        }
    }

    public class CreatePolarFieldElementComponent : GH_Component
    {
        public CreatePolarFieldElementComponent() : base("Create Polar Field Element", "CreatePolarElement", "Create polar field elements (spin forces)", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8795cd34-4b1e-49e1-a613-9405b8676b40}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.polarField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Plane defining polar field Z axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item,50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane geometry = Plane.Unset;
            double weights = 1;
            double distances = 50;
            double attens = 2;

            if (!DA.GetData(0, ref geometry)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }


                    IFieldElement pts = new PolarFieldElement(
                        geometry.ToPlane3D(),
                        (float)weights,
                        (float)distances,
                        (float)attens);

            DA.SetData(0, pts);

        }
    }

    public class CreateNoiseFieldElement : GH_Component
    {
        public CreateNoiseFieldElement() : base("Create Noise Field Element", "CreateNoiseElement", "Create noise field elements", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c6e17c04-e71f-4a3f-85fd-ba822dd25136}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.noiseField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Noise Location", "L", "Location of noise for doing falloff", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item, 50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);
            pManager.AddNumberParameter("Noise Scale", "N", "Scale of noise ", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Rotation Scale", "R", "Rotation effect of noise ", GH_ParamAccess.item,1);
            pManager.AddBooleanParameter("XY", "XY", "Limit noise to world xy plane", GH_ParamAccess.item,true); 

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d locations = Point3d.Unset;
            double weights = 1;
            double distances = 50;
            double attens = 2;
            double scales = 0.1;
            double rScales = 1;
            bool XY = true;

            if (!DA.GetData(0, ref locations)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }
            if (!DA.GetData(4, ref scales)) { return; }
            if (!DA.GetData(5, ref rScales)) { return; }
            if (!DA.GetData(6, ref XY)) { return; }


            IFieldElement pts = new NoiseFieldElement( new Toxiclibs.core.Vec3D((float)locations.X, (float)locations.Y, (float)locations.Z),
                   (float) weights,
                        (float)distances,
                        (float)attens,
                        (float)scales,
                        (float)rScales,
                        XY);


            DA.SetData(0, pts);

        }
    }

    public class CreateFieldComponent : GH_Component
    {
        public CreateFieldComponent() : base("Create Field", "CreateField", "Create Field from planes", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{32709dc6-16dc-4038-98b2-e76e565def1b}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(),"Field Elements", "E", "Field Elements", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "F", "Field", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_FieldElement> geometry = new List<GH_FieldElement>();

            if (!DA.GetDataList(0, geometry)) { return; }

            Field field = new Field();

            for(int i = 0;i<geometry.Count;i++)
            {
                field.insertElement(geometry[i].Value);
            }
            field.updateBounds();

            DA.SetData(0, field);

        }
    }

}
