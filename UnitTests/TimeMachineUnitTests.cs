using System;
using NUnit.Framework;
using UMLEditor.Classes;

namespace UnitTests;

[TestFixture]
public class TimeMachineUnitTests
{
    private readonly Diagram _d1 = new(), _d2 = new(), _d3 = new();
    [SetUp] 
    public void Setup()
    { }

    [Test]
    public void AddStateTest()
    {
        TimeMachine.AddState(_d1);
        TimeMachine.AddState(_d2);
        Assert.AreEqual(true, TimeMachine.PreviousStateExists());
        Assert.AreEqual(false, TimeMachine.NextStateExists());
    }
    
    [Test]
    public void ClearTest()
    {
        Diagram d1 = new Diagram(); 
        TimeMachine.AddState(d1);
        TimeMachine.ClearTimeMachine();
        Assert.AreEqual(false, TimeMachine.PreviousStateExists());
        Assert.AreEqual(false, TimeMachine.NextStateExists());
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
        Assert.AreEqual(true, TimeMachine.MoveToPreviousState().ClassExists("Diagram1Class"));
        // Try to go back another, but at beginning of list
        Assert.Throws<InvalidOperationException>(delegate { TimeMachine.MoveToPreviousState(); });
        // Create new future
        TimeMachine.AddState(_d3);
        Assert.AreEqual(true, TimeMachine.MoveToPreviousState().ClassExists("Diagram1Class"));
        Assert.AreEqual(true,TimeMachine.MoveToNextState().ClassExists("Diagram3Class"));
        Assert.Throws<InvalidOperationException>(delegate { TimeMachine.MoveToNextState(); });
    }
}