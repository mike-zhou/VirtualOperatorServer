using System.Text.Json;

namespace VirtualOperatorServer.CommandAndReply
{

class CmdEcho: CommandAndReply
{
    static private byte[] createCommand(JsonElement jsonRoot)
    {
        var data = jsonRoot.GetProperty("data");
        var bytes = data.EnumerateArray()
                        .Select(e => e.GetByte())
                        .ToArray();

        if(bytes == null)
        {
            throw new InvalidRequestBodyException("No data of Uint8Array is included in Echo command");
        }

        byte[] cmd = new byte[1 + bytes.Length];
        cmd[0] = (byte)CommandEnum.ECHO;
        for(int i = 0; i < bytes.Length; i++) 
        {
            cmd[1 + i] = bytes[i];
        }

        return cmd;
    }

    public CmdEcho(JsonElement jsonRoot) : base(createCommand(jsonRoot)) { }

    public override string ParseReply()
    {
        if(reply == null)
        {
            return "No echo is received";
        }
        if(reply.Length < 1)
        {
            return "Invalid reply";
        }

        var cmd = Command;
        if(cmd[0] != reply[0])
        {
            return $"Not ECHO reply: {reply[0]}";
        }

        if(cmd.Length != reply.Length)
        {
            return $"Echo cmd length({cmd.Length}) != reply length({reply.Length})";
        }

        for(int i = 0; i < reply.Length; i++)
        {
            if(cmd[i] != reply[i])
            {
                return $"Wrong data at {i}";
            }
        }

        return "success";
    }
}


}
