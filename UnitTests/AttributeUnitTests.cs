using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class AttributeUnitTests
{
    
    [SetUp]
    public void Setup()
    {
        methodName1 = "TestMethod1";
        methodName2 = "TestMethod2";
        sameMethodName = methodName1;
        validType = "string";
    }

    [Test]
    public void CreateNewMethodObjectTest()
    {
        Method newMethod = new Method(methodName1, validType);
        Assert.IsInstanceOf<Method>(newMethod);
    }
    /*
    [Test]
    public void CreateDefaultAttributeObjectTest()
    {
        AttributeObject defaultAttribute = new AttributeObject();
        Assert.IsInstanceOf<AttributeObject>(defaultAttribute);
    }

    [Test]
    public void CreateAttributeObjectTest()
    {
        Assert.AreEqual(attribName, (new AttributeObject(attribName)).AttributeName);
    }

    [Test]
    public void ToStringTest()
    {
        Assert.AreEqual($"Attribute: {attribName}", (new AttributeObject(attribName)).ToString());
    }

    [Test]
    public void CreateInvalidAttributeObjectTest()
    {
        Assert.Throws<InvalidNameException>(delegate { new AttributeObject("%#0923"); });
    }

    [Test]
    public void RenameAttributeTest()
    {
        AttributeObject attrib = new AttributeObject(attribName);
        attrib.AttRename("diffName");
        Assert.AreEqual("diffName", attrib.AttributeName);
    }

    [Test]
    public void RenameAttributeInvalidNameTest()
    {
        AttributeObject attrib = new AttributeObject(attribName);
        Assert.Throws<InvalidNameException>(delegate { attrib.AttRename("%#0923"); });
    }*/

    private string methodName1 = "";
    private string methodName2 = "";
    private string sameMethodName = "";
    private string validType = "";

}