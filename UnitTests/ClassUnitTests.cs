using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateDefaultClassTest()
    {
        Class defaultClass = new Class();
        Assert.IsInstanceOf<Class>(defaultClass);
    }
    [Test]
    public void CreateClassTest()
    {
        Class newClass = new Class();

        // Make sure we can instantiate a test class
        newClass = new Class("TestClass1");
        Assert.AreEqual("TestClass1", newClass.ClassName);
    }

    [Test]
    public void CreateInvalidClassTest()
    {
        Assert.Throws<InvalidNameException>(delegate { new Class("%#0923"); });
    }
    
    [Test]
    public void RenameClassToInvalidClassTest()
    {
        Class c = new Class("TestClass");
        Assert.Throws<InvalidNameException>(delegate { c.Rename("%#0923"); });
    }

    //TODO Do we have / want a toString() method for the Class type?
    // [Test]
    // public void TestToString()
    // {
    //     Class newClass = new Class("toString");
    //     Assert.Equals(newClass.ToString(), "Class toString");
    //     TestingSled.PrintColoredLine("Class toString: " + newClass.ToString(), TestingSled.SUCCESS_COLOR);
    // }
}