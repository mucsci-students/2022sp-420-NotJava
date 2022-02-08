using System;
using System.Net.Security;
using DynamicData.Kernel;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Diagram
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("classes")]
    public List<Class> Classes { get; private set; }
    
    [JsonProperty("relationships")]
    public List<Relationship> Relationships { get; private set; }

    /// <summary>
    /// Default constructor for a new Diagram.
    /// </summary>
    public Diagram()
    {
        Classes = new List<Class>();
        Relationships = new List<Relationship>();
    }

    /// <summary>
    /// Check if specified class exists.
    /// </summary>
    /// <param name="name">Name of the class you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    public bool ClassExists (string name)
    {

        return GetClassByName(name) != null;

    }

    /// <summary>
    /// Finds the class with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of class you are looking for</param>
    /// <returns>Returns the class if exists, or null if it does not</returns>
    public Class GetClassByName(string name)
    {
        foreach (Class CurrentClass in Classes)
        {
            if (CurrentClass.ClassName == name)
            {
                return CurrentClass;
            }
        }
        return null; 
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="SourceName"></param>
    /// <param name="DestName"></param>
    /// <returns></returns>
    public Relationship GetRelationshipByName(string SourceName, string DestName)
    {
        foreach (Relationship r in Relationships)
        {
            if (r.SourceClass == SourceName && r.DestinationClass == DestName)
            {
                return r;
            }
        }

        return null;
    }
    
    /// <summary>
    /// Creates a relationship between the two classes, if they exist
    /// </summary>
    /// <param name="SourceClassName">The source class for the relationship</param>
    /// <param name="DestClassName">The destination class for the relationship</param>
    /// <exception cref="ClassNonexistentException">If either class does not exist</exception>
    public void AddRelationship(string SourceClassName, string DestClassName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent class name entered ({0}).";
        // Ensure the provided classes exist
        if (!ClassExists(SourceClassName))
        {

            throw new ClassNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, SourceClassName));
            
        }
        
        else if (!(ClassExists(DestClassName)))
        {

            throw new ClassNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, DestClassName));

        }

        // Create and add the new relationship
        Relationship newRel = new Relationship(SourceClassName, DestClassName);
        Relationships.Add(newRel);
        
    }

    /// <summary>
    /// Adds a class to the diagram.  Ensures the desired class to add does not already exist
    /// </summary>
    /// <param name="ClassName">The name of the class to add</param>
    /// <exception cref="ClassAlreadyExistsException">Ensures there is not already a class by this name</exception>
    public void AddClass(string ClassName)
    {
        if (ClassExists(ClassName))
        {
            throw new ClassAlreadyExistsException(string.Format("Class {0} already exists", ClassName));
        }
        
        // Create a new class
        Classes.Add(new Class(ClassName));

    }
    
    public void DeleteClass(string ClassName)
    {
        if (!ClassExists(ClassName))
        {
            throw new ClassNonexistentException(string.Format("Class {0} does not exist", ClassName));
        }
        
        Classes.Remove(GetClassByName(ClassName));
    }
    
    /// <summary>
    /// Renames a class oldName to newName
    /// </summary>
    /// <param name="oldName">The name of the class to rename</param>
    /// <param name="newName">The new name of the class</param>
    /// <exception cref="ClassNonexistentException">Thrown if the class oldName does not exist</exception>
    /// <exception cref="ClassAlreadyExistsException">Thrown if the class newName already exists</exception>
    public void RenameClass(string oldName, string newName)
    {
        if (!ClassExists(oldName))
        {
            throw new ClassNonexistentException(string.Format("Class {0} does not exist", oldName));
        }
        if (ClassExists(newName))
        {
            throw new ClassAlreadyExistsException(string.Format("Class {0} already exists", newName));
        }
        
        // Rename class
        GetClassByName(oldName).Rename(newName);

    }
    
    
    /// <summary>
    /// List all classes of the current diagram, or a message that there are no classes.
    /// </summary>
    /// <returns>A string containing all classes of the given diagram, separated by new lines.</returns>
    public string ListClasses()
    {
        string msg = "";
        if (Classes.Count == 0)
        {
            msg = "There are no classes currently.";
        }
        else
        {
            foreach (Class c in Classes)
            {
                msg += string.Format("{0}\n", c.ClassName);
            }
        }

        return msg;
    }
    
    /// <summary>
    /// List all relationships in the current diagram, or a message that there are no relationships.
    /// </summary>
    /// <returns>A string containing all relationships of the given diagram, separated by new lines.</returns>
    public string ListRelationships()
    {
        Console.Write("In method");
        string msg = "";
        if (Relationships.Count == 0)
        {
            msg = "There are no relationships currently.";
        }
        else
        {
            foreach (Relationship r in Relationships)
            {
                msg += string.Format("{0}\n", r.ToString());
            }
        }

        return msg;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="SourceName"></param>
    /// <param name="DestName"></param>
    /// <exception cref="RelationshipNonexistentException"></exception>
    public void DeleteRelationship(string SourceName, string DestName)
    {
        if (!RelationshipExists(SourceName, DestName))
        {
            throw new RelationshipNonexistentException(string.Format("Relationship {0} -> {1} does not exist", SourceName, DestName));
        }
        
        // Delete relationship
        Relationships.Remove(GetRelationshipByName(SourceName, DestName));
    }
}
