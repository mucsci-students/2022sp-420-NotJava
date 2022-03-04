using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class AddMethodPanel : UserControl
{
    
    public string ClassName
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
    }
    
    public string MethodName
    {
        get => this.FindControl<TextBox>("MethodName").Text;
    }
    
    public string ReturnType
    {
        get => this.FindControl<TextBox>("ReturnType").Text;
    }
    
    public AddMethodPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}