using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class RenameClassPanel : UserControl
{
    
    public string OldName
    {
        get => this.FindControl<TextBox>("OldName").Text;
    }
    
    public string NewName
    {
        get => this.FindControl<TextBox>("NewName").Text;
    }
    
    public RenameClassPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}