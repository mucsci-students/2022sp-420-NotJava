using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views.CustomControls.Modal.InputForms;

/// <summary>
/// RenameMethodPanel.cs
/// </summary>
public class RenameMethodPanel : UserControl
{
    /// <summary>
    /// Getter for TargetClass
    /// </summary>
    public string TargetClass
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
    }
    
    /// <summary>
    /// Getter for OldName
    /// </summary>
    public string OldName
    {
        get => this.FindControl<TextBox>("OldName").Text;
    }
    
    /// <summary>
    /// Getter for NewName
    /// </summary>
    public string NewName
    {
        get => this.FindControl<TextBox>("NewName").Text;
    }
    
    /// <summary>
    /// Initializer for RenameMethodPanel
    /// </summary>
    public RenameMethodPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}