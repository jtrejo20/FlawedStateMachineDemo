namespace FlawedStateMachineDemo.Models
{
    public class FlawedStateMachine
    {

        private int _state;
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

                    case 0:
                        if (command == 0xAA) _state = 1;
                        // Edge case: Potential overflow if data is large enough
                        _accumulator += data;
                        break;
                    case 1:
                        if (command == 0xBB) _state = 2;
                        // Edge case: Potential division by zero if data is zero
                        break;
                    case 2:
                        // Reset or trigger a mock deep state error if payload is malformed
                        // Edge case: Potential crash if command is 0xFF
                        if (command == 0xFF) throw new InvalidOperationException("Crash state!");
                        _state = 0;
                        break;
                    default:
                        _state = 0;
                        break;
                }

            }

        }
    }
}
