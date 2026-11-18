using FlawedStateMachineDemo.Models;

var stateMachine = new OpticalControllerStateMachine();

var stateMachineData = new byte[] { 170, 0};

// Command sequence causes crash state

stateMachine.ExecuteCommand(stateMachineData);

stateMachineData = new byte[] { 187, 0 };

stateMachine.ExecuteCommand(stateMachineData);

stateMachineData = new byte[] { 255, 0 };

stateMachine.ExecuteCommand(stateMachineData);


