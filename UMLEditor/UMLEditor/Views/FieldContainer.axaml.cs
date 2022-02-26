using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class FieldContainer : UserControl
{
    public FieldContainer()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}