using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class AddFieldPanel : UserControl
{
    
    public string ClassName
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
    }
    
    public string FieldName
    {
        get => this.FindControl<TextBox>("TargetField").Text;
    }
    
    public string FieldType
    {
        get => this.FindControl<TextBox>("FieldType").Text;
    }
    
    public AddFieldPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}