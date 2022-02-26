using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Media;
using DynamicData.Binding;

namespace UMLEditor.Views;

public class ClassBox : UserControl
{

    private Grid _titleBar;
    private bool _beingDragged;
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

        _beingDragged = false;
        _titleBar = this.FindControl<Grid>("TitleBar");
        _titleBar.PointerPressed += (object sender, PointerPressedEventArgs args) =>
        {
            _beingDragged = true;
        };

        _titleBar.PointerReleased += (object sender, PointerReleasedEventArgs args) =>
        {
            _beingDragged = false;
        };

        PointerMoved += (object sender, PointerEventArgs args) =>
        {

            if (_beingDragged)
            {

                // Get the position of the cursor relative to the canvas
                Point pointerLocation = args.GetPosition(this.Parent);
                
                // Set the ClassBox's location to the location of the cursor
                Canvas.SetLeft(this, pointerLocation.X);
                Canvas.SetTop(this, pointerLocation.Y);

            }
            
        };

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}