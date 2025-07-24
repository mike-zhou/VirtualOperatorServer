
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using VirtualOperatorServer.CommandAndReply;

namespace VirtualOperatorServer.Facade
{

    public class StatusFacade
    {
        static uint previousMainLoop = 0;
        static readonly JsonSerializerOptions facadeSerializerOption = new()
        {
            Converters = { new JsonStringEnumConverter() },
            IncludeFields = true
        };

        class Facade
        {
            public const int GpioPortCount = 11;
            public const int PowerOutputCount = 12;
            public const int PositionDetectorCount = 49;
            public const int BdcControlCount = 3;
            public const int EncoderCount = 8;
            public const int FlexTimerCount = 6;
            public const int StepperCount = 10;

            public struct GpioPort
            {
                public const int PIN_COUNT = 16;
                public struct Pin
                {
                    public bool isHigh;
                    public bool isWritable;
                }


                public Pin[] pins;

                public GpioPort()
                {
                    pins = new Pin[PIN_COUNT];
                }
            }

            public struct PowerOutput
            {
                public bool isEnabled;
                public bool isOutputDetected;
            }

            public struct PositionDetector
            {
                public bool isDetected;
            }

            public struct BdcControl
            {
                public enum Mode
                {
                    COAST = 0,
                    REVERSE,
                    FORWARD,
                    BRAKE
                }

                public bool isPowerAvailable;
                public Mode mode;
            }

            public struct DynamicStatus
            {
                public uint mainLoopPerSecond;
                public ushort maxFlexTimerIsrPeriod;
                public ushort maxFixTimerIsrPeriod;
            }

            public struct Encoder
            {
                public ushort count;
            }

            public struct Timer
            {
                public enum EnumState
                {
                    UNINITIALIZED = 0,
                    IDLE,
                    BUSY
                }

                public EnumState state;
                public ushort prescaler;
                public ushort prescalerConfig;
            }

            public struct Stepper
            {
                public struct Gpios
                {
                    public bool isDisableHigh;
                    public bool isForwardHigh;
                    public bool isClockHigh;
                }

                public struct Status
                {
                    public enum State
                    {
                        UNINITIALIZED = 0,
                        INITIALIZED,
                        RETURN_TO_HOME_BOUNDARY,
                        HOME_BOUNDARY_TO_READY,
                        READY,
                        RUNNING_ACTIVE,
                        RUNNING_PASSIVE,
                        RUNNING_FORCED,
                        OUT_OF_SYNC,
                        OUT_OF_BOUNDARY,
                        DRIVER_ALARM,
                    }

                    public State state;
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

                public struct Configuration
                {
                    public enum EnumTimer
                    {
                        FLEX_TIMER_0 = 0,
                        FLEX_TIMER_1 = 1,
                        FLEX_TIMER_2 = 2,
                        FLEX_TIMER_3 = 3,
                        FLEX_TIMER_4 = 4,
                        FLIX_TIMER_5 = 5,
                        FIX_TIMER = 6,
                        NOT_SELECTED = 255
                    }

                    public enum EnumEncoder
                    {
                        ENC0 = 0,
                        ENC1 = 1,
                        ENC2 = 2,
                        ENC3 = 3,
                        ENC4 = 4,
                        ENC5 = 5,
                        ENC6 = 6,
                        ENC7 = 7,
                        NOT_SELECTED = 255
                    }

                    public enum EnumMode
                    {
                        FORCED = 0,
                        ACTIVE,
                        PASSIVE,
                        NOT_SELECTED = 255
                    }

                    public enum EnumStepper
                    {
                        STEPPER_0 = 0,
                        STEPPER_1,
                        STEPPER_2,
                        STEPPER_3,
                        STEPPER_4,
                        STEPPER_5,
                        STEPPER_6,
                        STEPPER_7,
                        STEPPER_8,
                        STEPPER_9,
                        NOT_SELECTED = 255
                    }

                    public struct StepperModeForced
                    {
                        ushort pulseWidth;
                    }

                    public struct StepperModeActive
                    {
                        ushort startingPulseWidth;
                        ushort accelerationSteps;
                        ushort cruisePulseWidth;
                        ushort deacceleratingSteps;
                        ushort endingPulseWidth;
                    }

                    public struct StepperModePassive
                    {
                        EnumStepper activeStepper;

                        public StepperModePassive()
                        {
                            activeStepper = EnumStepper.NOT_SELECTED;
                        }
                    }

                    public EnumTimer timer;
                    public EnumEncoder encoder;
                    public EnumMode mode;
                    public StepperModeForced forcedModeConfig;
                    public StepperModeActive activeModeConfig;
                    public StepperModePassive passiveModeConfig;

                    public Configuration()
                    {
                        timer = EnumTimer.NOT_SELECTED;
                        encoder = EnumEncoder.NOT_SELECTED;
                        mode = EnumMode.NOT_SELECTED;
                    }
                }

                public bool isAlarmTriggered;
                public Gpios gpios;
                public Status status;
                public Configuration config;
            }

            public string virtualOperatorVersion = "";
            public GpioPort[] gpioPorts = new GpioPort[GpioPortCount];
            public PowerOutput[] powerOutputs = new PowerOutput[PowerOutputCount];
            public PowerOutput bdcPowerOutput;
            public PositionDetector[] positionDetectors = new PositionDetector[PositionDetectorCount];
            public BdcControl[] bdcControls = new BdcControl[BdcControlCount];
            public DynamicStatus dynamicStatus;
            public Encoder[] encoders = new Encoder[EncoderCount];
            public Timer[] flexTimers = new Timer[FlexTimerCount];
            public Timer fixTimer;
            public Stepper[] steppers = new Stepper[StepperCount];

            public Facade()
            {
                for (int i = 0; i < GpioPortCount; i++)
                {
                    gpioPorts[i] = new GpioPort();
                }
            }
        }

        public static string GetStatusJsonString()
        {
            if (CmdGetVersion.Version == null)
            {
                return "";
            }
            if (CmdGetGPIOMode.GpioModes == null)
            {
                return "";
            }
            if (CmdGetStatus.Status == null)
            {
                return "";
            }

            Facade facade = new Facade();

            var modes = CmdGetGPIOMode.GpioModes;
            var status = CmdGetStatus.Status;

            facade.virtualOperatorVersion = CmdGetVersion.Version;

            // GPIO
            Debug.Assert(Facade.GpioPortCount == VirtualOperatorStatus.PortCount, "Different port count between Facade and VirtualOperatorStatus");
            Debug.Assert(Facade.GpioPortCount == CmdGetGPIOMode.PORT_COUNT, "Different port count between Facade and CmdGetGPIOMode");
            for (int portIndex = 0; portIndex < Facade.GpioPortCount; portIndex++)
            {
                var mode = modes[portIndex];
                var value = status.ports[portIndex];

                for (int pinIndex = 0; pinIndex < 16; pinIndex++)
                {
                    ushort mask = (ushort)(0x1 << pinIndex);

                    if ((value & mask) != 0)
                    {
                        facade.gpioPorts[portIndex].pins[pinIndex].isHigh = true;
                    }
                    else
                    {
                        facade.gpioPorts[portIndex].pins[pinIndex].isHigh = false;
                    }

                    if ((mode & mask) != 0)
                    {
                        facade.gpioPorts[portIndex].pins[pinIndex].isWritable = true;
                    }
                    else
                    {
                        facade.gpioPorts[portIndex].pins[pinIndex].isWritable = false;
                    }
                }
            }

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

            // Power Output
            {

                uint[][] powerMatrix =
                [
                    ['G',   9,    'G',  13],    // PO 0: GP98, GP101
                    ['J',   14,   'J',  13],    // PO 1: GP96, GP95
                    ['C',   11,   'D',  6],     // PO 2: GP84, GP92
                    ['I',   3,    'I',  2],     // PO 3: GP82, GP81
                    ['I',   0,    'I',  1],     // PO 4: GP79, GP80
                    ['H',   15,   'H',  14],    // PO 5: GP78, GP77
                    ['K',   4,    'G',  14],    // PO 6: GP104, GP102
                    ['K',   7,    'G',  15],    // PO 7: GP107, GP108
                    ['E',   0,    'B',  7],     // PO 8: GP113, GP110
                    ['E',   1,    'I',  5],     // PO 9: GP114, GP116
                    ['E',   3,    'E',  2],     // PO 10: GP1, GP0
                    ['E',   6,    'C',  14],    // PO 11: GP3, GP6
                ];

                Debug.Assert(powerMatrix.Length == Facade.PowerOutputCount, "wrong lenght of powerMatrix");
                for (int i = 0; i < powerMatrix.Length; i++)
                {
                    facade.powerOutputs[i].isEnabled = IsGpioHigh((char)powerMatrix[i][0], powerMatrix[i][1]);
                    facade.powerOutputs[i].isOutputDetected = IsGpioHigh((char)powerMatrix[i][2], powerMatrix[i][3]);
                }

                facade.bdcPowerOutput.isEnabled = IsGpioHigh('C', 15);
                facade.bdcPowerOutput.isOutputDetected = IsGpioHigh('I', 10);
            }

            // Position Detector
            {
                uint[][] matrix = [
                    // 0 - 9
                    ['B', 8], // GP111
                    ['B', 4], // GP109    
                    ['K', 6], // GP106    
                    ['K', 5], // GP105    
                    ['K', 3], // GP103
                    ['G', 11], // GP100
                    ['G', 10], // GP99
                    ['J', 15], // GP97
                    ['J', 12], // GP94
                    ['D', 7], // GP93
                    // 10 - 19
                    ['D', 5], // GP91
                    ['D', 1], // GP87
                    ['D', 4], // GP90
                    ['D', 3], // GP89
                    ['D', 2], // GP88
                    ['A', 4], // GP28
                    ['H', 5], // GP27
                    ['H', 3], // GP25
                    ['C', 1], // GP22
                    ['F', 10], // GP21
                    // 20 - 29
                    ['F', 8], // GP19
                    ['F', 4], // GP17
                    ['F', 3], // GP16
                    ['I', 13], // GP14
                    ['F', 2], // GP12
                    ['F', 1], // GP11
                    ['I', 12], // GP13
                    ['I', 14], // GP15
                    ['F', 5], // GP18
                    ['F', 9], // GP20
                    // 30 - 39
                    ['C', 2], // GP23
                    ['A', 2], // GP24
                    ['H', 4], // GP26
                    ['C', 4], // GP29
                    ['C', 5], // GP30
                    ['I', 15], // GP32
                    ['B', 2], // GP31
                    ['J', 0], // GP33
                    ['J', 1], // GP34
                    ['J', 2], // GP35
                    // 40 - 48
                    ['J', 3], // GP36
                    ['J', 4], // GP37
                    ['E', 7], // GP45
                    ['G', 0], // GP43
                    ['F', 11], // GP38
                    ['F', 13], // GP40
                    ['F', 12], // GP39
                    ['F', 14], // GP41
                    ['F', 15] // GP42
                ];

                for (int i = 0; i < matrix.Length; i++)
                {
                    facade.positionDetectors[i].isDetected = IsGpioHigh((char)matrix[i][0], matrix[i][1]);
                }
            }

            // BDC Controls
            {
                bool input0, input1;

                // BDC 0: GP115, GP112, GP117
                input0 = IsGpioHigh('I', 4);
                input1 = IsGpioHigh('B', 9);
                if (!input0 && !input1)
                {
                    facade.bdcControls[0].mode = Facade.BdcControl.Mode.COAST;
                }
                else if (!input0 && input1)
                {
                    facade.bdcControls[0].mode = Facade.BdcControl.Mode.REVERSE;
                }
                else if (input0 && !input1)
                {
                    facade.bdcControls[0].mode = Facade.BdcControl.Mode.FORWARD;
                }
                else
                {
                    facade.bdcControls[0].mode = Facade.BdcControl.Mode.BRAKE;
                }
                facade.bdcControls[0].isPowerAvailable = IsGpioHigh('I', 6);

                // BDC 1: GP2, GP118, GP4
                input0 = IsGpioHigh('E', 4);
                input1 = IsGpioHigh('I', 7);
                if (!input0 && !input1)
                {
                    facade.bdcControls[1].mode = Facade.BdcControl.Mode.COAST;
                }
                else if (!input0 && input1)
                {
                    facade.bdcControls[1].mode = Facade.BdcControl.Mode.REVERSE;
                }
                else if (input0 && !input1)
                {
                    facade.bdcControls[1].mode = Facade.BdcControl.Mode.FORWARD;
                }
                else
                {
                    facade.bdcControls[1].mode = Facade.BdcControl.Mode.BRAKE;
                }
                facade.bdcControls[1].isPowerAvailable = IsGpioHigh('I', 8);

                // BDC 2: GP8, GP5, GP10
                input0 = IsGpioHigh('I', 9);
                input1 = IsGpioHigh('C', 13);
                if (!input0 && !input1)
                {
                    facade.bdcControls[2].mode = Facade.BdcControl.Mode.COAST;
                }
                else if (!input0 && input1)
                {
                    facade.bdcControls[2].mode = Facade.BdcControl.Mode.REVERSE;
                }
                else if (input0 && !input1)
                {
                    facade.bdcControls[2].mode = Facade.BdcControl.Mode.FORWARD;
                }
                else
                {
                    facade.bdcControls[2].mode = Facade.BdcControl.Mode.BRAKE;
                }
                facade.bdcControls[2].isPowerAvailable = IsGpioHigh('F', 0);
            }

            // Dynamic Status
            {
                // this is not an accurate calculation.
                uint loopPerSecond;
                if (status.mainLoopCount >= previousMainLoop)
                {
                    loopPerSecond = status.mainLoopCount - previousMainLoop;
                }
                else
                {
                    loopPerSecond = uint.MaxValue - previousMainLoop + status.mainLoopCount;
                }
                previousMainLoop = status.mainLoopCount;

                facade.dynamicStatus.mainLoopPerSecond = loopPerSecond;

                facade.dynamicStatus.maxFlexTimerIsrPeriod = status.maxFlexTimerIsrPeriod;
                facade.dynamicStatus.maxFixTimerIsrPeriod = status.maxFixTimerIsrPeriod;
            }

            // Encoders
            Debug.Assert(Facade.EncoderCount == VirtualOperatorStatus.EncoderCount, "Different encoder count between Facade and VirtualOperatorStatus");
            {
                for (int i = 0; i < Facade.EncoderCount; i++)
                {
                    facade.encoders[i].count = status.encoders[i];
                }
            }

            // Timers
            Debug.Assert(Facade.FlexTimerCount == (VirtualOperatorStatus.TimerCount - 1), "Different flex timer count between Facade and VirtualOperatorStatus");
            {
                for (int i = 0; i < Facade.FlexTimerCount; i++)
                {
                    facade.flexTimers[i].state = (Facade.Timer.EnumState)(status.timersData[i].state);
                    facade.flexTimers[i].prescaler = status.timersData[i].prescaler;
                }

                facade.fixTimer.state = (Facade.Timer.EnumState)(status.timersData[VirtualOperatorStatus.TimerCount - 1].state);
                facade.fixTimer.prescaler = status.timersData[VirtualOperatorStatus.TimerCount - 1].prescaler;
            }

            // Steppers
            Debug.Assert(Facade.StepperCount == VirtualOperatorStatus.StepperCount, "Different stepper count between Facade and VirtualOperatorStatus");
            {
                // gpio and alarm
                {
                    // stepper 0: GP52, GP53, GP44, CLK_STP0
                    facade.steppers[0].gpios.isForwardHigh = IsGpioHigh('J', 5);
                    facade.steppers[0].gpios.isDisableHigh = IsGpioHigh('H', 7);
                    facade.steppers[0].isAlarmTriggered = IsGpioHigh('G', 1);
                    facade.steppers[0].gpios.isClockHigh = IsGpioHigh('E', 5);

                    // stepper 1: GP54, GP55, GP46, CLK_STP1
                    facade.steppers[1].gpios.isForwardHigh = IsGpioHigh('H', 8);
                    facade.steppers[1].gpios.isDisableHigh = IsGpioHigh('H', 9);
                    facade.steppers[1].isAlarmTriggered = IsGpioHigh('E', 8);
                    facade.steppers[1].gpios.isClockHigh = IsGpioHigh('F', 6);

                    // stepper 2: GP56, USB_DEC_1, GP47, CLK_STP2
                    facade.steppers[2].gpios.isForwardHigh = IsGpioHigh('H', 12);
                    facade.steppers[2].gpios.isDisableHigh = IsGpioHigh('D', 8);
                    facade.steppers[2].isAlarmTriggered = IsGpioHigh('E', 10);
                    facade.steppers[2].gpios.isClockHigh = IsGpioHigh('F', 7);

                    // stepper 3: GP57, GP58, GP48, CLK_STP3
                    facade.steppers[3].gpios.isForwardHigh = IsGpioHigh('D', 9);
                    facade.steppers[3].gpios.isDisableHigh = IsGpioHigh('D', 10);
                    facade.steppers[3].isAlarmTriggered = IsGpioHigh('E', 12);
                    facade.steppers[3].gpios.isClockHigh = IsGpioHigh('A', 6);

                    // stepper 4: GP59, GP60, GP49, CLK_STP4
                    facade.steppers[4].gpios.isForwardHigh = IsGpioHigh('D', 14);
                    facade.steppers[4].gpios.isDisableHigh = IsGpioHigh('D', 15);
                    facade.steppers[4].isAlarmTriggered = IsGpioHigh('E', 13);
                    facade.steppers[4].gpios.isClockHigh = IsGpioHigh('A', 7);

                    // stepper 5: GP61, GP62, GP50, CLK_STP5
                    facade.steppers[5].gpios.isForwardHigh = IsGpioHigh('J', 7);
                    facade.steppers[5].gpios.isDisableHigh = IsGpioHigh('J', 9);
                    facade.steppers[5].isAlarmTriggered = IsGpioHigh('E', 14);
                    facade.steppers[5].gpios.isClockHigh = IsGpioHigh('H', 6);

                    // stepper 6: GP64, GP65, GP51, GP63
                    facade.steppers[6].gpios.isForwardHigh = IsGpioHigh('J', 11);
                    facade.steppers[6].gpios.isDisableHigh = IsGpioHigh('K', 0);
                    facade.steppers[6].isAlarmTriggered = IsGpioHigh('E', 15);
                    facade.steppers[6].gpios.isClockHigh = IsGpioHigh('J', 10);

                    // stepper 7: GP67, GP68, GP86, GP66
                    facade.steppers[7].gpios.isForwardHigh = IsGpioHigh('K', 2);
                    facade.steppers[7].gpios.isDisableHigh = IsGpioHigh('G', 2);
                    facade.steppers[7].isAlarmTriggered = IsGpioHigh('D', 0);
                    facade.steppers[7].gpios.isClockHigh = IsGpioHigh('K', 1);

                    // stepper 8: GP70, GP71, GP83, GP69
                    facade.steppers[8].gpios.isForwardHigh = IsGpioHigh('G', 4);
                    facade.steppers[8].gpios.isDisableHigh = IsGpioHigh('G', 5);
                    facade.steppers[8].isAlarmTriggered = IsGpioHigh('C', 10);
                    facade.steppers[8].gpios.isClockHigh = IsGpioHigh('G', 3);

                    // stepper 9: GP73, GP74, GP85, GP72
                    facade.steppers[9].gpios.isForwardHigh = IsGpioHigh('G', 7);
                    facade.steppers[9].gpios.isDisableHigh = IsGpioHigh('G', 8);
                    facade.steppers[9].isAlarmTriggered = IsGpioHigh('C', 12);
                    facade.steppers[9].gpios.isClockHigh = IsGpioHigh('G', 6);
                }

                // status
                for (int i = 0; i < Facade.StepperCount; i++)
                {
                    Facade.Stepper.Status stepperStatus;
                    ref var data = ref status.steppersData[i];

                    stepperStatus.state = (Facade.Stepper.Status.State)data.state;
                    stepperStatus.isForward = data.isForward;
                    stepperStatus.isEnabled = data.isEnabled;
                    stepperStatus.endBoundaryTouched = data.endBoundaryTouched;
                    stepperStatus.homeBoundaryTouched = data.homeBoundaryTouched;
                    stepperStatus.isEnableHigh = data.isEnableHigh;
                    stepperStatus.isForwardHigh = data.isForwardHigh;
                    stepperStatus.isRisingEdgeDriven = data.isRisingEdgeDriven;
                    stepperStatus.isPassiveStepsPopulated = data.isPassiveStepsPopulated;
                    stepperStatus.isRampdownPopulated = data.isRampdownPopulated;
                    stepperStatus.isCruisePopulated = data.isCruisePopulated;
                    stepperStatus.isRampupPopulated = data.isRampupPopulated;
                    stepperStatus.offset = data.offset;
                    stepperStatus.encoderOffset = data.encoderOffset;
                    stepperStatus.maxEncoderOffsetError = data.maxEncoderOffsetError;

                    facade.steppers[i].status = stepperStatus;
                }
            }

            var jsonStr = JsonSerializer.Serialize(facade, facadeSerializerOption);
            return jsonStr;
        }

    }

}
