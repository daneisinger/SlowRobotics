using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Agent.Behaviours;
using Grasshopper.Kernel.Parameters;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;

namespace SlowRoboticsGH
{
    public class SpringComponent : GH_Component
    {
        public SpringComponent() : base("Spring Behaviour", "Spring", "Verlet Spring Behaviours", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c9a35ba6-b679-446b-860e-d28244cd6360}");
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
            pManager.AddNumberParameter("Damping", "D", "Spring Damping", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Verlet", "V", "Update Both Particles", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Spring spring = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double damping = 0.1;
            bool verlet = true;
            int priority = 5;

            if (!DA.GetData(0, ref damping)) { return; }
            if (!DA.GetData(1, ref verlet)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (spring!=null)
            {
                
                spring.damping = (float)damping;
                spring.verlet = verlet;
                spring.priority = priority;
                
            }
            else
            {
                spring = new Spring(priority, (float)damping, verlet);
                
            }
            DA.SetData(0,spring);

        }
    }

    public class AlignXXToNearLinksComponent : GH_Component
    {
        public AlignXXToNearLinksComponent() : base("Align XX With Near Links", "AlignNearLinks", "Align the X axis of a plane with the direction of neighbouring links", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{d612542b-e64f-4249-8a77-7c04fbe4028c}");
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
            pManager.AddNumberParameter("Strength", "S", "Cohere Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for effect", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use Parent", "U", "Include links that share a parent node with this agent", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AlignXXToNearLinks alignNearLinks = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            bool useParent = true;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref useParent)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (alignNearLinks != null)
            {

                alignNearLinks.strength = (float)strength;
                alignNearLinks.searchRadius = (float)maxDist;
                alignNearLinks.useParent = useParent;
                alignNearLinks.priority = priority;

            }
            else
            {
                alignNearLinks = new AlignXXToNearLinks(priority, (float)maxDist, (float)strength, useParent);
                
            }
            DA.SetData(0, alignNearLinks);

        }
    }

    public class CohereInZAxisComponent : GH_Component
    {
        public CohereInZAxisComponent() : base("Cohere in ZAxis Behaviour", "CohereZ", "Cohere with neighbours by moving in ZAxis", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{a2d895cf-bf77-4ec1-955d-dbb5151c4884}");
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
            pManager.AddNumberParameter("Strength", "S", "Cohere Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public CohereInZAxis cZAxis = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
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
                cZAxis = new CohereInZAxis(priority, (float)strength, (float)maxDist);
                
            }

            DA.SetData(0, cZAxis);
        }
    }

    public class CohereToNearestLinkComponent : GH_Component
    {
        public CohereToNearestLinkComponent() : base("Cohere to Links Behaviour", "CohereLinks", "Cohere to nearest point on links", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{f23407b5-4513-4f52-97da-6b36b3ecc514}");
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
            pManager.AddParameter(new LinkMeshParameter(), "Parent LinkMesh", "P", "LinkMesh to parent links to", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Cohere Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum Distance to effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public CohereToNearestLink cLink = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            LinkMesh parent = null;
            double strength = 0.1;
            double maxDist = 10;
            int priority = 5;
            //Object b = null;

            if (!DA.GetData(0, ref parent)) { return; }
            if (!DA.GetData(1, ref strength)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (cLink != null)
            {

                cLink.parent = parent;
                cLink.strength = (float)strength;
                cLink.searchRadius = (float)maxDist;
                cLink.priority = priority;
            }
            else
            {
                cLink = new CohereToNearestLink(priority, parent, (float) maxDist, (float) strength);
                
            }
            DA.SetData(0, cLink);
        }
    }

    //TODO - too much overlap between this and the duplicate behaviour
    public class AddLinkComponent : GH_Component
    {
        public AddLinkComponent() : base("Add Link Behaviour", "AddLink", "Duplicate an agent and create a braced link", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2962c6c8-c190-4a81-835f-b3b489349199}");
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
            pManager.AddParameter(new BehaviourParameter(), "New Behaviours", "B", "Behaviours for duplicated agent", GH_ParamAccess.list);
            pManager.AddParameter(new LinkMeshParameter(), "Parent LinkMesh", "P", "LinkMesh to parent links to", GH_ParamAccess.item);
            pManager.AddVectorParameter("Offset Vector", "V", "Offset with this vector before duplicating node", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Link Stiffness", GH_ParamAccess.item);
            pManager.AddNumberParameter("Brace Stiffness", "Bs", "Bracing Link Stiffness", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Add Frequency", "F", "Add a link every n steps", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Add Dynamic", "D", "Toggle between adding a static or dynamic node", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Brace", "B", "Try to add brace springs for bend resistance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);

    }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AddLink addLink = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            LinkMesh parent = null;
            Vector3d offset = Vector3d.Unset;
            double stiffness = 0.1;
            double braceStiffness = 0.05;
            int freq = 1;
            bool dynamic = true;
            bool brace = true;
            int priority = 5;
            //Object b = null;

            if (!DA.GetDataList(0, behaviours)) { return; }
            if (!DA.GetData(2, ref parent)) { return; }
            if (!DA.GetData(3, ref offset)) { return; }
            if (!DA.GetData(4, ref stiffness)) { return; }
            if (!DA.GetData(5, ref braceStiffness)) { return; }
            if (!DA.GetData(6, ref freq)) { return; }
            if (!DA.GetData(7, ref dynamic)) { return; }
            if (!DA.GetData(8, ref brace)) { return; }
            if (!DA.GetData(9, ref priority)) { return; }

            if (addLink != null)
            {
                addLink.behaviours = (behaviours.ConvertAll(b => { return b.Value; }));
                addLink.parent = parent;
                addLink.stiffness = (float)stiffness;
                addLink.braceStiffness = (float)braceStiffness;
                addLink.offset = IO.ConvertToVec3D(offset);
                addLink.frequency = freq;
                addLink.dynamic = dynamic;
                addLink.tryToBrace = brace;
                addLink.priority = priority;
            }
            else
            {
                addLink = new AddLink(priority, parent, brace, freq, IO.ConvertToVec3D(offset), (float)stiffness, (float) braceStiffness, behaviours.ConvertAll(b => { return b.Value; }),dynamic);
                
            }
            DA.SetData(0, addLink);
        }
    }

    public class FrictionComponent : GH_Component
    {
        public FrictionComponent() : base("Friction Behaviour", "Friction", "Add inertia from nearby particles", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e75e117d-6662-49d2-a06e-e38a09f9a4a6}");
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
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for friction effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Friction Coefficient", "F", "Maximum inertia modifier", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Friction friction = null;

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
                friction = new Friction(priority, (float)maxDist, (float)frictionCof);
                
            }
            DA.SetData(0, friction);
        }
    }

    public class InertiaLockComponent : GH_Component
    {
        public InertiaLockComponent() : base("Inertia Lock Behaviour", "InertiaLock", "Lock agents with low inertia value", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{1cf1d232-9f49-40d3-97e5-02dd4dc769cf}");
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
            pManager.AddNumberParameter("Minimum Inertia", "I", "Minimum Inertia value before locking", GH_ParamAccess.item);
            pManager.AddNumberParameter("Speed Multiplier", "S", "Effect of speed on inertia", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Minimum Age", "A", "Minimum age value before locking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public InertiaLock iLock = null;

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
                iLock = new InertiaLock(priority, (float)minInertia, (float)speedFactor, minAge);
                
            }
            DA.SetData(0, iLock);
        }
    }

    public class ZLockComponent : GH_Component
    {
        public ZLockComponent() : base("ZLock Behaviour", "ZLock", "Lock agents below Z value", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{94d9995e-bb05-4172-9e9d-a4d40d3cbfbb}");
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
            pManager.AddNumberParameter("Minimum Z", "Z", "Minimum Z value before locking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public ZLock zLock = null;

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
                zLock = new ZLock(priority, (float)minZ);
                
            }
            DA.SetData(0, zLock);
        }
    }

    public class InteractComponent : GH_Component
    {

        public InteractComponent() : base("Interact Behaviour", "Interact", "Interact with neighbouring nodes", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9327e8c7-a048-49af-aaaa-3c29bd5201c5}");
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
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for interacting with other nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
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

    public class NewtonComponent : GH_Component
    {
        public NewtonComponent() : base("Newton Behaviour", "Newton", "Move particle with a force", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8974b28e-b8d7-4783-9b9f-3d4adc85e36a}");
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
            pManager.AddVectorParameter("Force", "F", "Newton Force", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
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
                newton.force = IO.ConvertToVec3D(force);
                newton.priority = priority;
            }
            else
            {
                newton = new Newton(priority, IO.ConvertToVec3D(force));
                
            }
            DA.SetData(0, newton);
        }
    }

    public class AlignAxisToLinksComponent : GH_Component
    {

        public AlignAxisToLinksComponent() : base("Align Links Behaviour", "AlignLinks", "Align Plane with connected links", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{3d1a1fdf-84ce-41dc-822c-87484b9acf5f}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "A", "Axis to align", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("X Axis", 0);
            param.AddNamedValue("Y Axis", 1);
            param.AddNamedValue("Z Axis", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AlignAxisToLinks aAxis = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            int axis = 0;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (aAxis != null)
            {
                aAxis.strength = (float)strength;
                aAxis.axis = axis;
                aAxis.priority = priority;

            }
            else
            {
                aAxis = new AlignAxisToLinks(priority, (float)strength, axis);
               
            }
            DA.SetData(0, aAxis);
        }
    }

    public class AlignAxisToVelocityComponent : GH_Component
    {

        public AlignAxisToVelocityComponent() : base("Align Velocity Behaviour", "AlignVelocity", "Align Plane with velocity", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{f8540ae4-0554-427f-9c2f-4017d060285e}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "A", "Axis to align", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("X Axis", 0);
            param.AddNamedValue("Y Axis", 1);
            param.AddNamedValue("Z Axis", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AlignAxisToVelocity aVel = null;

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
                aVel = new AlignAxisToVelocity(priority, (float)strength, axis);
                
            }
            DA.SetData(0, aVel);
        }
    }

    public class AlignPlanesComponent : GH_Component
    {

        public AlignPlanesComponent() : base("Align Planes Behaviour", "AlignPlanes", "Align Plane with neighbours", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{d6ff6e8b-5e96-44f8-a7c8-71069f9d7bf5}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AlignPlanes alignPlanes = null;

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
                alignPlanes.orientToNeighbour = (float)strength;
                alignPlanes.maxDist = (float)maxDist;
                alignPlanes.priority = priority;
            }
            else
            {
                alignPlanes = new AlignPlanes(priority, (float)maxDist, (float)strength);
                
            }
            DA.SetData(0, alignPlanes);
        }
    }

    public class AlignZZToBestFitComponent : GH_Component
    {

        public AlignZZToBestFitComponent() : base("Align Best Fit Behaviour", "AlignBestFit", "Align Plane Z Axis to best fit plane of nearest neighbours", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{3e0578d3-7cd5-4764-9c4d-04aa7bf0013a}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public AlignZZToBestFit bestFit = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (bestFit != null)
            {
                bestFit.orientToBestFit = (float)strength;
                bestFit.maxDist = (float)maxDist;
                bestFit.priority = priority;

            }
            else
            {
                bestFit = new AlignZZToBestFit(priority, (float)maxDist, (float)strength);
                
            }
            DA.SetData(0,bestFit);
        }
    }

    public class AttractComponent: GH_Component
    {

        public AttractComponent() : base("Attract Behaviour", "Attract", "Attract Agents", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{d3e8a92e-16f3-4de0-98aa-73ce5d948723}");
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
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddBooleanParameter("In XY", "XY", "Attract only in Plane XY", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Attract attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }

            if (attract != null)
            {

                attract.inXY = inXY;
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.priority = priority;

            }
            else
            {
                attract = new Attract(priority, (float)minDist, (float)maxDist, inXY, (float)strength);
                
            }
            DA.SetData(0, attract);
        }
    }

    public class SeparateComponent : GH_Component
    {
       

        public SeparateComponent() : base("Separate Behaviour", "Separate", "Separate Agents", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{38563a34-1be4-4c68-bd94-cb4b2e15a4f2}");
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
            pManager.AddNumberParameter("Strength", "S", "Seperation Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Separation Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Separation Distance", GH_ParamAccess.item);
            pManager.AddBooleanParameter("In XY", "XY", "Separate only in Plane XY", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Separate separate = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }

            if (separate != null) {

                separate.inXY = inXY;
                separate.strength = (float)strength;
                separate.minDist = (float)minDist;
                separate.maxDist = (float)maxDist;
                separate.priority = priority;

            }
            else
            {
                separate = new Separate(priority, (float)minDist, (float)maxDist, inXY, (float)strength);
            }
            DA.SetData(0, separate);
        }
    }

    public class SearchComponent : GH_Component
    {
        public SearchComponent() : base("Search Behaviour", "Search", "Search Nodes", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{0979e24f-914b-46cc-986a-6638ffedba71}");
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

            pManager.AddNumberParameter("Radius", "R", "Search Radius", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Search Method", "S", "Search Method", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("Dynamic", 0);
            param.AddNamedValue("Static", 1);
            param.AddNamedValue("All", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Search searchBehaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radius = 10;
            int method = 2;
            int priority = 5;

            if (!DA.GetData(0, ref radius)) { return; }
            if (!DA.GetData(1, ref method)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            
            Search.SearchMethod search = Search.SearchMethod.All;

            switch (method)
            {
                case (0):
                    search= Search.SearchMethod.Dynamic;
                    break;
                case (1):
                    search = Search.SearchMethod.Static;
                    break;
                case (2):
                    search = Search.SearchMethod.All;
                    break;
                default:
                    search = Search.SearchMethod.All;
                    break;
            }

            if (searchBehaviour != null)
            {
                searchBehaviour.dynamicRadius = (float)radius;
                searchBehaviour.staticRadius = (float)radius;
                searchBehaviour.method = search;
                searchBehaviour.priority = priority;
            }
            else
            {
                searchBehaviour = new Search(priority, (float)radius, search);
            } 
            DA.SetData(0, searchBehaviour);
        }
    }

}
