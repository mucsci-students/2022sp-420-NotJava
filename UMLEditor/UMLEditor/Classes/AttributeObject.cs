using Newtonsoft.Json;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class AttributeObject
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("attributename")]
    public string AttributeName { get; private set; }

    public AttributeObject()
    {
        AttributeName = "";
    }

    public AttributeObject(string name)
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
    
    public void AttRename(string name)
    {
        AttributeName = name;
    }
}