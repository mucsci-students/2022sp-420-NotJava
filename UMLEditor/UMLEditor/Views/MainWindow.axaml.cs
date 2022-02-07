using System;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram ActiveDiagram;

        private TextBox OutputBox;
        private TextBox InputBox;

        private JSONDiagramFile ActiveFile;

        private OpenFileDialog OpenDialog;
        private SaveFileDialog SaveDialog;
        
        private Button SaveDiagramButton;
        private Button LoadDiagramButton;
        
        public MainWindow()
        {
            
            InitializeComponent();
            
            ActiveDiagram = new Diagram();
            ActiveFile = new JSONDiagramFile();
            
            OutputBox = this.FindControl<TextBox>("OutputBox");
            InputBox = this.FindControl<TextBox>("InputBox");

            SaveDiagramButton = this.FindControl<Button>("SaveDiagramButton");
            LoadDiagramButton = this.FindControl<Button>("LoadDiagramButton");
            InitFileDialogs(out OpenDialog, out SaveDialog, new []{ "json" });
            
            // MATTHEW & CJ adding a new class should be as simple as this, just remember to add input verification.
            
             ActiveDiagram.Classes.Add(new Class("HELLO"));
             ActiveDiagram.Classes.Add(new Class("WORLD"));
             
             Class CurrentClass = ActiveDiagram.GetClassByName("HELLO");
             CurrentClass.Attributes.Add(new AttributeObject("Test"));
             CurrentClass.Attributes.Add(new AttributeObject("Test2"));

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Initializes the provided file selection dialogs with the provided extension filters
        /// </summary>
        /// <param name="openFD">The OpenFileDialog to configure</param>
        /// <param name="saveFD">The SaveFileDialog to configure</param>
        /// <param name="filteredExtensions">An array of file extensions that will be selectable in the dialogs.
        /// No need for the "." in the extension.</param>
        private void InitFileDialogs(out OpenFileDialog openFD, out SaveFileDialog saveFD, string[] filteredExtensions)
        {

            string WorkingDir = Directory.GetCurrentDirectory();
            
            openFD = new OpenFileDialog();
            openFD.Title = "Load Diagram From File";
            openFD.AllowMultiple = false;
            
            saveFD = new SaveFileDialog();
            saveFD.Title = "Save Diagram To File";

            // Make the save/ open dialog open to the working directory by default
            openFD.Directory = WorkingDir;
            saveFD.Directory = WorkingDir;

            foreach (string extension in filteredExtensions)
            {
                
                // Establish a filter for the current file extension
                FileDialogFilter filter = new FileDialogFilter();
                filter.Name = string.Format(".{0} Diagram Files", extension);
                filter.Extensions.Add(extension);

                openFD.Filters.Add(filter);
                saveFD.Filters.Add(filter);
                
            }
            
        }
        
        /// <summary>
        /// Clears the input box in a thread safe manner
        /// </summary>
        private void ClearInputBox()
        {

            Dispatcher.UIThread.Post(() =>
            {
                
                InputBox.Text = "";                
                
            });
            
        }

        /// <summary>
        /// Allows writing text to the output box in a thread safe manner
        /// </summary>
        /// <param name="text">The text to write to the output box</param>
        /// <param name="append">If true, then text will be appended.
        /// Otherwise, the output box's text is set to text</param>
        private void WriteToOutput(string text, bool append = false)
        {

            if (append)
            {

                Dispatcher.UIThread.Post(() =>
                {

                    OutputBox.Text += text;

                });
                
                return;

            }
            
            Dispatcher.UIThread.Post((() =>
            {

                OutputBox.Text = text;

            }));
            
        }
        
        private void ExitB_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void HelpB_OnClick(object sender, RoutedEventArgs e)
        {
            
            ClearInputBox();            
            
            // Output list of possible commands.
            OutputBox.Text = 
                "New Class: Add a new class to the diagram" + 
                "\nDelete Class: Delete an existing class" +
                "\nRename Class: Rename an existing class" +
                "\nAdd Relationship: Add a relationship between classes" +
                "\nDelete Relationship: Delete an existing relationship" +
                "\nAdd Attribute: Add an attribute to an existing Class" +
                "\nDelete Attribute: Delete an existing class attribute" +
                "\nRename Attribute: Rename an existing attribute" +
                "\nSave: Save your progress" +
                "\nLoad: Load a previously saved diagram" +
                "\nList Classes: List all existing classes" +
                "\nList Attributes: List all attributes of a class" +
                "\nList Relationships: List all relationships of a class";
            
        }

        // On button click: Display all classes of current diagram to output box
        private void List_Classes_OnClick(object sender, RoutedEventArgs e)
        {
            OutputBox.Text = ActiveDiagram.ListClasses();
        }
        
        // On button click: Display all attributes of selected class to output box
        private void List_Attributes_OnClick(object sender, RoutedEventArgs e)
        {
            //Get input from input box
            string input = InputBox.Text;
            
            //Split the input into class names
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != 1)
            {
                // Error for invalid input
                OutputBox.Text =
                    "To list the attributes of an existing class enter the " +
                    "class name into the input box and then click 'List Attributes'.";
                InputBox.Focus();
                return;
            }
            
            Class CurrentClass = ActiveDiagram.GetClassByName(words[0]);

            // If the TargetClass does not exist throw an error
            if (CurrentClass == null)
            {
                OutputBox.Text = "Nonexistent class entered";
                InputBox.Focus();
                return;
            }
            OutputBox.Text = CurrentClass.ListAttributes();
        }
        
        // On button click: Display all relationships of current diagram to output box
        private void List_Relationships_OnClick(object sender, RoutedEventArgs e)
        {
            OutputBox.Text = ActiveDiagram.ListRelationships();
        }

        private void AddRelationship_OnClick(object sender, RoutedEventArgs e)
        {

            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = InputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                OutputBox.Text = 
                    "To create a new relationship, please enter source and destination in the format " +
                    "'A B' into the input box and then click 'Add Relationship'.";
                
                InputBox.Focus();
                return;

            }
            
            else if (words.Length != 2)
            {
                
                // Invalid input arguments
                OutputBox.Text = 
                    "Input must be in the form 'A B' with only one source and one destination.  " +
                    "Please enter your relationship into the input box and click 'Add Relationship'.";
                
                InputBox.Focus();
                return;
                
            }

            string SourceClassName = words[0];
            string DestClassName = words[1];
            
            try
            {
                
                ActiveDiagram.AddRelationship(SourceClassName, DestClassName);
                
            }
            
            catch (ClassNonexistentException exception)
            {

                OutputBox.Text = exception.Message;
                InputBox.Focus();
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Relationship created ({0} => {1})", SourceClassName, DestClassName);

        }

        private void SaveButtonOnClick(object sender, RoutedEventArgs e)
        {

            // Disable the save diagram button to disallow opening selector multiple times
            SaveDiagramButton.IsEnabled = false;
            
            /* Open the file save dialog on its own thread
             * Obtain a future from this action */
            Task<string?> saveTask = SaveDialog.ShowAsync(this);
            saveTask.ContinueWith((Task<string?> finishedTask) =>
            {

                // Grab the selected output file
                string? selectedFile = finishedTask.Result;

                // Make sure a file was selected
                if (selectedFile != null)
                {

                    try
                    {

                        ActiveFile.SaveDiagram(ref ActiveDiagram, selectedFile);
                        WriteToOutput(string.Format("Current diagram saved to {0}", selectedFile));
                        ClearInputBox();

                    }

                    catch (Exception exception)
                    {

                        WriteToOutput(exception.Message);

                    }
                    
                }

                else
                {
                    
                    WriteToOutput("Diagram not saved");
                    
                }

                // Regardless of the outcome, ask the UI thread to re-enable the save button
                Dispatcher.UIThread.Post(() =>
                {

                    SaveDiagramButton.IsEnabled = true;

                });
                
            });
            
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {

            // Disable the loading diagram button to disallow opening selector multiple times
            LoadDiagramButton.IsEnabled = false;

            /* Open the file selection dialog on its own thread
             * Obtain a future from this action */
            Task<string[]?> loadTask = OpenDialog.ShowAsync(this);
            loadTask.ContinueWith((Task<string[]?> taskResult) =>
            {

                // Called when the future is resolved
                
                /* Get the files the user selected
                 * This will be null if the user canceled the operation or closed the window */
                string[]? selectedFiles = taskResult.Result;
                bool hasSelectedFile = selectedFiles != null && selectedFiles.Length >= 1;

                if (hasSelectedFile)
                {

                    // Pull only the first selected file (AllowMultiple should be turned off on the dialog)
                    string chosenFile = selectedFiles[0];
                    
                    try
                    {

                        ActiveDiagram = ActiveFile.LoadDiagram(chosenFile);
                        ClearInputBox();
                        WriteToOutput(string.Format("Diagram loaded from {0}", chosenFile));

                    }
            
                    catch (Exception exception)
                    {

                        WriteToOutput(exception.Message);

                    }

                }

                else
                {
                    
                    WriteToOutput("No diagram file selected");
                    
                }
                
                // Regardless of the outcome, ask the UI thread to re-enable the load button
                Dispatcher.UIThread.Post(() =>
                {

                    LoadDiagramButton.IsEnabled = true;

                });
                
            });
            
        }

        private void Delete_Attribute_OnClick(object sender, RoutedEventArgs e)
        {
            
            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, delete attribute if is.
            string input = InputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                OutputBox.Text = 
                    "To delete an existing attribute enter source class and attribute in the format " +
                    "'Class Attribute' into the input box and then click 'Delete Attribute'.";
                
                InputBox.Focus();
                return;

            }
            
            else if (words.Length != 2)
            {
                
                // Invalid input arguments
                OutputBox.Text = 
                    "Input must be in the form 'Class Attribute' with only one class and one attribute.  " +
                    "Please enter this into the input box and click 'Delete Attribute'.";
                
                InputBox.Focus();
                return;
                
            }
            
            string TargetClassName = words[0];
            string TargetAttributeName = words[1];

            // Create CurrentClass for use in reaching its attributes
            Class CurrentClass = ActiveDiagram.GetClassByName(TargetClassName);

            // If the TargetClass does not exist throw an error
            if (CurrentClass == null)
            {
                OutputBox.Text = "Nonexistent class entered";
                InputBox.Focus();
                return;
            }

            try
            {
                
                CurrentClass.DeleteAttribute(TargetAttributeName);

            }
            
            // Check if the attribute doesn't exist.
            catch (AttributeNonexistentException exception)
            {

                OutputBox.Text = exception.Message;
                InputBox.Focus();
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Attribute Deleted ({0} => {1})", TargetClassName, TargetAttributeName);

        }

        private void Class_AddClass_OnClick (object sender, RoutedEventArgs e)
        {
            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = InputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                OutputBox.Text = 
                    "To create a new Class, please enter a class name " +
                    "into the input box and then click 'Add Class'.";
                
                InputBox.Focus();
                return;

            }
            
            else if (words.Length != 1 || (!Char.IsLetter(words[0][0]) && words[0][0] != '_'))
            {
                
                // Invalid input arguments
                OutputBox.Text = 
                    "Class must be a single word that starts with an alphabetic character or an underscore.  " +
                    "Please enter your Class into the input box and click 'Add Class'.";
                
                InputBox.Focus();
                return;
                
            }

            try
            {
                
                ActiveDiagram.AddClass(words[0]);
                
            }
            
            catch (ClassAlreadyExistsException exception)
            {

                OutputBox.Text = exception.Message;
                InputBox.Focus();
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Class Created {0}", words[0]);

        }

        private void Class_DeleteClass_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Class_RenameClass_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
        
    }
}
