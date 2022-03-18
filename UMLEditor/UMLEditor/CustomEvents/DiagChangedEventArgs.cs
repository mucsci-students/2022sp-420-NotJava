using System;
using UMLEditor.Classes;

namespace UMLEditor.CustomEvents;

/// <summary>
/// Event args for when diagram is changed
/// </summary>
public class DiagChangedEventArgs : EventArgs
{
    /// <summary>
    /// Previous state of diagram before change
    /// </summary>
    public Diagram PrevState { get; private set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="diagram">Previous state of diagram before change</param>
    public DiagChangedEventArgs(Diagram diagram)
    {
        PrevState = diagram;
    }
}