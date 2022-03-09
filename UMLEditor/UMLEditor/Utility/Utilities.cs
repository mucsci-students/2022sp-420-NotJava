namespace UMLEditor.Utility;

using System;
using System.Collections.Generic;

/// <summary>
/// Utilities 
/// </summary>
public class Utilities
{

    /// <summary>
    /// Clones a container to a new list
    /// </summary>
    /// <param name="toClone">The container to clone</param>
    /// <typeparam name="T">The type stored in the container. Must implement cloneable</typeparam>
    /// <returns>A list that contains clones of the elements in toClone</returns>
    public static List<T> CloneContainer<T>(IEnumerable<T> toClone) where T: ICloneable
    {
        List<T> result = new List<T>();

        foreach (var item in toClone)
        {
            result.Add((T)item.Clone());
        }
        
        return result;
    }

}