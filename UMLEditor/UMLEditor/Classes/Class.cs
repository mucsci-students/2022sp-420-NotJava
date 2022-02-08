using System.Reflection.PortableExecutable;
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
    /// Adds attribute to class.
    /// Pre-condition: name of attribute is valid
    /// </summary>
    /// <param name="name">Valid name of new attribute</param>
    public void AddAttribute(string name)
    {
        Attributes.Add(new AttributeObject(name));
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
    /// <param name="TargetAttributeName">AttributeObject to be deleted</param>
    /// <exception cref="AttributeNonexistentException">If attribute does not exist</exception>
    public void DeleteAttribute(string TargetAttributeName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent attribute name entered ({0}).";
        
        // Ensure the provided classes exist
        bool RemoveWorked = Attributes.Remove(GetAttributeByName(TargetAttributeName));
        if (!RemoveWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, TargetAttributeName));
            
        }

    }
    
    /// <summary>
    /// Lists the attributes within the class or a message that there are no attributes
    /// </summary>
    /// <returns>A string containing all attributes of the class, separated by new lines.</returns>
    public string ListAttributes()
    {
        string msg = "";

        if (Attributes.Count == 0)
        {
            msg = "There are no attributes currently.";
        }
        else
        {
            msg += string.Format("{0} attributes: \n", ClassName);
            foreach (AttributeObject a in Attributes)
            {
                msg += string.Format("    {0}\n", a.ToString());
            }
        }

        return msg;
    }

    /// <summary>
    /// Renames class
    /// Pre-condition: name is a validly formatted class name
    /// </summary>
    /// <param name="name"></param>
    public void Rename(string name)
    {
        ClassName = name;
    }
}