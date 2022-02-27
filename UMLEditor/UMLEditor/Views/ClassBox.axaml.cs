using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using Avalonia.Media;

namespace UMLEditor.Views;

public class ClassBox : UserControl
{
    public ClassBox()
    {
        InitializeComponent();

        StackPanel fieldsArea = this.FindControl<StackPanel>("FieldsArea");
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());
        fieldsArea.Children.Add(new FieldContainer());

        StackPanel methodsArea = this.FindControl<StackPanel>("MethodsArea");
        methodsArea.Children.Add(new MethodContainer());
        methodsArea.Children.Add(new MethodContainer());
        methodsArea.Children.Add(new MethodContainer());
        methodsArea.Children.Add(new MethodContainer());
        methodsArea.Children.Add(new MethodContainer());

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