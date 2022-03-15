using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.Media;
using UMLEditor.Classes;
using UMLEditor.Interfaces;
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

        private List<RelationshipLine> _relationshipLines = new List<RelationshipLine>();

        private IDiagramFile _activeFile;

        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        
        // Line specifications
        private const double SymbolWidth = 15;
        private const double SymbolHeight = 10;
        private const int LineThickness = 2;
        private readonly IBrush _brush = Brushes.CornflowerBlue;

        private bool _inEditMode; 
        
        // Data structure for the display of a Relationship line
        struct RelationshipLine
        {
            public UserControl SourceClass;
            public UserControl DestClass;
            // ReSharper disable once NotAccessedField.Local
            public string RelationshipType;
            public Line StartLine;
            public Line MidLine;
            public Line EndLine;
            public Polyline Symbol;
        }
        
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

            _canvas = this.FindControl<Canvas>("MyCanvas");
            _inEditMode = false;

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
                            ClearCanvas();
                            RenderClasses(_activeDiagram.Classes);
                            Dispatcher.UIThread.RunJobs();
                            RenderLines(_activeDiagram.Relationships);
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

        private ClassBox GetClassBoxByName(string fromName)
        {
            ClassBox foundClass = new ClassBox();
            foreach (var classBox in _classBoxes)
            {
                if (classBox.ClassName == fromName)
                {
                    foundClass = classBox;
                }
            }

            return foundClass;
        }

        private RelationshipLine GetRelationshipByClassNames(string sourceName, string destinationName)
        {
            RelationshipLine foundLine = new RelationshipLine();
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
                        DrawRelationship(sourceClassBox, destClassBox, relationshipType);
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
                        RelationshipLine currentLine = GetRelationshipByClassNames(sourceName, destinationName);
                        ClassBox sourceClassBox = GetClassBoxByName(sourceName);
                        ClassBox destClassBox = GetClassBoxByName(destinationName);
                        ClearAllLines();
                        _relationshipLines.Remove(currentLine);
                        RenderLines(_activeDiagram.Relationships);
                        DrawRelationship(sourceClassBox, destClassBox, relationshipType);
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
                        RelationshipLine currentLine = GetRelationshipByClassNames(sourceName, destinationName);
                        ClearAllLines();
                        _relationshipLines.Remove(currentLine);
                        RenderLines(_activeDiagram.Relationships);
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
        /// Renders a list of classes, by provided name
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
                
            }
            
        }
        
        /// <summary>
        /// Renders a list of classes
        /// </summary>
        /// <param name="withClasses">The list of classes to be added to the rendered area</param>
        private void RenderClasses(List<Class> withClasses)
        {
            
            foreach (Class currentClass in withClasses)
            {
                
                ClassBox newClass = new ClassBox(currentClass, ref _activeDiagram, this, _inEditMode);
                _classBoxes.Add(newClass);
                _canvas.Children.Add(newClass);
                
            }

        }

        /// <summary>
        /// Iterates through the given list of Relationships and draws each line
        /// </summary>
        /// <param name="withRelationships">The list of Relationships to be added to the rendered area</param>
        private void RenderLines(List<Relationship> withRelationships)
        {
            foreach (Relationship currentRelation in withRelationships)
            {
                ClassBox sourceClassBox = GetClassBoxByName(currentRelation.SourceClass);
                ClassBox destClassBox = GetClassBoxByName(currentRelation.DestinationClass);

                DrawRelationship(sourceClassBox, destClassBox, currentRelation.RelationshipType);
            }
        }
        
        /// <summary>
        /// Draws the relationship arrow between two classes
        /// </summary>
        /// <param name="startCtrl">The source class to start drawing from</param>
        /// <param name="endCtrl">The destination class to draw to</param>
        /// <param name="relationshipType">The type of relationship to draw</param>
        private void DrawRelationship(UserControl startCtrl, UserControl endCtrl, string relationshipType)
        {
            RelationshipLine newLine = new RelationshipLine();
            newLine.SourceClass = startCtrl;
            newLine.DestClass = endCtrl;
            newLine.RelationshipType = relationshipType;
            
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
                    diamondPoints = new List<Point>
                    {
                        end,
                        new(end.X + SymbolHeight, end.Y + SymbolWidth),
                        new(end.X, end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y + SymbolWidth),
                        end
                    };
                    trianglePoints = new List<Point>
                    {
                        new(end.X + SymbolHeight, end.Y),
                        new(end.X, end.Y + (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y),
                        new(end.X + SymbolHeight, end.Y)
                    };
                }
                else
                {
                    // Goes bottom to top
                    start = new Point(start.X, start.Y - startHalfHeight);
                    end = new Point(end.X, end.Y + endHalfHeight + (2 * SymbolWidth));
                    diamondPoints = new List<Point>
                    {
                        end,
                        new(end.X + SymbolHeight, end.Y - SymbolWidth),
                        new(end.X, end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y - SymbolWidth),
                        end
                    };
                    trianglePoints = new List<Point>
                    {
                        new(end.X + SymbolHeight, end.Y),
                        new(end.X, end.Y - (2 * SymbolWidth)),
                        new(end.X - SymbolHeight, end.Y),
                        new(end.X + SymbolHeight, end.Y)
                    };
                }
            }

            newLine.StartLine = CreateRelationshipLine(start,midStart);
            newLine.MidLine = CreateRelationshipLine(midStart, midEnd);
            newLine.EndLine = CreateRelationshipLine(midEnd, end);
            
            // Add lines to the canvas
            _canvas.Children.Add(newLine.StartLine);
            _canvas.Children.Add(newLine.MidLine);
            _canvas.Children.Add(newLine.EndLine);

            // Draw the relationship symbol based on provided type
            switch (relationshipType)
            {
                case "aggregation":
                    _canvas.Children.Add(CreateRelationshipSymbol(diamondPoints));
                    break;
                case "composition":
                    Polyline polyline = CreateRelationshipSymbol(diamondPoints);
                    polyline.Fill = _brush;
                    _canvas.Children.Add(polyline);
                    break;
                case "inheritance":
                    _canvas.Children.Add(CreateRelationshipSymbol(trianglePoints));
                    break;
                case "realization":
                    newLine.StartLine.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    newLine.MidLine.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    newLine.EndLine.StrokeDashArray = new AvaloniaList<double>(5, 3);
                    newLine.Symbol = CreateRelationshipSymbol(trianglePoints);
                    _canvas.Children.Add(newLine.Symbol);
                    break;
            }
            _relationshipLines.Add(newLine);
        }

        /// <summary>
        /// Draws the relationship symbol from the given list of points
        /// </summary>
        /// <param name="points">A list of points for the vertices to draw</param>
        /// <returns>The new relationship symbol</returns>
        private Polyline CreateRelationshipSymbol(List<Point> points)
        {
            Polyline polyline = new Polyline();
            polyline.Name = "Polyline";
            polyline.Points = points;
            polyline.Stroke = _brush;
            polyline.StrokeThickness = LineThickness;
            return polyline;
        }

        /// <summary>
        /// Creates a line from the given start to end points
        /// </summary>
        /// <param name="lineStart">Point to start at</param>
        /// <param name="lineEnd">Point to end at</param>
        /// <returns>The new line</returns>
        private Line CreateRelationshipLine(Point lineStart, Point lineEnd)
        {
            Line l = new Line();
            l.Name = "Line";
            l.StartPoint = lineStart;
            l.EndPoint = lineEnd;
            l.Stroke = _brush;
            l.StrokeThickness = LineThickness;
            l.ZIndex = 10;
            return l;
        }
        
        /// <summary>
        /// Removes all drawn lines from the canvas
        /// </summary>
        private void ClearAllLines()
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
            ReconsiderCanvasSize();
        }
        
        /// <summary>
        /// Wipes everything off of the canvas
        /// </summary>
        private void ClearCanvas()
        {
            _canvas.Children.Clear();
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
            
            RedrawLines();
            #pragma warning restore CS8629
        }
    }
    
}
