namespace UMLEditor.Views;

using Avalonia.Interactivity;
using UMLEditor.Classes;

/// <summary>
/// A subclass of FieldContainer that defines more specific behaviors for method parameters.
/// The views for this are defined in FieldContainer's .axaml file
/// </summary>
public class ParameterContainer : FieldContainer
{
    
    // The MethodContainer that this ParameterContainer is sitting in
    private MethodContainer _parentMethod;
    
    /// <summary>
    /// Constructs a method parameter container to the provided specifications
    /// </summary>
    /// <param name="template">A NameTypeObject to use as a template for this method parameter</param>
    /// <param name="inMethod">The MethodContainer that this ParameterContainer sits in</param>
    /// <param name="parentWindow">The parent ClassBox that this ParameterContainer is within</param>
    /// <param name="isInEditMode">Indicates if this control is in edit mode</param>
    public ParameterContainer(NameTypeObject template, MethodContainer inMethod, ClassBox parentWindow, bool isInEditMode = false)
        : base(template.Type, template.AttributeName, parentWindow, true, isInEditMode)
    {

        _parentMethod = inMethod;
        
        // Install the event handler for method parameters
        _deleteButton.Click += (object sender, RoutedEventArgs args) =>
        {
            parentWindow.RequestParameterDeletion(this, _parentMethod);
        };

        _editButton.Click += (object sender, RoutedEventArgs args) =>
        {
            parentWindow.RequestParamRename(this, _parentMethod);
        };

    }
    
}