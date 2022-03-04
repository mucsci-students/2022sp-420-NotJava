using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class AddClassPanel : UserControl
{

    public string ClassName
    {
        get => this.FindControl<TextBox>("EnteredName").Text;
    }

    public AddClassPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}