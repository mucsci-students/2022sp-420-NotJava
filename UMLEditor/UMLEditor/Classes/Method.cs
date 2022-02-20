namespace UMLEditor.Classes;

using System.Collections.Generic;
using Newtonsoft.Json;
using UMLEditor.Exceptions;

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
    /// <param name="withName">The name of the new method</param>
    /// <param name="returnType">The type this method returns</param>
    public Method(string withName, string returnType) : this()
    {
        
        CheckValidAttributeName(withName);
        AttributeName = withName;
        ReturnType = returnType; 

    }

    private bool ParamInList(NameTypeObject param)
    {
        bool found = false;
        foreach (NameTypeObject p in _parameters)
        {
            if (p == param)
            {
                found = true;
            }
        }

        return found;
    }

    /// <summary>
    /// Adds one parameter to the method
    /// </summary>
    /// <param name="param">Parameter to add to method</param>
    /// <exception cref="NameTypeObjectAlreadyExistsException">If parameter (name and type) already exists</exception>
    public void AddParam(NameTypeObject param)
    {

        if (ParamInList(param))
        {
            throw new NameTypeObjectAlreadyExistsException($"{param} already exists as a parameter.");
        }
        
        _parameters.Add(param);
    }

    /// <summary>
    /// Overloaded AddParam function to add multiple parameters to the method
    /// </summary>
    /// <param name="parameters">List of parameters to add to method</param>
    /// <exception cref="NameTypeObjectAlreadyExistsException">If parameter (name and type) already exists</exception>
    public void AddParam(List<NameTypeObject> parameters)
    {
        NameTypeObject foundParam = null;
        foreach (NameTypeObject p in parameters)
        {
            if (ParamInList(p))
            {
                foundParam = p;
            }
        }
        if (foundParam is null)
        {
            throw new NameTypeObjectAlreadyExistsException($"{foundParam} already exists as a parameter.");
        }

        foreach (NameTypeObject p in parameters)
        {
            _parameters.Add(p);
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