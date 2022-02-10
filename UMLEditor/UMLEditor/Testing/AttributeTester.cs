using System;
using System.Diagnostics;

namespace UMLEditor.Testing;

using UMLEditor.Classes;

public static class AttributeTester
{

    public static void TestCreation()
    {

        // Make sure we can instantiate a default attribute
        AttributeObject newAttrib = new AttributeObject();

        string attribName = "TestAttrib";
        
        // Make sure we can instantiate a test attribute
        newAttrib = new AttributeObject("TestAttrib");
        
        // At this point, if no exceptions were thrown then we are okay
        // Make sure the name actually came through
        Debug.Assert(
            
            newAttrib.AttributeName == "TestAttrib", 
            string.Format("New Attribute name was incorrect. " +
                          "Expected \"{0}\" but got \"{1}\"", attribName, newAttrib.AttributeName)
            
        );

        
        
    }
    
}