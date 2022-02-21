using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public class GenericModal : Window
{
    public GenericModal()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ModalExit_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}