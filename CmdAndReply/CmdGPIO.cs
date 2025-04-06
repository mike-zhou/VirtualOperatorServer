using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VirtualOperatorServer.CommandAndReply
{

class CmdGetGPIOMode: CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.GET_GPIO_MODE;

        return cmd;
    }
    public CmdGetGPIOMode(): base(createCommand()) {}

    public override string ParseReply()
    {
        if(reply == null)
        {
            return "No reply was received";
        }
        else
        {
            if(reply.Length != 23)
            {
                return "Invalid rely";
            }
            if(reply.Length == 0)
            {
                return "Invalid reply";
            }
            var cmd = Command;
            if(cmd[0] != reply[0])
            {
                return "Wrong reply";
            }

            GpioMode = reply;
            return "Succeed";
        }
    }

    static public byte[]? GpioMode { get; private set; }
}

class CmdReadGPIO: CommandAndReply
{
    static private byte[] createCommand()
    {
        byte[] cmd = new byte[1];
        cmd[0] = (byte)CommandEnum.READ_GPIO;

        return cmd;
    }

    public CmdReadGPIO(): base(createCommand()) {}

    public override string ParseReply()
    {
        if(reply == null)
        {
            return "No reply was received";
        }
        else
        {
            if(reply.Length != 23)
            {
                return "Invalid rely";
            }
            if(reply.Length == 0)
            {
                return "Invalid reply";
            }
            var cmd = Command;
            if(cmd[0] != reply[0])
            {
                return "Wrong reply";
            }

            return ParseGpioRead();
        }
    }

    private string ParseGpioRead()
    {
        var gpioMode = CmdGetGPIOMode.GpioMode;
        if((gpioMode == null) || (reply == null))
        {
            return "";
        }

        StringBuilder htmlBuilder = new StringBuilder();
        List<string> portNameList = new List<string>{
            "PA",
            "PB",
            "PC",
            "PD",
            "PE",
            "PF",
            "PG",
            "PH",
            "PI",
            "PJ",
            "PK"
        };
        Debug.Assert(portNameList.Count == 11);

        htmlBuilder.Append("<table>");
        for(int portIndex = 0; portIndex < portNameList.Count; portIndex++)
        {
            var portName = portNameList[portIndex];
            ushort value = reply[1 + portIndex * 2 + 1];
            value <<= 8;
            value += reply[1 + portIndex * 2];
            
            ushort mode = gpioMode[1 + portIndex * 2 + 1];
            mode <<=8;
            mode += gpioMode[1 + portIndex * 2];

            htmlBuilder.Append("<tr>");
            for(int bitIndex = 0; bitIndex < 16; bitIndex++)
            {
                string gpioId = $"id_setGpio_{portName}_{bitIndex}";
                bool isInput = ((mode & 0x1) == 0);

                htmlBuilder.Append("<td>");
                if(isInput)
                {
                    htmlBuilder.Append($"<label style=\"color: gray; cursor: not-allowed;\" for=\"{gpioId}\">  {portName}{bitIndex}: </lable>");
                }
                else
                {
                    htmlBuilder.Append($"<label for=\"{gpioId}\">  {portName}{bitIndex}: </lable>");
                }

                if((value & 0x1) == 1)
                {
                    // high level
                    if(isInput)
                    {
                        htmlBuilder.Append($"<input type=\"checkbox\" id=\"{gpioId}\" checked disabled>");
                    }
                    else
                    {
                        htmlBuilder.Append($"<input type=\"checkbox\" id=\"{gpioId}\" checked>");
                    }
                }
                else
                {
                    // low level
                    if(isInput)
                    {
                        htmlBuilder.Append($"<input type=\"checkbox\" id=\"{gpioId}\" disabled>");
                    }
                    else
                    {
                        htmlBuilder.Append($"<input type=\"checkbox\" id=\"{gpioId}\">");
                    }
                }
                htmlBuilder.Append("</td>");

                value >>= 1;
                mode >>= 1;
            }
            htmlBuilder.Append("</tr>");
        }
        htmlBuilder.Append("</table>");

        return htmlBuilder.ToString();
    }
}


}