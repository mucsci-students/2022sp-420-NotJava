using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace UMLEditor.Views;

public enum AlertIcon
{
    
    NONE,
    ERROR,
    WARNING,
    QUESTION,
    INFO
    
}

public class AlertPanel : UserControl
{

    private Image _errorIcon;
    private Image _warningIcon;
    private Image _questionIcon;
    private Image _infoIcon;
    
    
    public string AlertTitle
    {

        get => (string)this.FindControl<TextBlock>("Title").Text;

        set => this.FindControl<TextBlock>("Title").Text = value;

    }

    public string AlertMessage
    {

        get => (string)this.FindControl<TextBlock>("Message").Text;

        set => this.FindControl<TextBlock>("Message").Text = value;

    }

    private AlertIcon _currrentIcon;

    public AlertIcon DialogIcon
    {

        get => _currrentIcon;
        set
        {
            ChangeIcon(_currrentIcon = value);
        }

    }

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

    private void ChangeIcon(AlertIcon toIcon)
    {

        // Hide all icons
        _warningIcon.IsVisible = false;
        _errorIcon.IsVisible = false;
        _questionIcon.IsVisible = false;
        _infoIcon.IsVisible = false;
        
        switch (toIcon)
        {
           
            case AlertIcon.ERROR:
                _errorIcon.IsVisible = true;
                break;
            
            case AlertIcon.WARNING:
                _warningIcon.IsVisible = true;
                break;
            
            case AlertIcon.QUESTION:
                _questionIcon.IsVisible = true;
                break;
            
            case AlertIcon.INFO:
                _infoIcon.IsVisible = true;
                break;
            
        }
        
    }
}