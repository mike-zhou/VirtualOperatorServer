

namespace VirtualOperatorServer.CommandAndReply
{

static class CommandSequenceNumber
{
    private static byte sequenceNumber = 0;

    public static byte GetSequenceNumber()
    {
        return sequenceNumber++;
    }
}

enum CommandEnum
{
    GET_VERSION = 0,
    GET_GPIO,
    SET_GPIO
}

class CommandAndReply
{
    private byte[] command;
    private byte[]? reply;

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
