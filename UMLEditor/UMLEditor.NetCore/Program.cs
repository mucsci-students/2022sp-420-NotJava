using System;
using Avalonia;
using Avalonia.ReactiveUI;
using UMLEditor.Testing;

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
            ERROR_FAILED_UNIT_TEST = 6

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
                    
                    case("-test"):
                        
                        // Unit testing mode
                        try
                        {
                            TestingSled.RunTests();
                        }

                        // If any unhandled exceptions come out of RunTests, display info here 
                        catch (Exception e)
                        {

                            string message = string.Format("FATAL: Exception when running unit tests: {0}\n" +
                                                           "Stack Trace:\n{1}", e.Message, e.StackTrace);
                            
                            TestingSled.PrintColoredLine(message, TestingSled.ERROR_COLOR);
                            Environment.ExitCode = (int)CustomExitCodes.ERROR_FAILED_UNIT_TEST;
                            
                        }
                        
                        // Pause the program for user input (ensure user always gets to read output)
                        Console.Write("Press enter to exit... ");
                        Console.ReadLine();
                        break;
                    
                    // CLI mode
                    case("-cli"):
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