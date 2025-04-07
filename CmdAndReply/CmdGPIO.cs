using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

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
        if(gpioMode == null)
        {
            return "No GPIO mode data";
        }
        if(reply == null)
        {
            return "No reply";
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

class CmdSetGPIO: CommandAndReply
{
    static private byte[] createCommand(JsonElement jsonRoot)
    {
        var portName = jsonRoot.GetProperty("portName").GetString();
        var bitIndex = jsonRoot.GetProperty("bitIndex").GetUInt16();
        var level = jsonRoot.GetProperty("level").GetUInt16();

        byte[] cmd = new byte[4];
        
        cmd[0] = (byte)CommandEnum.SET_GPIO;

        if (portName == "PA")
            cmd[1] = 0;
        else if (portName == "PB")
            cmd[1] = 1;
        else if (portName == "PC")
            cmd[1] = 2;
        else if (portName == "PD")
            cmd[1] = 3;
        else if (portName == "PE")
            cmd[1] = 4;
        else if (portName == "PF")
            cmd[1] = 5;
        else if (portName == "PG")
            cmd[1] = 6;
        else if (portName == "PH")
            cmd[1] = 7;
        else if (portName == "PI")
            cmd[1] = 8;
        else if (portName == "PJ")
            cmd[1] = 9;
        else if (portName == "PK")
            cmd[1] = 10;
        else 
            throw new InvalidRequestBodyException($"Invalid portName '{portName}' in SetGPIO command");
        
        if(bitIndex > 15)
        {
            throw new InvalidRequestBodyException($"Invalid bitIndex '{bitIndex}' in SetGPIO command");
        }
        cmd[2] = (byte)bitIndex;

        if ((level != 0) && (level != 1))
        {
            throw new InvalidRequestBodyException($"Invalid level '{level}' in SetGPIO command");
        }
        cmd[3] = (byte)level;

        return cmd;
    }

    public CmdSetGPIO(JsonElement jsonRoot) : base(createCommand(jsonRoot)) { }

    
    public override string ParseReply()
    {
        if(reply == null)
        {
            return "No reply is received";
        }
        if(reply.Length < 1)
        {
            return "Invalid reply";
        }

        var cmd = Command;
        if(cmd[0] != reply[0])
        {
            return $"Not SetGPIO reply: {reply[0]}";
        }
        if(reply.Length == 1)
        {
            return $"Failed to run SetGPIO";
        }
        if(reply.Length != 4)
        {
            return $"Wrong SetGPIO reply length: {reply.Length}";
        }        
        if(reply[3] != cmd[3])
        {
            return $"Failed to set GPIO to {cmd[3]}";
        }

        return "success";
    }
}

}