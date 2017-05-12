using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Behaviour interface - handles queue inertion and run methods for base object data.
    /// See Behaviour and ScaledBehaviour implementations for extended method documentation.
    /// </summary>
    public interface IBehaviour : IComparable<IBehaviour>
    {
        int priority { get; set; }
        void onAdd();
        bool lateUpdate { get; set; }
        void run(IAgent<object> a);
        void interact(IAgent<object> a, object b);
    }

    /// <summary>
    /// Generic behaviour interface - provides methods for running on specific object type.
    /// See Behaviour and ScaledBehaviour implementations for extended method documentation.
    /// </summary>
    /// <typeparam name="T">Object type for behaviour to run on</typeparam>
    public interface IBehaviour<in T> : IBehaviour where T : class
    {
        void runOn(T a);
        void interactWith(T a, object b);
    }

    /// <summary>
    /// Scaled behaviour interface - provides methods for scaling behaviour parameters
    /// </summary>
    public interface IScaledBehaviour : IBehaviour
    {
        float scaleFactor { get; set; }
        void scale(float factor);
    }
}
