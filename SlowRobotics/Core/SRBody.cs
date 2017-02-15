using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SRBody : SRParticle
    {
        public List<SRParticle> pts;
        public Vec3D LinearVelocity;
        public Vec3D AngularVelocity;
        public Vec3D Torque;
        public string reporting;

        private float Mass = 1;

        public SRBody(Plane3D _centreOfMass) : base(_centreOfMass)
    {
            pts = new List<SRParticle>();

            AngularVelocity = new Vec3D();
            LinearVelocity = new Vec3D();
            Torque = new Vec3D();
            reporting = "";

        }

        public void insertPoint(SRParticle _pt)
        {
            pts.Add(_pt);
        }

        public void insertPoints(IEnumerable<SRParticle> _pts)
        {
            pts.AddRange(_pts);
        }

        private void sumForces()
        {
            foreach (SRParticle p in pts)
            {
                ApplyForce(p, p.getLimitedAccel());
            }
        }

        //adds to accel and torque
        public void ApplyForce(Vec3D pos, Vec3D force)
        {
            accel.addSelf(force);

            Vec3D ab = pos.sub(this);
            float d = ab.magnitude();

            Vec3D crossAb = ab.cross(force);
            crossAb.scaleSelf(1f / d);
            Torque.addSelf(crossAb);

        }

        public void transformBody(Matrix4x4 t)
        {
            transform(t); //transform this plane
            addSelf(vel);
            foreach (SRParticle p in pts)
            {
                p.addSelf(vel);
                Vec3D localP = p.sub(this);
                t.applyToSelf(localP);
                p.set(localP.addSelf(this));
            }

        }

        public override void step(float dt)
        {
            sumForces(); //sets torque and accel based on forces on particles
            vel.addSelf(accel.scale(dt));
            vel.limit(spd);
            vel.scaleSelf(dt);

            AngularVelocity.addSelf(Torque.scale(dt));
            AngularVelocity.scaleSelf(0.9f);

            Matrix4x4 t = new Matrix4x4();

            if (AngularVelocity.magnitude() > 0.1f)
            {
                Toxiclibs.core.Quaternion rot = Toxiclibs.core.Quaternion.createFromAxisAngle(AngularVelocity, AngularVelocity.magnitude() * 0.01f * dt);
                rot.normalize();
                t = rot.toMatrix4x4();
            }

            transformBody(t);
            Torque = new Vec3D();
            accel = new Vec3D();

            age++;
        }

        public static SRBody CreateFromGraph(Graph<SRParticle, Spring> graph)
        {
            SRBody body = new SRBody(new Plane3D()); //create temporary centre of mass
            int num = graph.Geometry.Count;
            if (num == 0) return body;

            foreach (SRParticle p in graph.Geometry)
            {
                p.f = true; //don't move particles
                body.insertPoint(p); //add to body
            }
            body.set(SR_Math.averageVectors(graph.Geometry.ConvertAll(x => (Vec3D)x)));

            return body;
        }
    }
}
