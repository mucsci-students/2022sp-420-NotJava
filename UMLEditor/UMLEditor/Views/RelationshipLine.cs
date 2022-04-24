using System;
using System.Collections.Generic;
using System.Numerics;
using AStarSharp;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using UMLEditor.Utility;

namespace UMLEditor.Views;

/// <summary>
/// RelationshipLine class for use in the main window.
/// </summary>
public class RelationshipLine
{
    /// <summary>
    /// Class to represent a pair of points composing a relationship line
    /// </summary>
    public class PointPair
    {
        /// <summary>
        /// Start point of the pair
        /// </summary>
        public Point Start { get; }
        /// <summary>
        /// End point of the pair
        /// </summary>
        public Point End { get; }
        /// <summary>
        /// Straight line distance between the points
        /// </summary>
        public float Distance { get; }
        /// <summary>
        /// Edge of the starting box
        /// </summary>
        public EdgeEnum StartEdge { get; }
        /// <summary>
        /// Edge of the ending box
        /// </summary>
        public EdgeEnum EndEdge { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p1">Starting point</param>
        /// <param name="p2">Ending point</param>
        /// <param name="e1">Starting edge</param>
        /// <param name="e2">Ending edge</param>
        /// <param name="d">Distance between the points</param>
        public PointPair(Point p1, Point p2, EdgeEnum e1, EdgeEnum e2, float d)
        {
            Start = p1;
            End = p2;
            StartEdge = e1;
            EndEdge = e2;
            Distance = d;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PointPair()
        { }
    }
    
    private const double SymbolHalfWidth = 16;
    private const double SymbolHalfHeight = 12;
    private const int LineThickness = 2;

    private static bool _magicToggle = true;
    private static List<List<Node>> _grid = new();
    private static Astar _astar = new(_grid);

    private List<Point> _gridPoints = new();
    
    /// <summary>
    /// Queue listing each point pair in order from closest to furthest
    /// </summary>
    public PriorityQueue<PointPair, float> PointPairQueue = new();
    private PointPair _closestPair;
    
    /// <summary>
    /// SourceClass control for relationship line
    /// </summary>
    public ClassBox SourceClass { get; }
    
    /// <summary>
    /// Destination class for relationship line
    /// </summary>
    public ClassBox DestClass { get; }
    // Resharper disable once NotAccessedField.local

    private string RelationshipType { get; }
    private Polyline Symbol { get; set; }
    private List<Line> Segments { get;}

    /// <summary>
    /// Default Ctor
    /// </summary>
    /// <param name="startCtrl">Source class control</param>
    /// <param name="endCtrl">Dest class control</param>
    /// <param name="relationshipType">Relationship type for line</param>
    public RelationshipLine(ClassBox startCtrl, ClassBox endCtrl, string relationshipType)
    {
        SourceClass = startCtrl;
        DestClass = endCtrl;
        RelationshipType = relationshipType;
        Segments = new List<Line>();
        Symbol = new Polyline();
        _closestPair = new PointPair();
        
        CreatePointPairList();
    }

    /// <summary>
    /// Toggles flag to use magic lines algorithm
    /// </summary>
    /// <param name="b">Uses magic lines if true, default if false</param>
    public static void ToggleMagicLines(bool b)
    {
        _magicToggle = b;
        ResetGrid();
    }

    /// <summary>
    /// Initializes the pathing grid to the correct size and creates nodes for each coordinate
    /// </summary>
    /// <param name="canvas">The canvas to get size of for grid (currently not working)</param>
    public static void InitializeGrid(Canvas canvas)
    {
        // Currently not getting bounds correctly, using placeholder values
        int maxX = 6000 / Node.NODE_SIZE, maxY = 6000 / Node.NODE_SIZE;
        if (canvas.Bounds.Width != 0)
        {
            maxX = (int)canvas.Bounds.Width;
        }

        if (canvas.Bounds.Height != 0)
        {
            maxY = (int)canvas.Bounds.Height;
        }
        
        for (int x = 0; x < maxX; ++x)
        {
            List<Node> row = new List<Node>();
            _grid.Add(row);
            for (int y = 0; y < maxY; ++y)
            {
                Node myNode = new Node(new Vector2(x,y), true);
                _grid[x].Add(myNode);
            }
        }
    }

    /// <summary>
    /// Resets the pathing grid to be fully walkable
    /// </summary>
    public static void ResetGrid()
    {
        for (int x = 0; x < _grid.Count; ++x)
        {
            for (int y = 0; y < _grid[x].Count; ++y)
            {
                _grid[x][y].Walkable = true;
            }
        }
    }

    private void CreatePointPairList()
    {
        PointPairQueue.Clear();
        
        // Get midpoints for each edge
        double startHalfWidth = SourceClass.Bounds.Width / 2;
        double startHalfHeight = SourceClass.Bounds.Height / 2;
        double endHalfWidth = DestClass.Bounds.Width / 2;
        double endHalfHeight = DestClass.Bounds.Height / 2;

        // Create lists of edge midpoints for start and destination classes
        // 0 - Top, 1 - Right, 2 - Bottom, 3 - Left
        List<Point> startEdgePoints = new List<Point>();
        startEdgePoints.Add(new Point(SourceClass.Bounds.X + startHalfWidth, SourceClass.Bounds.Y));
        startEdgePoints.Add(new Point(SourceClass.Bounds.X + SourceClass.Bounds.Width,
            SourceClass.Bounds.Y + startHalfHeight));
        startEdgePoints.Add(new Point(SourceClass.Bounds.X + startHalfWidth,
            SourceClass.Bounds.Y + SourceClass.Bounds.Height));
        startEdgePoints.Add(new Point(SourceClass.Bounds.X, SourceClass.Bounds.Y + startHalfHeight));

        List<Point> endEdgePoints = new List<Point>();
        endEdgePoints.Add(new Point(DestClass.Bounds.X + endHalfWidth, DestClass.Bounds.Y));
        endEdgePoints.Add(new Point(DestClass.Bounds.X + DestClass.Bounds.Width,
            DestClass.Bounds.Y + endHalfHeight));
        endEdgePoints.Add(new Point(DestClass.Bounds.X + endHalfWidth,
            DestClass.Bounds.Y + DestClass.Bounds.Height));
        endEdgePoints.Add(new Point(DestClass.Bounds.X, DestClass.Bounds.Y + endHalfHeight));

        // Find distance between each edge pair
        foreach (Point p1 in startEdgePoints)
        {
            foreach (Point p2 in endEdgePoints)
            {
                float thisDistance = GetDistance(p1, p2);
                if (p1.X > 0 && p1.Y > 0 && p2.X > 0 && p2.Y > 0)
                {
                    EdgeEnum e1 = EdgeEnum.Right;
                    EdgeEnum e2 = EdgeEnum.Left;
                    switch (startEdgePoints.IndexOf(p1))
                    {
                        case 0:
                            e1 = EdgeEnum.Top;
                            break;
                        case 1:
                            e1 = EdgeEnum.Right;
                            break;
                        case 2:
                            e1 = EdgeEnum.Bottom;
                            break;
                        case 3:
                            e1 = EdgeEnum.Left;
                            break;
                    }

                    switch (endEdgePoints.IndexOf(p2))
                    {
                        case 0:
                            e2 = EdgeEnum.Top;
                            break;
                        case 1:
                            e2 = EdgeEnum.Right;
                            break;
                        case 2:
                            e2 = EdgeEnum.Bottom;
                            break;
                        case 3:
                            e2 = EdgeEnum.Left;
                            break;
                    }

                    PointPairQueue.Enqueue(new PointPair(p1, p2, e1, e2, thisDistance), thisDistance);
                }
            }
        }

        _closestPair = PointPairQueue.Peek();
    }
    
    /// <summary>
    /// Gets the straight line distance between two points
    /// </summary>
    /// <param name="p1">First point</param>
    /// <param name="p2">Second point</param>
    /// <returns></returns>
    private float GetDistance(Point p1, Point p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y,2));
    }

    /// <summary>
    /// Sets a grid location to not be walkable for the path search algorithm
    /// </summary>
    /// <param name="x">X coordinate of point</param>
    /// <param name="y">Y coordinate of point</param>
    public static void MakeNotWalkable(int x, int y)
    {
        if (x >= 0 && x < _grid[0].Count && y >= 0 && y < _grid.Count)
        {
            _grid[x][y].Walkable = false;
        }
    }

    /// <summary>
    /// Draws a line for a relationship line based on the midpoint between them
    /// </summary>
    /// <param name="myCanvas">Canvas to add the line to</param>
    private void DrawSimpleLine(Canvas myCanvas)
    {
        _gridPoints.Clear();
        
        Point midPointStart, midPointEnd;
        // Draw vertically
        //if (Math.Abs(closestPair.Start.X - closestPair.End.X) > Math.Abs(closestPair.Start.Y - closestPair.End.Y))
        if ((_closestPair.StartEdge == EdgeEnum.Left && _closestPair.EndEdge == EdgeEnum.Right) ||
            (_closestPair.StartEdge == EdgeEnum.Right && _closestPair.EndEdge == EdgeEnum.Left))
        {
            midPointStart = new Point((_closestPair.Start.X + _closestPair.End.X) / 2, _closestPair.Start.Y);
            midPointEnd = new Point((_closestPair.Start.X + _closestPair.End.X) / 2, _closestPair.End.Y);
        }
        // Draw horizontally
        else if ((_closestPair.StartEdge == EdgeEnum.Top && _closestPair.EndEdge == EdgeEnum.Bottom) ||
                 (_closestPair.StartEdge == EdgeEnum.Bottom && _closestPair.EndEdge == EdgeEnum.Top))
        {
            midPointStart = new Point(_closestPair.Start.X, (_closestPair.Start.Y + _closestPair.End.Y) / 2);
            midPointEnd = new Point(_closestPair.End.X, (_closestPair.Start.Y + _closestPair.End.Y) / 2);
        }
        // Draw a right angle
        else if ((_closestPair.StartEdge == EdgeEnum.Right && _closestPair.EndEdge == EdgeEnum.Top) ||
                 (_closestPair.StartEdge == EdgeEnum.Left && _closestPair.EndEdge == EdgeEnum.Top) ||
                 (_closestPair.StartEdge == EdgeEnum.Right && _closestPair.EndEdge == EdgeEnum.Bottom) ||
                 (_closestPair.StartEdge == EdgeEnum.Left && _closestPair.EndEdge == EdgeEnum.Bottom))
        {
            midPointStart = new Point(_closestPair.End.X, _closestPair.Start.Y);
            midPointEnd = midPointStart;
        }
        else
        {
            midPointStart = new Point(_closestPair.Start.X, _closestPair.End.Y);
            midPointEnd = midPointStart;
        }
        
        Point symbolPoint = CalculateSymbolPoints(_closestPair);
        myCanvas.Children.Add(Symbol);
        
        myCanvas.Children.Add(CreateRelationshipLine(_closestPair.Start, midPointStart));
        myCanvas.Children.Add(CreateRelationshipLine(midPointStart, midPointEnd));
        myCanvas.Children.Add(CreateRelationshipLine(midPointEnd, symbolPoint));
    }

    /// <summary>
    /// Draws a line for a relationship line based on the shortest path discovered by A*. If no path is found,
    /// uses DrawSimpleLine.
    /// </summary>
    /// <param name="myCanvas">Canvas to add the line to</param>
    public void Draw(Canvas myCanvas)
    {
        Stack<Node>? shortestPath = null;
        Stack<Node>? path;
        _closestPair = PointPairQueue.Peek();
        PointPair pair = new();
        Point symbolPoint = new();
        
        // Finds the shortest path between each of the edge pairs
        while (_magicToggle && shortestPath is null && PointPairQueue.Count > 0)
        {
            pair = PointPairQueue.Dequeue();
            CalculateSymbolPoints(pair);
            path = GetPath(pair);
            
            if (path is not null && shortestPath is null)
            {
                shortestPath = new Stack<Node>(path);
                _closestPair = pair;
            }
            else if (path is not null && shortestPath is not null) 
            {
                if (path.Count < shortestPath.Count)
                {
                    shortestPath = new Stack<Node>(path);
                    _closestPair = pair;
                }
            }
        }
        
        // Gets the shortest path
        shortestPath = GetPath(_closestPair);
        
        // Draw lines
        // Situation 1: A path was found
        if (_magicToggle && shortestPath is not null)
        {
            symbolPoint = CalculateSymbolPoints(_closestPair);
            myCanvas.Children.Add(Symbol);
            
            Point pos = shortestPath.Pop().Center;
            _gridPoints.Add(pos);
            Line newLine = CreateRelationshipLine(_closestPair.Start, pos);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            while (shortestPath.Count > 0)
            {
                Point newPos = shortestPath.Pop().Center;
                _gridPoints.Add(newPos);
                newLine = CreateRelationshipLine(pos, newPos);
                Segments.Add(newLine);
                myCanvas.Children.Add(newLine);
                pos = newPos;
            }

            _gridPoints.Add(_closestPair.End);
            newLine = CreateRelationshipLine(symbolPoint, pos);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            
            // Makes each point of the line unwalkable
            foreach (Point g in _gridPoints)
            {
                MakeNotWalkable((int) g.X / Node.NODE_SIZE, (int) g.Y / Node.NODE_SIZE);
            }
        }
        // Situation 2: No path found, draw straight line from start to end
        else
        {
            DrawSimpleLine(myCanvas);
        }
    }

    private Stack<Node>? GetPath(PointPair pair)
    {
        Point startPoint = new();
        Point endPoint = new();
        
        _gridPoints.Clear();

        switch (pair.StartEdge)
        {
            case EdgeEnum.Top:
                startPoint = new Point(pair.Start.X, pair.Start.Y - 2 * Node.NODE_SIZE);
                _gridPoints.Add(new Point(pair.Start.X,pair.Start.Y - Node.NODE_SIZE));
                break;
            case EdgeEnum.Right:
                startPoint = new Point(pair.Start.X + 2 * Node.NODE_SIZE, pair.Start.Y);
                _gridPoints.Add(new Point(pair.Start.X + Node.NODE_SIZE,pair.Start.Y));
                break;
            case EdgeEnum.Bottom:
                startPoint = new Point(pair.Start.X, pair.Start.Y + 2 * Node.NODE_SIZE);
                _gridPoints.Add(new Point(pair.Start.X + Node.NODE_SIZE,pair.Start.Y));
                break;
            case EdgeEnum.Left:
                startPoint = new Point(pair.Start.X - 2 * Node.NODE_SIZE, pair.Start.Y);
                _gridPoints.Add(new Point(pair.Start.X,pair.Start.Y - Node.NODE_SIZE));
                break;
        }
        switch (pair.EndEdge)
        {
            case EdgeEnum.Top:
                endPoint = new Point(pair.End.X, pair.End.Y - 2 * Node.NODE_SIZE);
                _gridPoints.Add(new Point(pair.End.X,pair.End.Y - Node.NODE_SIZE));
                break;
            case EdgeEnum.Right:
                endPoint = new Point(pair.End.X + 2 * Node.NODE_SIZE, pair.End.Y);
                _gridPoints.Add(new Point(pair.End.X + Node.NODE_SIZE,pair.End.Y));
                break;
            case EdgeEnum.Bottom:
                endPoint = new Point(pair.End.X, pair.End.Y + 2 * Node.NODE_SIZE);
                _gridPoints.Add(new Point(pair.End.X,pair.End.Y + Node.NODE_SIZE));
                break;
            case EdgeEnum.Left:
                endPoint = new Point(pair.End.X - 2 * Node.NODE_SIZE, pair.End.Y);
                _gridPoints.Add(new Point(pair.End.X - Node.NODE_SIZE,pair.End.Y));
                break;
        }

        // Make symbol area non-walkable
        _gridPoints.Add(endPoint);
        _gridPoints.Add(startPoint);
        for (int y = (int) (Symbol.Bounds.Y / Node.NODE_SIZE) - 1;
             y <= (int) ((Symbol.Bounds.Y + Symbol.Bounds.Height) / Node.NODE_SIZE) + 1;
             ++y)
        {
            for (int x = (int) (Symbol.Bounds.X / Node.NODE_SIZE) - 1;
                 x <= (int) ((Symbol.Bounds.X + Symbol.Bounds.Width) / Node.NODE_SIZE) + 1;
                 ++x)
            {
                if (y >= 0 && x >= 0 && y < _grid.Count && x < _grid[0].Count)
                {
                    _grid[x][y].Walkable = false;
                }
            }
        }
        
        return _astar.FindPath(new Vector2((float) startPoint.X, (float) startPoint.Y),
            new Vector2((float) endPoint.X, (float) endPoint.Y));  
    }

    private Point CalculateSymbolPoints(PointPair pair)
    {
        List<Point> diamondPoints = new List<Point>();
        List<Point> trianglePoints = new List<Point>();
        Point symbolPoint = new();
        switch (pair.EndEdge)
        {
            case EdgeEnum.Top:
                symbolPoint = new Point(pair.End.X, pair.End.Y - (2 * SymbolHalfWidth));
                diamondPoints = new List<Point>
                {
                    symbolPoint,
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y + SymbolHalfWidth),
                    new(symbolPoint.X, symbolPoint.Y + (SymbolHalfWidth * 2)),
                    new(symbolPoint.X - SymbolHalfHeight, symbolPoint.Y + SymbolHalfWidth),
                    symbolPoint
                };
                trianglePoints = new List<Point>
                {
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y),
                    new(symbolPoint.X, symbolPoint.Y + (SymbolHalfWidth * 2)),
                    new(symbolPoint.X - SymbolHalfHeight, symbolPoint.Y),
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y)
                };
                break;
            case EdgeEnum.Right:
                symbolPoint = new Point(pair.End.X + (2 * SymbolHalfWidth), pair.End.Y);
                diamondPoints = new List<Point>
                {
                    symbolPoint,
                    new(symbolPoint.X - SymbolHalfWidth, symbolPoint.Y - SymbolHalfHeight),
                    new(symbolPoint.X - (SymbolHalfWidth * 2), symbolPoint.Y),
                    new(symbolPoint.X - SymbolHalfWidth, symbolPoint.Y + SymbolHalfHeight),
                    symbolPoint
                };
                trianglePoints = new List<Point>
                {
                    new(symbolPoint.X, symbolPoint.Y - SymbolHalfHeight),
                    new(symbolPoint.X - (SymbolHalfWidth * 2), symbolPoint.Y),
                    new(symbolPoint.X, symbolPoint.Y + SymbolHalfHeight),
                    new(symbolPoint.X, symbolPoint.Y - SymbolHalfHeight)
                };
                break;
            case EdgeEnum.Bottom:
                symbolPoint = new Point(pair.End.X, pair.End.Y + (2 * SymbolHalfWidth));
                diamondPoints = new List<Point>
                {
                    symbolPoint,
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y - SymbolHalfWidth),
                    new(symbolPoint.X, symbolPoint.Y - (SymbolHalfWidth * 2)),
                    new(symbolPoint.X - SymbolHalfHeight, symbolPoint.Y - SymbolHalfWidth),
                    symbolPoint
                };
                trianglePoints = new List<Point>
                {
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y),
                    new(symbolPoint.X, symbolPoint.Y - (SymbolHalfWidth * 2)),
                    new(symbolPoint.X - SymbolHalfHeight, symbolPoint.Y),
                    new(symbolPoint.X + SymbolHalfHeight, symbolPoint.Y)
                };
                break;
            case EdgeEnum.Left:
                symbolPoint = new Point(pair.End.X - (2 * SymbolHalfWidth), pair.End.Y);
                diamondPoints = new List<Point>
                {
                    symbolPoint,
                    new(symbolPoint.X + SymbolHalfWidth, symbolPoint.Y - SymbolHalfHeight),
                    new(symbolPoint.X + (SymbolHalfWidth * 2), symbolPoint.Y),
                    new(symbolPoint.X + SymbolHalfWidth, symbolPoint.Y + SymbolHalfHeight),
                    symbolPoint
                };
                trianglePoints = new List<Point>
                {
                    new(symbolPoint.X, symbolPoint.Y - SymbolHalfHeight),
                    new(symbolPoint.X + (SymbolHalfWidth * 2), symbolPoint.Y),
                    new(symbolPoint.X, symbolPoint.Y + SymbolHalfHeight),
                    new(symbolPoint.X, symbolPoint.Y - SymbolHalfHeight)
                };
                break;
        }
        
        // Draw the relationship symbol based on provided type
        switch (RelationshipType)
        {
            case "aggregation":
                Symbol = CreateRelationshipSymbol(diamondPoints);
                break;
            case "composition":
                Symbol = CreateRelationshipSymbol(diamondPoints);
                Symbol.Fill = Theme.Current.LinesColor;
                break;
            case "inheritance":
                Symbol = CreateRelationshipSymbol(trianglePoints);
                break;
            case "realization":
                foreach (Line l in Segments)
                {
                    l.StrokeDashArray = new AvaloniaList<double>(5, 3);
                }
                Symbol = CreateRelationshipSymbol(trianglePoints);
                break;
        }
        return symbolPoint;
    }
    
    /// <summary>
    /// Draws the relationship symbol from the given list of points
    /// </summary>
    /// <param name="points">A list of points for the vertices to draw</param>
    /// <returns>The new relationship symbol</returns>
    private Polyline CreateRelationshipSymbol(List<Point> points)
    {
        Polyline polyline = new Polyline();
        polyline.Name = "Polyline";
        polyline.Points = points;
        polyline.Stroke = Theme.Current.LinesColor;
        polyline.StrokeThickness = LineThickness;
        return polyline;
    }

    /// <summary>
    /// Creates a line from the given start to end points
    /// </summary>
    /// <param name="lineStart">Point to start at</param>
    /// <param name="lineEnd">Point to end at</param>
    /// <returns>The new line</returns>
    private Line CreateRelationshipLine(Point lineStart, Point lineEnd)
    {
        Line l = new Line();
        l.Name = "Line";
        l.StartPoint = lineStart;
        l.EndPoint = lineEnd;
        l.Stroke = Theme.Current.LinesColor;
        l.StrokeThickness = LineThickness;
        l.ZIndex = 10;
        return l;
    }
}