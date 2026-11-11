using FlawedStateMachineDemo.Models;

Console.WriteLine("Hello, World!");

var stateMachineData = new byte[] { 1, 2, 3, 4 };

var stateMachineCommand = new StateMachineCommand(stateMachineData);

var stateMachine = new StateMachine(stateMachineCommand);

stateMachine.Execute();

// Error expected

public class StateMachineCommand
{
    public StateMachineCommand(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}