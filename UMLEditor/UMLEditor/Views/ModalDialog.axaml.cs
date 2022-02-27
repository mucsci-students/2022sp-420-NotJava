using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views;

/// <summary>
/// Enum for the various button combinations that could be passed into a modal dialogue
/// </summary>
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
/// <summary>
/// Default ctor for a ModalDialog
/// </summary>
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

    /// <summary>
    /// Ctor for a modal dialog given params windowTitle and enabledButtons
    /// </summary>
    /// <param name="windowTitle">The desired title for the new window</param>
    /// <param name="enabledButtons">The combination of buttons to be enabled for the new window</param>
    private ModalDialog(string windowTitle, params DialogButtons[] enabledButtons) : this()
    {

        // For each of the buttons within enabledButtons, use a compound assignment to toggle visibility
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

    /// <summary>
    /// Creating a new modal dialog
    /// </summary>
    /// <param name="windowTitle">Desired title for new window</param>
    /// <param name="withButtons">Buttons that should be visible on new window</param>
    /// <typeparam name="Content_T">Controller that should have all the specific axaml and c# for each task</typeparam>
    /// <returns></returns>
    public static ModalDialog CreateDialog<Content_T>(string windowTitle, params DialogButtons[] withButtons)
        where Content_T : UserControl, new()
    {

        ModalDialog result = new ModalDialog(windowTitle, withButtons);
        result.Prompt = new Content_T();

        return result;

    }

    /// <summary>
    /// Getting the content as a UserControl for use in new window dialogue
    /// </summary>
    /// <typeparam name="Content_T">Content that will be used in dialogue</typeparam>
    /// <returns></returns>
    public Content_T GetPrompt<Content_T>() where Content_T : UserControl, new()
    {
        return (Content_T) Prompt;
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
    }
    
    /// <summary>
    /// The various 'resolutions' for a modal dialog based on which button was selected.
    /// </summary>
    private void OnModalResolved(object sender, RoutedEventArgs e)
    {
        
        if (sender.Equals(_yesButton))
        {
            Close((int)DialogButtons.YES);
        }
        
        else if (sender.Equals(_noButton))
        {
            Close((int)DialogButtons.NO);
        }
        
        else if (sender.Equals(_okayButton))
        {
            Close((int)DialogButtons.OKAY);
        }
        
        else
        {
            Close((int)DialogButtons.CANCEL);
        }

    }
    
}