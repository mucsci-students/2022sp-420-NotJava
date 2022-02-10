using System;
using System.Diagnostics;
using UMLEditor.Classes;

namespace UMLEditor.Testing;

public static class ClassTester
{
    public static void runTests()
    {
        Console.WriteLine("Running Class Tests");
        createClassTest();
    }

    private static void createClassTest()
    {
        Diagram diagram = new Diagram();

        string goodClassName = "_TestClass1";
        diagram.AddClass(goodClassName);
        
        Debug.Assert(diagram.ClassExists(goodClassName), 
            String.Format("Class {0} add unsuccessful", goodClassName));

        string badClassName = "123BadClassName";
        diagram.AddClass(badClassName);
        
        Debug.Assert(!diagram.ClassExists(badClassName), 
                        String.Format("Class {0} with improper name was added, this is bad.", badClassName));
    }
}