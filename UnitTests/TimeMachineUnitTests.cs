using NUnit.Framework;
using UMLEditor.Classes;
using UMLEditor.Exceptions;

namespace UnitTests;

[TestFixture]
public class TimeMachineUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void InitializeTimeMachineTest()
    {
        TimeMachine.Initialize();
        Assert.AreEqual(0, TimeMachine.Size);
        Assert.AreEqual(TimeMachine.Head,TimeMachine.Tail);
        Assert.AreEqual(TimeMachine.Current,TimeMachine.Tail);
        Assert.AreEqual(null, TimeMachine.Current);
    }

    [Test]
    public void AddStateTest()
    {
        TimeMachine.Initialize();
        Diagram d1 = new Diagram();
        TimeMachine.AddState(d1);
        Assert.AreEqual(1, TimeMachine.Size);
        Assert.AreEqual(TimeMachine.Current, TimeMachine.Head);
        Assert.AreEqual(TimeMachine.Current, TimeMachine.Tail);
        Diagram d2 = new Diagram();
        TimeMachine.AddState(d2);
        Assert.AreEqual(2,TimeMachine.Size);
        Assert.AreEqual(TimeMachine.Head, TimeMachine.Current.PrevNode);
        Assert.AreEqual(TimeMachine.Tail, TimeMachine.Current);
        Assert.AreEqual(null, TimeMachine.Current.NextNode);
    }

    [Test]
    public void IncrementTest()
    {
        TimeMachine.Initialize();
        Diagram d1 = new Diagram();
        d1.AddClass("Diagram1Class");
        Diagram d2 = new Diagram();
        d2.AddClass("Diagram2Class");
        TimeMachine.AddState(d1);
        TimeMachine.AddState(d2);
        // Go back one step
        Assert.AreEqual(TimeMachine.Head.StateDiagram, TimeMachine.PreviousState());
        // Try to go back another, but at beginning of list
        Assert.AreEqual(null, TimeMachine.PreviousState());
        Diagram d3 = new Diagram();
        d3.AddClass("Diagram3Class");
        // Create new future
        TimeMachine.AddState(d3);
        Assert.AreEqual("Diagram1Class", TimeMachine.PreviousState().GetClassByName("Diagram1Class").ClassName);
        Assert.AreEqual("Diagram3Class",TimeMachine.NextState().GetClassByName("Diagram3Class").ClassName);
        Assert.AreEqual(null, TimeMachine.NextState());
    }

    [Test]
    public void ClearTest()
    {
        TimeMachine.Initialize();
        Diagram d1 = new Diagram();
        TimeMachine.AddState(d1);
        TimeMachine.ClearTimeMachine();
        Assert.AreEqual(null, TimeMachine.Head);
        Assert.AreEqual(null, TimeMachine.Current);
        Assert.AreEqual(null, TimeMachine.Tail);
        Assert.AreEqual(0, TimeMachine.Size);
    }
}