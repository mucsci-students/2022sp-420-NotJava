using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AStarSharp
{
    /// <summary>
    /// Class to represent a node in the grid
    /// </summary>
    public class Node
    {
        // Change this depending on what the desired size is for each element in the grid
        public static int NODE_SIZE = 15;
        
        /// <summary>
        /// Parent node to this node
        /// </summary>
        public Node Parent;
        /// <summary>
        /// Grid position of the node
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Center position of the node
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + NODE_SIZE / 2, Position.Y + NODE_SIZE / 2);
            }
        }
        
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
        public float Weight;
        /// <summary>
        /// Sum of distance and cost
        /// </summary>
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
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
            Parent = null;
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
        List<List<Node>> Grid;
        int GridRows
        {
            get
            {
               return Grid[0].Count;
            }
        }
        int GridCols
        {
            get
            {
                return Grid.Count;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid">Grid to create the path on</param>
        public Astar(List<List<Node>> grid)
        {
            Grid = grid;
        }

        /// <summary>
        /// Finds the best path from start to end without traversing any non-walkable nodes
        /// </summary>
        /// <param name="Start">Coordinates to start at</param>
        /// <param name="End">Coordinates to end at</param>
        /// <returns></returns>
        public Stack<Node>? FindPath(Vector2 Start, Vector2 End)
        {
            Node start = new Node(new Vector2((int)(Start.X/Node.NODE_SIZE), (int) (Start.Y/Node.NODE_SIZE)), true);
            Node end = new Node(new Vector2((int)(End.X / Node.NODE_SIZE), (int)(End.Y / Node.NODE_SIZE)), true);

            Stack<Node> Path = new Stack<Node>();
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            List<Node> adjacencies;
            Node current = start;
           
            // add start node to Open List
            OpenList.Add(start);

            while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

 
                foreach(Node n in adjacencies)
                {
                    if (!ClosedList.Contains(n) && n.Walkable)
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
                            n.Cost = n.Weight + n.Parent.Cost;
                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                        }
                    }
                }
            }
            
            // construct path, if end was not closed return null
            if(!ClosedList.Exists(x => x.Position == end.Position))
            {
                return null;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp is null) return null;
            do
            {
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp is not null) ;
            return Path;
        }
		
        private List<Node> GetAdjacentNodes(Node n)
        {
            List<Node> temp = new List<Node>();

            int row = (int)n.Position.Y;
            int col = (int)n.Position.X;

            if(row + 1 < GridRows)
            {
                temp.Add(Grid[col][row + 1]);
            }
            if(row - 1 >= 0)
            {
                temp.Add(Grid[col][row - 1]);
            }
            if(col - 1 >= 0)
            {
                temp.Add(Grid[col - 1][row]);
            }
            if(col + 1 < GridCols)
            {
                temp.Add(Grid[col + 1][row]);
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