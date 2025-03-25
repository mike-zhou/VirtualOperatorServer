namespace VirtualOperatorServer.CommandAndReply
{


class CmdGetVersion: CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[2];
        cmd[0] = (byte)CommandEnum.GET_VERSION;
        cmd[1] = CommandSequenceNumber.GetSequenceNumber();

        return cmd;
    }

    public CmdGetVersion() : base(createCommand()) { }

    public override string ParseReply()
    {
        throw new NotImplementedException();
    }
}








}