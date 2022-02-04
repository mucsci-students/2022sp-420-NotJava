using Newtonsoft.Json;
using UMLEditor.Exceptions;

namespace UMLEditor.Classes;

using System.Collections.Generic;
public class Class
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("classname")]
    public string ClassName { get; private set; }
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("attributes")]
    public List<AttributeObject> Attributes { get; private set; }

    /// <summary>
    /// Default constructor for class
    /// </summary>
    public Class()
    {
        ClassName = "";
        Attributes = new List<AttributeObject>();
    }

    /// <summary>
    /// Constructor for class "name"
    /// </summary>
    /// <param name="name">Name of the class being created</param>
    public Class(string name) : this()
    {

        ClassName = name;

    }
    
    /// <summary>
    /// Check if specified attribute exists.
    /// </summary>
    /// <param name="name">Name of the attribute you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    public bool AttributeExists (string name)
    {

        return GetAttributeByName(name) != null;

    }

    /// <summary>
    /// Finds the attribute with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of attribute you are looking for</param>
    /// <returns>Returns the attribute if exists, or null if it does not</returns>
    public AttributeObject GetAttributeByName(string name)
    {
        foreach (AttributeObject CurrentAttribute in Attributes)
        {
            if (CurrentAttribute.AttributeName == name)
            {
                return CurrentAttribute;
            }
        }
        return null; 
    }
    
    /// <summary>
    /// Deletes an attribute within a specified class
    /// </summary>
    /// <param name="ClassName">The source class for the relationship</param>
    /// <param name="AttributeName">AttributeObject to be deleted</param>
    /// <exception cref="ClassNonexistentException">If class does not exist</exception>
    /// <exception cref="AttributeNonexistentException">If attribute does not exist</exception>
    public void DeleteAttribute(string ClassName, string TargetAttributeName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent attribute name entered ({0}).";
        
        // Ensure the provided classes exist
        bool RemoveWorked = Attributes.Remove(GetAttributeByName(TargetAttributeName));
        if (!RemoveWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, TargetAttributeName));
            
        }
        
        // Delete the specified attribute from the class.
        //Attributes.Remove(GetAttributeByName(TargetAttributeName));

    }
}