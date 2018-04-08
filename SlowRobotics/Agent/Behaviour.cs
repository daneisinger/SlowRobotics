using SlowRobotics.SRMath;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Order to run behaviour - low numbers run first
        /// </summary>
        public int priority { get; set; }
        protected Stopwatch stopWatch;

        /// <summary>
        /// Flag to set whether to run the behaviour on the multithreaded core or the single threaded late update.
        /// Typically behaviours that are needing to add to collections will need to be run using the late update loop.
        /// A typical particle model will use late update to run particle integration methods while using the multithreaded loop
        /// to calculate particle forces in the model.
        /// </summary>
        public bool lateUpdate { get; set; }

        /// <summary>
        /// Creates a behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Priority of the behaviour</param>
        public Behaviour(int _priority)
        {
            priority = _priority;
            lateUpdate = false;
            stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Compare to function for sorting the behaviour in the agent behaviour queue
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IBehaviour other)
        {
            if (other.priority > priority) return -1;
            if (other.priority < priority) return 1;
            return 0;
        }

        /// <summary>
        /// Utility method that is called when the behaviour is added to the agent queue
        /// </summary>
        public abstract void onAdd();

        /// <summary>
        /// Abstract run implementation
        /// </summary>
        /// <param name="a"></param>
        public abstract void run(IAgent<object> a);

        /// <summary>
        /// Abstract interact implementation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public abstract void interact(IAgent<object> a, object b);

        public abstract string debug(IAgent<object> a);

    }

    /// <summary>
    /// Generic behaviour class that runs on a single object type
    /// by casting from the abstract object base type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Behaviour<T> : IBehaviour<T> where T : class
    {
        public int priority { get; set; }
        public bool lateUpdate { get; set; }
        private Stopwatch stopWatch;

        public Behaviour(int _priority)
        {
            priority = _priority;
            lateUpdate = false;
            stopWatch = new Stopwatch();
        }

        public int CompareTo(IBehaviour other)
        {
            if (other.priority > priority) return -1;
            if (other.priority < priority) return 1;
            return 0;
        }

        /// <summary>
        /// Calls the behaviour runOn(T data) function by attempting to cast
        /// object returned by a.getData() to type T.
        /// </summary>
        /// <param name="a">Agent to get data for</param>
        public virtual void run(IAgent<object> a)
        {
            T data = a.getData() as T;
            if (data != null)
            {
                runOn(data);
            }
        }

        /// <summary>
        /// Calls the behaviour interactWith(T data, object b) function by attempting to cast
        /// object returned by a.getData() to type T.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public virtual void interact(IAgent<object> a, object b)
        {
            T data = a.getData() as T;
            if (data != null)
            {
                interactWith(data, b);
            }
        }

        /// <summary>
        /// Utility method that is called when the behaviour is added to the agent queue
        /// </summary>
        public virtual void onAdd() { }

        /// <summary>
        /// Behaviour function that runs on wrapped agent data type
        /// </summary>
        /// <param name="a"></param>
        public virtual void runOn(T a){}

        /// <summary>
        /// Behaviour function that runs on wrapped agent data type and a given object from the agent neighbour list
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public virtual void interactWith(T a, object b){}

        /// <summary>
        /// Gets runtime of behaviour for a given agent in milliseconds
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public virtual string debug(IAgent<object> a)
        {
            stopWatch.Reset();
            stopWatch.Start();
            run(a);
            stopWatch.Stop();
            return "Name: " + ToString() + "Run Time: "+stopWatch.ElapsedMilliseconds.ToString();
        }

    }

    /// <summary>
    /// Extension of behaviour to allow for scaling of behaviour parameters each step
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScaledBehaviour<T> : Behaviour<T>, IScaledBehaviour where T : class
    {
        /// <summary>
        /// Scale factor for behaviour
        /// </summary>
        public float scaleFactor { get; set; }
        
        /// <summary>
        /// Intepolation strategy for the behaviour - used to add falloff effects to behaviour scaling
        /// </summary>
        public InterpolateStrategy interpolator;


        /// <summary>
        /// Default constructor with priority 0 and linear interpolator
        /// </summary>
        public ScaledBehaviour() : this(0) { }

        /// <summary>
        /// Default constructor with linear interpolator
        /// </summary>
        /// <param name="_priority"></param>
        public ScaledBehaviour(int _priority) : base(_priority)
        {
            scaleFactor = 1;
            interpolator = new LinearInterpolation();
        }

        /// <summary>
        /// Calls the behaviour runOn(T data) function by attempting to cast
        /// object returned by a.getData() to type T. After this function is called
        /// the reset() function is called.
        /// </summary>
        /// <param name="a">Agent to get data for</param>
        public override void run(IAgent<object> a)
        {
            T data = a.getData() as T;
            if (data != null)
            {
                runOn(data);
            }
        }

        /// <summary>
        /// Default reset implementation - sets scaleFactor to 1.
        /// </summary>
        public virtual void reset()
        {
            scaleFactor = 1;
        }

        /// <summary>
        /// Sets scaleFactor to a given value
        /// </summary>
        /// <param name="factor">New scale factor</param>
        public void scale(float factor)
        {
            scaleFactor = factor;
        }

        /// <summary>
        /// Sets the behaviour interpolation strategy
        /// </summary>
        /// <param name="_interpolator"></param>
        public void setInterpolateStrategy(InterpolateStrategy _interpolator)
        {
            interpolator = _interpolator;
        }

        public override string debug(IAgent<object> a)
        {
            return base.debug(a) + ", Scale factor: " + scaleFactor;
        }

    }
}
