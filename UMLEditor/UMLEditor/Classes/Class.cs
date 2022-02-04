using Newtonsoft.Json;

namespace UMLEditor.Classes;

using System.Collections.Generic;
public class Class
{
    // Used for JSON serialization  and deserialization
    [JsonProperty("classname")]
    public string ClassName { get; private set; }
    
    // Used for JSON serialization  and deserialization
    [JsonProperty("attributes")]
    public List<Attribute> Attributes { get; private set; }

    /// <summary>
    /// Default constructor for class
    /// </summary>
    public Class()
    {
        ClassName = "";
        Attributes = new List<Attribute>();
    }

    /// <summary>
    /// Constructor for class "name"
    /// </summary>
    /// <param name="name">Name of the class being created</param>
    public Class(string name) : this()
    {

        ClassName = name;

    }
    
}