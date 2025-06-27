using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace VirtualOperatorServer.CommandAndReply
{

    class CmdGetGPIOMode : CommandAndReply
    {
        public static ushort[]? GpioModes { get; private set; } = null;
        
        static private byte[] CreateCommand()
        {
            byte[] cmd = new byte[1];
            cmd[0] = (byte)CommandEnum.GET_GPIO_MODE;

            return cmd;
        }
        public CmdGetGPIOMode() : base(CreateCommand()) { }

        public override (bool result, string reason) ParseReply()
        {
            if (reply == null)
            {
                return (false, "No reply was received");
            }
            if (reply.Length != 23)
            {
                return (false, "Invalid rely");
            }
            if (reply.Length == 0)
            {
                return (false, "Invalid reply)");
            }
            var cmd = Command;
            if (cmd[0] != reply[0])
            {
                return (false, "Wrong reply");
            }

            ushort value;
            int i;

            var modes = new ushort[11];

            i = 1;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[0] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[1] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[2] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[3] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[4] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[5] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[6] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[7] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[8] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[9] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            modes[10] = value;

            GpioModes = modes;

            return (true, "");
        }
    }

    class CmdReadGPIO: CommandAndReply
    {
        static private byte[] CreateCommand()
        {
            byte[] cmd = new byte[1];
            cmd[0] = (byte)CommandEnum.READ_GPIO;

            return cmd;
        }

        public CmdReadGPIO(): base(CreateCommand()) {}

        public ushort[]? GpioValues { get; private set; } = null;

        public override (bool result, string reason) ParseReply()
        {
            if (reply == null)
            {
                return (false, "No reply was received");
            }
            if (reply.Length != 23)
            {
                return (false, "Invalid rely");
            }
            if (reply.Length == 0)
            {
                return (false, "Invalid reply");
            }
            var cmd = Command;
            if (cmd[0] != reply[0])
            {
                return (false, "Wrong reply");
            }

            ushort value;
            int i;

            var values = new ushort[11];

            i = 1;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[0] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[1] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[2] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[3] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[4] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[5] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[6] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[7] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[8] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[9] = value;

            i += 2;
            value = reply[i + 1];
            value <<= 8;
            value += reply[i];
            values[10] = value;

            GpioValues = values;

            return (true, "");
        }
    }

    class CmdSetGPIO: CommandAndReply
    {
        public class GPIO
        {
            public string? PortName {get; set;}
            public byte BitIndex {get; set;}
            public byte Level {get; set;}
        }

        static private byte[] CreateCommand(JsonElement jsonRoot)
        {
            /*
                jsonRoot:
                [
                    {
                        "portName":"PA",
                        "bitIndex":0,
                        "level":0
                    },
                    {
                        "portName":"PE",
                        "bitIndex":15,
                        "level":1
                    },
                    ...
                ]
            */

            var rawText = "";
            GPIO[]? gpioArray;
            try
            {
                rawText = jsonRoot.GetRawText();
                gpioArray = JsonSerializer.Deserialize<GPIO[]>(rawText);
            }
            catch (Exception)
            {
                throw new InvalidRequestBodyException($"Exception in deserializing: '{rawText}'");
            }

            if(gpioArray == null)
            {
                throw new InvalidRequestBodyException($"Not an JSON array in SetGPIO command");
            }

            return createCommand(gpioArray);
        }

        static private byte[] createCommand(GPIO[] gpioArray)
        {
            var portCount = gpioArray.Length;
            if(portCount > 32)
            {
                throw new InvalidRequestBodyException($"Too many GPIOs ({portCount}) in SetGPIO command");
            }

            byte[] cmd = new byte[1 + portCount * 3];
            cmd[0] = (byte)CommandEnum.SET_GPIO;

            for(int i = 0; i < portCount; i++)
            {
                var portName = gpioArray[i].PortName;
                var bitIndex = gpioArray[i].BitIndex;
                var level = gpioArray[i].Level;


                if (portName == "PA")
                    cmd[1 + i * 3] = 0;
                else if (portName == "PB")
                    cmd[1 + i * 3] = 1;
                else if (portName == "PC")
                    cmd[1 + i * 3] = 2;
                else if (portName == "PD")
                    cmd[1 + i * 3] = 3;
                else if (portName == "PE")
                    cmd[1 + i * 3] = 4;
                else if (portName == "PF")
                    cmd[1 + i * 3] = 5;
                else if (portName == "PG")
                    cmd[1 + i * 3] = 6;
                else if (portName == "PH")
                    cmd[1 + i * 3] = 7;
                else if (portName == "PI")
                    cmd[1 + i * 3] = 8;
                else if (portName == "PJ")
                    cmd[1 + i * 3] = 9;
                else if (portName == "PK")
                    cmd[1 + i * 3] = 10;
                else 
                    throw new InvalidRequestBodyException($"Invalid portName '{portName}' in SetGPIO command");
                
                if(bitIndex > 15)
                {
                    throw new InvalidRequestBodyException($"Invalid bitIndex '{bitIndex}' in SetGPIO command");
                }
                cmd[2 + i * 3] = (byte)bitIndex;

                if ((level != 0) && (level != 1))
                {
                    throw new InvalidRequestBodyException($"Invalid level '{level}' in SetGPIO command");
                }
                cmd[3 + i * 3] = (byte)level;            
            }

            return cmd;
        }

        public CmdSetGPIO(JsonElement jsonRoot) : base(CreateCommand(jsonRoot)) { }
        public CmdSetGPIO(GPIO[] gpioArray) : base(createCommand(gpioArray)) { }
        
        public override (bool result, string reason) ParseReply()
        {
            if(reply == null)
            {
                return (false, "No reply is received");
            }
            if(reply.Length < 1)
            {
                return (false, "Invalid reply");
            }

            var cmd = Command;
            if(cmd[0] != reply[0])
            {
                return (false, $"Not SetGPIO reply: {reply[0]}");
            }
            if(reply.Length == 1)
            {
                return (false, $"Failed to run SetGPIO");
            }
            if(reply.Length != cmd.Length)
            {
                return (false, $"Wrong SetGPIO reply length: {reply.Length}, expected: {cmd.Length}");
            } 

            var portCount = (cmd.Length - 1) / 3;

            for(var i=0; i<portCount; i++)
            {
                if(cmd[1 + i * 3] != reply[1 + i * 3])
                {
                    return (false, $"Error in the {i}th port name in reply: ${reply[1 + i * 3]}, expected: {cmd[1 + i * 3]}");
                }
                if(cmd[2 + i * 3] != reply[2 + i * 3])
                {
                    return (false, $"Error in the {i}th port bitIndex in reply: ${reply[1 + i * 3]}, expected: {cmd[1 + i * 3]}");
                }
                if(cmd[3 + i * 3] != reply[3 + i * 3])
                {
                    return (false, $"Error in the {i}th port value in reply: ${reply[1 + i * 3]}, expected: {cmd[1 + i * 3]}");
                }
            }

            return (true, "");
        }
    }

}