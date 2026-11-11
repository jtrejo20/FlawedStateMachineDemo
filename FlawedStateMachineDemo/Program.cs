using FlawedStateMachineDemo.Models;

var stateMachineData = new byte[] { 1, 2, 3, 4 };

var stateMachineCommand = new StateMachineCommand(stateMachineData);

var stateMachine = new StateMachine(stateMachineCommand);

stateMachine.Execute();

public class StateMachineCommand
{
    public StateMachineCommand(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}