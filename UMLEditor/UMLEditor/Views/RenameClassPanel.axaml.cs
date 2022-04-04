using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// RenameClassPanel.cs
/// </summary>
public class RenameClassPanel : UserControl
{
    
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
    /// Getter for RenameClassPanel
    /// </summary>
    public RenameClassPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}