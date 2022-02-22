using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UMLEditor.Classes;
using UMLEditor.Interfaces;

namespace UMLEditor.Views;

public class CommandLine
{
    private Diagram _activeDiagram;
    private IDiagramFile _activeFile;
    public static readonly ConsoleColor ERROR_COLOR = ConsoleColor.Red;

    public void runCLI()
    {
        _activeDiagram = new Diagram();
        _activeFile = new JSONDiagramFile();
        while (true)
        {
            Console.Write("Enter Command: ");
            try
            {
                ExecuteCommand(Console.ReadLine());
            }
            catch (Exception e)
            {
                PrintColoredLine(e.Message, ERROR_COLOR);
            }
        }
    }

    private void ExecuteCommand(string input)
    {
        //TODO The following is a good way to handle input for basic commands.  For commands that can have variable input, a differemt method, such as regular expressions, should be utilized for reading commandline input
         
        List<string> words = (input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();
        string command = words[0].ToLower();
        var arguments = words.Skip(1);
        switch (command)
        {
            case ("add_class"):
                if (arguments.Count() == 1)
                {
                    // Try to create a class with the first provided word as its name
                    _activeDiagram.AddClass(arguments.ElementAt(0));

                    Console.WriteLine($"Class {words[1]} created");
                }

                else
                {
                    PrintColoredLine("To create a new Class, please enter a class name " +
                                     "into the input box and then click press enter.", ERROR_COLOR);
                }

                break;

            case ("delete_class"):
                //TODO 
                break;

            case ("rename_class"):
                //TODO 
                break;

            case ("add_attribute"):
                //TODO 
                break;

            case ("delete_attribute"):
                //TODO 
                break;

            case ("rename_attribute"):
                //TODO 
                break;

            case ("list_classes"):
                //TODO 
                break;

            case ("list_attributes"):
                //TODO 
                break;

            case ("add_relationship"):
                //TODO 
                break;

            case ("delete_relationship"):
                //TODO 
                break;

            case ("list_relationships"):
                //TODO 
                break;

            case ("help"):
                PrintColoredLine("\nadd_class: Add a new class to the diagram" +
                                 "\ndelete_class: Delete an existing class" +
                                 "\nrename_class: Rename an existing class" +
                                 "\nadd_relationship: Add a relationship between classes" +
                                 "\ndelete_relationship: Delete an existing relationship" +
                                 "\nadd_attribute: Add an attribute to an existing Class" +
                                 "\ndelete_attribute: Delete an existing class attribute" +
                                 "\nrename_attribute: Rename an existing attribute" +
                                 "\nsave_diagram: Save your progress" +
                                 "\nload_diagram: Load a previously saved diagram" +
                                 "\nlist_classes: List all existing classes" +
                                 "\nlist_attributes: List all attributes of a class" +
                                 "\nlist_relationships: List all relationships of a class\n", ConsoleColor.Green);
                break;

            case ("save_diagram"):
                if (arguments.Count() == 1)
                {
                    string[] fileString = arguments.ElementAt(0)
                        .Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (fileString[1] != "json")
                    {
                        PrintColoredLine("Please use .json file extension", ERROR_COLOR);
                        break;
                    }

                    _activeFile.SaveDiagram(ref _activeDiagram, arguments.ElementAt(0));


                    Console.WriteLine($"Current diagram saved to {arguments.ElementAt(0)}");
                }

                else
                {
                    PrintColoredLine("Please provide a file to save the diagram to", ERROR_COLOR);
                }

                break;

            case ("load_diagram"):
                if (arguments.Count() == 1)
                {
                    string[] fileString = arguments.ElementAt(0)
                        .Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (fileString[1] != "json")
                    {
                        PrintColoredLine("Please provide a file with a .json file extension", ERROR_COLOR);
                        break;
                    }

                    _activeDiagram = _activeFile.LoadDiagram(arguments.ElementAt(0));


                    Console.WriteLine($"Diagram {arguments.ElementAt(0)} loaded.");
                }

                else
                {
                    PrintColoredLine("Please provide a file to load a diagram from", ERROR_COLOR);
                }

                break;

            case ("exit"):
                Environment.Exit(0);
                break;

            default:
                PrintColoredLine("Please provide a valid command", ERROR_COLOR);
                break;
        }
    }

    /// <summary>
    /// Prints the provided text in a specified color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="inColor">The color you want the text printed in.</param>
    private static void PrintColoredLine(string text, ConsoleColor inColor = ConsoleColor.White)
    {
        ConsoleColor oldTextColor = Console.ForegroundColor;

        Console.ForegroundColor = inColor;
        Console.WriteLine(text);

        Console.ForegroundColor = oldTextColor;
    }
}