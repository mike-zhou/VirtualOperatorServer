namespace VirtualOperatorServer.CommandAndReply
{
    class CmdSetStepperActiveRampupPulseWidth(byte stepperId, byte batchIndex, byte totalBatches, ushort[] widths) :
        CommandAndReply(CreateCommand(stepperId, batchIndex, totalBatches, widths))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte batchIndex, byte totalBatches, ushort[] widths)
        {
            /**
            * cmd is in the format:
            * 	byte:	command ID
            * 	byte: 	stepper ID
            * 	byte: 	batch index
            * 	byte: 	total batches
            * 	byte: 	pulse0 low byte
            * 	byte:	pulse0 high byte
            * 	byte:	...
            * 	byte: 	pulseN high byte
            */
            byte[] cmd = new byte[1 + 1 + 1 + 1 + 2 * widths.Length];

            if (cmd.Length >= 255)
            {
                throw new InvalidRequestBodyException($"too many widths: {widths.Length}");
            }

            cmd[0] = (byte)CommandEnum.SET_STEPPER_ACTIVE_RAMPUP_PULSE_WIDTH;
            cmd[1] = stepperId;
            cmd[2] = batchIndex;
            cmd[3] = totalBatches;

            for (int i = 0; i < widths.Length; i++)
            {
                cmd[4 + i * 2] = (byte)widths[i];
                cmd[4 + i * 2 + 1] = (byte)(widths[i] >> 8);
            }

            return cmd;
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
                    return (false, "command is too long");
                case 2:
                    return (false, "command is too short");
                case 3:
                    return (false, "widths are wrongly aligned");
                case 4:
                    return (false, "stepper_set_active_rampup_pulse_widths() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }


    class CmdSetStepperActiveRampdownPulseWidth(byte stepperId, byte batchIndex, byte totalBatches, ushort[] widths) :
        CommandAndReply(CreateCommand(stepperId, batchIndex, totalBatches, widths))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte batchIndex, byte totalBatches, ushort[] widths)
        {
            /**
            * cmd is in the format:
            * 	byte:	command ID
            * 	byte: 	stepper ID
            * 	byte: 	batch index
            * 	byte: 	total batches
            * 	byte: 	pulse0 low byte
            * 	byte:	pulse0 high byte
            * 	byte:	...
            * 	byte: 	pulseN high byte
            */
            byte[] cmd = new byte[1 + 1 + 1 + 1 + 2 * widths.Length];

            if (cmd.Length >= 255)
            {
                throw new InvalidRequestBodyException($"too many widths: {widths.Length}");
            }

            cmd[0] = (byte)CommandEnum.SET_STEPPER_ACTIVE_RAMPDOWN_PULSE_WIDTH;
            cmd[1] = stepperId;
            cmd[2] = batchIndex;
            cmd[3] = totalBatches;

            for (int i = 0; i < widths.Length; i++)
            {
                cmd[4 + i * 2] = (byte)widths[i];
                cmd[4 + i * 2 + 1] = (byte)(widths[i] >> 8);
            }

            return cmd;
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
                    return (false, "command is too long");
                case 2:
                    return (false, "command is too short");
                case 3:
                    return (false, "widths are wrongly aligned");
                case 4:
                    return (false, "stepper_set_active_rampdown_pulse_widths() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }


    class CmdSetStepperActiveCruisePulseWidth(byte stepperId, ushort width) :
        CommandAndReply(CreateCommand(stepperId, width))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, ushort width)
        {
            /**
            * cmd is in the format:
            * 	byte:	command ID
            * 	byte: 	stepper ID
            * 	byte: 	pulse low byte
            * 	byte:	pulse high byte
            */
            byte[] cmd = new byte[4];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_ACTIVE_CRUISE_PULSE_WIDTH;
            cmd[1] = stepperId;
            cmd[2] = (byte)width;
            cmd[3] = (byte)(width >> 8);

            return cmd;
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
                    return (false, "invalid length");
                case 2:
                    return (false, "stepper_set_active_cruise_pulse_width() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }


    class CmdSetStepperPassiveStepIndexes(byte stepperId, byte batchIndex, byte totalBatches, ushort[] stepIndexes) :
        CommandAndReply(CreateCommand(stepperId, batchIndex, totalBatches, stepIndexes))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte batchIndex, byte totalBatches, ushort[] stepIndexes)
        {
            /**
            * cmd is in the format:
            * 	0:	command ID
            * 	1: 	stepper ID
            * 	2: 	batch index
            * 	3: 	total batches
            * 	4: 	1/2 index0
            * 	5:	2/2 index0
            * 	 :	...
            * 	n: 	2/2 indexN
            */
            byte[] cmd = new byte[1 + 1 + 1 + 1 + 2 * stepIndexes.Length];

            if (cmd.Length >= 255)
            {
                throw new InvalidRequestBodyException($"too many indexes: {stepIndexes.Length}");
            }

            cmd[0] = (byte)CommandEnum.SET_STEPPER_PASSIVE_STEP_INDEXES;
            cmd[1] = stepperId;
            cmd[2] = batchIndex;
            cmd[3] = totalBatches;

            for (int i = 0; i < stepIndexes.Length; i++)
            {
                cmd[4 + i * 2] = (byte)stepIndexes[i];
                cmd[4 + i * 2 + 1] = (byte)(stepIndexes[i] >> 8);
            }

            return cmd;
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
                    return (false, "command is too long");
                case 2:
                    return (false, "command is too short");
                case 3:
                    return (false, "indexes are wrongly aligned");
                case 4:
                    return (false, "stepper_set_passive_step_indexes() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }


    class CmdSetStepperControls(byte stepperId,
                                bool isRisingEdgeDriven,
                                bool isForwardHigh,
                                bool isEnableHigh,
                                byte gpioPortIndexHomeBoundary,
                                byte gpioPinIndexHomeBoundary,
                                byte gpioPortIndexEndBoundary,
                                byte gpioPinIndexEndBoudnary,
                                byte gpioPortIndexEnable,
                                byte gpioPinIndexEnable,
                                byte gpioPortIndexForward,
                                byte gpioPinIndexForward,
                                byte gpioPortIndexClock,
                                byte gpioPinIndexClock,
                                ushort stepsHomeBoundaryToReady,
                                uint range,
                                ushort stepsPerRotation,
                                byte encoderId,
                                ushort encoderCountsPerRotation,
                                ushort encoderOffsetErrorThreshold) :
                CommandAndReply(CreateCommand(stepperId,
                                isRisingEdgeDriven,
                                isForwardHigh,
                                isEnableHigh,
                                gpioPortIndexHomeBoundary,
                                gpioPinIndexHomeBoundary,
                                gpioPortIndexEndBoundary,
                                gpioPinIndexEndBoudnary,
                                gpioPortIndexEnable,
                                gpioPinIndexEnable,
                                gpioPortIndexForward,
                                gpioPinIndexForward,
                                gpioPortIndexClock,
                                gpioPinIndexClock,
                                stepsHomeBoundaryToReady,
                                range,
                                stepsPerRotation,
                                encoderId,
                                encoderCountsPerRotation,
                                encoderOffsetErrorThreshold))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId,
                                            bool isRisingEdgeDriven,
                                            bool isForwardHigh,
                                            bool isEnableHigh,
                                            byte gpioPortIndexHomeBoundary,
                                            byte gpioPinIndexHomeBoundary,
                                            byte gpioPortIndexEndBoundary,
                                            byte gpioPinIndexEndBoudnary,
                                            byte gpioPortIndexEnable,
                                            byte gpioPinIndexEnable,
                                            byte gpioPortIndexForward,
                                            byte gpioPinIndexForward,
                                            byte gpioPortIndexClock,
                                            byte gpioPinIndexClock,
                                            ushort stepsHomeBoundaryToReady,
                                            uint range,
                                            ushort stepsPerRotation,
                                            byte encoderId,
                                            ushort encoderCountsPerRotation,
                                            ushort encoderOffsetErrorThreshold)
        {
            /**
            * cmd is in the format:
            *  0: 	command id
            * 	1: 	stepper id
            * 	2: 	is rising edge driven
            * 	3:	is forward high
            * 	4:	is enable high
            * 	5:	gpio port index of home boundary 
            * 	6: 	gpio pin index of home boundary
            * 	7:	gpio port index of end boundary
            * 	8:	gpio pin index of end boundary
            * 	9:	gpio port index of enable signal
            * 	10:	gpio pin index of enable signal
            * 	11:	gpio port index of forward signal
            * 	12:	gpio pin index of forward signal
            * 	13:	gpio port index of clock signal
            * 	14:	gpio pin idnex of clock signal
            * 	15:	1/2 of steps from home boundary to ready
            * 	16:	2/2 of steps from home boundary to ready
            * 	17: 1/4 of range
            * 	18: 2/4 of range
            * 	19:	3/4 of range
            * 	20: 4/4 of range
            * 	21: 1/2 of steps when the stepper turns a round
            * 	22: 2/2 of steps when the stepper turns a round
            * 	23: encoder id
            * 	24: 1/2 of counts when the encoder turns a round
            * 	25: 2/2 of counts when the encoder turns a round
            * 	26: 1/2 of max allowable difference between actual position and ideal position
            * 	27: 2/2 of max allowable difference between actual position and ideal position
            */
            byte[] cmd = new byte[28];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_CONTROLS;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isRisingEdgeDriven ? 1 : 0);
            cmd[3] = (byte)(isForwardHigh ? 1 : 0);
            cmd[4] = (byte)(isEnableHigh ? 1 : 0);
            cmd[5] = gpioPortIndexHomeBoundary;
            cmd[6] = gpioPinIndexHomeBoundary;
            cmd[7] = gpioPortIndexEndBoundary;
            cmd[8] = gpioPinIndexEndBoudnary;
            cmd[9] = gpioPortIndexEnable;
            cmd[10] = gpioPinIndexEnable;
            cmd[11] = gpioPortIndexForward;
            cmd[12] = gpioPinIndexForward;
            cmd[13] = gpioPortIndexClock;
            cmd[14] = gpioPinIndexClock;
            cmd[15] = (byte)(stepsHomeBoundaryToReady);
            cmd[16] = (byte)(stepsHomeBoundaryToReady >> 8);
            cmd[17] = (byte)range;
            cmd[18] = (byte)(range >> 8);
            cmd[19] = (byte)(range >> 16);
            cmd[20] = (byte)(range >> 24);
            cmd[21] = (byte)stepsPerRotation;
            cmd[22] = (byte)(stepsPerRotation >> 8);
            cmd[23] = encoderId;
            cmd[24] = (byte)encoderCountsPerRotation;
            cmd[25] = (byte)(encoderCountsPerRotation >> 8);
            cmd[26] = (byte)encoderOffsetErrorThreshold;
            cmd[27] = (byte)(encoderOffsetErrorThreshold >> 8);

            return cmd;
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
                    return (false, "invalid stepper id");
                case 2:
                    return (false, "gpio port index of home boundary is out of range");
                case 3:
                    return (false, "gpio pin index of home boundary is out of range");
                case 4:
                    return (false, "gpio port index of end boundary is out of range");
                case 5:
                    return (false, "gpio pin index of end boundary is out of range");
                case 6:
                    return (false, "gpio port index of enable is out of range");
                case 7:
                    return (false, "gpio pin index of enable is out of range");
                case 8:
                    return (false, "gpio port index of forward is out of range");
                case 9:
                    return (false, "gpio pin index of forward is out of range");
                case 10:
                    return (false, "gpio port index of clock is out of range");
                case 11:
                    return (false, "gpio pin index of clock is out of range");
                case 12:
                    return (false, "invalid encoder id");
                case 13:
                    return (false, "stepper_set_controls() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdSetStepperEnable(byte stepperId, bool isEnable) :
        CommandAndReply(CreateCommand(stepperId, isEnable))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, bool isEnable)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2:	enable stepper
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_ENABLE;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isEnable ? 1 : 0);

            return cmd;
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
                    return (false, "stepper_set_enable() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdSetStepperCrossBoundary(byte stepperId, CmdSetStepperCrossBoundary.CrossBoudanry crossBoundary) :
        CommandAndReply(CreateCommand(stepperId, crossBoundary))
    {
        private const int REPLY_LENGTH = 2;

        public struct CrossBoundaryItem
        {
            public bool enabled;
            public int offset;
            public ushort error;

            public CrossBoundaryItem()
            {
                enabled = false;
                offset = 0;
                error = 0;
            }  
        }

        public struct CrossBoudanry
        {
            public bool enabled;
            public int negativeRange;
            public CrossBoundaryItem[] items;

            public CrossBoudanry()
            {
                enabled = false;
                negativeRange = 0;
                items = new CrossBoundaryItem[4];
            }
        }

        static private byte[] CreateCommand(byte stepperId, CmdSetStepperCrossBoundary.CrossBoudanry crossBoundary)
        {
            /**
            * command format:
            * 0: command id
            * 1: stepper id
            * 2: crossBoundaryEnabled
            * 3: 1/4 negativeRange
            * 4: 2/4 negativeRange
            * 5: 3/4 negativeRange
            * 6: 4/4 negativeRange
            * 7: item0 enabled
            * 8: item0 1/4 offset
            * 9: item0 2/4 offset
            * 10: item0 3/4 offset
            * 11: item0 4/4 offset
            * 12: item0 1/2 error
            * 13: item0 2/2 error
            * 14: item1 enabled
            * 15: item1 1/4 offset
            * 16: item1 2/4 offset
            * 17: item1 3/4 offset
            * 18: item1 4/4 offset
            * 19: item1 1/2 error
            * 20: item1 2/2 error
            * 21: item2 enabled
            * 22: item2 1/4 offset
            * 23: item2 2/4 offset
            * 24: item2 3/4 offset
            * 25: item2 4/4 offset
            * 26: item2 1/2 error
            * 27: item2 2/2 error
            * 28: item3 enabled
            * 29: item3 1/4 offset
            * 30: item3 2/4 offset
            * 31: item3 3/4 offset
            * 32: item3 4/4 offset
            * 33: item3 1/2 error
            * 34: item3 2/2 error
            */

            byte[] cmd = new byte[35];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_CROSS_BOUNDARY;
            cmd[1] = stepperId;
            cmd[2] = (byte)(crossBoundary.enabled ? 1 : 0);
            cmd[3] = (byte)crossBoundary.negativeRange;
            cmd[4] = (byte)(crossBoundary.negativeRange >> 8);
            cmd[5] = (byte)(crossBoundary.negativeRange >> 16);
            cmd[6] = (byte)(crossBoundary.negativeRange >> 24);

            for (int i = 0; i < 4; i++)
            {
                CrossBoundaryItem item = crossBoundary.items[i];
                int baseIndex = 7 + i * 7;

                cmd[baseIndex] = (byte)(item.enabled ? 1 : 0);
                cmd[baseIndex + 1] = (byte)item.offset;
                cmd[baseIndex + 2] = (byte)(item.offset >> 8);
                cmd[baseIndex + 3] = (byte)(item.offset >> 16);
                cmd[baseIndex + 4] = (byte)(item.offset >> 24);
                cmd[baseIndex + 5] = (byte)item.error;
                cmd[baseIndex + 6] = (byte)(item.error >> 8);
            }

            return cmd;
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
                    return (false, "stepper_set_crossBoundary() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdSetStepperForward(byte stepperId, bool isForward) :
        CommandAndReply(CreateCommand(stepperId, isForward))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, bool isForward)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2:	enable stepper
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_FORWARD;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isForward ? 1 : 0);

            return cmd;
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
                    return (false, "stepper_set_forward() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdStartStepperHomePositioning(byte stepperId, byte timerId) :
        CommandAndReply(CreateCommand(stepperId, timerId))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte timerId)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2: 	timer id
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.START_STEPPER_HOME_POSITIONING;
            cmd[1] = stepperId;
            cmd[2] = timerId;

            return cmd;
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
                    return (false, "stepper_start_home_positioning() failed");
                case 3:
                    return (false, "stepper_get_startup_pulse_width() failed");
                case 4:
                    return (false, "timer_start() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdRunStepperForce(byte stepperId, byte timerId, ushort pulseWidth, ushort steps) :
        CommandAndReply(CreateCommand(stepperId, timerId, pulseWidth, steps))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte timerId, ushort pulseWidth, ushort steps)
        {
            /**
            * command format:
            * 0:	command id
            * 1: 	stepper id
            * 2: 	timer id
            * 3:	1/2 pulse width
            * 4:	2/2 pulse width
            * 5: 	1/2 steps
            * 6:	2/2 steps
            */
            byte[] cmd = new byte[7];

            cmd[0] = (byte)CommandEnum.RUN_STEPPER_FORCE;
            cmd[1] = stepperId;
            cmd[2] = timerId;
            cmd[3] = (byte)pulseWidth;
            cmd[4] = (byte)(pulseWidth >> 8);
            cmd[5] = (byte)steps;
            cmd[6] = (byte)(steps >> 8);

            return cmd;
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
                    return (false, "stepper_run_force() failed");
                case 3:
                    return (false, "stepper_get_startup_pulse_width() failed");
                case 4:
                    return (false, "timer_start() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdRunStepperPasive(byte stepperId, byte activeStepperId) :
        CommandAndReply(CreateCommand(stepperId, activeStepperId))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte activeStepperId)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2:	active stepper id
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.RUN_STEPPER_PASSIVE;
            cmd[1] = stepperId;
            cmd[2] = activeStepperId;

            return cmd;
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
                    return (false, "stepper_couple_passive() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdSetStepperActive(byte stepperId, uint steps) :
        CommandAndReply(CreateCommand(stepperId, steps))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, uint steps)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2:	1/4 steps
            * 3:	2/4 steps
            * 4:	3/4 steps
            * 5:	4/4 steps
            */
            byte[] cmd = new byte[6];

            cmd[0] = (byte)CommandEnum.SET_STEPPER_ACTIVE;
            cmd[1] = stepperId;
            cmd[2] = (byte)steps;
            cmd[3] = (byte)(steps >> 8);
            cmd[4] = (byte)(steps >> 16);
            cmd[5] = (byte)(steps >> 24);

            return cmd;
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
                    return (false, "stepper_run_active() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdTestStepperSignalEnable(byte stepperId, bool isEnable) :
        CommandAndReply(CreateCommand(stepperId, isEnable))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, bool isEnable)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            * 2: 	isEnable
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_SIGNAL_ENABLE;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isEnable ? 1 : 0);

            return cmd;
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
                    return (false, "stepper_test_enable() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdTestStepperSignalForward(byte stepperId, bool isForward) :
        CommandAndReply(CreateCommand(stepperId, isForward))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, bool isForward)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            * 2: 	isForward
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_SIGNAL_FORWARD;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isForward ? 1 : 0);

            return cmd;
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
                    return (false, "stepper_test_forward() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdTestStepperSignalClock(byte stepperId, bool isFirstHalf) :
        CommandAndReply(CreateCommand(stepperId, isFirstHalf))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, bool isFirstHalf)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            * 2: 	isFirstHalf
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_SIGNAL_CLOCK;
            cmd[1] = stepperId;
            cmd[2] = (byte)(isFirstHalf ? 1 : 0);

            return cmd;
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
                    return (false, "stepper_test_clock() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdTestStepperPulseEnd(byte stepperId) :
        CommandAndReply(CreateCommand(stepperId))
    {
        private const int REPLY_LENGTH = 5;

        static private byte[] CreateCommand(byte stepperId)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            */
            byte[] cmd = new byte[2];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_PULSE_END;
            cmd[1] = stepperId;

            return cmd;
        }

        public override (bool result, string reason) ParseReply()
        {
            /**
            * reply format:
            * 0: 	command id
            * 1:	error code
            * 2:	StepperReturnCode
            * 3:	1/2 next pulse width
            * 4:	2/2 next pulse width
            */
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
                    StepperReturnCode = reply[2];
                    NextPulseWidth = (ushort)(reply[4] * 256 + reply[3]);
                    return (true, "");
                case 1:
                    return (false, "invalid command length");
                case 2:
                    StepperReturnCode = reply[2];
                    NextPulseWidth = (ushort)(reply[4] * 256 + reply[3]);
                    return (false, "on_interupt_stepper_pulse_end() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }

        public byte? StepperReturnCode { get; private set; } = null;
        public ushort? NextPulseWidth { get; private set; } = null;
    }
    
    /// <summary>
    /// Set stepper to STEPPER_STATE_RUNNING_FORCED for testing
    /// </summary>
    /// <param name="stepperId">stepper id</param>
    /// <param name="pulseWidth">pulse width</param>
    /// <param name="steps">steps to run</param>
    class CmdTestStepperStateRunningForce(byte stepperId, ushort pulseWidth, ushort steps) :
        CommandAndReply(CreateCommand(stepperId, pulseWidth, steps))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, ushort pulseWidth, ushort steps)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            * 2:	1/2 pulse width
            * 3:	2/2 pulse width
            * 4:	1/2 steps
            * 5:	2/2 steps
            */

            /**
            * reply format:
            * 0: 	command id
            * 1:	error code
            */

            byte[] cmd = new byte[6];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_STATE_RUNNING_FORCE;
            cmd[1] = stepperId;
            cmd[2] = (byte)pulseWidth;
            cmd[3] = (byte)(pulseWidth >> 8);
            cmd[4] = (byte)steps;
            cmd[5] = (byte)(steps >> 8);

            return cmd;
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
                    return (false, "stepper_run_force() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    /// <summary>
    /// Set stepper to STEPPER_STATE_READY for testing
    /// </summary>
    /// <param name="stepperId">stepper id</param>
    class CmdTestStepperStateReady(byte stepperId) :
        CommandAndReply(CreateCommand(stepperId))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            */

            /**
            * reply format:
            * 0: 	command id
            * 1:	error code
            */

            byte[] cmd = new byte[2];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_STATE_READY;
            cmd[1] = stepperId;

            return cmd;
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
                    return (false, "stepper_test_state_ready() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    /// <summary>
    /// Set stepper to STEPPER_STATE_RUNNING_ACTIVE for testing
    /// </summary>
    /// <param name="stepperId"></param>
    /// <param name="steps"></param>
    class CmdTestStepperStateRunningActive(byte stepperId, uint steps) :
        CommandAndReply(CreateCommand(stepperId, steps))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, uint steps)
        {
            /**
            * command format:
            * 0: 	command id
            * 1:	stepper id
            * 2:	1/4 steps
            * 3:	2/4 steps
            * 4:	3/4 steps
            * 5:	4/4 steps
            */

            /**
            * reply format:
            * 0: 	command id
            * 1:	error code
            */

            byte[] cmd = new byte[6];

            cmd[0] = (byte)CommandEnum.TEST_STEPPER_STATE_RUNNING_ACTIVE;
            cmd[1] = stepperId;
            cmd[2] = (byte)steps;
            cmd[3] = (byte)(steps >> 8);
            cmd[4] = (byte)(steps >> 16);
            cmd[5] = (byte)(steps >> 24);

            return cmd;
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
                    return (false, "stepper_run_active() failed");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

    class CmdRunStepperActive(byte stepperId, byte timerId) :
        CommandAndReply(CreateCommand(stepperId, timerId))
    {
        private const int REPLY_LENGTH = 2;

        static private byte[] CreateCommand(byte stepperId, byte timerId)
        {
            /**
            * command format:
            * 0:	command id
            * 1:	stepper id
            * 2:	timer id
            */
            byte[] cmd = new byte[3];

            cmd[0] = (byte)CommandEnum.RUN_STEPPER_ACTIVE;
            cmd[1] = stepperId;
            cmd[2] = timerId;

            return cmd;
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
                    return (false, "invalid stepper id");
                case 3:
                    return (false, "invalid timer id");
                case 4:
                    return (false, "stepper_get_state() failure:");
                case 5:
                    return (false, "wrong stepper state");
                case 6:
                    return (false, "stepper_get_startup_pulse_width() failure");
                case 7:
                    return (false, "timer_start() failure");
                default:
                    return (false, $"unknown error code {errorCode}");
            }
        }
    }

}
