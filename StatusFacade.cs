
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace VirtualOperatorServer.Facade
{

    public class StatusFacade
    {
        public struct GpioPort
        {
            public const int PIN_COUNT = 16;
            public struct Pin
            {
                public bool isHigh;
                public bool isReadonly;
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

        public enum BdcMode
        {
            COAST = 0,
            REVERSE = 1,
            FORWARD = 2,
            BRAKE = 3
        }

        public struct BdcControl
        {
            public bool isPowerAvailable;
            public BdcMode mode;
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
            public ushort prescaler;
        }


        public struct Stepper
        {
            public struct StepperGpios
            {
                public bool isDisableHigh;
                public bool isForwardHigh;
                public bool isClockHigh;
            }

            public struct StepperStatus
            {
                public enum EnumStepperState
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

                public EnumStepperState state;
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

            public struct StepperConfiguration
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

                public StepperConfiguration()
                {
                    timer = EnumTimer.NOT_SELECTED;
                    encoder = EnumEncoder.NOT_SELECTED;
                    mode = EnumMode.NOT_SELECTED;
                }
            }

            public bool isAlarmTriggered;
            public StepperGpios stepperGpios;
            public StepperStatus status;
            public StepperConfiguration config;
        }

        public class Facade
        {
            public const int GpioPortCount = 11;
            public const int PowerOutputCount = 12;
            public const int PositionDetectorCount = 49;
            public const int BdcOutputCount = 3;
            public const int EncoderCount = 8;
            public const int FlexTimerCount = 6;
            public const int StepperCount = 10;

            public GpioPort[] GpioPorts { get; private set; }
            public PowerOutput[] PowerOutputs { get; private set; }
            public PowerOutput BDCPowerOutput { get; private set; }
            public PositionDetector[] PositionDetectors { get; private set; }
            public BdcControl[] BdcOutputs { get; private set; }
            public DynamicStatus DynamicStatus { get; private set; }
            public Encoder[] Encoders { get; private set; }
            public Timer[] FlexTimers { get; private set; }
            public Timer FixTimer { get; private set; }
            public Stepper[] Steppers { get; private set; }

            public Facade()
            {
                GpioPorts = new GpioPort[GpioPortCount];
                PowerOutputs = new PowerOutput[PowerOutputCount];
                PositionDetectors = new PositionDetector[PositionDetectorCount];
                BdcOutputs = new BdcControl[BdcOutputCount];
                Encoders = new Encoder[EncoderCount];
                FlexTimers = new Timer[FlexTimerCount];
                Steppers = new Stepper[StepperCount];
            }
        }

        public static Facade? GetStatusFacade()
        {
            Facade? facade = null;

            return facade;
        }

    }

}
