using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// MainView.cs
/// </summary>
public class MainView : UserControl
{
    /// <summary>
    /// MainView initializer
    /// </summary>
    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}