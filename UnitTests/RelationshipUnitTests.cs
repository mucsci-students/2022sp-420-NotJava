using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;
using System.Collections.Generic;

namespace UnitTests;

[TestFixture]
public class RelationshipUnitTests
{
    
    [SetUp]
    public void Setup()
    {
        class1Name = "Test1";
        class2Name = "Test2";
        types.Add("aggregation");
        types.Add("composition");
        types.Add("Inheritance");
        types.Add("Realizations");
        types.Add("potato");
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
        Relationship rel = new Relationship(class1Name, class2Name, types[0]);
        Assert.AreEqual(class1Name, rel.SourceClass);
        Assert.AreEqual(class2Name, rel.DestinationClass);
    }

    //This test should probably be run from here, but class name checking is done in diagram class
    /*[Test]
    public void InvalidRelationshipClassesTest()
    {
        Assert.Throws<ClassNonexistentException>(delegate { new Relationship("foobar", class1Name, types[0]); });
        Assert.Throws<ClassNonexistentException>(delegate { new Relationship(class2Name, "barfoo", types[0]); });
        Assert.Throws<ClassNonexistentException>(delegate { new Relationship("foobar", "barfoo", types[0]); });
    }*/

    [Test]
    public void RelationshipToStringTest()
    {
        Relationship rel = new Relationship(class1Name, class2Name, types[1]);
        Assert.AreEqual($"{types[1]}: {class1Name} => {class2Name}", rel.ToString());
    }

    [Test]
    public void InvalidTypeTest()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<InvalidRelationshipTypeException>(delegate { new Relationship(class2Name, class1Name, types[2]); });
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<InvalidRelationshipTypeException>(delegate { new Relationship(class2Name, class1Name, types[3]); });
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<InvalidRelationshipTypeException>(delegate { new Relationship(class2Name, class1Name, types[4]); });
    }

    [Test]
    public void ChangeTypeTest()
    {
        Relationship testRel = new Relationship(class1Name, class2Name, types[0]);
        testRel.ChangeType(types[1]);
        Assert.AreEqual(types[1], testRel.RelationshipType);
    }

    [Test]
    public void InvalidChangeTypeTest()
    {
        Relationship testRel = new Relationship(class1Name, class2Name, types[0]);
        Assert.Throws<InvalidRelationshipTypeException>(delegate { testRel.ChangeType(types[2]); });
        Assert.Throws<InvalidRelationshipTypeException>(delegate { testRel.ChangeType(types[4]); });
    }
    
    private string class1Name = "";
    private string class2Name = "";
    private List<string> types = new List<string>();
}