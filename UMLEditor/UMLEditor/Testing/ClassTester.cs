using System;
using System.Diagnostics;
using UMLEditor.Classes;

namespace UMLEditor.Testing;

public static class ClassTester
{
    public static void TestCreation()
    {
        Console.Write("Testing Class: \n");
        // Make sure we can instantiate a default class
        Class newClass = new Class();
        Console.Write("New default class constructed.\n");

        // Make sure we can instantiate a test class
        newClass = new Class("TestClass1");
        Console.Write("New class TestClass1 created\n");
        
        // At this point, if no exceptions were thrown then we are okay
        // Make sure the name actually came through
        Debug.Assert(
            newClass.ClassName == "TestClass1",
            string.Format("New Class name was incorrect. " +
                          "Expected \"{0}\" but got \"{1}\"\n"
                , "TestClass1", newClass.ClassName)
        );
        Console.Write("Tested fields of TestClass1\n");
        
        // Test creation of a class with invalid names
        Class invalidClass = new Class("%#0923");
        Console.Write("Tested new class with invalid name %#0923\n");
        Class duplicateClass = new Class("TestClass1");
        Console.Write("Tested new class with name in use TestClass1\n");
        
        // Test ToString function
        Console.Write("Class toString: " + newClass.ToString() + "\n");
        
        // Test Attributes
        AttributeTester.TestCreation();
    }
}