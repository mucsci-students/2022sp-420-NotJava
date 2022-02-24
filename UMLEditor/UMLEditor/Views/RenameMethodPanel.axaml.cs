using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public partial class RenameMethodPanel : UserControl
{
    public string TargetClass
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
    }
    
    public string OldName
    {
        get => this.FindControl<TextBox>("OldName").Text;
    }
    
    public string NewName
    {
        get => this.FindControl<TextBox>("NewName").Text;
    }
    public RenameMethodPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}