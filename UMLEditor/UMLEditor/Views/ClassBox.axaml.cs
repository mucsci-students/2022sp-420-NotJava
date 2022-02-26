using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace UMLEditor.Views;

public class ClassBox : UserControl
{
    public ClassBox()
    {
        InitializeComponent();

        StackPanel vs = this.FindControl<StackPanel>("FieldsArea");
        vs.Children.Add(new FieldContainer());
        vs.Children.Add(new FieldContainer());
        vs.Children.Add(new FieldContainer());
        
        /*
        vs.Content = new FieldContainer[]
        {
            new FieldContainer(),
            new FieldContainer()
        };
        */

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}