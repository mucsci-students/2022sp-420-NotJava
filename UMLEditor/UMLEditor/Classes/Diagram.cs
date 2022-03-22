using System;

namespace UMLEditor.Classes;

using Utility;
using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

/// <summary>
/// Diagram Class
/// </summary>
public class Diagram : ICloneable
{
    
    [JsonProperty("classes", Required = Required.Always)]
    private List<Class> _classes;

    [JsonProperty("relationships", Required = Required.Always)]
    private List<Relationship> _relationships;
    
    /// <summary>
    /// Public accessor for Classes
    /// Creates copies to ensure data integrity
    /// </summary>
    [JsonIgnore]
    public List<Class> Classes
    {
        get => Utilities.CloneContainer(_classes);
    }
    
    /// <summary>
    /// Public accessor for Relationships
    /// Creates copies to ensure data integrity
    /// </summary>
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
    /// Copy constructor for a Diagram
    /// </summary>
    /// <param name="d">diagram to copy</param>
    public Diagram(Diagram d) : this()
    {
        foreach (var item in d._classes)
        {
            _classes.Add((Class) item.Clone());
        }
        
        foreach (var item in d._relationships)
        {
            _relationships.Add((Relationship) item.Clone());
        }
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
    /// Checks if the current diagram has the given class
    /// </summary>
    /// <param name="className">The name of the class to check</param>
    /// <exception cref="ClassNonexistentException">Exception if class does not exist</exception>
    private void MustHaveClass(string className)
    {
        if (!ClassExists(className))
        {
            throw new ClassNonexistentException($"Class '{className}' does not exist");
        }
    }
    
    /// <summary>
    /// Checks if the current diagram has the given class
    /// </summary>
    /// <param name="className">The name of the class to check</param>
    /// <exception cref="ClassAlreadyExistsException">Exception if class already exists</exception>
    private void MustNotHaveClass(string className)
    {
        if (ClassExists(className))
        {
            throw new ClassAlreadyExistsException($"Class '{className}' already exists");
        }
    }
    
    /// <summary>
    /// Checks if the current diagram has a relationship with the given source / destination
    /// </summary>
    /// <param name="sourceClassName">Source class for the relationship</param>
    /// <param name="destClassName">Destination class for the relationship</param>
    /// <exception cref="RelationshipNonexistentException">Exception if relationship does not exist</exception>
    private void MustHaveRelationship(string sourceClassName, string destClassName)
    {
        if (!RelationshipExists(sourceClassName, destClassName))
        {
            throw new RelationshipNonexistentException($"Relationship {sourceClassName} => {destClassName} does not exist.");
        }
    }
    
    /// <summary>
    /// Checks if the current diagram has a relationship with the given source / destination
    /// </summary>
    /// <param name="sourceClassName">Source class for the relationship</param>
    /// <param name="destClassName">Destination class for the relationship</param>
    /// <exception cref="RelationshipAlreadyExistsException">Exception if relationship already exists</exception>
    private void MustNotHaveRelationship(string sourceClassName, string destClassName)
    {
        if (RelationshipExists(sourceClassName, destClassName))
        {
            throw new RelationshipAlreadyExistsException($"Relationship {sourceClassName} => {destClassName} already exists.");
        }
    }

    /// <summary>
    /// Checks if a relationship between the two classes.
    /// </summary>
    /// <param name="sourceName">The source class in the relationship</param>
    /// <param name="destName">The destination class in the relationship</param>
    /// <returns>True if the relationship exists, false otherwise</returns>
    private bool RelationshipExists (string sourceName, string destName)
    {

        return GetRelationship(sourceName, destName) is not null;

    }
    
    /// <summary>
    /// Gets the relationship between the two classes, if it exists.
    /// </summary>
    /// <param name="sourceName">The source class in the relationship</param>
    /// <param name="destName">The destination class in the relationship</param>
    /// <returns>The found relationship object, or null if none exists</returns>
    private Relationship? GetRelationship(string sourceName, string destName)
    {
        foreach (Relationship r in _relationships)
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

        MustHaveClass(sourceClassName);
        MustHaveClass(destClassName);
        
        // Ensure the provided relationship does not exist
        MustNotHaveRelationship(sourceClassName, destClassName);
        
        // Create and add the new relationship
        Relationship newRel = new Relationship(sourceClassName, destClassName, relationshipType);
        _relationships.Add(newRel);
        
        NotifyChanged();
        
    }
    
    /// <summary>
    /// Changes type of existing relationship
    /// </summary>
    /// <param name="sourceClass">Name of source class</param>
    /// <param name="destClass">Name of destination class</param>
    /// <param name="newRelationshipType">Type to change relationship into</param>
    /// <exception cref="RelationshipNonexistentException">Thrown if the relationship already exists</exception>
    /// <exception cref="RelationshipTypeAlreadyExists">Thrown if the existing relationship is already of type newRelationshipType</exception>
    public void ChangeRelationship(string sourceClass, string destClass, string newRelationshipType)
    {
        //Check if relationship exists at all
        MustHaveRelationship(sourceClass,destClass);

        //Check if the relationship between the two classes is already of type newRelationshipType
        Relationship? r = GetRelationship(sourceClass, destClass);
        if (r!.RelationshipType == newRelationshipType)
        {
            throw new RelationshipTypeAlreadyExists($"Relationship '{sourceClass} => {destClass}' is already of type '{newRelationshipType}'.");
        }

        r.ChangeType(newRelationshipType);
        
        NotifyChanged();
    }

    /// <summary>
    /// Adds a method to a class
    /// </summary>
    /// <param name="toClass">Class to add method to</param>
    /// <param name="returnType">Return type of method</param>
    /// <param name="methodName">Name of method</param>
    public void AddMethod(string toClass, string returnType, string methodName)
    {
        MustHaveClass(toClass);
        GetClassByName(toClass)!.AddMethod(returnType, methodName);
        
        NotifyChanged();
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
        
        MustHaveClass(toClass);
        GetClassByName(toClass)!.AddMethod(returnType, methodName, paramList);
        
        NotifyChanged();
        
    }

    /// <summary>
    /// Adds a class to the diagram.  Ensures the desired class to add does not already exist
    /// </summary>
    /// <param name="className">The name of the class to add</param>
    /// <exception cref="ClassAlreadyExistsException">Ensures there is not already a class by this name</exception>
    public void AddClass(string className)
    {

        // Ensure class does not already exist
        MustNotHaveClass(className);
        
        // Create a new class
        _classes.Add(new Class(className));
        
        NotifyChanged();

    }
    
    /// <summary>
    /// Deletes the class that has the provided name
    /// </summary>
    /// <param name="className">The name of the class to delete</param>
    /// <exception cref="ClassNonexistentException">If the provided class does not exist</exception>
    /// <exception cref="ClassInUseException">If the class is currently involved in a relationship</exception>
    public void DeleteClass(string className)
    {
        MustHaveClass(className);

        if (IsClassInRelationship(className))
        {
            throw new ClassInUseException($"Class '{className}' is in use by a relationship and cannot be deleted");
        }
        
        _classes.Remove(GetClassByName(className)!);
        
        NotifyChanged();
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

        // Ensure old class exists
        MustHaveClass(oldName);
        
        // Ensure new class does not already exist
        MustNotHaveClass(newName);
        
        // Rename class
        Class? foundClass = GetClassByName(oldName); 
        foundClass!.Rename(newName);

        foreach (Relationship currentRel in GetInvolvedRelationships(oldName))
        {
            currentRel.RenameMember(oldName, newName);
        }
        
        NotifyChanged();
    }

    /// <summary>
    /// Renames existing method to newName
    /// </summary>
    /// <param name="onClass">Class that method is in</param>
    /// <param name="oldName">Method to rename</param>
    /// <param name="newName">New name of method</param>
    /// <exception cref="ClassNonexistentException">Thrown if the class does not exist</exception>
    public void RenameMethod(string onClass, string oldName, string newName)
    {

        // Ensure class exists
        MustHaveClass(onClass);
        
        GetClassByName(onClass)!.RenameMethod(oldName, newName);
        
        NotifyChanged();
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
    /// Lists attributes of a class
    /// </summary>
    /// <param name="onClass">Class to list attributes of</param>
    /// <returns></returns>
    /// <exception cref="ClassNonexistentException">Thrown if class does not exist</exception>
    public string ListAttributes(string onClass)
    {

        // Ensure class exists
        MustHaveClass(onClass);

        return GetClassByName(onClass)!.ListAttributes();
    }
    
    /// <summary>
    /// Deletes the provided relationship, if it exists
    /// </summary>
    /// <param name="sourceName">Source class in the relationship</param>
    /// <param name="destName">Destination class in the relationship</param>
    /// <exception cref="RelationshipNonexistentException">If the relationship does not exist</exception>
    public void DeleteRelationship(string sourceName, string destName)
    {

        // Ensure the relationship exists
        MustHaveRelationship(sourceName, destName);
        
        // Delete relationship
        _relationships.Remove(GetRelationship(sourceName, destName)!);
        
        NotifyChanged();
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
        // Ensure the class exists
        MustHaveClass(toClass);

        Class? targetClass = GetClassByName(toClass);
        targetClass!.AddField(withType, withName);
        
        NotifyChanged();

    }

    /// <summary>
    /// Deletes a field from a class in the diagram
    /// </summary>
    /// <param name="fromClass">The target class to remove the field from</param>
    /// <param name="withName">The name of the field to remove</param>
    /// <exception cref="ClassNonexistentException">If the provided class does not exist</exception>
    public void DeleteField(string fromClass, string withName)
    {
        // Ensures the class exists
        MustHaveClass(fromClass);

        Class? targetClass = GetClassByName(fromClass);
        targetClass!.DeleteField(withName);
        
        NotifyChanged();

    }

    /// <summary>
    /// Renames a field on a given class
    /// </summary>
    /// <param name="onClass">The class to rename the field on</param>
    /// <param name="oldName">The current (old) name of the field to be renamed</param>
    /// <param name="newName">The new name for the field</param>
    public void RenameField(string onClass, string oldName, string newName)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.RenameField(oldName, newName);
        
        NotifyChanged();

    }

    /// <summary>
    /// Change the type of an existing field
    /// </summary>
    /// <param name="onClass">The class to change the field on</param>
    /// <param name="fieldToChange">The field to change the type of</param>
    /// <param name="newType">The new type of the field</param>
    public void ChangeFieldType(string onClass, string fieldToChange, string newType)
    {
        // Ensures the class exists
        MustHaveClass(onClass);
        
        Class? targetClass = GetClassByName(onClass);
        targetClass!.ChangeFieldType(fieldToChange, newType);
        
        NotifyChanged();
    }
    
    /// <summary>
    /// Changes type of method
    /// </summary>
    /// <param name="onClass">Class method exists in</param>
    /// <param name="onMethod">Method to change type of</param>
    /// <param name="newType">New type of method</param>
    /// <exception cref="ClassNonexistentException">Thrown if class does not exist</exception>
    public void ChangeMethodType(string onClass, string onMethod, string newType)
    {
        // Ensures the class exists
        MustHaveClass(onClass);
        
        Class? targetClass = GetClassByName(onClass);
        targetClass!.ChangeMethodType(onMethod, newType);
        
        NotifyChanged();
    }

    /// <summary>
    /// Changes the anatomy of the provided field
    /// </summary>
    /// <param name="onClass">The class to change the field on</param>
    /// <param name="toRename">The field to rename</param>
    /// <param name="newField">The new field</param>
    public void ReplaceField(string onClass, NameTypeObject toRename, NameTypeObject newField)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.ReplaceField(toRename, newField);
        
        NotifyChanged();

    }

    /// <summary>
    /// Changes the anatomy of the provided parameter
    /// </summary>
    /// <param name="onClass">The class to change the parameter on</param>
    /// <param name="inMethod">The method that the parameter belongs to</param>
    /// <param name="toReplace">The parameter to replace</param>
    /// <param name="newParameter">The new parameter</param>
    public void ReplaceParameter(string onClass, string inMethod, string toReplace,
        NameTypeObject newParameter)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.ReplaceParameter(inMethod, toReplace, newParameter);
        
        NotifyChanged();

    }

    /// <summary>
    /// Clears parameters from a method
    /// </summary>
    /// <param name="onClass">Class that method exists in</param>
    /// <param name="inMethod">Method to clear parameters from</param>
    /// <exception cref="ClassNonexistentException">Thrown if class does not exist</exception>
    public void ClearParameters(string onClass, string inMethod)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        GetClassByName(onClass)!.ClearParameters(inMethod);
        
        NotifyChanged();
    }
    
    /// <summary>
    /// Deletes a parameter from a provided method on a provided class
    /// </summary>
    /// <param name="paramName">The parameter to delete</param>
    /// <param name="inMethod">The method to delete from</param>
    /// <param name="onClass">The class to delete from</param>
    public void DeleteParameter(string paramName, string inMethod, string onClass)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.DeleteMethodParameter(paramName, inMethod);
        
        NotifyChanged();

    }

    /// <summary>
    /// Renames a specific parameter on a provided method
    /// </summary>
    /// <param name="onClass">The class the method is within</param>
    /// <param name="onMethod">The name of the method the parameter is in</param>
    /// <param name="oldParamName">The current (old) name of the parameter</param>
    /// <param name="newParamName">The new name for the parameter</param>
    /// <exception cref="ClassNonexistentException"></exception>
    public void RenameParameter(string onClass, string onMethod, string oldParamName, string newParamName)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.RenameMethodParameter(onMethod, oldParamName, newParamName);
        
        NotifyChanged();

    }

    /// <summary>
    /// Adds parameter to an existing method
    /// </summary>
    /// <param name="className">Name of class method is in</param>
    /// <param name="methodName">Method to add parameter to</param>
    /// <param name="paramType">Return type of parameter</param>
    /// <param name="paramName">Name of parameter</param>
    /// <exception cref="ClassNonexistentException">Thrown if class does not exist</exception>
    public void AddParameter(string className, string methodName, string paramType, string paramName)
    {
        // Ensures the class exists
        MustHaveClass(className);
        GetClassByName(className)!.AddParameter(methodName, paramType, paramName);
        
        NotifyChanged();
    }

    /// <summary>
    /// Change the name and type of the provided method
    /// </summary>
    /// <param name="onClass">The class the method is on</param>
    /// <param name="methodName">The name of the method to change</param>
    /// <param name="newMethodAnatomy">The new name and type of the method</param>
    public void ChangeMethodNameType(string onClass, string methodName, NameTypeObject newMethodAnatomy)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.ChangeMethodNameType(methodName, newMethodAnatomy);
        
        NotifyChanged();
    }

    /// <summary>
    /// Adds a new parameter to the provided method
    /// </summary>
    /// <param name="inClass">The class the method is on</param>
    /// <param name="toMethod">The method to add a parameter to</param>
    /// <param name="parameter">The new parameter to add</param>
    public void AddParameter(string inClass, string toMethod, NameTypeObject parameter)
    {
        // Ensures the class exists
        MustHaveClass(inClass);

        Class? targetClass = GetClassByName(inClass);
        targetClass!.AddParameter(toMethod, parameter.Type, parameter.AttributeName);
        
        NotifyChanged();
    }
    
    /// <summary>
    /// Deletes the provided method on the provided class
    /// </summary>
    /// <param name="onClass">The class the method is on</param>
    /// <param name="methodName">The name of the method to delete</param>
    public void DeleteMethod(string onClass, string methodName)
    {
        // Ensures the class exists
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.DeleteMethod(methodName);
        
        NotifyChanged();
    }

    /// <summary>
    /// Changes the recorded location of the provided class
    /// </summary>
    /// <param name="onClass">The class to change</param>
    /// <param name="x">The new X</param>
    /// <param name="y">The new Y</param>
    public void ChangeBoxLocation(string onClass, double x, double y)
    {
        
        MustHaveClass(onClass);

        Class? targetClass = GetClassByName(onClass);
        targetClass!.ChangeLocation(x, y);
        
        NotifyChanged();

    }

    /// <summary>
    /// Clones a diagram
    /// </summary>
    /// <returns>A clone of the diagram</returns>
    public object Clone()
    {
        return new Diagram(this);
    }

    /// <summary>
    /// Diagram changed event
    /// </summary>
    public static event EventHandler? DiagramChanged;

    /// <summary>
    /// Invokes the DiagramChanged Event
    /// </summary>
    public void NotifyChanged()
    {
        DiagramChanged?.Invoke(this, EventArgs.Empty);
    }
}