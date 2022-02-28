using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace UMLEditor.Views;

public class MethodContainer : UserControl
{
    public MethodContainer()
    {
        InitializeComponent();

        StackPanel paramsArea = this.FindControl<StackPanel>("ParamsArea");

        Color newBackground = new Color(255, 255, 120, 0);
        paramsArea.Children.Add(new FieldContainer("     double otherThing", newBackground));        

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}