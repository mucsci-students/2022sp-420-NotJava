using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class TimeMachineUnitTests
{
    private readonly Diagram? _d1 = new(), _d2 = new(), _d3 = new();
    [SetUp] 
    public void Setup()
    { }

    [Test]
    public void ClearTest()
    {
        Diagram d1 = new Diagram();
        TimeMachine.AddState(d1);
        TimeMachine.ClearTimeMachine();
        Assert.AreEqual(null, TimeMachine.Head);
        Assert.AreEqual(null, TimeMachine.Current);
        Assert.AreEqual(null, TimeMachine.Tail);
        Assert.AreEqual(0, TimeMachine.Size);
    }
    
    [Test]
    public void AddStateTest()
    {
        TimeMachine.ClearTimeMachine();
        TimeMachine.AddState(_d1);
        Assert.AreEqual(1, TimeMachine.Size);
        Assert.AreEqual(TimeMachine.Current, TimeMachine.Head);
        Assert.AreEqual(TimeMachine.Current, TimeMachine.Tail);
        TimeMachine.AddState(_d2);
        Assert.AreEqual(2,TimeMachine.Size);
        Assert.AreEqual(TimeMachine.Head, TimeMachine.Current.PrevNode);
        Assert.AreEqual(TimeMachine.Tail, TimeMachine.Current);
        Assert.AreEqual(null, TimeMachine.Current.NextNode);
    }

    [Test]
    public void IncrementTest()
    {
        TimeMachine.ClearTimeMachine();
        _d1.AddClass("Diagram1Class");
        _d2.AddClass("Diagram2Class");
        _d3.AddClass("Diagram3Class");
        TimeMachine.AddState(_d1);
        TimeMachine.AddState(_d2);
        // Go back one step
        Assert.AreEqual(TimeMachine.Head.StateDiagram, TimeMachine.MoveToPreviousState());
        // Try to go back another, but at beginning of list
        Assert.AreEqual(null, TimeMachine.MoveToPreviousState());
        // Create new future
        TimeMachine.AddState(_d3);
        Assert.AreEqual("Diagram1Class", TimeMachine.MoveToPreviousState().GetClassByName("Diagram1Class").ClassName);
        Assert.AreEqual("Diagram3Class",TimeMachine.MoveToNextState().GetClassByName("Diagram3Class").ClassName);
        Assert.AreEqual(null, TimeMachine.MoveToNextState());
    }

    [Test]
    public void StateCheckTest()
    {
        TimeMachine.ClearTimeMachine();
        Assert.AreEqual(false,TimeMachine.NextStateExists());
        Assert.AreEqual(false,TimeMachine.PreviousStateExists());
        TimeMachine.AddState(_d1);
        TimeMachine.AddState(_d2);
        TimeMachine.AddState(_d3);
        TimeMachine.MoveToPreviousState();
        Assert.AreEqual(true, TimeMachine.NextStateExists());
        Assert.AreEqual(true,TimeMachine.PreviousStateExists());
    }
}