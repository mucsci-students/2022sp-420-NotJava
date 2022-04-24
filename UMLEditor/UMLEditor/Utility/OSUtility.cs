using System;
using System.Runtime.InteropServices;

namespace UMLEditor.Utility;

/// <summary>
/// A utilities class for getting OS related info
/// </summary>
public static class OSUtility
{

    /// <summary>
    /// Indicates whether or not the program is running under Linux
    /// </summary>
    public static bool IsLinux
    {
        
        get => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        
    }
    
    /// <summary>
    /// Indicates whether or not the program is running under macOS
    /// </summary>
    public static bool IsMacos
    {
        
        get => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        
    }
    
    /// <summary>
    /// Indicates whether or not the program is running under Windows
    /// </summary>
    public static bool IsWindows
    {
        
        get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        
    }

    /// <summary>
    /// Gets the home folder for the user that the program is running under
    /// </summary>
    public static string HomeFolder
    {

        get
        {

            string? result = Environment.GetEnvironmentVariable((IsWindows ? "USERPROFILE" : "HOME"));
            if (result is null)
            {
                throw new InvalidOperationException("Unable to determine the home folder location");
            }

            // Return the path with a "\" or "/" tacked to the end
            return $"{result}{(IsWindows ? "\\" : "/")}";

        }
        
    }
    
}