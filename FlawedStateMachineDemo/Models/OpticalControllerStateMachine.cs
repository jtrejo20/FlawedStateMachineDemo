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

    public enum OpCode : byte
    {
        StartWarmingUp = 0x10,
        SetInterlock = 0x11,
        TickThermal = 0x12,
        RunCalibration = 0x20,
        SetExposure = 0x21,
        ArmLaser = 0x30,
        StartAcquisition = 0x40,
        ReadFrame = 0x41,
        Reset = 0xFF
    }

    public class OpticalControllerStateMachine
    {
        private ControllerState _state = ControllerState.Standby;
        private int _accumulator;
        public void ExecuteCommand(byte[] input)
        {
            // Inputs are expected to be 4 bytes long
            // Ex. byte[] command
            // command[0] = opCode
            // command[1-2] = argument
            // command[3] = flag
            
            if (input == null || input.Length < 4) return;

            OpCode command = (OpCode)input[0];

            byte data = input[1];
            
            checked
            {
                switch (_state)
                {

                    case ControllerState.Standby:
                        if (command == OpCode.StartWarmingUp) _state = ControllerState.WarmingUp;
                        // Edge case: Potential overflow if data is large enough
                        _accumulator += data;
                        break;

                    case ControllerState.WarmingUp:
                        if (command == OpCode.RunCalibration) _state = ControllerState.Calibrating;
                        // Edge case: Potential division by zero if data is zero
                        break;

                    case ControllerState.Calibrating:
                        // Reset or trigger a mock deep state error if payload is malformed
                        // Edge case: Potential crash if command is 0xFF
                        if (command == OpCode.Reset) _state = ControllerState.Fault;
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
