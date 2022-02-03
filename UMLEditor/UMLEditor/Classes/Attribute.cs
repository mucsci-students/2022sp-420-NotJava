using Newtonsoft.Json;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Attribute
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("attributename")]
    private string AttributeName;

    public Attribute()
    {
        AttributeName = "";
    }

    public Attribute(string name)
    {
        AttributeName = name;
    }
    
    private bool IsValidName(string name)
    {
        // TODO
        return true;
    }
    
    public override string ToString()
    {
        return string.Format("Attribute: {0}", AttributeName);
    }
}