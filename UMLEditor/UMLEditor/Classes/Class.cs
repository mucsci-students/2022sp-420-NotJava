namespace UMLEditor.Classes;

using UMLEditor.Utility;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

public class Class: ICloneable
{
    [JsonProperty("name")]
    public string ClassName { get; private set; }
    
    [JsonProperty("fields")]
    private List<NameTypeObject> _fields;
    
    // Public accessor for Fields
    // Creates copies to ensure data integrity
    public List<NameTypeObject> Fields
    {
        get => Utilities.CloneContainer(_fields);
    }

    [JsonProperty("methods")]
    private List<Method> _methods;
    
    // Public accessor for Methods
    // Creates copies to ensure data integrity
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
            throw new AttributeAlreadyExistsException(string.Format("Field {0} already exists", name));
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
            throw new AttributeAlreadyExistsException(string.Format("Method {0} already exists", name));
        }
        _methods.Add(new Method(returnType, name));
        
    }

    /// <summary>
    /// Check if specified Field exists.
    /// </summary>
    /// <param name="name">Name of the Field you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    private bool FieldExists (string name)
    {

        return GetFieldByName(name) is not null;

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
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent field name entered ({0}).";
        
        // Ensure the provided classes exist
        bool removeWorked = _fields.Remove(GetFieldByName(targetFieldName));
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
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent method name entered ({0}).";
        
        // Ensure the provided classes exist
        bool removeWorked = _methods.Remove(GetMethodByName(targetMethodName));
        if (!removeWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, targetMethodName));
            
        }

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
        
        string msg = string.Format("{0} fields: \n", ClassName);
        
        if (_fields.Count == 0)
        {
            msg += "    There are no fields currently. \n";
        }
        else
        {
            foreach (NameTypeObject a in _fields)
            {
                msg += string.Format("    {0}\n", a.ToString());
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
        
        string msg = string.Format("{0} methods: \n", ClassName);
        
        if (_methods.Count == 0)
        {
            msg += "    There are no methods currently. \n";
        }
        else
        {
            foreach (Method a in _methods)
            {
                msg += string.Format("    {0}\n", a.ToString());
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
            throw new AttributeNonexistentException(string.Format("Field {0} does not exist", oldName));
        }
        if (FieldExists(newName))
        {
            throw new AttributeAlreadyExistsException(string.Format("Field {0} already exists", newName));
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
            throw new AttributeNonexistentException(string.Format("Method {0} does not exist", oldName));
        }
        if (MethodExists(newName))
        {
            throw new AttributeAlreadyExistsException(string.Format("Method {0} already exists", newName));
        }
        
        // Rename field
        GetMethodByName(oldName)!.AttRename(newName);
        
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
    /// <param name="name">Name to rename class to</param>
    public void Rename(string name)
    {
        CheckValidClassName(name);
        ClassName = name;
    }

    public object Clone()
    {
        return new Class(this);
    }
}
