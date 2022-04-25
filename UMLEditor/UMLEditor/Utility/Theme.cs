using System;
using System.IO;
using Avalonia.Media;
using Newtonsoft.Json;

namespace UMLEditor.Utility;

/// <summary>
/// Represents a color theme the user has 
/// </summary>
public class Theme : ICloneable
{

    /// <summary>
    /// Event raised when the theme has been reassigned
    /// </summary>
    public static event EventHandler<Theme>? ThemeChanged; 

    private static Theme _current = new Theme();
    
    /// <summary>
    /// Gets the currently loaded theme
    /// </summary>
    public static Theme Current
    {

        get => (Theme)_current.Clone();

        set
        {

            _current = value;
            ThemeChanged?.Invoke(null, _current);

        }
        
    }

    /// <summary>
    /// Attempts to load a theme from the provided file
    /// </summary>
    /// <param name="toLoad">The theme file to load</param>
    /// <returns>True if the theme was loaded, false otherwise</returns>
    public static bool TryLoadTheme(string toLoad)
    {

        try
        {

            string fileText = File.ReadAllText(toLoad);
            Theme? loadedTheme = JsonConvert.DeserializeObject<Theme>(fileText);

            if (loadedTheme is null)
            {
                return false;
            }

            Current = loadedTheme;

        }

        catch (Exception)
        {
            return false; 
        }
        
        return true;

    }

    /// <summary>
    /// Attempts to save a theme to the provided file
    /// </summary>
    /// <param name="toFile">The theme file to save to</param>
    /// <returns>True if the theme was saved, false otherwise</returns>
    public bool TrySaveTheme(string toFile)
    {

        try
        {
            File.WriteAllText(toFile, JsonConvert.SerializeObject(this));
        }

        catch (Exception)
        {
            return false; 
        }
        
        return true;

    }
    
    
    [JsonProperty(PropertyName = "CanvasColor")]
    private string _canvasColorString;
    
    /// <summary>
    /// The user's preferred color for the canvas
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush CanvasColor
    {
        
        get => new SolidColorBrush(Color.Parse(_canvasColorString));

        private set => _canvasColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "ClassBoxColor")]
    private string _classboxColorString;
    
    /// <summary>
    /// The user's preferred color for the canvas
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush ClassBoxColor
    {
        
        get => new SolidColorBrush(Color.Parse(_classboxColorString));

        private set => _classboxColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "MTitleColor")]
    private string _memberTitleColorString;
    
    /// <summary>
    /// The user's preferred color for the backgrounds of member titles in ClassBoxes
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush MemberTitleBackgroundColor
    {
        
        get => new SolidColorBrush(Color.Parse(_memberTitleColorString));

        private set => _memberTitleColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "AttribColor")]
    private string _attribBackgroundColorString;
    
    /// <summary>
    /// The user's preferred color for the backgrounds of member titles in ClassBoxes
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush AttributeBackgroundColor
    {
        
        get => new SolidColorBrush(Color.Parse(_attribBackgroundColorString));

        private set => _attribBackgroundColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "ParamColor")]
    private string _paramBackgroundColorString;
    
    /// <summary>
    /// The user's preferred color for the backgrounds of member titles in ClassBoxes
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush ParameterBackgroundColor
    {
        
        get => new SolidColorBrush(Color.Parse(_paramBackgroundColorString));

        private set => _paramBackgroundColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "TextColor")]
    private string _textBackgroundColorString;
    
    /// <summary>
    /// The user's preferred color for the backgrounds of member titles in ClassBoxes
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush TextColor
    {
        
        get => new SolidColorBrush(Color.Parse(_textBackgroundColorString));

        private set => _textBackgroundColorString = ColorBrushToHex(value);

    }

    [JsonProperty(PropertyName = "LinesColor")]
    private string _linesColor;
    
    /// <summary>
    /// The user's preferred color for the backgrounds of member titles in ClassBoxes
    /// </summary>
    [JsonIgnore]
    public SolidColorBrush LinesColor
    {
        
        get => new SolidColorBrush(Color.Parse(_linesColor));

        private set => _linesColor = ColorBrushToHex(value);

    }
    
    [JsonIgnore]
    private const string COLOR_BLACK = "#000000";
    
    [JsonIgnore]
    private const string COLOR_WHITE = "#FFFFFF";
    
    [JsonIgnore]
    private const string COLOR_CLASSBOX_DEFAULT = "#963E00";
    
    [JsonIgnore]
    private const string COLOR_MEMBERTITLE_DEFAULT = "#D85A00";
    
    [JsonIgnore]
    private const string COLOR_ATTRIBUTE_DEFAULT = "#FF6A00";
    
    [JsonIgnore]
    private const string COLOR_PARAMETER_DEFAULT = "#FF7800";
    
    [JsonIgnore]
    private const string COLOR_LINES_DEFAULT = "#6495ED";
    
    /// <summary>
    /// Default ctor
    /// </summary>
    public Theme()
    {
        
        Console.Write("");
        
        _canvasColorString = COLOR_BLACK;
        _classboxColorString = COLOR_CLASSBOX_DEFAULT;
        _memberTitleColorString = COLOR_MEMBERTITLE_DEFAULT;
        _attribBackgroundColorString = COLOR_ATTRIBUTE_DEFAULT;
        _paramBackgroundColorString = COLOR_PARAMETER_DEFAULT;
        _textBackgroundColorString = COLOR_WHITE;
        _linesColor = COLOR_LINES_DEFAULT;
        
    }

    /// <summary>
    /// Copy ctor
    /// </summary>
    /// <param name="toClone">The theme to clone</param>
    private Theme(Theme toClone)
    {

        _canvasColorString = toClone._canvasColorString;
        _classboxColorString = toClone._classboxColorString;
        _memberTitleColorString = toClone._memberTitleColorString;
        _attribBackgroundColorString = toClone._attribBackgroundColorString;
        _paramBackgroundColorString = toClone._paramBackgroundColorString;
        _textBackgroundColorString = toClone._textBackgroundColorString;
        _linesColor = toClone._linesColor;

    }
    
    /// <summary>
    /// Constructs a new theme with the provided colors
    /// </summary>
    /// <param name="canvasColor">The background color for the canvas</param>
    /// <param name="classboxColor">The color for ClassBox outlines</param>
    /// <param name="memberTitleColor">The background color for the "Fields" and "Methods" titles</param>
    /// <param name="attributeColor">The background color for fields and method signatures</param>
    /// <param name="parameterColor">The background color for method parameters</param>
    /// <param name="textColor">The font color for ClassBoxes</param>
    /// <param name="linesColor">The color for lines</param>
    public Theme(SolidColorBrush canvasColor, SolidColorBrush classboxColor, SolidColorBrush memberTitleColor, 
        SolidColorBrush attributeColor, SolidColorBrush parameterColor, SolidColorBrush textColor, SolidColorBrush linesColor) : this()
    {

        CanvasColor = canvasColor;
        ClassBoxColor = classboxColor;
        MemberTitleBackgroundColor = memberTitleColor;
        AttributeBackgroundColor = attributeColor;
        ParameterBackgroundColor = parameterColor;
        TextColor = textColor;
        LinesColor = linesColor;

    } 
    
    /// <summary>
    /// Clones this theme
    /// </summary>
    /// <returns>A distinct copy of this theme</returns>
    public object Clone()
    {

        return new Theme(this);

    }

    /// <summary>
    /// Converts the provided color brush into a hex string representation
    /// </summary>
    /// <param name="toConvert">The SolidColorBrush to convert</param>
    /// <returns>A hex string representation of the brush</returns>
    public static string ColorBrushToHex(SolidColorBrush toConvert)
    {
        
        string red   = ByteToHex(toConvert.Color.R);
        string green = ByteToHex(toConvert.Color.G);
        string blue  = ByteToHex(toConvert.Color.B);

        return $"#{red}{green}{blue}";
        
    }
    
    /// <summary>
    /// Converts the provided byte to a two character hex string
    /// </summary>
    /// <param name="toConvert">The byte to convert</param>
    /// <returns>A two character hex representation of byte</returns>
    private static string ByteToHex(byte toConvert)
    {
        
        string result = string.Format("{0:X}", toConvert);
        if (result.Length == 1)
        {
            result = $"0{result}";
        }

        return result;

    }
    
}