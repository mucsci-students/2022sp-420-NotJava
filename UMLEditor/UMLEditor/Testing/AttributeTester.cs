using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace UMLEditor.Testing;

using UMLEditor.Classes;

public static class AttributeTester
{
    
    /// <summary>
    /// Runs all tests for AttributeObject
    /// </summary>
    public static void RunTests()
    {
        
        string attribName = "TestAttrib";

        TestCreation(ref attribName);
        TestToString(ref attribName);
        TestRename(ref attribName);
        
    }

    private static void PrintSuccess(string format, params object[] args)
    {
        
        TestingSled.PrintColoredLine(string.Format(format, args), TestingSled.SUCCESS_COLOR);
        
    }
    
    private static void TestCreation(ref string attribName)
    {

        // Make sure we can instantiate a default attribute
        AttributeObject newAttrib = new AttributeObject();
        PrintSuccess("PASS: AttributeObject's default ctor works");

        // Make sure we can instantiate a named test attribute
        newAttrib = new AttributeObject("TestAttrib");
        
        // Make sure the name actually came through
        Debug.Assert(
            
            newAttrib.AttributeName.Equals("TestAttrib"), 
            string.Format("New Attribute name was incorrect. " +
                          "Expected \"{0}\" but got \"{1}\"", attribName, newAttrib.AttributeName)
            
        );
        PrintSuccess("PASS: AttributeObject's name ctor works");

    }
    
    private static void TestToString (ref string attribName)
    {
        
        string tostringFormat = "Attribute: {0}";
        string expectedToString = string.Format(tostringFormat, attribName);

        AttributeObject newAttrib = new AttributeObject(attribName);

        Debug.Assert(

            newAttrib.ToString().Equals(expectedToString),
            string.Format("Attribute's ToString is wrong. " +
                          "Expected {0} but got {1}", expectedToString, newAttrib.ToString())

        );
        PrintSuccess("PASS: AttributeObject's ToString works");

    }

    private static void TestRename(ref string attribName)
    {

        AttributeObject toTest = new AttributeObject(attribName);
        string newName = string.Format("{0}- RENAMED", attribName);
        
        toTest.AttRename(newName);
        
        // Make sure the new name is correct
        Debug.Assert(
        
            toTest.AttributeName.Equals(newName),
            string.Format("A renamed AttributeObject should adopt the new name. " +
                          "Expected {0} but was {1}", newName, toTest.AttributeName)
            
        );
        
    }
    
}