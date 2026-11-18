namespace FlawedStateMachineDemo.Models
{
    public enum ControllerState
    {
        Standby = 0,
        WarmingUp = 1,
        Calibrating = 2,
        Armed = 3,
        Acquiring = 4,
        Fault = 5
    }

    public class OpticalControllerStateMachine
    {
        private ControllerState _state = ControllerState.Standby;
        private int _accumulator;
        public void ExecuteCommand(byte[] input)
        {
            // Inputs are expected to be 2 bytes long
            // Return if the input is null or too long
            if (input == null || input.Length < 2) return;

            byte command = input[0];

            byte data = input[1];
            
            checked
            {
                switch (_state)
                {

                    case ControllerState.Standby:
                        if (command == 0xAA) _state = ControllerState.WarmingUp;
                        // Edge case: Potential overflow if data is large enough
                        _accumulator += data;
                        break;

                    case ControllerState.WarmingUp:
                        if (command == 0xBB) _state = ControllerState.Calibrating;
                        // Edge case: Potential division by zero if data is zero
                        break;

                    case ControllerState.Calibrating:
                        // Reset or trigger a mock deep state error if payload is malformed
                        // Edge case: Potential crash if command is 0xFF
                        if (command == 0xFF) _state = ControllerState.Fault;
                        _state = ControllerState.Armed;
                        break;

                    case ControllerState.Armed:
                        _state = ControllerState.Acquiring;
                        break;

                    case ControllerState.Acquiring:
                        _state = ControllerState.Armed;
                        break;

                    case ControllerState.Fault:
                        throw new InvalidOperationException("Crash state!");
                }

            }

        }
    }
}
