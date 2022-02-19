namespace UMLEditor.Classes;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

public class Class
{
    [JsonProperty("classname")]
    public string ClassName { get; private set; }
    
    [JsonProperty("fields")]
    public List<Parameter> Fields;

    [JsonProperty("methods")]
    public List<Method> Methods;
    
    /*
    [JsonProperty("attributes")]
    public List<AttributeObject> Attributes { get; private set; }
    */

    /// <summary>
    /// Default constructor for class
    /// </summary>
    public Class()
    {
        ClassName = "";
        Fields = new List<Parameter>();
    }

    /// <summary>
    /// Constructor for class "name"
    /// </summary>
    /// <param name="name">Name of the class being created</param>
    /// <param name="withFields">(Optional) A list of fields to include in this class</param>
    /// <exception cref="InvalidNameException">Thrown if the name provided is invalid</exception>
    public Class(string name, params Parameter[] withFields) : this()
    {
        CheckValidClassName(name);
        ClassName = name;
        
        // TODO: Add an initializer for the methods
        // Copy over the provided fields
        Fields.AddRange(withFields);
    }

    /// <summary>
    /// Adds attribute to class.
    /// Pre-condition: name of attribute is valid
    /// </summary>
    /// <param name="name">Valid name of new attribute</param>
    public void AddAttribute(string name)
    {
        if (AttributeExists(name))
        {
            throw new AttributeAlreadyExistsException(string.Format("Attribute {0} already exists", name));
        }
        
        // Attributes.Add(new AttributeObject(name));
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
    public AttributeObject? GetAttributeByName(string name)
    {
        /*
        foreach (AttributeObject currentAttribute in Attributes)
        {
            if (currentAttribute.AttributeName == name)
            {
                return currentAttribute;
            }
        }
        */
        return null; 
    }
    
    /// <summary>
    /// Deletes an attribute within a specified class
    /// </summary>
    /// <param name="targetAttributeName">AttributeObject to be deleted</param>
    /// <exception cref="AttributeNonexistentException">If attribute does not exist</exception>
    public void DeleteAttribute(string targetAttributeName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent attribute name entered ({0}).";
        
        // Ensure the provided classes exist
        // bool removeWorked = Attributes.Remove(GetAttributeByName(targetAttributeName));
        bool removeWorked = false;
        if (!removeWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, targetAttributeName));
            
        }

    }
    
    /// <summary>
    /// Lists the attributes within the class or a message that there are no attributes
    /// </summary>
    /// <returns>A string containing all attributes of the class, separated by new lines.</returns>
    public string ListAttributes()
    {
        string msg = "";

        /*
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
        */

        return msg;
    }
    
    /// <summary>
    /// Renames an attribute
    /// </summary>
    /// <param name="oldName">Attribute to rename</param>
    /// <param name="newName">New name of attribute</param>
    /// <exception cref="AttributeNonexistentException">Thrown if oldName attribute does not exist</exception>
    /// <exception cref="AttributeAlreadyExistsException">Thrown if newName attribute already exists</exception>
    public void RenameAttribute(string oldName, string newName)
    {
        if (!AttributeExists(oldName))
        {
            throw new AttributeNonexistentException(string.Format("Attribute {0} does not exist", oldName));
        }
        if (AttributeExists(newName))
        {
            throw new AttributeAlreadyExistsException(string.Format("Attribute {0} already exists", newName));
        }
        
        // Rename attribute
        GetAttributeByName(oldName).AttRename(newName);
        
    }

    /// <summary>
    /// Checks if a given class name is valid.  Throws an exception if not
    /// </summary>
    /// <param name="name">Name that is checked for validity</param>
    /// <exception cref="InvalidNameException">Thrown if the name is not valid</exception>
    private void CheckValidClassName(string name)
    {
        if (!Char.IsLetter(name[0]) && name[0] != '_')
        {
            throw new InvalidNameException(String.Format("{0} is an invalid class name.  " +
                                                         "Class name must be a single word that starts with an alphabetic " +
                                                         "character or an underscore.  " +
                                                         "Please Try again.", name));
        }
    }

    /// <summary>
    /// Renames class.  Checks to ensure name is valid
    /// Pre-condition: Class "name" does not already exist
    /// </summary>
    /// <param name="name">name to rename class to</param>
    public void Rename(string name)
    {
        CheckValidClassName(name);
        ClassName = name;
    }
}