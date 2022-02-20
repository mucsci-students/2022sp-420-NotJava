using System;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using UMLEditor.Classes;
using UMLEditor.Exceptions;
using UMLEditor.Interfaces;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram _activeDiagram;

        private readonly TextBox _outputBox;
        private readonly TextBox _inputBox;

        private IDiagramFile _activeFile;

        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        
        private Button SaveDiagramButton;
        private Button LoadDiagramButton;
        
        public MainWindow()
        {
            
            InitializeComponent();
            
            _activeDiagram = new Diagram();
            _activeFile = new JSONDiagramFile();
            
            _outputBox = this.FindControl<TextBox>("OutputBox");
            _inputBox = this.FindControl<TextBox>("InputBox");

            SaveDiagramButton = this.FindControl<Button>("SaveDiagramButton");
            LoadDiagramButton = this.FindControl<Button>("LoadDiagramButton");
            InitFileDialogs(out _openFileDialog, out _saveFileDialog, "json");
            
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
        private void InitFileDialogs(out OpenFileDialog openFD, out SaveFileDialog saveFD, params string[] filteredExtensions)
        {

            string workingDir = Directory.GetCurrentDirectory();
            
            /* - Construct the open file dialog
             * - Set its title
             * - Disallow selecting multiple files in this dialog */
            openFD = new OpenFileDialog();
            openFD.Title = "Load Diagram From File";
            openFD.AllowMultiple = false;
            
            // Construct / init the save dialog
            saveFD = new SaveFileDialog();
            saveFD.Title = "Save Diagram To File";

            // Make the save / open dialog open to the working directory by default
            openFD.Directory = workingDir;
            saveFD.Directory = workingDir;

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

            // Ask the UI thread to clear the input box
            Dispatcher.UIThread.Post(() =>
            {
                
                _inputBox.Text = "";                
                
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

                    _outputBox.Text += text;

                });
                
                return;

            }
            
            Dispatcher.UIThread.Post((() =>
            {

                _outputBox.Text = text;

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
            _outputBox.Text = 
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
        
        private void List_Classes_OnClick(object sender, RoutedEventArgs e)
        {
            _outputBox.Text = _activeDiagram.ListClasses();
        }
        
        private void List_Attributes_OnClick(object sender, RoutedEventArgs e)
        {
            //Get input from input box
            string input = _inputBox.Text;
            
            //Split the input into class names
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != 1)
            {
                // Error for invalid input
                _outputBox.Text =
                    "To list the attributes of an existing class enter the " +
                    "class name into the input box and then click 'List Attributes'.";
                _inputBox.Focus();
                return;
            }
            
            Class currentClass = _activeDiagram.GetClassByName(words[0]);

            // If the TargetClass does not exist throw an error
            if (currentClass == null)
            {
                _outputBox.Text = "Nonexistent class entered";
                _inputBox.Focus();
                return;
            }
            _outputBox.Text = currentClass.ListAttributes();
        }
        
        private void List_Relationships_OnClick(object sender, RoutedEventArgs e)
        {
            _outputBox.Text = _activeDiagram.ListRelationships();
        }

        private void AddRelationship_OnClick(object sender, RoutedEventArgs e)
        {

            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = _inputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To create a new relationship, please enter source and destination in the format " +
                    "'A B C' into the input box and then click 'Add Relationship'.";
                
                _inputBox.Focus();
                return;

            }
            
            else if (words.Length != 3)
            {
                
                // Invalid input arguments
                _outputBox.Text = 
                    "Input must be in the form 'A B C' with only one source, one destination and the type." +
                    "Please enter your relationship into the input box and click 'Add Relationship'.";
                
                _inputBox.Focus();
                return;
                
            }

            string sourceClassName = words[0];
            string destClassName = words[1];
            string relationshipType = words[2];
            
            try
            {
                
                _activeDiagram.AddRelationship(sourceClassName, destClassName, relationshipType);
                
            }
            
            catch (ClassNonexistentException exception)
            {

                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;

            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Relationship created ({0} => {1})", sourceClassName, destClassName);

        }

        private void Save_Button_OnClick(object sender, RoutedEventArgs e)
        {

            // Disable the save diagram button to disallow opening selector multiple times
            SaveDiagramButton.IsEnabled = false;
            
            /* Open the file save dialog on its own thread
             * Obtain a future from this action */
            Task<string?> saveTask = _saveFileDialog.ShowAsync(this);
            saveTask.ContinueWith((Task<string?> finishedTask) =>
            {

                // Grab the selected output file
                string? selectedFile = finishedTask.Result;

                // Make sure a file was selected
                if (selectedFile != null)
                {

                    try
                    {

                        _activeFile.SaveDiagram(ref _activeDiagram, selectedFile);
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
            Task<string[]?> loadTask = _openFileDialog.ShowAsync(this);
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

                        _activeDiagram = _activeFile.LoadDiagram(chosenFile);
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

        private void Add_Attribute_OnClick(object sender, RoutedEventArgs e)
        {
            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid
            string input = _inputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);
            
            // Make sure at least two words were provided in the input box
            if (words.Length != 2)
            {
                
                // Invalid input arguments
                _outputBox.Text = 
                    "To add an attribute enter source class and attribute in the format " +
                    "'Class Attribute' into the input box and then click 'Add Attribute'.";
                
                _inputBox.Focus();
                return;
                
            }

            // Make sure the first word is the name of a class that exists
            if (!_activeDiagram.ClassExists(words[0]))
            {
                string message = string.Format("Class {0} does not exist", words[0]);
                _outputBox.Text = message;
                _inputBox.Focus();
                return;
            }

            try
            {

                // Add the attribute to the provided class
                //_activeDiagram.GetClassByName(words[0]).AddAttribute(words[1]);

            }

            catch (Exception exception)
            {
                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;
            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Class {0} given Attribute {1}", words[0], words[1]);
        }
        
        
        private void Delete_Attribute_OnClick(object sender, RoutedEventArgs e)
        {
            
            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, delete attribute if is.
            string input = _inputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To delete an existing attribute enter source class and attribute in the format " +
                    "'Class Attribute' into the input box and then click 'Delete Attribute'.";
                
                _inputBox.Focus();
                return;

            }
            
            else if (words.Length != 2)
            {
                
                // Invalid input arguments
                _outputBox.Text = 
                    "Input must be in the form 'Class Attribute' with only one class and one attribute.  " +
                    "Please enter this into the input box and click 'Delete Attribute'.";
                
                _inputBox.Focus();
                return;
                
            }
            
            string targetClassName = words[0];
            string targetAttributeName = words[1];

            // Create CurrentClass for use in reaching its attributes
            Class currentClass = _activeDiagram.GetClassByName(targetClassName);

            // If the TargetClass does not exist throw an error
            if (currentClass == null)
            {
                _outputBox.Text = "Nonexistent class entered";
                _inputBox.Focus();
                return;
            }

            try
            {
                
                //currentClass.DeleteAttribute(targetAttributeName);

            }
            
            // Check if the attribute doesn't exist.
            catch (AttributeNonexistentException exception)
            {

                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;

            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Attribute Deleted ({0} => {1})", targetClassName, targetAttributeName);

        }

        private void Class_AddClass_OnClick (object sender, RoutedEventArgs e)
        {
            // User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = _inputBox.Text;
            
            // Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);
            
            // Assures only one word was given as input for a class name
            if (words.Length != 1)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To create a new Class, please enter a class name " +
                    "into the input box and then click 'Add Class'.";
                
                _inputBox.Focus();
                return;

            }

            try
            {

                // Try to create a class with the first provided word as its name
                _activeDiagram.AddClass(words[0]);

            }

            catch (Exception exception)
            {
                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;
            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Class Created {0}", words[0]);

        }

        private void Class_DeleteClass_OnClick(object sender, RoutedEventArgs e)
        {
            // User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, delete attribute if is.
            string input = _inputBox.Text;
            
            // Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To delete an existing class enter the name of class" +
                    "'ClassName' into the input box and then click 'Delete Class'. "+
                    "Deleting will delete relationship and attributes with it.";
                
                _inputBox.Focus();
                return;

            }
            
            else if (words.Length != 1 || (!Char.IsLetter(words[0][0]) && words[0][0] != '_'))
            {
                
                // Invalid input arguments
                _outputBox.Text = 
                    "Class must be a single word that starts with an alphabetic character or an underscore. " +
                    "Please enter this into the input box and click 'Delete Class'.";
                
                _inputBox.Focus();
                return;
                
            }
            
            string targetClassName = words[0];

            // Create CurrentClass for use in reaching its attributes
            Class currentClass = _activeDiagram.GetClassByName(targetClassName);

            // If the TargetClass does not exist throw an error
            if (currentClass == null)
            {
                _outputBox.Text = "Nonexistent class entered";
                _inputBox.Focus();
                return;
            }

            try
            {
                _activeDiagram.DeleteClass(words[0]);
            }

            catch (Exception exception)

            {
                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;
            }

            ClearInputBox();
            _outputBox.Text = string.Format("Class Deleted {0}", words[0]);

        }

        private void Class_RenameClass_OnClick(object sender, RoutedEventArgs e)
        {
            // User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = _inputBox.Text;
            
            // Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);
            
            // Assures two words were given as input for a class rename
            if (words.Length != 2)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To rename a Class, please enter the class to rename followed by " +
                    " the new name in the form 'A B' into the input box and then click 'Rename Class'.";
                
                _inputBox.Focus();
                return;

            }

            try
            {
                _activeDiagram.RenameClass(words[0], words[1]);
            }

            catch (Exception exception)
            {
                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;
            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Class Renamed {0} to {1}", words[0], words[1]);
        }
        
        private void DeleteRelationship_OnClick(object sender, RoutedEventArgs e)
        {

            // User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = _inputBox.Text;
            
            // Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                _outputBox.Text = 
                    "To delete a relationship, please enter source and destination in the format " +
                    "'A B' into the input box and then click 'Delete Relationship'.";
                
                _inputBox.Focus();
                return;

            }
            
            //Ensures two classes entered
            if (words.Length != 2)
            {
                _outputBox.Text =
                    "Two classes required to delete relationship";
                
                _inputBox.Focus();
                return;
            }
            
            string sourceClassName = words[0];
            string destClassName = words[1];
            
            try
            {
                
                _activeDiagram.DeleteRelationship(sourceClassName, destClassName);
                
            }
            
            catch (RelationshipNonexistentException exception)
            {

                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;

            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Relationship Deleted ({0} => {1})", sourceClassName, destClassName);

        }

        private void Class_RenameAttribute_OnClick(object sender, RoutedEventArgs e)
        {
            // User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = _inputBox.Text;

            // Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Assures three words were given as input for a attribute rename
            if (words.Length != 3)
            {

                // No input arguments
                _outputBox.Text =
                    "To rename a attribute please enter the class followed by" +
                    " the old name and new name"+
                    "in the form 'A B C' into the input box and then click 'Rename Attribute'.";

                _inputBox.Focus();
                return;

            }
            
            string targetClassName = words[0];

            // Get CurrentClass for use in reaching its attributes
            Class currentClass = _activeDiagram.GetClassByName(targetClassName);
            // If the TargetClass does not exist throw an error
            if (currentClass == null)
            {
                _outputBox.Text = "Nonexistent class entered";
                _inputBox.Focus();
                return;
            }
            
            // ensures user didn't change old name to new name
            if (words[1] == words[2])
            {
                _outputBox.Text = "New name cannot match previous name.";
                _inputBox.Focus();
                return;
            }

            try
            {

                //currentClass.RenameAttribute(words[1], words[2]);

            }

            catch (Exception exception)
            {
                _outputBox.Text = exception.Message;
                _inputBox.Focus();
                return;
            }
            
            ClearInputBox();
            _outputBox.Text = string.Format("Attribute renamed {0} to {1}", words[1], words[2]);
        }
    }
}
