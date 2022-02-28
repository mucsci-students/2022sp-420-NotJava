using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Diagnostics;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using ReactiveUI;
using UMLEditor.Classes;
using UMLEditor.Exceptions;
using UMLEditor.Interfaces;
using Path = System.IO.Path;

using System.Threading;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram _activeDiagram;

        private readonly TextBox _outputBox;
        private readonly TextBox _inputBox;

        private readonly StackPanel _mainPanel;
        private readonly Canvas _canvas;

        private IDiagramFile _activeFile;

        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;

        private Button SaveDiagramButton;
        private Button LoadDiagramButton;
        
        /// <summary>
        /// Main method to create the window
        /// </summary>
        public MainWindow()
        {
            
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();   
#endif
            
            _activeDiagram = new Diagram();
            _activeFile = new JSONDiagramFile();
            
            _outputBox = this.FindControl<TextBox>("OutputBox");
            _inputBox = this.FindControl<TextBox>("InputBox");

            SaveDiagramButton = this.FindControl<Button>("SaveDiagramButton");
            LoadDiagramButton = this.FindControl<Button>("LoadDiagramButton");
            InitFileDialogs(out _openFileDialog, out _saveFileDialog, "json");

            // Get size of StackPanel
            _mainPanel = this.FindControl<StackPanel>("MainPanel");

            // return;
            _canvas = this.FindControl<Canvas>("MyCanvas");
            _canvas.Height = _mainPanel.Height;
            _canvas.Width = _mainPanel.Width;

            ClassBox c1 = this.FindControl<ClassBox>("Class1");
            ClassBox c2 = this.FindControl<ClassBox>("Class2");

            Dispatcher.UIThread.Post(() =>
            {
                
                DrawRelationship(c1, c2, "composition");                
                
            });
            
        }
        
        /// <summary>
        /// Initialize the program with the xaml specifications
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private const double SymbolWidth = 15;
        private const double SymbolHeight = 10;
        /// <summary>
        /// Draws the relationship arrow between two classes
        /// </summary>
        /// <param name="startCtrl">The source class to start drawing from</param>
        /// <param name="endCtrl">The destination class to draw to</param>
        /// <param name="relationshipType">The type of relationship to draw</param>
        private void DrawRelationship(Control startCtrl, Control endCtrl, string relationshipType)
        {
            // Calculate lengths of controls
            double startHalfWidth = startCtrl.Bounds.Width / 2;
            double startHalfHeight = startCtrl.Bounds.Height / 2;
            double endHalfWidth = endCtrl.Bounds.Width / 2;
            double endHalfHeight = endCtrl.Bounds.Height / 2;
            // Initialize points to middle of controls
            Point start = new Point(
                startCtrl.Bounds.X + startHalfWidth,
                startCtrl.Bounds.Y + startHalfHeight);
            Point end = new Point(
                endCtrl.Bounds.X + endHalfWidth,
                endCtrl.Bounds.Y + endHalfHeight);

            // Set points to draw lines
            Point midStart;
            Point midEnd;
            List<Point> diamondPoints;
            List<Point> trianglePoints;
            if (Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y))
            {
                // Arrow is horizontal
                midStart = new Point(Math.Abs(start.X + end.X) / 2, start.Y);
                midEnd = new Point(Math.Abs(start.X + end.X) / 2, end.Y);
                if (start.X < end.X)
                {
                    // Goes from left to right
                    start = new Point(start.X + startHalfWidth, start.Y);
                    end = new Point(end.X - endHalfWidth - (2 * SymbolWidth), end.Y);
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X + SymbolWidth,end.Y - SymbolHeight),
                        new(end.X + (2 * SymbolWidth),end.Y),
                        new(end.X + SymbolWidth,end.Y + SymbolHeight),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X,end.Y - SymbolHeight),
                        new(end.X + (2 * SymbolWidth),end.Y),
                        new(end.X,end.Y + SymbolHeight),
                        new(end.X,end.Y - SymbolHeight)
                    };
                }
                else
                {
                    // Goes from right to left
                    start = new Point(start.X - startHalfWidth, start.Y);
                    end = new Point(end.X + endHalfWidth + (2 * SymbolWidth), end.Y);
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X - SymbolWidth,end.Y - SymbolHeight),
                        new(end.X - (2 * SymbolWidth),end.Y),
                        new(end.X - SymbolWidth,end.Y + SymbolHeight),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X,end.Y - SymbolHeight),
                        new(end.X - (2 * SymbolWidth),end.Y),
                        new(end.X,end.Y + SymbolHeight),
                        new(end.X,end.Y - SymbolHeight)
                    };
                }
            }
            else
            {
                // Arrow is vertical
                midStart = new Point(start.X, Math.Abs(start.Y + end.Y) / 2);
                midEnd = new Point(end.X, Math.Abs(start.Y + end.Y) / 2);
                if (start.Y < end.Y)
                {
                    // Goes top to bottom
                    start = new Point(start.X, start.Y + startHalfHeight);
                    end = new Point(end.X, end.Y - endHalfHeight - (2 * SymbolWidth));
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X + SymbolHeight,end.Y + SymbolWidth),
                        new(end.X,end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight,end.Y + SymbolWidth),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X + SymbolHeight,end.Y),
                        new(end.X,end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight,end.Y),
                        new(end.X + SymbolHeight,end.Y)
                    };
                }
                else
                {
                    // Goes bottom to top
                    start = new Point(start.X, start.Y - startHalfHeight);
                    end = new Point(end.X, end.Y + endHalfHeight + (2 * SymbolWidth));
                    diamondPoints = new List<Point> { 
                        end,
                        new(end.X + SymbolHeight,end.Y - SymbolWidth),
                        new(end.X,end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight,end.Y - SymbolWidth),
                        end };
                    trianglePoints = new List<Point> { 
                        new(end.X + SymbolHeight,end.Y),
                        new(end.X,end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight,end.Y),
                        new(end.X + SymbolHeight,end.Y)
                    };
                }
            }
            Line line1 = GetLine(start,midStart);
            Line line2 = GetLine(midStart, midEnd);
            Line line3 = GetLine(midEnd, end);
            
            // Add lines to the canvas
            _canvas.Children.Add(line1);
            _canvas.Children.Add(line2);
            _canvas.Children.Add(line3);

            // Draw the relationship symbol based on provided type
            switch (relationshipType)
            {
                case "aggregation":
                    _canvas.Children.Add(GetSymbol(diamondPoints));
                    break;
                case "composition":
                    Polyline polyline = GetSymbol(diamondPoints);
                    polyline.Fill = Brushes.White;
                    _canvas.Children.Add(polyline);
                    break;
                case "inheritance": 
                    _canvas.Children.Add(GetSymbol(trianglePoints));
                    break;
                case "realization": 
                    line1.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    line2.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    line3.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    _canvas.Children.Add(GetSymbol(trianglePoints));
                    break;
            }
        }

        /// <summary>
        /// Draws the relationship symbol from the given list of points
        /// </summary>
        /// <param name="points">A list of points for the vertices to draw</param>
        /// <returns>The new relationship symbol</returns>
        private Polyline GetSymbol(List<Point> points)
        {
            Polyline polyline = new Polyline();
            polyline.Name = "Polyline";
            polyline.Points = points;
            polyline.Stroke = Brushes.White;
            polyline.StrokeThickness = 2;
            return polyline;
        }

        /// <summary>
        /// Creates a line from the given start to end points
        /// </summary>
        /// <param name="lineStart">Point to start at</param>
        /// <param name="lineEnd">Point to end at</param>
        /// <returns>The new line</returns>
        private Line GetLine(Point lineStart, Point lineEnd)
        {
            Line l = new Line();
            l.Name = "Line";
            l.StartPoint = lineStart;
            l.EndPoint = lineEnd;
            l.Stroke = Brushes.White;
            l.StrokeThickness = 2;
            l.ZIndex = 10;
            return l;
        }

        /// <summary>
        /// Removes all drawn lines from the canvas
        /// </summary>
        private void ClearLines()
        {
            List<IControl> children = new List<IControl>();
            foreach (IControl child in _canvas.Children)
            {
                children.Add(child);
            }

            foreach (IControl child in children)
            {
                if (child.Name == "Line" || child.Name == "Polyline")
                {
                    _canvas.Children.Remove(child);
                }
            }
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
            RaiseAlert(
                "Help", 
                "",
                "", // TODO Figure out the wording for the new help functionality
                AlertIcon.INFO
            );
            return;
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
        }
        
        private void List_Relationships_OnClick(object sender, RoutedEventArgs e)
        {
            _outputBox.Text = _activeDiagram.ListRelationships();
        }

        private void AddRelationship_OnClick(object sender, RoutedEventArgs e)
        {
            // Create a new modal dialogue and wire it up to the 'AddRelationshipPanel'
            ModalDialog addRelationshipModal = ModalDialog.CreateDialog<AddRelationshipPanel>("Add New Relationship", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = addRelationshipModal.ShowDialog<DialogButtons>(this);
            
            // Spin up the result
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {
                // Case where user does not select OKAY button.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                // Dispatching a UIThread to ensure the body is executed in thread-safe manor.
                Dispatcher.UIThread.Post(() =>
                {
                    // Creating the variables that we will be snagging from the 'AddRelationshipPanel'
                    string sourceName = addRelationshipModal.GetPrompt<AddRelationshipPanel>().SourceClass;
                    string destinationName = addRelationshipModal.GetPrompt<AddRelationshipPanel>().DestinationClass;
                    string relationshipType = addRelationshipModal.GetPrompt<AddRelationshipPanel>().SelectedType;
                  
                    // Verification to check if no input was added
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
                    // Verification to check if no input was added
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
                    // Verification to check if no input was added
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
                    switch (result.Result)
                    {
                        // If OKAY was selected...
                        case DialogButtons.OKAY:

                            try
                            {
                                // Attempt to create a new relationship with the information given.  If succeeds raise an alert.
                                _activeDiagram.AddRelationship(sourceName,destinationName,relationshipType);
                                RaiseAlert(
                                    "Relationship Added",
                                    $"Relationship '{sourceName} => {destinationName}' of type '{relationshipType}' created",
                                    "",
                                    AlertIcon.INFO);
                            }
                            // Alert if the add fails.
                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Class Creation Failed",
                                    $"Could not create relationship '{sourceName} => {destinationName}'",
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
            // Create a new modal dialogue and wire it up to 'AddFieldPanel'
            ModalDialog AddFieldModal = ModalDialog.CreateDialog<AddFieldPanel>("Add New Field", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddFieldModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a result
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {
                // Case where 'OKAY' was not selected, return.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                // Dispatch a UIThread to execute in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    // Variables that we need to snag from the 'AddFieldPanel'
                    string targetClass = AddFieldModal.GetPrompt<AddFieldPanel>().ClassName;
                    string targetField = AddFieldModal.GetPrompt<AddFieldPanel>().FieldName;
                    string fieldType = AddFieldModal.GetPrompt<AddFieldPanel>().FieldType;
                    
                    // Taking the targetClass string and finding a corresponding class in diagram.
                    Class currentClass = _activeDiagram.GetClassByName(targetClass);
                    
                    // If input is empty...
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
                    // If class does not exist...
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Field Creation Failed",
                            "Could Not Create Field",
                            $"class '{targetClass}' does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    // If input is left empty...
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

                    // If input is left empty...
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

                    switch (result.Result)
                    {
                        // In the case of clicking 'OKAY'
                        case DialogButtons.OKAY:

                            try
                            {
                                // Try to create a new field with the information given.  Raise an alert on succeed.
                                currentClass.AddField(fieldType,targetField);
                                RaiseAlert(
                                    "Field Added",
                                    $"Field '{targetField}' with type '{fieldType}' created",
                                    "",
                                    AlertIcon.INFO);
                            }

                            // On failure, raise an alert.
                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Field Creation Failed",
                                    $"Could not create field '{targetField}'",
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
            
            ClearInputBox();
            _outputBox.Text = string.Format("Attribute Deleted ({0} => {1})", targetClassName, targetAttributeName);
        }

        private void Class_AddClass_OnClick (object sender, RoutedEventArgs e)
        {
            // Create and wire up a new modal dialogue to the 'AddClassPanel'
            ModalDialog AddClassModal = ModalDialog.CreateDialog<AddClassPanel>("Add New Class", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddClassModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a result
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {
                
                // Case in which 'OKAY' is not selected.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                // Dispatch a UIThread to execute body in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    // Variables used in the creation of a new class
                    string enteredName = AddClassModal.GetPrompt<AddClassPanel>().ClassName;
                    
                    // If the input is left empty...
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
                    switch (result.Result)
                    {
                        // If the user selects 'OKAY'
                        case DialogButtons.OKAY:
                            try
                            {
                                // Attempt to create a new class with the given information.  Alert if succeeds
                                _activeDiagram.AddClass(enteredName);
                                RaiseAlert(
                                    "Class Added",
                                    $"Class '{enteredName}' created",
                                    "",
                                    AlertIcon.INFO);
                            }
                            // If fails, raise an alert.
                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Class Creation Failed",
                                    $"Could not create class '{enteredName}'",
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

            ClearInputBox();
            _outputBox.Text = string.Format("Class Deleted {0}", words[0]);
        }

        private void Class_RenameClass_OnClick(object sender, RoutedEventArgs e)
        {
            // Create and wire up a new modal dialog to the 'RenameClassPanel'
            ModalDialog RenameClassModal = ModalDialog.CreateDialog<RenameClassPanel>("Rename Class", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = RenameClassModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a new dialog
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                // Case in which user does not select 'OKAY'
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                // Dispatch UIThread to execute in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    
                    // Variables that we will be using from the modal input
                    string oldName = RenameClassModal.GetPrompt<RenameClassPanel>().OldName;
                    string newName = RenameClassModal.GetPrompt<RenameClassPanel>().NewName;
                    
                    // If input is left empty...
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
                    
                    // If input is left empty...
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

                    switch (result.Result)
                    {
    
                        // Case in which 'OKAY' is entered
                        case DialogButtons.OKAY:

                            try
                            {

                                // Attempt to rename class with given information.  If succeeds raise alert.
                                _activeDiagram.RenameClass(oldName,newName);
                                RaiseAlert(
                                    "Class Renamed",
                                    $"Class '{oldName}' renamed to '{newName}'",
                                    "",
                                    AlertIcon.INFO);

                            }

                            // If fails raise alert...
                            catch (Exception e)
                            {
                            
                                RaiseAlert(
                                    "Class Rename Failed",
                                    $"Could not rename class '{oldName}'",
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

                throw new NotImplementedException("Delete relationship needs to be refactored");
                // _activeDiagram.DeleteRelationship(sourceClassName, destClassName);

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

        private void Add_Method_OnClick(object sender, RoutedEventArgs e)
        {
            // Create a new modal dialog and wire it up to the 'AddMethodPanel'
            ModalDialog AddMethodModal = ModalDialog.CreateDialog<AddMethodPanel>("Add New Method", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = AddMethodModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a new result
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                // Case in which 'OKAY' is not selected
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                // Dispatch a UIThread to execute in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    // Variables we would like to obtain from the 'AddMethodPanel' modal
                    string className = AddMethodModal.GetPrompt<AddMethodPanel>().ClassName;
                    string methodName = AddMethodModal.GetPrompt<AddMethodPanel>().MethodName;
                    string returnType = AddMethodModal.GetPrompt<AddMethodPanel>().ReturnType;
                    
                    // Taking the className string and accessing a corresponding class in active diagram
                    Class currentClass = _activeDiagram.GetClassByName(className);
                    
                    // If the input is left empty...
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
                    // If the class does not exist...
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Method Creation Failed",
                            "Could Not Create Method",
                            $"class '{className}' does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    // if input is left empty....
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

                    // If input is left empty...
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

                    switch (result.Result)
                    {
                        // In the case of selecting 'OKAY'
                        case DialogButtons.OKAY:

                            try
                            {

                                    // Attempt to enter a new method using given information.  Raise alert if succeed.
                                    currentClass.AddMethod(returnType,methodName);
                                    RaiseAlert(
                                    "Relationship Added",
                                    $"Method '{methodName}' with return type '{returnType}' created",
                                    "",
                                    AlertIcon.INFO);

                            }

                            // Raise an exception if fail.
                            catch (Exception e)
                            {
                            
                                    RaiseAlert(
                                    "Method Creation Failed",
                                    $"Could not create Method '{methodName}'",
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
            // Create a new modal dialogue and wire up to 'RenameFieldPanel'
            ModalDialog RenameFieldModal = ModalDialog.CreateDialog<RenameFieldPanel>("Rename Field", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = RenameFieldModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a new result.
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {

                // Case in which 'OKAY' is not selected.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                // Dispatch a new UIThread to execute in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    // Variables we would like to get from the modal dialogue
                    string targetClass = RenameFieldModal.GetPrompt<RenameFieldPanel>().TargetClass;
                    string oldName = RenameFieldModal.GetPrompt<RenameFieldPanel>().OldName;
                    string newName = RenameFieldModal.GetPrompt<RenameFieldPanel>().NewName;
                    
                    // Using the string targetClass to get a corresponding class in diagram
                    Class currentClass = _activeDiagram.GetClassByName(targetClass);
                    
                    // If the input is left empty...
                    if (targetClass is null || targetClass.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Field Rename Failed", 
                            "Could Not Rename Field",
                            "The class name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    // If the class does not exist...
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Field Rename Failed",
                            "Could Not Rename Field",
                            $"class '{targetClass}' does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    
                    // If input is left empty...
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

                    // If input is left empty...
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

                    switch (result.Result)
                    {

                        // If user selects 'OKAY'
                        case DialogButtons.OKAY:

                            try
                            {
                                // Try to rename field using the given information.  Raise alert if succeeds.
                                currentClass.RenameField(oldName,newName);
                                RaiseAlert(
                                    "Field Renamed",
                                    $"Field '{oldName}' renamed to '{newName}'",
                                    "",
                                    AlertIcon.INFO);
                            }

                            // Throw an exception if fails...
                            catch (Exception e)
                            {
                            
                                RaiseAlert(
                                    "Field Rename Failed",
                                    $"Could not rename Field '{oldName}'",
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
            // Create a new modal dialogue and wire up to 'RenameMethodPanel'
            ModalDialog RenameMethodModal =
                ModalDialog.CreateDialog<RenameMethodPanel>("Rename Method", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult =
                RenameMethodModal.ShowDialog<DialogButtons>(this);

            // Spin up a new result
            modalResult.ContinueWith((Task<DialogButtons> result) =>
            {
                // If user presses cancel, then leave the window
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                
                // Dispatch UIThread to execute in a thread-safe manor
                Dispatcher.UIThread.Post(() =>
                {
                    // Information we'd like from the modal dialogue
                    string targetClass = RenameMethodModal.GetPrompt<RenameMethodPanel>().TargetClass;
                    string oldName = RenameMethodModal.GetPrompt<RenameMethodPanel>().OldName;
                    string newName = RenameMethodModal.GetPrompt<RenameMethodPanel>().NewName;

                    // Use targetClass to acquire a corresponding string in diagram
                    Class currentClass = _activeDiagram.GetClassByName(targetClass);

                    // If the input is left empty...
                    if (targetClass is null || targetClass.Trim().Length == 0)
                    {

                        RaiseAlert(
                            "Method Rename Failed", 
                            "Could Not Rename Method",
                            "The class name cannot be empty",
                            AlertIcon.ERROR
                        );
                        return;
                    }
                    // If the class does not exist...
                    if (currentClass is null)
                    {
                        RaiseAlert(
                            "Method Rename Failed",
                            "Could Not Rename Method",
                            $"class '{targetClass}' does not exist",
                            AlertIcon.ERROR
                        );
                        return;
                    }

                    // If the input is left empty...
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

                    // If input is left empty...
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

                    switch (result.Result)
                    {
                        // If the users selects 'OKAY'
                        case DialogButtons.OKAY:
                            try
                            {
                                // Try to rename method with given information.  Raise alert if succeeds.
                                currentClass.RenameMethod(oldName, newName);
                                RaiseAlert(
                                    "Method Renamed",
                                    $"Method '{oldName}' renamed to '{newName}'",
                                    "",
                                    AlertIcon.INFO);
                            }
                            // If fails, raise an alert.
                            catch (Exception e)
                            {
                                RaiseAlert(
                                    "Field Rename Failed",
                                    $"Could not rename Field '{oldName}'",
                                    e.Message,
                                    AlertIcon.ERROR
                                );
                            }

                            break;
                    }
                });
            });

        }

        /// <summary>
        ///  Raises and alert with the given parameters
        /// </summary>
        /// <param name="windowTitle">The desired title for the raised alert</param>
        /// <param name="messageTitle">The desired message title for the raised alert</param>
        /// <param name="messageBody">The message body for the raise alert</param>
        /// <param name="alertIcon">The icon you would like present within the alert</param>
        private void RaiseAlert(string windowTitle, string messageTitle, string messageBody, AlertIcon alertIcon)
        {
            // Create and wire up a new modal dialogue to 'AlertPanel' with the parameters being a title and the visible buttons.
            ModalDialog AlertDialog = ModalDialog.CreateDialog<AlertPanel>(windowTitle, DialogButtons.OKAY);
            AlertPanel content = AlertDialog.GetPrompt<AlertPanel>();
            
            // Fill the content, alert message, and icon depending on the situation in which the alert is being raised.
            content.AlertTitle = messageTitle;
            content.AlertMessage = messageBody;
            content.DialogIcon = alertIcon;

            AlertDialog.ShowDialog(this);
        }
    }
    
    
}
