
namespace VirtualOperatorServer.CommandAndReply
{

    class CmdSetTimerPrescaler : CommandAndReply
    {
        public const int TIMER_COUNT = 7;
        private const int REPLY_LENGTH = 2;
        static private byte[] CreateCommand(ushort[] prescalers)
        {
            if (prescalers.Length != TIMER_COUNT)
            {
                throw new InvalidRequestBodyException($"Wrong length of prescalers: {prescalers.Length}");
            }

            byte[] cmd = new byte[1 + 2 * TIMER_COUNT];

            cmd[0] = (byte)CommandEnum.SET_TIMER_PRESCALER;

            for (int i = 0; i < prescalers.Length; i++)
            {
                cmd[1 + i * 2] = (byte)prescalers[i];
                cmd[1 + i * 2 + 1] = (byte)(prescalers[i] >> 8);
            }

            return cmd;
        }

        public CmdSetTimerPrescaler(ushort[] prescalers) : base(CreateCommand(prescalers))
        {
        }

        public override (bool result, string reason) ParseReply()
        {
            if (reply == null)
            {
                return (false, "No reply is received");
            }
            if (reply.Length != REPLY_LENGTH)
            {
                return (false, "Invalid reply");
            }

            byte errorCode = reply[1];

            switch (errorCode)
            {
                case 0:
                    return (true, "");
                case 1:
                    return (false, "invalid prescaler length");
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    return (false, $"timer_set_prescaler failure for timer {errorCode - 2}");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdTestTimer : CommandAndReply
    {
        public const int TIMER_COUNT = 7;
        private const int REPLY_LENGTH = 2;
        static private byte[] CreateCommand(byte timerId, ushort pulseWidth, ushort totoalPulse, ushort logPeriod)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	timer id
            * 2: 	1/2 pulse width
            * 3:	2/2 pulse width
            * 4:	1/2 total pulse
            * 5:	2/2 total pulse
            * 6:	1/2 log interval
            * 7:	2/2 log interval
            */

            if (timerId >= TIMER_COUNT)
            {
                throw new InvalidRequestBodyException($"Wrong length of prescalers: {timerId}");
            }

            byte[] cmd = new byte[8];

            cmd[0] = (byte)CommandEnum.TEST_TIMER;
            cmd[1] = timerId;
            cmd[2] = (byte)pulseWidth;
            cmd[3] = (byte)(pulseWidth >> 8);
            cmd[4] = (byte)totoalPulse;
            cmd[5] = (byte)(totoalPulse >> 8);
            cmd[6] = (byte)logPeriod;
            cmd[7] = (byte)(logPeriod >> 8);

            return cmd;
        }

        public CmdTestTimer(byte timerId, ushort pulseWidth, ushort totoalPulse, ushort logPeriod) :
            base(CreateCommand(timerId, pulseWidth, totoalPulse, logPeriod))
        {
        }

        public override (bool result, string reason) ParseReply()
        {
            if (reply == null)
            {
                return (false, "No reply is received");
            }
            if (reply.Length != REPLY_LENGTH)
            {
                return (false, "Invalid reply");
            }

            byte errorCode = reply[1];

            switch (errorCode)
            {
                case 0:
                    return (true, "");
                case 1:
                    return (false, "invalid command length");
                case 2:
                    return (false, "timer_test failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

}