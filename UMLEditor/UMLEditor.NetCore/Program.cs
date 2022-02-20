﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using UMLEditor.Classes;
using UMLEditor.Interfaces;

namespace UMLEditor.NetCore
{
    class Program
    {
        
        /// <summary>
        /// Custom defined exit codes
        /// </summary>
        private enum CustomExitCodes
        {
            
            ERROR_UNIMPLEMENTED = 5,

        }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            
            // Ensure at least one arg is provided
            if (args.Length >= 1)
            {

                // Extract the first flag
                string providedArg = args[0];

                switch (providedArg)
                {
                    
                    // CLI mode
                    case("-cli"):

                        IDiagramFile FileIO = new JSONDiagramFile();
                        Diagram current =  FileIO.LoadDiagram("example.json");
                        
                        /*
                        Diagram testDiagram = new Diagram();
                        
                        testDiagram.AddClass("A");
                        testDiagram.AddClass("B");
                        
                        testDiagram.GetClassByName("A").AddField("Test1","int");
                        testDiagram.GetClassByName("A").AddField("Test2","potato");
                        testDiagram.GetClassByName("A").AddMethod("Test2","potato");
                        testDiagram.GetClassByName("A").AddMethod("Test3","string");
                        testDiagram.GetClassByName("A").AddMethod("Test4","potato");
                        
                        testDiagram.GetClassByName("A").GetMethodByName("Test2").AddParam(new NameTypeObject("P1", "string") );
                        testDiagram.GetClassByName("A").GetMethodByName("Test2").AddParam(new NameTypeObject("P1", "string") );
                        
                        */

                        Environment.Exit((int)CustomExitCodes.ERROR_UNIMPLEMENTED);
                        break;
                    
                    default:
                        Console.WriteLine("Invalid argument provided: {0}", providedArg);
                        break;
                    
                }
                
                return;

            }
            
            // GUI mode is the default mode
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        
    }
}