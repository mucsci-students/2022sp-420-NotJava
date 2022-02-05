using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram ActiveDiagram;

        private TextBox OutputBox;
        private TextBox InputBox;

        private JSONDiagramFile ActiveFile;

        public MainWindow()
        {
            
            InitializeComponent();
            
            ActiveDiagram = new Diagram();
            ActiveFile = new JSONDiagramFile();
            
            OutputBox = this.FindControl<TextBox>("OutputBox");
            InputBox = this.FindControl<TextBox>("InputBox");
            
            // MATTHEW & CJ adding a new class should be as simple as this, just remember to add input verification.
            
             //ActiveDiagram.Classes.Add(new Class("HELLO"));
             //ActiveDiagram.Classes.Add(new Class("WORLD"));
             
             //Class CurrentClass = ActiveDiagram.GetClassByName("HELLO");
             //CurrentClass.Attributes.Add(new AttributeObject("Test"));

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ClearInputBox()
        {

            InputBox.Text = "";

        }

        private void ExitB_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void HelpB_OnClick(object sender, RoutedEventArgs e)
        {
            
            ClearInputBox();            
            
            // Output list of possible commands.
            OutputBox.Text = 
                "New Class: Add a new class to the diagram" + 
                "\nDelete Class: Delete an existing class" +
                "\nRename Class: Rename an existing class" +
                "\nAdd Relationship: Add a relationship between classes" +
                "\nDelete Relationship: Delete an existing relationship" +
                "\nAdd Attribute: Add an attribute to an existing Class" +
                "\nDelete Attribute: Delete an existing class attribute" +
                "\nRename Attribute: Rename an existing attribute" +
                "\nSave: Save your progress" +
                "\nLoad: Load a previously saved diagram" +
                "\nList Classes: List all existing classes" +
                "\nList Attributes: List all attributes of a class" +
                "\nList Relationships: List all relationships of a class";
            
        }

        private void List_Classes_OnClick(object sender, RoutedEventArgs e)
        {
            
            // TextBox outputBox = this.FindControl<TextBox>("OutputBox");

            // outputBox.Text = "laksjdflaskjdf";

        }

        private void AddRelationship_OnClick(object sender, RoutedEventArgs e)
        {

            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, add relationship if valid.
            string input = InputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                OutputBox.Text = 
                    "To create a new relationship, please enter source and destination in the format " +
                    "'A B' into the input box and then click 'Add Relationship'.";
                
                InputBox.Focus();
                return;

            }
            
            else if (words.Length != 2)
            {
                
                // Invalid input arguments
                OutputBox.Text = 
                    "Input must be in the form 'A B' with only one source and one destination.  " +
                    "Please enter your relationship into the input box and click 'Add Relationship'.";
                
                InputBox.Focus();
                return;
                
            }

            string SourceClassName = words[0];
            string DestClassName = words[1];
            
            try
            {
                
                ActiveDiagram.AddRelationship(SourceClassName, DestClassName);
                
            }
            
            catch (ClassNonexistentException exception)
            {

                OutputBox.Text = exception.Message;
                InputBox.Focus();
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Relationship created ({0} => {1})", SourceClassName, DestClassName);

        }

        private void SaveButtonOnClick(object sender, RoutedEventArgs e)
        {

            string filename = InputBox.Text;

            if (filename.Trim().Length == 0)
            {

                OutputBox.Text = "To save to a JSON file, please enter the path of the file" +
                                 " you wish to save into the input box and click \"Save\"";

                return;

            }

            try
            {

                ActiveFile.SaveDiagram(ref ActiveDiagram, filename);
                OutputBox.Text = string.Format("Current diagram saved to {0}", filename);
                ClearInputBox();

            }
            
            catch (Exception exception)
            {

                // For when the user enters invalid characters in the file path
                string message = exception.Message;
                if (message.StartsWith("The filename, directory name, or volume label syntax is incorrect"))
                {

                    OutputBox.Text = string.Format("Invalid file name/path: {0}", filename);
                    return;

                }

                OutputBox.Text = exception.Message;
                
            }
            
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {

            string filename = InputBox.Text;
            if (filename.Trim().Length == 0)
            {

                OutputBox.Text = "To load a diagram from a JSON file, please enter the path of the file" +
                                 " you wish to load into the input box and click \"Load\"";
                return;

            }
            
            try
            {

                ActiveDiagram = ActiveFile.LoadDiagram(filename);
                ClearInputBox();
                OutputBox.Text = String.Format("Diagram loaded from {0}", filename);

            }
            
            catch (Exception exception)
            {

                // For when the user enters invalid characters in the file path
                string message = exception.Message;
                if (message.StartsWith("The filename, directory name, or volume label syntax is incorrect"))
                {

                    OutputBox.Text = string.Format("Invalid file name/path: {0}", filename);
                    return;

                }
                
                OutputBox.Text = message;

            }
            
        }

        private void Delete_Attribute_OnClick(object sender, RoutedEventArgs e)
        {
            
            //User input is taken in from the textbox, validation is done to make sure that what the user entered is valid, delete attribute if is.
            string input = InputBox.Text;
            
            //Split the input into words to use later on.
            string[] words = input.Split(" ".ToCharArray() , StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                
                // No input arguments
                OutputBox.Text = 
                    "To delete an existing attribute enter source class and attribute in the format " +
                    "'Class Attribute' into the input box and then click 'Delete Attribute'.";
                
                InputBox.Focus();
                return;

            }
            
            else if (words.Length != 2)
            {
                
                // Invalid input arguments
                OutputBox.Text = 
                    "Input must be in the form 'Class Attribute' with only one class and one attribute.  " +
                    "Please enter this into the input box and click 'Delete Attribute'.";
                
                InputBox.Focus();
                return;
                
            }
            
            string TargetClassName = words[0];
            string TargetAttributeName = words[1];

            // Create CurrentClass for use in reaching its attributes
            Class CurrentClass = ActiveDiagram.GetClassByName(TargetClassName);

            // If the TargetClass does not exist throw an error
            if (CurrentClass == null)
            {
                OutputBox.Text = "Nonexistent class entered";
                InputBox.Focus();
                return;
            }

            try
            {
                
                CurrentClass.DeleteAttribute(TargetAttributeName);

            }
            
            // Check if the attribute doesn't exist.
            catch (AttributeNonexistentException exception)
            {

                OutputBox.Text = exception.Message;
                InputBox.Focus();
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Attribute Deleted ({0} => {1})", TargetClassName, TargetAttributeName);

        }
    }
}
