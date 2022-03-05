using DynamicData.Binding;

namespace UMLEditor.Views;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using UMLEditor.Classes;
using System.Runtime.InteropServices;

/// <summary>
/// A control that is a graphical depiction of a class in the diagram
/// </summary>
public class ClassBox : UserControl
{

    private string _className;
    public string ClassName
    {
        get => _className;
        private set
        {

            _className = value;
            _classNameLabel.Content = $"_{_className}";

        }
    }

    private bool _inEditMode;
    
    private StackPanel fieldsArea;
    private StackPanel methodsArea;
    private Label _classNameLabel;
    private Grid _titleBar;
    private Point _clickedLocation;
    
    private Button _editButton;
    private Button _deleteButton;
    private Button _addFieldButton;
    private Button _addMethodButton;
    
    private bool _beingDragged; // True if the user is dragging this class
    private Diagram _activeDiagram; // The active diagram this class is a part of
    private MainWindow _parentWindow; // The parent window this class is depicted on (for raising alerts/ confirmations)
    
    public ClassBox()
    {
        
        InitializeComponent();

        // Grab controls
        fieldsArea = this.FindControl<StackPanel>("FieldsArea");
        methodsArea = this.FindControl<StackPanel>("MethodsArea");
        _classNameLabel = this.FindControl<Label>("ClassNameLabel");
        _titleBar = this.FindControl<Grid>("TitleBar");
        
        _editButton = this.FindControl<Button>("EditButton");
        _deleteButton = this.FindControl<Button>("DeleteButton");
        _addFieldButton = this.FindControl<Button>("AddFieldButton");
        _addMethodButton = this.FindControl<Button>("AddMethodButton");

        _inEditMode = false;
        
        /////////////////////////////////////////////////////////////////////////////////////////
        /// Mouse clicking/ dragging event handers
        
        _beingDragged = false;
        _titleBar.PointerPressed += (object sender, PointerPressedEventArgs args) =>
        {
            
            // If the class title bar is clicked, switch on the dragging flag
            _beingDragged = true;
            
            // Record where the user clicked the title bar
            _clickedLocation = args.GetPosition(_titleBar);

        };

        _titleBar.PointerReleased += (object sender, PointerReleasedEventArgs args) =>
        {
            
            // If the mouse button is released, switch off the dragging flag
            _beingDragged = false; 
            
            // Redraw the lines again (so "flinging" the box doesn't mess up the lines)
            _parentWindow!.RedrawLines();
            
            // Save the new location of the class box
            _activeDiagram!.ChangeBoxLocation(ClassName, this.Bounds.X, this.Bounds.Y);
            _parentWindow!.ReconsiderCanvasSize();
            
        };

        _titleBar.PointerEnter += (object sender, PointerEventArgs pe) =>
        {

            StandardCursorType desiredCursor = StandardCursorType.SizeAll;
            
            // Change the mouse pointer to the four arrows when hovering over the title bar
            // Linux has to be special...
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                desiredCursor = StandardCursorType.DragLink;

            }
            
            ChangeMousePointer(_titleBar, pe, desiredCursor);            

        };

        // Revert to normal pointer when hovering over buttons
        _editButton.PointerEnter += (object sender, PointerEventArgs pe) =>
        {
            ChangeMousePointer(_editButton, pe, StandardCursorType.Arrow);
        };

        _deleteButton.PointerEnter += (object sender, PointerEventArgs pe) =>
        {
            ChangeMousePointer(_deleteButton, pe, StandardCursorType.Arrow);
        };
        
        PointerMoved += (object sender, PointerEventArgs args) =>
        {

            // If this class box is being dragged...
            if (_beingDragged)
            {

                // Get the position of the cursor relative to the canvas
                Point pointerLocation = args.GetPosition(this.Parent);
                double newX = Math.Max(0, (pointerLocation.X - _clickedLocation.X));
                double newY = Math.Max(0, (pointerLocation.Y - _clickedLocation.Y));
                
                // Set the ClassBox's location to the location of the cursor
                Canvas.SetLeft(this, newX);
                Canvas.SetTop(this, newY);

                _parentWindow!.RedrawLines();
                _parentWindow!.ReconsiderCanvasSize();
                
            }
            
        };
        
        /////////////////////////////////////////////////////////////////////////////////////////
        
    }

    /// <summary>
    /// Constructs a class box with the provided properties
    /// </summary>
    /// <param name="withClassName">The name of this class (to be displayed)</param>
    /// <param name="inDiagram">A reference to the diagram this class is from</param>
    /// <param name="parentWindow">A reference to the window this class is shown in</param>
    /// <param name="inEditMode">Sets whether or not this should default to being in edit mode</param>
    public ClassBox(string withClassName, ref Diagram inDiagram, MainWindow parentWindow, bool inEditMode = false) : this()
    {
        Name = withClassName;
        ClassName = withClassName;
        _activeDiagram = inDiagram;
        _parentWindow = parentWindow;

        ToggleEditMode(inEditMode);
        
    }

    /// <summary>
    /// Constructs a class box with the provided properties
    /// </summary>
    /// <param name="template">A class object to base this depiction off of</param>
    /// <param name="inDiagram">A reference to the diagram this class is from</param>
    /// <param name="parentWindow">A reference to the window this class is shown in</param>
    /// <param name="inEditMode">Sets whether or not this should default to being in edit mode</param>
    public ClassBox(Class template, ref Diagram inDiagram, MainWindow parentWindow, bool inEditMode = false) : this(template.ClassName, ref inDiagram, parentWindow, inEditMode)
    {
        
        // Render methods and fields
        AddFields(template.Fields);
        AddMethods(template.Methods);
        
        // Recall the last position
        Canvas.SetLeft(this, template.ClassLocation.X);
        Canvas.SetTop(this, template.ClassLocation.Y);
        
    }

    /// <summary>
    /// Adds a list of fields to this class
    /// </summary>
    /// <param name="fields">The fields to be added</param>
    private void AddFields(List<NameTypeObject> fields)
    {
        
        foreach (NameTypeObject currentField in fields)
        {
            fieldsArea.Children.Add(new FieldContainer(currentField, this, isInEditMode:_inEditMode));
        }
        
    }

    /// <summary>
    /// Adds a list of methods to this class
    /// </summary>
    /// <param name="methods">The methods to be added</param>
    private void AddMethods(List<Method> methods)
    {

        foreach (Method currentMtd in methods)
        {
            methodsArea.Children.Add(new MethodContainer(currentMtd, this, inEditMode:_inEditMode));
        }
        
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Event handler for the class deletion button
    /// </summary>
    private void DeleteClass_Button_OnClick(object sender, RoutedEventArgs e)
    {
        
        _parentWindow.RaiseConfirmation(
            $"Delete class '{ClassName}'?",
            $"Delete class '{ClassName}'?",
            $"Are you sure you want to delete the class '{ClassName}'?",
            AlertIcon.QUESTION,
            (selection) =>
            {

                if (selection.Result == DialogButtons.YES)
                {

                    Dispatcher.UIThread.Post(() =>
                    {

                        try
                        {

                            _activeDiagram.DeleteClass(ClassName);
                            _parentWindow.UnrenderClass(this);

                        }

                        catch (Exception e)
                        {

                            _parentWindow.RaiseAlert(
                                "Class Deletion Failed",
                                $"Could not delete class '{ClassName}'",
                                e.Message,
                                AlertIcon.ERROR
                            );

                        }

                    });
                    
                }
            }
        );
        
    }

    /// <summary>
    /// Event handler for the rename class button
    /// </summary>
    private void RenameClass_Button_OnClick(object sender, RoutedEventArgs e)
    {

        ModalDialog dialog = ModalDialog.CreateDialog<AddClassPanel>($"Rename '{ClassName}'", DialogButtons.OK_CANCEL);
        dialog.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {

                Dispatcher.UIThread.Post(() =>
                {
                    
                    try
                    {

                        AddClassPanel form = dialog.GetPrompt<AddClassPanel>();
                        _activeDiagram.RenameClass(ClassName, form.ClassName);
                        ClassName = form.ClassName;

                    }

                    catch (Exception e)
                    {
                            
                        _parentWindow.RaiseAlert(
                            "Class Renaming Failed",
                            $"Could not rename class '{ClassName}'",
                            e.Message,
                            AlertIcon.ERROR
                        );
                            
                    }
                    
                });
            }
            
        });

    }

    /// <summary>
    /// Prompts to add a field in this class
    /// </summary>
    private void AddFieldButton_OnClick(object sender, RoutedEventArgs e)
    {
        
        ModalDialog dialog = ModalDialog.CreateDialog<AddFieldPanel>($"Add Field To '{ClassName}'", DialogButtons.OK_CANCEL);
        dialog.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {

                Dispatcher.UIThread.Post(() =>
                {
                    
                    try
                    {

                        AddFieldPanel form = dialog.GetPrompt<AddFieldPanel>();
                        NameTypeObject newField = new NameTypeObject(form.FieldType, form.FieldName);
                            
                        _activeDiagram.AddField(ClassName, newField.Type, newField.AttributeName);
                        fieldsArea.Children.Add(new FieldContainer(newField, this, isInEditMode:_inEditMode));

                    }

                    catch (Exception e)
                    {
                            
                        _parentWindow.RaiseAlert(
                            "Field Add Failed",
                            $"Could not add field to '{ClassName}'",
                            e.Message,
                            AlertIcon.ERROR
                        );
                            
                    }
                    
                });

            }
            
        });
        
    }

    /// <summary>
    /// Prompts to remove the provided field from this class
    /// </summary>
    /// <param name="toDelete">The FieldContainer containing the field to be deleted</param>
    public void RequestFieldDeletion(FieldContainer toDelete)
    {

        _parentWindow.RaiseConfirmation 
        (
        
            "Confirm Deletion",
            $"Delete field '{toDelete.FieldName}' from '{ClassName}'?",
            "Are you sure you want to delete this?",
            AlertIcon.QUESTION,
            (selection) =>
            {

                if (selection.Result == DialogButtons.YES)
                {

                    Dispatcher.UIThread.Post(() =>
                    {
                        
                        try
                        {

                            _activeDiagram.DeleteField(ClassName, toDelete.FieldName);
                            fieldsArea.Children.Remove(toDelete);

                        }

                        catch (Exception e)
                        {
                    
                            _parentWindow.RaiseAlert(
                                "Deletion Failed",
                                "Deletion Failed",
                                e.Message,
                                AlertIcon.ERROR
                            );
                        
                        }
                        
                    });
                
                }
            
            }
        
        );
        
    }

    /// <summary>
    /// Prompts to remove the provided parameter in the provided method
    /// </summary>
    /// <param name="toDelete">The parameter to delete</param>
    /// <param name="inMethod">The parent method the deleted parameter is in</param>
    public void RequestParameterDeletion(ParameterContainer toDelete, MethodContainer inMethod)
    {

        _parentWindow.RaiseConfirmation
        (

            "Confirm Deletion",
            $"Delete parameter '{toDelete.FieldName}' from '{inMethod.MethodName}'?",
            "Are you sure you want to delete this?",
            AlertIcon.QUESTION,
            (selection) =>
            {

                if (selection.Result == DialogButtons.YES)
                {

                    Dispatcher.UIThread.Post(() =>
                    {
                        
                        try
                        {

                            _activeDiagram.DeleteParameter(toDelete.NTO_Representation.AttributeName, inMethod.MethodName, ClassName);
                            inMethod.WipeParameter(toDelete);
                        
                        }

                        catch (Exception e)
                        {
                    
                            _parentWindow.RaiseAlert(
                                "Deletion Failed",
                                "Deletion Failed",
                                e.Message,
                                AlertIcon.ERROR
                            );
                        
                        }
                        
                    });
                    
                }
                
            }

        );

    }

    /// <summary>
    /// Prompts the user to change the name of a field
    /// </summary>
    /// <param name="onContainer">The FieldContainer the click event was raised from</param>
    public void RequestFieldRename(FieldContainer onContainer)
    {

        ModalDialog editBox = ModalDialog.CreateDialog<AddFieldPanel>($"Rename Field '{onContainer.FieldName}'", DialogButtons.OK_CANCEL);
        AddFieldPanel form = editBox.GetPrompt<AddFieldPanel>();
        form.FieldName = onContainer.FieldName;
        form.FieldType = onContainer.Type;
        
        editBox.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {
                
                Dispatcher.UIThread.Post(() =>
                {

                    try
                    {
                                            
                        NameTypeObject oldField = new NameTypeObject(onContainer.Type, onContainer.FieldName);
                        NameTypeObject newField = new NameTypeObject(form.FieldType, form.FieldName);

                        _activeDiagram.ReplaceField(ClassName, oldField, newField);
                        onContainer.ChangeAnatomy(newField);

                    }

                    catch (Exception e)
                    {
                        
                        _parentWindow.RaiseAlert(
                            "Field Rename Failed",
                            "Field Rename Failed",
                            e.Message,
                            AlertIcon.ERROR
                        );
                        
                    }

                });
                
            }

        });

    }

    /// <summary>
    /// Prompts the user to modify a parameter on a method
    /// </summary>
    /// <param name="onContainer">The FieldContainer the event was raised from</param>
    /// <param name="onParentMethod">The method this parameter is a member of</param>
    public void RequestParamRename(FieldContainer onContainer, MethodContainer onParentMethod)
    {

        ModalDialog editBox = ModalDialog.CreateDialog<AddFieldPanel>($"Rename Parameter '{onContainer.FieldName}'", DialogButtons.OK_CANCEL);
        AddFieldPanel form = editBox.GetPrompt<AddFieldPanel>();
        form.FieldName = onContainer.FieldName;
        form.FieldType = onContainer.Type;
        
        editBox.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {
                
                Dispatcher.UIThread.Post(() =>
                {

                    try
                    {
                                            
                        NameTypeObject oldParam = new NameTypeObject(onContainer.Type, onContainer.FieldName);
                        NameTypeObject newParam = new NameTypeObject(form.FieldType, form.FieldName);

                        _activeDiagram.ReplaceParameter(ClassName, onParentMethod.MethodName, oldParam.AttributeName, newParam);
                        onContainer.ChangeAnatomy(newParam);

                    }

                    catch (Exception e)
                    {
                        
                        _parentWindow.RaiseAlert(
                            "Parameter Rename Failed",
                            "Parameter Rename Failed",
                            e.Message,
                            AlertIcon.ERROR
                        );
                        
                    }

                });
                
            }

        });

    }

    /// <summary>
    /// Prompts the user to delete a specific method
    /// </summary>
    /// <param name="toDelete">The method container the event came from</param>
    public void RequestMethodDelete(MethodContainer toDelete)
    {
        
        _parentWindow.RaiseConfirmation
        (
            
            $"Delete Method '{toDelete.ReturnType} {toDelete.MethodName}'",
            $"Delete Method '{toDelete.ReturnType} {toDelete.MethodName}'?",
            $"Are you sure you want to delete '{toDelete.ReturnType} {toDelete.MethodName}?'",
            AlertIcon.QUESTION,
            (selection) =>
            {

                if (selection.Result == DialogButtons.YES)
                {

                    Dispatcher.UIThread.Post(() =>
                    {
                        
                        try
                        {
    
                            _activeDiagram.DeleteMethod(ClassName, toDelete.MethodName);
                            methodsArea.Children.Remove(toDelete);
    
                        }
                        
                        catch (Exception e)
                        {
                            
                            _parentWindow.RaiseAlert(
                                "Method Deletion Failed",
                                "Method Deletion Failed",
                                e.Message,
                                AlertIcon.ERROR
                            );
                            
                        }
                        
                    });
                    
                }
                
            }

        );
        
    }

    /// <summary>
    /// Prompts the user to change the name and type of the provided method
    /// </summary>
    /// <param name="onMethod">The MethodContainer the event came from</param>
    public void RequestMtdNameTypeChange(MethodContainer onMethod)
    {

        ModalDialog prompt = ModalDialog.CreateDialog<AddFieldPanel>($"Edit '{onMethod.ReturnType} {onMethod.MethodName}'", DialogButtons.OK_CANCEL);
        AddFieldPanel form = prompt.GetPrompt<AddFieldPanel>();
        form.FieldName = onMethod.MethodName;
        form.FieldType = onMethod.ReturnType;
        
        prompt.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {
                
                Dispatcher.UIThread.Post(() =>
                {
            
                    try
                    {

                        NameTypeObject newAnatomy = new NameTypeObject(form.FieldType, form.FieldName);
                        _activeDiagram.ChangeMethodNameType
                        (
                            ClassName, 
                            onMethod.MethodName, 
                            newAnatomy
                        );

                        onMethod.ChangeNameType(newAnatomy);

                    }
                    
                    catch (Exception e)
                    {
                        
                        _parentWindow.RaiseAlert(
                            "Method Change Failed",
                            "Method Change Failed",
                            e.Message,
                            AlertIcon.ERROR
                        );

                    }

                });
                
            }
            
        });

    }

    /// <summary>
    /// Prompts the user to add a parameter to the provided method
    /// </summary>
    /// <param name="onMethod">The method to add a parameter to</param>
    public void RequestParameterAdd(MethodContainer onMethod)
    {

        ModalDialog prompt = ModalDialog.CreateDialog<AddFieldPanel>($"Add Parameter To Method '{onMethod.MethodName}'", DialogButtons.OK_CANCEL);
        AddFieldPanel form = prompt.GetPrompt<AddFieldPanel>();
        
        prompt.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {
                
                Dispatcher.UIThread.Post(() =>
                {
            
                    try
                    {

                        NameTypeObject newParam = new NameTypeObject(form.FieldType, form.FieldName);
                        _activeDiagram.AddParameter
                        (
                            ClassName, 
                            onMethod.MethodName, 
                            newParam
                        );

                        onMethod.AddParams(newParam);

                    }
                    
                    catch (Exception e)
                    {
                        
                        _parentWindow.RaiseAlert(
                            "Parameter Add Failed",
                            "Parameter Add Failed",
                            e.Message,
                            AlertIcon.ERROR
                        );

                    }

                });
                
            }
            
        });

    }
    
    private void AddMethodButton_OnClick(object sender, RoutedEventArgs e)
    {

        ModalDialog prompt = ModalDialog.CreateDialog<AddFieldPanel>("Create New Method", DialogButtons.OK_CANCEL);
        prompt.ShowDialog<DialogButtons>(_parentWindow).ContinueWith((selection) =>
        {

            if (selection.Result == DialogButtons.OKAY)
            {
                
                Dispatcher.UIThread.Post(() =>
                {

                    try
                    {

                        AddFieldPanel form = prompt.GetPrompt<AddFieldPanel>();
                        _activeDiagram.AddMethod(ClassName, form.FieldType, form.FieldName);
                        methodsArea.Children.Add(new MethodContainer(form.FieldType, form.FieldName, this, inEditMode:_inEditMode));

                    }
                    
                    catch (Exception e)
                    {
                        
                        _parentWindow.RaiseAlert(
                            "Method Add Failed",
                            "Method Add Failed",
                            e.Message,
                            AlertIcon.ERROR
                        );
                        
                    }

                });
                
            }
            
        });

    }

    /// <summary>
    /// Changes the mouse pointer
    /// </summary>
    /// <param name="inputElement">The element to focus on to change the pointer</param>
    /// <param name="args">Arguments from the pointer event</param>
    /// <param name="newCursor">The new cursor to show</param>
    private void ChangeMousePointer(InputElement? inputElement, PointerEventArgs args, StandardCursorType newCursor)
    {

        args.Pointer.Capture(inputElement); // Capture the UI element (so you can change the cursor)
        inputElement!.Cursor = new Cursor(newCursor); // Change the cursor for the element
        args.Pointer.Capture(null); // Un-capture the cursor (so it can do other things)

    }

    /// <summary>
    /// Flips this ClassBox in and out of edit mode
    /// </summary>
    /// <param name="inEditMode">Sets if the box is in edit mode or not</param>
    public void ToggleEditMode(bool inEditMode)
    {

        _inEditMode = inEditMode;
        
        _editButton.IsVisible = _inEditMode;
        _deleteButton.IsVisible = _inEditMode;
        _addFieldButton.IsVisible = _inEditMode;
        _addMethodButton.IsVisible = _inEditMode;

        foreach (var currentItem in fieldsArea.Children)
        {
            
            ((FieldContainer)currentItem).ToggleEditMode(_inEditMode);
            
        }
        
        foreach (var currentItem in methodsArea.Children)
        {
            
            ((MethodContainer)currentItem).ToggleEditMode(_inEditMode);
            
        }
        
    }
    
}