using System;
using Newtonsoft.Json;

namespace UMLEditor.Classes;

/// <summary>
/// Box Location
/// </summary>
public class BoxLocation : ICloneable
{

    /// <summary>
    /// X value
    /// </summary>
    [JsonProperty("x")]
    public int X { get; private set; }

    /// <summary>
    /// Y Value
    /// </summary>
    [JsonProperty("y")]
    public int Y { get; private set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    public BoxLocation()
    {

        X = 0;
        Y = 0;

    }

    /// <summary>
    /// Copy ctor
    /// </summary>
    /// <param name="other">The location to copy</param>
    private BoxLocation(BoxLocation other)
    {

        this.X = other.X;
        this.Y = other.Y;

    }
    
    /// <summary>
    /// Changes the X and Y values of this location
    /// </summary>
    /// <param name="newX">The new X value</param>
    /// <param name="newY">The new Y value</param>
    public void ChangeXY(int newX, int newY)
    {

        X = newX;
        Y = newY;

    }

    /// <summary>
    /// Clone function
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        return new BoxLocation(this);
    }
    
}