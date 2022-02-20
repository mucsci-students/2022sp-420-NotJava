namespace UMLEditor.Exceptions;

using System;

public class ClassNonexistentException : Exception
{

    public ClassNonexistentException(string message) : base(message)
    {  }

}

public class ClassInUseException : Exception
{
    public ClassInUseException(string message) : base(message)
    {  }
}

public class AttributeNonexistentException : Exception
{

    public AttributeNonexistentException(string message) : base(message)
    {  }

}

public class NameTypeObjectAlreadyExistsException : Exception
{
    public NameTypeObjectAlreadyExistsException(string message) : base(message)
    {  }
}

public class ClassAlreadyExistsException : Exception
{
    public ClassAlreadyExistsException(string message) : base(message)
    { }
}

public class AttributeAlreadyExistsException : Exception
{
    public AttributeAlreadyExistsException(string message) : base(message)
    { }
}

public class RelationshipNonexistentException : Exception
{
    public RelationshipNonexistentException(string message) : base(message)
    {  }
}

public class InvalidRelationshipTypeException : Exception
{
    public InvalidRelationshipTypeException(string message) : base(message)
    {  }
}

public class InvalidNameException : Exception
{
    public InvalidNameException(string message) : base(message)
    { }
}
