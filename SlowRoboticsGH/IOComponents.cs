using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Parameters;
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

    public class UnwrapComponent : GH_Component
    {
        public UnwrapComponent() : base("Unwrap", "Unwrap", "Unwraps a wrapped object into object and dictionary of properties", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{486f4df6-0d6e-4bcf-a13f-6a9c07fd882c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Unwrap;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new SRWrapperParameter(),"Wrapper", "W", "Wrapper to unwrap", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Wrapped object", GH_ParamAccess.item);
            pManager.AddGenericParameter("Properties", "P", "Wrapper properties", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_SRWrapper s = null;


            if (!DA.GetData(0, ref s)) { return; };
               

            DA.SetData(0, s.Value.data);
            DA.SetDataList(1, s.Value.properties.Values);
        }

    }

    public class DrawNeighboursComponent : GH_Component
    {
        public DrawNeighboursComponent() : base("Draw Neighbours", "DrawNeighbours", "Draws lines to points in the agent neighbour list", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{2a3d7cd7-f1c0-4be7-ba97-a52975ab1c3f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DrawNeighbours;

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

    /*

        TODO - generic deconstruct 

    public class DeconstructComponent : GH_Component, IGH_VariableParameterComponent
    {
        public DeconstructComponent() : base("Deconstruct", "Deconstruct", "Deconstructs a nursery object into subobjects", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6eb5c7de-1ba2-4eb3-8e74-4001be7da0cb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructAgentList;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Object to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper o  = null;
            if (!DA.GetData(0, ref o)) { return; }

            //destroy all output parameters
            for (int i = 0; i < Params.Output.Count; i++) {
                Params.UnregisterOutputParameter(Params.Output[i]);
            }

            if(o.Value is AgentList)
            {
                AgentListParameter param = new AgentListParameter();
                param.Name = "Agents";
                param.NickName = "A";
                param.Description = "Agents in agentlist";
                param.Access = GH_ParamAccess.list;

                Params.RegisterOutputParam(param);
                Params.OnParametersChanged();

                DA.SetDataList(0, ((AgentList)o.Value).getAgents());
              //  return;
            }

            if (o.Value is IAgent)
            {

            }
            if (o.Value is SRBody)
            {

            }
            if (o.Value is SRParticle)
            {

            }
            if (o.Value is Graph<SRParticle,Spring>)
            {

            }
            //DA.SetDataList(0, agentlist.Value.getAgents());
        }

        
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {

            Param_GenericObject param = new Param_GenericObject();
            param.Name = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Output);
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
    */

    public class DeconstructAgentListComponent : GH_Component
    {
        public DeconstructAgentListComponent() : base("Deconstruct AgentList", "DeAgentList", "Deconstructs an agentlist into individual agent", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{79986af8-1d40-4c8d-87e7-24e6cecd6f3e}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructAgentList;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructAgent;

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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructGraph;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to deconstruct", GH_ParamAccess.item);
            pManager.AddTextParameter("Tag", "T", "Get springs with tag", GH_ParamAccess.item, "");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles in graph", GH_ParamAccess.list);
            pManager.AddParameter(new SpringParameter(), "Springs", "S", "Springs in graph", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Graph graph = null;
            string tag = "";
            if (!DA.GetData(0, ref graph)) { return; }
            if (!DA.GetData(1, ref tag)) { return; }

            DA.SetDataList(0, graph.Value.Geometry);
            DA.SetDataList(1, graph.Value.Edges.Where(s => s.tag== tag));
        }
    }

    public class GraphTopologyComponent : GH_Component
    {
        public GraphTopologyComponent() : base("Graph Topology", "GraphTopology", "Gets topological representation of graph as a collection of indexes", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{56866a6b-11a5-43d6-9f97-cfb4b779a298}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GraphTopology;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GraphParameter(), "Graph", "G", "Graph to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Node Indexes", "N", "Node Indexes", GH_ParamAccess.list);
            pManager.AddTextParameter("Node Tags", "T", "Node Tags", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Start Indexes", "S", "Edge Start Indexes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("End Indexes", "E", "Edge End Indexes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Graph graph = null;

            if (!DA.GetData(0, ref graph)) { return; }
            
            DA.SetDataList(0, graph.Value.Nodes.ConvertAll(n=> n.Index));
            DA.SetDataList(1, graph.Value.Nodes.ConvertAll(n => n.Tag));
            DA.SetDataList(2, graph.Value.Edges.ConvertAll(e=> e.a.Index));
            DA.SetDataList(3, graph.Value.Edges.ConvertAll(e => e.b.Index));
        }
    }

    public class DeconstructParticleComponent : GH_Component
    {
        public DeconstructParticleComponent() : base("Deconstruct Particle", "DeParticle", "Deconstructs a particle into location, acceleration, velocity and other properties", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6d2c9bb0-68a0-4a18-a6e6-603b24562a16}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructParticles;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particle", "P", "Particle to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Location", "L", "Particle location", GH_ParamAccess.item);
            pManager.AddVectorParameter("Velocity", "V", "Particle velocity", GH_ParamAccess.item);
            pManager.AddVectorParameter("Acceleration", "A", "Particle acceleration", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mass", "M", "Particle mass", GH_ParamAccess.item);
            pManager.AddTextParameter("Tag", "T", "Particle tag", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Particle particle = null;

            if (!DA.GetData(0, ref particle)) { return; }

            DA.SetData(0, particle.Value.get().ToPlane());
            DA.SetData(1, particle.Value.vel.ToVector3d());
            DA.SetData(2, particle.Value.accel.ToVector3d());
            DA.SetData(3, particle.Value.mass);
            DA.SetData(4, particle.Value.tag);
        }
    }

    public class DeconstructBodyComponent : GH_Component
    {
        public DeconstructBodyComponent() : base("Deconstruct Body", "DeBody", "Deconstructs a body into centre of mass and body particles", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{23c4a27f-7fd0-41d6-a1bc-b466a6aa61f9}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructBody;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BodyParameter(), "Body", "B", "Body to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Centre of Mass", "C", "Centre of mass of the body", GH_ParamAccess.item);
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles contained in body", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Body body = null;

            if (!DA.GetData(0, ref body)) { return; }

            DA.SetData(0, body.Value.get().ToPlane());
            DA.SetDataList(1, body.Value.pts);
        }
    }
}
