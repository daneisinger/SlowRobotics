using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using SlowRobotics.Rhino.IO;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
   public static class SR_GH_IO
    {
        public static List<IAgent> getFromWrapper(GH_ObjectWrapper wrapper)
        {
            List<IAgent> addTo = new List<IAgent>();

            if (wrapper.Value is GH_AgentList)
            {
                AgentList agents = ((GH_AgentList)wrapper.Value).Value;
                addTo = agents.getAgents();
            }
            else if (wrapper.Value is AgentList)
            {
                AgentList agents = (AgentList)wrapper.Value;
                addTo = agents.getAgents();
            }
            else if (wrapper.Value is List<IAgent>) addTo = (List<IAgent>)wrapper.Value;
            else if (wrapper.Value is IAgent) addTo.Add((IAgent)wrapper.Value);
            else if (wrapper.Value is List<GH_Agent>)
            {
                List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                foreach (GH_Agent a in agents) addTo.Add(a.Value);
            }
            else if (wrapper.Value is GH_Agent) addTo.Add(((GH_Agent)wrapper.Value).Value);

            return addTo;
        }
    }

    public class DrawNeighboursComponent : GH_Component
    {
        public DrawNeighboursComponent() : base("Draw Neighbours", "DrawNeighbours", "Draws lines to points in the agent neighbour list", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{2a3d7cd7-f1c0-4be7-ba97-a52975ab1c3f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "P", "Agents to draw connections for", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Neighbours", "N", "Neighbours", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;

            if (!DA.GetData(0, ref wrapper)) { return; }


            List<Line> connections = new List<Line>();

            foreach (IAgent agent in SR_GH_IO.getFromWrapper(wrapper))
            {
                IAgent<object> a = (IAgent<object>)agent;
                Vec3D p = a.getData() as Vec3D;
                if (p != null)
                {
                    foreach (Vec3D v in a.neighbours)
                    {
                        connections.Add(new Line(p.ToPoint3d(), v.ToPoint3d()));
                    }
                }
            }
            DA.SetDataList(0, connections);
        }
    }

    public class DeconstructAgentListComponent : GH_Component
    {
        public DeconstructAgentListComponent() : base("Deconstruct AgentList", "DeAgentList", "Deconstructs an agentlist into individual agent", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{79986af8-1d40-4c8d-87e7-24e6cecd6f3e}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(), "AgentList", "A", "AgentList to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new AgentParameter(), "Agents", "A", "Agents in list", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_AgentList agentlist = null;
            if (!DA.GetData(0, ref agentlist)) { return; }
            DA.SetDataList(0, agentlist.Value.getAgents());
        }
    }

    public class DeconstructAgentComponent : GH_Component
    {
        public DeconstructAgentComponent() : base("Deconstruct Agent", "DeAgent", "Deconstructs an agent into wrapped object data", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{a21720f3-b133-4e98-8466-60246ba461e6}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentParameter(), "Agent", "A", "Agent to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Agent agent = null;
            if (!DA.GetData(0, ref agent)) { return; }
            IAgent<object> a = (IAgent<object>)agent.Value;
            DA.SetData(0, new GH_ObjectWrapper(a.getData()));
        }
    }

    public class DeconstructGraphComponent : GH_Component
    {
        public DeconstructGraphComponent() : base("Deconstruct Graph", "DeGraph", "Deconstructs a graph into particles and springs", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{1c8b373d-aa3d-4571-b9f7-9f9d3aa9f88f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles in graph", GH_ParamAccess.list);
            pManager.AddParameter(new SpringParameter(), "Springs", "S", "Springs in graph", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Graph graph = null;

            if (!DA.GetData(0, ref graph)) { return; }

            DA.SetDataList(0, graph.Value.Geometry);
            DA.SetDataList(1, graph.Value.Edges);
        }
    }
}
