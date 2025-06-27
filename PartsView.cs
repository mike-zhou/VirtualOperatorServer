
using System.Diagnostics;
using System.Text;
using VirtualOperatorServer.CommandAndReply;

internal static class PartsView
{
    public static string CreateGpioTable(ushort[]? modes, ushort[]? values)
    {
        if(modes == null)
        {
            return "No GPIO mode data";
        }
        if(values == null)
        {
            return "No GPIO port values";
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
            ushort value = values[portIndex];
            ushort mode = modes[portIndex];

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
