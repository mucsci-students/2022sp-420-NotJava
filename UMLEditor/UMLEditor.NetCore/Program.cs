using System;
using Avalonia;
using Avalonia.ReactiveUI;
using UMLEditor.Classes;
using UMLEditor.Views;

namespace UMLEditor.NetCore
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            int counter = 0;
            Diagram.DiagramChanged += (sender, _) =>
            {
                Console.WriteLine($"Diagram Changed {counter++}");
                TimeMachine.AddState((Diagram)sender!);
            };
            // Ensure at least one arg is provided
            if (args.Length >= 1)
            {

                // Extract the first flag
                string providedArg = args[0];

                switch (providedArg)
                {
                    
                    // CLI mode
                    case("-cli"):
                        CommandLine c = new CommandLine();
                        c.RunCli();
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