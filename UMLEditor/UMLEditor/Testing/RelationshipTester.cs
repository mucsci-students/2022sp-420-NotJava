using System;
using System.Diagnostics;

namespace UMLEditor.Testing;

using UMLEditor.Classes;

public class RelationshipTester
{
    public static void TestCreation()
    {
        // Make sure we can instantiate a default relationship
        Relationship newRelation = new Relationship();
        Console.Write("New default relationship constructed.\n");

        // Create new classes for testing
        Class class1 = new Class("Test1");
        Class class2 = new Class("Test2");
        
        // Make sure we can instantiate a test relationship
        newRelation = new Relationship("Test1", "Test2");
        Console.Write("New relationship between Test1 and Test2 created\n");
        
        // At this point, if no exceptions were thrown then we are okay
        // Make sure the name actually came through
        Debug.Assert(
            newRelation.SourceClass == "Test1" && newRelation.DestinationClass == "Test2",
            string.Format("New Relationship classes were incorrect. " +
                          "Expected \"{0}\" and \"{1}\" but got \"{2}\" and \"{3}\"\n"
                , class1.ClassName, class2.ClassName, newRelation.SourceClass, newRelation.DestinationClass)
        );
        Console.Write("Tested fields of relationship\n");
        
        // Test creation of a relationship with nonexistent classes
        newRelation = new Relationship("Test1", "Test3");
        Console.Write("Testing new relationship with nonexistent Test3 class\n");
        
        Console.Write("Relationship toString: " + newRelation.ToString() + "\n");
    }
}