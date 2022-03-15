﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// AddFieldPanel.cs
/// </summary>
public class AddFieldPanel : UserControl
{
    
    /// <summary>
    /// Getters and setters for Classname
    /// </summary>
    public string ClassName
    {
        get => this.FindControl<TextBox>("TargetClass").Text;
        set => this.FindControl<TextBox>("TargetClass").Text = value;
    }
    
    /// <summary>
    /// Getters and setters for FieldName
    /// </summary>
    public string FieldName
    {
        get => this.FindControl<TextBox>("TargetField").Text;
        set => this.FindControl<TextBox>("TargetField").Text = value;
    }
    
    /// <summary>
    /// Getters and setters for FieldType
    /// </summary>
    public string FieldType
    {
        get => this.FindControl<TextBox>("FieldType").Text;
        set => this.FindControl<TextBox>("FieldType").Text = value;
    }
    
    /// <summary>
    /// Initializer for AddFieldPanel
    /// </summary>
    public AddFieldPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}