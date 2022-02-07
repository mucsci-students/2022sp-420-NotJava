
namespace UMLEditor.Exceptions;

using System;

public class ClassNonexistentException : Exception
{

    public ClassNonexistentException(string message) : base(message)
    {  }

}

public class AttributeNonexistentException : Exception
{

    public AttributeNonexistentException(string message) : base(message)
    {  }

}

public class ClassAlreadyExistsException : Exception
{
    public ClassAlreadyExistsException(string message) : base(message)
    { }
}