
namespace UMLEditor.Classes;

using System.Data;
using Utility;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

/// <summary>
/// 'Class' Class
/// </summary>
public class Class: ICloneable
{
    /// <summary>
    /// ClassName property
    /// </summary>
    [JsonProperty("name")]
    public string ClassName { get; private set; }
    
    [JsonProperty("fields")]
    private List<NameTypeObject> _fields;

    [JsonProperty("location")] 
    private BoxLocation _classLocation;

    /// <summary>
    /// Class Location property
    /// </summary>
    [JsonIgnore]
    public BoxLocation ClassLocation
    {
        get => (BoxLocation)_classLocation.Clone();
    }
    
    /// <summary>
    /// Public accessor for Fields
    /// Creates copies to ensure data integrity
    /// </summary>
    [JsonIgnore]
    public List<NameTypeObject> Fields
    {
        get => Utilities.CloneContainer(_fields);
    }

    [JsonProperty("methods")]
    private List<Method> _methods;
    
    /// <summary>
    /// Public accessor for Methods
    /// Creates copies to ensure data integrity
    /// </summary>
    [JsonIgnore]
    public List<Method> Methods
    {
        get => Utilities.CloneContainer(_methods);
    }

    /// <summary>
    /// Default constructor for class
    /// </summary>
    public Class()
    {
        
        ClassName = "";
        _fields = new List<NameTypeObject>();
        _methods = new List<Method>();
        _classLocation = new BoxLocation();

    }

    /// <summary>
    /// Constructor for class "name"
    /// </summary>
    /// <param name="name">Name of the class being created</param>
    /// <exception cref="InvalidNameException">Thrown if the name provided is invalid</exception>
    public Class(string name) : this()
    {
        CheckValidClassName(name);
        ClassName = name;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="c">Class object to copy</param>
    public Class(Class c) : this(c.ClassName)
    {
        _fields = Utilities.CloneContainer(c._fields);
        _methods = Utilities.CloneContainer(c._methods);
        _classLocation = (BoxLocation)c._classLocation.Clone();
    }

    /// <summary>
    /// Adds field to class.
    /// Pre-condition: name of field is valid
    /// </summary>
    /// <param name="type">Valid type of new field</param>
    /// <param name="name">Valid name of new field</param>
    public void AddField(string type, string name)
    {
        if (FieldExists(name))
        {
            throw new AttributeAlreadyExistsException($"Field '{name}' already exists");
        }
        _fields.Add(new NameTypeObject(type, name));
        
    }
    
    /// <summary>
    /// Adds method to class.
    /// Pre-condition: name of method is valid
    /// </summary>
    /// <param name="returnType">Valid returnType of new method</param>
    /// <param name="name">Valid name of new method</param>
    public void AddMethod(string returnType, string name)
    {
        
        if (MethodExists(name))
        {
            throw new AttributeAlreadyExistsException($"Method '{name}' already exists");
        }
        
        _methods.Add(new Method(returnType, name));
        
    }

    /// <summary>
    /// Adds method to class.
    /// Pre-condition: name of method is valid
    /// </summary>
    /// <param name="returnType">Valid returnType of new method</param>
    /// <param name="name">Valid name of new method</param>
    /// <param name="paramList">List of parameters</param>
    public void AddMethod(string returnType, string name, List<NameTypeObject> paramList)
    {
        
        if (MethodExists(name))
        {
            throw new AttributeAlreadyExistsException(($"Method '{name}' already exists"));
        }
        
        // Ensure there are no duplicate parameters in the param list
        List<string> seenNames = new List<string>();
        foreach (NameTypeObject param in paramList)
        {

            if (seenNames.Contains(param.AttributeName))
            {
                throw new DuplicateNameException($"Duplicate parameter: '{param}'");
            }
         
            seenNames.Add(param.AttributeName);
            
        }
        
        _methods.Add(new Method(returnType, name, paramList));
        
    }

    /// <summary>
    /// Check if specified Field exists.
    /// </summary>
    /// <param name="name">Name of the Field you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    public bool FieldExists (string name)
    {

        return GetFieldByName(name) is not null;

    }

    /// <summary>
    /// Checks if a specific field exists
    /// </summary>
    /// <param name="field">The field to check</param>
    /// <returns>True if the field exists, false otherwise</returns>
    public bool FieldExists(NameTypeObject field)
    {
        return GetField(field) is not null;
    }

    /// <summary>
    /// Gets a reference to the provided field, if it exists
    /// </summary>
    /// <param name="toFind">The field to search for</param>
    /// <returns>A reference to the provided field, or null if it does not exist</returns>
    private NameTypeObject? GetField(NameTypeObject toFind)
    {

        foreach (NameTypeObject current in _fields)
        {

            if (current == toFind)
            {
                return current;
            }

        }
        
        return null;

    }
    
    /// <summary>
    /// Finds the field with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of field you are looking for</param>
    /// <returns>Returns the field if exists, or null if it does not</returns>
    private NameTypeObject? GetFieldByName(string name)
    {
        
        foreach (NameTypeObject currentField in _fields)
        {
            if (currentField.AttributeName == name)
            {
                return currentField;
            }
        }
        
        return null; 
    }    
    
    /// <summary>
    /// Check if specified method exists.
    /// </summary>
    /// <param name="name">Name of the method you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    public bool MethodExists (string name)
    {
        return GetMethodByName(name) is not null;
    }

    /// <summary>
    /// Finds the method with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of method you are looking for</param>
    /// <returns>Returns the method if it exists, or null if it does not</returns>
    private Method? GetMethodByName(string name)
    {
        
        foreach (Method currentMethod in _methods)
        {
            if (currentMethod.AttributeName == name)
            {
                return currentMethod;
            }
        }

        return null;
    }

    /// <summary>
    /// Deletes a field within this class.
    /// </summary>
    /// <param name="targetFieldName">Field to be deleted</param>
    /// <exception cref="AttributeNonexistentException">Thrown if field does not exist</exception>
    public void DeleteField(string targetFieldName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent field name entered ('{0}').";
        
        // Ensure the provided classes exist
        bool removeWorked = _fields.Remove(GetFieldByName(targetFieldName)!);
        if (!removeWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, targetFieldName));
            
        }

    }
    
    /// <summary>
    /// Deletes a method within this class.
    /// </summary>
    /// <param name="targetMethodName">Method to be deleted</param>
    /// <exception cref="AttributeNonexistentException">Thrown if method does not exist</exception>
    public void DeleteMethod(string targetMethodName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent method name entered ('{0}').";
        
        // Ensure the provided classes exist
        bool removeWorked = _methods.Remove(GetMethodByName(targetMethodName)!);
        if (!removeWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, targetMethodName));
            
        }

    }

    /// <summary>
    /// Deletes a parameter on the provided method
    /// </summary>
    /// <param name="param">The parameter to remove</param>
    /// <param name="onMethod">The target method the parameter is a part of</param>
    /// <exception cref="AttributeNonexistentException">If the provided method does not exist</exception>
    public void DeleteMethodParameter(string param, string onMethod)
    {

        if (!MethodExists(onMethod))
        {
            throw new AttributeNonexistentException($"Method '{onMethod}' does not exist");
        }

        Method targetMtd = GetMethodByName(onMethod)!;
        targetMtd.RemoveParam(param);

    }
    
    /// <summary>
    /// Lists the attributes within the class or a message that there are no attributes
    /// </summary>
    /// <returns>A string containing all attributes of the class, separated by new lines.</returns>
    public string ListAttributes()
    {
        return ListFields() + ListMethods();
    }
    
    /// <summary>
    /// Lists the fields within the class or a message that there are none.
    /// </summary>
    /// <returns>A string containing all fields of the class, separated by new lines.</returns>
    public string ListFields()
    {
        
        string msg = $"{ClassName} fields: \n";
        
        if (_fields.Count == 0)
        {
            msg += "    There are no fields currently. \n";
        }
        else
        {
            foreach (NameTypeObject a in _fields)
            {
                msg += $"    {a}\n";
            }
        }

        return msg;
    }
    
    /// <summary>
    /// Lists the methods within the class or a message that there are none.
    /// </summary>
    /// <returns>A string containing all methods of the class, separated by new lines.</returns>
    public string ListMethods()
    {
        
        string msg = $"{ClassName} methods: \n";
        
        if (_methods.Count == 0)
        {
            msg += "    There are no methods currently. \n";
        }
        else
        {
            foreach (Method a in _methods)
            {
                msg += $"    {a}\n";
            }
        }

        return msg;
    }

    /// <summary>
    /// Renames a field
    /// </summary>
    /// <param name="oldName">Field to rename</param>
    /// <param name="newName">New name of field</param>
    /// <exception cref="AttributeNonexistentException">Thrown if oldName field does not exist</exception>
    /// <exception cref="AttributeAlreadyExistsException">Thrown if newName field already exists</exception>
    /// <exception cref="InvalidNameException">Thrown if the provided name is invalid.</exception>
    public void RenameField(string oldName, string newName)
    {
        
        if (!FieldExists(oldName))
        {
            throw new AttributeNonexistentException($"Field '{oldName}' does not exist");
        }
        if (FieldExists(newName))
        {
            throw new AttributeAlreadyExistsException($"Field '{newName}' already exists");
        }
        
        // Rename field
        GetFieldByName(oldName)!.AttRename(newName);
        
    }
    
    /// <summary>
    /// Renames a method
    /// </summary>
    /// <param name="oldName">Method to rename</param>
    /// <param name="newName">New name of method</param>
    /// <exception cref="AttributeNonexistentException">Thrown if oldName method does not exist</exception>
    /// <exception cref="AttributeAlreadyExistsException">Thrown if newName method already exists</exception>
    /// <exception cref="InvalidNameException">Thrown if the provided name is invalid.</exception>
    public void RenameMethod(string oldName, string newName)
    {
        
        if (!MethodExists(oldName))
        {
            throw new AttributeNonexistentException($"Method '{oldName}' does not exist");
        }
        if (MethodExists(newName))
        {
            throw new AttributeAlreadyExistsException($"Method '{newName}' already exists");
        }
        
        // Rename field
        GetMethodByName(oldName)!.AttRename(newName);
        
    }

    /// <summary>
    /// Renames a parameter on the provided method
    /// </summary>
    /// <param name="onMethod">The method the parameter is on</param>
    /// <param name="oldParamName">The current (old) name of the parameter</param>
    /// <param name="newParamName">The new name for the parameter</param>
    public void RenameMethodParameter(string onMethod, string oldParamName, string newParamName)
    {

        if (!MethodExists(onMethod))
        {
            throw new AttributeNonexistentException($"Method '{onMethod}' does not exist");
        }

        Method? targetMtd = GetMethodByName(onMethod);
        targetMtd!.RenameParam(oldParamName, newParamName);

    }
    
    /// <summary>
    /// Checks if a given class name is valid.  Throws an exception if not
    /// </summary>
    /// <param name="name">Name that is checked for validity</param>
    /// <exception cref="InvalidNameException">Thrown if the name is not valid</exception>
    private void CheckValidClassName(string name)
    {
        if (name is null || name.Length == 0 || !Char.IsLetter(name[0]) && name[0] != '_' || name.Contains(" "))
        {
            throw new InvalidNameException($"'{name}' is an invalid class name.  " +
                                                                "Class name must be a single word that starts with an alphabetic " +
                                                                "character or an underscore.  " +
                                                                "Please Try again.");
        }
    }

    /// <summary>
    /// Changes the type of an existing field
    /// </summary>
    /// <param name="fieldName">Field to change type of </param>
    /// <param name="newType">New type of field</param>
    /// <exception cref="AttributeNonexistentException">Thrown if the field does not exist</exception>
    public void ChangeFieldType(string fieldName, string newType)
    {
        if (!FieldExists(fieldName))
        {
            throw new AttributeNonexistentException($"Field '{fieldName}' does not exist");
        }

        NameTypeObject? fieldToChange = GetFieldByName(fieldName);
        ReplaceField(fieldToChange!, new NameTypeObject(newType, fieldName));
    }
    
    /// <summary>
    /// Changes the type of a method
    /// </summary>
    /// <param name="methodName">Method to change the type of</param>
    /// <param name="newType">New type for method</param>
    /// <exception cref="AttributeNonexistentException">Thrown if the method does not exist</exception>
    public void ChangeMethodType(string methodName, string newType)
    {
        if (!MethodExists(methodName))
        {
            throw new AttributeNonexistentException($"Method '{methodName}' does not exist");
        }

        Method? methodToChange = GetMethodByName(methodName);
        methodToChange!.ChangeMethodType(newType);
    }
    
    /// <summary>
    /// Overwrites a field with a new one.
    /// </summary>
    /// <param name="toReplace">The field to replace</param>
    /// <param name="replaceWith">The new anatomy of the field</param>
    public void ReplaceField(NameTypeObject toReplace, NameTypeObject replaceWith)
    {

        // Check that a new name is not a duplicate
        bool nameWasChanged = toReplace.AttributeName != replaceWith.AttributeName;
        if (nameWasChanged && FieldExists(replaceWith.AttributeName))
        {
            throw new AttributeAlreadyExistsException($"A field by the name of '{replaceWith.AttributeName}' already exists");
        }

        // Check that the target field exists
        if (!FieldExists(toReplace))
        {
            throw new AttributeNonexistentException($"Field '{toReplace}' does not exist");
        }

        /* - Get a reference to the Field to be changed (which is a NameTypeObject)
         * - Apply the new name and type to the Field */
        NameTypeObject? target = GetField(toReplace);
        target!.AttRename(replaceWith.AttributeName);
        target.ChangeType(replaceWith.Type);

    }

    /// <summary>
    /// Replaces the provided parameter on the provided method
    /// </summary>
    /// <param name="onMethod">The method the parameter is a part of</param>
    /// <param name="toReplace">The parameter to replace</param>
    /// <param name="replaceWith">The new anatomy of the parameter</param>
    public void ReplaceParameter(string onMethod, string toReplace, NameTypeObject replaceWith)
    {

        if (!MethodExists(onMethod))
        {
            throw new AttributeNonexistentException($"Method '{onMethod}' does not exist");
        }

        Method? targetMethod = GetMethodByName(onMethod);
        targetMethod!.ReplaceParam(toReplace, replaceWith);
        
    }

    /// <summary>
    /// Clears all parameters from a method
    /// </summary>
    /// <param name="onMethod">Method to clear parameters from</param>
    /// <exception cref="AttributeNonexistentException"></exception>
    public void ClearParameters(string onMethod)
    {
        if (!MethodExists(onMethod))
        {
            throw new AttributeNonexistentException($"Method '{onMethod}' does not exist");
        }
        
        GetMethodByName(onMethod)!.ClearParameters();

    }
    
    /// <summary>
    /// Adds parameter to a method
    /// </summary>
    /// <param name="methodName">Method to add parameter to</param>
    /// <param name="paramType">Type of parameter to add</param>
    /// <param name="paramName">Name of parameter to add</param>
    /// <exception cref="MethodNonexistentRelationship">Thrown if the method does not exist</exception>
    public void AddParameter(string methodName, string paramType, string paramName)
    {
        if (!MethodExists(methodName))
        {
            throw new AttributeNonexistentException($"Method '{methodName}' does not exist");
        }
        GetMethodByName(methodName)!.AddParam(new NameTypeObject(paramType, paramName));
    }
    
    /// <summary>
    /// Changes the name and type of a method within this class.  This is primarily used by the GUI
    /// </summary>
    /// <param name="onMethod">The method to modify</param>
    /// <param name="newMethodAnatomy">The new name and return type of the method</param>
    /// <exception cref="AttributeNonexistentException">If the provided method does not exist</exception>
    public void ChangeMethodNameType(string onMethod, NameTypeObject newMethodAnatomy)
    {
        
        if (!MethodExists(onMethod))
        {
            throw new AttributeNonexistentException($"Method '{onMethod}' does not exist");
        }

        // If the name changed, check that it isn't a duplicate
        if (onMethod != newMethodAnatomy.AttributeName && MethodExists(newMethodAnatomy.AttributeName))
        {
            throw new AttributeAlreadyExistsException($"A method by the name '{newMethodAnatomy.AttributeName}' already exists");
        }
        
        Method? targetMethod = GetMethodByName(onMethod);
        targetMethod!.AttRename(newMethodAnatomy.AttributeName);
        targetMethod.ChangeReturnType(newMethodAnatomy.Type);
        
    }
    
    /// <summary>
    /// Renames class.  Checks to ensure name is valid
    /// Pre-condition: Class "name" does not already exist
    /// </summary>
    /// <param name="name">Name to rename class to</param>
    public void Rename(string name)
    {
        CheckValidClassName(name);
        ClassName = name;
    }

    /// <summary>
    /// Changes the stored location for this class
    /// </summary>
    /// <param name="x">The new x</param>
    /// <param name="y">The new y</param>
    public void ChangeLocation(double x, double y)
    {
        _classLocation.ChangeXY((int)x, (int)y);
    }
    
    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        return new Class(this);
    }
}
