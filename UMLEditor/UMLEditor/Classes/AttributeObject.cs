using System;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

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

    /// <summary>
    /// Constructs Attribute object.  Ensures name is valid
    /// </summary>
    /// <param name="name">Name of attribute</param>
    public AttributeObject(string name)
    {
        checkValidAttributeName(name);
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
        checkValidAttributeName(name);
        AttributeName = name;
    }
    
    private void checkValidAttributeName(string name)
    {
        if (!Char.IsLetter(name[0]) && name[0] != '_')
        {
            throw new InvalidNameException(String.Format("{0} is an invalid class name.  " +
                                                         "Class name must be a single word that starts with an alphabetic " +
                                                         "character or an underscore.  " +
                                                         "Please Try again.", name));
        }
    }
}