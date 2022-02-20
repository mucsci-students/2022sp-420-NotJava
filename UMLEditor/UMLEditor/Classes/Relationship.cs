namespace UMLEditor.Classes;

using Newtonsoft.Json;
using System.Collections.Generic;
using Exceptions;

public class Relationship
{
    // Valid relationship types
    private readonly List<string> _validTypes = new List<string>{"aggregation", "composition", "inheritance", "realization"};
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("source")]
    public string SourceClass { get; private set; }
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("destination")]
    public string DestinationClass { get; private set; }
    
    // Used for JSON serialization and deserialization
    [JsonProperty("type")]
    public string RelationshipType { get; private set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    public Relationship()
    {
        SourceClass = "";
        DestinationClass = "";
        RelationshipType = "";
    }

    /// <summary>
    /// Constructs a relationship between the two classes
    /// </summary>
    /// <param name="source">The source class in the relationship</param>
    /// <param name="destination">The destination class in the relationship</param>
    /// <param name="type">The type of relationship</param>
    /// <exception cref="InvalidRelationshipTypeException">The given type is not valid</exception>
    public Relationship(string source, string destination, string type)
    {
        SourceClass = source;
        DestinationClass = destination;
        if (_validTypes.Contains(type))
        {
            RelationshipType = type;
        }
        else
        {
            throw new InvalidRelationshipTypeException(string.Format("{0} is not a valid relationship type.", type));
        }
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
        return string.Format("{0}: {1} => {2}",RelationshipType, SourceClass, DestinationClass);
    }
}
