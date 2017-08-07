using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Agent.Behaviours;
using Grasshopper.Kernel.Parameters;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;
using SlowRobotics.Field;
using SlowRobotics.Core;
using Toxiclibs.core;
using SlowRobotics.Spatial;
using SlowRobotics.SRMath;
using Grasshopper.Kernel.Types;

namespace SlowRoboticsGH
{

    public class RebuildComponent : GH_Component
    {

        /*
        //todo - at the moment rebuild tree needs a reference to a collection in order
        //to handle dynamic populations... direct passing of a collection of vec3ds won't
        //update unless the grasshopper solution is expired... problem.

        protected IEnumerable<Vec3D> tryGetVector(GH_ObjectWrapper o)
        {
            if(o.Value is Vec3D)
            {
                yield return (Vec3D)o.Value;
            }else if (o.Value is GH_Plane3D)
            {
                yield return ((GH_Plane3D)o.Value).Value;
            }
            else if(o.Value is GH_Particle)
            {
                yield return ((GH_Particle)o.Value).Value.get();
            }else if (o.Value is GH_Body)
            {
                yield return ((GH_Body)o.Value).Value.get();
            }
            else if (o.Value is GH_Agent)
            {
                GH_Agent a = (GH_Agent)o.Value;
                foreach (Vec3D v in tryGetVector(a.Value)) yield return v;
            }
            else if(o.Value is GH_AgentList)
            {
                GH_AgentList al = (GH_AgentList)o.Value;
                foreach(IAgent a in al.Value.getAgents())
                {
                    foreach (Vec3D v in tryGetVector(a)) yield return v;
                }
            }
            yield return null;
        }

        protected IEnumerable<Vec3D> tryGetVector(IAgent a)
        {
            IAgent<object> ao = (IAgent<object>)a;
            if(ao!= null)
            {
                Vec3D v = (Vec3D)ao.getData();
                if (v != null) yield return v;
            }
            yield return null;
        }*/

        public RebuildComponent() : base("Rebuild Tree", "Rebuild", "ISearchable Behaviour: Rebuilds spatial structure with new objects", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{71a0c2d9-bcde-4d03-b079-245205106639}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.rebuildTree;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(), "Agents", "A", "Try and add objects from these agents", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public RebuildTree rebuildtree = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_AgentList pop = null;
            int priority = 5;

            if (!DA.GetData(0, ref pop)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (rebuildtree != null)
            {

                if (pop.Value != null) rebuildtree.pop = pop.Value;
                rebuildtree.priority = priority;

            }
            else
            {
                if (pop.Value != null) rebuildtree = new RebuildTree(priority, pop.Value);
            }

            DA.SetData(0, rebuildtree);

        }
    }

    public class IntegrateComponent : GH_Component
    {
        public IntegrateComponent() : base("Integrate", "Integrate", "SRParticle Behaviour: Integrates particle accelleration and velocity", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e2df1927-29ad-4fe0-bd86-b651aff77b92}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.integrate;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Damping", "D", "Particle Damping", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,100);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public IntegrateBehaviour integrate = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double damping = 0.1;
            int priority = 5;

            if (!DA.GetData(0, ref damping)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (damping > 1) damping = 1;
            if (integrate != null)
            {

                integrate.damping = (float)damping;
                integrate.priority = priority;

            }
            else
            {
                integrate = new IntegrateBehaviour(priority, (float)damping);

            }
            DA.SetData(0, integrate);

        }
    }
    public class SpringComponent : GH_Component
    {
        public SpringComponent() : base("Springs", "Spring", "Graph<SRParticle,Spring> Behaviour: Adds hookes law springs", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c9a35ba6-b679-446b-860e-d28244cd6360}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.spring1;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Damping", "D", "Spring Damping", GH_ParamAccess.item,1);
            pManager.AddBooleanParameter("Hookes", "H", "Update Both Particles", GH_ParamAccess.item,true);
            pManager.AddNumberParameter("Scale", "S", "Rest length scale", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public SpringBehaviour spring = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double damping = 0.1;
            bool verlet = true;
            int priority = 5;
            double rs = 1;

            if (!DA.GetData(0, ref damping)) { return; }
            if (!DA.GetData(1, ref verlet)) { return; }
            if (!DA.GetData(2, ref rs)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (spring!=null)
            {
                
                spring.damping = (float)damping;
                spring.verlet = verlet;
                spring.priority = priority;
                spring.restLengthScale = (float)rs;
                
            }
            else
            {
                spring = new SpringBehaviour(priority, (float)damping, verlet, (float)rs);
                
            }
            DA.SetData(0,spring);

        }
    }

    public class CohereInZAxisComponent : GH_Component
    {
        public CohereInZAxisComponent() : base("Attract in Z", "AttractZ", "SRParticle Interaction Behaviour: Cohere with neighbours by moving in ZAxis", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{a2d895cf-bf77-4ec1-955d-dbb5151c4884}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.attractInZ;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Cohere Strength", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for effect", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.TogetherInZ cZAxis = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.005;
            double maxDist = 7;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (cZAxis != null)
            {

                cZAxis.strength = (float)strength;
                cZAxis.maxDist = (float) maxDist;
                cZAxis.priority = priority;

            }
            else
            {
                cZAxis = new Move.TogetherInZ(priority, (float)strength, (float)maxDist);
                
            }

            DA.SetData(0, cZAxis);
        }
    }
   
    //TODO - too much overlap between this and the duplicate behaviour
    public class SpringTrailComponent : GH_Component
    {
        public SpringTrailComponent() : base("Leave Spring Trail", "SpringTrail", "Graph Behaviour: Duplicate and reconnect first node in a graph", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("{2962c6c8-c190-4a81-835f-b3b489349199}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.leaveSpringTrail;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "New Behaviours", "B", "Behaviours for duplicated agent", GH_ParamAccess.list);
            pManager.AddVectorParameter("Offset Vector", "V", "Offset with this vector before duplicating node", GH_ParamAccess.item, Vector3d.ZAxis);
            pManager.AddNumberParameter("Stiffness", "S", "Link Stiffness", GH_ParamAccess.item,0.15);
            pManager.AddIntegerParameter("Add Frequency", "F", "Add a link every n steps", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("Reset", "R", "Delete existing agents", GH_ParamAccess.item, false);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
            pManager.AddParameter(new AgentListParameter(), "New Agents", "A", "Agents", GH_ParamAccess.item);
        }

        public Add.Extend addLink = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            Vector3d offset = Vector3d.Unset;
            double stiffness = 0.1;
            int freq = 1;
            int priority = 5;
            bool reset = false;


            if (!DA.GetData(1, ref offset)) { return; }
            if (!DA.GetData(2, ref stiffness)) { return; }
            if (!DA.GetData(3, ref freq)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }
            if (!DA.GetData(5, ref reset)) { return; }


            DA.GetDataList(0, behaviours);

            if (addLink != null)
            {
                addLink.behaviours = (behaviours.ConvertAll(b => { return b.Value; }));
                addLink.stiffness = (float)stiffness;
                addLink.offset = offset.ToVec3D();
                addLink.frequency = freq;
                addLink.priority = priority;

            }
            else
            {
                addLink = new Add.Extend(priority, freq, offset.ToVec3D(), (float)stiffness, behaviours.ConvertAll(b => { return b.Value; }));
            }
            if (reset) addLink.pop = new AgentList();
            DA.SetData(0, addLink);
            DA.SetData(1, addLink.pop);
        }
    }
    
    public class SplitSpringByMinLength : GH_Component
    {
        public SplitSpringByMinLength() : base("Split Springs", "SpringSplit", "Graph Behaviour: Split springs over a given length", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("{00b9cafa-8464-4304-b1d3-e2180d194848}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.splitSprings;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "New Behaviours", "B", "Behaviours for duplicated agent", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Length", "Mx", "Max length before split", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Reset", "R", "Delete existing agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
            pManager.AddParameter(new AgentListParameter(), "New Agents", "A", "Agents", GH_ParamAccess.item);
        }

        public Add.Split addLink = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            double maxLength = 1; 
            int priority = 5;
            bool reset = false;

            if (!DA.GetDataList(0, behaviours)) { return; }
            if (!DA.GetData(1, ref maxLength)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }
            if (!DA.GetData(3, ref reset)) { return; }

            if (addLink != null)
            {
                addLink.behaviours = (behaviours.ConvertAll(b => { return b.Value; }));
                addLink.maxLength = (float)maxLength;
                addLink.priority = priority;
            }
            else
            {
                addLink = new Add.Split(priority, behaviours.ConvertAll(b => { return b.Value; }), (float) maxLength);
            }
            if (reset) addLink.pop = new AgentList();
            DA.SetData(0, addLink);
            DA.SetData(1, addLink.pop);
        }
    }
    
    public class FrictionComponent : GH_Component
    {
        public FrictionComponent() : base("Friction", "Friction", "SRParticle Interaction Behaviour: Add inertia from nearby particles", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{e75e117d-6662-49d2-a06e-e38a09f9a4a6}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.friction;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for friction effect", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Friction Coefficient", "F", "Maximum inertia modifier", GH_ParamAccess.item,0.1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Friction friction = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double maxDist = 10;
            double frictionCof = -0.1;
            int priority = 5;

            if (!DA.GetData(0, ref maxDist)) { return; }
            if (!DA.GetData(1, ref frictionCof)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (friction != null)
            {

                friction.maxDist = (float)maxDist;
                friction.frictionCof = (float)frictionCof;
                friction.priority = priority;

            }
            else
            {
                friction = new Arrest.Friction(priority, (float)maxDist, (float)frictionCof);
                
            }
            DA.SetData(0, friction);
        }
    }

    public class SlowComponent : GH_Component
    {
        public SlowComponent() : base("Slow", "Slow", "SRParticle Behaviour: Add inertia to particles over a certain age", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{00aa22df-0027-47d2-ac1c-73a750920b34}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.friction;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Minimum Age", "A", "Minimum age value before slowing", GH_ParamAccess.item,100);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Slow slow = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int minAge = 100;
            int priority = 5;

            if (!DA.GetData(0, ref minAge)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (slow != null)
            {
                slow.minAge = minAge;
                slow.priority = priority;
            }
            else
            {
                slow = new Arrest.Slow(priority, minAge);

            }
            DA.SetData(0, slow);
        }
    }

    public class FreezeComponent : GH_Component
    {
        public FreezeComponent() : base("Freeze", "Freeze", "SRParticle Behaviour: Freeze particles with low inertia and velocity", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{1cf1d232-9f49-40d3-97e5-02dd4dc769cf}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.freeze;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Minimum Inertia", "I", "Minimum Inertia value before locking", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Speed Multiplier", "S", "Effect of speed on inertia", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Minimum Age", "A", "Minimum age value before locking", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Freeze iLock = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double minInertia = 0;
            double speedFactor = 4;
            int minAge = 0;
            int priority = 5;

            if (!DA.GetData(0, ref minInertia)) { return; }
            if (!DA.GetData(1, ref speedFactor)) { return; }
            if (!DA.GetData(2, ref minAge)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (iLock != null)
            {

                iLock.minInertia = (float)minInertia;
                iLock.speedFactor = (float)speedFactor;
                iLock.minAge = minAge;
                iLock.priority = priority;

            }
            else
            {
                iLock = new Arrest.Freeze(priority, (float)minInertia, (float)speedFactor, minAge);
                
            }
            DA.SetData(0, iLock);
        }
    }

    public class ArrestGroundBehaviour : GH_Component
    {
        public ArrestGroundBehaviour() : base("Freeze ground", "Ground", "SRParticle Behaviour: Freeze particles below Z value", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{94d9995e-bb05-4172-9e9d-a4d40d3cbfbb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.freezeGround;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Minimum Z", "Z", "Minimum Z value before locking", GH_ParamAccess.item,0);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Z zLock = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double minZ = 0;
            int priority = 5;

            if (!DA.GetData(0, ref minZ)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (zLock != null)
            {

                zLock.minZ = (float) minZ;
                zLock.priority = priority;

            }
            else
            {
                zLock = new Arrest.Z(priority, (float)minZ);
                
            }
            DA.SetData(0, zLock);
        }
    }

    public class InteractComponent : GH_Component
    {

        public InteractComponent() : base("Interact", "Interact", "IAgent Behaviour: Interact with neighbouring points", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9327e8c7-a048-49af-aaaa-3c29bd5201c5}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.interact;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for interacting with other nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Interact interact = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            int priority = 5;

            if (!DA.GetDataList(0, behaviours)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }
            
            if (interact != null)
            {
                interact.setBehaviours(behaviours.ConvertAll(b=> { return b.Value; }));
                interact.priority = priority;
            }
            else
            {
                interact = new Interact(priority, behaviours.ConvertAll(b => { return b.Value; }));
            }
            DA.SetData(0, interact);
        }
    }

    public class FilterClosestNeighbourComponent : GH_Component
    {

        public FilterClosestNeighbourComponent() : base("Filter Closest", "FilterClosest", "IAgent Behaviour: Filter out all but closest neighbours", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{fb1100ba-1d65-4bd6-99f2-f3f0b14422b3}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.filterClosest;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number", "N", "Number of neighbours to keep", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public FilterClosest filter = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int num = 1;
            int priority = 5;
            if (!DA.GetData(0, ref num)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (filter != null)
            {
                filter.priority = priority;
                filter.num = num;
            }
            else
            {
                filter = new FilterClosest(priority, num);
            }
            DA.SetData(0, filter);
        }
    }

    public class FilterParentComponent : GH_Component
    {

        public FilterParentComponent() : base("Filter Parents", "FilterParent", "IAgent Behaviour: Filter out neighbours with same parent", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{42fa9ca5-e6b5-4633-b343-73325baf86ae}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.filterParents;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public FilterParents filter = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            int priority = 5;

            if (!DA.GetData(0, ref priority)) { return; }

            if (filter != null)
            {
                filter.priority = priority;
            }
            else
            {
                filter = new FilterParents(priority);
            }
            DA.SetData(0, filter);
        }
    }

    public class NewtonComponent : GH_Component
    {
        public NewtonComponent() : base("Newton", "Newton", "SRParticle Behaviour: Move particle with a force", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8974b28e-b8d7-4783-9b9f-3d4adc85e36a}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.newton;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddVectorParameter("Force", "F", "Newton Force", GH_ParamAccess.item, new Vector3d(0,0,-1));
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Newton newton = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d force = Vector3d.Unset;
            int priority = 5;

            if (!DA.GetData(0, ref force)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (newton != null)
            {
                newton.force = force.ToVec3D();
                newton.priority = priority;
            }
            else
            {
                newton = new Newton(priority, force.ToVec3D());

            }
            DA.SetData(0, newton);
        }
    }

    public class TraverseFieldComponent : GH_Component
    {
        public TraverseFieldComponent() : base("Traverse field", "TraverseField", "SRParticle Behaviour: Move a particle through a field", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("{f21c5bee-d4b2-47f6-875c-1ce463657a02}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.traverseField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "F", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.InField fieldf = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField field = null;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref field)) { return; }
            if (!DA.GetData(1, ref strength)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (fieldf != null)
            {
                fieldf.field = field;
                fieldf.strength = (float)strength;
                fieldf.priority = priority;
            }
            else
            {
                fieldf = new Move.InField(priority, field, (float)strength);

            }
            DA.SetData(0, fieldf);
        }
    }

    public class AlignFieldComponent : GH_Component
    {
        public AlignFieldComponent() : base("Align to field", "AlignField", "Plane3D Behaviour: Align a plane with a field", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("{66de0eb8-09c2-4fc7-bd61-724e7d70442e}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.alignToField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Field", "F", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "A", "Axis of field plane", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item, 0.1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisToField fieldf = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField field = null;
            int axis = 0;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref field)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref strength)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }
            
            if (fieldf != null)
            {
                fieldf.field = field;
                fieldf.axis = axis;
                fieldf.strength = (float)strength;
                fieldf.priority = priority;
            }
            else
            {
                fieldf = new Align.AxisToField(priority, field, (float)strength, axis);
                
            }
            DA.SetData(0, fieldf);
        }
    }

    public class MoveInAxisComponent : GH_Component
    {
        public MoveInAxisComponent() : base("Move in axis", "MoveAxis", "SRParticle Behaviour: Move a particle with one of its axes", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e8e65855-a4ec-46b2-b9e3-ee4fb426353f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.moveInAxis;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Axis", "A", "Axis of field plane - 0 is x, 1 is y, 2 is z", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item,0.1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.InAxis move = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int axis = 0;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref axis)) { return; }
            if (!DA.GetData(1, ref strength)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (move != null)
            {
                move.axis = axis;
                move.strength = (float)strength;
                move.priority = priority;
            }
            else
            {
                move = new Move.InAxis(priority,  (float)strength, axis);

            }
            DA.SetData(0, move);
        }
    }

   
    public class AlignAxisToVelocityComponent : GH_Component
    {

        public AlignAxisToVelocityComponent() : base("Align to velocity", "AlignVelocity", "SRParticle Behaviour: Align particle with velocity", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{f8540ae4-0554-427f-9c2f-4017d060285e}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.alignToVel;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item,0.1);
            pManager.AddIntegerParameter("Axis", "A", "Axis to align", GH_ParamAccess.item,0);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("X Axis", 0);
            param.AddNamedValue("Y Axis", 1);
            param.AddNamedValue("Z Axis", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisToVelocity aVel = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            int axis = 0;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (aVel != null)
            {
                aVel.strength = (float)strength;
                aVel.axis = axis;
                aVel.priority = priority;

            }
            else
            {
                aVel = new Align.AxisToVelocity(priority, (float)strength, axis);
                
            }
            DA.SetData(0, aVel);
        }
    }

    public class AlignPlanesComponent : GH_Component
    {

        public AlignPlanesComponent() : base("Align to planes", "AlignPlanes", "Plane3D interaction Behaviour: Align Plane with neighbours", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{d6ff6e8b-5e96-44f8-a7c8-71069f9d7bf5}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.alignToPlanes;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.Planes alignPlanes = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (alignPlanes != null)
            {
                alignPlanes.strength = (float)strength;
                alignPlanes.maxDist = (float)maxDist;
                alignPlanes.priority = priority;
            }
            else
            {
                alignPlanes = new Align.Planes(priority, (float)maxDist, (float)strength);
                
            }
            DA.SetData(0, alignPlanes);
        }
    }

    public class AlignAxisToBestFitComponent : GH_Component
    {

        public AlignAxisToBestFitComponent() : base("Align to best fit plane", "AlignBestFit", "Plane3D interaction Behaviour: Align Axis to best fit plane of nearest neighbours", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{3e0578d3-7cd5-4764-9c4d-04aa7bf0013a}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.alignToBestFit;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item,0.005);
            pManager.AddIntegerParameter("Axis", "A", "Plane axis to align", GH_ParamAccess.item,0);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("X Axis", 0);
            param.AddNamedValue("Y Axis", 1);
            param.AddNamedValue("Z Axis", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisToBestFitPlane bestFit = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.004;
            int axis = 0;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (bestFit != null)
            {
                bestFit.strength = (float)strength;
                bestFit.priority = priority;
                bestFit.axis = axis;

            }
            else
            {
                bestFit = new Align.AxisToBestFitPlane(priority,(float)strength,axis);
                
            }
            DA.SetData(0,bestFit);
        }
    }


    public class BrownianComponent : GH_Component
    {

        public BrownianComponent() : base("Brownian Motion", "Brownian", "Adds a random force to particles", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{1fdbdc42-9cef-43e9-bd33-bef99b9028fe}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.brownian;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item, 0.1);
            pManager.AddBooleanParameter("XY", "XY", "Toggle to limit random vector to World XY plane", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Brownian brownian = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            bool inXY = false;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref inXY)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (brownian != null)
            {
                brownian.strength = (float)strength;
                brownian.inXY = inXY;
                brownian.priority = priority;
            }
            else
            {
                brownian = new Brownian(priority, (float)strength, inXY);

            }
            DA.SetData(0, brownian);
        }
    }

    public class LineToLineComponent : GH_Component
    {

        public LineToLineComponent() : base("Line To Line", "LineLine", "SRLinearParticle interaction Behaviour: Attract lines to lines", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{07895539-0e74-40a0-b6f1-794a89e94f26}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.lineToLine;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.LineToLine attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.priority = priority;
            }
            else
            {
                attract = new Move.LineToLine(priority, (float)minDist, (float)maxDist, (float)strength);
            }
            DA.SetData(0, attract);
        }
    }

    public class PointToLineComponent : GH_Component
    {

        public PointToLineComponent() : base("Point To Line", "PointLine", "SRParticle interaction Behaviour: Attract particles to lines", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{099cc560-1b5b-4b94-9443-9326029e270b}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.pointToLine;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.PointToLine attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.priority = priority;
            }
            else
            {
                attract = new Move.PointToLine(priority, (float)minDist, (float)maxDist, (float)strength);
            }
            DA.SetData(0, attract);
        }
    }

    public class AttractComponent: GH_Component
    {

        public AttractComponent() : base("Attract", "Attract", "SRParticle interaction Behaviour: Attract particles", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{d3e8a92e-16f3-4de0-98aa-73ce5d948723}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.attract;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item,10);
            pManager.AddBooleanParameter("In XY", "XY", "Attract only in Plane XY", GH_ParamAccess.item,false);
            pManager.AddIntegerParameter("Falloff", "F", "Falloff Strategy", GH_ParamAccess.item,0);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
            Param_Integer param = pManager[4] as Param_Integer;

            param.AddNamedValue("No Falloff", 0);
            param.AddNamedValue("Linear Falloff", 1);
            param.AddNamedValue("Inverse Falloff", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.Together attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;
            int falloff = 0;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref falloff)) { return; }
            if (!DA.GetData(5, ref priority)) { return; }

            FalloffStrategy f;

            switch (falloff)
            {
                case (0):
                    f = new NoFalloffStrategy();
                    break;
                case (1):
                    f = new LinearFalloffStrategy();
                    break;
                case (2):
                    f = new InverseFalloffStrategy();
                    break;
                default:
                    f = new NoFalloffStrategy();
                    break;
            }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.inXY = inXY;
                attract.priority = priority;
                attract.falloff = f;
            }
            else
            {
                attract = new Move.Together(priority, (float)strength, (float)minDist, (float)maxDist, inXY);
                attract.falloff = f;
            }
            DA.SetData(0, attract);
        }
    }

    public class RepelComponent : GH_Component
    {
       
        public RepelComponent() : base("Repel", "Repel", "SRParticle interaction Behaviour: Repel particles", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{38563a34-1be4-4c68-bd94-cb4b2e15a4f2}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.repel;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Strength", "S", "Repel Strength", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Separation Distance", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Separation Distance", GH_ParamAccess.item,10);
            pManager.AddBooleanParameter("In XY", "XY", "Separate only in Plane XY", GH_ParamAccess.item,false);
            pManager.AddIntegerParameter("Falloff", "F", "Falloff Strategy", GH_ParamAccess.item, 2);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item, 0);
            Param_Integer param = pManager[4] as Param_Integer;

            param.AddNamedValue("No Falloff", 0);
            param.AddNamedValue("Linear Falloff", 1);
            param.AddNamedValue("Inverse Falloff", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.Apart separate = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;
            int falloff = 0;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref falloff)) { return; }
            if (!DA.GetData(5, ref priority)) { return; }

            FalloffStrategy f;

            switch (falloff)
            {
                case (0):
                    f = new NoFalloffStrategy();
                    break;
                case (1):
                    f = new LinearFalloffStrategy();
                    break;
                case (2):
                    f = new InverseFalloffStrategy();
                    break;
                default:
                    f = new NoFalloffStrategy();
                    break;
            }

            if (separate != null) {

                separate.inXY = inXY;
                separate.strength = (float)strength;
                separate.minDist = (float)minDist;
                separate.maxDist = (float)maxDist;
                separate.priority = priority;
                separate.falloff = f;
            }
            else
            {
                separate = new Move.Apart(priority, (float)strength, (float)minDist, (float)maxDist, inXY);
                separate.falloff = f;
            }
            DA.SetData(0, separate);
        }
    }

    public class SearchComponent : GH_Component
    {
        public SearchComponent() : base("Search", "Search", "IAgent Behaviour: Search points", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{0979e24f-914b-46cc-986a-6638ffedba71}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.search;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Radius", "R", "Search Radius", GH_ParamAccess.item,10);
            pManager.AddGenericParameter("Structure", "S", "Spatial structure to search", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Num Points", "N", "Limit number of returned points", GH_ParamAccess.item,255);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Search searchBehaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radius = 10;
            ISearchable pts = null;
            int priority = 5;
            int num = 255;

            if (!DA.GetData(0, ref radius)) { return; }
            if (!DA.GetData(1, ref pts)) { return; }
            if (!DA.GetData(2, ref num)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }


            if (searchBehaviour != null)
            {
                searchBehaviour.radius = (float)radius;
                searchBehaviour.pts = pts;
                searchBehaviour.priority = priority;
            }
            else
            {
                searchBehaviour = new Search(priority, (float)radius, pts, num);
            } 
            DA.SetData(0, searchBehaviour);
        }
    }

    public class ScaleBehaviourByDistanceToBoxComponent : GH_Component
    {
        public ScaleBehaviourByDistanceToBoxComponent() : base("Scale box", "ScaleBox", "IAgent Behaviour: Scales a list of behaviours by distance to a box", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("{6e557a31-193a-4f9a-8d94-9de59f74c03d}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.scalebox;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddBoxParameter("Box", "Bx", "Box to scale behaviours", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "Be", "Behaviours to scale", GH_ParamAccess.list);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance from box for scaling effect", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour priority", GH_ParamAccess.item,0);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Scale.ByDistToBoundingBox scaleBox = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> ghBehaviours = new List<GH_Behaviour>();
            Box b = Box.Unset;
            double maxDist = 10;
            int priority = 0;

            if (!DA.GetData(0, ref b)) { return; }
            if (!DA.GetDataList(1, ghBehaviours)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            List<IScaledBehaviour> behaviours = new List<IScaledBehaviour>();
            foreach (GH_Behaviour _b in ghBehaviours) if (_b.Value is IScaledBehaviour)behaviours.Add((IScaledBehaviour)_b.Value);

            AABB bounds = b.ToAABB();

            if (scaleBox != null)
            {
                scaleBox.box = bounds;
                scaleBox.behaviours = behaviours;
                scaleBox.maxDist = (float)maxDist;
                scaleBox.priority = priority;
            }
            else
            {
                scaleBox = new Scale.ByDistToBoundingBox(priority,behaviours,bounds,(float)maxDist);
            }
            DA.SetData(0, scaleBox);
        }
    }

    public class LeaveTraceComponent : GH_Component
    {
        public LeaveTraceComponent() : base("Leave trace", "Leave Trace", "Vec3D Behaviour: Creates a static copy of the point", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("{03a7d3c3-5494-4654-bb3c-e685b9c3a7a9}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.leaveTrace;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddIntegerParameter("Frequency", "F", "Save frequency", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item,0);
            pManager.AddGenericParameter("Structure", "S", "Structure to store point", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Add.InsertPoint addBehaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int frequency = 2;
            int priority = 0;
            ISearchable pts = null;

            if (!DA.GetData(0, ref frequency)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }
            if (!DA.GetData(2, ref pts)) { return; }

            if (addBehaviour != null)
            {
                addBehaviour.frequency = frequency;
                addBehaviour.priority = priority;
                addBehaviour.pts = pts;
            }
            else
            {
                addBehaviour = new Add.InsertPoint(priority,pts,frequency);
            }
            DA.SetData(0, addBehaviour);
        }
    }
    

}
