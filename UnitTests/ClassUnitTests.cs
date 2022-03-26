using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class ClassUnitTests
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
        Class newClass;

        // Make sure we can instantiate a test class
        newClass = new Class("TestClass1");
        Assert.AreEqual("TestClass1", newClass.ClassName);
    }

    [Test]
    public void CreateInvalidClassTest()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<InvalidNameException>(delegate { new Class("%#0923"); });
    }
    
    [Test]
    public void ChangeClassBoxLocationTest()
    {
        Class newClass = new Class();
        newClass.ChangeLocation(100,100);
        
        Assert.AreEqual(100, newClass.ClassLocation.X);
        Assert.AreEqual(100, newClass.ClassLocation.Y);
    }
    
    [Test]
    public void RenameClassToInvalidClassTest()
    {
        Class c = new Class("TestClass");
        Assert.Throws<InvalidNameException>(delegate { c.Rename("%#0923"); });
    }
    
}