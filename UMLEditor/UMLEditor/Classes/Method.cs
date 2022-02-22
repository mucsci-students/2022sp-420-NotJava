﻿namespace UMLEditor.Classes;

using System.Collections.Generic;
using Newtonsoft.Json;
using Exceptions;

public class Method : AttributeObject
{

    [JsonProperty("params")]
    private List<NameTypeObject> _parameters;

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
        AttributeName = withName;
        ReturnType = returnType; 

    }

    /// <summary>
    /// Checks if given parameter is in the current parameter list using NameTypeObject
    /// </summary>
    /// <param name="param">Parameter to check</param>
    /// <returns>True if the given parameter is in the list, false otherwise</returns>
    public bool IsParamInList(NameTypeObject param)
    {
        foreach (NameTypeObject p in _parameters)
        {
            if (p.AttributeName == param.AttributeName)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Checks if given parameter is in the current parameter list using name
    /// </summary>
    /// <param name="paramName">Name of the parameter to check</param>
    /// <returns>True if the given parameter is in the list, false otherwise</returns>
    public bool IsParamInList(string paramName)
    {
        foreach (NameTypeObject p in _parameters)
        {
            if (p.AttributeName == paramName)
            {
                return true;
            }
        }
        return false;
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
    public void RemoveParam(NameTypeObject param)
    {
        if (_parameters.Contains(param))
        {
            _parameters.Remove(param);
        }
        else
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
    /// Changes existing parameter list to the provided one
    /// </summary>
    /// <param name="parameters">List of new parameters to use in method</param>
    public void ChangeParam(List<NameTypeObject> parameters)
    {
        _parameters = parameters;
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
}