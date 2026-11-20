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

        private bool _interlockClosed;
        public void ExecuteCommand(byte[] input)
        {
            // Inputs are expected to be 4 bytes long
            // Ex. byte[] command
            // command[0] = opCode
            // command[1-2] = argument
            // command[3] = flag
            
            if (input == null || input.Length < 4) return;

            OpCode command = (OpCode)input[0];

            ushort argument = (ushort)((input[1] << 8) | input[2]);
            
            checked
            {
                switch (_state)
                {

                    case ControllerState.Standby:
                        if (command == OpCode.StartWarmingUp)
                        {
                            _state = ControllerState.WarmingUp;
                        }
                        // Edge case: Potential overflow if data is large enough
                        break;

                    case ControllerState.WarmingUp:
                        if(command == OpCode.SetInterlock)
                        {
                            // Set interlock closed when argument > 0
                            _interlockClosed = (argument > 0);
                        }
                        if(command == OpCode.TickThermal)
                        {
                            // Use variable to accumulate ticks
                            
                            // if # of ticks >= 3 && interlock is closed, set state to calibrating

                        }

                        // Edge case: Potential division by zero if data is zero
                        break;

                    case ControllerState.Calibrating:
                        if(command == OpCode.SetInterlock && argument == 0)
                        {
                            // Interlock open
                            _interlockClosed = false;
                            _state = ControllerState.WarmingUp;
                        }
                        if (command == OpCode.RunCalibration)
                        {
                            // Store calibration offset
                            _state = ControllerState.Armed;
                        }
                        break;

                    case ControllerState.Armed:
                        if (command == OpCode.SetExposure)
                        {
                            // Set exposure
                        }
                        else if (command == OpCode.StartAcquisition)
                        {
                            if (!_interlockClosed)
                            {
                                _state = ControllerState.Fault;
                                throw new InvalidOperationException("Crash state!");
                            }
                            _state = ControllerState.Acquiring;

                        }
                        break;

                    case ControllerState.Acquiring:
                        if(command == OpCode.ReadFrame)
                        {

                        }
                        else if (command == OpCode.ArmLaser)
                        {
                            _state = ControllerState.Armed;

                        }
                        break;

                    case ControllerState.Fault:
                        break;
                }

            }

        }
    }
}
