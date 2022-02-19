namespace UMLEditor.Classes;

using Newtonsoft.Json;

public class Parameter : AttributeObject
{
    
    [JsonProperty("type")]
    public string Type { get; private set; }

    /// <summary>
    /// Constructs a parameter with the provided name and type
    /// </summary>
    /// <param name="withName">The name of the parameter</param>
    /// <param name="withType">The name of the type of the parameter</param>
    public Parameter(string withName, string withType)
    {

        AttributeName = withName;
        Type = withType;

    }

    public override string ToString()
    {
        return $"{Type} {AttributeName}";
    }
    
}