
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
        GET_STATUS
    }

    class CommandAndReply
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
