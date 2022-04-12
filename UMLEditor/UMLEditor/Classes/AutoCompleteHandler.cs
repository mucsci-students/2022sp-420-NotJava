using System;
using System.Collections.Generic;

namespace UMLEditor.Classes;

/// <summary>
/// Handles tab completion
/// </summary>
public class AutoCompleteHandler : IAutoCompleteHandler
{
    /// <summary>
    /// Separate different parts of list
    /// </summary>
    public char[] Separators { get; set; } = new char[] {' '};

    /// <summary>
    /// List of commands supported
    /// </summary>
    public string[] SupportedCommands { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Diagram property that is changed
    /// </summary>
    public Diagram? Diagram { get; set; }

    /// <summary>
    /// Checks if a string is a likely candidiate
    /// </summary>
    /// <param name="toTest">String to test</param>
    /// <param name="candidate">possible candidate to test against</param>
    /// <returns>Returns true if the candidate is a likely candidate</returns>
    private bool IsLikelyCandidate(string toTest, string candidate)
    {
        return candidate.StartsWith(toTest);
    }

    /// <summary>
    /// Gets suggestions for rest of command given partial typed command
    /// </summary>
    /// <param name="forLine">Partially completed command from user</param>
    /// <param name="index">Index of where user stopped typing</param>
    /// <returns>array of possible commands</returns>
    public string[] GetSuggestions(string forLine, int index)
    {
        List<string> candidates = new List<string>();

        foreach (var command in SupportedCommands)
        {
            if (IsLikelyCandidate(forLine, command))
            {
                candidates.Add(command);
            }

        }

        if (forLine.Contains("delete_class") 
            || forLine.Contains("rename_class") 
            || forLine.Contains("list_attributes")
            || forLine.Contains("add_field")
            || forLine.Contains("add_method"))
        {
            if (forLine.Split(' ').Length == 2)
            {
                return GetPossibleClasses(forLine);
            }
        }
        
        else if (forLine.Contains("add_relationship") || forLine.Contains("change_relationship_type"))
        {
            List<string> possibleCandidates = new List<string>();
            string[] splitLine = forLine.Split(' ');

            if (splitLine.Length is 2 or 3)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 4)
            {
                string[] relationshipTypes = Relationship.ValidTypes.ToArray();
                foreach (var relationshipType in relationshipTypes)
                {
                    if (IsLikelyCandidate(
                            splitLine[^1],
                            relationshipType))
                    {
                        possibleCandidates.Add(relationshipType);
                    }
                }

                return possibleCandidates.ToArray();
            }
        }

        else if (forLine.Contains("delete_relationship"))
        {
            string[] splitLine = forLine.Split(' ');

            if (splitLine.Length is 2 or 3)
            {
                return GetPossibleClasses(forLine);
            }
        }

        else if (forLine.Contains("delete_field") 
                 || forLine.Contains("change_field_type") 
                 || forLine.Contains("rename_field"))
        {
            string[] splitLine = forLine.Split(' ');
            if (splitLine.Length == 2)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 3)
            {
                return GetPossibleFields(forLine, splitLine[^2]);
            }
            
        }
        
        else if (forLine.Contains("delete_method")
                 || forLine.Contains("change_method_type")
                 || forLine.Contains("rename_method")
                 || forLine.Contains("add_parameter"))
        {
            string[] splitLine = forLine.Split(' ');
            if (splitLine.Length == 2)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 3)
            {
                return GetPossibleMethods(forLine, splitLine[^2]);
            }  
        }
        
        else if (forLine.Contains("delete_parameter")
                 || forLine.Contains("rename_parameter"))
        {
            string[] splitLine = forLine.Split(' ');
            if (splitLine.Length == 2)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 3)
            {
                return GetPossibleMethods(forLine, splitLine[^2]);
            }

            if (splitLine.Length == 4)
            {
                return GetPossibleParameters(forLine, splitLine[^3], splitLine[^2]);
            }
        }
        
        return candidates.ToArray();
    }

    
    /// <summary>
    /// Returns possible classes for tab completion.
    /// </summary>
    /// <param name="forLine">The command typed so far by the user.</param>
    /// <returns>An array of possible classes.</returns>
    private string[] GetPossibleClasses(string forLine)
    {
        List<string> possibleCandidates = new List<string>();
        foreach (var classObject in Diagram!.Classes)
        {
            if (IsLikelyCandidate(forLine.Split(' ')[^1],
                    classObject.ClassName))
            {
                possibleCandidates.Add(classObject.ClassName);
            }
        }

        return possibleCandidates.ToArray();
    }
    
    /// <summary>
    /// Returns possible fields for tab completion.
    /// </summary>
    /// <param name="forLine">The command typed so far by the user.</param>
    /// <param name="className">The class name to return the fields of.</param>
    /// <returns></returns>
    private string[] GetPossibleFields(string forLine, string className)
    {
        List<string> possibleCandidates = new List<string>();
        Class correctClass = GetClassByName(className);
        foreach (var fieldObject in correctClass.Fields)
        {
            if (IsLikelyCandidate(forLine.Split(' ')[^1],
                    fieldObject.AttributeName))
            {
                possibleCandidates.Add(fieldObject.AttributeName);
            }
        }

        return possibleCandidates.ToArray();
    }

    /// <summary>
    /// Returns possible methods for tab completion.
    /// </summary>
    /// <param name="forLine">The command typed so far by the user.</param>
    /// <param name="className">The class name to return the methods of.</param>
    /// <returns>The possible methods.</returns>
    private string[] GetPossibleMethods(string forLine, string className)
    {
        List<string> possibleCandidates = new List<string>();
        Class correctClass = GetClassByName(className);
        foreach (var fieldObject in correctClass.Methods)
        {
            if (IsLikelyCandidate(forLine.Split(' ')[^1],
                    fieldObject.AttributeName))
            {
                possibleCandidates.Add(fieldObject.AttributeName);
            }
        }

        return possibleCandidates.ToArray();
    }

    /// <summary>
    /// Returns possible parameters for tab completion
    /// </summary>
    /// <param name="forLine">The command typed so far by the user.</param>
    /// <param name="className">The class that the method is contained in.</param>
    /// <param name="methodName">The method name to return the parameters of.</param>
    /// <returns>The possible parameters.</returns>
    private string[] GetPossibleParameters(string forLine, string className, string methodName)
    {
        Class correctClass = GetClassByName(className);

        Method method = GetMethodByName(correctClass, methodName);
        
        List<string> possibleCandidates = new List<string>();
        foreach (var parameter in method.Parameters)
        {
            if (IsLikelyCandidate(forLine.Split(' ')[^1], parameter.AttributeName))
            {
                possibleCandidates.Add(parameter.AttributeName);
            }
        }

        return possibleCandidates.ToArray();
    }

    /// <summary>
    /// Returns class object given name of class.
    /// </summary>
    /// <param name="className">String name of class.</param>
    /// <returns>Class object of the class.</returns>
    private Class GetClassByName(string className)
    {
        Class correctClass = new Class();
        foreach (var classObject in Diagram!.Classes)
        {
            if (classObject.ClassName == className)
            {
                correctClass = classObject;
                break;
            }
        }

        return correctClass;
    }
    
    /// <summary>
    /// Returns method object given class object and method name
    /// </summary>
    /// <param name="classObject">The class to get the method from.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>Method object of the method.</returns>
    private Method GetMethodByName(Class classObject, string methodName)
    {
        Method method = new Method();
        foreach (var methodObject in classObject.Methods)
        {
            if (methodObject.AttributeName == methodName)
            {
                method = methodObject;
                break;
            }
        }

        return method;
    }
}

