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


        if (forLine.Contains("add_relationship") || forLine.Contains("change_relationship_type"))
        {
            List<string> possibleCandidates = new List<string>();
            string[] splitLine = forLine.Split(' ');

            if (splitLine.Length == 2 || splitLine.Length == 3)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 4)
            {
                string[] relationshipTypes = Relationship.ValidTypes.ToArray();
                foreach (var relationshipType in relationshipTypes)
                {
                    if (IsLikelyCandidate(
                            splitLine[ /*possibly replace with reverse index operator*/ splitLine.Length - 1],
                            relationshipType))
                    {
                        possibleCandidates.Add(relationshipType);
                    }
                }

                return possibleCandidates.ToArray();
            }
        }

        if (forLine.Contains("delete_relationship"))
        {
            string[] splitLine = forLine.Split(' ');

            if (splitLine.Length == 2 || splitLine.Length == 3)
            {
                return GetPossibleClasses(forLine);
            }
        }

        if (forLine.Contains("delete_field"))
        {
            string[] splitLine = forLine.Split(' ');
            if (splitLine.Length == 2)
            {
                return GetPossibleClasses(forLine);
            }

            if (splitLine.Length == 3)
            {
                return GetPossibleFields(forLine, splitLine[ /*possibly replace with reverse index operator*/
                    splitLine.Length - 2]);
            }
            
        }

        return candidates.ToArray();
    }

    
    /// <summary>
    /// Returns possible classes for tab completion.
    /// </summary>
    /// <param name="forLine">The command typed so far by the user.</param>
    /// <returns>An array of possible classes.</returns>
    public string[] GetPossibleClasses(string forLine)
    {
        string[] splitLine = forLine.Split(' ');
        List<string> possibleCandidates = new List<string>();
        foreach (var classObject in Diagram!.Classes)
        {
            if (IsLikelyCandidate(splitLine[ /*possibly replace with reverse index operator*/ splitLine.Length - 1],
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
    public string[] GetPossibleFields(string forLine, string className)
    {
        List<string> possibleCandidates = new List<string>();
        Class correctClass = new Class();
        foreach (var classObject in Diagram!.Classes)
        {
            if (classObject.ClassName == className)
            {
                correctClass = classObject;
                break;
            }
        }
        
        foreach (var fieldObject in correctClass.Fields)
        {
            if (IsLikelyCandidate(forLine.Split(' ')[forLine.Split(' ').Length - 1],
                    fieldObject.AttributeName))
            {
                possibleCandidates.Add(fieldObject.AttributeName);
            }
        }

        return possibleCandidates.ToArray();
    }
}


