namespace UMLEditor.Classes;

using System.Collections.Generic;
public class Class
{
    public string ClassName { get; private set; }
    public List<Attribute> Attributes { get; private set; }

    public Class()
    {
        ClassName = "";
        Attributes = new List<Attribute>();
    }
}