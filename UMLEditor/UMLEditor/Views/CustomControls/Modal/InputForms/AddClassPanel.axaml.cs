using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views.CustomControls.Modal.InputForms;

/// <summary>
/// AddClassPanel.cs
/// </summary>
public class AddClassPanel : UserControl
{

    /// <summary>
    /// Public accessor for the class name
    /// </summary>
    public string ClassName
    {
        get => this.FindControl<TextBox>("EnteredName").Text;
    }

    /// <summary>
    /// Initializer for panel
    /// </summary>
    public AddClassPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}