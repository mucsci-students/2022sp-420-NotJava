namespace UMLEditor.Classes;

using System;
using UMLEditor.Utility;
using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

public class Method : AttributeObject, ICloneable
{

    [JsonProperty("params")]
    private List<NameTypeObject> _parameters;

    // Public accessor for Parameters
    // Creates copies to ensure data integrity
    [JsonIgnore]
    public List<NameTypeObject> Parameters
    {
        get => Utilities.CloneContainer(_parameters);
    }

    [JsonProperty("return_type")] 
    public string ReturnType { get; private set; }

    /// <summary>
    /// Default ctor
    /// </summary>
    public Method()
    {

        AttributeName = "";
        ReturnType = "";
        _parameters = new List<NameTypeObject>();

    }

    /// <summary>
    /// Constructs a new method with the provided name and (optionally) a list of parameters
    /// </summary>
    /// <param name="returnType">The type this method returns</param>
    /// <param name="withName">The name of the new method</param>
    public Method(string returnType, string withName) : this()
    {
        
        CheckValidAttributeName(withName);
        CheckValidAttributeName(returnType);
        AttributeName = withName;
        ReturnType = returnType; 

    }
    
    /// <summary>
    /// Constructs a new method with the provided name, type, and a list of parameters.
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="withName"></param>
    /// <param name="parameters"></param>
    public Method(string returnType, string withName, List<NameTypeObject> parameters) : this(returnType, withName)
    {
        _parameters = parameters;
    }
    
    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="m">Method object to copy</param>
    public Method(Method m) : this(m.ReturnType, m.AttributeName)
    {
        _parameters = Utilities.CloneContainer(m._parameters);
    }

    public void ChangeMethodType(string newType)
    {
        CheckValidAttributeName(newType);
        ReturnType = newType;
    }

    /// <summary>
    /// Checks if given parameter is in the current parameter list using NameTypeObject
    /// </summary>
    /// <param name="param">Parameter to check</param>
    /// <returns>True if the given parameter is in the list, false otherwise</returns>
    public bool IsParamInList(NameTypeObject param)
    {
        return FindParamInList(param) is not null;
    }

    /// <summary>
    /// Locates the provided parameter contained in this method
    /// and returns the reference to the parameter in the list 
    /// </summary>
    /// <param name="param">The parameter to locate</param>
    /// <returns>The reference of the parameter in the list, if it exists at all</returns>
    public NameTypeObject? FindParamInList(NameTypeObject param)
    {
        foreach (NameTypeObject p in _parameters)
        {
            if (p.AttributeName == param.AttributeName)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>
    /// Locates the provided parameter in this method and returns the reference to the parameter in the list 
    /// </summary>
    /// <param name="paramName">The name of the parameter to search for</param>
    /// <returns>The reference of the parameter in the list, if it exists at all</returns>
    public NameTypeObject? FindParamInList(string paramName)
    {
        foreach (NameTypeObject p in _parameters)
        {
            if (p.AttributeName == paramName)
            {
                return p;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Checks if given parameter is in the current parameter list using name
    /// </summary>
    /// <param name="paramName">Name of the parameter to check</param>
    /// <returns>True if the given parameter is in the list, false otherwise</returns>
    public bool IsParamInList(string paramName)
    {
        return FindParamInList(paramName) is not null;
    }

    /// <summary>
    /// Adds one parameter to the method
    /// </summary>
    /// <param name="param">Parameter to add to method</param>
    /// <exception cref="AttributeAlreadyExistsException">If parameter name already exists</exception>
    public void AddParam(NameTypeObject param)
    {

        if (IsParamInList(param))
        {
            throw new AttributeAlreadyExistsException($"{param} already exists as a parameter.");
        }
        
        _parameters.Add(param);
    }

    /// <summary>
    /// Overloaded AddParam function to add multiple parameters to the method
    /// </summary>
    /// <param name="parameters">List of parameters to add to method</param>
    /// <exception cref="AttributeAlreadyExistsException">If parameter name already exists in method</exception>
    public void AddParam(List<NameTypeObject> parameters)
    {
        // Check entire list of parameters first
        foreach (NameTypeObject p in parameters)
        {
            if (IsParamInList(p))
            {
                throw new AttributeAlreadyExistsException($"{p} already exists as a parameter.");
            }
        }

        foreach (NameTypeObject p in parameters)
        {
            _parameters.Add(p);
        }
    }

    /// <summary>
    /// Removes all parameters from method
    /// </summary>
    public void RemoveParam()
    {
        _parameters.Clear();
    }

    /// <summary>
    /// Overloaded function to remove parameter from method
    /// </summary>
    /// <param name="param">Parameter to remove from method</param>
    /// <exception cref="AttributeNonexistentException">If the provided parameter is not in the list</exception>
    public void RemoveParam(string param)
    {
        if (!(_parameters.Remove(FindParamInList(param)!)))
        {
            throw new AttributeNonexistentException($"{param} does not exist in the method");
        }
    }

    /// <summary>
    /// Changes a single parameter of the method to the provided one
    /// </summary>
    /// <param name="oldParam">The existing parameter to replace</param>
    /// <param name="newParam">The new parameter to put in method</param>
    /// <exception cref="AttributeNonexistentException">If oldParam does not exist in method</exception>
    /// <exception cref="AttributeAlreadyExistsException">If newParam already exists in method</exception>
    public void ChangeParam(NameTypeObject oldParam, NameTypeObject newParam)
    {
        if (!IsParamInList(oldParam))
        {
            throw new AttributeNonexistentException($"{oldParam} does not exist");
        }

        if (IsParamInList(newParam))
        {
            throw new AttributeAlreadyExistsException($"{newParam} already exists");
        }

        _parameters[_parameters.IndexOf(oldParam)] = newParam;
    }

    /// <summary>
    /// Changes the name on the provided parameter
    /// </summary>
    /// <param name="oldParamName">The current (old) parameter name</param>
    /// <param name="newParamName">The new name for the parameter</param>
    public void RenameParam(string oldParamName, string newParamName)
    {

        if (!IsParamInList(oldParamName))
        {
            throw new AggregateException($"Method '{AttributeName}' does not have a parameter named '{oldParamName}'");
        }

        NameTypeObject? targetParam = FindParamInList(oldParamName);
        targetParam!.AttRename(newParamName);

    }

    /// <summary>
    /// Replaces a provided parameter with a new one
    /// </summary>
    /// <param name="toReplace">The parameter to replace</param>
    /// <param name="replaceWith">The new anatomy of the parameter</param>
    public void ReplaceParam(string toReplace, NameTypeObject replaceWith)
    {

        // Make sure the new name is not a duplicate
        bool nameChanged = toReplace != replaceWith.AttributeName;
        if (nameChanged && IsParamInList(replaceWith))
        {
            throw new AttributeAlreadyExistsException($"A parameter by the name of '{replaceWith.AttributeName}' already exists in method '{AttributeName}'");
        }

        // Make sure the source parameter exists
        if (!IsParamInList(toReplace))
        {
            throw new AttributeNonexistentException($"Parameter '{replaceWith.ToString()}' does not exist in method '{AttributeName}'");
        }

        /* - Find a reference to the target parameter
         * - Rename the target parameter and apply the new type */
        NameTypeObject? targetParam = FindParamInList(toReplace);
        targetParam!.AttRename(replaceWith.AttributeName);
        targetParam!.ChangeType(replaceWith.Type);

    }
    
    /// <summary>
    /// Changes existing parameter list to the provided one
    /// </summary>
    /// <param name="parameters">List of new parameters to use in method</param>
    public void ChangeParam(List<NameTypeObject> parameters)
    {
        _parameters = parameters;
    }

    /// <summary>
    /// Clears the existing parameter list
    /// </summary>
    public void ClearParameters()
    {
        _parameters = new List<NameTypeObject>();
    }
    
    /// <summary>
    /// Returns the parameter list as a string separated list
    /// </summary>
    /// <returns>A string with a comma separated list of the parameters</returns>
    private string ParamsToString()
    {

        string result = "";
        
        for (int indx = 0; indx < _parameters.Count; ++indx)
        {

            result += _parameters[indx];
            
            // Only print the ", " when this is not the last element 
            if ((indx + 1) < _parameters.Count)
            {

                result += ", ";

            }

        }

        return result;

    }
    
    /// <summary>
    /// Converts method to a string
    /// </summary>
    /// <returns>String representing method</returns>
    public override string ToString()
    {
        return $"{ReturnType} {AttributeName} ({ParamsToString()})";
    }

    public object Clone()
    {
        return new Method(this);
    }
}