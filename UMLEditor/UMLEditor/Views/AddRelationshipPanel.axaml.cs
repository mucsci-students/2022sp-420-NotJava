using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UMLEditor.Classes;
using System.Collections.Generic;

namespace UMLEditor.Views;

public class AddRelationshipPanel : UserControl
{
    public string SourceClass
    {
        
        get
        {

            ComboBoxItem? selectedItem = (ComboBoxItem?) _sourceSelector.SelectedItem;

            if (selectedItem is not null)
            {
                return (string) selectedItem.Content;
            }

            return "";

        }
        
    }
    
    public string DestinationClass
    {
        
        get
        {

            ComboBoxItem? selectedItem = (ComboBoxItem?) _destSelector.SelectedItem;

            if (selectedItem is not null)
            {
                return (string) selectedItem.Content;
            }

            return "";

        }
        
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
    private readonly ComboBox _sourceSelector;
    private readonly ComboBox _destSelector;
    
    public AddRelationshipPanel()
    {
        
        InitializeComponent();

        _typeSelector = this.FindControl<ComboBox>("TypeSelection");
        _sourceSelector = this.FindControl<ComboBox>("SourceClassSelector");
        _destSelector = this.FindControl<ComboBox>("DestClassSelector");
        
        AddStringOptionsTo(_typeSelector, Relationship.ValidTypes.ToArray());
        
    }

    /// <summary>
    /// Adds an array of class names to the source/ dest class selection boxes
    /// </summary>
    /// <param name="classNames"></param>
    public void LoadClassNames(params string[] classNames)
    {
        
        AddStringOptionsTo(_sourceSelector, classNames);
        AddStringOptionsTo(_destSelector, classNames);
        
    }

    /// <summary>
    /// Makes the type selection prompts invisible
    /// </summary>
    public void HideTypeSelection()
    {

        this.FindControl<Label>("TypeSelectionLabel").IsVisible = false;
        _typeSelector.IsVisible = false;

    }
    
    private void AddStringOptionsTo(ComboBox targetBox, params string[] toAdd)
    {
        
        List<ComboBoxItem> newItems = new List<ComboBoxItem>();
        foreach (string item in toAdd)
        {
            ComboBoxItem newItem = new ComboBoxItem();
            newItem.Content = item;
            newItems.Add(newItem);
        }
        
        targetBox.Items = newItems;
        targetBox.SelectedIndex = 0;
        
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}