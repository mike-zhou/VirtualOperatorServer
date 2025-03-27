namespace VirtualOperatorServer.CommandAndReply
{


class CmdGetVersion: CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.GET_VERSION;

        return cmd;
    }

    public CmdGetVersion() : base(createCommand()) { }

    public override string ParseReply()
    {
        if(reply == null)
        {
            return "No version is received";
        }
        else
        {
            if(reply.Length < 1)
            {
                return "Invalid reply";
            }

            var cmd = Command;
            if(cmd[0] != reply[0])
            {
                return "Wrong reply";
            }

            var versionLength = reply.Length - 1;
            if(versionLength == 0)
            {
                return "Empty version";
            }

            byte[] versionBytes = new byte[versionLength];
            Array.Copy(reply, 1, versionBytes, 0, versionLength);

            string version = System.Text.Encoding.UTF8.GetString(versionBytes);
            return version;
        }
    }
}








}