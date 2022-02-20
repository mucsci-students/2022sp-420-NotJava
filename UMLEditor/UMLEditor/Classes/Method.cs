using UMLEditor.Exceptions;

namespace UMLEditor.Classes;

using System.Collections.Generic;
using Newtonsoft.Json;

public class Method : AttributeObject
{

    [JsonProperty("params")]
    private List<NameTypeObject> _parameters;

    [JsonProperty("return_type")] 
    public string ReturnType { get; private set; }

    /// <summary>
    /// Constructs a new method with the provided name and (optionally) a list of parameters
    /// </summary>
    /// <param name="withName">The name of the new method</param>
    /// <param name="returnType">The type this method returns</param>
    public Method(string withName, string returnType)
    {
        
        CheckValidAttributeName(withName);
        AttributeName = withName;
        ReturnType = returnType; 
        
        _parameters = new List<NameTypeObject>();

    }

    /// <summary>
    /// Adds one parameter to the method
    /// </summary>
    /// <param name="param">Parameter to add to method</param> 
    public void AddParam(NameTypeObject param)
    {
        bool found = false;
        foreach (NameTypeObject p in _parameters)
        {
            if (p == param)
            {
                found = true;
            }
        }

        if (found)
        {
            throw new NameTypeObjectAlreadyExistsException($"{param} already exists as a parameter.");
        }
        
        _parameters.Add(param);
    }

    public void AddParam(List<NameTypeObject> parameters)
    {
        foreach (NameTypeObject p in parameters)
        {
            
        }
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
    
    public override string ToString()
    {
        return $"{ReturnType} {AttributeName} ({ParamsToString()})";
    }
}