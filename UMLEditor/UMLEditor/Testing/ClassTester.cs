using System;
using System.Diagnostics;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UMLEditor.Testing;

public static class ClassTester
{
    public static void RunTests()
    {
        // Make sure we can instantiate a default class
        Class newClass = new Class();
        TestingSled.PrintColoredLine("New default class constructed.", TestingSled.SUCCESS_COLOR);

        // Make sure we can instantiate a test class
        newClass = new Class("TestClass1");
        TestingSled.PrintColoredLine("New class TestClass1 created", TestingSled.SUCCESS_COLOR);
        
        // At this point, if no exceptions were thrown then we are okay
        // Make sure the name actually came through
        Debug.Assert(
            newClass.ClassName == "TestClass1",
            string.Format("New Class name was incorrect. " +
                          "Expected \"{0}\" but got \"{1}\"\n"
                , "TestClass1", newClass.ClassName)
        );
        TestingSled.PrintColoredLine("Tested fields of TestClass1", TestingSled.SUCCESS_COLOR);
        
        // Test creation of a class with invalid names
        try
        {

            Class invalidClass = new Class("%#0923");
            Debug.Fail("Class should reject invalid names on construction. Class did not reject %#0923");

        }

        catch (InvalidNameException)
        {
            
            TestingSled.PrintColoredLine("Tested new class with invalid name %#0923", TestingSled.SUCCESS_COLOR);
            
        }
        
        // Test renaming a class to an invalid name
        try
        {

            newClass.Rename("%#0923");
            Debug.Fail("Class should reject invalid names when renaming. Class did not reject %#0923");

        }

        catch (InvalidNameException)
        {
            
            TestingSled.PrintColoredLine("Tested renaming class with invalid name %#0923", TestingSled.SUCCESS_COLOR);
            
        }
        
        Class duplicateClass = new Class("TestClass1");
        TestingSled.PrintColoredLine("Tested new class with name in use TestClass1", TestingSled.SUCCESS_COLOR);
        
        // Test ToString function
        TestingSled.PrintColoredLine("Class toString: " + newClass.ToString(), TestingSled.SUCCESS_COLOR);
    }
}