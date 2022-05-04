using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views.CustomControls.Modal.InputForms;

/// <summary>
/// AddMethodPanel.cs
/// </summary>
public class AddMethodPanel : UserControl
{
    
    /// <summary>
    /// Getter for ClassName
    /// </summary>
    public string ClassName
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
    }
    
    /// <summary>
    /// Getter for MethodName
    /// </summary>
    public string MethodName
    {
        get => this.FindControl<TextBox>("MethodName").Text;
    }
    
    /// <summary>
    /// Getter for ReturnType
    /// </summary>
    public string ReturnType
    {
        get => this.FindControl<TextBox>("ReturnType").Text;
    }
    
    /// <summary>
    /// Initializer for AddMethodPanel
    /// </summary>
    public AddMethodPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}