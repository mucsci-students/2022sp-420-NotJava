namespace UMLEditor.Exceptions;

using System;

/// <summary>
/// Exception when class does not exist
/// </summary>
public class ClassNonexistentException : Exception
{

    /// <summary>
    /// Class Does not exist
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public ClassNonexistentException(string message) : base(message)
    {  }

}

/// <summary>
/// Exception when class is in use
/// </summary>
public class ClassInUseException : Exception
{
    /// <summary>
    /// Class already in use
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public ClassInUseException(string message) : base(message)
    {  }
}

/// <summary>
/// Exception when attribute does not exist
/// </summary>
public class AttributeNonexistentException : Exception
{

    /// <summary>
    /// Attribute does not exist
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public AttributeNonexistentException(string message) : base(message)
    {  }

}

/// <summary>
/// Exception when class already exists
/// </summary>
public class ClassAlreadyExistsException : Exception
{
    /// <summary>
    /// Class already exists
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public ClassAlreadyExistsException(string message) : base(message)
    { }
}

/// <summary>
/// Exception when attribute already exists
/// </summary>
public class AttributeAlreadyExistsException : Exception
{
    /// <summary>
    /// Attribute already exists
    /// </summary>
    /// <param name="message"></param>
    public AttributeAlreadyExistsException(string message) : base(message)
    { }
}

/// <summary>
/// Exception when relationship does not exist
/// </summary>
public class RelationshipNonexistentException : Exception
{
    /// <summary>
    /// Relationship does not exist
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public RelationshipNonexistentException(string message) : base(message)
    {  }
}

/// <summary>
/// Exception when relationship already exists
/// </summary>
public class RelationshipAlreadyExistsException : Exception
{
    /// <summary>
    /// Relationship already exists
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public RelationshipAlreadyExistsException(string message) : base(message)
    {  }
}

/// <summary>
/// Exception when a relationship type already exists
/// </summary>
public class RelationshipTypeAlreadyExists : Exception
{
    /// <summary>
    /// Relationship type already exists
    /// </summary>
    /// <param name="message">Message sent by the exception</param>
    public RelationshipTypeAlreadyExists(string message) : base(message)
    {  }
}

/// <summary>
/// Exception when relationship type is invalid
/// </summary>
public class InvalidRelationshipTypeException : Exception
{
    /// <summary>
    /// Invalid relationship type
    /// </summary>
    /// <param name="message">Message sent by the exception</param>
    public InvalidRelationshipTypeException(string message) : base(message)
    {  }
}

/// <summary>
/// Exception when name is invalid
/// </summary>
public class InvalidNameException : Exception
{
    /// <summary>
    /// Name is invalid
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public InvalidNameException(string message) : base(message)
    { }
}

/// <summary>
/// Exception when method already exists
/// </summary>
public class MethodAlreadyExistsException : Exception
{
    /// <summary>
    /// Method already exists
    /// </summary>
    /// <param name="message">Message to be sent by exception</param>
    public MethodAlreadyExistsException(string message) : base(message)
    { }
}

/// <summary>
/// Exception when method does not exist
/// </summary>
public class MethodNonexistentRelationship : Exception
{
    /// <summary>
    /// Method does not exist
    /// </summary>
    /// <param name="message">Message to be sent by the exception</param>
    public MethodNonexistentRelationship(string message) : base(message)
    { }
}
