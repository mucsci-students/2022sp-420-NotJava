namespace UMLEditor.Classes;

using UMLEditor.Utility;
using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

public class Diagram
{
    
    [JsonProperty("classes", Required = Required.Always)]
    private List<Class> _classes;

    [JsonProperty("relationships", Required = Required.Always)]
    private List<Relationship> _relationships;

    // Public accessor for Classes
    // Creates copies to ensure data integrity
    [JsonIgnore]
    public List<Class> Classes
    {
        get => Utilities.CloneContainer(_classes);
    }

    // Public accessor for Relationships
    // Creates copies to ensure data integrity
    [JsonIgnore]
    public List<Relationship> Relationships
    {
        get => Utilities.CloneContainer(_relationships);
    }

    /// <summary>
    /// Default constructor for a new Diagram.
    /// </summary>
    public Diagram()
    {
        _classes = new List<Class>();
        _relationships = new List<Relationship>();
    }

    /// <summary>
    /// Check if specified class exists.
    /// </summary>
    /// <param name="name">Name of the class you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    private bool ClassExists (string name)
    {

        return GetClassByName(name) is not null;

    }

    /// <summary>
    /// Check if class is currently used in a relationship.
    /// </summary>
    /// <param name="name">Name of the class you are checking</param>
    /// <returns>Returns true if class is in a relationship in the diagram, false if not.</returns>
    private bool IsClassInRelationship(string name)
    {
        foreach (Relationship r in _relationships)
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
    /// <param name="type">The type of the relationship</param>
    /// <returns>True if the relationship exists, false otherwise</returns>
    private bool RelationshipExists (string sourceName, string destName, string type)
    {

        return GetRelationship(sourceName, destName, type) is not null;

    }
    
    /// <summary>
    /// Gets the relationship between the two classes, if it exists.
    /// </summary>
    /// <param name="sourceName">The source class in the relationship</param>
    /// <param name="destName">The destination class in the relationship</param>
    /// <param name="type">The type of the relationship</param>
    /// <returns>The found relationship object, or null if none exists</returns>
    private Relationship? GetRelationship(string sourceName, string destName, string type)
    {
        foreach (Relationship r in _relationships)
        {
            if (r.SourceClass == sourceName && r.DestinationClass == destName && r.RelationshipType == type)
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
        
        // TODO: GetClassByName should be made private
        
        foreach (Class currentClass in _classes)
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
        
        // Ensure the provided classes exist
        if (!ClassExists(sourceClassName))
        {

            throw new ClassNonexistentException($"Nonexistent class name entered ({sourceClassName}).");
            
        }
        
        else if (!(ClassExists(destClassName)))
        {

            throw new ClassNonexistentException($"Nonexistent class name entered ({destClassName}).");

        }

        // Create and add the new relationship
        Relationship newRel = new Relationship(sourceClassName, destClassName, relationshipType);
        _relationships.Add(newRel);
        
    }

    /// <summary>
    /// Adds a method to a class
    /// </summary>
    /// <param name="toClass">Class to add method to</param>
    /// <param name="returnType">Return type of method</param>
    /// <param name="methodName">Name of method</param>
    public void AddMethod(string toClass, string returnType, string methodName)
    {
        if (!ClassExists(toClass))
        {
            throw new ClassNonexistentException($"Class {toClass} does not exist");
        }
        GetClassByName(toClass).AddMethod(returnType, methodName);
    }
    
    /// <summary>
    /// Adds a method to a class
    /// </summary>
    /// <param name="toClass">Class to add method to</param>
    /// <param name="returnType">Return type of method</param>
    /// <param name="methodName">Name of method</param>
    /// <param name="paramList">A list of parameters</param>
    public void AddMethod(string toClass, string returnType, string methodName, List<NameTypeObject> paramList)
    {
        
        if (!ClassExists(toClass))
        {
            throw new ClassNonexistentException($"Class {toClass} does not exist");
        }
        
        GetClassByName(toClass).AddMethod(returnType, methodName, paramList);
        
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
            throw new ClassAlreadyExistsException($"Class {className} already exists");
        }
        
        // Create a new class
        _classes.Add(new Class(className));

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
        
        _classes.Remove(GetClassByName(className));
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
            throw new ClassNonexistentException($"Class {oldName} does not exist");
        }
        if (ClassExists(newName))
        {
            throw new ClassAlreadyExistsException($"Class {newName} already exists");
        }
        
        // Rename class
        Class? foundClass = GetClassByName(oldName); 
        foundClass!.Rename(newName);

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
        if (_classes.Count == 0)
        {
            msg = "There are no classes currently.";
        }
        else
        {
            foreach (Class c in _classes)
            {
                msg += $"{c.ClassName}\n";
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
        if (_relationships.Count == 0)
        {
            msg = "There are no relationships currently.";
        }
        else
        {
            foreach (Relationship r in _relationships)
            {
                msg += $"{r}\n";
            }
        }

        return msg;
    }
    
    /// <summary>
    /// Deletes the provided relationship, if it exists
    /// </summary>
    /// <param name="sourceName">Source class in the relationship</param>
    /// <param name="destName">Destination class in the relationship</param>
    /// <param name="type">Type of the relationship</param>
    /// <exception cref="RelationshipNonexistentException">If the relationship does not exist</exception>
    public void DeleteRelationship(string sourceName, string destName, string type)
    {
        if (!RelationshipExists(sourceName, destName, type))
        {
            throw new RelationshipNonexistentException($"Relationship {type} {sourceName} -> {destName} does not exist");
        }
        
        // Delete relationship
        _relationships.Remove(GetRelationship(sourceName, destName, type)!);
    }

    /// <summary>
    /// Returns a list of all relationships the provided class is involved with
    /// </summary>
    /// <param name="onClassName">The class to find relationships for</param>
    /// <returns>A list containing all relationships the provided class is involved with</returns>
    private List<Relationship> GetInvolvedRelationships(string onClassName)
    {

        List<Relationship> result = new List<Relationship>();

        foreach (Relationship currentRel in _relationships)
        {

            if (currentRel.SourceClass == onClassName || currentRel.DestinationClass == onClassName)
            {
                result.Add(currentRel);
            }
            
        }
        
        return result;

    }

    /// <summary>
    /// Adds a field to a class in the diagram
    /// </summary>
    /// <param name="toClass">The target class to add the field to</param>
    /// <param name="withType">The type of the field to add</param>
    /// <param name="withName">The name of the field to add</param>
    /// <exception cref="ClassNonexistentException">If the provided class does not exist</exception>
    public void AddField(string toClass, string withType, string withName)
    {

        if (!ClassExists(toClass))
        {
            throw new ClassNonexistentException($"Class '{toClass}' does not exist");
        }

        Class targetClass = GetClassByName(toClass)!;
        targetClass!.AddField(withType, withName);

    }

    /// <summary>
    /// Deletes a field from a class in the diagram
    /// </summary>
    /// <param name="fromClass">The target class to remove the field from</param>
    /// <param name="withName">The name of the field to remove</param>
    /// <exception cref="ClassNonexistentException">If the provided class does not exist</exception>
    public void RemoveField(string fromClass, string withName)
    {

        if (!ClassExists(fromClass))
        {
            throw new ClassNonexistentException($"Class '{fromClass}' does not exist");
        }

        Class targetClass = GetClassByName(fromClass)!;
        targetClass!.DeleteField(withName);

    }

    /// <summary>
    /// Deletes a parameter from a provided method on a provided class
    /// </summary>
    /// <param name="paramName">The parameter to delete</param>
    /// <param name="inMethod">The method to delete from</param>
    /// <param name="onClass">The class to delete from</param>
    public void RemoveParameter(NameTypeObject param, string inMethod, string onClass)
    {

        if (!ClassExists(onClass))
        {
            throw new ClassNonexistentException($"Class '{onClass}' does not exist"); 
        }

        Class targetClass = GetClassByName(onClass)!;
        targetClass!.DeleteMethodParameter(param, inMethod);

    }
    
}