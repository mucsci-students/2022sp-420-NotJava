namespace UMLEditor.Classes;

using Newtonsoft.Json;

/// <summary>
/// A generic type that contains two string fields, a name, and a type.
/// </summary>
public class NameTypeObject : AttributeObject
{
    
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


    public static bool operator ==(NameTypeObject a, NameTypeObject b)
    {
        if (a is null && b is null)
        {
            return true;
        }
        else if ((a is null && !(b is null)) || (!(a is null) && b is null))
        {
            return false;
        }
        return ((a.Type == b.Type) && (a.AttributeName == b.AttributeName));
    }
    
    public static bool operator !=(NameTypeObject a, NameTypeObject b)
    {
        return !(a == b);
    }
    
    public override string ToString()
    {
        return $"{Type} {AttributeName}";
    }
    
}