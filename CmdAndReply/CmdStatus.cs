
namespace VirtualOperatorServer.CommandAndReply
{
    class CmdGetStatus : CommandAndReply
    {
        public class CStatus
        {
            // gpio ports
            public ushort portA;
            public ushort portB;
            public ushort portC;
            public ushort portD;
            public ushort portE;
            public ushort portF;
            public ushort portG;
            public ushort portH;
            public ushort portI;
            public ushort portJ;
            public ushort portK;

            // encoders
            public ushort encoder0;
            public ushort encoder1;
            public ushort encoder2;
            public ushort encoder3;
            public ushort encoder4;
            public ushort encoder5;
            public ushort encoder6;
            public ushort encoder7;

            // main loop count
            public uint mainLoopCount;

            // max flex timer ISR period
            public ushort maxFlexTimerIsrPeriod;

            // max fix timer ISR period
            public ushort maxFixTimerIsrPeriod;
        }

        static private byte[] CreateCommand()
        {
            byte[] cmd = new byte[1];
            cmd[0] = (byte)CommandEnum.GET_STATUS;

            return cmd;
        }

        public CmdGetStatus() : base(CreateCommand()) { }


        public static CStatus? Status { get; private set; } = null;

        public override (bool result, string reason) ParseReply()
        {
            if (reply == null)
            {
                return (false, "No status is received");
            }

            if (reply.Length < 47)
            {
                return (false, "Invalid status reply");
            }

            var cmd = Command;
            if (cmd[0] != reply[0])
            {
                return (false, "Wrong reply");
            }

            var status = new CStatus();
            byte b0, b1, b2, b3;
            ushort value;

            // gpio ports, 1 - 22
            b0 = reply[1];
            b1 = reply[2];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portA = value;

            b0 = reply[3];
            b1 = reply[4];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portB = value;

            b0 = reply[5];
            b1 = reply[6];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portC = value;

            b0 = reply[7];
            b1 = reply[8];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portD = value;

            b0 = reply[9];
            b1 = reply[10];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portE = value;

            b0 = reply[11];
            b1 = reply[12];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portF = value;

            b0 = reply[13];
            b1 = reply[14];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portG = value;

            b0 = reply[15];
            b1 = reply[16];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portH = value;

            b0 = reply[17];
            b1 = reply[18];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portI = value;

            b0 = reply[19];
            b1 = reply[20];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portJ = value;

            b0 = reply[21];
            b1 = reply[22];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.portK = value;

            // encoders, 23 - 38
            b0 = reply[23];
            b1 = reply[24];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder0 = value;

            b0 = reply[25];
            b1 = reply[26];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder1 = value;

            b0 = reply[27];
            b1 = reply[28];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder2 = value;

            b0 = reply[29];
            b1 = reply[30];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder3 = value;

            b0 = reply[31];
            b1 = reply[32];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder4 = value;

            b0 = reply[33];
            b1 = reply[34];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder5 = value;

            b0 = reply[35];
            b1 = reply[36];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder6 = value;

            b0 = reply[37];
            b1 = reply[38];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.encoder7 = value;

            // main loop count, 39 - 42
            b0 = reply[39];
            b1 = reply[40];
            b2 = reply[41];
            b3 = reply[42];
            status.mainLoopCount = b3;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b2;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b1;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b0;

            // flex ISR period, 43 - 44
            b0 = reply[43];
            b1 = reply[44];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.maxFlexTimerIsrPeriod = value;

            // fixed ISR period, 45 - 46
            b0 = reply[45];
            b1 = reply[46];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.maxFixTimerIsrPeriod = value;

            Status = status;

            return (true, "");
        }
    }
}
