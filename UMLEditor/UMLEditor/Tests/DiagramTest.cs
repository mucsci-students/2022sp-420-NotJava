namespace UMLEditor.Tests;
using UMLEditor.Classes;

public class DiagramTest
{
    public Diagram diagram = new Diagram();
    public Diagram GetDiagram()
    {
        //Class c = new Class();
        //c.ClassName = "One";
        //c = new Class();
        //diagram.Classes.Add(c);
        //c.ClassName = "Two";
        Relationship r = new Relationship("One", "Two");
        diagram.Relationships.Add(r);
        r = new Relationship("Three", "Four");
        diagram.Relationships.Add(r);
        return diagram;
    }
}