using System;

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
}