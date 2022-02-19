namespace UMLEditor.Classes;

using System.Collections.Generic;
using Newtonsoft.Json;

public class Method : AttributeObject
{

    [JsonProperty("params")]
    private List<Parameter> _parameters;

    [JsonProperty("params")] 
    public string ReturnType { get; private set; }

    /// <summary>
    /// Constructs a new method with the provided name and (optionally) a list of parameters
    /// </summary>
    /// <param name="name">The name of the new method</param>
    /// <param name="returnType">The type this method returns</param>
    /// <param name="withParams">An array of parameters to use</param>
    public Method(string name, string returnType, params Parameter[] withParams)
    {

        // NOTE: Maybe withParams should be a List<Parameter> and not an array?
        
        AttributeName = name;
        ReturnType = returnType; 
        
        _parameters = new List<Parameter>();
        _parameters.AddRange(withParams);

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