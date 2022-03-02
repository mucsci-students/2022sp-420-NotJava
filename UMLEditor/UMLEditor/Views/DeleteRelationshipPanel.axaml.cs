﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UMLEditor.Classes;
using System.Collections.Generic;

namespace UMLEditor.Views;

public class DeleteRelationshipPanel : UserControl
{
    public string SourceClass
    {
        get => this.FindControl<TextBox>("SourceClass").Text;
    }
    
    public string DestinationClass
    {
        get => this.FindControl<TextBox>("DestinationClass").Text;
    }
    
    public DeleteRelationshipPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}