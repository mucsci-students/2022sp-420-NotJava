using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace UMLEditor.Views.CustomControls.Modal.InputForms;

/// <summary>
/// Enum to differentiate between the different icons that can be used within an 'AlertPanel'
/// </summary>
public enum AlertIcon
{

#pragma warning disable CS1591
    NONE,
    ERROR,
    WARNING,
    QUESTION,
    INFO
#pragma warning restore CS1591
    
}

/// <summary>
/// The required images for use in an 'AlertPanel'
/// </summary>
public class AlertPanel : UserControl
{

    private Image _errorIcon;
    private Image _warningIcon;
    private Image _questionIcon;
    private Image _infoIcon;
    
    /// <summary>
    /// The title given to the panel upon creation
    /// </summary>
    public string AlertTitle
    {

        get => (string)this.FindControl<TextBlock>("Title").Text;
        set => this.FindControl<TextBlock>("Title").Text = value;

    }

    /// <summary>
    /// The message given to the panel upon creation
    /// </summary>
    public string AlertMessage
    {

        get => (string)this.FindControl<TextBlock>("Message").Text;
        set => this.FindControl<TextBlock>("Message").Text = value;

    }

    private AlertIcon _currentIcon;

    /// <summary>
    /// Setter for the icon
    /// </summary>
    public AlertIcon DialogIcon
    {

        get => _currentIcon;
        set => ChangeIcon(_currentIcon = value);

    }

    /// <summary>
    /// Initializes with icon from specified name
    /// </summary>
    public AlertPanel()
    {
        InitializeComponent();

        _warningIcon = this.FindControl<Image>("WarningIcon");
        _errorIcon = this.FindControl<Image>("ErrorIcon");
        _questionIcon = this.FindControl<Image>("QuestionIcon");
        _infoIcon = this.FindControl<Image>("InfoIcon");
        DialogIcon = AlertIcon.NONE;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Allows for the swapping in and out of the various icon types
    /// </summary>
    /// <param name="toIcon">The panel we are defining an icon for</param>
    private void ChangeIcon(AlertIcon toIcon)
    {

        // Hide all icons
        _warningIcon.IsVisible = false;
        _errorIcon.IsVisible = false;
        _questionIcon.IsVisible = false;
        _infoIcon.IsVisible = false;
        
        // Switch statement for each icon scenario...
        switch (toIcon)
        {
            // If 'ERROR'...
            case AlertIcon.ERROR:
                _errorIcon.IsVisible = true;
                break;
            
            // If 'WARNING'...
            case AlertIcon.WARNING:
                _warningIcon.IsVisible = true;
                break;
            
            // If 'QUESTION'...
            case AlertIcon.QUESTION:
                _questionIcon.IsVisible = true;
                break;
            
            // If 'INFO'...
            case AlertIcon.INFO:
                _infoIcon.IsVisible = true;
                break;
            
        }
        
    }
}