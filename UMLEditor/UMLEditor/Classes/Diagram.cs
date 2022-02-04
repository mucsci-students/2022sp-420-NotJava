using System;
using System.Reactive.Concurrency;
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

}