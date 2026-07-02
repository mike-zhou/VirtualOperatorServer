using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Provides precomputed timer pulse widths for stepper motor acceleration,
/// deceleration, and short-distance moves.
/// </summary>
public interface IStepperPulses
{
    /// <summary>
    /// Gets the pulse widths used to accelerate from startup speed to cruising speed.
    /// </summary>
    IReadOnlyList<ushort> SCurvePulsesRampup { get; }

    /// <summary>
    /// Gets the pulse widths used to decelerate from cruising speed to startup speed.
    /// </summary>
    IReadOnlyList<ushort> SCurvePulsesRampdown { get; }

    /// <summary>
    /// Gets the complete pulse-width sequence for a move too short to reach cruising speed.
    /// </summary>
    /// <param name="stepCount">The number of steps in the move.</param>
    /// <returns>
    /// The pulse-width sequence for <paramref name="stepCount"/>, or <c>null</c> when the move
    /// is outside the supported short-distance range.
    /// </returns>
    IReadOnlyList<ushort>? GetShortDistancePulses(int stepCount);
}

/// <summary>
/// Creates and caches immutable pulse tables for a specific stepper motor timing configuration.
/// </summary>
/// <remarks>
/// Call <see cref="Create"/> once per unique timing configuration and reuse the returned
/// <see cref="IStepperPulses"/> instance when issuing moves. Use
/// <see cref="IStepperPulses.SCurvePulsesRampup"/> followed by cruising pulses and
/// <see cref="IStepperPulses.SCurvePulsesRampdown"/> for moves long enough to reach cruising
/// speed. For shorter moves, call <see cref="IStepperPulses.GetShortDistancePulses"/> with the
/// total step count and use the returned sequence directly when it is not <c>null</c>.
/// Pulse widths are timer counts for one half-step period.
/// </remarks>
public static class StepperPulsesFactory
{
    private static readonly object CacheLock = new();
    private static readonly Dictionary<StepperPulsesKey, IStepperPulses> Cache = new();

    public static IStepperPulses Create(
        ushort startupPulseWidth,
        uint maxSpeedUpSteps,
        ushort cruisingPulseWidth,
        uint timerClockWidthNs)
    {
        var key = new StepperPulsesKey(startupPulseWidth, maxSpeedUpSteps, cruisingPulseWidth, timerClockWidthNs);

        lock (CacheLock)
        {
            if (!Cache.TryGetValue(key, out IStepperPulses? stepperPulses))
            {
                stepperPulses = new StepperPulses(startupPulseWidth, maxSpeedUpSteps, cruisingPulseWidth, timerClockWidthNs);
                Cache.Add(key, stepperPulses);
            }

            return stepperPulses;
        }
    }

    private readonly record struct StepperPulsesKey(
        ushort StartupPulseWidth,
        uint MaxSpeedUpSteps,
        ushort CruisingPulseWidth,
        uint TimerClockWidthNs);

    private sealed class StepperPulses : IStepperPulses
    {
        private const double VelocityCoefficient = 15.0 / 8.0;
        private const double AccelerationCoefficient = 10.0 / 1.7320508075688772;
        private const double JerkCoefficient = 60.0;
        private const double SolverTolerance = 1e-12;

        private ReadOnlyCollection<ReadOnlyCollection<ushort>> ShortDistancePulseTable { get; set; } =
            new(Array.Empty<ReadOnlyCollection<ushort>>());
        public IReadOnlyList<ushort> SCurvePulsesRampup { get; private set; } = Array.Empty<ushort>();
        public IReadOnlyList<ushort> SCurvePulsesRampdown { get; private set; } = Array.Empty<ushort>();


        private readonly ushort _startupPulseWidth;
        private readonly uint _maxSpeedUpSteps;
        private readonly uint _timerClockWidthNs;
        private readonly double _cruisingStepPerSeconds;

        public StepperPulses(ushort startupPulseWidth, uint maxSpeedUpSteps, ushort cruisingPulseWidth, uint timerClockWidthNs)
        {
            if (startupPulseWidth == 0)
                throw new ArgumentOutOfRangeException(nameof(startupPulseWidth), "startupPulseWidth must be greater than zero.");

            if (maxSpeedUpSteps == 0)
                throw new ArgumentOutOfRangeException(nameof(maxSpeedUpSteps), "Max speed-up steps must be greater than zero.");

            if (cruisingPulseWidth == 0)
                throw new ArgumentOutOfRangeException(
                    nameof(cruisingPulseWidth),
                    "cruisingPulseWidth must be greater than zero.");

            if (timerClockWidthNs == 0)
                throw new ArgumentOutOfRangeException(nameof(timerClockWidthNs), "timerClockWidthNs must be greater than zero.");

            _startupPulseWidth = startupPulseWidth;
            _maxSpeedUpSteps = maxSpeedUpSteps;
            _cruisingStepPerSeconds = 1_000_000_000.0 / 2 / cruisingPulseWidth / timerClockWidthNs;
            _timerClockWidthNs = timerClockWidthNs;

            SCurvePulsesRampup = CalculateNormalSCurvePulses();
            var rampdown = new List<ushort>(SCurvePulsesRampup);
            rampdown.Reverse();
            SCurvePulsesRampdown = rampdown.AsReadOnly();

            ShortDistancePulseTable = BuildShortDistancePulseTable(maxSpeedUpSteps, _cruisingStepPerSeconds);
        }

        public IReadOnlyList<ushort>? GetShortDistancePulses(int stepCount)
        {
            if (stepCount < 1 || stepCount > (2 * _maxSpeedUpSteps - 1))
            {
                return null;
            }

            return ShortDistancePulseTable[stepCount - 1];
        }

        private ReadOnlyCollection<ushort> CalculateNormalSCurvePulses()
        {
            var pulses = new List<ushort>((int)_maxSpeedUpSteps);
            double totalTime = 2.0 * _maxSpeedUpSteps / _cruisingStepPerSeconds;
            double previousTime = 0.0;

            for (int k = 1; k <= _maxSpeedUpSteps; k++)
            {
                double targetPosition = (double)k / _maxSpeedUpSteps;
                double u = SolveNormalizedRampTime(targetPosition);
                double currentTime = u * totalTime;
                double periodSeconds = currentTime - previousTime;

                // divide a step period to 2 pulses
                pulses.Add(ToTimerClockCount(periodSeconds / 2));
                previousTime = currentTime;
            }

            return pulses.AsReadOnly();
        }

        private ReadOnlyCollection<ReadOnlyCollection<ushort>> BuildShortDistancePulseTable(uint maxSpeedUpSteps, double cruisingStepPerSeconds)
        {
            uint maxShortMoveSteps = 2 * maxSpeedUpSteps - 1;
            var table = new List<ReadOnlyCollection<ushort>>((int)maxShortMoveSteps);

            double referenceTime = 2.0 * maxSpeedUpSteps / cruisingStepPerSeconds;
            double maxVelocity = cruisingStepPerSeconds;
            double maxAcceleration = 3.0 * maxSpeedUpSteps / (referenceTime * referenceTime);
            double maxJerk = 12.0 * maxSpeedUpSteps / (referenceTime * referenceTime * referenceTime);

            for (int stepCount = 1; stepCount <= maxShortMoveSteps; stepCount++)
            {
                table.Add(BuildMovePulses(
                    stepCount,
                    maxVelocity,
                    maxAcceleration,
                    maxJerk));
            }

            return table.AsReadOnly();
        }

        private ReadOnlyCollection<ushort> BuildMovePulses(
            int stepCount,
            double maxVelocity,
            double maxAcceleration,
            double maxJerk)
        {
            double totalTime = CalculateTotalTime(
                stepCount,
                maxVelocity,
                maxAcceleration,
                maxJerk);

            var periods = new List<ushort>(stepCount);
            double previousTime = 0.0;

            for (int k = 1; k <= stepCount; k++)
            {
                double targetPosition = (double)k / stepCount;
                double u = SolveNormalizedTime(targetPosition);
                double currentTime = u * totalTime;
                double periodSeconds = currentTime - previousTime;

                // divide a step into 2 pulses
                periods.Add(ToTimerClockCount(periodSeconds / 2));
                previousTime = currentTime;
            }

            return periods.AsReadOnly();
        }

        private double CalculateTotalTime(
            int stepCount,
            double maxVelocity,
            double maxAcceleration,
            double maxJerk)
        {
            double timeLimitedByVelocity = VelocityCoefficient * stepCount / maxVelocity;
            double timeLimitedByAcceleration = Math.Sqrt(AccelerationCoefficient * stepCount / maxAcceleration);
            double timeLimitedByJerk = CubeRoot(JerkCoefficient * stepCount / maxJerk);

            return Math.Max(
                timeLimitedByVelocity,
                Math.Max(timeLimitedByAcceleration, timeLimitedByJerk));
        }

        private double SolveNormalizedTime(double targetPosition)
        {
            if (targetPosition <= 0.0)
                return 0.0;

            if (targetPosition >= 1.0)
                return 1.0;

            double low = 0.0;
            double high = 1.0;

            while (high - low > SolverTolerance)
            {
                double u = (low + high) * 0.5;
                double position = SmoothStep5(u);

                if (position < targetPosition)
                    low = u;
                else
                    high = u;
            }

            return (low + high) * 0.5;
        }

        private double SolveNormalizedRampTime(double targetPosition)
        {
            if (targetPosition <= 0.0)
                return 0.0;

            if (targetPosition >= 1.0)
                return 1.0;

            double low = 0.0;
            double high = 1.0;

            while (high - low > SolverTolerance)
            {
                double u = (low + high) * 0.5;
                double position = IntegratedSmoothStep5(u);

                if (position < targetPosition)
                    low = u;
                else
                    high = u;
            }

            return (low + high) * 0.5;
        }

        private double SmoothStep5(double u)
        {
            double u2 = u * u;
            double u3 = u2 * u;

            return (10.0 * u3) - (15.0 * u3 * u) + (6.0 * u3 * u2);
        }

        private double IntegratedSmoothStep5(double u)
        {
            double u2 = u * u;
            double u3 = u2 * u;
            double u4 = u3 * u;
            double u5 = u4 * u;
            double u6 = u5 * u;

            return (5.0 * u4) - (6.0 * u5) + (2.0 * u6);
        }

        private double CubeRoot(double value)
        {
            return Math.Pow(value, 1.0 / 3.0);
        }

        private ushort ToTimerClockCount(double seconds)
        {
            double counts = Math.Round(seconds * 1_000_000_000.0 / _timerClockWidthNs);

            if (counts < 1.0)
                return 1;

            if (counts > _startupPulseWidth)
                counts = _startupPulseWidth;

            return (ushort)counts;
        }
    }
}
