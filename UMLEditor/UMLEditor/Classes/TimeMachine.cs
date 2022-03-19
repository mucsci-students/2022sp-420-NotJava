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
    /// References to the beginning, end, and current state of the time machine
    /// </summary>
    public static Node? Head, Tail, Current;
    /// <summary>
    /// Counter for number of states in the time machine
    /// </summary>
    public static int? Size;

    /// <summary>
    /// Initializes the time machine to an empty list
    /// </summary>
    public static void Initialize()
    {
        Head = null;
        Tail = Head;
        Current = Tail;
        Size = 0;
    }

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
    public static Diagram? NextState()
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
    public static Diagram? PreviousState()
    {
        if (Current != Head)
        {
            Current = Current?.PrevNode;
            return Current?.StateDiagram;
        }

        return null;
    }

    /// <summary>
    /// Clears entire history. Dereferences all nodes.
    /// </summary>
    public static void ClearTimeMachine()
    {
        Current = Head;
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