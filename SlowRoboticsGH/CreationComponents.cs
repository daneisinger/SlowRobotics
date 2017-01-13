﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Agent.Behaviours;
using Grasshopper.Kernel.Parameters;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using Grasshopper.Kernel.Types;
using SlowRobotics.Rhino.MeshTools;
using SlowRobotics.Field;
using SlowRobotics.Field.Elements;
using System.Linq;
using Toxiclibs.core;
using SlowRobotics.Agent.Types;

namespace SlowRoboticsGH
{
    /*
    public class CreateAgentsFromPlanesComponent : GH_Component
    {
        public CreateAgentsFromPlanesComponent() : base("Agents From Planes", "CreateAgents", "Create Agents from a collection of planes", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{93c48701-8b2f-43ed-8a6a-fb82ebdf3737}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Agent Planes", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
        }

        public List<IAgent> agents = new List<IAgent>();

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Plane> planes = new List<Plane>();

            if (!DA.GetDataList(0, planes)) { return; }
          
                agents = new List<IAgent>();
                foreach (Plane3D p in planes.ConvertAll(x => { return IO.ToPlane3D(x); }))
                {
                    SlowRobotics.Core.Particle p1 = new SlowRobotics.Core.Particle(p);
                    IAgent a = new ParticleAgent(p1);
                    agents.Add(a);
                }

            DA.SetData(0, new GH_ObjectWrapper(agents));

        }
    }
    */
    public class MeshToLinkMeshComponent : GH_Component
    {
        public MeshToLinkMeshComponent() : base("Convert Mesh to LinkMesh", "MeshToLinkMesh", "Converts mesh edges and vertices to links", "SlowRobotics", "Agent") { }
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
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            Mesh m = null;

            if (!DA.GetData(0, ref m)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph lm = new Graph(new Node(new Vec3D()));
            GH_ObjectWrapper agents = new GH_ObjectWrapper(IO.ConvertMeshToLinkMesh(m, (float)stiffness, out lm));

            DA.SetData(0, agents);
            DA.SetData(1, new GH_ObjectWrapper(lm));
        }
    }

    public class InterconnectNodesComponent : GH_Component
    {
        public InterconnectNodesComponent() : base("Interconnect Nodes", "Interconnect", "Interconnect all nodes in a link mesh", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{a1cac11b-f74a-4546-befd-88a304ff3eeb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph linkMesh = null;

            if (!DA.GetData(0, ref linkMesh)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph lm = linkMesh.Value;
            lm.interconnectTertiaryNodes(lm.getNodes().ToList(), (float)stiffness);

            DA.SetData(0, new GH_ObjectWrapper(lm));
        }
    }

    public class ConnectByProximityComponent : GH_Component
    {
        public ConnectByProximityComponent() : base("Connect by proximity", "ConnectProximity", "Connect proximate nodes", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{ff9181ca-01c3-4743-ac72-362777938324}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
            pManager.AddNumberParameter("Minimum Distance", "Mn", "Minimum connection distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum connection distance", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph linkMesh = null;
            double minD = 0;
            double maxD = 10;

            if (!DA.GetData(0, ref linkMesh)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }
            if (!DA.GetData(2, ref minD)) { return; }
            if (!DA.GetData(3, ref maxD)) { return; }
            Graph lm = linkMesh.Value;
            lm.connectByProximity(lm.getNodes().ToList(), (float)minD, (float)maxD,(float)stiffness);

            DA.SetData(0, new GH_ObjectWrapper(lm));
        }
    }

    public class ConnectNthNodesComponent : GH_Component
    {
        public ConnectNthNodesComponent() : base("Connect Nth Nodes", "ConnectNth", "Connect nth nodes in a link mesh", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{9310194c-fbb8-4a29-9ae4-c58733cd75b0}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double stiffness = 0.1;
            GH_Graph linkMesh = null;

            if (!DA.GetData(0, ref linkMesh)) { return; }
            if (!DA.GetData(1, ref stiffness)) { return; }

            Graph lm = linkMesh.Value;
            lm.braceNthLinks(lm.getLinks(), (float)stiffness);

            DA.SetData(0, new GH_ObjectWrapper(lm));
        }
    }
    public class CurveToLinkMeshComponent : GH_Component
    {
        public CurveToLinkMeshComponent() : base("Converts curve to LinkMesh", "CurveToLinkMesh", "Create Linked agents by dividing a curve", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{67691d26-05ad-4680-a28d-666f0b83e939}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Res", "R", "Number of division points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.item);
        }

        public List<IAgent> agents = new List<IAgent>();
        public Graph linkMesh = null;

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
                SlowRobotics.Core.Particle p1 = new SlowRobotics.Core.Particle(IO.ToPlane3D(currentPlane));
                ParticleAgent a = new ParticleAgent(p1);
                agents.Add(a);

                linkMesh = new Graph(p1);
                //all other agents
                double[] pts = curve.DivideByCount(res, true);

                for (int i = 1; i < pts.Length; i++)
                {
                    curve.FrameAt(pts[i], out currentPlane);
                SlowRobotics.Core.Particle p2 = new SlowRobotics.Core.Particle(IO.ToPlane3D(currentPlane));
                ParticleAgent b = new ParticleAgent(p2);
                    linkMesh.connectNodes(a.getData(), b.getData(), (float)stiffness);
                    agents.Add(b);
                    a = b;
                }

            DA.SetData(0, new GH_ObjectWrapper(agents));
            DA.SetData(1, new GH_ObjectWrapper(linkMesh));
        }
    }

    public class CreatePlaneFieldElementComponent : GH_Component
    {
        public CreatePlaneFieldElementComponent() : base("Create Plane Field Element", "CreatePlaneElement", "Create plane field elements", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9d565fc7-b92d-4464-b18c-80022e8f6e1d}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Field Geometry", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();

            if (!DA.GetDataList(0, geometry)) { return; }
            if (!DA.GetDataList(1, weights)) { return; }
            if (!DA.GetDataList(2, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < geometry.Count; i++)
            {
                IGH_GeometricGoo g = geometry[i];
                if (g is GH_Plane)
                {
                    pts.Add(new PlaneFieldElement(
                        IO.ToPlane3D(((GH_Plane)g).Value),
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)]
                        ));
                }
            }

            DA.SetDataList(0, pts);

        }
    }

    public class CreateDistanceFieldElementComponent : GH_Component
    {
        public CreateDistanceFieldElementComponent() : base("Create Distance Field Element", "CreateDistanceElement", "Create distance field elements (attracion forces)", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6522aa26-68a2-4e16-8ce4-16766d662245}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Locations", "L", "Point defining origin of field", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();

            if (!DA.GetDataList(0, geometry)) { return; }
            if (!DA.GetDataList(1, weights)) { return; }
            if (!DA.GetDataList(2, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < geometry.Count; i++)
            {
                IGH_GeometricGoo g = geometry[i];
                if (g is GH_Point)
                {
                    Point3d pp= ((GH_Point)g).Value;
                    pts.Add(new DistanceFieldElement(
                        new Toxiclibs.core.Vec3D((float)pp.X, (float)pp.Y, (float)pp.Z),
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)]
                        ));
                }
            }

            DA.SetDataList(0, pts);

        }
    }

    public class CreateMeshFieldElementComponent : GH_Component
    {
        public CreateMeshFieldElementComponent() : base("Create Mesh Field Element", "CreateMeshElement", "Create mesh field elements", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{df15c4b0-10b3-4aa1-baac-ce85323bf87f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Meshes", "M", "Mesh defining field", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();

            if (!DA.GetDataList(0, geometry)) { return; }
            if (!DA.GetDataList(1, weights)) { return; }
            if (!DA.GetDataList(2, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < geometry.Count; i++)
            {
                IGH_GeometricGoo g = geometry[i];
                if (g is GH_Mesh)
                {
                    Mesh mesh = ((GH_Mesh)g).Value;
                    pts.Add(new MeshFieldElement(
                        mesh,
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)]
                        ));
                }
            }
            DA.SetDataList(0, pts);
        }
    }

    public class CreateBitmapFieldElementComponent : GH_Component
    {
        public CreateBitmapFieldElementComponent() : base("Create Bitmap Field Element", "CreateBitmapElement", "Create 2D bitmap field elements", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6cc8ba1f-c21b-4bdf-9ea7-98d98c14bb8c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Image File", "F", "Location of image to load as bitmap", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "Point defining 0,0 corner of bitmap", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "S", "Scale the image", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<String> files = new List<String>();
            List<Point3d> locs = new List<Point3d>();
            List<double> scales = new List<double>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();

            if (!DA.GetDataList(0, files)) { return; }
            if (!DA.GetDataList(1, locs)) { return; }
            if (!DA.GetDataList(2, scales)) { return; }
            if (!DA.GetDataList(3, weights)) { return; }
            if (!DA.GetDataList(3, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < locs.Count; i++)
            {
                string s = files[i];

                Bitmap bmp = new Bitmap(s);
                if (bmp != null)
                {
                    pts.Add(new BitmapFieldElement(
                        bmp,
                        IO.ToVec3D(locs[Math.Min(i, weights.Count - 1)]),
                        (float)scales[Math.Min(i, weights.Count - 1)],
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)]
                        ));
                }
            }
            DA.SetDataList(0, pts);
        }
    }

    public class CreatePolarFieldElementComponent : GH_Component
    {
        public CreatePolarFieldElementComponent() : base("Create Polar Field Element", "CreatePolarElement", "Create polar field elements (spin forces)", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8795cd34-4b1e-49e1-a613-9405b8676b40}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Plane defining polar field Z axis", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();

            if (!DA.GetDataList(0, geometry)) { return; }
            if (!DA.GetDataList(1, weights)) { return; }
            if (!DA.GetDataList(2, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < geometry.Count; i++)
            {
                IGH_GeometricGoo g = geometry[i];
                if (g is GH_Plane)
                {
                    pts.Add(new PolarFieldElement(
                        IO.ToPlane3D(((GH_Plane)g).Value),
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)]
                        ));
                }
            }

            DA.SetDataList(0, pts);

        }
    }

    public class CreateNoiseFieldElement : GH_Component
    {
        public CreateNoiseFieldElement() : base("Create Noise Field Element", "CreateNoiseElement", "Create noise field elements", "SlowRobotics", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c6e17c04-e71f-4a3f-85fd-ba822dd25136}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Noise Location", "L", "Location of noise for doing falloff", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weight", "W", "Field element weight", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Distance for field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Attenuation", "A", "Attenuation of field element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Noise Scale", "N", "Scale of noise ", GH_ParamAccess.list);
            pManager.AddNumberParameter("Rotation Scale", "R", "Scale of noise ", GH_ParamAccess.list);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FieldElementParameter(), "Field Elements", "F", "Field ELements", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> locations = new List<Point3d>();
            List<double> weights = new List<double>();
            List<double> distances = new List<double>();
            List<double> attens = new List<double>();
            List<double> scales = new List<double>();
            List<double> rScales = new List<double>();

            if (!DA.GetDataList(0, locations)) { return; }
            if (!DA.GetDataList(1, weights)) { return; }
            if (!DA.GetDataList(2, distances)) { return; }
            if (!DA.GetDataList(3, attens)) { return; }
            if (!DA.GetDataList(4, scales)) { return; }
            if (!DA.GetDataList(5, rScales)) { return; }

            List<IFieldElement> pts = new List<IFieldElement>();

            for (int i = 0; i < locations.Count; i++)
            {
                Point3d g = locations[i];

                    pts.Add(new NoiseFieldElement(
                        new Toxiclibs.core.Vec3D((float)g.X, (float)g.Y, (float)g.Z),
                        (float)weights[Math.Min(i, weights.Count - 1)],
                        (float)distances[Math.Min(i, distances.Count - 1)],
                        (float)attens[Math.Min(i, attens.Count - 1)],
                        (float)scales[Math.Min(i, scales.Count - 1)],
                        (float)rScales[Math.Min(i, rScales.Count - 1)]
                        ));
            }

            DA.SetDataList(0, pts);

        }
    }

    public class CreateFieldComponent : GH_Component
    {
        public CreateFieldComponent() : base("Create Field", "CreateField", "Create Field from planes", "SlowRobotics", "Field") { }
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

    public class CreateLinkMeshComponent : GH_Component
    {
        public CreateLinkMeshComponent() : base("Create LinkMesh", "CreateLinkMesh", "Creates a Link Mesh", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{b79c2e7d-22d4-4696-905c-d7e76a0cbec0}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Parent Node", "N", "Parent node of the LinkMesh", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.item);
        }

        public Graph linkMesh = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper node = null;

            if (!DA.GetData(0, ref node)) { return; }

            if (node.Value is IAgent)
            {
                linkMesh = new Graph((Node)node.Value);
            }

            else if (node.Value is Node)
            {
                linkMesh = new Graph((Node)node.Value);
            }

            else if (node.Value is Plane3D)
            {
                linkMesh = new Graph(new Node((Plane3D)node.Value));

            } 


            DA.SetData(0, new GH_ObjectWrapper(linkMesh));
        }
    }

    public class CreateSimpleWorldComponent : GH_Component
    {
        public CreateSimpleWorldComponent() : base("Create World", "CreateWorld", "Creates a simple world", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e66c21d6-b0ad-4d81-8013-67fff231e5e4}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Extents", "E", "Extents of the world from the origin", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double extents = 100;

            if (!DA.GetData(0, ref extents)) { return; }

            DA.SetData(0, new GH_World(new World((float)extents)));
        }
    }
}
