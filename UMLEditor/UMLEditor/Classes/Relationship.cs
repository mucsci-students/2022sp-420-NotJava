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
    public Relationship(string source, string destination, string type)
    {
        SourceClass = source;
        DestinationClass = destination;
        if (IsValidType(type))
        {
            RelationshipType = type;
        }
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="r">Relationship object to copy</param>
    public Relationship(Relationship r) : this(r.SourceClass, r.DestinationClass, r.RelationshipType)
    { }

    /// <summary>
    /// Helper function to check if a given relationship type is valid
    /// </summary>
    /// <param name="type">The relationship type to check</param>
    /// <returns></returns>
    /// <exception cref="InvalidRelationshipTypeException">If the given type is not valid</exception>
    private bool IsValidType(string type)
    {
        if (!_validTypes.Contains(type))
        {
            throw new InvalidRelationshipTypeException($"{type} is not a valid relationship type.");
        }
        return true;
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

    /// <summary>
    /// Changes the type of a relationship to the given type
    /// </summary>
    /// <param name="newType">The new relationship type</param>
    public void ChangeType(string newType)
    {
        if (IsValidType(newType))
        {
            RelationshipType = newType;
        }
    }
    
    /// <summary>
    /// Overridden function to display relationship as a string
    /// </summary>
    /// <returns>The relationship as [RelationshipType]: [SourceClass] => [DestinationClass]</returns>
    public override string ToString()
    {
        return $"{RelationshipType}: {SourceClass} => {DestinationClass}";
    }
}
