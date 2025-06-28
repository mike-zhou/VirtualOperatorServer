
namespace VirtualOperatorServer.CommandAndReply
{
    class CmdGetStatus : CommandAndReply
    {
        public class CStatus
        {
            public const int PortCount = 11;
            public const int EncoderCount = 8;

            // gpio ports
            public ushort[] ports = new ushort[PortCount];

            // encoders
            public ushort[] encoders = new ushort[EncoderCount];

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
            for (int i = 0; i < CStatus.PortCount; i++)
            {
                b0 = (byte)reply[i * 2 + 1];
                b1 = (byte)reply[i * 2 + 2];
                value = b1;
                value = (ushort)((value << 8) + b0);
                status.ports[i] = value;
            }

            // encoders, 23 - 38
            for (int i = 0; i < CStatus.EncoderCount; i++)
            {
                b0 = (byte)reply[i * 2 + 23];
                b1 = (byte)reply[i * 2 + 24];
                value = b1;
                value = (ushort)((value << 8) + b0);
                status.encoders[i] = value;
            }

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
