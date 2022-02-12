using System;

namespace UMLEditor.Testing;

/// <summary>
/// A tool that will invoke the unit tests
/// </summary>
public static class TestingSled
{

    // Text colors for different statuses
    public static readonly ConsoleColor WARNING_COLOR = ConsoleColor.Yellow;
    public static readonly ConsoleColor SUCCESS_COLOR = ConsoleColor.Green;
    public static readonly ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    
    /// <summary>
    /// Invokes all unit tests
    /// </summary>
    public static void RunTests()
    {
        
        PrintColoredLine("Running unit tests...\n", WARNING_COLOR);

        // Test AttributeObject class
        ///////////////////////////////////////////////////////////////////////////////////////
        
        PrintColoredLine("Testing AttributeObject...", SUCCESS_COLOR);
        AttributeTester.RunTests();
        PrintColoredLine("\nAttributeObject has passed all tests\n", SUCCESS_COLOR);
        
        ///////////////////////////////////////////////////////////////////////////////////////
        
        // Test Class class
        ///////////////////////////////////////////////////////////////////////////////////////
        
        PrintColoredLine("Testing Class...", SUCCESS_COLOR);
        ClassTester.RunTests();
        PrintColoredLine("\nClass has passed all tests\n", SUCCESS_COLOR);
        
        ///////////////////////////////////////////////////////////////////////////////////////
        
        // Test Relationship class
        ///////////////////////////////////////////////////////////////////////////////////////
        
        PrintColoredLine("Testing Relationship...", SUCCESS_COLOR);
        RelationshipTester.RunTests();
        PrintColoredLine("\nRelationship has passed all tests\n", SUCCESS_COLOR);
        
        ///////////////////////////////////////////////////////////////////////////////////////
        
        // Test...
        
        PrintColoredLine("All tests passed", SUCCESS_COLOR);

    }

    /// <summary>
    /// Prints the provided text in a specified color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="inColor">The color you want the text printed in.</param>
    public static void PrintColoredLine(string text, ConsoleColor inColor = ConsoleColor.White)
    {

        ConsoleColor oldTextColor = Console.ForegroundColor;

        Console.ForegroundColor = inColor;
        Console.WriteLine(text);
        
        Console.ForegroundColor = oldTextColor;

    }
    
}