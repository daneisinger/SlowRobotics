using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Interface for handling agents with neighbours 
    /// e.g. proximity links
    /// </summary>
    public interface IGraphAgent : IAgent
    {

        List<Vec3D> neighbours { get; set; }
        void addNeighbours(List<Vec3D> neighbours);
        bool hasNeighbours();
        Node getNode();
    }

}
