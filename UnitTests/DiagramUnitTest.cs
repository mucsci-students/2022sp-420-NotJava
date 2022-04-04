using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class DiagramUnitTest
{
    
    [Test]
    public void CloneDiagram()
    {
        Diagram diagram = new Diagram();
        
        diagram.AddClass("Hello");
        diagram.AddClass("So_long");
        diagram.AddClass("Fare_Well");
        diagram.AddRelationship("Fare_Well", "So_long", "inheritance");

        Diagram clonedDiagram = (Diagram)diagram.Clone();
        
        //If the equality operator is overloaded, this will not work
        Assert.AreNotEqual(diagram, clonedDiagram);
        Assert.AreEqual(diagram.ListClasses(), clonedDiagram.ListClasses());
        Assert.AreEqual(diagram.ListRelationships(), clonedDiagram.ListRelationships());
        
        //Checking to ensure cloned diagram has a copy of a class, not the original reference
        //If the copy does not throw an exception, the references were referring to the same class, not copies
        diagram.RenameClass("Hello", "Darkness");
        Assert.Throws<ClassNonexistentException>(delegate { clonedDiagram.RenameClass("Darkness", "My_old_friend");});
        
        diagram.ChangeRelationship("Fare_Well", "So_long", "realization");
        
        //If references are the same, then a RelationshipAlreadyExists exception would be thrown
        Assert.DoesNotThrow(delegate { clonedDiagram.ChangeRelationship("Fare_Well", "So_long", "realization"); });
    }
    
    //Test ListClasses
}