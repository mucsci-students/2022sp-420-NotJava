using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia;

namespace UMLEditor.Views.CustomControls
{
    /// <summary>
    /// Class to represent a node in the grid
    /// </summary>
    public class Node
    {
        // Change this depending on what the desired size is for each element in the grid
        /// <summary>
        /// Size of each node of the grid, in pixels
        /// </summary>
        public static int NODE_SIZE = 20;

        /// <summary>
        /// Parent node to this node
        /// </summary>
        //public Node? Parent;
        public Node? Previous;

        /// <summary>
        /// Grid position of the node
        /// </summary>
        public Vector2 Position;
        
        /// <summary>
        /// Center position of the node
        /// </summary>
        public Point Center =>
            // ReSharper disable twice PossibleLossOfFraction
            // Need to convert floating point to integer
            new (Position.X * NODE_SIZE + NODE_SIZE / 2, Position.Y * NODE_SIZE + NODE_SIZE / 2);

        /// <summary>
        /// Distance to current target
        /// </summary>
        public float DistanceToTarget;
        
        /// <summary>
        /// Cost of reaching this node from current start
        /// </summary>
        public float Cost;
        
        /// <summary>
        /// Weight of traversing this node
        /// </summary>
        public readonly float Weight;
        
        /// <summary>
        /// Sum of distance and cost
        /// </summary>
        public float F
        {
            get
            {
                if (DistanceToTarget >= -1 && Cost >= -1)
                    return DistanceToTarget + Cost;
                return -1;
            }
        }
        
        /// <summary>
        /// Flag for whether the node can be traversed or not
        /// </summary>
        public bool Walkable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position of node</param>
        /// <param name="walkable">Can be traversed?</param>
        /// <param name="weight">Weight of traversing</param>
        public Node(Vector2 pos, bool walkable, float weight = 1)
        {
            //Parent = null;
            Previous = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }

    /// <summary>
    /// Class to represent the A* path
    /// </summary>
    public class Astar
    {
        readonly List<List<Node>> _grid;
        int GridRows => _grid[0].Count;

        int GridCols => _grid.Count;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid">Grid to create the path on</param>
        public Astar(List<List<Node>> grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Finds the best path from start to end without traversing any non-walkable nodes
        /// </summary>
        /// <param name="start">Coordinates to start at</param>
        /// <param name="end">Coordinates to end at</param>
        /// <returns></returns>
        public Stack<Node>? FindPath(Vector2 start, Vector2 end)
        {
            Node startNode = new Node(new Vector2((int)(start.X/Node.NODE_SIZE), (int) (start.Y/Node.NODE_SIZE)), true);
            Node endNode = new Node(new Vector2((int)(end.X / Node.NODE_SIZE), (int)(end.Y / Node.NODE_SIZE)), true);
            Stack<Node> path = new Stack<Node>();
            PriorityQueue<Node, float> openList = new();
            List<Node> closedList = new List<Node>();
            Node current = startNode;
           
            // add start node to Open List
            openList.Enqueue(startNode, startNode.F);
            int count = 0;
            while(openList.Count != 0 && !closedList.Exists(x => x.Position == endNode.Position) && count < 5000)
            {
                ++count;
                current = openList.Dequeue();
                closedList.Add(current);
                var adjacencies = GetAdjacentNodes(current);

                foreach(Node n in adjacencies)
                {
                    if (!closedList.Contains(n) && n.Walkable)
                    {
                        bool isFound = false;
                        foreach (var oLNode in openList.UnorderedItems)
                        {
                            if (oLNode.Element == n)
                            {
                                isFound = true;
                            }
                        }
                        if (!isFound)
                        {
                            n.Previous = current;
                            n.DistanceToTarget = Math.Abs(n.Position.X - endNode.Position.X) + Math.Abs(n.Position.Y - endNode.Position.Y);
                            n.Cost = n.Weight + n.Previous.Cost;
                            openList.Enqueue(n, n.F);
                        }
                    }
                }
            }
            
            // construct path, if end was not closed return null
            if(!closedList.Exists(x => x.Position == endNode.Position))
            {
                return null;
            }

            // if all good, return path
            Node? temp = closedList[closedList.IndexOf(current)];
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (temp is null) return null;
            do
            {
                path.Push(temp);
                temp = temp.Previous;
            } while (temp != startNode && temp is not null) ;
            return path;
        }
		
        /// <summary>
        /// Gets the 4 adjacent nodes to a node
        /// </summary>
        /// <param name="n">Node to check</param>
        /// <returns>List of the adjacent nodes</returns>
        private List<Node> GetAdjacentNodes(Node n)
        {
            List<Node> temp = new List<Node>();
            int row = (int)n.Position.Y;
            int col = (int)n.Position.X;
            if(row + 1 < GridRows && col < GridCols)
            {
                temp.Add(_grid[col][row + 1]);
            }
            if(row - 1 >= 0 && col >= 0)
            {
                temp.Add(_grid[col][row - 1]);
            }
            if(col - 1 >= 0 && row >= 0)
            {
                temp.Add(_grid[col - 1][row]);
            }
            if(col + 1 < GridCols && row < GridRows)
            {
                temp.Add(_grid[col + 1][row]);
            }

            return temp;
        }
    }
}

/*
MIT License

Copyright (c) 2021 davecusatis

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/