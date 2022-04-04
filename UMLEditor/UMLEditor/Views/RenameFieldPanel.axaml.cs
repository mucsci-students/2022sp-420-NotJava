using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// RenameFieldPanel.cs
/// </summary>
public class RenameFieldPanel : UserControl
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
    /// Getter for RenameFieldPanel
    /// </summary>
    public RenameFieldPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}