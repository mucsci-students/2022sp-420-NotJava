using System;


namespace UMLEditor.Views.CommandLine;

/// <summary>
/// Structure that contains output string and desired color for output string.
/// This is used for the Commandline View
/// </summary>
public struct StringColorStruct
{
    /// <summary>
    /// Output string
    /// </summary>
    public string Output { get; private set; }
    
    /// <summary>
    /// Output color
    /// </summary>
    public ConsoleColor OutColor { get; private set; }

    /// <summary>
    /// Constructor for StringColorStruct
    /// </summary>
    /// <param name="str">string</param>
    /// <param name="col">desired color of string</param>
    public StringColorStruct(string str, ConsoleColor col = ConsoleColor.White)
    {
        Output = str;
        OutColor = col;
    }

}