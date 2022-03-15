namespace UMLEditor.Classes;

using System;
using Newtonsoft.Json;
using Exceptions;

/// <summary>
/// Attribute Class
/// </summary>
public abstract class AttributeObject
{
    #pragma warning disable CS1591
    #pragma warning disable CS8618
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("name")]
    public string AttributeName { get; protected set; }
    
    /// <summary>
    /// Stringifies Attribute
    /// </summary>
    /// <returns>Formatted string of attribute</returns>
    public override string ToString()
    {
        return ($"Attribute: {AttributeName}");
    }
    
    #pragma warning restore CS8618
    #pragma warning restore CS1591
    
    /// <summary>
    /// Renames attribute.  Checks to make sure name is valid
    /// Pre condition: attribute "name" does not exist
    /// </summary>
    /// <param name="name">Name to rename attribute to</param>
    public void AttRename(string name)
    {
        CheckValidAttributeName(name);
        AttributeName = name;
    }
    
    /// <summary>
    /// Checks if the provided name is a valid attribute name.
    /// </summary>
    /// <param name="name">The name to test.</param>
    /// <exception cref="InvalidNameException">If the provided name is invalid.</exception>
    protected void CheckValidAttributeName(string name)
    {
        if (name is null || name.Length == 0 || !Char.IsLetter(name[0]) && name[0] != '_' || name.Contains(" "))
        {
            throw new InvalidNameException($"'{name}' is an invalid attribute name or type.  " +
                                                         "Attribute name/type must be a single word that starts with an alphabetic " +
                                                         "character or an underscore.  " +
                                                         "Please Try again.");
        }
    }
}