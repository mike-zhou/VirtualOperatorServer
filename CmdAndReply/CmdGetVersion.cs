namespace VirtualOperatorServer.CommandAndReply
{


class CmdGetVersion: CommandAndReply
{
    static private byte[] CreateCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.GET_VERSION;

        return cmd;
    }

    public static string? Version { get; private set; } = null;

    public CmdGetVersion() : base(CreateCommand()) { }

    public override (bool result, string reason) ParseReply()
    {
        if(reply == null)
        {
            return (false, "No version is received");
        }
        if(reply.Length < 1)
        {
            return (false, "Invalid reply");
        }

        var cmd = Command;
        if(cmd[0] != reply[0])
        {
            return (false, "Wrong reply");
        }

        var versionLength = reply.Length - 1;
        if(versionLength == 0)
        {
            return (false, "Empty version");
        }

        byte[] versionBytes = new byte[versionLength];
        Array.Copy(reply, 1, versionBytes, 0, versionLength);

        Version = System.Text.Encoding.UTF8.GetString(versionBytes);

        return (true, "");
    }
}








}