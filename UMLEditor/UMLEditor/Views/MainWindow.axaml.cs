using System;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram ActiveDiagram;
        private TextBox OutputBox;
        private TextBox InputBox;
        

        public MainWindow()
        {
            
            InitializeComponent();
            
            ActiveDiagram = new Diagram();
            OutputBox = this.FindControl<TextBox>("OutputBox");

            InputBox = this.FindControl<TextBox>("InputBox");
            
            // MATTHEW & CJ adding a new class should be as simple as this, just remember to add input verification.
            
            // ActiveDiagram.Classes.Add(new Class("HELLO"));
            // ActiveDiagram.Classes.Add(new Class("WORLD"));
            

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
                return;

            }
            
            ClearInputBox();
            OutputBox.Text = string.Format("Relationship created ({0} => {1})", SourceClassName, DestClassName);

        }
        
    }
}
