namespace VirtualOperatorServer.CommandAndReply
{

class CmdGetGPIO: CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.GET_GPIO;

        return cmd;
    }

    public CmdGetGPIO(): base(createCommand()) {}

    public override string ParseReply()
    {
        throw new NotImplementedException();
    }
}


}