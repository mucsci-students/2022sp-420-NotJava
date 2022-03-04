using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class TestingPanel : UserControl
{

    public string EnteredName
    {
        get => this.FindControl<TextBox>("NameField").Text;
    }
    public string EnteredType
    {
        get => this.FindControl<TextBox>("TypeField").Text;
    }

    public TestingPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}