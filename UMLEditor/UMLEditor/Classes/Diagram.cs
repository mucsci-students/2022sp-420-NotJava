using System;
using System.Net.Security;
using DynamicData.Kernel;

namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Diagram
{
    public List<Class> Classes { get; private set; }
    public List<Relationship> Relationships { get; private set; }

    public Diagram()
    {
        Classes = new List<Class>();
        Relationships = new List<Relationship>();
    }

    public Class GetClassByName(string name)
    {
        // TODO
        return null;
    }

    public string ListClasses()
    {
        string output = "";
        if (Relationships.Count == 0)
        {
            output = "No classes in diagram!";
        }
        else
        {
            foreach (Relationship r in Relationships)
            {
                output += r.ToString() + "\n";
            }
        }

        return output;
    }
}