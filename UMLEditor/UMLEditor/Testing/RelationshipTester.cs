using System;
using System.Diagnostics;

namespace UMLEditor.Testing;

using UMLEditor.Classes;

public class RelationshipTester
{
    public static void RunTests()
    {
        // Make sure we can instantiate a default relationship
        Relationship newRelation = new Relationship();
        TestingSled.PrintColoredLine("New default relationship constructed.", TestingSled.SUCCESS_COLOR);

        // Create new classes for testing
        Class class1 = new Class("Test1");
        Class class2 = new Class("Test2");
        
        // Make sure we can instantiate a test relationship
        newRelation = new Relationship("Test1", "Test2");
        TestingSled.PrintColoredLine("New relationship between Test1 and Test2 created", TestingSled.SUCCESS_COLOR);
        
        // At this point, if no exceptions were thrown then we are okay
        // Make sure the name actually came through
        Debug.Assert(
            newRelation.SourceClass == "Test1" && newRelation.DestinationClass == "Test2",
            string.Format("New Relationship classes were incorrect. " +
                          "Expected \"{0}\" and \"{1}\" but got \"{2}\" and \"{3}\"\n"
                , class1.ClassName, class2.ClassName, newRelation.SourceClass, newRelation.DestinationClass)
        );
        TestingSled.PrintColoredLine("Tested fields of relationship\n", TestingSled.SUCCESS_COLOR);
        
        // Test creation of a relationship with nonexistent classes
        newRelation = new Relationship("Test1", "Test3");
        TestingSled.PrintColoredLine("Testing new relationship with nonexistent Test3 class", TestingSled.SUCCESS_COLOR);
        
        TestingSled.PrintColoredLine("Relationship toString: " + newRelation.ToString(), TestingSled.SUCCESS_COLOR);
    }
}