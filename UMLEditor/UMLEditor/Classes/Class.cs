namespace UMLEditor.Classes;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

public class Class
{
    [JsonProperty("name")]
    public string ClassName { get; private set; }
    
    [JsonProperty("fields")]
    public List<NameTypeObject> Fields;

    [JsonProperty("methods")]
    public List<Method> Methods;
    

    /// <summary>
    /// Default constructor for class
    /// </summary>
    public Class()
    {
        ClassName = "";
        Fields = new List<NameTypeObject>();
        Methods = new List<Method>();
    }

    /// <summary>
    /// Constructor for class "name"
    /// </summary>
    /// <param name="name">Name of the class being created</param>
    /// <param name="withFields">(Optional) A list of fields to include in this class</param>
    /// <exception cref="InvalidNameException">Thrown if the name provided is invalid</exception>
    public Class(string name) : this()
    {
        CheckValidClassName(name);
        ClassName = name;
        
    }

    /// <summary>
    /// Adds field to class.
    /// Pre-condition: name of field is valid
    /// </summary>
    /// <param name="name">Valid name of new field</param>
    /// <param name="type">Valid type of new field</param>
    public void AddField(string name, string type)
    {
        if (FieldExists(name))
        {
            throw new AttributeAlreadyExistsException(string.Format("Field {0} already exists", name));
        }
        Fields.Add(new NameTypeObject(name, type));
        
    }
    
    /// <summary>
    /// Adds method to class.
    /// Pre-condition: name of method is valid
    /// </summary>
    /// <param name="name">Valid name of new method</param>
    /// <param name="returnType">Valid returnType of new method</param>
    public void AddMethod(string name, string returnType)
    {
        if (MethodExists(name))
        {
            throw new AttributeAlreadyExistsException(string.Format("Method {0} already exists", name));
        }
        Methods.Add(new Method(name, returnType));
        
    }

    /// <summary>
    /// Check if specified Field exists.
    /// </summary>
    /// <param name="name">Name of the Field you are checking</param>
    /// <returns>Returns true if exists, false if not.</returns>
    public bool FieldExists (string name)
    {

        return GetFieldByName(name) != null;

    }

    /// <summary>
    /// Finds the field with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of field you are looking for</param>
    /// <returns>Returns the field if exists, or null if it does not</returns>
    public NameTypeObject? GetFieldByName(string name)
    {
        
        foreach (NameTypeObject currentField in Fields)
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

        return GetMethodByName(name) != null;

    }

    /// <summary>
    /// Finds the method with the specified name, if it exists
    /// </summary>
    /// <param name="name">Name of method you are looking for</param>
    /// <returns>Returns the method if it exists, or null if it does not</returns>
    public Method? GetMethodByName(string name)
    {
        
        foreach (Method currentMethod in Methods)
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
    /// <param name="targetField">Field to be deleted</param>
    /// <exception cref="AttributeNonexistentException">If field does not exist</exception>
    public void DeleteField(string targetFieldName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent field name entered ({0}).";
        
        // Ensure the provided classes exist
        bool removeWorked = Fields.Remove(GetFieldByName(targetFieldName));
        if (!removeWorked)
        {

            throw new AttributeNonexistentException(string.Format(NONEXISTENT_NAME_FORMAT, targetFieldName));
            
        }

    }
    
    /// <summary>
    /// Deletes a method within this class.
    /// </summary>
    /// <param name="targetMethod">Method to be deleted</param>
    /// <exception cref="AttributeNonexistentException">If method does not exist</exception>
    public void DeleteMethod(string targetMethodName)
    {
        
        const string NONEXISTENT_NAME_FORMAT = "Nonexistent method name entered ({0}).";
        
        // Ensure the provided classes exist
        bool removeWorked = Methods.Remove(GetMethodByName(targetMethodName));
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
        
        if (Fields.Count == 0)
        {
            msg += "    There are no fields currently. \n";
        }
        else
        {
            foreach (NameTypeObject a in Fields)
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
        
        if (Methods.Count == 0)
        {
            msg += "    There are no methods currently. \n";
        }
        else
        {
            foreach (Method a in Methods)
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
    /// <exception cref="InvalidNameException">If the provided name is invalid.</exception>
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
        GetFieldByName(oldName).AttRename(newName);
        
    }
    
    /// <summary>
    /// Renames a method
    /// </summary>
    /// <param name="oldName">Method to rename</param>
    /// <param name="newName">New name of method</param>
    /// <exception cref="AttributeNonexistentException">Thrown if oldName method does not exist</exception>
    /// <exception cref="AttributeAlreadyExistsException">Thrown if newName method already exists</exception>
    /// <exception cref="InvalidNameException">If the provided name is invalid.</exception>
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
        GetMethodByName(oldName).AttRename(newName);
        
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
}