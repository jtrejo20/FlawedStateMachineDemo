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

            // Store flag in varable if needed
            byte flag = input[3];


            checked
            {
                // check for reset command, which is valid in any state,
                // execute warm reset if flag is set to 0x01, otherwise do nothing
                if (command == OpCode.Reset)
                {
                    // Reset frame buffer
                    // Reset thermal tick counter

                    // Causes state-desynchronization if flag is set to 0x01
                    if (flag != 0x01)
                    {
                        _state = ControllerState.Standby;
                    }

                }
                switch (_state)
                {
                    case ControllerState.Standby:
                        if (command == OpCode.StartWarmingUp)
                        {
                            _state = ControllerState.WarmingUp;
                        }
                        break;
                    case ControllerState.WarmingUp:
                        if(command == OpCode.SetInterlock)
                        {
                            _interlockClosed = (argument > 0);
                        }
                        if(command == OpCode.TickThermal)
                        {
                            // Use variable to accumulate ticks
                            
                            // if # of ticks >= 3 && interlock is closed, set state to calibrating

                        }
                        break;

                    case ControllerState.Calibrating:
                        if(command == OpCode.SetInterlock && argument == 0)
                        {
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
                            // Interlock must be closed to start acquisition, otherwise crash the state machine
                            if (!_interlockClosed)
                            {
                                _state = ControllerState.Fault;
                                throw new InvalidOperationException("Crash state!");
                            }
                            // Calculate framerate based on exposure time 
                            // Allocate memory for frame buffer
                            _state = ControllerState.Acquiring;
                        }
                        break;

                    case ControllerState.Acquiring:
                        if(command == OpCode.ReadFrame)
                        {
                            // Calculate pixel index based on argument and buffer size
                            // Read pixel value from buffer and return to caller
                        }
                        else if (command == OpCode.ArmLaser)
                        {
                            // Free buffer memory
                            _state = ControllerState.Armed;
                        } 
                        break;

                    case ControllerState.Fault:
                        // Locked until reset command is received
                        break;
                }

            }

        }
    }
}
