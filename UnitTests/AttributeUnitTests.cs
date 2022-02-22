using System;
using System.Collections.Generic;
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
        invalidMethodName = "*&23";
        validType = "string";
        paramName1 = "TestParam1";
        paramName2 = "TestParam2";
        sameParamName = paramName1;
        invalidParamName = invalidMethodName;
    }

    [Test]
    public void CreateDefaultMethodObjectTest()
    {
        Method defaultMethod = new Method();
        Assert.IsInstanceOf<Method>(defaultMethod);
    }

    [Test]
    public void CreateNewMethodObjectTest()
    {
        Method newMethod = new Method(validType, methodName1);
        Assert.IsInstanceOf<Method>(newMethod);
        Assert.AreEqual(validType, newMethod.ReturnType);
        Assert.AreEqual(methodName1, newMethod.AttributeName);
    }

    [Test]
    public void CreateInvalidMethodTest()
    {
        Class testClass = new Class("TestClass");
        testClass.AddMethod(validType, methodName1);
        Assert.Throws<InvalidNameException>(delegate { testClass.AddMethod(validType, invalidMethodName); });
        Assert.Throws<AttributeAlreadyExistsException>(delegate { testClass.AddMethod(validType, sameMethodName); });
    }

    [Test]
    public void RenameMethodTest()
    {
        Method newMethod = new Method(validType, methodName1);
        newMethod.AttRename(methodName2);
        Assert.AreEqual(methodName2, newMethod.AttributeName);
        Assert.Throws<InvalidNameException>(delegate { newMethod.AttRename(invalidMethodName); });
    }

    [Test]
    public void DeleteMethodTest()
    {
        Class testClass = new Class("TestClass");
        testClass.AddMethod(validType, methodName1);
        Assert.AreEqual(true, testClass.MethodExists(methodName1));
        Assert.Throws<AttributeNonexistentException>(delegate { testClass.DeleteMethod(methodName2); });
        testClass.DeleteMethod(methodName1);
        Assert.AreEqual(false, testClass.MethodExists(methodName1));
    }

    [Test]
    public void CreateNewParamObjectTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        Assert.IsInstanceOf<NameTypeObject>(testParam1);
        Assert.AreEqual(validType, testParam1.Type);
        Assert.AreEqual(paramName1, testParam1.AttributeName);
    }

    [Test]
    public void CreateInvalidParamObjectTest()
    {
        Assert.Throws<InvalidNameException>(delegate { new NameTypeObject(validType, invalidParamName); });
    }

    [Test]
    public void AddParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        NameTypeObject testParam2 = new NameTypeObject(validType, paramName2);
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};
        
        testMethod.AddParam(testParam1);
        Assert.AreEqual(true, testMethod.IsParamInList(paramName1));
        Assert.AreEqual(false, testMethod.IsParamInList(testParam2));
        Method testMethod2 = new Method(validType, methodName2);
        
        testMethod2.AddParam(testParams);
        Assert.AreEqual(true, testMethod2.IsParamInList(testParam1));
        Assert.AreEqual(true, testMethod2.IsParamInList(paramName2));
    }

    [Test]
    public void InvalidAddParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        NameTypeObject testParam2 = new NameTypeObject(validType, paramName2);
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};
        testMethod.AddParam(testParam1);
        Assert.Throws<AttributeAlreadyExistsException>(delegate { testMethod.AddParam(testParam1);});
        Assert.Throws<AttributeAlreadyExistsException>(delegate { testMethod.AddParam(testParams); });
    }

    [Test]
    public void ChangeParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        NameTypeObject testParam2 = new NameTypeObject(validType, paramName2);
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};
        testMethod.AddParam(testParam1);
        Assert.AreEqual(true, testMethod.IsParamInList(testParam1));
        testMethod.ChangeParam(testParam1, testParam2);
        Assert.AreEqual(false, testMethod.IsParamInList(testParam1));
        Assert.AreEqual(true, testMethod.IsParamInList(testParam2));

        testMethod.ChangeParam(testParams);
        Assert.AreEqual(true, testMethod.IsParamInList(paramName1));
        Assert.AreEqual(true, testMethod.IsParamInList(paramName2));
    }

    [Test]
    public void InvalidChangeParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        NameTypeObject testParam2 = new NameTypeObject(validType, paramName2);
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};
        testMethod.AddParam(testParam1);

        Assert.Throws<AttributeNonexistentException>(delegate { testMethod.ChangeParam(testParam2, testParam2);});
        Assert.Throws<AttributeAlreadyExistsException>(delegate { testMethod.ChangeParam(testParam1, testParam1); });
    }

    [Test]
    public void RemoveParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        testMethod.AddParam(testParam1);
        Assert.AreEqual(true, testMethod.IsParamInList(testParam1));
        testMethod.RemoveParam(testParam1);
        Assert.AreEqual(false, testMethod.IsParamInList(testParam1));
        testMethod.AddParam(testParam1);
        Assert.AreEqual(true, testMethod.IsParamInList(testParam1));
        testMethod.RemoveParam();
        Assert.AreEqual(false, testMethod.IsParamInList(testParam1));
    }

    [Test]
    public void InvalidRemoveParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        Assert.Throws<AttributeNonexistentException>(delegate { testMethod.RemoveParam(testParam1);});
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
    private string invalidMethodName = "";
    private string validType = "";
    private string paramName1 = "";
    private string paramName2 = "";
    private string sameParamName = "";
    private string invalidParamName = "";
}