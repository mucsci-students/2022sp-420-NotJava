namespace UMLEditor.Classes;

using System;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

public class AttributeObject
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("attributename")]
    public string AttributeName { get; private set; }

    public AttributeObject()
    {
        AttributeName = "";
    }

    /// <summary>
    /// Constructs Attribute object.  Ensures name is valid
    /// </summary>
    /// <param name="name">Name of attribute</param>
    public AttributeObject(string name)
    {
        CheckValidAttributeName(name);
        AttributeName = name;
    }

    /// <summary>
    /// Stringifies Attribute
    /// </summary>
    /// <returns>Formatted string of attribute</returns>
    public override string ToString()
    {
        return string.Format("Attribute: {0}", AttributeName);
    }
    
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
    private void CheckValidAttributeName(string name)
    {
        if (!Char.IsLetter(name[0]) && name[0] != '_')
        {
            throw new InvalidNameException(String.Format("{0} is an invalid attribute name.  " +
                                                         "Attribute name must be a single word that starts with an alphabetic " +
                                                         "character or an underscore.  " +
                                                         "Please Try again.", name));
        }
    }
}