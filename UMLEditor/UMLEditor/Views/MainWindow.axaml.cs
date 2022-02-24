using System;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;
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
            
            // Commented out because the GUI will be reworked
            /*Class currentClass = _activeDiagram.GetClassByName(words[0]);
            

            // Warn user if the requested class does not exist
            if (currentClass == null)
            {
                _outputBox.Text = "Nonexistent class entered";
                _inputBox.Focus();
                return;
            }
            _outputBox.Text = currentClass.ListAttributes();*/
        }
        
        private void List_Relationships_OnClick(object sender, RoutedEventArgs e)
        {
            _outputBox.Text = _activeDiagram.ListRelationships();
        }

        private void AddRelationship_OnClick(object sender, RoutedEventArgs e)
        {
            ModalDialog AddRelationshipModal = ModalDialog.CreateDialog<AddRelationshipPanel>("Add New Relationship", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddRelationshipModal.ShowDialog<DialogButtons>(this);
            
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    
                    string sourceName = AddRelationshipModal.GetPrompt<AddRelationshipPanel>().SourceClass;
                    string destinationName = AddRelationshipModal.GetPrompt<AddRelationshipPanel>().DestinationClass;
                    string relationshipType = AddRelationshipModal.GetPrompt<AddRelationshipPanel>().RelationshipType;
                    
                    if (sourceName is null || sourceName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "The source name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;

                    }
                    
                    if (!isCorrectNumArguments(sourceName, 1))
                    {
                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "Only one argument expected for source name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (destinationName is null || destinationName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "The destination name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;

                    }
                    
                    if (!isCorrectNumArguments(destinationName, 1))
                    {
                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "Only one argument expected for destination name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (relationshipType is null || relationshipType.Trim().Length == 0)
                    {
                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "The relationship type name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(relationshipType, 1))
                    {
                        RaiseAlert(
                            "Relationship Creation Failed", 
                            "Could Not Create Relationship",
                            "Only one argument expected for relationship type",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    switch (result.Result)
                    {
                        case DialogButtons.OKAY:

                            try
                            {
                                _activeDiagram.AddRelationship(sourceName,destinationName,relationshipType);
                                RaiseAlert(
                                    "Relationship Added",
                                    $"Relationship {sourceName} => {destinationName} of type {relationshipType} created",
                                    "",
                                    AlertIcon.INFO);
                            }

                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Class Creation Failed",
                                    $"Could not create relationship {sourceName} => {destinationName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }
                            break;
                    }
                });
            });
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
                    string chosenFile = selectedFiles![0];
                    
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

        private void Add_Field_OnClick(object sender, RoutedEventArgs e)
        {
            ModalDialog AddFieldModal = ModalDialog.CreateDialog<AddFieldPanel>("Add New Field", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddFieldModal.ShowDialog<DialogButtons>(this);
            
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    string targetClass = AddFieldModal.GetPrompt<AddFieldPanel>().ClassName;
                    string targetField = AddFieldModal.GetPrompt<AddFieldPanel>().FieldName;
                    string fieldType = AddFieldModal.GetPrompt<AddFieldPanel>().FieldType;
                    
                    Class currentClass = _activeDiagram.GetClassByName(targetClass);
                    
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Field Creation Failed",
                            "Could Not Creation Field",
                            "class does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    if (targetClass is null || targetClass.Trim().Length == 0)
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "The target class cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    if (!isCorrectNumArguments(targetClass, 1))
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "Only one argument expected for target class",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (targetField is null || targetField.Trim().Length == 0)
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "The field name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(targetField, 1))
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "Only one argument expected for target field",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (fieldType is null || fieldType.Trim().Length == 0)
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "The field type cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(fieldType, 1))
                    {
                        RaiseAlert(
                            "Field Creation Failed", 
                            "Could Not Create Field",
                            "Only one argument expected for target type",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    switch (result.Result)
                    {
                        case DialogButtons.OKAY:

                            try
                            {
                                currentClass.AddField(fieldType,targetField);
                                RaiseAlert(
                                    "Field Added",
                                    $"Field {targetField} created",
                                    "",
                                    AlertIcon.INFO);
                            }

                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Class Creation Failed",
                                    $"Could not create field {targetField}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }
                            break;
                    }
                });
            });
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

            // Commented out because gui will be reworked
            // Create CurrentClass for use in reaching its attributes
            /*Class currentClass = _activeDiagram.GetClassByName(targetClassName);

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

            }*/
            
            ClearInputBox();
            _outputBox.Text = string.Format("Attribute Deleted ({0} => {1})", targetClassName, targetAttributeName);
        }

        private void Class_AddClass_OnClick (object sender, RoutedEventArgs e)
        {
            ModalDialog AddClassModal = ModalDialog.CreateDialog<AddClassPanel>("Add New Class", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddClassModal.ShowDialog<DialogButtons>(this);
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    string enteredName = AddClassModal.GetPrompt<AddClassPanel>().ClassName;
                    if (enteredName is null || enteredName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Class Creation Failed", 
                            "Could Not Create Class",
                            "The class name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;

                    }
                    
                    if (!isCorrectNumArguments(enteredName, 1))
                    {
                        RaiseAlert(
                            "Class Creation Failed", 
                            "Could Not Create Class",
                            "Only one argument expected",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    switch (result.Result)
                    {
                        case DialogButtons.OKAY:

                            try
                            {
                                _activeDiagram.AddClass(enteredName);
                                RaiseAlert(
                                    "Class Added",
                                    $"Class {enteredName} created",
                                    "",
                                    AlertIcon.INFO);
                            }

                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Class Creation Failed",
                                    $"Could not create class {enteredName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }
                            break;
                    }
                });
            });
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

            // Commented out because gui will be reworked
            // Create CurrentClass for use in reaching its attributes
            /*Class currentClass = _activeDiagram.GetClassByName(targetClassName);

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
            }*/

            ClearInputBox();
            _outputBox.Text = string.Format("Class Deleted {0}", words[0]);
        }

        private void Class_RenameClass_OnClick(object sender, RoutedEventArgs e)
        {
            ModalDialog RenameClassModal = ModalDialog.CreateDialog<RenameClassPanel>("Rename Class", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = RenameClassModal.ShowDialog<DialogButtons>(this);
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    
                    string oldName = RenameClassModal.GetPrompt<RenameClassPanel>().OldName;
                    string newName = RenameClassModal.GetPrompt<RenameClassPanel>().NewName;
                    
                    if (oldName is null || oldName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Class Rename Failed", 
                            "Could Not Rename Class",
                            "The old name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(oldName, 1))
                    {
                        RaiseAlert(
                            "Class Rename Failed", 
                            "Could Not Rename Class",
                            "Only one argument expected for old name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (newName is null || newName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Class Rename Failed", 
                            "Could Not Rename Class",
                            "The new name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(newName, 1))
                    {
                        RaiseAlert(
                            "Class Rename Failed", 
                            "Could Not Rename Class",
                            "Only one argument expected for new name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                
                    switch (result.Result)
                    {

                        case DialogButtons.OKAY:

                            try
                            {

                                _activeDiagram.RenameClass(oldName,newName);
                                RaiseAlert(
                                    "Class Renamed",
                                    $"Class {oldName} renamed to {newName}",
                                    "",
                                    AlertIcon.INFO);

                            }

                            catch (Exception e)
                            {
                            
                                RaiseAlert(
                                    "Class Rename Failed",
                                    $"Could not rename class {oldName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            
                            }
                        
                            break;
                    }
                });
            });
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

        private void TestModal_OnClick(object sender, RoutedEventArgs e)
        {
            var window = ModalDialog.CreateDialog<AlertPanel>("Oh No!", DialogButtons.OKAY);
            window.GetPrompt<AlertPanel>().AlertTitle = "Could not create class";
            window.GetPrompt<AlertPanel>().AlertMessage = "Class \"1234\" is not a valid class name";
            window.GetPrompt<AlertPanel>().DialogIcon = AlertIcon.QUESTION;
            window.ShowDialog(this);

            // var window = new GenericModal();
            // window.Show();
        }

        private void RaiseAlert(string windowTitle, string messageTitle, string messageBody, AlertIcon alertIcon)
        {
            ModalDialog AlertDialog = ModalDialog.CreateDialog<AlertPanel>(messageTitle, DialogButtons.OKAY);
            AlertPanel content = AlertDialog.GetPrompt<AlertPanel>();
            
            content.AlertTitle = messageTitle;
            content.AlertMessage = messageBody;
            content.DialogIcon = alertIcon;

            AlertDialog.ShowDialog(this);
        }

        private void Add_Method_OnCLick(object sender, RoutedEventArgs e)
        {
            ModalDialog AddMethodModal = ModalDialog.CreateDialog<AddMethodPanel>("Add New Method", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddMethodModal.ShowDialog<DialogButtons>(this);
            
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    string className = AddMethodModal.GetPrompt<AddMethodPanel>().ClassName;
                    string methodName = AddMethodModal.GetPrompt<AddMethodPanel>().MethodName;
                    string returnType = AddMethodModal.GetPrompt<AddMethodPanel>().ReturnType;
                    
                    Class currentClass = _activeDiagram.GetClassByName(className);
                    
                    if (className is null || className.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Create Method",
                            "The class name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(className, 1))
                    {
                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Creation Method",
                            "Only one argument expected for class name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (methodName is null || methodName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Create Method",
                            "The Method name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(methodName, 1))
                    {
                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Creation Method",
                            "Only one argument expected for method name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (returnType is null || returnType.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Create Method",
                            "The return type cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(returnType, 1))
                    {
                        RaiseAlert(
                            "Method Creation Failed", 
                            "Could Not Creation Method",
                            "Only one argument expected for return type",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    switch (result.Result)
                    {
                        case DialogButtons.OKAY:

                            try
                            {

                                    currentClass.AddMethod(returnType,methodName);
                                    RaiseAlert(
                                    "Relationship Added",
                                    $"Method {methodName} with return type {returnType} created",
                                    "",
                                    AlertIcon.INFO);

                            }

                            catch (Exception e)
                            {
                            
                                    RaiseAlert(
                                    "Method Creation Failed",
                                    $"Could not create Method {methodName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            
                            }
                            break;
                    }
                });
            });
        }

        private void Rename_Field_OnClick(object sender, RoutedEventArgs e)
        {
            ModalDialog RenameFieldModal = ModalDialog.CreateDialog<RenameFieldPanel>("Rename Field", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = RenameFieldModal.ShowDialog<DialogButtons>(this);
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    string targetClass = RenameFieldModal.GetPrompt<RenameFieldPanel>().TargetClass;
                    string oldName = RenameFieldModal.GetPrompt<RenameFieldPanel>().OldName;
                    string newName = RenameFieldModal.GetPrompt<RenameFieldPanel>().NewName;
                    
                    Class currentClass = _activeDiagram.GetClassByName(targetClass);
                    
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Field Rename Failed",
                            "Could Not Rename Field",
                            "class does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (oldName is null || oldName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Field Rename Failed", 
                            "Could Not Rename Field",
                            "The old name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(oldName, 1))
                    {
                        RaiseAlert(
                            "Field Rename Failed", 
                            "Could Not Rename Field",
                            "Only one argument expected for old name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (newName is null || newName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Field Rename Failed", 
                            "Could Not Rename Field",
                            "The new name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    if (!isCorrectNumArguments(newName, 1))
                    {
                        RaiseAlert(
                            "Field Rename Failed", 
                            "Could Not Rename Field",
                            "Only one argument expected for new name",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                
                    switch (result.Result)
                    {

                        case DialogButtons.OKAY:

                            try
                            {
                                currentClass.RenameField(oldName,newName);
                                RaiseAlert(
                                    "Field Renamed",
                                    $"Field {oldName} renamed to {newName}",
                                    "",
                                    AlertIcon.INFO);
                            }

                            catch (Exception e)
                            {
                            
                                RaiseAlert(
                                    "Field Rename Failed",
                                    $"Could not rename Field {oldName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }
                            break;
                    }
                });
            });
        }

        private void Rename_Method_OnClick(object sender, RoutedEventArgs e)
        {
            ModalDialog RenameMethodModal =
                ModalDialog.CreateDialog<RenameMethodPanel>("Rename Method", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult =
                RenameMethodModal.ShowDialog<DialogButtons>(this);

            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {
                // If user presses cancel, then leave the window
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                Dispatcher.UIThread.Post(() =>
                {
                    string targetClass = RenameMethodModal.GetPrompt<RenameMethodPanel>().TargetClass;
                    string oldName = RenameMethodModal.GetPrompt<RenameMethodPanel>().OldName;
                    string newName = RenameMethodModal.GetPrompt<RenameMethodPanel>().NewName;

                    Class currentClass = _activeDiagram.GetClassByName(targetClass);

                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            "class does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    if (oldName is null || oldName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            "The old name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    if (!isCorrectNumArguments(oldName, 1))
                    {
                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            "Only one argument expected for old name",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    if (newName is null || newName.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            "The new name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    if (!isCorrectNumArguments(newName, 1))
                    {
                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            "Only one argument expected for new name",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    switch (result.Result)
                    {
                        case DialogButtons.OKAY:
                            try
                            {
                                currentClass.RenameMethod(oldName, newName);
                                RaiseAlert(
                                    "Method Renamed",
                                    $"Method {oldName} renamed to {newName}",
                                    "",
                                    AlertIcon.INFO);
                            }
                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Field Rename Failed",
                                    $"Could not rename Field {oldName}",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }

                            break;
                    }
                });
            });

        }

        private bool isCorrectNumArguments(string arguments, int numArgsExpected)
        {
            string[] args = arguments.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return args.Length == numArgsExpected;
        }
    }
}
