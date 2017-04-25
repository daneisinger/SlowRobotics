using SlowRobotics.SRMath;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{

    /// <summary>
    /// Abstract behaviour implementation
    /// </summary>
    public abstract class Behaviour : IBehaviour
    {
        public int priority { get; set; }
        public bool lateUpdate { get; set; }
        public Behaviour(int _priority)
        {
            priority = _priority;
            lateUpdate = false;
        }

        public int CompareTo(IBehaviour other)
        {
            if (other.priority > priority) return -1;
            if (other.priority < priority) return 1;
            return 0;
        }

        public abstract void onAdd();
        public abstract void run(IAgentT<object> a);
        public abstract void interact(IAgentT<object> a, object b);

    }

    /// <summary>
    /// Generic behaviour class that runs on a single object type
    /// by casting from the abstract object base type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Behaviour<T> : IBehaviourT<T> where T : class
    {
        public int priority { get; set; }
        public bool lateUpdate { get; set; }

        public Behaviour(int _priority)
        {
            priority = _priority;
            lateUpdate = false;
        }

        public int CompareTo(IBehaviour other)
        {
            if (other.priority > priority) return -1;
            if (other.priority < priority) return 1;
            return 0;
        }

        public virtual void run(IAgentT<object> a)
        {

            T data = a.getData() as T;
            if (data != null)
            {
                runOn(data);
            }
            else
            {
                throw new InvalidCastException("Behaviour of type " + typeof(T) + " cannot run on agent type:" + a.getData().GetType());
            }

        }

        public virtual void interact(IAgentT<object> a, object b)
        {
            T data = a.getData() as T;
            if (data != null)
            {
                interactWith(data, b);
            }
            else
            {
                throw new InvalidCastException("Behaviour of type " + typeof(T) + " cannot run on agent type:" + a.getData().GetType());
            }
        }

        public virtual void onAdd() { }

        public virtual void runOn(T a){}

        public virtual void interactWith(T a, object b){}
    }

    /// <summary>
    /// Extension of behaviour to allow for scaling of behaviour parameters each step
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScaledBehaviour<T> : Behaviour<T>, IScaledBehaviour where T :class
    {
        public float scaleFactor { get; set; }
        public InterpolateStrategy interpolator;

        public ScaledBehaviour() : this(1) { }

        public ScaledBehaviour(int _priority) : base(_priority)
        {
            scaleFactor = 1;
            interpolator = new LinearInterpolation();
        }

        public override void run(IAgentT<object> a)
        {

            T data = a.getData() as T; 
            if (data !=null)
            {
                runOn(data);
                reset();
            }
            else
            {
                throw new InvalidCastException("Behaviour of type " + typeof(T) + " cannot run on agent type:" + a.getData().GetType());
            }
        }

        public virtual void reset()
        {
            scaleFactor = 1;
        }

        public void scale(float factor)
        {
            scaleFactor = factor;
        }
        public void setInterpolateStrategy(InterpolateStrategy _interpolator)
        {
            interpolator = _interpolator;
        }

    }
}
