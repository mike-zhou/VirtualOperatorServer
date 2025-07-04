
using System.Diagnostics;
using System.Text;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

internal static class ViewFactory
{
    public static string CreateVersion()
    {
        if (!BackService.Connected)
        {
            return "";
        }
        if (CmdGetVersion.Version == null)
        {
            return "";
        }

        return CmdGetVersion.Version;
    }

    public static string CreateGpioTable(ushort[]? modes, ushort[]? values)
    {
        if (modes == null)
        {
            return "";
        }
        if (values == null)
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
        for (int portIndex = 0; portIndex < portNameList.Count; portIndex++)
        {
            var portName = portNameList[portIndex];
            ushort value = values[portIndex];
            ushort mode = modes[portIndex];

            htmlBuilder.Append("<tr>");
            for (int bitIndex = 0; bitIndex < 16; bitIndex++)
            {
                string gpioId = $"id_setGpio_{portName}_{bitIndex}";
                bool isInput = ((mode & 0x1) == 0);

                htmlBuilder.Append("<td>");
                if (isInput)
                {
                    htmlBuilder.Append($"<label style=\"color: gray; cursor: not-allowed;\" for=\"{gpioId}\">  {portName}{bitIndex}: </lable>");
                }
                else
                {
                    htmlBuilder.Append($"<label for=\"{gpioId}\">  {portName}{bitIndex}: </lable>");
                }

                if ((value & 0x1) == 1)
                {
                    // high level
                    if (isInput)
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
                    if (isInput)
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

    public static string CreateGpioTable()
    {
        if (!BackService.Connected)
        {
            return "";
        }
        if (CmdGetStatus.Status == null)
        {
            return "";
        }
        if (CmdGetGPIOMode.GpioModes == null)
        {
            return "";
        }

        var modes = CmdGetGPIOMode.GpioModes;
        var ports = CmdGetStatus.Status.ports;
        return CreateGpioTable(modes, ports);
    }

    public static string CreatePeripheralStatus()
    {
        if (!BackService.Connected)
        {
            return "";
        }
        if (CmdGetStatus.Status == null)
        {
            return "";
        }

        var status = CmdGetStatus.Status;

        bool IsGpioHigh(char port, uint bitIndex)
        {
            var portIndex = port - 'A';
            Debug.Assert(portIndex >= 0);
            Debug.Assert(portIndex <= 10);

            ushort portValue = status.ports[portIndex];

            if ((portValue & (1 << (int)bitIndex)) == 0)
                return false;
            else
                return true;
        }

        string BridgeControlString(bool input1, bool input2)
        {
            if (input1)
            {
                if (input2)
                    return "Brake";
                else
                    return "Forward";
            }
            else
            {
                if (input2)
                    return "Reverse";
                else
                    return "Coast";
            }
        }

        string JsonStr(bool value)
        {
            if (value)
                return "true";
            else
                return "false";
        }

        string GetPowerStatus()
        {
            string Po0Enabled;
            string Po0Available;
            string Po1Enabled;
            string Po1Available;
            string Po2Enabled;
            string Po2Available;
            string Po3Enabled;
            string Po3Available;
            string Po4Enabled;
            string Po4Available;
            string Po5Enabled;
            string Po5Available;

            // PO 0: GP98, GP101
            if (IsGpioHigh('G', 9))
                Po0Enabled = "true";
            else
                Po0Enabled = "false";
            if (IsGpioHigh('G', 13))
                Po0Available = "true";
            else
                Po0Available = "false";

            // PO 1: GP96, GP95
            if (IsGpioHigh('J', 14))
                Po1Enabled = "true";
            else
                Po1Enabled = "false";
            if (IsGpioHigh('J', 13))
                Po1Available = "true";
            else
                Po1Available = "false";

            // PO 2: GP84, GP92
            if (IsGpioHigh('C', 11))
                Po2Enabled = "true";
            else
                Po2Enabled = "false";
            if (IsGpioHigh('D', 6))
                Po2Available = "true";
            else
                Po2Available = "false";

            // PO 3: GP82, GP81
            if (IsGpioHigh('I', 3))
                Po3Enabled = "true";
            else
                Po3Enabled = "false";
            if (IsGpioHigh('I', 2))
                Po3Available = "true";
            else
                Po3Available = "false";

            // PO 4: GP79, GP80
            if (IsGpioHigh('I', 0))
                Po4Enabled = "true";
            else
                Po4Enabled = "false";
            if (IsGpioHigh('I', 1))
                Po4Available = "true";
            else
                Po4Available = "false";

            // PO 5: GP78, GP77
            if (IsGpioHigh('H', 15))
                Po5Enabled = "true";
            else
                Po5Enabled = "false";
            if (IsGpioHigh('H', 14))
                Po5Available = "true";
            else
                Po5Available = "false";

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            jsonBuilder.Append("\"PO0\":{\"Enabled\":" + Po0Enabled + ",\"Available\":" + Po0Available + "},");
            jsonBuilder.Append("\"PO1\":{\"Enabled\":" + Po1Enabled + ",\"Available\":" + Po1Available + "},");
            jsonBuilder.Append("\"PO2\":{\"nabled\":" + Po2Enabled + ",\"Available\":" + Po2Available + "},");
            jsonBuilder.Append("\"PO3\":{\"Enabled\":" + Po3Enabled + ",\"Available\":" + Po3Available + "},");
            jsonBuilder.Append("\"PO4\":{\"Enabled\":" + Po4Enabled + ",\"Available\":" + Po4Available + "},");
            jsonBuilder.Append("\"PO5\":{\"Enabled\":" + Po5Enabled + ",\"Available\":" + Po5Available + "}");
            jsonBuilder.Append('}');

            return jsonBuilder.ToString();
        }

        string GetStepperPowerStatus()
        {
            string Po6Enabled;
            string Po6Available;
            string Po7Enabled;
            string Po7Available;
            string Po8Enabled;
            string Po8Available;
            string Po9Enabled;
            string Po9Available;
            string Po10Enabled;
            string Po10Available;
            string Po11Enabled;
            string Po11Available;

            // PO 6: GP104, GP102
            if (IsGpioHigh('K', 4))
                Po6Enabled = "true";
            else
                Po6Enabled = "false";
            if (IsGpioHigh('G', 14))
                Po6Available = "true";
            else
                Po6Available = "false";

            // PO 7: GP107, GP108
            if (IsGpioHigh('K', 7))
                Po7Enabled = "true";
            else
                Po7Enabled = "false";
            if (IsGpioHigh('G', 15))
                Po7Available = "true";
            else
                Po7Available = "false";

            // PO 8: GP113, GP110
            if (IsGpioHigh('E', 0))
                Po8Enabled = "true";
            else
                Po8Enabled = "false";
            if (IsGpioHigh('B', 7))
                Po8Available = "true";
            else
                Po8Available = "false";

            // PO 9: GP114, GP116
            if (IsGpioHigh('E', 1))
                Po9Enabled = "true";
            else
                Po9Enabled = "false";
            if (IsGpioHigh('I', 5))
                Po9Available = "true";
            else
                Po9Available = "false";

            // PO 10: GP1, GP0
            if (IsGpioHigh('E', 3))
                Po10Enabled = "true";
            else
                Po10Enabled = "false";
            if (IsGpioHigh('E', 2))
                Po10Available = "true";
            else
                Po10Available = "false";

            // PO 11: GP3, GP6
            if (IsGpioHigh('E', 6))
                Po11Enabled = "true";
            else
                Po11Enabled = "false";
            if (IsGpioHigh('C', 14))
                Po11Available = "true";
            else
                Po11Available = "false";

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            jsonBuilder.Append("\"PO6\":{\"Enabled\":" + Po6Enabled + ",\"Available\":" + Po6Available + "},");
            jsonBuilder.Append("\"PO7\":{\"Enabled\":" + Po7Enabled + ",\"Available\":" + Po7Available + "},");
            jsonBuilder.Append("\"PO8\":{\"Enabled\":" + Po8Enabled + ",\"Available\":" + Po8Available + "},");
            jsonBuilder.Append("\"PO9\":{\"Enabled\":" + Po9Enabled + ",\"Available\":" + Po9Available + "},");
            jsonBuilder.Append("\"PO10\":{\"Enabled\":" + Po10Enabled + ",\"Available\":" + Po10Available + "},");
            jsonBuilder.Append("\"PO11\":{\"Enabled\":" + Po11Enabled + ",\"Available\":" + Po11Available + "}");
            jsonBuilder.Append('}');

            return jsonBuilder.ToString();
        }

        string GetBDCPowerStatus()
        {
            string Enabled;
            string Available;

            // GP, GP9
            if (IsGpioHigh('C', 15))
                Enabled = "true";
            else
                Enabled = "false";
            if (IsGpioHigh('I', 10))
                Available = "true";
            else
                Available = "false";

            string json = "{\"Enabled\":" + Enabled + ",\"Available\":" + Available + "}";
            return json;
        }

        string GetBDCMotorStatus()
        {
            bool bdc0Input1, bdc0Input2, bdc0PowerAvailable;
            bool bdc1Input1, bdc1Input2, bdc1PowerAvailable;
            bool bdc2Input1, bdc2Input2, bdc2PowerAvailable;

            // BDC 0: GP115, GP112, GP117
            bdc0Input1 = IsGpioHigh('I', 4);
            bdc0Input2 = IsGpioHigh('B', 9);
            bdc0PowerAvailable = IsGpioHigh('I', 6);

            // BDC 1: GP2, GP118, GP4
            bdc1Input1 = IsGpioHigh('E', 4);
            bdc1Input2 = IsGpioHigh('I', 7);
            bdc1PowerAvailable = IsGpioHigh('I', 8);

            // BDC 2: GP8, GP5, GP10
            bdc2Input1 = IsGpioHigh('I', 9);
            bdc2Input2 = IsGpioHigh('C', 13);
            bdc2PowerAvailable = IsGpioHigh('F', 0);

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            jsonBuilder.Append("\"BDC0\":{\"Control\":\"" + BridgeControlString(bdc0Input1, bdc0Input2) + "\",\"PowerAvailable\":" + JsonStr(bdc0PowerAvailable) + "},");
            jsonBuilder.Append("\"BDC1\":{\"Control\":\"" + BridgeControlString(bdc1Input1, bdc1Input2) + "\",\"PowerAvailable\":" + JsonStr(bdc1PowerAvailable) + "},");
            jsonBuilder.Append("\"BDC2\":{\"Control\":\"" + BridgeControlString(bdc2Input1, bdc2Input2) + "\",\"PowerAvailable\":" + JsonStr(bdc2PowerAvailable) + "}");
            jsonBuilder.Append('}');

            return jsonBuilder.ToString();
        }

        string GetStepperMotorStatus()
        {
            bool stp0Forward, stp0Disable, stp0Alarm, stp0Clock;
            bool stp1Forward, stp1Disable, stp1Alarm, stp1Clock;
            bool stp2Forward, stp2Disable, stp2Alarm, stp2Clock;
            bool stp3Forward, stp3Disable, stp3Alarm, stp3Clock;
            bool stp4Forward, stp4Disable, stp4Alarm, stp4Clock;
            bool stp5Forward, stp5Disable, stp5Alarm, stp5Clock;
            bool stp6Forward, stp6Disable, stp6Alarm, stp6Clock;
            bool stp7Forward, stp7Disable, stp7Alarm, stp7Clock;
            bool stp8Forward, stp8Disable, stp8Alarm, stp8Clock;
            bool stp9Forward, stp9Disable, stp9Alarm, stp9Clock;

            // stepper 0: GP52, GP53, GP44, CLK_STP0
            stp0Forward = IsGpioHigh('J', 5);
            stp0Disable = IsGpioHigh('H', 7);
            stp0Alarm = IsGpioHigh('G', 1);
            stp0Clock = IsGpioHigh('E', 5);

            // stepper 1: GP54, GP55, GP46, CLK_STP1
            stp1Forward = IsGpioHigh('H', 8);
            stp1Disable = IsGpioHigh('H', 9);
            stp1Alarm = IsGpioHigh('E', 8);
            stp1Clock = IsGpioHigh('F', 6);

            // stepper 2: GP56, USB_DEC_1, GP47, CLK_STP2
            stp2Forward = IsGpioHigh('H', 12);
            stp2Disable = IsGpioHigh('D', 8);
            stp2Alarm = IsGpioHigh('E', 10);
            stp2Clock = IsGpioHigh('F', 7);

            // stepper 3: GP57, GP58, GP48, CLK_STP3, CLK_STP3
            stp3Forward = IsGpioHigh('D', 9);
            stp3Disable = IsGpioHigh('D', 10);
            stp3Alarm = IsGpioHigh('E', 12);
            stp3Clock = IsGpioHigh('A', 6);

            // stepper 4: GP59, GP60, GP49, CLK_STP4
            stp4Forward = IsGpioHigh('D', 14);
            stp4Disable = IsGpioHigh('D', 15);
            stp4Alarm = IsGpioHigh('E', 13);
            stp4Clock = IsGpioHigh('A', 7);

            // stepper 5: GP61, GP62, GP50, CLK_STP5
            stp5Forward = IsGpioHigh('J', 7);
            stp5Disable = IsGpioHigh('J', 9);
            stp5Alarm = IsGpioHigh('E', 14);
            stp5Clock = IsGpioHigh('H', 6);

            // stepper 6: GP64, GP65, GP51, GP63
            stp6Forward = IsGpioHigh('J', 11);
            stp6Disable = IsGpioHigh('K', 0);
            stp6Alarm = IsGpioHigh('E', 15);
            stp6Clock = IsGpioHigh('J', 10);

            // stepper 7: GP67, GP68, GP86, GP66
            stp7Forward = IsGpioHigh('K', 2);
            stp7Disable = IsGpioHigh('G', 2);
            stp7Alarm = IsGpioHigh('D', 0);
            stp7Clock = IsGpioHigh('K', 1);

            // stepper 8: GP70, GP71, GP83, GP69
            stp8Forward = IsGpioHigh('G', 4);
            stp8Disable = IsGpioHigh('G', 5);
            stp8Alarm = IsGpioHigh('C', 10);
            stp8Clock = IsGpioHigh('G', 3);

            // stepper 9: GP73, GP74, GP85, GP72
            stp9Forward = IsGpioHigh('G', 7);
            stp9Disable = IsGpioHigh('G', 8);
            stp9Alarm = IsGpioHigh('C', 12);
            stp9Clock = IsGpioHigh('G', 6);

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            jsonBuilder.Append("\"stepper0\":{\"forward\":" + JsonStr(stp0Forward) + ",\"disable\":" + JsonStr(stp0Disable) + ",\"alarm\":" + JsonStr(stp0Alarm) + ",\"clock\":" + JsonStr(stp0Clock) + "},");
            jsonBuilder.Append("\"stepper1\":{\"forward\":" + JsonStr(stp1Forward) + ",\"disable\":" + JsonStr(stp1Disable) + ",\"alarm\":" + JsonStr(stp1Alarm) + ",\"clock\":" + JsonStr(stp1Clock) + "},");
            jsonBuilder.Append("\"stepper2\":{\"forward\":" + JsonStr(stp2Forward) + ",\"disable\":" + JsonStr(stp2Disable) + ",\"alarm\":" + JsonStr(stp2Alarm) + ",\"clock\":" + JsonStr(stp2Clock) + "},");
            jsonBuilder.Append("\"stepper3\":{\"forward\":" + JsonStr(stp3Forward) + ",\"disable\":" + JsonStr(stp3Disable) + ",\"alarm\":" + JsonStr(stp3Alarm) + ",\"clock\":" + JsonStr(stp3Clock) + "},");
            jsonBuilder.Append("\"stepper4\":{\"forward\":" + JsonStr(stp4Forward) + ",\"disable\":" + JsonStr(stp4Disable) + ",\"alarm\":" + JsonStr(stp4Alarm) + ",\"clock\":" + JsonStr(stp4Clock) + "},");
            jsonBuilder.Append("\"stepper5\":{\"forward\":" + JsonStr(stp5Forward) + ",\"disable\":" + JsonStr(stp5Disable) + ",\"alarm\":" + JsonStr(stp5Alarm) + ",\"clock\":" + JsonStr(stp5Clock) + "},");
            jsonBuilder.Append("\"stepper6\":{\"forward\":" + JsonStr(stp6Forward) + ",\"disable\":" + JsonStr(stp6Disable) + ",\"alarm\":" + JsonStr(stp6Alarm) + ",\"clock\":" + JsonStr(stp6Clock) + "},");
            jsonBuilder.Append("\"stepper7\":{\"forward\":" + JsonStr(stp7Forward) + ",\"disable\":" + JsonStr(stp7Disable) + ",\"alarm\":" + JsonStr(stp7Alarm) + ",\"clock\":" + JsonStr(stp7Clock) + "},");
            jsonBuilder.Append("\"stepper8\":{\"forward\":" + JsonStr(stp8Forward) + ",\"disable\":" + JsonStr(stp8Disable) + ",\"alarm\":" + JsonStr(stp8Alarm) + ",\"clock\":" + JsonStr(stp8Clock) + "},");
            jsonBuilder.Append("\"stepper9\":{\"forward\":" + JsonStr(stp9Forward) + ",\"disable\":" + JsonStr(stp9Disable) + ",\"alarm\":" + JsonStr(stp9Alarm) + ",\"clock\":" + JsonStr(stp9Clock) + "}");
            jsonBuilder.Append('}');

            return jsonBuilder.ToString();
        }

        string GetPositionDetectorStatus()
        {
            bool pd0 = IsGpioHigh('B', 8); // GP111
            bool pd1 = IsGpioHigh('B', 4); // GP109    
            bool pd2 = IsGpioHigh('K', 6); // GP106    
            bool pd3 = IsGpioHigh('K', 5); // GP105    
            bool pd4 = IsGpioHigh('K', 3); // GP103
            bool pd5 = IsGpioHigh('G', 11); // GP100
            bool pd6 = IsGpioHigh('G', 10); // GP99
            bool pd7 = IsGpioHigh('J', 15); // GP97
            bool pd8 = IsGpioHigh('J', 12); // GP94
            bool pd9 = IsGpioHigh('D', 7); // GP93
            bool pd10 = IsGpioHigh('D', 5); // GP91
            bool pd11 = IsGpioHigh('D', 1); // GP87
            bool pd12 = IsGpioHigh('D', 4); // GP90
            bool pd13 = IsGpioHigh('D', 3); // GP89
            bool pd14 = IsGpioHigh('D', 2); // GP88
            bool pd15 = IsGpioHigh('A', 4); // GP28
            bool pd16 = IsGpioHigh('H', 5); // GP27
            bool pd17 = IsGpioHigh('H', 3); // GP25
            bool pd18 = IsGpioHigh('C', 1); // GP22
            bool pd19 = IsGpioHigh('F', 10); // GP21
            bool pd20 = IsGpioHigh('F', 8); // GP19
            bool pd21 = IsGpioHigh('F', 4); // GP17
            bool pd22 = IsGpioHigh('F', 3); // GP16
            bool pd23 = IsGpioHigh('I', 13); // GP14
            bool pd24 = IsGpioHigh('F', 2); // GP12
            bool pd25 = IsGpioHigh('F', 1); // GP11
            bool pd26 = IsGpioHigh('I', 12); // GP13
            bool pd27 = IsGpioHigh('I', 14); // GP15
            bool pd28 = IsGpioHigh('F', 5); // GP18
            bool pd29 = IsGpioHigh('F', 9); // GP20
            bool pd30 = IsGpioHigh('C', 2); // GP23
            bool pd31 = IsGpioHigh('A', 2); // GP24
            bool pd32 = IsGpioHigh('H', 4); // GP26
            bool pd33 = IsGpioHigh('C', 4); // GP29
            bool pd34 = IsGpioHigh('C', 5); // GP30
            bool pd35 = IsGpioHigh('I', 15); // GP32
            bool pd36 = IsGpioHigh('B', 2); // GP31
            bool pd37 = IsGpioHigh('J', 0); // GP33
            bool pd38 = IsGpioHigh('J', 1); // GP34
            bool pd39 = IsGpioHigh('J', 2); // GP35
            bool pd40 = IsGpioHigh('J', 3); // GP36
            bool pd41 = IsGpioHigh('J', 4); // GP37
            bool pd42 = IsGpioHigh('E', 7); // GP45
            bool pd43 = IsGpioHigh('G', 0); // GP43
            bool pd44 = IsGpioHigh('F', 11); // GP38
            bool pd45 = IsGpioHigh('F', 13); // GP40
            bool pd46 = IsGpioHigh('F', 12); // GP39
            bool pd47 = IsGpioHigh('F', 14); // GP41
            bool pd48 = IsGpioHigh('F', 15); // GP42

            var pdValues = new bool[]
            {
                pd0, pd1, pd2, pd3, pd4, pd5, pd6, pd7, pd8, pd9,
                pd10, pd11, pd12, pd13, pd14, pd15, pd16, pd17, pd18, pd19,
                pd20, pd21, pd22, pd23, pd24, pd25, pd26, pd27, pd28, pd29,
                pd30, pd31, pd32, pd33, pd34, pd35, pd36, pd37, pd38, pd39,
                pd40, pd41, pd42, pd43, pd44, pd45, pd46, pd47, pd48
            };

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            for (int i = 0; i < pdValues.Length; i++)
            {
                jsonBuilder.Append($"\"pd{i}\":{JsonStr(pdValues[i])}");
                if (i < pdValues.Length - 1)
                    jsonBuilder.Append(',');
            }
            jsonBuilder.Append('}');

            return jsonBuilder.ToString();
        }

        StringBuilder jsonBuilder = new StringBuilder();

        jsonBuilder.Append('{');
        jsonBuilder.Append("\"Power\":" + GetPowerStatus() + ",");
        jsonBuilder.Append("\"StepperPower\":" + GetStepperPowerStatus() + ",");
        jsonBuilder.Append("\"BDCPower\":" + GetBDCPowerStatus() + ",");
        jsonBuilder.Append("\"BDCMotor\":" + GetBDCMotorStatus() + ",");
        jsonBuilder.Append("\"StepperMotor\":" + GetStepperMotorStatus() + ",");
        jsonBuilder.Append("\"PositionDetector\":" + GetPositionDetectorStatus());
        jsonBuilder.Append('}');

        var jsonStr = jsonBuilder.ToString();
        return jsonStr;
    }

    public static string CreateEncoders()
    {
        if (!BackService.Connected)
        {
            return "";
        }
        if (CmdGetStatus.Status == null)
        {
            return "";
        }

        var status = CmdGetStatus.Status;

        StringBuilder builder = new StringBuilder();

        builder.Append('{');
        builder.Append($"\"lptim1Counter\":{status.encoders[0]},");
        builder.Append($"\"lptim2Counter\":{status.encoders[1]},");
        builder.Append($"\"htim1Counter\":{status.encoders[2]},");
        builder.Append($"\"htim2Counter\":{status.encoders[3]},");
        builder.Append($"\"htim3Counter\":{status.encoders[4]},");
        builder.Append($"\"htim4Counter\":{status.encoders[5]},");
        builder.Append($"\"htim5Counter\":{status.encoders[6]},");
        builder.Append($"\"htim8Counter\":{status.encoders[7]}");
        builder.Append('}');

        return builder.ToString();
    }

    private static uint previousMainLoop = 0;

    public static string CreateDynamicStatus()
    {
        if (!BackService.Connected)
        {
            return "";
        }
        if (CmdGetStatus.Status == null)
        {
            return "";
        }

        var status = CmdGetStatus.Status;

        if (previousMainLoop == 0)
        {
            previousMainLoop = status.mainLoopCount;
            return "";
        }

        uint loopPerSecond;
        if (status.mainLoopCount >= previousMainLoop)
        {
            loopPerSecond = status.mainLoopCount - previousMainLoop;
        }
        else
        {
            loopPerSecond = (uint.MaxValue - previousMainLoop) + status.mainLoopCount;
        }
        previousMainLoop = status.mainLoopCount;

        StringBuilder builder = new StringBuilder();
        builder.Append("<table>");
        builder.Append($"<tr><td>Main loop/s: </td><td>{loopPerSecond}</td></tr>");
        builder.Append($"<tr><td>Max flex timer ISR period: </td><td>{status.maxFlexTimerIsrPeriod}</td></tr>");
        builder.Append($"<tr><td>Max fix timer ISR period: </td><td>{status.maxFixTimerIsrPeriod}</td></tr>");
        builder.Append("</table>");

        return builder.ToString();
    }

    private static string CreateStepperMode(int stepperId)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<table>");
        builder.Append("<tr>");
        {
            builder.Append("<td>");
            builder.Append($"<input type=\"radio\" id=\"id_stepper_mode_forced_{stepperId}\" name=\"stepper_mode_{stepperId}\">");
            builder.Append($"<label for=\"id_stepper_mode_forced_{stepperId}\">Forced</label>");
            builder.Append("</td>");

            builder.Append("<td>");
            builder.Append($"<div id='id_stepper_group_mode_forced_{stepperId}'>");
            builder.Append($"<label>Pulse period: </label><input type=\"number\" id=\"id_stepper_period_force_{stepperId}\" min=\"1\" step=\"1\" max=\"65536\">");
            builder.Append("</div>");
            builder.Append("</td>");
        }
        builder.Append("</tr>");
        builder.Append("<tr>");
        {
            builder.Append("<td>");
            builder.Append($"<input type=\"radio\" id=\"id_stepper_mode_active_{stepperId}\" name=\"stepper_mode_{stepperId}\">");
            builder.Append($"<label for=\"id_stepper_mode_active_{stepperId}\">Active</label>");
            builder.Append("</td>");

            builder.Append("<td>");
            builder.Append($"<div id='id_stepper_group_mode_active_{stepperId}'>");
            builder.Append($"<label>Starting pulse period: </label><input type=\"number\" id=\"id_stepper_period_active_starting_{stepperId}\" min=\"1\" step=\"1\" max=\"65536\">");
            builder.Append($"<label>Acceleration steps: </label><input type=\"number\" id=\"id_stepper_period_active_accelerationSteps_{stepperId}\" min=\"1\" step=\"1\" max=\"1024\">");
            builder.Append("<br>");
            builder.Append($"<label>Cruising period: </label><input type=\"number\" id=\"id_stepper_period_active_cruising_{stepperId}\" min=\"1\" step=\"1\" max=\"65536\">");
            builder.Append("<br>");
            builder.Append($"<label>Ending pulse period: </label><input type=\"number\" id=\"id_stepper_period_active_ending_{stepperId}\" min=\"1\" step=\"1\" max=\"65536\">");
            builder.Append($"<label>Deacceleration steps: </label><input type=\"number\" id=\"id_stepper_period_active_deaccelerationSteps_{stepperId}\" min=\"1\" step=\"1\" max=\"1024\">");
            builder.Append("</div>");
            builder.Append("</td>");
        }
        builder.Append("</tr>");
        builder.Append("<tr>");
        {
            builder.Append("<td>");
            builder.Append($"<input type=\"radio\" id=\"id_stepper_mode_passive_{stepperId}\" name=\"stepper_mode_{stepperId}\">");
            builder.Append($"<label for=\"id_stepper_mode_passive_{stepperId}\">Passive</label>");
            builder.Append("</td>");

            builder.Append("<td>");
            builder.Append($"<div id='id_stepper_group_mode_passive_{stepperId}'>");
            builder.Append($"<label>ActiveStepper: </label><select id=\"id_stepper_period_passive_stepper_{stepperId}\">");
            for (int i = 0; i < 10; i++)
            {
                if (i == stepperId)
                {
                    continue;
                }
                builder.Append($"<option value=\"{i}\">Stepper {i}</option>");
            }
            builder.Append($"<option value=\"255\" selected>Not selected</option>");
            builder.Append("</select>");
            builder.Append("</div>");
            builder.Append("</td>");
        }
        builder.Append("</tr>");
        builder.Append("</table>");

        return builder.ToString();
    }

    public static string CreateSteppers()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<h1>Stepper Control</h1>");
        for (int i = 0; i < 10; i++)
        {
            builder.Append("<div>");
            builder.Append($"<h2>Stepper {i}</h2>");
            builder.Append("<div>");
            builder.Append("<div>");
            builder.Append("<div>");
            builder.Append("<label>Alarm:</label>");
            builder.Append($"<div class=\"unknown - dot\" id=\"id_stepperAlarm_state_{i}\"></div>");
            builder.Append("</div>");
            builder.Append($"<label>Disable<input type=\"checkbox\" id=\"id_stepper_disable_{i}\"></label>");
            builder.Append($"<label>Forward<input type=\"checkbox\" id=\"id_stepper_forward_{i}\"></label>");
            builder.Append($"<label>Clock<input type=\"checkbox\" id=\"id_stepper_clock_{i}\"></label>");
            builder.Append("</div>");
            builder.Append($"<div>{CreateStepperMode(i)}</div>");
            builder.Append("<div>");
            builder.Append($"<button id=\"id_stepper_go_1_{i}\">1</button>");
            builder.Append($"<button id=\"id_stepper_go_2_{i}\">2</button>");
            builder.Append($"<button id=\"id_stepper_go_4_{i}\">4</button>");
            builder.Append($"<button id=\"id_stepper_go_8_{i}\">8</button>");
            builder.Append($"<button id=\"id_stepper_go_16_{i}\">16</button>");
            builder.Append($"<button id=\"id_stepper_go_32_{i}\">32</button>");
            builder.Append($"<button id=\"id_stepper_go_64_{i}\">64</button>");
            builder.Append($"<input type=\"number\" id=\"id_stepper_steps_{i}\" min=\"1\" step=\"1\" max=\"1024\">");
            builder.Append($"<button id=\"id_stepper_go_steps_{i}\">go</button>");
            builder.Append("</div>");
            builder.Append("</div>");
            builder.Append("</div>");
        }

        return builder.ToString();
    }

}
