using SlowRobotics.SRGraph;
using SlowRobotics.SRMath;
using System.Collections.Generic;
using System.Linq;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Rigid body implementation
    /// </summary>
    public class SRBody : SRParticle
    {
        public List<IParticle> pts;
        public Vec3D LinearVelocity;
        public Vec3D AngularVelocity;
        public Vec3D Torque;
        public bool rigid;

        /// <summary>
        /// Default constructor. Creates an SRBody from a centre of mass. 
        /// Use insertPoint() to add particles to the body.
        /// </summary>
        /// <param name="_centreOfMass"></param>
        public SRBody(Plane3D _centreOfMass) : this(_centreOfMass,true)
        {
        }

        /// <summary>
        /// Constructs a body from a collection of points and assigns centre of mass
        /// to the average position of all points. Setting the rigid flag to false allows
        /// particles to move relative to the body.
        /// </summary>
        /// <param name="_pts"></param>
        /// <param name="_rigid"></param>
        public SRBody(IEnumerable<IParticle> _pts, bool _rigid) : this(calculateCoM(_pts), _rigid)
        {
            insertPoints(_pts);
        }

        /// <summary>
        /// Creates an SRBody from a centre of mass. Setting the rigid flag to false allows
        /// particles to move relative to the body.
        /// </summary>
        /// <param name="_centreOfMass"></param>
        /// <param name="_rigid"></param>
        public SRBody(Plane3D _centreOfMass, bool _rigid) : base(_centreOfMass)
        {
            pts = new List<IParticle>();
            AngularVelocity = new Vec3D();
            LinearVelocity = new Vec3D();
            Torque = new Vec3D();
            rigid = _rigid;
        }
        /// <summary>
        /// Recalculate centre of mass from particles in the body. 
        /// This function currently treates mass of all particles as equal.
        /// </summary>
        /// <returns>Centre of mass for all particles</returns>
        public static Plane3D calculateCoM(IEnumerable<IParticle> _pts)
        {
            Vec3D origin = MathUtils.averageVectors(_pts.Select(pt => (Vec3D)pt).ToList());
            Vec3D xx = MathUtils.averageVectors(_pts.Select(pt => pt.get().xx).ToList());
            Vec3D yy = MathUtils.averageVectors(_pts.Select(pt => pt.get().yy).ToList());
            return new Plane3D(origin, xx, yy);
        }

        /// <summary>
        /// Sets the centre of mass of the body to a given plane.
        /// </summary>
        /// <param name="p"></param>
        public void setCoM(Plane3D p)
        {
            set(p);
        }

        /// <summary>
        /// Inserts a particle into the particle collection of the body
        /// </summary>
        /// <param name="_pt">Point to insert</param>
        public void insertPoint(IParticle _pt)
        {
            pts.Add(_pt);
        }

        /// <summary>
        /// Inserts a collection of particles into the particle collection of the body
        /// </summary>
        /// <param name="_pts">Points to insert</param>
        public void insertPoints(IEnumerable<IParticle> _pts)
        {
            pts.AddRange(_pts);
        }

        /// <summary>
        /// Calls the ApplyImpulses() function for each particle in the body
        /// </summary>
        private void sumForces()
        {
            foreach (IParticle p in pts)
            {
                ApplyImpulses(p.getImpulse());
            }
        }

        /// <summary>
        /// Integrates particle forces into body Accel and Torque
        /// </summary>
        /// <param name="impulses"></param>
        public void ApplyImpulses(IEnumerable<Impulse> impulses)
        {
            foreach (Impulse i in impulses)
            {

                if (!i.torqueOnly)accel.addSelf(i.dir); //integrate force

                Vec3D ab = i.pos.sub(this);
                float d = ab.magnitude();

                Vec3D crossAb = ab.cross(i.dir);
                float a = ab.angleBetween(ab.add(i.dir), true);

                if (!float.IsNaN(a)) Torque.addSelf(crossAb.scale(a)); //integrate torque
            }
        }

        /// <summary>
        /// Transforms body centre of mass and all particles in the body.
        /// </summary>
        /// <param name="t"></param>
        public override void transform(Matrix4x4 t)
        {
            //transform plane
            xx = t.applyTo(xx).normalize();
            zz = t.applyTo(zz).normalize();
            yy = t.applyTo(yy).normalize();

            //transform body
            foreach (SRParticle p in pts)
            {
                p.transform(t); //reorient particleplane
                Vec3D localP = p.sub(this); //get relative position of particle
                t.applyToSelf(localP); //transform relative position
                p.set(localP.addSelf(this)); //update particle position
                
            }
        }

        //TODO - override interpolate and set functions for rigid body so that these 
        //transforms are applied to all particles in the body

        /// <summary>
        /// Calls sumForces() and integrates acceleration and torque into body position and orientation by transforming the body.
        /// </summary>
        /// <param name="dt"></param>
        public override void step(float dt)
        {
            //get accel and torque from all forces acting on particles in the body
            sumForces();

            //sum all forces on the body and apply this as a direct impulse to the location of the body
            //TODO implement body mass
            Vec3D impulse = accel.scale(dt);
            addSelf(impulse);

            //loop through all particles and apply forces for rigid or soft bodies
            foreach (SRParticle p in pts)
            {
                if (rigid)
                {
                    p.addSelf(impulse);
                    p.reset(); //reset forces on particle
                }
                else
                {
                    p.step(dt);
                }
            }


            //add angular velocity as impulse and transform
            Matrix4x4 t = new Matrix4x4();
            Quaternion rot = Quaternion.createFromAxisAngle(Torque, Torque.magnitude() * 1f * dt);
            rot.normalize();
            t = rot.toMatrix4x4();
            transform(t); 

            Torque = new Vec3D();
            accel = new Vec3D();

            age++;
        }

        /// <summary>
        /// Static method to create a body from a graph
        /// </summary>
        /// <param name="graph">Graph to create body from</param>
        /// <returns></returns>
        public static SRBody CreateFromGraph(Graph<SRParticle, Spring> graph, bool rigid)
        {
            SRBody body = new SRBody(new Plane3D(), rigid); //create temporary centre of mass
            int num = graph.Geometry.Count;
            if (num == 0) return body;

            foreach (SRParticle p in graph.Geometry)
            {
                if(rigid)p.f = true; //don't move particles
                body.insertPoint(p); //add to body
            }
            body.set(MathUtils.averageVectors(graph.Geometry.ConvertAll(x => (Vec3D)x)));

            return body;
        }

        public override IParticle duplicate()
        {
            SRBody copy = new SRBody(this);
            copy.copyAttributes(this);
            copy.insertPoints(pts);
            return copy;
        }
    }
}
