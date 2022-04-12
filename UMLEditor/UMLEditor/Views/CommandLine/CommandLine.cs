using System;
using System.Diagnostics.CodeAnalysis;
using UMLEditor.Classes;
namespace UMLEditor.Views.CommandLine;

/// <summary>
/// CommandLine.cs
/// </summary>
public class CommandLine
{
    private CommandLineController _controller;
    private Diagram _diagram;
    private static readonly ConsoleColor ERROR_COLOR = ConsoleColor.Red;

    /// <summary>
    /// Default Constructor for CommandLine
    /// </summary>
    public CommandLine()
    {
        _controller = new CommandLineController();
        _diagram = new Diagram();
        TimeMachine.AddState(_diagram);
    }
    
    /// <summary>
    /// RunCLI function
    /// </summary>
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public void RunCli()
    {
        AutoCompleteHandler tabCompleteHandler = new AutoCompleteHandler();
        tabCompleteHandler.SupportedCommands = new[] 
        {
            "add_class", "delete_class", "rename_class", "add_field", 
            "delete_field", "rename_field", "change_field_type", "change_method_type", 
            "add_method", "delete_method", "add_parameter", "delete_parameter", "rename_method", 
            "rename_parameter", "replace_parameter", "list_classes", "list_attributes", 
            "add_relationship", "delete_relationship", "change_relationship_type", 
            "list_relationships", "help", "save_diagram", "load_diagram", "undo", "redo", "exit"
        };
        
        ReadLine.AutoCompletionHandler = tabCompleteHandler;
        while (true)
        {
            tabCompleteHandler.Diagram = _diagram;
            string input = ReadLine.Read("Enter Command: ");
            try
            {
                StringColorStruct result = _controller.ExecuteCommand(input, ref _diagram);
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