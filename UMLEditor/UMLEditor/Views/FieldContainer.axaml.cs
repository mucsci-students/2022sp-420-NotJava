using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace UMLEditor.Views;

public class FieldContainer : UserControl
{

    private Label _display;
    private Grid _backing;
    
    public FieldContainer()
    {
        InitializeComponent();
        _display = this.FindControl<Label>("ParamDisplay");
        _backing = this.FindControl<Grid>("Backing");

    }

    public FieldContainer(string withText, Color backgroundColor) : this()
    {

        _display.Content = withText;
        _backing.Background = new SolidColorBrush(backgroundColor);

    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}