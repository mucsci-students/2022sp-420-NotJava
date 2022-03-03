using Avalonia.Interactivity;

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

    }

    /// <summary>
    /// Constructs a MethodContainer with the provided parameters
    /// </summary>
    /// <param name="returnType">The type the method will return</param>
    /// <param name="name">The name of the method</param>
    /// <param name="parentWindow">The classbox this method is shown within</param>
    public MethodContainer(string returnType, string name, ClassBox parentWindow) : this()
    {

        ReturnType = returnType;
        MethodName = name;
        RefreshLabel();
        
        _parentWindow = parentWindow;
        
    }

    /// <summary>
    /// Updates the text on the label to reflect the current return type and name
    /// </summary>
    private void RefreshLabel()
    {
        _methodSignature.Content = $"_{ReturnType} {MethodName}()";
    }
    
    /// <summary>
    /// Constructs a MethodContainer with the provided parameters
    /// </summary>
    /// <param name="template">A method to use as a template for rendering</param>
    /// <param name="parentWindow">The classbox this method is shown within</param>
    public MethodContainer(Method template, ClassBox parentWindow) : this(template.ReturnType, template.AttributeName, parentWindow)
    {
        AddParams(template.Parameters);
    }

    /// <summary>
    /// Adds a list of parameters to this method container
    /// </summary>
    /// <param name="toAdd">The list of parameters to add</param>
    private void AddParams(List<NameTypeObject> toAdd)
    {

        foreach (NameTypeObject currentParam in toAdd)
        {
            _paramsArea.Children.Add(new ParameterContainer(currentParam, this, _parentWindow));
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
    
}