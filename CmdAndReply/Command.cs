
namespace VirtualOperatorServer.CommandAndReply
{

    class InvalidRequestBodyException: Exception
    {
        public InvalidRequestBodyException(string message) : base(message) {}
    }

    enum CommandEnum
    {
        GET_VERSION = 0,
        ECHO,
        GET_GPIO_MODE,
        READ_GPIO,
        SET_GPIO,
        READ_ENCODERS,
        GET_STATUS,
        SET_STEPPER_ACTIVE_RAMPUP_PULSE_WIDTH,
        SET_STEPPER_ACTIVE_CRUISE_PULSE_WIDTH,
        SET_STEPPER_ACTIVE_RAMPDOWN_PULSE_WIDTH,
        SET_STEPPER_PASSIVE_STEP_INDEXES,
        SET_STEPPER_CONTROLS,
        SET_STEPPER_ENABLE,
        SET_STEPPER_FORWARD,
        START_STEPPER_HOME_POSITIONING,
        RUN_STEPPER_FORCE,
        RUN_STEPPER_PASSIVE,
        RUN_STEPPER_ACTIVE,
        SET_TIMER_PRESCALER,
        SET_POSITION_DETECTORS,
        TEST_TIMER,
        TEST_STEPPER_ENABLE,
        TEST_STEPPER_FORWARD,
        TEST_STEPPER_CLOCK
    }

    public class CommandAndReply
    {
        private byte[] command;
        protected byte[]? reply;

        public CommandAndReply(byte[] command)
        {
            this.command = command;
        }

        public byte[] Command {get => command;}
        public byte[]? Reply
        {
            set => reply = value;
        }

        public virtual (bool result, string reason) ParseReply()
        {
            return (false, "not implemented");
        }
    }

} // end of name space
