using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaColorPicker;
using UMLEditor.Utility;

namespace UMLEditor.Views.CustomControls.Modal.InputForms;

/// <summary>
/// A color editor for the theme
/// </summary>
public class ThemeEditor : UserControl
{

    private readonly ColorPicker _picker;
    private readonly Label _colorPreviewer;
    private readonly Canvas _canvas;
    private readonly ComboBox _targetSelector;
    private readonly Polyline _demoLine;
    
    // ClassBox one members
    private readonly StackPanel _classOneOutline;
    private readonly Label _classOneNameText;
    private readonly Grid _classOneFieldsBanner;
    private readonly Label _classOneFieldsTitle;
    private readonly Grid _classOneFieldOneBacking;
    private readonly Label _classOneFieldOneTitle;
    private readonly Grid _classOneMethodsBanner;
    private readonly Label _classOneMethodsTitle;
    private readonly Grid _classOneMethdoOneTitleBar;
    private readonly Label _classOneMethdoOneSignature;
    private readonly Grid _classOneParamOneBackground;
    private readonly Label _classOneParamOneText;

    /// <summary>
    /// The theme that the user has created
    /// </summary>
    public Theme NewTheme
    {

        get => new Theme(BackgroundColor, OutlinesColor, SectionsTitleColor, AttributeBackgroundColor, ParameterColor, TextColor, LineColor);

    }
    
    private SolidColorBrush BackgroundColor
    {

        get => (SolidColorBrush) _canvas.Background;
        set => ApplyBackgroundColor(value);

    }
    
    private SolidColorBrush OutlinesColor
    {

        get => (SolidColorBrush)_classOneOutline.Background;
        set => ApplyOutlineColors(value);

    }
    
    private SolidColorBrush SectionsTitleColor
    {

        get => (SolidColorBrush)_classOneFieldsBanner.Background;
        set => ApplyTitleColors(value);

    }
    
    private SolidColorBrush AttributeBackgroundColor
    {

        get => (SolidColorBrush)_classOneFieldOneBacking.Background;
        set => ApplyAttributeColors(value);

    }
    
    private SolidColorBrush ParameterColor
    {

        get => (SolidColorBrush)_classOneParamOneBackground.Background;
        set => ApplyParamColors(value);

    }
    
    private SolidColorBrush LineColor
    {

        get
        {

            IBrush? color =_demoLine.Stroke;
            if (color is not null) { return (SolidColorBrush) color; }

            return new SolidColorBrush(Colors.Red);

        }

        set => ApplyLineColor(value);

    }
    
    private SolidColorBrush TextColor
    {

        get
        {

            IBrush? foreground =_classOneFieldsTitle.Foreground;
            if (foreground is not null) { return (SolidColorBrush) foreground; }

            return new SolidColorBrush(Colors.White);

        }

        set => ApplyTextColors(value);

    }

    /// <summary>
    /// Default ctor
    /// </summary>
    public ThemeEditor()
    {
        
        InitializeComponent();

        _picker = this.FindControl<ColorPicker>("Picker");
        _colorPreviewer = this.FindControl<Label>("SelectedColorPreviewer");
        _demoLine = this.FindControl<Polyline>("DemoLine");

        // Load class one things
        _classOneOutline            = this.FindControl<StackPanel>("C1TitleBackground");
        _classOneFieldsBanner       = this.FindControl<Grid>("C1FBanner");
        _classOneFieldOneBacking    = this.FindControl<Grid>("C1F1Backing");
        _classOneMethodsBanner      = this.FindControl<Grid>("C1MTitleBackground");
        _classOneMethdoOneTitleBar  = this.FindControl<Grid>("C1M1TitleBar");
        _classOneParamOneBackground = this.FindControl<Grid>("C1P1Background");
        
        _classOneFieldsTitle        = this.FindControl<Label>("C1FTitle");
        _classOneFieldOneTitle      = this.FindControl<Label>("C1F1Title");
        _classOneMethodsTitle       = this.FindControl<Label>("C1MTitleText");
        _classOneMethdoOneSignature = this.FindControl<Label>("C1M1Signature");
        _classOneNameText           = this.FindControl<Label>("C1Title");
        _classOneParamOneText       = this.FindControl<Label>("C1P1Text");

        _targetSelector = this.FindControl<ComboBox>("TargetSelector");
        _canvas = this.FindControl<Canvas>("Canvas");
        LoadColors(Theme.Current);
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Applies the provided color to any text on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyTextColors(SolidColorBrush newColor)
    {

        _classOneFieldsTitle.Foreground        = newColor;
        _classOneFieldOneTitle.Foreground      = newColor;
        _classOneMethodsTitle.Foreground       = newColor;
        _classOneMethdoOneSignature.Foreground = newColor;
        _classOneNameText.Foreground           = newColor;
        _classOneParamOneText.Foreground       = newColor;
        
    }

    /// <summary>
    /// Applies the provided color to ClassBox outline on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyOutlineColors(SolidColorBrush newColor)
    {

        _classOneOutline.Background = newColor;

    }
    
    /// <summary>
    /// Applies the provided color to the members sections the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyTitleColors(SolidColorBrush newColor)
    {

        _classOneFieldsBanner.Background  = newColor;
        _classOneMethodsBanner.Background = newColor;

    }
    
    /// <summary>
    /// Applies the provided color to the method and field entries on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyAttributeColors(SolidColorBrush newColor)
    {

        _classOneFieldOneBacking.Background = newColor;
        _classOneMethdoOneTitleBar.Background = newColor;

    }

    /// <summary>
    /// Applies the provided color to the parameter on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyParamColors(SolidColorBrush newColor)
    {

        _classOneParamOneBackground.Background = newColor;

    }

    /// <summary>
    /// Applies the provided color the lines on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyLineColor(SolidColorBrush newColor)
    {

        _demoLine.Stroke = newColor;

    }

    /// <summary>
    /// Applies the provided color to the background on the previewer
    /// </summary>
    /// <param name="newColor">The color to apply</param>
    private void ApplyBackgroundColor(SolidColorBrush newColor)
    {

        _canvas.Background = newColor;

    }

    /// <summary>
    /// Loads all colors from the theme onto the previewer
    /// </summary>
    private void LoadColors(Theme fromTheme)
    {

        BackgroundColor = fromTheme.CanvasColor;
        OutlinesColor = fromTheme.ClassBoxColor;
        SectionsTitleColor = fromTheme.MemberTitleBackgroundColor;
        AttributeBackgroundColor = fromTheme.AttributeBackgroundColor;
        ParameterColor = fromTheme.ParameterBackgroundColor;
        LineColor = fromTheme.LinesColor;
        TextColor = fromTheme.TextColor;

        TargetSelector_OnSelectionChanged(null, null);
        
    }
    
    private void TargetSelector_OnSelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {

        /* Technically it is true that this should always be false
         * HOWEVER, with the way events work it is possible for this event to fire before the ctor is finished */ 
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (_targetSelector is null) { return; }

        SolidColorBrush newColor = new SolidColorBrush(Colors.Red);
        
        switch ((SelectedTarget)_targetSelector.SelectedIndex)
        {
            
            case SelectedTarget.BACKGROUND_COLOR:
                newColor = BackgroundColor;
                break;
            
            case SelectedTarget.CLASSBOX_OUTLINES_COLOR:
                newColor = OutlinesColor;
                break;
            
            case SelectedTarget.SECTIONS_BACKGROUND_COLOR:
                newColor = SectionsTitleColor;
                break;
            
            case SelectedTarget.ATTRIBUTES_BACKGROUND_COLOR:
                newColor = AttributeBackgroundColor;
                break;
            
            case SelectedTarget.PARAMETER_BACKGROUND_COLOR:
                newColor = ParameterColor;
                break;
            
            case SelectedTarget.LINES_COLOR:
                newColor = LineColor;
                break;
            
            case SelectedTarget.FONT_COLOR:
                newColor = TextColor;
                break;
            
        }

        _picker.Color = newColor.Color;
        _colorPreviewer.Background = newColor;

    }
    
    private void Picker_OnLayoutUpdated(object? sender, EventArgs ex)
    {

        Color e = _picker.Color;
        SolidColorBrush newColor = new SolidColorBrush(new Color(e.A, e.R, e.G, e.B));
        _colorPreviewer.Background = newColor;
        
        switch ((SelectedTarget)_targetSelector.SelectedIndex)
        {
            
            case SelectedTarget.BACKGROUND_COLOR:
                BackgroundColor = newColor;
                break;
            
            case SelectedTarget.CLASSBOX_OUTLINES_COLOR:
                OutlinesColor = newColor;
                break;
            
            case SelectedTarget.SECTIONS_BACKGROUND_COLOR:
                SectionsTitleColor = newColor;
                break;
            
            case SelectedTarget.ATTRIBUTES_BACKGROUND_COLOR:
                AttributeBackgroundColor = newColor;
                break;
            
            case SelectedTarget.PARAMETER_BACKGROUND_COLOR:
                ParameterColor = newColor;
                break;
            
            case SelectedTarget.LINES_COLOR:
                LineColor = newColor;
                break;
            
            case SelectedTarget.FONT_COLOR:
                TextColor = newColor;
                break;
            
        }
        
    }

    private void Reset_OnClick(object? sender, RoutedEventArgs e) => LoadColors(new Theme());
    
}

enum SelectedTarget
{
    
    BACKGROUND_COLOR = 0,
    CLASSBOX_OUTLINES_COLOR = 1,
    SECTIONS_BACKGROUND_COLOR = 2,
    ATTRIBUTES_BACKGROUND_COLOR = 3,
    PARAMETER_BACKGROUND_COLOR = 4,
    LINES_COLOR = 5,
    FONT_COLOR = 6
    
}