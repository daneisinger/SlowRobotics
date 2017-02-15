using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Rhino.GraphTools
{

    //Modified from Josh Baldwin 
    //https://github.com/jbaldwin/astar.cs


    /// <summary>
    /// AStar algorithm states while searching for the goal.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The AStar algorithm is still searching for the goal.
        /// </summary>
        Searching,

        /// <summary>
        /// The AStar algorithm has found the goal.
        /// </summary>
        GoalFound,

        /// <summary>
        /// The AStar algorithm has failed to find a solution.
        /// </summary>
        Failed
    }

    /// <summary>
    /// System.Collections.Generic.SortedList by default does not allow duplicate items.
    /// Since items are keyed by TotalCost there can be duplicate entries per key.
    /// </summary>
    internal class DuplicateComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return (x <= y) ? -1 : 1;
        }
    }

    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public class AStar<T>
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private SortedList<int, INode<T>> openList;

        /// <summary>
        /// The closed list.
        /// </summary>
        private SortedList<int, INode<T>> closedList;

        /// <summary>
        /// The current node.
        /// </summary>
        private INode<T> current;

        /// <summary>
        /// The goal node.
        /// </summary>
        private INode<T> goal;

        /// <summary>
        /// Gets the current amount of steps that the algorithm has performed.
        /// </summary>
        public int Steps { get; private set; }

        /// <summary>
        /// Gets the current state of the open list.
        /// </summary>
        public IEnumerable<INode<T>> OpenList { get { return openList.Values; } }

        /// <summary>
        /// Gets the current state of the closed list.
        /// </summary>
        public IEnumerable<INode<T>> ClosedList { get { return closedList.Values; } }

        /// <summary>
        /// Gets the current node that the AStar algorithm is at.
        /// </summary>
        public INode<T> CurrentNode { get { return current; } }

        /// <summary>
        /// Creates a new AStar algorithm instance with the provided start and goal nodes.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public AStar(INode<T> start, INode<T> goal)
        {
            var duplicateComparer = new DuplicateComparer();
            openList = new SortedList<int, INode<T>>(duplicateComparer);
            closedList = new SortedList<int, INode<T>>(duplicateComparer);
            Reset(start, goal);
        }

        /// <summary>
        /// Resets the AStar algorithm with the newly specified start node and goal node.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public void Reset(INode<T> start, INode<T> goal)
        {
            openList.Clear();
            closedList.Clear();
            current = start;
            this.goal = goal;
            openList.Add(current.Cost, current);
            current.SetOpenList(true); //open
        }

        /// <summary>
        /// Steps the AStar algorithm forward until it either fails or finds the goal node.
        /// </summary>
        /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
        public State Run()
        {
            // Continue searching until either failure or the goal node has been found.
            while (true)
            {
                State s = Step();
                if (s != State.Searching)
                    return s;
            }
        }

        /// <summary>
        /// Moves the AStar algorithm forward one step.
        /// </summary>
        /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
        public State Step()
        {
            Steps++;
            while (true)
            {
                // There are no more nodes to search, return failure.
                if (openList.Count == 0)
                {
                    return State.Failed;
                }

                // Check the next best node in the graph by TotalCost.
                INode<T> top = openList.Values[0];
                openList.RemoveAt(0);
                current = top;

                if (current.IsClosedList(ClosedList))
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // Remove from the open list and place on the closed list 
            // since this node is now being searched.
            current.SetOpenList(false);
            closedList.Add(current.Cost, current);
            current.SetClosedList(true);

            // Found the goal, stop searching.
            if (current == goal)
            {
                return State.GoalFound;
            }

            // Node was not the goal so add all children nodes to the open list.
            // Each child needs to have its movement cost set and estimated cost.
            foreach (IEdge<T> child in current.Edges)
            {
                INode<T> other = child.Other(current);
                // If the child has already been searched (closed list) or is on
                // the open list to be searched then do not modify its movement cost
                // or estimated cost since they have already been set previously.
                if (other.IsOpenList(OpenList) || other.IsClosedList(ClosedList))
                {
                    continue;
                }

                //NOTE = not using distances
                other.parent = current;
                other.Cost = (current.Cost + 1);
                // other.SetEstimatedCost(goal); 
                openList.Add(other.Cost, other);
                other.SetOpenList(true);
            }

            // This step did not find the goal so return status of still searching.
            return State.Searching;
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns null if the algorithm has never been run.</returns>
        public IEnumerable<INode<T>> GetPath()
        {
            if (current != null)
            {
                var next = current;
                var path = new List<INode<T>>();
                while (next != null)
                {
                    path.Add(next);
                    next = next.parent;
                }
                path.Reverse();
                return path.ToArray();
            }
            return null;
        }
    }
}
