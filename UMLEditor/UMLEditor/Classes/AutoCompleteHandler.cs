using System;
using System.Collections.Generic;
using System.Linq;

namespace UMLEditor.Classes;

/// <summary>
/// Handles tab completion
/// </summary>
public class AutoCompleteHandler : IAutoCompleteHandler
{
    /// <summary>
    /// Separate different parts of list
    /// </summary>
    public char[] Separators { get; set; } = new char[] { ' ' };

    /// <summary>
    /// List of commands supported
    /// </summary>
    public string[] SupportedCommands { get; set; } = Array.Empty<string>();

    public string[] classes { get; set; } = Array.Empty<string>();
    public string[] fields { get; set; } = Array.Empty<string>();
    public string[] methods { get; set; } = Array.Empty<string>();

    public Diagram diagram { get; set; }

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

        if (forLine.Contains("delete_class"))
        {
            string[] listClasses = diagram.ListClasses().Split('\n');
            List<string> possibleClasses = new List<string>();
            foreach (var className in listClasses)
            {
                if (IsLikelyCandidate(forLine.Split(' ')[1], className))
                {
                    possibleClasses.Add(className);
                }
            }
            return possibleClasses.ToArray();
        }

        if (forLine.Contains("add_relationship"))
        {
            List<string> possibleCandidates = new List<string>();
            string[] splitLine = forLine.Split(' ');

            if (splitLine.Length == 2 || splitLine.Length == 3)
            {
                string[] listClasses = diagram.ListClasses().Split('\n');
                foreach (var className in listClasses)
                {
                    if (IsLikelyCandidate(splitLine[/*possibly replace with reverse index operator*/ splitLine.Length - 1], className))
                    {
                        possibleCandidates.Add(className);
                    }
                }
                return possibleCandidates.ToArray();   
            }

            if (splitLine.Length == 4)
            {
                string[] relationshipTypes = Relationship.ValidTypes.ToArray();
                foreach (var relationshipType in relationshipTypes)
                {
                    if (IsLikelyCandidate(splitLine[/*possibly replace with reverse index operator*/ splitLine.Length - 1], relationshipType))
                    {
                        possibleCandidates.Add(relationshipType);
                    }
                }
                return possibleCandidates.ToArray();
            }
        }
            
        return candidates.ToArray();
    }
    
}

