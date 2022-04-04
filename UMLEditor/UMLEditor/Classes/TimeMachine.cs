using System;
using System.Collections.Generic;

namespace UMLEditor.Classes;

/// <summary>
/// Class to represent the action history of the UML program
/// </summary>
public static class TimeMachine
{
    private static List<Diagram> _diagramList = new ();

    private static int _head = -1, _current = -1, _tail = -1;

    /// <summary>
    /// Adds a new diagram to the end of the list. Erases redo history if diagram is added to middle of list.
    /// </summary>
    /// <param name="d">Diagram to add</param>
    public static void AddState(Diagram d)
    {
        if (_current != _tail)
        {
            _diagramList.RemoveRange(_current + 1, _tail - _current);
        }
        _current++;
        _diagramList.Add(new Diagram(d));
        _tail = _current;

        if (_diagramList.Count == 1)
        {
            _head = _current;
        }
    }

    /// <summary>
    /// Moves to next state in the time machine (redo)
    /// </summary>
    /// <exception cref="InvalidOperationException">If next state does not exist</exception>
    /// <returns>The next diagram</returns>
    public static Diagram MoveToNextState()
    {
        if (_current == _tail)
        {
            throw new InvalidOperationException("Next state does not exist");
        }
        _current++;
        return new Diagram(_diagramList[_current]);
    }

    /// <summary>
    /// Moves to previous state in the time machine (undo)
    /// </summary>
    /// <exception cref="InvalidOperationException">If previous state does not exist</exception>
    /// <returns>The previous diagram</returns>
    public static Diagram MoveToPreviousState()
    {
        if (_current == _head)
        {
            throw new InvalidOperationException("Previous state does not exist");
        }
        _current--;
        return new Diagram(_diagramList[_current]);
    }

    /// <summary>
    /// Checks if there is a next state to move to without moving to it
    /// </summary>
    /// <returns>True if a state exists, false otherwise</returns>
    public static bool NextStateExists()
    {
        return _current != _tail;
    }

    /// <summary>
    /// Checks if there is a previous state to move to without moving to it
    /// </summary>
    /// <returns>True if a state exists, false otherwise</returns>
    public static bool PreviousStateExists()
    {
        return _current != _head;
    }

    /// <summary>
    /// Clears entire history
    /// </summary>
    public static void ClearTimeMachine()
    {
        _diagramList.Clear();
        _head = -1;
        _current = -1;
        _tail = -1;
    }
}