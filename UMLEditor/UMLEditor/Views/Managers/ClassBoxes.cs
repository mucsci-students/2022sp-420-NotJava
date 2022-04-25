using System;
using System.Collections.Generic;

namespace UMLEditor.Views.Managers;

/// <summary>
/// Keeps tabs on rendered ClassBoxes. Allows accessing by class name. 
/// </summary>
public static class ClassBoxes
{

    /// <summary>
    /// Maintains a mapping of names -> ClassBoxes
    /// </summary>
    private static readonly Dictionary<string, ClassBox> Register = new();

    /// <summary>
    /// Allows creating a new registration for a ClassBox
    /// </summary>
    /// <param name="toRegister">The ClassBox to register</param>
    /// <exception cref="InvalidOperationException">If a registration for a ClassBox of the same class name exists</exception>
    public static void RegisterClassBox(ClassBox toRegister)
    {

        string className = toRegister.ClassName;
        RegisterClassBox(className, toRegister);

    }

    /// <summary>
    /// Allows creating a new registration for a ClassBox
    /// </summary>
    /// <param name="className">The name to make this registration under</param>
    /// <param name="toRegister">The ClassBox to register</param>
    /// <exception cref="InvalidOperationException">If a registration for a ClassBox of the same class name exists</exception>
    private static void RegisterClassBox(string className, ClassBox toRegister)
    {
        
        // Disallow duplicate registrations 
        if (Register.ContainsKey(className))
        {
            throw new InvalidOperationException($"A registration for the ClassBox for {className} already exists");
        }
        
        // Register the ClassBox
        Register[className] = toRegister;
        
    }

    /// <summary>
    /// Deletes the ClassBox registration for the class by the provided ClassBox
    /// </summary>
    /// <param name="toUnregister">The ClassBox to unregister</param>
    /// <returns>True if a ClassBox for className was registered and removed, false otherwise</returns>
    public static bool UnregisterClassBox(ClassBox toUnregister) => UnregisterClassBox(toUnregister.ClassName);
    
    /// <summary>
    /// Deletes the ClassBox registration for the class by the provided name
    /// </summary>
    /// <param name="className">The name of the class to unregister</param>
    /// <returns>True if a ClassBox for className was registered and removed, false otherwise</returns>
    private static bool UnregisterClassBox(string className)
    {
        
        // Remove the registration
        return Register.Remove(className);

    }

    /// <summary>
    /// Updates the name for the binding for a ClassBox 
    /// </summary>
    /// <param name="oldName">The old name for the binding of the ClassBox</param>
    /// <param name="newName">The new name for the binding of the ClassBox</param>
    /// <exception cref="InvalidOperationException">If no binding exists under oldName
    /// or a binding already exists under newName</exception>
    public static void UpdateRegistration(string oldName, string newName)
    {

        // Ensure that a binding for the old name exists
        if (!Register.ContainsKey(oldName))
        {
            throw new InvalidOperationException($"A binding for the class {oldName} does not exist");
        }
        
        // Ensure that a binding for the new name does not exist
        if (!Register.ContainsKey(oldName))
        {
            throw new InvalidOperationException($"A binding for the class {newName} already exists");
        }
            
        // Grab the ClassBox behind the old name
        var target = Register[oldName];
        
        UnregisterClassBox(oldName);
        RegisterClassBox(newName, target);

    }

    /// <summary>
    /// Gets a registered ClassBox by its name. Requires a registration to exist under the provided name.
    /// </summary>
    /// <param name="bindingName">The class name that the ClassBox is registered under</param>
    /// <returns>The matching ClassBox for the binding</returns>
    /// <exception cref="InvalidOperationException">If a binding by bindingName does not exist</exception>
    public static ClassBox FindByName(string bindingName)
    {

        ClassBox result = Register[bindingName];
        if (result is not null)
        {
            return result;
        }

        throw new InvalidOperationException($"A binding for a ClassBox for {bindingName} does not exist");

    }

    /// <summary>
    /// Removes all registrations this manager tracks
    /// </summary>
    public static void UnregisterAll() => Register.Clear();

}