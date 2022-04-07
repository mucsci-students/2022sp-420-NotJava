using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace UMLEditor.Views;

/// <summary>
/// RelationshipLine class for use in the main window.
/// </summary>
public class RelationshipLine
{
    
    private const double SymbolWidth = 15;
    private const double SymbolHeight = 10;
    private const int LineThickness = 2;
    private readonly IBrush _brush = Brushes.CornflowerBlue;
    
    /// <summary>
    /// SourceClass control for relationship line
    /// </summary>
    public UserControl SourceClass { get; private set; }
    
    /// <summary>
    /// Destination class for relationship line
    /// </summary>
    public UserControl DestClass { get; private set; }
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
    public RelationshipLine(UserControl startCtrl, UserControl endCtrl, string relationshipType)
    {
        SourceClass = startCtrl;
        DestClass = endCtrl;
        RelationshipType = relationshipType;
        Segments = new List<Line>();
    }

    /// <summary>
    /// Function to draw the line.
    /// </summary>
    /// <param name="myCanvas">The canvas in which the line will be drawn.</param>
    public void Draw(Canvas myCanvas)
    {
            // Calculate lengths of controls.  This is accomplished via dividing all bounds by two.
            double startHalfWidth = SourceClass.Bounds.Width / 2;
            double startHalfHeight = SourceClass.Bounds.Height / 2;
            double endHalfWidth = DestClass.Bounds.Width / 2;
            double endHalfHeight = DestClass.Bounds.Height / 2;
            // Initialize points to middle of controls
            Point start = new Point(
                SourceClass.Bounds.X + startHalfWidth,
                SourceClass.Bounds.Y + startHalfHeight);
            Point end = new Point(
                DestClass.Bounds.X + endHalfWidth,
                DestClass.Bounds.Y + endHalfHeight);

            // Set points to draw lines
            Point midStart;
            Point midEnd;
            List<Point> diamondPoints;
            List<Point> trianglePoints;
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                // Arrow is horizontal.  Checks the distance between the x on the box and the y on the box.  
                // Draws the arrow horizontally if the x is longer than the y.
                midStart = new Point(Math.Abs(start.X + end.X) / 2, start.Y);
                midEnd = new Point(Math.Abs(start.X + end.X) / 2, end.Y);
                if (start.X < end.X)
                {
                    // Goes from left to right.  
                    start = new Point(start.X + startHalfWidth, start.Y);
                    end = new Point(end.X - endHalfWidth - (2 * SymbolWidth), end.Y);
                    // Two different symbols, each one made up of a list of points.
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X + SymbolWidth,end.Y - SymbolHeight),
                        new(end.X + (2 * SymbolWidth),end.Y),
                        new(end.X + SymbolWidth,end.Y + SymbolHeight),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X,end.Y - SymbolHeight),
                        new(end.X + (2 * SymbolWidth),end.Y),
                        new(end.X,end.Y + SymbolHeight),
                        new(end.X,end.Y - SymbolHeight)
                    };
                }
                else
                {
                    // Goes from right to left
                    start = new Point(start.X - startHalfWidth, start.Y);
                    end = new Point(end.X + endHalfWidth + (2 * SymbolWidth), end.Y);
                    // Two different symbols, each one made up of a list of points.
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X - SymbolWidth,end.Y - SymbolHeight),
                        new(end.X - (2 * SymbolWidth),end.Y),
                        new(end.X - SymbolWidth,end.Y + SymbolHeight),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X,end.Y - SymbolHeight),
                        new(end.X - (2 * SymbolWidth),end.Y),
                        new(end.X,end.Y + SymbolHeight),
                        new(end.X,end.Y - SymbolHeight)
                    };
                }
            }
            else
            {
                // Arrow is vertical
                // Draws the arrow horizontally if the y is longer than the x.
                midStart = new Point(start.X, Math.Abs(start.Y + end.Y) / 2);
                midEnd = new Point(end.X, Math.Abs(start.Y + end.Y) / 2);
                if (start.Y < end.Y)
                {
                    // Goes top to bottom
                    start = new Point(start.X, start.Y + startHalfHeight);
                    end = new Point(end.X, end.Y - endHalfHeight - (2 * SymbolWidth));
                    // Two different symbols, each one made up of a list of points.
                    diamondPoints = new List<Point>
                    {
                        end,
                        new(end.X + SymbolHeight, end.Y + SymbolWidth),
                        new(end.X, end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y + SymbolWidth),
                        end
                    };
                    trianglePoints = new List<Point>
                    {
                        new(end.X + SymbolHeight, end.Y),
                        new(end.X, end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y),
                        new(end.X + SymbolHeight, end.Y)
                    };
                }
                else
                {
                    // Goes bottom to top
                    start = new Point(start.X, start.Y - startHalfHeight);
                    end = new Point(end.X, end.Y + endHalfHeight + (2 * SymbolWidth));
                    // Two different symbols, each one made up of a list of points.
                    diamondPoints = new List<Point>
                    {
                        end,
                        new(end.X + SymbolHeight, end.Y - SymbolWidth),
                        new(end.X, end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y - SymbolWidth),
                        end
                    };
                    trianglePoints = new List<Point>
                    {
                        new(end.X + SymbolHeight, end.Y),
                        new(end.X, end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y),
                        new(end.X + SymbolHeight, end.Y)
                    };
                }
            }
            
            // Adding three lines to the list of segments, accounting for start, middle, end, and the between.
            Segments.Add(CreateRelationshipLine(start, midStart));
            Segments.Add(CreateRelationshipLine(midStart, midEnd));
            Segments.Add(CreateRelationshipLine(midEnd, end));

            // Add lines to the canvas
            myCanvas.Children.Add(Segments[0]);
            myCanvas.Children.Add(Segments[1]);
            myCanvas.Children.Add(Segments[2]);

            // Draw the relationship symbol based on provided type
            switch (RelationshipType)
            {
                case "aggregation":
                    myCanvas.Children.Add(CreateRelationshipSymbol(diamondPoints));
                    break;
                case "composition":
                    Polyline polyline = CreateRelationshipSymbol(diamondPoints);
                    polyline.Fill = _brush;
                    myCanvas.Children.Add(polyline);
                    break;
                case "inheritance":
                    Symbol = CreateRelationshipSymbol(trianglePoints);
                    myCanvas.Children.Add(Symbol);
                    break;
                case "realization":
                    Segments[0].StrokeDashArray = new AvaloniaList<double>(5, 3);
                    Segments[1].StrokeDashArray = new AvaloniaList<double>(5, 3);
                    Segments[2].StrokeDashArray = new AvaloniaList<double>(5, 3);
                    Symbol = CreateRelationshipSymbol(trianglePoints);
                    myCanvas.Children.Add(Symbol);
                    break;
            }
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

