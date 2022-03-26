namespace UMLEditor.Classes;

using System;
using Newtonsoft.Json;

/// <summary>
/// A generic type that contains two string fields, a name, and a type.
/// </summary>
#pragma warning disable CS0660, CS0661
public class NameTypeObject : AttributeObject, ICloneable
#pragma warning restore CS0660, CS0661
{
    
    /// <summary>
    /// Properties for type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; private set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    public NameTypeObject()
    {

        AttributeName = "";
        Type = "";

    }

    /// <summary>
    /// Constructs a parameter with the provided name and type
    /// </summary>
    /// <param name="withType">The name of the type of the parameter</param>
    /// <param name="withName">The name of the parameter</param>
    public NameTypeObject(string withType, string withName)
    {
        
        CheckValidAttributeName(withName);
        CheckValidAttributeName(withType);
        AttributeName = withName;
        Type = withType;

    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="n">NameTypeObject to copy</param>
    public NameTypeObject(NameTypeObject n) : this(n.Type, n.AttributeName)
    { }

    /// <summary>
    /// Changes the type of this NameTypeObject
    /// </summary>
    /// <param name="newType">The new type to apply</param>
    public void ChangeType(string newType)
    {
        
        CheckValidAttributeName(newType);
        Type = newType;

    }
    
    /// <summary>
    /// Equals operation for NameTypeObject
    /// </summary>
    /// <param name="a">The first argument given</param>
    /// <param name="b">The second argument given</param>
    /// <returns></returns>
    public static bool operator ==(NameTypeObject? a, NameTypeObject? b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        else if ((a is null && !(b is null)) || (!(a is null) && b is null))
        {
            return false;
        }
        return ((a!.Type == b!.Type) && (a.AttributeName == b.AttributeName));
    }
    
    /// <summary>
    /// Not equals operator
    /// </summary>
    /// <param name="a">The first argument given</param>
    /// <param name="b">The second argument given</param>
    /// <returns></returns>
    public static bool operator !=(NameTypeObject a, NameTypeObject b)
    {
        return !(a == b);
    }
    
    /// <summary>
    /// To string function for NameTypeObject
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Type} {AttributeName}";
    }

    /// <summary>
    /// Clone function for NameTypeObject
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        return new NameTypeObject(this);
    }

}