using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UMLEditor.Classes;
using System.Collections.Generic;

namespace UMLEditor.Views;

public class ChangeRelationshipPanel : UserControl
{
    public string SourceClass
    {
        get => this.FindControl<TextBox>("SourceClass").Text;
    }
    
    public string DestinationClass
    {
        get => this.FindControl<TextBox>("DestinationClass").Text;
    }

    public string SelectedType
    {

        get
        {
            ComboBoxItem? selectedItem = (ComboBoxItem?) _typeSelector.SelectedItem;

            if (selectedItem is not null)
            {
                return (string) selectedItem.Content;
            }
  
            return "";
        }

    }

    private readonly ComboBox _typeSelector;
    
    public ChangeRelationshipPanel()
    {
        InitializeComponent();

        _typeSelector = this.FindControl<ComboBox>("TypeSelection");
        List<ComboBoxItem> newItems = new List<ComboBoxItem>();
        foreach (string relationshipType in  Relationship.ValidTypes)
        {
            
            ComboBoxItem newItem = new ComboBoxItem();
            newItem.Content = relationshipType;
            newItems.Add(newItem);
        }

        _typeSelector.Items = newItems;
        _typeSelector.SelectedIndex = 0;

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}