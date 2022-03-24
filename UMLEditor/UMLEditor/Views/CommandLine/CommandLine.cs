using System;
using System.Diagnostics.CodeAnalysis;

namespace UMLEditor.Views.CommandLine;

/// <summary>
/// CommandLine.cs
/// </summary>
public class CommandLine
{
    private CommandLineController? _controller;
    private static readonly ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    
    /// <summary>
    /// RunCLI function
    /// </summary>
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public void RunCli()
    {
        _controller = new CommandLineController();
        
        
        while (true)
        {
            Console.Write("Enter Command: ");
            try
            {
                StringColorStruct result = _controller.ExecuteCommand(Console.ReadLine()!);
                PrintColoredLine(result.Output, result.OutColor);
            }
            catch (Exception e)
            {
                PrintColoredLine(e.Message, ERROR_COLOR);
            }
        }
    }

    /// <summary>
    /// Prints the provided text in a specified color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="inColor">The color you want the text printed in.</param>
    private static void PrintColoredLine(string text, ConsoleColor inColor = ConsoleColor.White)
    {
        if (text.Equals(""))
        {
            return;
        }
        ConsoleColor oldTextColor = Console.ForegroundColor;

        Console.ForegroundColor = inColor;
        Console.WriteLine(text);

        Console.ForegroundColor = oldTextColor;
    }
    
    
}