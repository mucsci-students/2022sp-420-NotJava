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
    private MethodContainer(string returnType, string name, ClassBox parentWindow) : this()
    {

        ReturnType = returnType;
        MethodName = name;
        
        _methodSignature.Content = $"{returnType} {name}()";
        _parentWindow = parentWindow;
        
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
    /// <param name="toAdd">The list of paramters to add</param>
    private void AddParams(List<NameTypeObject> toAdd)
    {

        foreach (NameTypeObject currentParam in toAdd)
        {
            _paramsArea.Children.Add(new ParameterContainer(currentParam, this, _parentWindow));
        }
        
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Deletes a provided paramter from this container
    /// </summary>
    /// <param name="toWipe">The paramter to wipe</param>
    public void WipeParameter(ParameterContainer toWipe)
    {
        _paramsArea.Children.Remove(toWipe);
    }
    
}