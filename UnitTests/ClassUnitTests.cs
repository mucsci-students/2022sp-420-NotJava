using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class ClassUnitTests
{
    
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
        newClass.ChangeLocation(100, 100);

        Assert.AreEqual(100, newClass.ClassLocation.X);
        Assert.AreEqual(100, newClass.ClassLocation.Y);
    }

    [Test]
    public void RenameClassToInvalidClassTest()
    {
        Class c = new Class("TestClass");
        Assert.Throws<InvalidNameException>(delegate { c.Rename("%#0923"); });
    }

    [Test]
    public void AddFieldTest()
    {
        Class newClass = new Class("TestClass");
        newClass.AddField("String", "TestField");

        Assert.AreEqual(true, newClass.FieldExists("TestField"));
        
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.AddField("Double","TestField"); });

    }
    
    [Test]
    public void FieldExistsTest()
    {
        Class newClass = new Class("TestClass");
        newClass.AddField("String", "TestField");
        
        Assert.AreEqual(true, newClass.FieldExists("TestField"));
        Assert.AreEqual(true, newClass.FieldExists(new NameTypeObject("String", "TestField")));
        
        Assert.AreEqual(false, newClass.FieldExists("TestFieldOrange"));
        Assert.AreEqual(false, newClass.FieldExists(new NameTypeObject("String", "TestFieldOrange")));
    }
    
    [Test]
    public void DeleteFieldTest()
    {
        Class newClass = new Class("TestClass");
        newClass.AddField("String", "TestField");
        
        Assert.AreEqual(true, newClass.FieldExists("TestField"));
        newClass.DeleteField("TestField");
        Assert.AreEqual(false, newClass.FieldExists("TestField"));
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.DeleteField("TestField"); });
    }
    
    [Test]
    public void ReplaceFieldTest()
    {
        Class newClass = new Class("TestClass");
        newClass.AddField("String", "TestField");
        newClass.AddField("String", "Pineapple3");
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ReplaceField (
            new NameTypeObject("String", "Pineapple"), 
            new NameTypeObject("String", "Pineapple2")); });
        
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.ReplaceField (
                new NameTypeObject("String", "TestField"),
                new NameTypeObject("String", "Pineapple3")); });
        
        newClass.ReplaceField(new NameTypeObject("String", "TestField"),
            new NameTypeObject("String", "ThisWorked"));
        
        Assert.AreEqual(true, newClass.FieldExists("ThisWorked"));
    }
    
    [Test]
    public void DeleteMethodParamTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("Orange", "paramName2");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};

        Class newClass = new Class("TestClass");
        
        newClass.AddMethod("String", "TestMethod", testParams);
        
        newClass.DeleteMethodParameter("paramName1", "TestMethod");
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.DeleteMethodParameter("paramName1", "TestMethodOrange"); });
        
        // To test that the delete was successful without making getMethodByName public, we have to look for a AttributeNonexistentException here.
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.DeleteMethodParameter("paramName1", "TestMethod"); });
    }
    
    [Test]
    public void ListAttributesTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "Name");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1};

        Class newClass = new Class("TestClass");
        Class newClassEmpty = new Class("TestClassEmpty");
        
        newClass.AddField("String", "B");
        newClass.AddMethod("Double", "C");
        newClass.AddMethod("Int", "Pineapple", testParams);

        string master = "TestClass fields: \n"
                        + "    String B\n"
                        + "TestClass methods: \n"
                        + "    Double C ()\n"
                        + "    Int Pineapple (String Name)\n";

        string masterEmpty = "TestClassEmpty fields: \n"
                             + "    There are no fields currently. \n"
                             + "TestClassEmpty methods: \n"
                             + "    There are no methods currently. \n";
        
        Assert.AreEqual(master, newClass.ListAttributes());
        Assert.AreEqual(masterEmpty, newClassEmpty.ListAttributes());
    }
    
    [Test]
    public void RenameFieldTest()
    {
        Class newClass = new Class("TestClass");

        newClass.AddField("String", "B");
        newClass.AddField("Double", "C");
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.RenameField("A","B"); });
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.RenameField("B","C"); });
        
        newClass.RenameField("B", "Pineapple");
        
        Assert.AreEqual(true, newClass.FieldExists("Pineapple"));
    }
    
    [Test]
    public void ChangeFieldTypeTest()
    {
        Class newClass = new Class("TestClass");
        string master = "TestClass fields: \n"
                        + "    Double B\n";

        newClass.AddField("String", "B");
        newClass.ChangeFieldType("B", "Double");

        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ChangeFieldType("A","Pineapple"); });
        
        Assert.AreEqual(master, newClass.ListFields());
    }
    
    [Test]
    public void ChangeMethodTypeTest()
    {
        Class newClass = new Class("TestClass");
        string master = "TestClass methods: \n"
                        + "    Double B ()\n";

        newClass.AddMethod("String", "B");
        newClass.ChangeMethodType("B", "Double");

        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ChangeMethodType("A","Pineapple"); });
        
        Assert.AreEqual(master, newClass.ListMethods());
    }
    
    [Test]
    public void RenameMethodTest()
    {
        Class newClass = new Class("TestClass");

        newClass.AddMethod("String", "B");
        newClass.AddMethod("Double", "C");
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.RenameMethod("A","B"); });
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.RenameMethod("B","C"); });
        
        newClass.RenameMethod("B", "Pineapple");
        
        Assert.AreEqual(true, newClass.MethodExists("Pineapple"));
    }
    
    
    [Test]
    public void ChangeMethodNameTypeTest()
    {
        Class newClass = new Class("TestClass");

        newClass.AddMethod("String", "B");
        newClass.AddMethod("Double", "C");
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ChangeMethodNameType(
            "A",new NameTypeObject("String", "Pineapple")); });
        
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.ChangeMethodNameType(
            "B",new NameTypeObject("String", "C")); });

        newClass.ChangeMethodNameType(
            "B", new NameTypeObject("String", "ThisWorked"));
        
        Assert.AreEqual(true, newClass.MethodExists("ThisWorked"));
    }
    
    [Test]
    public void RenameMethodParamTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("Orange", "paramName2");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};

        Class newClass = new Class("TestClass");

        string master = "TestClass methods: \n"
            + "    String TestMethod (String Pineapple, Orange paramName2)\n";
        
        newClass.AddMethod("String", "TestMethod", testParams);
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.RenameMethodParameter("Pineapple", "paramName1", "ParamName3"); });
        
        newClass.RenameMethodParameter("TestMethod", "paramName1","Pineapple");
        
        Assert.AreEqual(master, newClass.ListMethods());
    }
    
    [Test]
    public void ReplaceMethodParamTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("Orange", "paramName2");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};

        Class newClass = new Class("TestClass");

        string master = "TestClass methods: \n"
                        + "    String TestMethod (String ThisWorked, Orange paramName2)\n";
        
        newClass.AddMethod("String", "TestMethod", testParams);
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ReplaceParameter("Pineapple", "paramName1", new NameTypeObject("Orange", "paramName3")); });
        
        newClass.ReplaceParameter("TestMethod", "paramName1",new NameTypeObject("String", "ThisWorked"));
        
        Assert.AreEqual(master, newClass.ListMethods());
    }
    
    [Test]
    public void ClearParametersTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("Orange", "paramName2");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};

        Class newClass = new Class("TestClass");

        string master = "TestClass methods: \n"
                        + "    String TestMethod ()\n";
        
        newClass.AddMethod("String", "TestMethod", testParams);
        
        Assert.Throws<AttributeNonexistentException>(delegate { newClass.ClearParameters("Pineapple"); });
        
        newClass.ClearParameters("TestMethod");
        
        Assert.AreEqual(master, newClass.ListMethods());
    }
    
    [Test]
    public void AddParameterTest()
    {
        Class newClass = new Class("TestClass");

        string master = "TestClass methods: \n"
                        + "    String TestMethod (Int TestParam1)\n";
        
        newClass.AddMethod("String", "TestMethod");
        
        Assert.Throws<AttributeNonexistentException>(
            delegate { newClass.AddParameter("Pineapple", "Int", "TestParam1"); });
        
        newClass.AddParameter("TestMethod","Int", "TestParam1");
        
        Assert.AreEqual(master, newClass.ListMethods());
    }
    
    [Test]
    public void AddMethodTest()
    {
        NameTypeObject testParam1 = new NameTypeObject("String", "paramName1");
        NameTypeObject testParam2 = new NameTypeObject("Orange", "paramName2");
        NameTypeObject testParam3 = new NameTypeObject("Orange", "paramName2");
        List<NameTypeObject> testParams = new List<NameTypeObject>{testParam1, testParam2};
        List<NameTypeObject> testParams2 = new List<NameTypeObject>{testParam2, testParam3};
        
        Class newClass = new Class("TestClass");
        newClass.AddMethod("String", "TestMethod", testParams);

        Assert.AreEqual(true, newClass.MethodExists("TestMethod"));
        
        Assert.Throws<AttributeAlreadyExistsException>(delegate { newClass.AddMethod("Double","TestMethod", testParams); });
        
        Assert.Throws<DuplicateNameException>(delegate { newClass.AddMethod("Double","TestMethodDupParam", testParams2); });
    }
    
    [Test]
    public void GetterTests()
    {
        Class newClass = new Class("TestClass");

        newClass.AddMethod("String", "TestMethod");
        newClass.AddField("String", "TestField");
        
        Assert.AreEqual(1, newClass.Methods.Count);
        Assert.AreEqual(1, newClass.Fields.Count);
        
    }
    
}