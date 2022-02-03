using System;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using UMLEditor.Classes;

namespace UMLEditor.Views
{
    public partial class MainWindow : Window
    {

        private Diagram activeDiagram;
        private TextBox OutputBox;
        private TextBox InputBox;
        

        public MainWindow()
        {
            
            InitializeComponent();
            
            activeDiagram = new Diagram();
            OutputBox = this.FindControl<TextBox>("OutputBox");

            InputBox = this.FindControl<TextBox>("InputBox");
    
            // MATTHEW & CJ adding a new class should be as simple as this, just remember to add input verification.
            
            //activeDiagram.Classes.Add(new Class("HELLO"));
            //activeDiagram.Classes.Add(new Class("WORLD"));

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

            const string NONEXISTENT_NAME_FORMAT = "Nonexistent class name entered ({0}).";

            if (words.Length == 0)
            {
                // No input arguments
                OutputBox.Text =
                    "To create a new relationship, please enter source and destination in the format 'A B' into the input box.";
                InputBox.Focus();
                return;

            }
            else if (words.Length != 2)
            {
                // Invalid input arguments
                OutputBox.Text = 
                    "Input must be in the form 'A B' with only one source and one destination.  Please enter your relationship into the input box.";
                InputBox.Focus();
                return;
                
            }
            else if (!activeDiagram.ClassExists(words[0]))
            {

                // First provided class name does not exist
                OutputBox.Text = string.Format(NONEXISTENT_NAME_FORMAT, words[0]);
                InputBox.Focus();
                return;

            }
            else if (!(activeDiagram.ClassExists(words[1])))
            {

                // Second provided class name does not exist
                OutputBox.Text = string.Format(NONEXISTENT_NAME_FORMAT, words[1]);
                InputBox.Focus();
                return;

            }
            
            // Create a new relationship between the source and destination provided.  Print success message and clear input.
            Relationship newRel = new Relationship(words[0], words[1]);
            activeDiagram.Relationships.Add(newRel);

            OutputBox.Text = string.Format("Success!  Relationship added!\n{0}", newRel.ToString());
            ClearInputBox();

        }
        
    }
}