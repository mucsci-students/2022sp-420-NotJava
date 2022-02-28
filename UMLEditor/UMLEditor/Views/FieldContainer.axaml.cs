namespace UMLEditor.Views;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using UMLEditor.Classes;

public class FieldContainer : UserControl
{

    private Label _display;
    private Grid _backing;
    private ClassBox _parentWindow;
    protected Button _deleteButton; 
    protected Button _editButton; 
    
    // Properties for field name/ type
    public string FieldName { get; protected set; }
    public string Type { get; protected set; }
    
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
    public FieldContainer()
    {
        
        InitializeComponent();
        
        // Grab controls
        _display = this.FindControl<Label>("ParamDisplay");
        _backing = this.FindControl<Grid>("Backing");
        _editButton = this.FindControl<Button>("EditButton");
        _deleteButton = this.FindControl<Button>("DeleteButton");

    }

    /// <summary>
    /// Constructs a FieldContainer to the provided specifications
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="withType">The type of the field</param>
    /// <param name="parentWindow">The parent ClassBox this sits in</param>
    /// <param name="isMethodParam">Indicates whether or not this contains a method parameter</param>
    public FieldContainer(string withType, string fieldName, ClassBox parentWindow, bool isMethodParam = false) : this()
    {

        FieldName = fieldName;
        Type = withType;
        _display.Content = $"{withType} {fieldName}";
        _parentWindow = parentWindow;
        
        if (isMethodParam)
        {
            
            // Adopt style for a method parameter
            _backing.Background = new SolidColorBrush(paramBackground);
            _display.Content = "     " + _display.Content;

        }

        /* If this is a field display, then install the event handler for fields
         * Event handlers for method parameters need to be installed on the subclass for it */
        else
        {

            _deleteButton.Click += (object sender, RoutedEventArgs args) =>
            {
                _parentWindow.RequestFieldDeletion(this);   
            };

        }
        
    }

    /// <summary>
    /// Constructs a FieldContainer to the provided specifications
    /// </summary>
    /// <param name="template">A NameTypeObject to use as a template for this method</param>
    /// <param name="parentWindow">The parent classbox this is within</param>
    /// <param name="isMethodParam">Indicates whether or not this is a method parameter</param>
    public FieldContainer(NameTypeObject template, ClassBox parentWindow, bool isMethodParam = false) 
        : this(template.Type, template.AttributeName, parentWindow, isMethodParam)
    {  }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}