using System;
using System.Collections.Generic;

namespace UMLEditor.Classes;

/// <summary>
/// 
/// </summary>
public class AutoCompleteHandler
{
    /// <summary>
    /// 
    /// </summary>
    public char[] Seperators { get; set; } = new char[] { ' ' };

    /// <summary>
    /// 
    /// </summary>
    public string[] SupportCommands { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="forLine"></param>
    /// <returns></returns>
    public string[] GetSuggestions(int index, string forLine)
    {
        List<string> candidates = new List<string>();
        foreach (var command in SupportCommands)
        {
            if (IsLikelyCandidate(forLine, command))
            {
                candidates.Add(command);
            }
        }

        return candidates.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="toTest"></param>
    /// <param name="candidate"></param>
    /// <returns></returns>
    private bool IsLikelyCandidate(string toTest, string candidate)
    {
        return LastMatchingIndex(toTest, candidate) != -1;
    }
}

