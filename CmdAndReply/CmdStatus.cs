
using System;
using System.Data;
using System.Diagnostics;

namespace VirtualOperatorServer.CommandAndReply
{
    class CmdGetStatus : CommandAndReply
    {
        public static readonly Stopwatch monoClock = Stopwatch.StartNew();

        public class CStatus
        {
            public class TimerData
            {
                public byte state;
                public ushort prescaler;
            }

            public class StepperData
            {
                public byte state;
                public bool isForward;
                public bool isEnabled;
                public bool endBoundaryTouched;
                public bool homeBoundaryTouched;
                public bool isEnableHigh;
                public bool isForwardHigh;
                public bool isRisingEdgeDriven;
                public bool isPassiveStepsPopulated;
                public bool isRampdownPopulated;
                public bool isCruisePopulated;
                public bool isRampupPopulated;
                public uint offset;
                public uint encoderOffset;
                public byte maxEncoderOffsetError;
            }

            public TimeSpan timeSpan = monoClock.Elapsed;
            public const int PortCount = 11;
            public const int EncoderCount = 8;
            public const int TimerCount = 7;
            public const int StepperCount = 10;

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

            public TimerData[] timersData;
            public StepperData[] steppersData;

            public CStatus()
            {
                timersData = new TimerData[TimerCount];
                steppersData = new StepperData[StepperCount];

                for (int i = 0; i < TimerCount; i++)
                    timersData[i] = new TimerData();

                for (int i = 0; i < StepperCount; i++)
                    steppersData[i] = new StepperData();
            }
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

            if (reply.Length != 189)
            {
                return (false, $"Wrong length of status reply: {reply.Length}");
            }

            var cmd = Command;
            if (cmd[0] != reply[0])
            {
                return (false, $"Wrong reply for cmd: {reply[0]}");
            }
            if (reply[1] != 0)
            {
                return (false, $"Error code {reply[1]}");
            }

            var status = new CStatus();
            byte offset;
            byte b0, b1, b2, b3;
            ushort value;

            // gpio ports, 2 - 23
            offset = 2;
            for (int i = 0; i < CStatus.PortCount; i++)
            {
                b0 = (byte)reply[offset];
                b1 = (byte)reply[offset + 1];
                value = b1;
                value = (ushort)((value << 8) + b0);
                status.ports[i] = value;
                offset += 2;
            }

            // encoders, 24 - 39
            offset = 24;
            for (int i = 0; i < CStatus.EncoderCount; i++)
            {
                b0 = (byte)reply[offset];
                b1 = (byte)reply[offset + 1];
                value = b1;
                value = (ushort)((value << 8) + b0);
                status.encoders[i] = value;
                offset += 2;
            }

            // main loop count, 40 - 43
            b0 = reply[40];
            b1 = reply[41];
            b2 = reply[42];
            b3 = reply[43];
            status.mainLoopCount = b3;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b2;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b1;
            status.mainLoopCount <<= 8;
            status.mainLoopCount += b0;

            // flex ISR period, 44 - 45
            b0 = reply[44];
            b1 = reply[45];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.maxFlexTimerIsrPeriod = value;

            // fixed ISR period, 46 - 47
            b0 = reply[46];
            b1 = reply[47];
            value = b1;
            value = (ushort)((value << 8) + b0);
            status.maxFixTimerIsrPeriod = value;

            // timer data, 48 - 68
            offset = 48;
            for (int i = 0; i < CStatus.TimerCount; i++)
            {
                value = reply[offset + 2];
                value <<= 8;
                value += reply[offset + 1];

                status.timersData[i].state = reply[offset];
                status.timersData[i].prescaler = value;

                offset += 3;
            }

            // stepper data, 69 - 188
            offset = 69;
            for (int i = 0; i < CStatus.StepperCount; i++)
            {
                status.steppersData[i].state = reply[offset];

                b0 = reply[offset + 1];
                status.steppersData[i].isForward = (b0 & 0x01) != 0;
                status.steppersData[i].isEnabled = (b0 & 0x02) != 0;
                status.steppersData[i].endBoundaryTouched = (b0 & 0x04) != 0;
                status.steppersData[i].homeBoundaryTouched = (b0 & 0x08) != 0;
                status.steppersData[i].isEnableHigh = (b0 & 0x10) != 0;
                status.steppersData[i].isForwardHigh = (b0 & 0x20) != 0;
                status.steppersData[i].isRisingEdgeDriven = (b0 & 0x40) != 0;

                b0 = reply[offset + 2];
                status.steppersData[i].isPassiveStepsPopulated = (b0 & 0x01) != 0;
                status.steppersData[i].isRampdownPopulated = (b0 & 0x02) != 0;
                status.steppersData[i].isCruisePopulated = (b0 & 0x04) != 0;
                status.steppersData[i].isRampupPopulated = (b0 & 0x08) != 0;

                uint tmpInt = reply[offset + 6];
                tmpInt <<= 8;
                tmpInt += reply[offset + 5];
                tmpInt <<= 8;
                tmpInt += reply[offset + 4];
                tmpInt <<= 8;
                tmpInt += reply[offset + 3];
                status.steppersData[i].offset = tmpInt;

                tmpInt = reply[offset + 10];
                tmpInt <<= 8;
                tmpInt += reply[offset + 9];
                tmpInt <<= 8;
                tmpInt += reply[offset + 8];
                tmpInt <<= 8;
                tmpInt += reply[offset + 7];
                status.steppersData[i].encoderOffset = tmpInt;

                status.steppersData[i].maxEncoderOffsetError = reply[offset + 11];

                offset += 12;
            }

            Status = status;
            return (true, "");
        }
    }
}
