using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class AddFieldPanel : UserControl
{
    
    public string ClassName
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
        set => this.FindControl<TextBox>("TargetClass").Text = value;
    }
    
    public string FieldName
    {
        get => this.FindControl<TextBox>("TargetField").Text;
        set => this.FindControl<TextBox>("TargetField").Text = value;
    }
    
    public string FieldType
    {
        get => this.FindControl<TextBox>("FieldType").Text;
        set => this.FindControl<TextBox>("FieldType").Text = value;
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