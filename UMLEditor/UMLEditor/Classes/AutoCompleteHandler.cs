using System;
using System.Collections.Generic;

namespace UMLEditor.Classes;

/// <summary>
/// 
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
    /// Gets suggestions given what user has typed
    /// </summary>
    /// <param name="index">index of where user typed up to in string</param>
    /// <param name="forLine">Line to attempt to complete</param>
    /// <returns></returns>
    public string[] GetSuggestions(int index, string forLine)
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

    /// <summary>
    /// Returns the last matching index of a string
    /// </summary>
    /// <param name="first">The first string</param>
    /// <param name="second">The second string</param>
    /// <returns>The last matching index</returns>
    private int LastMatchingIndex(string first, string second)
    {
        int result = -1;

        int lastIndex = Math.Min(first.Length, second.Length);
        for (int index = 0; index < lastIndex; ++index)
        {
            if (first[index] == second[index])
            {
                ++result;
            }
            else
            {
                break;
            }
        }

        return result;
    }
    
    /// <summary>
    /// Checks if a string is a likely candidiate
    /// </summary>
    /// <param name="toTest">String to test</param>
    /// <param name="candidate">possible candidate to test against</param>
    /// <returns>Returns true if the candidate is a likely candidate</returns>
    private bool IsLikelyCandidate(string toTest, string candidate)
    {
        return LastMatchingIndex(toTest, candidate) != -1;
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

