using System;
using System.Collections.Generic;
using System.Numerics;
using AStarSharp;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using UMLEditor.Views.Managers;

namespace UMLEditor.Views;

/// <summary>
/// RelationshipLine class for use in the main window.
/// </summary>
public class RelationshipLine
{

    private const double SymbolHalfWidth = 16;
    private const double SymbolHalfHeight = 12;
    private const int LineThickness = 2;
    private readonly IBrush _brush = Brushes.CornflowerBlue;

    private static List<List<Node>> _grid = new();
    private static Astar _astar = new(_grid);

    private List<Point> _gridPoints = new();
    
    /// <summary>
    /// SourceClass control for relationship line
    /// </summary>
    public ClassBox SourceClass { get; private set; }
    
    /// <summary>
    /// Destination class for relationship line
    /// </summary>
    public ClassBox DestClass { get; private set; }
    // Resharper disable once NotAccessedField.local
    
    /// <summary>
    /// Relationship type for relationship line
    /// </summary>
    public string RelationshipType { get; private set; }
    
    /// <summary>
    /// Symbol for relationship line.
    /// </summary>
    public Polyline Symbol { get; private set; }

    /// <summary>
    /// List of line segments for relationship line
    /// </summary>
    public List<Line> Segments { get; private set; }

    /// <summary>
    /// Enum for source edge.
    /// </summary>
    public EdgeEnum SourceEdge { get; private set; }
    
    /// <summary>
    /// Enum for source edge.
    /// </summary>
    public EdgeEnum DestEdge { get; private set; }

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
    }

    /// <summary>
    /// Initializes the pathing grid to the correct size and creates nodes for each coordinate
    /// </summary>
    /// <param name="canvas">The canvas to get size of for grid (currently not working)</param>
    public static void InitializeGrid(Canvas canvas)
    {
        // Currently not getting bounds correctly, using placeholder values
        int maxX = 2000 / Node.NODE_SIZE, maxY = 2000 / Node.NODE_SIZE;
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
    
    /// <summary>
    /// Gets the straight line distance between two points
    /// </summary>
    /// <param name="p1">First point</param>
    /// <param name="p2">Second point</param>
    /// <returns></returns>
    private double GetDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y,2));
    }

    /// <summary>
    /// Sets a grid location to not be walkable for the path search algorithm
    /// </summary>
    /// <param name="x">X coordinate of point</param>
    /// <param name="y">Y coordinate of point</param>
    public static void MakeNotWalkable(int x, int y)
    {
        _grid[x][y].Walkable = false;
    }

    /// <summary>
    /// Function to draw the line.
    /// </summary>
    /// <param name="myCanvas">The canvas in which the line will be drawn.</param>
    public void Draw(Canvas myCanvas)
    {
        // Get ClassBox objects for source and destination
        //ClassBox sourceClassBox = ClassBoxes.FindByName(SourceClass.Name);
        //ClassBox destClassBox = ClassBoxes.FindByName(DestClass.Name);
        ClassBox sourceClassBox = SourceClass;
        ClassBox destClass = DestClass;
        
        // Reset grid nodes where previous lines were
        foreach (Point p in _gridPoints)
        {
            _grid[(int)p.X][(int)p.Y].Walkable = true;
        }
        _gridPoints.Clear();
        
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
       
        Point start = new Point();
        Point end = new Point();
        
        // Find shortest distance between edges
        double shortestDist = Double.MaxValue;
        foreach (Point p1 in startEdgePoints)
        {
            foreach (Point p2 in endEdgePoints)
            {
                double thisDistance = GetDistance(p1, p2);
                if (thisDistance < shortestDist)
                {
                    shortestDist = thisDistance;
                    start = p1;
                    end = p2;
                }
            }
        }

        int startEdgeIndex = startEdgePoints.IndexOf(start);
        int endEdgeIndex = endEdgePoints.IndexOf(end);
        
        Point lineStart = new Point();
        Point lineEnd = new Point();
        Point symbolPoint = new Point();
        List<Point> diamondPoints = new List<Point>();
        List<Point> trianglePoints = new List<Point>();

        Stack<Node>? path = null;
        int sourceCount = 0, destCount = 0;
        // Calculate path between edges
        while (path is null && sourceCount < 4)
        {
            if (path is null)
            {
                switch (startEdgeIndex)
                {
                    case 0:
                        lineStart = new Point(
                            startEdgePoints[startEdgeIndex].X - (startEdgePoints[startEdgeIndex].X % Node.NODE_SIZE),
                            startEdgePoints[startEdgeIndex].Y - (startEdgePoints[startEdgeIndex].Y % Node.NODE_SIZE) -
                            Node.NODE_SIZE);
                        break;
                    case 1:
                        lineStart = new Point(
                            startEdgePoints[startEdgeIndex].X +
                            (Node.NODE_SIZE - (startEdgePoints[startEdgeIndex].X % Node.NODE_SIZE)) + Node.NODE_SIZE,
                            startEdgePoints[startEdgeIndex].Y - (startEdgePoints[startEdgeIndex].Y % Node.NODE_SIZE));
                        break;
                    case 2:
                        lineStart = new Point(
                            startEdgePoints[startEdgeIndex].X - (startEdgePoints[startEdgeIndex].X % Node.NODE_SIZE),
                            startEdgePoints[startEdgeIndex].Y +
                            (Node.NODE_SIZE - (startEdgePoints[startEdgeIndex].Y % Node.NODE_SIZE)) + Node.NODE_SIZE);
                        break;
                    case 3:
                        lineStart = new Point(
                            startEdgePoints[startEdgeIndex].X - (startEdgePoints[startEdgeIndex].X % Node.NODE_SIZE) -
                            Node.NODE_SIZE,
                            startEdgePoints[startEdgeIndex].Y - (startEdgePoints[startEdgeIndex].Y % Node.NODE_SIZE));
                        break;
                }

                // Set end edge and create symbol points
                switch (endEdgeIndex)
                {
                    case 0:
                        end = new Point(endEdgePoints[endEdgeIndex].X,
                            endEdgePoints[endEdgeIndex].Y - (SymbolHalfWidth * 2));
                        lineEnd = new Point(end.X - (end.X % Node.NODE_SIZE), end.Y - (end.Y % Node.NODE_SIZE));
                        _gridPoints.Add(end);
                        diamondPoints = new List<Point>
                        {
                            end,
                            new(end.X + SymbolHalfHeight, end.Y + SymbolHalfWidth),
                            new(end.X, end.Y + (SymbolHalfWidth * 2)),
                            new(end.X - SymbolHalfHeight, end.Y + SymbolHalfWidth),
                            end
                        };
                        trianglePoints = new List<Point>
                        {
                            new(end.X + SymbolHalfHeight, end.Y),
                            new(end.X, end.Y + (SymbolHalfWidth * 2)),
                            new(end.X - SymbolHalfHeight, end.Y),
                            new(end.X + SymbolHalfHeight, end.Y)
                        };
                        symbolPoint = end;
                        break;
                    case 1:
                        end = new Point(endEdgePoints[endEdgeIndex].X + (SymbolHalfWidth * 2),
                            endEdgePoints[endEdgeIndex].Y);
                        lineEnd = new Point(end.X + (Node.NODE_SIZE - (end.X % Node.NODE_SIZE)),
                            end.Y - (end.Y % Node.NODE_SIZE));
                        _gridPoints.Add(end);
                        diamondPoints = new List<Point>
                        {
                            end,
                            new(end.X - SymbolHalfWidth, end.Y - SymbolHalfHeight),
                            new(end.X - (SymbolHalfWidth * 2), end.Y),
                            new(end.X - SymbolHalfWidth, end.Y + SymbolHalfHeight),
                            end
                        };
                        trianglePoints = new List<Point>
                        {
                            new(end.X, end.Y - SymbolHalfHeight),
                            new(end.X - (SymbolHalfWidth * 2), end.Y),
                            new(end.X, end.Y + SymbolHalfHeight),
                            new(end.X, end.Y - SymbolHalfHeight)
                        };
                        symbolPoint = end;
                        break;
                    case 2:
                        end = new Point(endEdgePoints[endEdgeIndex].X,
                            endEdgePoints[endEdgeIndex].Y + (SymbolHalfWidth * 2));
                        lineEnd = new Point(end.X - (end.X % Node.NODE_SIZE),
                            end.Y + (Node.NODE_SIZE - (end.Y % Node.NODE_SIZE)));
                        _gridPoints.Add(end);
                        diamondPoints = new List<Point>
                        {
                            end,
                            new(end.X + SymbolHalfHeight, end.Y - SymbolHalfWidth),
                            new(end.X, end.Y - (SymbolHalfWidth * 2)),
                            new(end.X - SymbolHalfHeight, end.Y - SymbolHalfWidth),
                            end
                        };
                        trianglePoints = new List<Point>
                        {
                            new(end.X + SymbolHalfHeight, end.Y),
                            new(end.X, end.Y - (SymbolHalfWidth * 2)),
                            new(end.X - SymbolHalfHeight, end.Y),
                            new(end.X + SymbolHalfHeight, end.Y)
                        };
                        symbolPoint = end;
                        break;
                    case 3:
                        end = new Point(endEdgePoints[endEdgeIndex].X - (SymbolHalfWidth * 2),
                            endEdgePoints[endEdgeIndex].Y);
                        lineEnd = new Point(end.X - (end.X % Node.NODE_SIZE), end.Y - (end.Y % Node.NODE_SIZE));
                        _gridPoints.Add(end);
                        diamondPoints = new List<Point>
                        {
                            end,
                            new(end.X + SymbolHalfWidth, end.Y - SymbolHalfHeight),
                            new(end.X + (SymbolHalfWidth * 2), end.Y),
                            new(end.X + SymbolHalfWidth, end.Y + SymbolHalfHeight),
                            end
                        };
                        trianglePoints = new List<Point>
                        {
                            new(end.X, end.Y - SymbolHalfHeight),
                            new(end.X + (SymbolHalfWidth * 2), end.Y),
                            new(end.X, end.Y + SymbolHalfHeight),
                            new(end.X, end.Y - SymbolHalfHeight)
                        };
                        symbolPoint = end;
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
                        Symbol.Fill = _brush;
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
                
                // Make symbol area non-walkable
                for (int y = (int)(Symbol.Bounds.Y / Node.NODE_SIZE)-1;
                     y <= (int)((Symbol.Bounds.Y + Symbol.Bounds.Height) / Node.NODE_SIZE)+1;
                     ++y)
                {
                    for (int x= (int)(Symbol.Bounds.X / Node.NODE_SIZE)-1;
                         x <= (int)((Symbol.Bounds.X + Symbol.Bounds.Width) / Node.NODE_SIZE)+1;
                         ++x)
                    {
                        if (y >= 0 && x >= 0)
                        {
                            _grid[x][y].Walkable = false;
                        }
                    }
                }
                
                // Check for out of bounds
                lineStart = new Point(Math.Clamp(lineStart.X, 0, _grid[0].Count * Node.NODE_SIZE),
                    Math.Clamp(lineStart.Y, 0, _grid.Count * Node.NODE_SIZE));
                end = new Point(Math.Clamp(end.X, 0, _grid[0].Count * Node.NODE_SIZE),
                    Math.Clamp(end.Y, 0, _grid.Count * Node.NODE_SIZE));

                // Calculate path from source edge to destination edge
                path = _astar.FindPath(new Vector2((float) lineStart.X, (float) lineStart.Y),
                    new Vector2((float) end.X, (float) end.Y));
                // If no path found, cycle through edges
                if (path is null)
                {
                    ++destCount;
                    ++endEdgeIndex;
                    if (endEdgeIndex > 3)
                    {
                        endEdgeIndex = 0;
                    }

                    end = endEdgePoints[endEdgeIndex];
                    if (destCount == 4)
                    {
                        destCount = 0;
                        ++sourceCount;
                        ++startEdgeIndex;
                        if (startEdgeIndex > 3)
                        {
                            startEdgeIndex = 0;
                        }

                        start = startEdgePoints[startEdgeIndex];
                    }
                }
            }
        }
        
        // Set source edge
        switch (startEdgeIndex)
        {
            case 0:
                SourceEdge = EdgeEnum.Top;
                sourceClassBox.AddToEdge(this, EdgeEnum.Top);
                break;
            case 1:
                SourceEdge = EdgeEnum.Right;
                sourceClassBox.AddToEdge(this, EdgeEnum.Right);
                break;
            case 2:
                SourceEdge = EdgeEnum.Bottom;
                sourceClassBox.AddToEdge(this, EdgeEnum.Bottom);
                break;
            case 3:
                SourceEdge = EdgeEnum.Left;
                sourceClassBox.AddToEdge(this, EdgeEnum.Left);
                break;
        }
    
        // Set end edge
        switch (endEdgeIndex)
        {
            case 0:
                DestEdge = EdgeEnum.Top;
                //destClassBox.AddToEdge(this, EdgeEnum.Top);
                break;
            case 1:
                DestEdge = EdgeEnum.Right;
                //destClassBox.AddToEdge(this, EdgeEnum.Right);
                break;
            case 2:
                DestEdge = EdgeEnum.Bottom;
                //destClassBox.AddToEdge(this, EdgeEnum.Bottom);
                break;
            case 3:
                DestEdge = EdgeEnum.Left;
                //destClassBox.AddToEdge(this, EdgeEnum.Left);
                break;
        }
        
        // Draw lines
        // Situation 1: A path was found
        if (path is not null)
        {
            _gridPoints.Add(lineStart);
            Line newLine = CreateRelationshipLine(start, lineStart);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            Point pos = path.Pop().Center;
            _gridPoints.Add(pos);
            newLine = CreateRelationshipLine(lineStart, pos);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            while (path.Count > 1)
            {
                Point newPos = path.Pop().Center;
                _gridPoints.Add(newPos);
                newLine = CreateRelationshipLine(pos, newPos);
                Segments.Add(newLine);
                myCanvas.Children.Add(newLine);
                pos = newPos;
            }

            _gridPoints.Add(lineEnd);
            newLine = CreateRelationshipLine(lineEnd, pos);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            
            _gridPoints.Add(end);
            newLine = CreateRelationshipLine(lineEnd, symbolPoint);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            
            foreach (Point g in _gridPoints)
            {
                if (g.X >= 0 && g.Y >= 0)
                {
                    _grid[(int) g.X][(int) g.Y].Walkable = false;
                }
            }
        }
        // Situation 2: No path found, draw straight line from start to end
        else
        {
            Point midPoint = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            Line newLine = CreateRelationshipLine(start, midPoint);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
            newLine = CreateRelationshipLine(midPoint, end);
            Segments.Add(newLine);
            myCanvas.Children.Add(newLine);
        }

        /*using StreamWriter file = new("grid.txt");
        for (int y = 0; y < 100; ++y)
        {
            for (int x = 0; x < 100; ++x)
            {
                if (_grid[x][y].Walkable == false)
                    file.Write("X");
                else
                {
                    file.Write("_");
                }
            }
            file.WriteLine();
        }*/
        
        myCanvas.Children.Add(Symbol);
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
        polyline.Stroke = _brush;
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
        l.Stroke = _brush;
        l.StrokeThickness = LineThickness;
        l.ZIndex = 10;
        return l;
    }
}