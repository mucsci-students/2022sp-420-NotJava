using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using UMLEditor.Classes;
using UMLEditor.Interfaces;
using UMLEditor.Utility;
using UMLEditor.ViewModels;
using UMLEditor.Views.Managers;
using AStarSharp;

// ReSharper disable UnusedParameter.Local

namespace UMLEditor.Views
{
    /// <summary>
    /// MainWindow.cs
    /// </summary>
    public class MainWindow : Window
    {

        private Diagram _activeDiagram;
        private List<ClassBox> _classBoxes;

        private List<ClassBox> DrawnClassBoxes
        {

            get
            {

                List<ClassBox> result = new List<ClassBox>();

                foreach (var child in _canvas.Children)
                {

                    if (child.GetType() == typeof(ClassBox))
                    {
                        
                        result.Add((ClassBox)child);
                        
                    }
                    
                }
                
                return result;

            }
            
        }
        
        private string[] ClassNames
        {

            get
            {
                
                // Get an array of all class names in the diagram
                List<Class> currentClasses = _activeDiagram.Classes;
                string[] classNames = new string[currentClasses.Count];
                for (int indx = 0; indx < classNames.Length; ++indx)
                {
                    classNames[indx] = currentClasses[indx].ClassName;
                }

                return classNames;

            }

        }
        
        private readonly Canvas _canvas;

        private List<RelationshipLine> _relationshipLines = new();

        private IDiagramFile _activeFile;

        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        
        private readonly SaveFileDialog _exportDialog;
        private readonly OpenFileDialog _openThemeDialog;
        private readonly SaveFileDialog _saveThemeDialog;
        
        private bool _inEditMode;

        // The undo and redo buttons
        private readonly Button _undoButton;
        private readonly Button _redoButton;

        private static readonly string _themeExtension = "umltheme";
        private readonly string _themeFileLocation = $"{OSUtility.HomeFolder}notjavauml.{_themeExtension}";
        
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

            _classBoxes = new List<ClassBox>();
            _activeFile = new JSONDiagramFile();

            InitFileDialogs(out _openFileDialog, out _saveFileDialog, "json");
            InitExportDialog(out _exportDialog, "png");
            InitThemeDialogs(out _openThemeDialog, out _saveThemeDialog, _themeExtension);

            _canvas = this.FindControl<Canvas>("MyCanvas");
            _inEditMode = false;
            
            // Build grid for line placement
            RelationshipLine.InitializeGrid(_canvas);

            // Grab the undo and redo buttons
            _undoButton = this.FindControl<Button>("UndoButton");
            _redoButton = this.FindControl<Button>("RedoButton");

            // Push initial state to the TimeMachine
            TimeMachine.AddState(_activeDiagram);
            
            // Bind to the Diagram changed event to enable/ disable buttons from diagram changes
            Diagram.DiagramChanged += (sender, _) =>
            {

                ReconsiderUndoRedoVisibility();

            };
            
            // Bind to key combo events
            Dictionary<string, Action> comboBindings = new();
            MainViewModel.KeyComboIssued += (sender, combo) =>
            {

                // Invoke the action for this key combo
                comboBindings[combo].Invoke();

            };

            // Assign key combo actions
            comboBindings["ctrl+o"] = () => LoadButton_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+s"] = () => Save_Button_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+t"] = () =>
            {

                ToggleSwitch ts = this.FindControl<ToggleSwitch>("ViewEditToggle");
                if (ts.IsChecked is not null)
                {
                    
                    // Toggle the switch
                    ts.IsChecked = !ts.IsChecked;                    
                
                    // Fire the changed event
                    ViewEditToggle_OnClick(ts, new RoutedEventArgs());
                    
                }
                
            };
            
            comboBindings["ctrl+z"] = () =>
            {

                if (TimeMachine.PreviousStateExists())
                {
                    UndoButton_OnClick(this, new RoutedEventArgs());
                }
                
            };
            
            comboBindings["ctrl+y"] = () =>
            {

                if (TimeMachine.NextStateExists())
                {
                    RedoButton_OnClick(this, new RoutedEventArgs());
                }
                
            };

            comboBindings["f1"] = () => HelpB_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+c"] = () => Class_AddClass_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+shift+c"] = () => Change_Relationship_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+r"] = () => Add_Relationship_OnClick(this, new RoutedEventArgs());
            comboBindings["ctrl+d"] = () => Delete_Relationship_OnClick(this, new RoutedEventArgs());

            // Bind to the theme changed event
            Theme.ThemeChanged += (sender, newTheme) => this.Background = newTheme.CanvasColor;

            // Try to load the user's theme
            Theme.TryLoadTheme(_themeFileLocation);

        }
        
        /// <summary>
        /// Initialize the program with the xaml specifications
        /// </summary>
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
                filter.Name = $".{extension} Diagram Files";
                filter.Extensions.Add(extension);

                openFD.Filters.Add(filter);
                saveFD.Filters.Add(filter);
                
            }
        }

        /// <summary>
        /// Initializes an export file dialog with the provided extensions
        /// </summary>
        /// <param name="toInit">The dialog to initialize</param>
        /// <param name="filteredExtensions">The extensions this export dialog should support</param>
        private void InitExportDialog(out SaveFileDialog toInit, params string[] filteredExtensions)
        {
            
            string workingDir = Directory.GetCurrentDirectory();
            toInit = new SaveFileDialog();
            toInit.Title = "Export Diagram";
            toInit.Directory = workingDir;

            foreach (string extension in filteredExtensions)
            {
                // Establish a filter for the current file extension
                FileDialogFilter filter = new FileDialogFilter();
                filter.Name = $".{extension} file";
                filter.Extensions.Add(extension);

                toInit.Filters.Add(filter);
                
            }
            
        }

        /// <summary>
        /// Initializes a file dialog for opening themes with the provided extensions
        /// </summary>
        /// <param name="openDialog">The open dialog to initialize</param>
        /// <param name="saveDialog">The save dialog to initialize</param>
        /// <param name="filteredExtensions">The extensions this theme dialog should support</param>
        private void InitThemeDialogs(out OpenFileDialog openDialog, out SaveFileDialog saveDialog, params string[] filteredExtensions)
        {
            
            string workingDir = Directory.GetCurrentDirectory();
            openDialog = new OpenFileDialog();
            saveDialog = new SaveFileDialog();
            openDialog.Title = "Load Theme File";
            saveDialog.Title = "Save Theme File";
            openDialog.Directory = workingDir;
            saveDialog.Directory = workingDir;

            foreach (string extension in filteredExtensions)
            {
                // Establish a filter for the current file extension
                FileDialogFilter filter = new FileDialogFilter();
                filter.Name = $".{extension} theme file";
                filter.Extensions.Add(extension);

                openDialog.Filters.Add(filter);
                saveDialog.Filters.Add(filter);
                
            }
            
        }
        
        private void ExitB_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Environment.Exit(0);
        }

        private void HelpB_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            string link = "https://github.com/mucsci-students/2022sp-420-NotJava#gui-mode-help";
            RaiseAlert(
                "Help", 
                "",
                $"For detailed help instructions, see\n '{link}'", 
                AlertIcon.INFO
            );
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(link)
                    {UseShellExecute = true});
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", link);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", link);
            }
        }

        private void Save_Button_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            /* Open the file save dialog on its own thread
             * Obtain a future from this action */
            Task<string?> saveTask = _saveFileDialog.ShowAsync(this);
            saveTask.ContinueWith(finishedTask =>
            {
                
                Dispatcher.UIThread.Post(() => { 
                
                    // Grab the selected output file
                    string? selectedFile = finishedTask.Result;

                    // Make sure a file was selected
                    if (selectedFile != null)
                    {
                        try
                        {
                            _activeFile.SaveDiagram(ref _activeDiagram, selectedFile);
                            RaiseAlert(
                                "Save Successful",
                                $"Save Successful",
                                $"Current diagram saved to '{selectedFile}'",
                                AlertIcon.INFO
                            );
                        }

                        catch (Exception exception)
                        {
                            RaiseAlert(
                                "Save Failed",
                                $"Save Failed",
                                exception.Message,
                                AlertIcon.ERROR
                            );
                        }
                    }
                
                });
                
            });
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            /* Open the file selection dialog on its own thread
             * Obtain a future from this action */
            Task<string[]?> loadTask = _openFileDialog.ShowAsync(this);
            loadTask.ContinueWith(taskResult =>
            {
                // Called when the future is resolved
                Dispatcher.UIThread.Post(() =>
                {
                    
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
                            
                            _activeDiagram = _activeFile.LoadDiagram(chosenFile)!;
                            
                            // Wipe all ClassBox registrations and redraws the canvas
                            RedrawEverything();
                            
                            // Reset TM & push new state
                            TimeMachine.ClearTimeMachine();
                            TimeMachine.AddState(_activeDiagram);
                            ReconsiderUndoRedoVisibility();
                            
                            ReconsiderCanvasSize();
                            
                        }
            
                        catch (Exception exception)
                        {

                            RaiseAlert(
                                "Load Failed",
                                $"Load Failed",
                                exception.Message,
                                AlertIcon.ERROR
                            );

                        }
                    }
                    
                });
                
            });
        }
        private void Class_AddClass_OnClick (object sender, RoutedEventArgs routedEventArgs)
        {
            // Create and wire up a new modal dialogue to the 'AddClassPanel'
            ModalDialog addClassModal = ModalDialog.CreateDialog<AddClassPanel>("Add New Class", DialogButtons.OK_CANCEL);
            Task<DialogButtons> modalResult = addClassModal.ShowDialog<DialogButtons>(this);
            
            // Spin up a result
            modalResult.ContinueWith(result =>
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
                    string enteredName = addClassModal.GetPrompt<AddClassPanel>().ClassName;
                    // If the user selects 'OKAY'
                    try
                    {
                        // Attempt to create a new class with the given information.
                        _activeDiagram.AddClass(enteredName);
                        RenderClasses(enteredName);
                        ReconsiderCanvasSize();
                    }
                    // If fails, raise an alert.
                    catch (Exception error)
                    {
                        RaiseAlert(
                            "Class Creation Failed",
                            $"Could not create class '{enteredName}'",
                            error.Message,
                            AlertIcon.ERROR
                        );
                    }
                });
            });
        }

        private ClassBox GetClassBoxByName(string fromName) => ClassBoxes.FindByName(fromName);

        private RelationshipLine? GetRelationshipByClassNames(string sourceName, string destinationName)
        {
            RelationshipLine? foundLine = null;
            foreach (var line in _relationshipLines)
            {
                if (line.SourceClass.Name == sourceName &&
                    line.DestClass.Name == destinationName)
                {
                    foundLine = line;
                }
            }

            return foundLine;
        }

        /// <summary>
        /// Event handler to add a relationship
        /// </summary>
        private void Add_Relationship_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {

            // Create a new modal dialogue and wire it up to the 'AddRelationshipPanel'
            ModalDialog addRelationshipModal = 
                ModalDialog.CreateDialog<AddRelationshipPanel>("Add New Relationship", DialogButtons.OK_CANCEL);
            
            // Load the class names onto the prompt
            addRelationshipModal.GetPrompt<AddRelationshipPanel>().LoadClassNames(ClassNames);
            Task<DialogButtons> modalResult = addRelationshipModal.ShowDialog<DialogButtons>(this);

            // Spin up the result
            modalResult.ContinueWith(result =>
            {
                // Case where user does not select OKAY button.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }

                // Dispatching a UIThread to ensure the body is executed in thread-safe manner.
                Dispatcher.UIThread.Post(() =>
                {
                    // Creating the variables that we will be snagging from the 'AddRelationshipPanel'
                    string sourceName = addRelationshipModal.GetPrompt<AddRelationshipPanel>().SourceClass;
                    string destinationName =
                        addRelationshipModal.GetPrompt<AddRelationshipPanel>().DestinationClass;
                    string relationshipType = addRelationshipModal.GetPrompt<AddRelationshipPanel>().SelectedType;

                    try
                    {
                        // Attempt to create a new relationship with the information given.
                        _activeDiagram.AddRelationship(sourceName,destinationName,relationshipType);
                        ClassBox sourceClassBox = GetClassBoxByName(sourceName);
                        ClassBox destClassBox = GetClassBoxByName(destinationName);
                        RelationshipLine currentLine =
                            new RelationshipLine(sourceClassBox, destClassBox, relationshipType);
                        currentLine.Draw(_canvas);
                    }
                    // Alert if the add fails.
                    catch (Exception error)
                    {
                        RaiseAlert(
                            "Relationship Creation Failed",
                            $"Could not create relationship '{sourceName} => {destinationName}'",
                            error.Message,
                            AlertIcon.ERROR
                        );
                    }
                });
            });

        }

        /// <summary>
        ///  Event handler to change a relationship's type
        /// </summary>
        private void Change_Relationship_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            
            // Create a new modal dialogue and wire it up to the 'ChangeRelationshipPanel'
            ModalDialog changeRelationshipModal = ModalDialog.CreateDialog<AddRelationshipPanel>("Change Relationship Type", DialogButtons.OK_CANCEL);
            
            // Load the class names onto the prompt
            changeRelationshipModal.GetPrompt<AddRelationshipPanel>().LoadClassNames(ClassNames);
            Task<DialogButtons> modalResult = changeRelationshipModal.ShowDialog<DialogButtons>(this);
            
            // Spin up the result
            modalResult.ContinueWith(result =>
            {
                // Case where user does not select OKAY button.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                // Dispatching a UIThread to ensure the body is executed in thread-safe manner.
                Dispatcher.UIThread.Post(() =>
                {
                    // Creating the variables that we will be snagging from the 'AddRelationshipPanel'
                    string sourceName = changeRelationshipModal.GetPrompt<AddRelationshipPanel>().SourceClass;
                    string destinationName = changeRelationshipModal.GetPrompt<AddRelationshipPanel>().DestinationClass;
                    string relationshipType = changeRelationshipModal.GetPrompt<AddRelationshipPanel>().SelectedType;
                  
                   try
                    {
                        // Attempt to change a relationship type with the information given.
                        _activeDiagram.ChangeRelationship(sourceName,destinationName,relationshipType);
                        RelationshipLine currentLine = GetRelationshipByClassNames(sourceName, destinationName)!;
                        ClassBox sourceClassBox = GetClassBoxByName(sourceName);
                        ClassBox destClassBox = GetClassBoxByName(destinationName);
                        RelationshipLine newLine = new RelationshipLine(sourceClassBox, destClassBox, relationshipType);
                        ClearAllLines();
                        _relationshipLines.Remove(currentLine);
                        RedrawLines();
                        newLine.Draw(_canvas);
                    }
                    // Alert if the change fails.
                    catch (Exception error)
                    {
                        RaiseAlert(
                            "Type Change Failed",
                            $"Could not change type to '{relationshipType}'",
                            error.Message,
                            AlertIcon.ERROR
                        );
                    }
                });
            });
        }

        /// <summary>
        ///  Event handler to delete a relationship
        /// </summary>
        private void Delete_Relationship_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            // Create a new modal dialogue and wire it up to the 'DeleteRelationshipPanel'
            ModalDialog deleteRelationshipModal = ModalDialog.CreateDialog<AddRelationshipPanel>("Delete Relationship", DialogButtons.OK_CANCEL);
            
            // Load class names, disable type selection
            deleteRelationshipModal.GetPrompt<AddRelationshipPanel>().LoadClassNames(ClassNames);
            deleteRelationshipModal.GetPrompt<AddRelationshipPanel>().HideTypeSelection();
            Task<DialogButtons> modalResult = deleteRelationshipModal.ShowDialog<DialogButtons>(this);
            
            // Spin up the result
            modalResult.ContinueWith(result =>
            {
                // Case where user does not select OKAY button.
                if (result.Result != DialogButtons.OKAY)
                {
                    return;
                }
                // Dispatching a UIThread to ensure the body is executed in thread-safe manor.
                Dispatcher.UIThread.Post(() =>
                {
                    // Creating the variables that we will be snagging from the 'DeleteRelationshipPanel'
                    string sourceName = deleteRelationshipModal.GetPrompt<AddRelationshipPanel>().SourceClass;
                    string destinationName = deleteRelationshipModal.GetPrompt<AddRelationshipPanel>().DestinationClass;

                    try
                    {
                        // Attempt to delete relationship with the information given.
                        _activeDiagram.DeleteRelationship(sourceName,destinationName);
                        RelationshipLine currentLine = GetRelationshipByClassNames(sourceName, destinationName)!;
                        ClearAllLines();
                        _relationshipLines.Remove(currentLine);
                        RedrawLines();
                    }
                    // Alert if the delete fails.
                    catch (Exception error)
                    {
                        RaiseAlert(
                            "Relationship Delete Failed",
                            $"Could not delete relationship '{sourceName} => {destinationName}'",
                            error.Message,
                            AlertIcon.ERROR
                        );
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
        public void RaiseAlert(string windowTitle, string messageTitle, string messageBody, AlertIcon alertIcon)
        {
            // Create and wire up a new modal dialogue to 'AlertPanel' with the parameters being a title and the visible buttons.
            ModalDialog alertDialog = ModalDialog.CreateDialog<AlertPanel>(windowTitle, DialogButtons.OKAY);
            AlertPanel content = alertDialog.GetPrompt<AlertPanel>();
            
            // Fill the content, alert message, and icon depending on the situation in which the alert is being raised.
            content.AlertTitle = messageTitle;
            content.AlertMessage = messageBody;
            content.DialogIcon = alertIcon;

            alertDialog.ShowDialog(this);
        }

        /// <summary>
        /// Raises a yes/ no confirmation
        /// </summary>
        /// <param name="windowTitle">The title of the window</param>
        /// <param name="messageTitle">The title of the message</param>
        /// <param name="messageBody">The body of the message</param>
        /// <param name="alertIcon">The icon to use for the alert</param>
        /// <param name="callback">A callback function to use when the dialog is resolved</param>
        public void RaiseConfirmation(string windowTitle, string messageTitle, string messageBody, AlertIcon alertIcon, Action<Task<DialogButtons>> callback)
        {
            
            // Create and wire up a new modal dialogue to 'AlertPanel' with the parameters being a title and the visible buttons.
            ModalDialog alertDialog = ModalDialog.CreateDialog<AlertPanel>(windowTitle, DialogButtons.YES_NO);
            AlertPanel content = alertDialog.GetPrompt<AlertPanel>();
            
            // Fill the content, alert message, and icon depending on the situation in which the alert is being raised.
            content.AlertTitle = messageTitle;
            content.AlertMessage = messageBody;
            content.DialogIcon = alertIcon;

            alertDialog.ShowDialog<DialogButtons>(this).ContinueWith(callback);

        }

        /// <summary>
        /// Takes the bounds of the class box on the canvas, converts them to grid coordinates and sets the
        /// grid locations to non-walkable
        /// </summary>
        /// <param name="classBox">Class box to add non-walkable locations to the grid</param>
        private void AddClassBoxToGrid(ClassBox classBox)
        {
            // Add Class Box region to list of not walkable grid nodes
            for (int y = (int)(classBox.Bounds.Y / Node.NODE_SIZE)-1;
                 y <= (int)((classBox.Bounds.Y + classBox.Bounds.Height) / Node.NODE_SIZE)+1;
                 ++y)
            {
                for (int x= (int)(classBox.Bounds.X / Node.NODE_SIZE)-1;
                     x <= (int)((classBox.Bounds.X + classBox.Bounds.Width) / Node.NODE_SIZE)+1;
                     ++x)
                {
                    RelationshipLine.MakeNotWalkable(x, y);
                }
            }
        }
        
        /// <summary>
        /// Renders a list of classes, by provided name. Registers the ClassBoxes in the ClassBox manager
        /// </summary>
        /// <param name="withName">The names of classes to be rendered.
        /// The rendered classes will be default boxes with only the name being different.</param>
        private void RenderClasses(params string[] withName)
        {
            
            foreach (string currentClassName in withName)
            {
                
                ClassBox newClass = new ClassBox(currentClassName, ref _activeDiagram, this, _inEditMode);
                _classBoxes.Add(newClass);
                _canvas.Children.Add(newClass);
                ClassBoxes.RegisterClassBox(newClass);
                
            }
            
        }
        
        /// <summary>
        /// Renders a list of classes. Registers the ClassBoxes in the ClassBox manager
        /// </summary>
        /// <param name="withClasses">The list of classes to be added to the rendered area</param>
        private void RenderClasses(List<Class> withClasses)
        {
            foreach (Class currentClass in withClasses)
            {
                
                ClassBox newClass = new ClassBox(currentClass, ref _activeDiagram, this, _inEditMode);
                _classBoxes.Add(newClass);
                _canvas.Children.Add(newClass);
                ClassBoxes.RegisterClassBox(newClass);
                
            }

        }

        /// <summary>
        /// Iterates through the given list of Relationships and draws each line
        /// </summary>
        /// <param name="withRelationships">The list of Relationships to be added to the rendered area</param>
        private void RenderLines(List<Relationship> withRelationships)
        {
            _relationshipLines.Clear();
            RelationshipLine.ResetGrid();
            foreach (ClassBox c in _classBoxes)
            {
                AddClassBoxToGrid(c);
            }
            
            foreach (Relationship currentRelation in withRelationships)
            {
                
                ClassBox sourceClassBox = GetClassBoxByName(currentRelation.SourceClass);
                ClassBox destClassBox = GetClassBoxByName(currentRelation.DestinationClass);

                sourceClassBox.ClearAllEdges();
                destClassBox.ClearAllEdges();
                
                RelationshipLine newLine =
                    new RelationshipLine(sourceClassBox, destClassBox, currentRelation.RelationshipType);

                _relationshipLines.Add(newLine);
            }

            PriorityQueue<RelationshipLine, float> shortestLines = new();
            foreach (RelationshipLine l in _relationshipLines)
            {
                RelationshipLine.PointPair pair = l.pointPairQueue.Peek();
                shortestLines.Enqueue(l, pair.Distance);
            }

            while (shortestLines.Count > 0)
            {
                RelationshipLine line = shortestLines.Dequeue();
                line.Draw(_canvas);
            }
        }
        
        /// <summary>
        /// Removes all drawn lines from the canvas
        /// </summary>
        public void ClearAllLines()
        {
            List<IControl> children = new List<IControl>(_canvas.Children); 
            foreach (var control in children)
            {
                if (control.GetType() == typeof(Line) || control.GetType() == typeof(Polyline))
                {
                    _canvas.Children.Remove(control);
                }
            }
        }

        /// <summary>
        /// Removes the provided ClassBox from the rendered area
        /// </summary>
        /// <param name="toUnrender">The class to remove from the rendered area</param>
        public void UnrenderClass(ClassBox toUnrender)
        {
            
            _canvas.Children.Remove(toUnrender);
            ClassBoxes.UnregisterClassBox(toUnrender);
            ReconsiderCanvasSize();
            
        }
        
        /// <summary>
        /// Wipes everything off of the canvas. Unregisters all previously rendered ClassBoxes
        /// </summary>
        private void ClearCanvas()
        {
            
            _canvas.Children.Clear();
            ClassBoxes.UnregisterAll();
            RelationshipLine.ResetGrid();
        }

        /// <summary>
        /// Redraws all lines for the relationships
        /// </summary>
        public void RedrawLines()
        {
            
            Dispatcher.UIThread.Post(() =>
            {
                
                ClearAllLines();
                RenderLines(_activeDiagram.Relationships);

            });
        }

        /// <summary>
        /// Adjusts canvas size to accomodate all children
        /// </summary>
        public void ReconsiderCanvasSize()
        {

            // Make sure all classes are sized
            Dispatcher.UIThread.RunJobs();

            // Added to calculations to make canvas slightly larger
            double bias = 20.0;
            
            // Iterate over all classboxes, find the most distant ones
            double largestX = 0;
            double largestY = 0;

            foreach (ClassBox currentClass in DrawnClassBoxes)
            {

                double candidateX = currentClass.Bounds.X + currentClass.Bounds.Width + bias;
                double candidateY = currentClass.Bounds.Y + currentClass.Bounds.Height + bias;

                if (candidateX > largestX)
                {
                    largestX = candidateX;
                }

                if (candidateY > largestY)
                {
                    largestY = candidateY;
                }
            }

            // Resize the canvas to fit all children
            _canvas.Width = largestX;
            _canvas.Height = largestY;

        }

        /// <summary>
        /// Event handler for view/edit toggle
        /// </summary>
        /// <param name="sender">Object that generated the event</param>
        /// <param name="e">Extra arguments sent to the handler</param>
        private void ViewEditToggle_OnClick(object sender, RoutedEventArgs e)
        {
            #pragma warning disable CS8629
            ToggleSwitch viewSwitch = (ToggleSwitch) sender;

            _inEditMode = ((bool)viewSwitch.IsChecked);


            foreach (ClassBox currentBoxes in DrawnClassBoxes)
            {
                currentBoxes.ToggleEditMode(_inEditMode);
            }
     
            ReconsiderCanvasSize();   
            RedrawLines();    
            #pragma warning restore CS8629
        }

        private void MagicLinesToggle_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleSwitch viewSwitch = (ToggleSwitch) sender;

            RelationshipLine.ToggleMagicLines(((bool)viewSwitch.IsChecked));
            
            ReconsiderCanvasSize();   
            RedrawLines();  
        }

        /// <summary>
        /// Enables or disables the undo/ redo buttons based on if the TM has a prev or next state
        /// </summary>
        private void ReconsiderUndoRedoVisibility()
        {
            
            _undoButton.IsEnabled = TimeMachine.PreviousStateExists();
            _redoButton.IsEnabled = TimeMachine.NextStateExists();
            
        }

        /// <summary>
        /// Wipes the canvas and redraws everything
        /// </summary>
        private void RedrawEverything()
        {
            
            ClearCanvas();
            RenderClasses(_activeDiagram.Classes);
                            
            Dispatcher.UIThread.RunJobs();
            
            //RenderLines(_activeDiagram.Relationships);
            RedrawLines();
            ReconsiderCanvasSize();
            
        }
        
        private void UndoButton_OnClick(object sender, RoutedEventArgs e)
        {

            _activeDiagram = TimeMachine.MoveToPreviousState();
            
            ReconsiderUndoRedoVisibility();
            RedrawEverything();

        }

        private void RedoButton_OnClick(object sender, RoutedEventArgs e)
        {

            _activeDiagram = TimeMachine.MoveToNextState();
            
            ReconsiderUndoRedoVisibility();
            RedrawEverything();

        }

        /// <summary>
        /// Exports the diagram to the provided image file
        /// </summary>
        /// <param name="imageFile">The file to export to</param>
        private void ExportToImage(string imageFile)
        {
            
            // Grab the scroll viewer
            var scrollViewer = this.FindControl<ScrollViewer>("ScrollView");

            // Avalonia is weird. We have to set the scroller over to (0,0) for the export to have everything...
            var oldOffset = scrollViewer.Offset;
            scrollViewer.Offset = new Vector(0.0, 0.0);

            // Run all UI tasks
            Dispatcher.UIThread.RunJobs();
            
            // Create the render target
            var px = new PixelSize((int) _canvas.Width, (int) _canvas.Height);
            var renderTarget = new RenderTargetBitmap(px);
            
            // Write in the black background
            DrawingContext dc = new DrawingContext(renderTarget.CreateDrawingContext(null));
            dc.FillRectangle(Theme.Current.CanvasColor, new Rect(new Point(0.0, 0.0), px.ToSize(1.0)));

            // Render the canvas to the target
            renderTarget.Render(_canvas);

            // Write the image file and restore the old scroll value
            renderTarget.Save(imageFile);
            scrollViewer.Offset = oldOffset;

        }
        
        private void ExportToImage_OnClick(object sender, RoutedEventArgs e)
        {
            
            // Only open if the diagram is not empty, display an error otherwise
            if (_activeDiagram.Classes.Count != 0)
            {

                Task<string?> exportTask = _exportDialog.ShowAsync(this);
                exportTask.ContinueWith(taskResult =>
                {
                    
                    // Called when the future is resolved
                    Dispatcher.UIThread.Post(() =>
                    {

                        /* Get the files the user selected
                         * This will be null if the user canceled the operation or closed the window */
                        string? selectedFile = taskResult.Result;

                        if (selectedFile is not null && selectedFile.Length >= 1)
                        {

                            try
                            {

                                ExportToImage(selectedFile);
                                RaiseAlert
                                (
                                
                                    "Diagram Exported",
                                    "Diagram Exported",
                                    $"Exported to {selectedFile}",
                                    AlertIcon.INFO
                                    
                                );
                                
                            }

                            catch (Exception exception)
                            {

                                RaiseAlert(
                                    "Export Failed",
                                    $"Export Failed",
                                    exception.Message,
                                    AlertIcon.ERROR
                                );

                            }
                        }

                    });

                });
                
            }

            else
            {
                
                RaiseAlert
                (
                    
                    "Nothing To Export", 
                    "Nothing To Export", 
                    "The diagram is empty", 
                    AlertIcon.ERROR
                    
                );
                
            }
            
        }

        private void EditTheme_OnClick(object? sender, RoutedEventArgs e)
        {
            
            ModalDialog themeEditorWindow = ModalDialog.CreateDialog<ThemeEditor>("Theme Editor", DialogButtons.OK_CANCEL);
            
            // Expand the editor window to be larger
            themeEditorWindow.Width = 1200;
            themeEditorWindow.Height = 600;
            
            themeEditorWindow.ShowDialog<DialogButtons>(this).ContinueWith(result =>
            {

                if (result.Result == DialogButtons.OKAY)
                {

                    Dispatcher.UIThread.Post(() =>
                    {
                        
                        var resultForm = themeEditorWindow.GetPrompt<ThemeEditor>();
                        
                        // Apply the new theme
                        Theme newTheme = resultForm.NewTheme;
                        Theme.Current = newTheme;
                        RedrawLines();
                        
                        // Try to save the theme
                        newTheme.TrySaveTheme(_themeFileLocation);

                    });
                    
                }
                
            });

        }

        private void LoadTheme_OnClick(object? sender, RoutedEventArgs e)
        {
            
            Task<string[]?> loadTask = _openThemeDialog.ShowAsync(this);
            loadTask.ContinueWith(taskResult =>
            {
                    
                // Called when the future is resolved
                Dispatcher.UIThread.Post(() =>
                {

                    /* Get the files the user selected
                     * This will be null if the user canceled the operation or closed the window */
                    string[]? selectedFiles = taskResult.Result;

                    if (selectedFiles is not null && selectedFiles.Length >= 1)
                    {

                        // Attempt to load the theme AND save it
                        if (!Theme.TryLoadTheme(selectedFiles[0]) || !Theme.Current.TrySaveTheme(_themeFileLocation))
                        {

                            RaiseAlert(
                                "Load Failed",
                                $"Load Failed",
                                "Could not load theme",
                                AlertIcon.ERROR
                            );
                                
                        }
                        
                        RedrawLines();

                    }

                });

            });
            
        }

        private void ThemeSave_OnClick(object? sender, RoutedEventArgs e)
        {
            
            Task<string?> saveTask = _saveThemeDialog.ShowAsync(this);
            saveTask.ContinueWith(taskResult =>
            {
                    
                // Called when the future is resolved
                Dispatcher.UIThread.Post(() =>
                {

                    /* Get the file the user selected
                     * This will be null if the user canceled the operation or closed the window */
                    string? selectedFile = taskResult.Result;

                    if (selectedFile is not null && selectedFile.Length >= 1)
                    {

                        try
                        {

                            Theme.Current.TrySaveTheme(selectedFile);
                            RaiseAlert
                            (
                                
                                "Theme Saved",
                                "Theme Saved",
                                $"Theme saved to {selectedFile}",
                                AlertIcon.INFO
                                    
                            );
                                
                        }

                        catch (Exception exception)
                        {

                            RaiseAlert(
                                "Theme Save Failed",
                                $"Theme Save Failed",
                                exception.Message,
                                AlertIcon.ERROR
                            );

                        }
                    }

                });

            });
            
        }
    }
    
}
