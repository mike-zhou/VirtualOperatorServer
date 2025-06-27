using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;

internal static class CommandFactory
{

    public static CommandAndReply BuildGetCommand(string command)
    {
        CommandAndReply cmd = new([]);

        string[] cmdSegments = command.Split('/');
        if (cmdSegments[0] == "Version")
        {
            cmd = new CmdGetVersion();
        }
        else if (cmdSegments[0] == "GPIOMode")
        {
            cmd = new CmdGetGPIOMode();
        }
        else if (cmdSegments[0] == "GPIO")
        {
            cmd = new CmdReadGPIO();
        }
        else if (cmdSegments[0] == "PeripharalStatus")
        {
            cmd = new CmdPeripheralStatus();
        }
        else if (cmdSegments[0] == "Encoders")
        {
            cmd = new CmdReadEncoders();
        }
        else
        {
            Console.WriteLine($"Error: unknown GET command: {command}");
        }

        return cmd;
    }
    
    static List<string> portNameList = new List<string> { "PA", "PB", "PC", "PD", "PE", "PF", "PG", "PH", "PI", "PJ", "PK" };

    static CmdSetGPIO.GPIO CreateGPIO(string gpioStr, bool highLevel)
    {
        var gpio = new CmdSetGPIO.GPIO();

        if(gpioStr.Length < 3)
        {
            throw new InvalidRequestBodyException($"Invalid GPIO in {gpioStr}");
        }

        gpio.PortName = gpioStr.Substring(0, 2);
        if(!portNameList.Contains(gpio.PortName))
        {
            throw new InvalidRequestBodyException($"Invalid port name in {gpioStr}");
        }

        var bitStr = gpioStr.Substring(2);
        bool success = byte.TryParse(bitStr, out byte bitIndex);
        if(!success)
        {
            throw new InvalidRequestBodyException($"Invalid bit index in {gpioStr}");
        }
        if(bitIndex > 15)
        {
            throw new InvalidRequestBodyException($"Invalid bit index in {gpioStr}");
        }
        gpio.BitIndex = bitIndex;

        gpio.Level = (byte)(highLevel ? 1 : 0);

        return gpio;
    }

    static CommandAndReply BuildSetBdcPowerOutput(JsonElement jsonRoot)
    {
        CmdSetGPIO.GPIO[] gpioArray = [new CmdSetGPIO.GPIO()];
        gpioArray[0].PortName = "PC";
        gpioArray[0].BitIndex = 15;
        gpioArray[0].Level = jsonRoot.GetProperty("enable").GetBoolean()? (byte)1 : (byte)0;

        return new CmdSetGPIO(gpioArray);
    }

    static CommandAndReply BuildSetBdcControl(JsonElement jsonRoot)
    {
        // payload = 
        // { 
        //     action: actionStr,
        //     index: parseInt(indexStr, 10)
        // };

        string? action = jsonRoot.GetProperty("action").GetString();
        int index = jsonRoot.GetProperty("index").GetInt32();

        if(action == null)
            throw new InvalidRequestBodyException($"No valid action in {jsonRoot.GetRawText()}");

        CmdSetGPIO.GPIO[] gpioArray = [new CmdSetGPIO.GPIO(), new CmdSetGPIO.GPIO()];
        switch (index)
        {
            case 0:
                // GP115, GP112
                gpioArray[0].PortName = "PI";
                gpioArray[0].BitIndex = 4;
                gpioArray[1].PortName = "PB";
                gpioArray[1].BitIndex = 9;
                break;
            case 1:
                // GP2, GP118
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 4;
                gpioArray[1].PortName = "PI";
                gpioArray[1].BitIndex = 7;
                break;
            case 2:
                // GP8, GP5
                gpioArray[0].PortName = "PI";
                gpioArray[0].BitIndex = 9;
                gpioArray[1].PortName = "PC";
                gpioArray[1].BitIndex = 13;
                break;
            default:
                throw new InvalidRequestBodyException($"Invalid index in {jsonRoot.GetRawText()}");
        }

        if(action == "coast")
        {
            gpioArray[0].Level = 0;
            gpioArray[1].Level = 0;
        }
        else if(action == "reverse")
        {
            gpioArray[0].Level = 0;
            gpioArray[1].Level = 1;
        }
        else if(action == "forward")
        {
            gpioArray[0].Level = 1;
            gpioArray[1].Level = 0;
        }
        else if(action == "brake")
        {
            gpioArray[0].Level = 1;
            gpioArray[1].Level = 1;
        }
        else
        {
            throw new InvalidRequestBodyException($"Unknown action in {jsonRoot.GetRawText()}");
        }
        
        return new CmdSetGPIO(gpioArray);
    }

    static CommandAndReply BuildClockStepper(JsonElement jsonRoot)
    {
        CmdSetGPIO.GPIO[] gpioArray = [new CmdSetGPIO.GPIO()];
        var stepperIndex = jsonRoot.GetProperty("index").GetByte();
        var highLevel = jsonRoot.GetProperty("highLevel").GetBoolean();

        switch(stepperIndex)
        {
            case 0:
                // PE5
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 5;
                break;
            case 1:
                // PF6
                gpioArray[0].PortName = "PF";
                gpioArray[0].BitIndex = 6;
                break;
            case 2: 
                // PF7
                gpioArray[0].PortName = "PF";
                gpioArray[0].BitIndex = 7;
                break;
            case 3:
                // PA6
                gpioArray[0].PortName = "PA";
                gpioArray[0].BitIndex = 6;
                break;
            case 4:
                // PA7
                gpioArray[0].PortName = "PA";
                gpioArray[0].BitIndex = 7;
                break;
            case 5:
                // PH6
                gpioArray[0].PortName = "PH";
                gpioArray[0].BitIndex = 6;
                break;
            case 6:
                // PJ10
                gpioArray[0].PortName = "PJ";
                gpioArray[0].BitIndex = 11;
                break;
            case 7: 
                // PK1
                gpioArray[0].PortName = "PK";
                gpioArray[0].BitIndex = 1;
                break;
            case 8:
                // PG3
                gpioArray[0].PortName = "PG";
                gpioArray[0].BitIndex = 3;
                break;
            case 9:
                // PG6
                gpioArray[0].PortName = "PG";
                gpioArray[0].BitIndex = 6;
                break;
            default:
                throw new InvalidRequestBodyException($"Wrong stepper index in {jsonRoot.GetRawText()}");
        }

        gpioArray[0].Level = highLevel? (byte)1 : (byte)0;

        return new CmdSetGPIO(gpioArray);
    }

    static CommandAndReply BuildForwardStepper(JsonElement jsonRoot)
    {
        var stepperIndex = jsonRoot.GetProperty("index").GetByte();
        var forwardStepper = jsonRoot.GetProperty("forwardStepper").GetBoolean();
        CmdSetGPIO.GPIO gpio;

        switch(stepperIndex)
        {
            case 0:
                // GP52
                gpio = CreateGPIO("PJ5", forwardStepper);
                break;
            case 1:
                // GP54
                gpio = CreateGPIO("PH8", forwardStepper);
                break;
            case 2: 
                // GP56
                gpio = CreateGPIO("PH12", forwardStepper);
                break;
            case 3:
                // GP57
                gpio = CreateGPIO("PD9", forwardStepper);
                break;
            case 4:
                // GP59
                gpio = CreateGPIO("PD14", forwardStepper);
                break;
            case 5:
                // GP61
                gpio = CreateGPIO("PJ7", forwardStepper);
                break;
            case 6:
                // GP64
                gpio = CreateGPIO("PJ11", forwardStepper);
                break;
            case 7: 
                // GP67
                gpio = CreateGPIO("PK2", forwardStepper);
                break;
            case 8:
                // GP70
                gpio = CreateGPIO("PG4", forwardStepper);
                break;
            case 9:
                // GP73
                gpio = CreateGPIO("PG7", forwardStepper);
                break;
            default:
                throw new InvalidRequestBodyException($"Wrong stepper index in {jsonRoot.GetRawText()}");
        }

        CmdSetGPIO.GPIO[] gpioArray = [gpio];

        return new CmdSetGPIO(gpioArray);
    }

    static CommandAndReply BuildDisableStepper(JsonElement jsonRoot)
    {
        var index = jsonRoot.GetProperty("index").GetByte();
        var disableStepper = jsonRoot.GetProperty("disableStepper").GetBoolean();
        CmdSetGPIO.GPIO gpio;

        switch(index)
        {
            case 0:
                // GP53
                gpio = CreateGPIO("PH7", disableStepper);
                break;
            case 1:
                // GP55
                gpio = CreateGPIO("PH9", disableStepper);
                break;
            case 2: 
                // USB_DEC_1
                gpio = CreateGPIO("PD8", disableStepper);
                break;
            case 3:
                // GP58
                gpio = CreateGPIO("PD10", disableStepper);
                break;
            case 4:
                // GP60
                gpio = CreateGPIO("PD15", disableStepper);
                break;
            case 5:
                // GP62
                gpio = CreateGPIO("PJ9", disableStepper);
                break;
            case 6:
                // GP65
                gpio = CreateGPIO("PK0", disableStepper);
                break;
            case 7: 
                // GP68
                gpio = CreateGPIO("PG2", disableStepper);
                break;
            case 8:
                // GP71
                gpio = CreateGPIO("PG5", disableStepper);
                break;
            case 9:
                // GP74
                gpio = CreateGPIO("PG8", disableStepper);
                break;
            default:
                throw new InvalidRequestBodyException($"Wrong stepper index in {jsonRoot.GetRawText()}");
        }

        CmdSetGPIO.GPIO[] gpioArray = [gpio];

        return new CmdSetGPIO(gpioArray);
    }

    public static CommandAndReply BuildPostCommand(string command, JsonElement jsonRoot)
    {
        CommandAndReply cmd = new([]);

        string[] cmdSegments = command.Split('/');
        var restApi = cmdSegments[0];

        if(restApi == "Echo")
        {
            cmd = new CmdEcho(jsonRoot);
        }
        else if (restApi == "setGpio")
        {
            cmd = new CmdSetGPIO(jsonRoot);
        }
        else if (restApi == "setPowerOutput")
        {
            cmd = new CmdSetPowerOutput(jsonRoot);
        }
        else if (restApi == "setBDCPowerOutput")
        {
            cmd = BuildSetBdcPowerOutput(jsonRoot);
        }
        else if (restApi == "setBDCControl")
        {
            cmd = BuildSetBdcControl(jsonRoot);
        }
        else if(restApi == "disableStepper")
        {
            cmd = BuildDisableStepper(jsonRoot);
        }
        else if (restApi == "forwardStepper")
        {
            cmd = BuildForwardStepper(jsonRoot);
        }
        else if(restApi == "clockStepper")
        {
            cmd = BuildClockStepper(jsonRoot);
        }
        else if(restApi == "runStepper")
        {

        }
        else
        {
            Console.WriteLine($"Error: unknown POST command: {command}");
        }

        return cmd;
    }


}
