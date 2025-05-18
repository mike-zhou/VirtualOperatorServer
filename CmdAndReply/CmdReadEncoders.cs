using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace VirtualOperatorServer.CommandAndReply
{

class CmdReadEncoders : CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.READ_ENCODERS;

        return cmd;
    }

    public CmdReadEncoders() : base(createCommand()) { }

    public override string ParseReply()
    {
        if (reply == null)
        {
            return "No reply was received";
        }
        else
        {
            if (reply.Length != 17)
            {
                return "Invalid rely";
            }
            if (reply.Length == 0)
            {
                return "Invalid reply";
            }
            var cmd = Command;
            if (cmd[0] != reply[0])
            {
                return "Wrong reply";
            }

            return ParseEncoders();
        }
    }

    private string ParseEncoders()
    {
        ushort lptim1Counter;
        ushort lptim2Counter;
        ushort htim1Counter;
        ushort htim2Counter;
        ushort htim3Counter;
        ushort htim4Counter;    
        ushort htim5Counter;
        ushort htim8Counter;

        lptim1Counter = reply[2];
        lptim1Counter <<= 8;
        lptim1Counter += reply[1];

        lptim2Counter = reply[4];
        lptim2Counter <<= 8;
        lptim2Counter += reply[3];

        htim1Counter = reply[6];
        htim1Counter <<= 8;
        htim1Counter += reply[5];

        htim2Counter = reply[8];
        htim2Counter <<= 8;
        htim2Counter += reply[7];

        htim3Counter = reply[10];
        htim3Counter <<= 8;
        htim3Counter += reply[9];

        htim4Counter = reply[12];
        htim4Counter <<= 8;
        htim4Counter += reply[11];

        htim5Counter = reply[14];
        htim5Counter <<= 8;
        htim5Counter += reply[13];

        htim8Counter = reply[16];
        htim8Counter <<= 8;
        htim8Counter += reply[15];

        StringBuilder jsonBuilder = new StringBuilder();
        
        jsonBuilder.Append('{');
        jsonBuilder.Append($"\"lptim1Counter\":{lptim1Counter},");
        jsonBuilder.Append($"\"lptim2Counter\":{lptim2Counter},");
        jsonBuilder.Append($"\"htim1Counter\":{htim1Counter},");
        jsonBuilder.Append($"\"htim2Counter\":{htim2Counter},");
        jsonBuilder.Append($"\"htim3Counter\":{htim3Counter},");
        jsonBuilder.Append($"\"htim4Counter\":{htim4Counter},");
        jsonBuilder.Append($"\"htim5Counter\":{htim5Counter},");
        jsonBuilder.Append($"\"htim8Counter\":{htim8Counter}");
        jsonBuilder.Append('}');

        return jsonBuilder.ToString();
    }
}

}

