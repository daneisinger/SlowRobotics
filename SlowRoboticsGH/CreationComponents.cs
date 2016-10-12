using System;
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

namespace SlowRoboticsGH
{
    public class CreateAgentsFromPlanesComponent : GH_Component
    {
        public CreateAgentsFromPlanesComponent() : base("Agents From Planes", "CreateAgents", "Create Agents from a collection of planes", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{93c48701-8b2f-43ed-8a6a-fb82ebdf3737}");
        // protected override System.Drawing.Bitmap Icon => Properties.Resources.iconCommand;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Agent Planes", GH_ParamAccess.list);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to contain agents", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Create Agents", "C", "Create the agents", GH_ParamAccess.item);
           
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
        }

        public List<IAgent> agents = new List<IAgent>();

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Plane> planes = new List<Plane>();
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            GH_World world = null;

            bool create = false;

            if (!DA.GetDataList(0, planes)) { return; }
            
            if (!DA.GetData(2, ref world)) { return; }
            if (!DA.GetData(3, ref create)) { return; }

            if(!DA.GetDataList(1, behaviours))
            {
                behaviours = new List<GH_Behaviour>();
            }

            if (create)
            {
                agents = new List<IAgent>();
                foreach (Plane3D p in planes.ConvertAll(x => { return IO.ConvertToPlane3D(x); }))
                {
                    IAgent a = new PlaneAgent(p, world.Value);
                    foreach (GH_Behaviour b in behaviours) a.addBehaviour(b.Value);
                    agents.Add(a);
                }
            }
            DA.SetData(0, new GH_ObjectWrapper(agents));

        }
    }

    public class CreateAgentsFromCurveComponent : GH_Component
    {
        public CreateAgentsFromCurveComponent() : base("Agents From Curve", "CreateAgents", "Create Linked agents by dividing a curve", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{67691d26-05ad-4680-a28d-666f0b83e939}");
        // protected override System.Drawing.Bitmap Icon => Properties.Resources.iconCommand;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Res", "R", "Number of division points", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to contain agents", GH_ParamAccess.item);
            pManager.AddParameter(new LinkMeshParameter(), "LinkMesh", "L", "LinkMesh to contain links", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Stiffness of springs between agents", GH_ParamAccess.item);
            pManager.AddNumberParameter("Bend Stiffness", "B", "Stiffness of bracing springs", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Create Agents", "C", "Create the agents", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Wrapped list of agents", GH_ParamAccess.item);
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.item);
        }

        public List<IAgent> agents = new List<IAgent>();
        public LinkMesh linkMesh = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            GH_World world = null;
            double stiffness = 0.1;
            double bendStiffness = 0.1;
           // GH_LinkMesh linkMesh = null;
            int res = 0;
            bool create = false;

            if (!DA.GetData(0, ref curve)) { return; }
            if (!DA.GetData(1, ref res)) { return; }
            if (!DA.GetData(3, ref world)) { return; }
            if (!DA.GetData(7, ref create)) { return; }
            if (!DA.GetData(5, ref stiffness)) { return; }
            if (!DA.GetData(6, ref bendStiffness)) { return; }
            if (!DA.GetDataList(2, behaviours))
            {
                behaviours = new List<GH_Behaviour>();
            }

            if (create)
            {
                //reset agent list and behaviours
                agents = new List<IAgent>();
                List<IBehaviour> agentBehaviours= new List<IBehaviour>();
                foreach (GH_Behaviour ghb in behaviours) agentBehaviours.Add(ghb.Value);
                
                //first agent
                Plane currentPlane;
                curve.FrameAt(0, out currentPlane);
                PlaneAgent a = new PlaneAgent(IO.ConvertToPlane3D(currentPlane), world.Value);
                foreach (IBehaviour _b in agentBehaviours) a.addBehaviour(_b);
                agents.Add(a);

                linkMesh = new LinkMesh(a, world.Value);
                //all other agents
                double[] pts = curve.DivideByCount(res, true);

                for (int i = 1; i < pts.Length; i++)
                {
                    curve.FrameAt(pts[i], out currentPlane);
                    PlaneAgent b = new PlaneAgent(IO.ConvertToPlane3D(currentPlane), world.Value);
                    foreach (IBehaviour _b in agentBehaviours) b.addBehaviour(_b);

                    linkMesh.connectNodes(a, b, (float)stiffness);
                    agents.Add(b);
                    a = b;
                }

                if (linkMesh.hasLinks()) linkMesh.braceNthLinks(linkMesh.getLinks(), (float)bendStiffness);
            }

            DA.SetData(0, new GH_ObjectWrapper(agents));
            DA.SetData(1, new GH_ObjectWrapper(linkMesh));
        }
    }

   

    public class CreateNodeComponent : GH_Component
    {
        public CreateNodeComponent() : base("Create Node", "CreateNode", "Create Node from Plane", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{32709dc6-16dc-4038-98b2-e76e565def1b}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Node Plane", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new NodeParameter(), "Node", "N", "Node", GH_ParamAccess.item);
        }

        public Node node = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane p = Plane.Unset;

            if (!DA.GetData(0, ref p))
            {   
                p = Plane.WorldXY;
            }
            if(node!= null)
            {
                node.set(IO.ConvertToPlane3D(p));
            }
            else
            {
                node = new Node(IO.ConvertToPlane3D(p));
            }

            DA.SetData(0, new GH_Node(node));

        }
    }

    public class CreateLinkMeshComponent : GH_Component
    {
        public CreateLinkMeshComponent() : base("Create LinkMesh", "CreateLinkMesh", "Creates a Link Mesh", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{b79c2e7d-22d4-4696-905c-d7e76a0cbec0}");
        // protected override System.Drawing.Bitmap Icon => Properties.Resources.iconCommand;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new NodeParameter(), "Parent Node", "N", "Parent node of the LinkMesh", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
            pManager.AddParameter(new WorldParameter(), "World", "W", "World containing the linkMesh", GH_ParamAccess.item);

            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LinkMesh", "L", "LinkMesh", GH_ParamAccess.item);
        }

        public LinkMesh linkMesh = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Node node = null;
            GH_World world = null;
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();

            if (!DA.GetData(0, ref node)) { return; }
            if (!DA.GetData(2, ref world)) { return; }

            if (!DA.GetDataList(1, behaviours))
            {
                behaviours = new List<GH_Behaviour>();
            }

            if (linkMesh != null)
            {
                linkMesh.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            else
            {
                linkMesh = new LinkMesh(node.Value, world.Value);
                foreach (GH_Behaviour b in behaviours) linkMesh.addBehaviour(b.Value);
            }

            DA.SetData(0, new GH_ObjectWrapper(linkMesh));
        }
    }

    public class CreateSimpleWorldComponent : GH_Component
    {
        public CreateSimpleWorldComponent() : base("Create World", "CreateWorld", "Creates a simple world", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e66c21d6-b0ad-4d81-8013-67fff231e5e4}");
        // protected override System.Drawing.Bitmap Icon => Properties.Resources.iconCommand;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

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

            DA.SetData(0, new GH_World(new SimpleWorld((float)extents)));
        }
    }
}
