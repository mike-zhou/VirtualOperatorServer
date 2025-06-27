

using System.Text.Json;

namespace VirtualOperatorServer.CommandAndReply
{

class CmdSetPowerOutput: CmdSetGPIO
{
    static private CmdSetGPIO.GPIO[] createGpioArray(JsonElement jsonRoot)
    {
        // jsonRoot:
        // { 
        //     powerOutput: parseInt(powerNumber, 10),
        //     enable: enablePower
        // }        

        CmdSetGPIO.GPIO[] gpioArray = new CmdSetGPIO.GPIO[1];
        gpioArray[0] = new CmdSetGPIO.GPIO();

        byte powerIndex;
        bool enable;

        try
        {
            powerIndex = jsonRoot.GetProperty("powerOutput").GetByte();
            enable = jsonRoot.GetProperty("enable").GetBoolean();
        }
        catch (Exception)
        {
            throw new InvalidRequestBodyException($"Exception in deserializing: '{jsonRoot.GetRawText()}'");
        }

        switch(powerIndex)
        {
            case 0: 
                // GP98
                gpioArray[0].PortName = "PG";
                gpioArray[0].BitIndex = 9;
                break;
            case 1: 
                // GP96
                gpioArray[0].PortName = "PJ";
                gpioArray[0].BitIndex = 14;
                break;
            case 2: 
                // GP84
                gpioArray[0].PortName = "PC";
                gpioArray[0].BitIndex = 11;
                break;
            case 3: 
                // GP82
                gpioArray[0].PortName = "PI";
                gpioArray[0].BitIndex = 3;
                break;
            case 4: 
                // GP79
                gpioArray[0].PortName = "PI";
                gpioArray[0].BitIndex = 0;
                break;
            case 5:
                // GP78
                gpioArray[0].PortName = "PH";
                gpioArray[0].BitIndex = 15;
                break;
            case 6: 
                // GP104
                gpioArray[0].PortName = "PK";
                gpioArray[0].BitIndex = 4;
                break;
            case 7: 
                // GP107
                gpioArray[0].PortName = "PK";
                gpioArray[0].BitIndex = 7;
                break;
            case 8: 
                // GP113
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 0;
                break;
            case 9: 
                // GP114
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 1;
                break;
            case 10: 
                // GP1
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 3;
                break;
            case 11: 
                // GP3
                gpioArray[0].PortName = "PE";
                gpioArray[0].BitIndex = 6;
                break;
            default:
                throw new InvalidRequestBodyException($"Invalid power output index: '{powerIndex}'");
        }

        if(enable)
        {
            gpioArray[0].Level = 1;
        }
        else
        {
            gpioArray[0].Level = 0;
        }

        return gpioArray;
    }

    public CmdSetPowerOutput(JsonElement jsonRoot): base(createGpioArray(jsonRoot)) {}
}



}
