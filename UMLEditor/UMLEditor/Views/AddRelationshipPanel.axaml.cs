using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UMLEditor.Views;

public class AddRelationshipPanel : UserControl
{
    public string SourceClass
    {
        get => this.FindControl<TextBox>("SourceClass").Text;
    }
    
    public string DestinationClass
    {
        get => this.FindControl<TextBox>("DestinationClass").Text;
    }
    
    public string RelationshipType
    {
        get => this.FindControl<TextBox>("RelationshipType").Text;
    }
    
    public AddRelationshipPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}