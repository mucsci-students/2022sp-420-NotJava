# NotJava UML Editor

Cross-platform, GUI-based UML editor written for Millersville University CSCI 420

## Prerequisites
- [git CLI](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git) (optional)
- [.NET Core 6 SDK](https://dotnet.microsoft.com/en-us/download)

## Build Instructions
- Open Terminal
- Run command ``git clone https://github.com/mucsci-students/2022sp-420-NotJava.git`` (or download source code as .zip)
- Run ``cd 2022sp-420-NotJava``
- Run ``dotnet build``

## Running Instructions
### For GUI:
- To run the native executable in graphical mode: 
  - In Linux, MacOS, and Windows Powershell: ``./UMLEditor/UMLEditor.NetCore/bin/Debug/net6.0/UMLEditor.NetCore``
  - In Windows CMD: ``.\UMLEditor\UMLEditor.NetCore\bin\Debug\net6.0\UMLEditor.NetCore``
### For CLI:
- To run the native executable in CLI mode: <br />
  - In Linux, MacOS, and Windows Powershell: ``./UMLEditor/UMLEditor.NetCore/bin/Debug/net6.0/UMLEditor.NetCore -cli``
  - In Windows CMD: ``.\UMLEditor\UMLEditor.NetCore\bin\Debug\net6.0\UMLEditor.NetCore -cli``


## Testing Instructions
### Build And Run Unit Tests:
_This will run all available unit tests for the code and does nothing else._
- To build and execute unit tests, run the following from the root of the repo directory (where the .sln is): <br />``dotnet test``

## Writeups
- SQLite Writeup.pdf is where the writeup for our dropped SQLite task is located.
- Design Pattern Writeup.pdf is where the list of implemented design patterns is located.

## GUI Mode Help
- Menu Bar
  - File
    - Save: Opens dialog to specify a file to save the current diagram to.
    - Load: Opens dialog to specify a file to load a diagram from.
  - Class
    - Add: Opens dialog to add a class to the diagram.
  - Relationship:
    - Add: Opens dialog to add a relationship to the diagram.
    - Change: Opens dialog to change the relationship type for an existing relationship.
    - Delete: Opens dialog to delete a relationship from the diagram.
    - Redraw Lines: Redraws all relationship lines between classes.
  - Utility:
    - Help: Displays link to this document.
    - Exit: Exits program without saving diagram.
- Canvas
  - Class Box
    - Class Name: Shows the name of this class.
      - ![Edit Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PencilButton.png?raw=true "Edit Button") Edit Class: Opens dialog to rename this class.
      - ![Delete Button](/UMLEditor/UMLEditor/Assets/CustomIcons/TrashCanButton.png?raw=true "Delete Button") Delete Class: Opens dialog to delete this class from diagram.
    - Fields: Shows the various fields of the class.
      - ![Add Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PlusButton.png?raw=true "Add Button") Add Field: Opens dialog to add a field to the class.
      - ![Edit Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PencilButton.png?raw=true "Edit Button") Edit Field: Opens dialog to change type and/or name of the field.
      - ![Delete Button](/UMLEditor/UMLEditor/Assets/CustomIcons/TrashCanButton.png?raw=true "Delete Button") Delete Field: Opens dialog to delete this field from the class.
    - Methods: Shows the various methods of the class.
      - ![Add Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PlusButton.png?raw=true "Add Button") Add Method: Opens dialog to add a method to the class.
      - ![Edit Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PencilButton.png?raw=true "Edit Button") Edit Method: Opens dialog to change type and/or name of the field.
      - ![Delete Button](/UMLEditor/UMLEditor/Assets/CustomIcons/TrashCanButton.png?raw=true "Delete Button") Delete Method: Opens dialog to delete the method from the class.
      - Parameters: Shows the various parameters of a method.
        - ![Add Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PlusButton.png?raw=true "Add Button") Add Parameter: Opens dialog to add a parameter to a method.
        - ![Edit Button](/UMLEditor/UMLEditor/Assets/CustomIcons/PencilButton.png?raw=true "Edit Button") Edit Parameter: Opens dialog to change the type and/or name of a parameter.
        - ![Delete Button](/UMLEditor/UMLEditor/Assets/CustomIcons/TrashCanButton.png?raw=true "Delete Button") Delete Parameter: Opens dialog to delete the parameter from the method.

### Team Members

Daniel Foreacre<br />
Gavin Fry<br />
Matthew Young<br />
Robert Bunning<br />
CJ Sydorko
