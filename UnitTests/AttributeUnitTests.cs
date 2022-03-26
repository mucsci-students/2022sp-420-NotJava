using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
        // ReSharper disable once ObjectCreationAsStatement
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
        // ReSharper disable once UnusedVariable
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
        testMethod.RemoveParam(testParam1.AttributeName);
        Assert.AreEqual(false, testMethod.IsParamInList(testParam1));
        testMethod.AddParam(testParam1);
        Assert.AreEqual(true, testMethod.IsParamInList(testParam1));
        testMethod.RemoveParam();
        Assert.AreEqual(false, testMethod.IsParamInList(testParam1));
    }
    
    [Test]
    public void ListConstructorTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};
        Method testMethod = new Method("String", "TestName", testParams );
        
        Assert.AreEqual("String", testMethod.ReturnType);
        Assert.AreEqual("TestName", testMethod.AttributeName);
        
        Assert.AreEqual(true, testMethod.IsParamInList(testParam1));
    }
    
    [Test]
    public void ChangeMethodTypeTest()
    {
        Method testMethod = new Method("Double", "TestName");
        testMethod.ChangeMethodType("String");
        
        Assert.AreEqual("String", testMethod.ReturnType);
    }
    
    [Test]
    public void ClearParametersTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};
        Method testMethod = new Method("String", "TestName", testParams );
        testMethod.ClearParameters();
        
        Assert.AreEqual(0, testMethod.Parameters.Count);
    }
    
    [Test]
    public void RenameParameterTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, "paramName1");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};
        Method testMethod = new Method("String", "TestName", testParams );
        
        testMethod.RenameParam("paramName1", "wackus");
        
        Assert.AreEqual(true, testMethod.IsParamInList("wackus"));
    }
    
    [Test]
    public void ChangeReturnTypeTest()
    {
        Method testMethod = new Method("String", "TestName");
        testMethod.ChangeReturnType("Double");

        Assert.AreEqual("Double", testMethod.ReturnType);
    }
    
    [Test]
    public void ReplaceParamDoesNotExistTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, "paramName1");
        List<NameTypeObject> testParams = new List<NameTypeObject>{};
        Method testMethod = new Method("String", "TestName", testParams );

        Assert.Throws<AttributeNonexistentException>(delegate { testMethod.ReplaceParam("Param1",testParam1 );});
    }
    
    [Test]
    public void ReplaceParamTest()
    {
        NameTypeObject testParam1 = new NameTypeObject(validType, "paramName1");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};
        NameTypeObject testParam2 = new NameTypeObject(validType, "paramName2");

        Method testMethod = new Method("String", "TestName", testParams );

        testMethod.ReplaceParam("paramName1", testParam2);
        Assert.AreEqual(true, testMethod.IsParamInList("paramName2"));
    }
    
    [Test]
    public void ParamToStringTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};

        Method testMethod = new Method("String", "TestName", testParams );

        Assert.AreEqual("String paramName1", testMethod.ParamsToString());
    }
    
    [Test]
    public void MethodToStringTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};

        Method testMethod = new Method("String", "TestName", testParams );

        Assert.AreEqual("String TestName (String paramName1)", testMethod.ToString());
    }
    
    [Test]
    public void NameTypeEqualityTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("String", "paramName1");
        

        Assert.AreEqual(true, testParam1 == testParam2);
    }
    
    [Test]
    public void NameTypeInequalityTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("String", "paramName1");
        
        Assert.AreEqual(false, testParam1 != testParam2);
    }
    
    [Test]
    public void NameTypeDefaultCtorTest()
    {
        NameTypeObject testParam1 = new NameTypeObject();

        Assert.AreEqual("", testParam1.Type);
        Assert.AreEqual("", testParam1.AttributeName);
    }
    
    [Test]
    public void NameTypeCopyCtorTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject(testParam1);
        
        Assert.AreEqual(true, testParam2 == testParam1);
    }
    
    [Test]
    public void NameTypeCloneTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");

        Assert.AreEqual(true, (NameTypeObject) testParam1.Clone() == testParam1);
    }

    [Test]
    public void InvalidRemoveParamTest()
    {
        Method testMethod = new Method(validType, methodName1);
        NameTypeObject testParam1 = new NameTypeObject(validType, paramName1);
        Assert.Throws<AttributeNonexistentException>(delegate { testMethod.RemoveParam(testParam1.AttributeName);});
    }

    private string methodName1 = "";
    private string methodName2 = "";
    private string sameMethodName = "";
    private string invalidMethodName = "";
    private string validType = "";
    private string paramName1 = "";
    private string paramName2 = "";
    // ReSharper disable once NotAccessedField.Local
    private string sameParamName = "";
    private string invalidParamName = "";
}