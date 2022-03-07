using Avalonia.Interactivity;
using Avalonia.Threading;

namespace UMLEditor.Views;

using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UMLEditor.Classes;

/// <summary>
/// A control that is a graphical depiction of a method
/// </summary>
public class MethodContainer : UserControl
{

    private Label _methodSignature;
    private StackPanel _paramsArea;
    private ClassBox _parentWindow;

    private bool _isInEditMode;
    
    public string ReturnType { get; private set; }
    public string MethodName { get; private set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public MethodContainer()
    {
        
        InitializeComponent();

        _methodSignature = this.FindControl<Label>("MethodSignature");
        _paramsArea = this.FindControl<StackPanel>("ParamsArea");
        _isInEditMode = false;

    }

    /// <summary>
    /// Constructs a MethodContainer with the provided parameters
    /// </summary>
    /// <param name="returnType">The type the method will return</param>
    /// <param name="name">The name of the method</param>
    /// <param name="parentWindow">The classbox this method is shown within</param>
    /// <param name="inEditMode">Indicates if this control is in edit mode</param>
    public MethodContainer(string returnType, string name, ClassBox parentWindow, bool inEditMode = false) : this()
    {

        ReturnType = returnType;
        MethodName = name;
        RefreshLabel();

        _parentWindow = parentWindow;
        
        ToggleEditMode(inEditMode);
        
    }

    /// <summary>
    /// Constructs a MethodContainer with the provided parameters
    /// </summary>
    /// <param name="template">A method to use as a template for rendering</param>
    /// <param name="parentWindow">The classbox this method is shown within</param>
    /// <param name="inEditMode">Indicates if this control is in edit mode</param>
    public MethodContainer(Method template, ClassBox parentWindow, bool inEditMode) 
        : this(template.ReturnType, template.AttributeName, parentWindow)
    {
        
        AddParams(template.Parameters);
        ToggleEditMode(inEditMode);

    }

    /// <summary>
    /// Updates the text on the label to reflect the current return type and name
    /// </summary>
    private void RefreshLabel()
    {
        _methodSignature.Content = $"_{ReturnType} {MethodName}()";
    }

    /// <summary>
    /// Adds a list of parameters to this method container
    /// </summary>
    /// <param name="toAdd">The list of parameters to add</param>
    private void AddParams(List<NameTypeObject> toAdd)
    {

        foreach (NameTypeObject currentParam in toAdd)
        {
            _paramsArea.Children.Add(new ParameterContainer(currentParam, this, _parentWindow, _isInEditMode));
        }
        
    }

    /// <summary>
    /// Adds an array of parameters to this method container
    /// </summary>
    /// <param name="toAdd">The array of parameters to add</param>
    public void AddParams(params NameTypeObject[] toAdd)
    {
        AddParams(new List<NameTypeObject>(toAdd));
    }
    
    /// <summary>
    ///  Changes the name and return type of this method 
    /// </summary>
    /// <param name="newAnatomy">The new name and return type to show</param>
    public void ChangeNameType(NameTypeObject newAnatomy)
    {

        MethodName = newAnatomy.AttributeName;
        ReturnType = newAnatomy.Type;
        RefreshLabel();

    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Deletes a provided parameter from this container
    /// </summary>
    /// <param name="toWipe">The parameter to wipe</param>
    public void WipeParameter(ParameterContainer toWipe)
    {
        _paramsArea.Children.Remove(toWipe);
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        _parentWindow.RequestMethodDelete(this);
    }

    private void RenameMtdButton_OnClick(object sender, RoutedEventArgs e)
    {
        _parentWindow.RequestMtdNameTypeChange(this);
    }

    private void AddParamButton_OnClick(object sender, RoutedEventArgs e)
    {
        _parentWindow.RequestParameterAdd(this);
    }

    /// <summary>
    /// Sets if this control is in edit mode or not
    /// </summary>
    /// <param name="inEditMode">Indicates whether or not this control should be in edit mode</param>
    public void ToggleEditMode(bool inEditMode)
    {

        _isInEditMode = inEditMode;

        this.FindControl<Button>("EditButton").IsVisible = _isInEditMode;
        this.FindControl<Button>("DeleteButton").IsVisible = _isInEditMode;
        this.FindControl<Button>("AddButton").IsVisible = _isInEditMode;

        string paramsListing = "";
        
        // Adjust all parameters
        foreach (var control in _paramsArea.Children)
        {

            FieldContainer currentControl = (FieldContainer) control;
            currentControl.ToggleEditMode(_isInEditMode);

            paramsListing += $"{currentControl.Type} {currentControl.FieldName}, ";

        }

        if (_isInEditMode)
        {
            RefreshLabel();
        }

        else
        {
            
            // Pull the parameters up into the signature
            if (paramsListing.Length >= 2)
            {
                paramsListing = paramsListing.Substring(0, paramsListing.Length - 2);
            }

            _methodSignature.Content = $"{ReturnType} {MethodName}({paramsListing})";
            
        }

    }
    
}