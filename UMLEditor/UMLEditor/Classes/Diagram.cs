using System;
using System.Reactive.Concurrency;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Diagram
{
    
    public List<Class> Classes { get; private set; }
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

}