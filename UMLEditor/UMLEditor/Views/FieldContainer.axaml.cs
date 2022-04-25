using UMLEditor.Utility;

namespace UMLEditor.Views;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Classes;

/// <summary>
/// FieldContainer.cs
/// </summary>
public class FieldContainer : UserControl
{
    
    /// <summary>
    /// Delete Button
    /// </summary>
    protected Button _deleteButton; 
    /// <summary>
    /// Edit Button
    /// </summary>
    protected Button _editButton; 
    private Label _display;
    private Grid _backing;
    
    /// <summary>
    /// Properties for field name
    /// </summary>
    public string FieldName { get; protected set; }
    /// <summary>
    /// Properties for Type
    /// </summary>
    public string Type { get; protected set; }

    /// <summary>
    /// isMethodParam
    /// </summary>
    protected bool _isMethodParam;
    /// <summary>
    /// isInEditMode
    /// </summary>
    protected bool _isInEditMode;
    
    /// <summary>
    /// The NameTypeObject representation of this control
    /// </summary>
    public NameTypeObject NTO_Representation
    {
        get => new NameTypeObject(Type, FieldName);
    }

    // Background color to use when this is displaying a parameter to a method
    private static readonly Color paramBackground = new Color(255, 255, 120, 0);
    
    /// <summary>
    /// Default ctor
    /// </summary>
        #pragma warning disable CS8618
    public FieldContainer()
    {
        
        InitializeComponent();
        
        // Grab controls
        _display = this.FindControl<Label>("ParamDisplay");
        _backing = this.FindControl<Grid>("Backing");
        _editButton = this.FindControl<Button>("EditButton");
        _deleteButton = this.FindControl<Button>("DeleteButton");

        // Bind to theme changes
        Theme.ThemeChanged += (sender, newTheme) => ApplyColors();
        
        // Apply Colors
        ApplyColors();

    }
    #pragma warning restore CS8618
    
    /// <summary>
    /// Constructs a FieldContainer to the provided specifications
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="withType">The type of the field</param>
    /// <param name="parentWindow">The parent ClassBox this sits in</param>
    /// <param name="isMethodParam">Indicates whether or not this contains a method parameter</param>
    /// <param name="inEditMode">Indicates whether or not this is in edit mode</param>
    public FieldContainer(string withType, string fieldName, ClassBox parentWindow, bool isMethodParam = false, bool inEditMode = false) : this()
    {
        Type = withType;
        FieldName = fieldName;
        var parentClassBox = parentWindow;

        _isMethodParam = isMethodParam;
        if (_isMethodParam)
        {
            
            // Adopt style for a method parameter
            _backing.Background = new SolidColorBrush(paramBackground);

        }

        /* If this is a field display, then install the event handler for fields
         * Event handlers for method parameters need to be installed on the subclass for it */
        else
        {

            _deleteButton.Click += (_, _) =>
            {
                parentClassBox.RequestFieldDeletion(this);   
            };

            _editButton.Click += (_, _) =>
            {
                parentClassBox.RequestFieldRename(this);
            };

        }
        
        UpdateNameLabel();
        ToggleEditMode(inEditMode);
        
    }

    /// <summary>
    /// Constructs a FieldContainer to the provided specifications
    /// </summary>
    /// <param name="template">A NameTypeObject to use as a template for this method</param>
    /// <param name="parentWindow">The parent classbox this is within</param>
    /// <param name="isMethodParam">Indicates whether or not this is a method parameter</param>
    /// <param name="isInEditMode">Indicates whether or not this is in edit mode</param>
    public FieldContainer(NameTypeObject template, ClassBox parentWindow, bool isMethodParam = false, bool isInEditMode = false) 
        : this(template.Type, template.AttributeName, parentWindow, isMethodParam, inEditMode:isInEditMode)
    {  }

    /// <summary>
    /// Changes the type and name of this container
    /// </summary>
    /// <param name="newAnatomy">The new type / name to adopt</param>
    public void ChangeAnatomy(NameTypeObject newAnatomy)
    {

        FieldName = newAnatomy.AttributeName;
        Type = newAnatomy.Type;

        UpdateNameLabel();
        
    }

    /// <summary>
    /// Redraws the type / name on the displayed label
    /// </summary>
    private void UpdateNameLabel()
    {
        _display.Content = "_" + (_isMethodParam ? $"     {Type} {FieldName}" : $"{Type} {FieldName}");
    }

    /// <summary>
    /// Toggles this control into/ out of edit mode
    /// </summary>
    /// <param name="inEditMode">The new edit mode setting</param>
    public void ToggleEditMode(bool inEditMode)
    {

        _isInEditMode = inEditMode;
        _editButton.IsVisible = _isInEditMode;
        _deleteButton.IsVisible = _isInEditMode;

        if (_isMethodParam)
        {
            this.IsVisible = inEditMode;
        }
        
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Applies all colors to this container
    /// </summary>
    protected void ApplyColors()
    {

        if (!_isMethodParam) { _backing.Background = Theme.Current.AttributeBackgroundColor; }

        else { _backing.Background = Theme.Current.ParameterBackgroundColor; }

        _display.Foreground = Theme.Current.TextColor;

    }
    
}