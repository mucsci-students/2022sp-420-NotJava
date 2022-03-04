﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DynamicData;
using UMLEditor.Classes;
using UMLEditor.Exceptions;
using UMLEditor.Interfaces;

namespace UMLEditor.Views;

public class CommandLine
{
    private Diagram? _activeDiagram;
    private IDiagramFile? _activeFile;
    private static readonly ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    private static readonly ConsoleColor SUCCESS_COLOR = ConsoleColor.DarkGreen;
    
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public void RunCli()
    {
        _activeDiagram = new Diagram();
        _activeFile = new JSONDiagramFile();
        while (true)
        {
            Console.Write("Enter Command: ");
            try
            {
                ExecuteCommand(Console.ReadLine()!);
            }
            catch (Exception e)
            {
                PrintColoredLine(e.Message, ERROR_COLOR);
            }
        }
    }

    private void ExecuteCommand(string input)
    {
        //TODO if we want to seperate the view and the controller, put this function in a controller class and have it return a struct that contains a string message and a console color.

        // In the case no input was provided, do nothing
        if (input.Length == 0)
        {
            return;
        }

        List<string> words = (input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();
        string command = words[0].ToLower();
        List<string> arguments = words.Skip(1).ToList();
        switch (command)
        {
            case ("add_class"):
                if (arguments.Count == 1)
                {
                    // Try to create a class with the first provided word as its name
                    _activeDiagram!.AddClass(arguments[0]);

                    PrintColoredLine($"Class '{words[1]}' created", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To create a new Class, please enter \"add_class\" " +
                                     "followed by a class name into the console and then press enter.", ERROR_COLOR);
                }

                break;

            
            case ("delete_class"):
                if (arguments.Count == 1)
                {
                    // Try to Delete a Class with the first provided word as its name
                    _activeDiagram!.DeleteClass(arguments[0]);

                    PrintColoredLine($"Class '{arguments[0]}' deleted.", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To delete an existing Class, please enter \"delete_class\" " +
                                     "followed by a class name into the console and then press enter.", ERROR_COLOR);
                }
                break;
                
            case ("rename_class"):
                if (arguments.Count == 2)
                {
                    // Try to Rename a Class with the first provided word as its name
                    _activeDiagram!.RenameClass(arguments[0],  arguments[1]);

                    PrintColoredLine($"Class '{arguments[0]}' renamed to '{arguments[1]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To rename a Class, please enter \"rename_class\" " +
                                     "followed by the class to rename and then the new name.", ERROR_COLOR);
                }
                break;

            case ("add_field"):
                if (arguments.Count == 3)
                {
                    _activeDiagram!.AddField(arguments[0], arguments[1], arguments[2]);
                    
                    PrintColoredLine($"Field '{arguments[2]}' of type '{arguments[1]}' " +
                                     $"created in Class '{arguments[0]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To create a new Field, please enter \"add_field\" " +
                                     "followed by class name, field type and field name into the" +
                                     " console and then press enter.", ERROR_COLOR);
                }
                break;

            case ("delete_field"):
                if (arguments.Count == 2)
                {
                    _activeDiagram.RemoveField(arguments[0], arguments[1]);
                    
                    PrintColoredLine($"Field '{arguments[1]}' deleted in Class '{arguments[0]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To delete an existing Field, please enter \"delete_field\" " +
                                     "followed by a class name and field name " +
                                     "into the console and then press enter.", ERROR_COLOR);
                }
                break;
            
            case ("rename_field"):
                if (arguments.Count != 3)
                {
                    PrintColoredLine("To rename an existing Field, please enter \"rename_field\" " +
                                     "followed by a class name, the field name to rename, and the new name of the field " +
                                     "into the console and then press enter.", ERROR_COLOR);
                    break;
                }
                
                _activeDiagram!.RenameField(arguments[0], arguments[1], arguments[2]);
                PrintColoredLine($"Field '{arguments[0]}' successfully renamed to '{arguments[1]}' in class '{arguments[0]}'");
                break;

            case ("change_field_type"):
            {
                if (arguments.Count != 3)
                {
                    PrintColoredLine("To change the type of an existing Field, please enter \"change_field_type\" " +
                                     "followed by a class name, the field name to change the type of, and the new type of the field " +
                                     "into the console and then press enter.", ERROR_COLOR);
                    break;
                }
                
                _activeDiagram!.ChangeFieldType(arguments[0], arguments[1], arguments[2]);
                
                PrintColoredLine($"Field '{arguments[1]}' in class '{arguments[0]}' changed to type '{arguments[2]}'", SUCCESS_COLOR);
                break;
            }

            case ("change_method_type"):
            {
                if (arguments.Count != 3)
                {
                    PrintColoredLine("To change the type of an existing Method, please enter \"change_method_type\" " +
                                     "followed by a class name, the method name to change the type of, and the new type of the method " +
                                     "into the console and then press enter.", ERROR_COLOR);
                    break;
                }
                
                _activeDiagram!.ChangeMethodType(arguments[0], arguments[1], arguments[2]);
                
                PrintColoredLine($"Field '{arguments[1]}' in class '{arguments[0]}' changed to type '{arguments[2]}'", SUCCESS_COLOR);
                break;
            }

            case ("add_method"):

                string syntaxError = "Expected syntax: targetClass returnType methodName *optional parameter list in the form <paramType> <paramName>*\n" +
                                     "Examples:\n\tadd_method class1 int methodName\n\tadd_method class1 int methodName int x int y";
                
                /* Valid syntax:
                 * add_method <targetClass> <returnType> <methodName> OR
                 * add_method <targetClass> <returnType> <methodName> <parameter list (optional)> OR */
                string[] enteredTokens = input.Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (enteredTokens.Length >= 4)
                {
                    
                    /* Ensure an even number of words follows
                     * Valid parameter specification should be in the format of <type> <name> */
                    int remainingWords = enteredTokens.Length - 4;

                    if (remainingWords % 2 != 0)
                    {
                        
                        // Syntax error
                        PrintColoredLine(syntaxError, ERROR_COLOR);
                        return;

                    }

                    List<NameTypeObject> parameters = new List<NameTypeObject>();
                    for (int i = 4; i < enteredTokens.Length; i += 2)
                    {
                        
                        // Parse out the entered parameters 
                        string paramType = enteredTokens[i];
                        string paramName = enteredTokens[i + 1];
                        
                        parameters.Add(new NameTypeObject(paramType, paramName));

                    }

                    string targetClass = enteredTokens[1];
                    string returnType  = enteredTokens[2];
                    string methodName  = enteredTokens[3];
                    
                    _activeDiagram.AddMethod(targetClass, returnType, methodName, parameters);
                    PrintColoredLine("Method created", SUCCESS_COLOR);

                }

                else
                {
                    // Syntax error
                    PrintColoredLine(syntaxError, ERROR_COLOR);
                }
                
                break;

            case ("delete_method"):
                if (arguments.Count() == 2)
                {
                    _activeDiagram!.DeleteMethod(arguments[0], arguments[1]);
                    PrintColoredLine($"Method '{arguments[1]}' deleted in Class '{arguments[0]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To delete an existing Method, please enter \"delete_method\" " +
                                     "followed by a class name and method name into the console and then press enter.", ERROR_COLOR);

                }
                break;
            
            case ("add_parameter"):
                if (arguments.Count != 4)
                {
                    PrintColoredLine("To add a parameter to an existing method, please enter \"add_parameter\"" +
                                     "followed by a class name, a method name, the new parameter type, and the new parameter name", ERROR_COLOR);
                    break;
                }

                _activeDiagram!.AddParameter(arguments[0], arguments[1], arguments[2], arguments[3]);
                PrintColoredLine($"Parameter '{arguments[3]}' of type '{arguments[2]}' added to method '{arguments[1]}'", SUCCESS_COLOR);
                break;
            
            case ("remove_parameter"):
                if (arguments.Count != 3)
                {
                    PrintColoredLine("Top remove a parameter from an existing method, please enter " +
                                     "\"remove_parameter\" followed by a class name, a method name, and the " +
                                     "parameter name", ERROR_COLOR);
                    break;
                }

                if (arguments[2] == "-all")
                {
                    _activeDiagram!.ClearParameters(arguments[0], arguments[1]);
                    PrintColoredLine($"All parameters successfully removed from '{arguments[1]}'");
                }

                else
                {
                    _activeDiagram!.RemoveParameter(arguments[2], arguments[1], arguments[0]);
                    PrintColoredLine($"Parameter '{arguments[2]}' successfully removed from '{arguments[1]}'");
                }

                break;

            case ("rename_method"):
                if (arguments.Count == 3)
                {
                    _activeDiagram!.RenameMethod(arguments[0], arguments[1], arguments[2]);
                    
                    PrintColoredLine($"Method '{arguments[1]}' renamed to '{arguments[2]}' in Class '{arguments[0]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To rename an existing method, please enter \"rename_method\" " +
                                     "followed by a class name, the name of the method to change, and " +
                                     "a new name for the method and then press enter.", ERROR_COLOR);
                }
                break;
            
            case ("rename_parameter"):
                if (arguments.Count != 4)
                {
                    PrintColoredLine("To rename an existing parameter, please enter \"rename_parameter\"" +
                                     "followed by a class name, the name of the method, the name of the parameter to change," +
                                     "and the new parameter name");
                    break;
                }
                
                _activeDiagram!.RenameParameter(arguments[0], arguments[1], 
                    arguments[2], arguments[3]);
                break;
            
            case ("replace_parameter"):
                if (arguments.Count != 5)
                {
                    PrintColoredLine("To replace an existing parameter, please enter \"replace_parameter\"" +
                                     "followed by a class name, the name of the method, the name of the parameter to change," +
                                     "the new parameter type, and the new parameter name");
                    break;
                }
                
                _activeDiagram!.ReplaceParameter(arguments[0], arguments[1], 
                    arguments[2], new NameTypeObject(arguments[3], arguments[4]));
                PrintColoredLine($"Successfully replaced parameter '{arguments[2]}' with parameter of type" +
                                 $"'{arguments[3]}' and name '{arguments[4]}' in method '{arguments[1]}'");
                break;

            case ("list_classes"):
                if (arguments.Count == 0)
                {
                    PrintColoredLine(_activeDiagram!.ListClasses());
                }

                else
                {
                    PrintColoredLine("Invalid command: list classes does not take any arguments", ERROR_COLOR);
                }    

                break;

            case ("list_attributes"):
                if (arguments.Count == 1)
                {
                    PrintColoredLine(_activeDiagram!.ListAttributes(arguments[0]));
                }

                else
                {
                    PrintColoredLine("To list Attributes, please enter \"list_attribute\" " +
                                     "followed by a class name into the console and then press enter.", ERROR_COLOR);
                }
                break;

            case ("add_relationship"):
                if (arguments.Count == 3)
                {
                    // Try to add relationship with the first provided word as its name
                    _activeDiagram!.AddRelationship(arguments[0], arguments[1], 
                        arguments[2]);

                    PrintColoredLine($"Relationship '{arguments[0]} => {arguments[1]}' with type '{arguments[2]}' "+
                        "successfully created.", SUCCESS_COLOR);
                }

                else
                {
                    string validRelationshipTypes = "";
                    foreach (string type in Relationship.ValidTypes)
                    {
                        validRelationshipTypes += $"{type} ";
                    }

                    // Cut off the space from the last type
                    validRelationshipTypes = validRelationshipTypes.Substring(0, validRelationshipTypes.Length - 1);
                    
                    PrintColoredLine("To create a new Relationship, please enter \"add_relationship\" " +
                                     "followed by a source class, a destination class, " +
                                     $"and then the relationship type ('{validRelationshipTypes}').", ERROR_COLOR);
                } 
                break;

            case ("delete_relationship"):
                if (arguments.Count == 2)
                {
                    // Try to delete relationship with the given source and destination classes
                    _activeDiagram!.DeleteRelationship(arguments[0], arguments[1]);

                    PrintColoredLine($"Relationship '{arguments[0]} => {arguments[1]}' deleted", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("To delete an existing Relationship, please enter \"delete_relationship\" " +
                                     "followed by the source class and destination class then press enter.", ERROR_COLOR);
                } 
                break;
            
            case ("change_relationship_type"):
                if (arguments.Count != 3)
                {
                    string validRelationshipTypes = "";
                    foreach (string type in Relationship.ValidTypes)
                    {
                        validRelationshipTypes += $"{type} ";
                    }

                    // Cut off the space from the last type
                    validRelationshipTypes = validRelationshipTypes.Substring(0, validRelationshipTypes.Length - 1);
                    PrintColoredLine( "To change the type of an existing relationship, please provide a source and destination " +
                                     $"class and a new relationship type of a valid type ('{validRelationshipTypes}')", ERROR_COLOR);
                    break;
                }
                
                _activeDiagram!.ChangeRelationship(arguments[0], arguments[1], arguments[2]);
                
                PrintColoredLine($"Relationship type successfully changed to type '{arguments[2]}'");
                break;
            
            case ("list_relationships"):
                if (arguments.Count == 0)
                {
                    // Try to add relationship with the first provided word as its name
                    PrintColoredLine(_activeDiagram!.ListRelationships());
                }

                else
                {
                    PrintColoredLine("Invalid command: list relationships does not take any arguments", ERROR_COLOR);
                } 
                break;


            case ("help"):
                if (arguments.Count > 0)
                {
                    PrintColoredLine("Invalid command: help does not take any arguments", ERROR_COLOR);
                    break;
                }
                PrintColoredLine("\nadd_class: Add a new class to the diagram" +
                                 "\ndelete_class: Delete an existing class" +
                                 "\nrename_class: Rename an existing class" +
                                 "\nadd_relationship: Add a relationship between classes" +
                                 "\ndelete_relationship: Delete an existing relationship" +
                                 "\nchange_relationship_type: Changes the type of an existing relationship"+
                                 "\nadd_field: Add a field to an existing Class" +
                                 "\nadd_method: Add a method to an existing Class" +
                                 "\ndelete_field: Delete an existing field from a class" +
                                 "\ndelete_method: Delete an existing method from a class" +
                                 "\nrename_field: Rename an existing field on a class" +
                                 "\nrename_method: Rename an existing method on a class" +
                                 "\nchange_field_type: Changes the type of an existing field" +
                                 "\nchange_method_type: Changes the type of an existing method" +
                                 "\nadd_parameter: Adds a parameter to a method" +
                                 "\nremove_parameter: "+
                                 "\n\t-Removes an existing parameter from a method."+
                                 "\n\t-Providing -all for 3rd argument will remove all parameters from a method" +
                                 "\nrename_parameter: Renames an existing parameter in a method"+
                                 "\nsave_diagram: Save your progress" +
                                 "\nload_diagram: Load a previously saved diagram" +
                                 "\nlist_classes: List all existing classes" +
                                 "\nlist_attributes: List all attributes of a class" +
                                 "\nlist_relationships: List all relationships of a class\n", ConsoleColor.DarkCyan);
                break;

            case ("save_diagram"):
                if (arguments.Count == 1)
                {
                    string fileString = arguments.ElementAt(0);
                    if (!fileString.EndsWith(".json"))
                    {
                        PrintColoredLine("Please use .json file extension", ERROR_COLOR);
                        break;
                    }

                    _activeFile!.SaveDiagram(ref _activeDiagram!, arguments[0]);


                    PrintColoredLine($"Current diagram saved to '{arguments[0]}'", SUCCESS_COLOR);
                }

                else
                {
                    PrintColoredLine("Please provide a file to save the diagram to", ERROR_COLOR);
                }

                break;

            case ("load_diagram"):
                if (arguments.Count == 1)
                {
                    string fileString = arguments[0];
                    if (!fileString.EndsWith(".json"))
                    {
                        PrintColoredLine("Please provide a file with a .json file extension", ERROR_COLOR);
                        break;
                    }

                    _activeDiagram = _activeFile!.LoadDiagram(arguments[0]);


                    PrintColoredLine($"Diagram '{arguments[0]}' loaded.", SUCCESS_COLOR);
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