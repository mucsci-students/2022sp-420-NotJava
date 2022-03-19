using System.Dynamic;

namespace UMLEditor.Classes;

/// <summary>
/// Class to represent the action history of the UML program
/// </summary>
public static class TimeMachine
{
    /// <summary>
    /// Node class to represent a single state in the time machine history.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Reference to previous node
        /// </summary>
        public Node? PrevNode;
        /// <summary>
        /// Diagram at this node
        /// </summary>
        public Diagram? StateDiagram;
        /// <summary>
        /// Reference to next node
        /// </summary>
        public Node? NextNode;
    }
    /// <summary>
    /// Reference to the beginning state of the time machine
    /// </summary>
    public static Node? Head { get; private set; }
    /// <summary>
    /// Reference to the end state of the time machine
    /// </summary>
    public static Node? Tail { get; private set; }
    /// <summary>
    /// Reference to the current state of the time machine
    /// </summary>
    public static Node? Current { get; private set; }
    /// <summary>
    /// Counter for number of states in the time machine
    /// </summary>
    public static int Size { get; private set; }

    /// <summary>
    /// Adds a new diagram to the end of the list. Erases redo history if diagram is added to middle of list.
    /// </summary>
    /// <param name="d">Diagram to add</param>
    public static void AddState(Diagram d)
    {
        // If in the middle of the list clears out all future nodes
        if (Current?.NextNode is not null)
        {
            Node? temp = Current.NextNode;
            while (temp is not null)
            {
                Node? next = temp.NextNode;
                temp.StateDiagram = null;
                temp.PrevNode = null;
                temp.NextNode = null;
                temp = next;
                Size--;
            }
        }
        
        // Add new node to end of list
        Node newNode = new Node();
        newNode.StateDiagram = new Diagram(d);
        newNode.PrevNode = Current;
        newNode.NextNode = null;
        if (Current is not null)
        {
            Current.NextNode = newNode;
        }
        Tail = newNode;
        Size++;
        Current = Tail;
        if (Size == 1)
        {
            Head = newNode;
        }
    }

    /// <summary>
    /// Moves to next state in the time machine (redo)
    /// </summary>
    /// <returns>The next diagram, or null if there are none</returns>
    public static Diagram? MoveToNextState()
    {
        if (Current != Tail)
        {
            Current = Current?.NextNode;
            return Current?.StateDiagram;
        }

        return null;
    }

    /// <summary>
    /// Moves to previous state in the time machine (undo)
    /// </summary>
    /// <returns>The previous diagram, or null if there are none</returns>
    public static Diagram? MoveToPreviousState()
    {
        if (Current != Head)
        {
            Current = Current?.PrevNode;
            return Current?.StateDiagram;
        }

        return null;
    }

    /// <summary>
    /// Checks if there is a next state to move to without moving to it
    /// </summary>
    /// <returns>True if a state exists, false otherwise</returns>
    public static bool NextStateExists()
    {
        return (Current is not null) && (Current.NextNode is not null);
    }

    /// <summary>
    /// Checks if there is a previous state to move to without moving to it
    /// </summary>
    /// <returns>True if a state exists, false otherwise</returns>
    public static bool PreviousStateExists()
    {
        return (Current is not null) && (Current.PrevNode is not null);
    }

    /// <summary>
    /// Clears entire history. Dereferences all nodes.
    /// </summary>
    public static void ClearTimeMachine()
    {
        Current = Head;
        // Clear out all nodes
        while (Current is not null)
        {
            Node? next = Current.NextNode;
            Current.StateDiagram = null;
            Current.PrevNode = null;
            Current.NextNode = null;
            Current = next;
        }
        Head = null;
        Tail = Head;
        Current = Tail;
        Size = 0;
    }
}