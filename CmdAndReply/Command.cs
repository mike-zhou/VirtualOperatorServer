

namespace VirtualOperatorServer.CommandAndReply
{

public class InvalidRequestBodyException: Exception
{
    public InvalidRequestBodyException(string message) : base(message) {}
}

enum CommandEnum
{
    GET_VERSION = 0,
    ECHO,
    GET_GPIO,
    SET_GPIO
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

    public virtual string ParseReply()
    {
        return "Empty reply";
    }
}

} // end of name space
