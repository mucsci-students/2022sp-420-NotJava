using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// 
/// </summary>
public class TestingPanel : UserControl
{

    /// <summary>
    /// Getter for EnteredName
    /// </summary>
    public string EnteredName
    {
        get => this.FindControl<TextBox>("NameField").Text;
    }
    /// <summary>
    /// Getter for EnteredType
    /// </summary>
    public string EnteredType
    {
        get => this.FindControl<TextBox>("TypeField").Text;
    }

    /// <summary>
    /// Initializer for TestingPanel
    /// </summary>
    public TestingPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}