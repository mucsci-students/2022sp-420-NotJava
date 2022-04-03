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
    public char[] Separators { get; set; } = new char[] { ' ' };

    /// <summary>
    /// List of commands supported
    /// </summary>
    public string[] SupportedCommands { get; set; } = Array.Empty<string>();
    
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
            
        return candidates.ToArray();
    }
    
}

