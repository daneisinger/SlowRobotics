using System;
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

namespace SlowRoboticsGH
{
    public class CreatePopulationComponent : GH_Component
    {
        public CreatePopulationComponent() : base("Create Population", "CreatePop", "Creates an agent list for simulation", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{de35d78e-dc9a-4e2a-ae4c-708ee7ec823c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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

    public class CreateParticleComponent : GH_Component
    {
        public CreateParticleComponent() : base("Create Particle", "CreateParticle", "Creates a particle (or linearparticle) from plane and properties - you can create particles with default properties using the particle parameter", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{c878e7c2-c607-47fc-aa32-520e7f0bbdde}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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

    public class CreateKDTreeComponent : GH_Component
    {
        public CreateKDTreeComponent() : base("Create KDTree", "KDTree", "Creates a KDTree", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{76fcc476-f4d6-4f09-a31b-f9d3365989d3}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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

    public class MeshToGraph : GH_Component
    {
        public MeshToGraph() : base("Convert Mesh to Graph", "MeshToGraph", "Converts mesh edges and vertices to a graph", "Nursery", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{899106fc-bc8b-405c-bab3-21d3063b9ef9}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh To Convert", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(),"Graph", "L", "Graph", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            Mesh m = null;

            if (!DA.GetData(0, ref m)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SRParticle, Spring> g = SlowRobotics.Rhino.IO.SRConvert.MeshToGraph(m, (float)stiffness);

            DA.SetData(0, g);
        }
    }

    public class InterconnectNodesComponent : GH_Component
    {
        public InterconnectNodesComponent() : base("Interconnect Nodes", "Interconnect", "Interconnect all nodes in a link mesh", "Nursery", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{a1cac11b-f74a-4546-befd-88a304ff3eeb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "L", "Graph to contain Spings", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item, 0.15);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Graph", "L", "Graph", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph _graph = null;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SlowRobotics.Core.SRParticle, Spring> graph = _graph.Value;
            //lm.interconnectTertiaryNodes(lm.getNodes().ToList(), (float)stiffness); ----------------------TODO implement functions

            DA.SetData(0, new GH_ObjectWrapper(graph));
        }
    }

    public class ConnectByProximityComponent : GH_Component
    {
        public ConnectByProximityComponent() : base("Connect by proximity", "ConnectProximity", "Connect proximate nodes", "Nursery", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{ff9181ca-01c3-4743-ac72-362777938324}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item, 0.15);
            pManager.AddNumberParameter("Minimum Distance", "Mn", "Minimum connection distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum connection distance", GH_ParamAccess.item,1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph _graph = null;
            double minD = 0;
            double maxD = 10;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }
            if (!DA.GetData(2, ref minD)) { return; }
            if (!DA.GetData(3, ref maxD)) { return; }
            Graph<SlowRobotics.Core.SRParticle, Spring> g = _graph.Value;

            //lm.connectByProximity(lm.getNodes().ToList(), (float)minD, (float)maxD,(float)stiffness);-------------------TODO implement functions

            DA.SetData(0, new GH_ObjectWrapper(g));
        }
    }

    public class ConnectNthNodesComponent : GH_Component
    {
        public ConnectNthNodesComponent() : base("Connect Nth Nodes", "ConnectNth", "Connect nth nodes in a link mesh", "Nursery", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{9310194c-fbb8-4a29-9ae4-c58733cd75b0}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item,0.15);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph _graph = null;

            if (!DA.GetData(0, ref _graph)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph<SlowRobotics.Core.SRParticle, Spring> g = _graph.Value;
            //lm.braceNthLinks(lm.getLinks(), (float)stiffness);  ------------------------ TODO implement functions

            DA.SetData(0, new GH_ObjectWrapper(g));
        }
    }

    public class ConvertCurveToGraphComponent : GH_Component
    {
        public ConvertCurveToGraphComponent() : base("Converts curve to Graph", "CurveToGraph", "Create Linked agents by dividing a curve", "Nursery", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{67691d26-05ad-4680-a28d-666f0b83e939}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Res", "R", "Number of division points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item,0.15);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
            pManager.AddGenericParameter("Graph", "L", "Graph", GH_ParamAccess.item);
        }

        public List<IAgent> agents = new List<IAgent>();
        public Graph<SRParticle,Spring> graph = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double stiffness = 0.1;
            int res = 0;

            if (!DA.GetData(0, ref curve)) { return; }
            if (!DA.GetData(1, ref res)) { return; }

            if (!DA.GetData(5, ref stiffness)) { return; }

                //reset agent list and behaviours
                agents = new List<IAgent>();
                
                //first agent
                Plane currentPlane;
                curve.FrameAt(0, out currentPlane);
                SRParticle p1 = new SRParticle(currentPlane.ToPlane3D());
                AgentT<SRParticle> a = new AgentT<SRParticle>(p1);
                agents.Add(a);

                graph = new Graph<SRParticle, Spring>();
                graph.parent = p1;
                //all other agents
                double[] pts = curve.DivideByCount(res, true);

                for (int i = 1; i < pts.Length; i++)
                {
                    curve.FrameAt(pts[i], out currentPlane);
                    SRParticle p2 = new SRParticle(currentPlane.ToPlane3D());
                    AgentT<SRParticle> b = new AgentT<SRParticle>(p2);
                    Spring s = new Spring(a.getData(), b.getData());
                    s.s = (float)stiffness;
                    graph.insert(s);
                    agents.Add(b);
                    a = b;
                }

            DA.SetData(0, new GH_ObjectWrapper(agents));
            DA.SetData(1, new GH_ObjectWrapper(graph));
        }
    }

    public class CreatePlaneFieldElementComponent : GH_Component
    {
        public CreatePlaneFieldElementComponent() : base("Create Plane Field Element", "CreatePlaneElement", "Create plane field elements", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9d565fc7-b92d-4464-b18c-80022e8f6e1d}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Noise Location", "L", "Location of noise for doing falloff", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.item, 50);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.item,2);
            pManager.AddNumberParameter("Noise Scale", "N", "Scale of noise ", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Rotation Scale", "R", "Rotation effect of noise ", GH_ParamAccess.item,1);

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

            if (!DA.GetData(0, ref locations)) { return; }
            if (!DA.GetData(1, ref weights)) { return; }
            if (!DA.GetData(2, ref distances)) { return; }
            if (!DA.GetData(3, ref attens)) { return; }
            if (!DA.GetData(4, ref scales)) { return; }
            if (!DA.GetData(5, ref rScales)) { return; }



               IFieldElement pts = new NoiseFieldElement( new Toxiclibs.core.Vec3D((float)locations.X, (float)locations.Y, (float)locations.Z),
                   (float) weights,
                        (float)distances,
                        (float)attens,
                        (float)scales,
                        (float)rScales);


            DA.SetData(0, pts);

        }
    }

    public class CreateFieldComponent : GH_Component
    {
        public CreateFieldComponent() : base("Create Field", "CreateField", "Create Field from planes", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{32709dc6-16dc-4038-98b2-e76e565def1b}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

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

            GenericField field = new GenericField();

            for(int i = 0;i<geometry.Count;i++)
            {
                field.insertElement(geometry[i].Value);
            }
            field.updateBounds();

            DA.SetData(0, field);

        }
    }

}
