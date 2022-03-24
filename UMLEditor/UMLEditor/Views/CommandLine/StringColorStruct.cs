using System;


namespace UMLEditor.Views.CommandLine;

public struct StringColorStruct
{
    public string output { get; private set; }
    public ConsoleColor inColor { get; private set; }

    public StringColorStruct(string str, ConsoleColor col = ConsoleColor.White)
    {
        output = str;
        inColor = col;
    }

}