using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Interface for handling all agents that have states
    /// e.g. positions or properties that change over time
    /// </summary>
    public interface IStateAgent :IAgent, IState
    {

    }
}
