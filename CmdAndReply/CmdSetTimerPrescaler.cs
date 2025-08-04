
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
            }
            
            return (true, "");
        }
    }
}