using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

public enum DialogButtons
{
    
    OKAY = 1,
    CANCEL = 2,
    YES = 4,
    NO = 8,
    OK_CANCEL = 3,
    YES_NO = 12,
    YES_NO_CANCEL = 14
    
}

public class ModalDialog: Window
{

    private readonly Button _yesButton;
    private readonly Button _noButton;
    private readonly Button _okayButton;
    private readonly Button _cancelButton;

    private readonly ScrollViewer _contentArea;
    private object Prompt
    {
        get => _contentArea.Content;
        set => _contentArea.Content = value;
    }

    public ModalDialog()
    {
        
        InitializeComponent();

        #if DEBUG
            this.AttachDevTools();
        #endif
        
        // Load the buttons
        _yesButton = this.FindControl<Button>("YesButton");
        _noButton = this.FindControl<Button>("NoButton");
        _okayButton = this.FindControl<Button>("OkayButton");
        _cancelButton = this.FindControl<Button>("CancelButton");

        // Grab the content area
        _contentArea = this.FindControl<ScrollViewer>("ContentArea");

    }

    private ModalDialog(string windowTitle, params DialogButtons[] enabledButtons) : this()
    {

        int buttonsState = 0;
        foreach (var currentBtn in enabledButtons)
        {
            buttonsState |= (int)currentBtn;
        }
        
        _yesButton.IsVisible = (buttonsState & (int)DialogButtons.YES) != 0;
        _noButton.IsVisible = (buttonsState & (int)DialogButtons.NO) != 0;
        _okayButton.IsVisible = (buttonsState & (int)DialogButtons.OKAY) != 0;
        _cancelButton.IsVisible = (buttonsState & (int)DialogButtons.CANCEL) != 0;

        Title = windowTitle;

    }

    public static ModalDialog CreateDialog<Content_T>(string windowTitle, params DialogButtons[] withButtons)
        where Content_T : UserControl, new()
    {

        ModalDialog result = new ModalDialog(windowTitle, withButtons);
        result.Prompt = new Content_T();

        return result;

    }

    public Content_T GetPrompt<Content_T>() where Content_T : UserControl, new()
    {
        return (Content_T) Prompt;
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
    }
    
    private void OnModalResolved(object sender, RoutedEventArgs e)
    {

        if (sender == _yesButton)
        {
            Close((int)DialogButtons.YES);
        }
        
        else if (sender == _noButton)
        {
            Close((int)DialogButtons.NO);
        }
        
        else if (sender == _okayButton)
        {
            Close((int)DialogButtons.OKAY);
        }

        else
        {
            Close((int)DialogButtons.CANCEL);
        }

    }
    
}