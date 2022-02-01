namespace UMLEditor.Classes;

using System.Collections.Generic;

public class Relationship
{
    private string SourceClass;
    private string DestinationClass;

    public Relationship()
    {
        SourceClass = "";
        DestinationClass = "";
    }
    public Relationship(string source, string destination)
    {
        SourceClass = source;
        DestinationClass = destination;
    }
    
    public override string ToString()
    {
        return string.Format("{0} => {1}", SourceClass, DestinationClass);
    }
}