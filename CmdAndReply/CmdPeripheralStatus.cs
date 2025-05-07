
using System.Diagnostics;
using System.Text;

namespace VirtualOperatorServer.CommandAndReply
{
class CmdPeripheralStatus: CmdReadGPIO
{
    public CmdPeripheralStatus(): base() {}

    public override string ParseReply()
    {
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

    private string GetPowerStatus()
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
        if(IsGpioHigh('G', 9)) 
            Po0Enabled = "true";
        else
            Po0Enabled = "false";
        if(IsGpioHigh('G', 13)) 
            Po0Available = "true";
        else
            Po0Available = "false";

        // PO 1: GP96, GP95
        if(IsGpioHigh('J', 14)) 
            Po1Enabled = "true";
        else
            Po1Enabled = "false";
        if(IsGpioHigh('J', 13)) 
            Po1Available = "true";
        else
            Po1Available = "false";

        // PO 2: GP84, GP92
        if(IsGpioHigh('C', 11)) 
            Po2Enabled = "true";
        else
            Po2Enabled = "false";
        if(IsGpioHigh('D', 6)) 
            Po2Available = "true";
        else
            Po2Available = "false";

        // PO 3: GP82, GP81
        if(IsGpioHigh('I', 3)) 
            Po3Enabled = "true";
        else
            Po3Enabled = "false";
        if(IsGpioHigh('I', 2)) 
            Po3Available = "true";
        else
            Po3Available = "false";

        // PO 4: GP79, GP80
        if(IsGpioHigh('I', 0)) 
            Po4Enabled = "true";
        else
            Po4Enabled = "false";
        if(IsGpioHigh('I', 1)) 
            Po4Available = "true";
        else
            Po4Available = "false";

        // PO 5: GP78, GP77
        if(IsGpioHigh('H', 15)) 
            Po5Enabled = "true";
        else
            Po5Enabled = "false";
        if(IsGpioHigh('H', 14)) 
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

    private string GetStepperPowerStatus()
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
        if(IsGpioHigh('K', 4)) 
            Po6Enabled = "true";
        else
            Po6Enabled = "false";
        if(IsGpioHigh('G', 14)) 
            Po6Available = "true";
        else
            Po6Available = "false";

        // PO 7: GP107, GP108
        if(IsGpioHigh('K', 7)) 
            Po7Enabled = "true";
        else
            Po7Enabled = "false";
        if(IsGpioHigh('G', 15)) 
            Po7Available = "true";
        else
            Po7Available = "false";

        // PO 8: GP113, GP110
        if(IsGpioHigh('E', 0)) 
            Po8Enabled = "true";
        else
            Po8Enabled = "false";
        if(IsGpioHigh('B', 7)) 
            Po8Available = "true";
        else
            Po8Available = "false";

        // PO 9: GP114, GP116
        if(IsGpioHigh('E', 1)) 
            Po9Enabled = "true";
        else
            Po9Enabled = "false";
        if(IsGpioHigh('I', 5)) 
            Po9Available = "true";
        else
            Po9Available = "false";

        // PO 10: GP1, GP0
        if(IsGpioHigh('E', 3)) 
            Po10Enabled = "true";
        else
            Po10Enabled = "false";
        if(IsGpioHigh('E', 2)) 
            Po10Available = "true";
        else
            Po10Available = "false";

        // PO 11: GP3, GP6
        if(IsGpioHigh('E', 6)) 
            Po11Enabled = "true";
        else
            Po11Enabled = "false";
        if(IsGpioHigh('C', 14)) 
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

    private string GetBDCPowerStatus()
    {
        string Enabled;
        string Available;

         // GP, GP9
        if(IsGpioHigh('C', 15)) 
            Enabled = "true";
        else
            Enabled = "false";
        if(IsGpioHigh('I', 10)) 
            Available = "true";
        else
            Available = "false";

        string json = "{\"Enabled\":" + Enabled + ",\"Available\":" + Available + "}";
        return json;
   }

    private string GetBDCMotorStatus()
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

    private string GetStepperMotorStatus()
    {
        bool stp0Forward, stp0Enable, stp0Alarm, stp0Clock;
        bool stp1Forward, stp1Enable, stp1Alarm, stp1Clock;
        bool stp2Forward, stp2Enable, stp2Alarm, stp2Clock;
        bool stp3Forward, stp3Enable, stp3Alarm, stp3Clock;
        bool stp4Forward, stp4Enable, stp4Alarm, stp4Clock;
        bool stp5Forward, stp5Enable, stp5Alarm, stp5Clock;
        bool stp6Forward, stp6Enable, stp6Alarm, stp6Clock;
        bool stp7Forward, stp7Enable, stp7Alarm, stp7Clock;
        bool stp8Forward, stp8Enable, stp8Alarm, stp8Clock;
        bool stp9Forward, stp9Enable, stp9Alarm, stp9Clock;

        // stepper 0: GP52, GP53, GP44, CLK_STP0
        stp0Forward =   IsGpioHigh('J', 5);
        stp0Enable =    IsGpioHigh('H', 7);
        stp0Alarm =     IsGpioHigh('G', 1);
        stp0Clock =     IsGpioHigh('E', 5);

        // stepper 1: GP54, GP55, GP46, CLK_STP1
        stp1Forward =   IsGpioHigh('H', 8);
        stp1Enable =    IsGpioHigh('H', 9);
        stp1Alarm =     IsGpioHigh('E', 8);
        stp1Clock =     IsGpioHigh('F', 6);

        // stepper 2: GP56, USB_DEC_1, GP47, CLK_STP2
        stp2Forward =   IsGpioHigh('H', 12);
        stp2Enable =    IsGpioHigh('D', 8);
        stp2Alarm =     IsGpioHigh('E', 10);
        stp2Clock =     IsGpioHigh('F', 7);

        // stepper 3: GP57, GP58, GP48, CLK_STP3, CLK_STP3
        stp3Forward =   IsGpioHigh('D', 9);
        stp3Enable =    IsGpioHigh('D', 10);
        stp3Alarm =     IsGpioHigh('E', 12);
        stp3Clock =     IsGpioHigh('A', 6);

        // stepper 4: GP59, GP60, GP49, CLK_STP4
        stp4Forward =   IsGpioHigh('D', 14);
        stp4Enable =    IsGpioHigh('D', 15);
        stp4Alarm =     IsGpioHigh('E', 13);
        stp4Clock = IsGpioHigh('A', 7);

        // stepper 5: GP61, GP62, GP50, CLK_STP5
        stp5Forward =   IsGpioHigh('J', 7);
        stp5Enable =    IsGpioHigh('J', 9);
        stp5Alarm =     IsGpioHigh('E', 14);
        stp5Clock =     IsGpioHigh('H', 6);

        // stepper 6: GP64, GP65, GP51, GP63
        stp6Forward =   IsGpioHigh('J', 11);
        stp6Enable =    IsGpioHigh('K', 0);
        stp6Alarm =     IsGpioHigh('E', 15);
        stp6Clock =     IsGpioHigh('J', 10);

        // stepper 7: GP67, GP68, GP86, GP66
        stp7Forward =   IsGpioHigh('K', 2);
        stp7Enable =    IsGpioHigh('G', 2);
        stp7Alarm =     IsGpioHigh('D', 0);
        stp7Clock =     IsGpioHigh('K', 1);

        // stepper 8: GP70, GP71, GP83, GP69
        stp8Forward =   IsGpioHigh('G', 4);
        stp8Enable =    IsGpioHigh('G', 5);
        stp8Alarm =     IsGpioHigh('C', 10);
        stp8Clock =     IsGpioHigh('G', 3);

        // stepper 9: GP73, GP74, GP85, GP72
        stp9Forward =   IsGpioHigh('G', 7);
        stp9Enable =    IsGpioHigh('G', 8);
        stp9Alarm =     IsGpioHigh('C', 12);
        stp9Clock =     IsGpioHigh('G', 6);

        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append('{');
        jsonBuilder.Append("\"stepper0\":{\"forward\":" + JsonStr(stp0Forward) + ",\"enable\":" + JsonStr(stp0Enable) + ",\"alarm\":" + JsonStr(stp0Alarm) + ",\"clock\":" + JsonStr(stp0Clock) + "},");
        jsonBuilder.Append("\"stepper1\":{\"forward\":" + JsonStr(stp1Forward) + ",\"enable\":" + JsonStr(stp1Enable) + ",\"alarm\":" + JsonStr(stp1Alarm) + ",\"clock\":" + JsonStr(stp1Clock) + "},");
        jsonBuilder.Append("\"stepper2\":{\"forward\":" + JsonStr(stp2Forward) + ",\"enable\":" + JsonStr(stp2Enable) + ",\"alarm\":" + JsonStr(stp2Alarm) + ",\"clock\":" + JsonStr(stp2Clock) + "},");
        jsonBuilder.Append("\"stepper3\":{\"forward\":" + JsonStr(stp3Forward) + ",\"enable\":" + JsonStr(stp3Enable) + ",\"alarm\":" + JsonStr(stp3Alarm) + ",\"clock\":" + JsonStr(stp3Clock) + "},");
        jsonBuilder.Append("\"stepper4\":{\"forward\":" + JsonStr(stp4Forward) + ",\"enable\":" + JsonStr(stp4Enable) + ",\"alarm\":" + JsonStr(stp4Alarm) + ",\"clock\":" + JsonStr(stp4Clock) + "},");
        jsonBuilder.Append("\"stepper5\":{\"forward\":" + JsonStr(stp5Forward) + ",\"enable\":" + JsonStr(stp5Enable) + ",\"alarm\":" + JsonStr(stp5Alarm) + ",\"clock\":" + JsonStr(stp5Clock) + "},");
        jsonBuilder.Append("\"stepper6\":{\"forward\":" + JsonStr(stp6Forward) + ",\"enable\":" + JsonStr(stp6Enable) + ",\"alarm\":" + JsonStr(stp6Alarm) + ",\"clock\":" + JsonStr(stp6Clock) + "},");
        jsonBuilder.Append("\"stepper7\":{\"forward\":" + JsonStr(stp7Forward) + ",\"enable\":" + JsonStr(stp7Enable) + ",\"alarm\":" + JsonStr(stp7Alarm) + ",\"clock\":" + JsonStr(stp7Clock) + "},");
        jsonBuilder.Append("\"stepper8\":{\"forward\":" + JsonStr(stp8Forward) + ",\"enable\":" + JsonStr(stp8Enable) + ",\"alarm\":" + JsonStr(stp8Alarm) + ",\"clock\":" + JsonStr(stp8Clock) + "},");
        jsonBuilder.Append("\"stepper9\":{\"forward\":" + JsonStr(stp9Forward) + ",\"enable\":" + JsonStr(stp9Enable) + ",\"alarm\":" + JsonStr(stp9Alarm) + ",\"clock\":" + JsonStr(stp9Clock) + "}");
        jsonBuilder.Append('}');

        return jsonBuilder.ToString();
    }

    private string GetPositionDetectorStatus()
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

        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append('{');
        jsonBuilder.Append("\"pd0\":" + JsonStr(pd0) + ",");
        jsonBuilder.Append("\"pd1\":" + JsonStr(pd1) + ",");
        jsonBuilder.Append("\"pd2\":" + JsonStr(pd2) + ",");
        jsonBuilder.Append("\"pd3\":" + JsonStr(pd3) + ",");
        jsonBuilder.Append("\"pd4\":" + JsonStr(pd4) + ",");
        jsonBuilder.Append("\"pd5\":" + JsonStr(pd5) + ",");
        jsonBuilder.Append("\"pd6\":" + JsonStr(pd6) + ",");
        jsonBuilder.Append("\"pd7\":" + JsonStr(pd7) + ",");
        jsonBuilder.Append("\"pd8\":" + JsonStr(pd8) + ",");
        jsonBuilder.Append("\"pd9\":" + JsonStr(pd9) + ",");
        jsonBuilder.Append("\"pd10\":" + JsonStr(pd10) + ",");
        jsonBuilder.Append("\"pd11\":" + JsonStr(pd11) + ",");
        jsonBuilder.Append("\"pd12\":" + JsonStr(pd12) + ",");
        jsonBuilder.Append("\"pd13\":" + JsonStr(pd13) + ",");
        jsonBuilder.Append("\"pd14\":" + JsonStr(pd14) + ",");
        jsonBuilder.Append("\"pd15\":" + JsonStr(pd15) + ",");
        jsonBuilder.Append("\"pd16\":" + JsonStr(pd16) + ",");
        jsonBuilder.Append("\"pd17\":" + JsonStr(pd17) + ",");
        jsonBuilder.Append("\"pd18\":" + JsonStr(pd18) + ",");
        jsonBuilder.Append("\"pd19\":" + JsonStr(pd19) + ",");
        jsonBuilder.Append("\"pd20\":" + JsonStr(pd20) + ",");
        jsonBuilder.Append("\"pd21\":" + JsonStr(pd21) + ",");
        jsonBuilder.Append("\"pd22\":" + JsonStr(pd22) + ",");
        jsonBuilder.Append("\"pd23\":" + JsonStr(pd23) + ",");
        jsonBuilder.Append("\"pd24\":" + JsonStr(pd24) + ",");
        jsonBuilder.Append("\"pd25\":" + JsonStr(pd25) + ",");
        jsonBuilder.Append("\"pd26\":" + JsonStr(pd26) + ",");
        jsonBuilder.Append("\"pd27\":" + JsonStr(pd27) + ",");
        jsonBuilder.Append("\"pd28\":" + JsonStr(pd28) + ",");
        jsonBuilder.Append("\"pd29\":" + JsonStr(pd29) + ",");
        jsonBuilder.Append("\"pd30\":" + JsonStr(pd30) + ",");
        jsonBuilder.Append("\"pd31\":" + JsonStr(pd31) + ",");
        jsonBuilder.Append("\"pd32\":" + JsonStr(pd32) + ",");
        jsonBuilder.Append("\"pd33\":" + JsonStr(pd33) + ",");
        jsonBuilder.Append("\"pd34\":" + JsonStr(pd34) + ",");
        jsonBuilder.Append("\"pd35\":" + JsonStr(pd35) + ",");
        jsonBuilder.Append("\"pd36\":" + JsonStr(pd36) + ",");
        jsonBuilder.Append("\"pd37\":" + JsonStr(pd37) + ",");
        jsonBuilder.Append("\"pd38\":" + JsonStr(pd38) + ",");
        jsonBuilder.Append("\"pd39\":" + JsonStr(pd39) + ",");
        jsonBuilder.Append("\"pd40\":" + JsonStr(pd40) + ",");
        jsonBuilder.Append("\"pd41\":" + JsonStr(pd41) + ",");
        jsonBuilder.Append("\"pd42\":" + JsonStr(pd42) + ",");
        jsonBuilder.Append("\"pd43\":" + JsonStr(pd43) + ",");
        jsonBuilder.Append("\"pd44\":" + JsonStr(pd44) + ",");
        jsonBuilder.Append("\"pd45\":" + JsonStr(pd45) + ",");
        jsonBuilder.Append("\"pd46\":" + JsonStr(pd46) + ",");
        jsonBuilder.Append("\"pd47\":" + JsonStr(pd47) + ",");
        jsonBuilder.Append("\"pd48\":" + JsonStr(pd48));
        jsonBuilder.Append('}');

        return jsonBuilder.ToString();
    }

    private bool IsGpioHigh(char port, uint bitIndex)
    {
        var portIndex = port - 'A';
        Debug.Assert(portIndex >= 0);
        Debug.Assert(portIndex <= 10);
        Debug.Assert(reply != null);

        ushort portValue = reply[1 + portIndex * 2 + 1];
        portValue <<= 8;
        portValue += reply[1 + portIndex * 2];

        if((portValue & (1 << (int)bitIndex)) == 0)
            return false;
        else
            return true;
    }

    private string BridgeControlString(bool input1, bool input2)
    {
        if (input1)
        {
            if(input2)
                return "Brake";
            else
                return "Forward";
        }
        else
        {
            if(input2)
                return "Reverse";
            else
                return "Coast";
        }
    }

    private string JsonStr(bool value)
    {
        if(value)
            return "true";
        else
            return "false";
    }

}



}
