namespace UMLEditor.Classes;

using Newtonsoft.Json;

public class Relationship
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("sourceclass")]
    public string SourceClass { get; private set; }
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("destinationclass")]
    public string DestinationClass { get; private set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    public Relationship()
    {
        SourceClass = "";
        DestinationClass = "";
    }
    
    /// <summary>
    /// Constructs a relationship between the two classes
    /// </summary>
    /// <param name="source">The source class in the relationship</param>
    /// <param name="destination">The destination class in the relationship</param>
    public Relationship(string source, string destination)
    {
        SourceClass = source;
        DestinationClass = destination;
    }

    /// <summary>
    /// Renames the provided class to the new name, if it is a source or destination class.
    /// </summary>
    /// <param name="className">The class to rename.</param>
    /// <param name="newName">The new name.</param>
    public void RenameMember(string className, string newName)
    {

        if (SourceClass == className)
        {
            SourceClass = newName;
        }
        
        if (DestinationClass == className)
        {
            DestinationClass = newName;
        }
        
    }
    
    public override string ToString()
    {
        return string.Format("{0} => {1}", SourceClass, DestinationClass);
    }
}
