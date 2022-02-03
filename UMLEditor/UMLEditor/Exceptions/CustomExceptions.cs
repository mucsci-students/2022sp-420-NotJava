using UMLEditor.Classes;

namespace UMLEditor.Exceptions;

using System;

public class ClassNonexistentException : Exception
{

    public ClassNonexistentException(string message) : base(message)
    {  }

}