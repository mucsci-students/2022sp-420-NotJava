namespace UMLEditor.Classes;

using System.Collections.Generic;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

public class Diagram
{
    [JsonProperty("classes", Required = Required.Always)]
    public List<Class> Classes { get; private set; }
    
    [JsonProperty("relationships",  Required = Required.Always)]
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
    /// Check if class is currently used in a relationship.
    /// </summary>
    /// <param name="name">Name of the class you are checking</param>
    /// <returns>Returns true if class is in a relationship in the diagram, false if not.</returns>
    public bool IsClassInRelationship(string name)
    {
        foreach (Relationship r in Relationships)
        {
            if (r.SourceClass == name || r.DestinationClass == name)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a relationship between the two classes.
    /// </summary>
    /// <param name="sourceName">The source class in the relationship</param>
    /// <param name="destName">The destination class in the relationship</param>
    /// <returns>True if the relationship exists, false otherwise</returns>
    public bool RelationshipExists (string sourceName, string destName)
    {

        return GetRelationshipByName(sourceName, destName) != null;

    }
    
    /// <summary>
    /// Gets the relationship between the two classes, if it exists.
    /// </summary>
    /// <param name="sourceName">The source class in the relationship</param>
    /// <param name="destName">The destination class in the relationship</param>
    /// <returns>The found relationship object, or null if none exists</returns>
    public Relationship? GetRelationshipByName(string sourceName, string destName)
    {
        foreach (Relationship r in Relationships)
        {
            if (r.SourceClass == sourceName && r.DestinationClass == destName)
            {
                return r;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Finds the class with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of class you are looking for</param>
    /// <returns>Returns the class if exists, or null if it does not</returns>
    public Class? GetClassByName(string name)
    {
        foreach (Class currentClass in Classes)
        {
            if (currentClass.ClassName == name)
            {
                return currentClass;
            }
        }
        return null; 
    }

    /// <summary>
    /// Creates a relationship between the two classes, if they exist
    /// </summary>
    /// <param name="sourceClassName">The source class for the relationship</param>
    /// <param name="destClassName">The destination class for the relationship</param>
    /// <param name="relationshipType">The type of relationship</param>
    /// <exception cref="ClassNonexistentException">If either class does not exist</exception>
    /// <exception cfef="InvalidRelationshipTypeException">If the given relationship type is not valid</exception>
    public void AddRelationship(string sourceClassName, string destClassName, string relationshipType)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent class name entered ({0}).";
        // Ensure the provided classes exist
        if (!ClassExists(sourceClassName))
        {

            throw new ClassNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, sourceClassName));
            
        }
        
        else if (!(ClassExists(destClassName)))
        {

            throw new ClassNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, destClassName));

        }

        // Create and add the new relationship
        Relationship newRel = new Relationship(sourceClassName, destClassName, relationshipType);
        Relationships.Add(newRel);
        
    }

    /// <summary>
    /// Adds a class to the diagram.  Ensures the desired class to add does not already exist
    /// </summary>
    /// <param name="className">The name of the class to add</param>
    /// <exception cref="ClassAlreadyExistsException">Ensures there is not already a class by this name</exception>
    public void AddClass(string className)
    {
        if (ClassExists(className))
        {
            throw new ClassAlreadyExistsException(string.Format("Class {0} already exists", className));
        }
        
        // Create a new class
        Classes.Add(new Class(className));

    }
    
    /// <summary>
    /// Deletes the class that has the provided name
    /// </summary>
    /// <param name="className">The name of the class to delete</param>
    /// <exception cref="ClassNonexistentException">If the provided class does not exist</exception>
    /// <exception cref="ClassInUseException">If the class is currently involved in a relationship</exception>
    public void DeleteClass(string className)
    {
        if (!ClassExists(className))
        {
            throw new ClassNonexistentException(string.Format("Class {0} does not exist", className));
        }

        if (IsClassInRelationship(className))
        {
            throw new ClassInUseException(string.Format("Class {0} is in use by a relationship and cannot be deleted",
                className));
        }
        
        Classes.Remove(GetClassByName(className));
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
        Class foundClass = GetClassByName(oldName); 
        foundClass.Rename(newName);

        foreach (Relationship currentRel in GetInvolvedRelationships(oldName))
        {
            currentRel.RenameMember(oldName, newName);
        }
        
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
    /// Deletes the provided relationship, if it exists
    /// </summary>
    /// <param name="sourceName">Source class in the relationship</param>
    /// <param name="destName">Destination class in the relationship</param>
    /// <exception cref="RelationshipNonexistentException">If the relationship does not exist</exception>
    public void DeleteRelationship(string sourceName, string destName)
    {
        if (!RelationshipExists(sourceName, destName))
        {
            throw new RelationshipNonexistentException(string.Format("Relationship {0} -> {1} does not exist", sourceName, destName));
        }
        
        // Delete relationship
        Relationships.Remove(GetRelationshipByName(sourceName, destName));
    }

    /// <summary>
    /// Returns a list of all relationships the provided class is involved with
    /// </summary>
    /// <param name="onClassName">The class to find relationships for</param>
    /// <returns>A list containing all relationships the provided class is involved with</returns>
    public List<Relationship> GetInvolvedRelationships(string onClassName)
    {

        List<Relationship> result = new List<Relationship>();

        foreach (Relationship currentRel in Relationships)
        {

            if (currentRel.SourceClass == onClassName || currentRel.DestinationClass == onClassName)
            {
                result.Add(currentRel);
            }
            
        }
        
        return result;

    }
    
}