namespace FlawedStateMachineDemo.Models
{
    public class StateMachine
    {
        // commands are 4 bytes long
        byte[] _command;
        public StateMachine(StateMachineCommand stateMachineCommand)
        {
            _command = stateMachineCommand.Data;
        }

        public void Execute()
        {

        }
    }
}
