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

    private StackPanel fieldsArea;
    private StackPanel methodsArea;
    private Label _classNameLabel;
    private Grid _titleBar;
    
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
        
        /////////////////////////////////////////////////////////////////////////////////////////
        /// Mouse clicking/ dragging event handers
        
        _beingDragged = false;
        _titleBar.PointerPressed += (object sender, PointerPressedEventArgs args) =>
        {
            _beingDragged = true; // If the class title bar is clicked, switch on the dragging flag
        };

        _titleBar.PointerReleased += (object sender, PointerReleasedEventArgs args) =>
        {
            _beingDragged = false; // If the mouse button is released, switch off the dragging flag
        };

        PointerMoved += (object sender, PointerEventArgs args) =>
        {

            // If this class box is being dragged...
            if (_beingDragged)
            {

                // Get the position of the cursor relative to the canvas
                Point pointerLocation = args.GetPosition(this.Parent);
                
                // Set the ClassBox's location to the location of the cursor
                Canvas.SetLeft(this, pointerLocation.X);
                Canvas.SetTop(this, pointerLocation.Y);

                // TODO: At some point, use smarter math so the movement isn't snapped to the upper left corner of the box
                
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
    public ClassBox(string withClassName, ref Diagram inDiagram, MainWindow parentWindow) : this()
    {
        Name = withClassName;
        ClassName = withClassName;
        _activeDiagram = inDiagram;
        _parentWindow = parentWindow;

    }

    /// <summary>
    /// Constructs a class box with the provided properties
    /// </summary>
    /// <param name="template">A class object to base this depiction off of</param>
    /// <param name="inDiagram">A reference to the diagram this class is from</param>
    /// <param name="parentWindow">A reference to the window this class is shown in</param>
    public ClassBox(Class template, ref Diagram inDiagram, MainWindow parentWindow) : this(template.ClassName, ref inDiagram, parentWindow)
    {
        
        // Render methods and fields
        AddFields(template.Fields);
        AddMethods(template.Methods);
        
    }

    /// <summary>
    /// Adds a list of fields to this class
    /// </summary>
    /// <param name="fields">The fields to be added</param>
    private void AddFields(List<NameTypeObject> fields)
    {
        
        foreach (NameTypeObject currentField in fields)
        {
            fieldsArea.Children.Add(new FieldContainer(currentField, this));
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
            methodsArea.Children.Add(new MethodContainer(currentMtd, this));
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
            $"Delete class {ClassName}?",
            $"Delete class {ClassName}?",
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

        ModalDialog dialog = ModalDialog.CreateDialog<AddClassPanel>($"Rename {ClassName}", DialogButtons.OK_CANCEL);
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
        
        ModalDialog dialog = ModalDialog.CreateDialog<AddFieldPanel>($"Add Field To {ClassName}", DialogButtons.OK_CANCEL);
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
                        fieldsArea.Children.Add(new FieldContainer(newField, this));

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

                            _activeDiagram.RemoveField(ClassName, toDelete.FieldName);
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
            $"Delete parameter {toDelete.FieldName} from {inMethod.MethodName}?",
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

                            _activeDiagram.RemoveParameter(toDelete.NTO_Representation, inMethod.MethodName, ClassName);
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

                        _activeDiagram.RestructureField(ClassName, oldField, newField);
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

                        _activeDiagram.RestructureParameter(ClassName, onParentMethod.MethodName, oldParam, newParam);
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
    /// <param name="onMethod">The method to add a paramter to</param>
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
                        methodsArea.Children.Add(new MethodContainer(form.FieldType, form.FieldName, this));

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
    
}