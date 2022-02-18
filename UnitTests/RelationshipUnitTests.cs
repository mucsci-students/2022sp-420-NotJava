using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class RelationshipUnitTests
{
    
    [SetUp]
    public void Setup()
    {
        class1Name = "Test1";
        class2Name = "Test2";
    }
    
    [Test]
    public void CreateDefaultRelationshipTest()
    {
        Relationship defaultAttribute = new Relationship();
        Assert.IsInstanceOf<Relationship>(defaultAttribute);
    }

    [Test]
    public void CreateRelationshipTest()
    {
        Relationship rel = new Relationship(class1Name, class2Name);
        Assert.AreEqual(class1Name, rel.SourceClass);
        Assert.AreEqual(class2Name, rel.DestinationClass);
    }

    [Test]
    public void RelationshipToStringTest()
    {
        Relationship rel = new Relationship(class1Name, class2Name);
        Assert.AreEqual($"{class1Name} => {class2Name}", rel.ToString());
    }
    
    private string class1Name = "";
    private string class2Name = "";
}