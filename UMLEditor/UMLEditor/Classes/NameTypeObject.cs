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
    /// Constructs a parameter with the provided name and type
    /// </summary>
    /// <param name="withName">The name of the parameter</param>
    /// <param name="withType">The name of the type of the parameter</param>
    public NameTypeObject(string withName, string withType)
    {
        
        CheckValidAttributeName(withName);
        AttributeName = withName;
        Type = withType;

    }

    public override string ToString()
    {
        return $"{Type} {AttributeName}";
    }
    
}