using Newtonsoft.Json;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Relationship
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("sourceclass")]
    private string SourceClass;
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("destinationclass")]
    private string DestinationClass;

    public Relationship()
    {
        SourceClass = "";
        DestinationClass = "";
    }
    public Relationship(string source, string destination)
    {
        SourceClass = source;
        DestinationClass = destination;
    }
    
    public override string ToString()
    {
        return string.Format("{0} => {1}", SourceClass, DestinationClass);
    }
}