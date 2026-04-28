namespace VirtualOperatorServer.StepsBuilder
{
    public interface ActivePulsesBuilder
    {
        ushort[] GetAcceleratingPulses();
        ushort[] GetDeacceleratingPulses();
    }

    public class SCurvePulsesBuilder : ActivePulsesBuilder
    {
        private readonly int _N;        // total steps
        private readonly double _V;     // final speed (steps/sec)
        private readonly double _T;     // total motion time
        private readonly uint _timerClockNs;

        private readonly List<ushort> pulses;

        public SCurvePulsesBuilder(ushort stepCount, ushort finalPulseWidth, uint timerClockNs)
        {
            if (stepCount < 1 || finalPulseWidth < 1 || timerClockNs < 1) 
                throw new ArgumentException("steps must be > 0");

            _N = stepCount;
            _V = 1000000000 / 2 / finalPulseWidth / timerClockNs; // steps per second
            _timerClockNs = timerClockNs;

            // total time derived from motion constraint
            _T = 2.0 * _N / _V;

            pulses = new List<ushort>(0);
            GenerateStepIntervals();
        }

        public ushort[] GetAcceleratingPulses()
        {
            return pulses.ToArray();
        }

        public ushort[] GetDeacceleratingPulses()
        {
            return pulses.AsEnumerable().Reverse().ToArray();
        }

        private void GenerateStepIntervals()
        {
            var tmpSteps = new List<double>(_N);

            double lastTime = 0.0;

            for (int k = 1; k <= _N; k++)
            {
                double target = (double)k / _N;

                double u = SolveU(target);

                double t = u * _T;

                double interval = t - lastTime;
                lastTime = t;

                tmpSteps.Add(interval);
            }

            // convert interval to number of timer clock
            for(int i = 0; i< tmpSteps.Count; i++)
            {
                var pulse = tmpSteps[i];

                pulse = pulse * 1000000000 / _timerClockNs / 2;

                if(pulse > ushort.MaxValue)
                {
                    throw new OverflowException($"Pulse value at index {i} is too large: {pulse}");
                }

                pulses.Add((ushort)pulse);
            }
        }

        private double SolveU(double target)
        {
            double lo = 0.0;
            double hi = 1.0;

            for (int i = 0; i < 40; i++)
            {
                double u = (lo + hi) * 0.5;
                double x = 2.0 * u * u * u - u * u * u * u;

                if (x < target)
                    lo = u;
                else
                    hi = u;
            }

            return (lo + hi) * 0.5;
        }
    }


    public class LinearPulsesBuilder : ActivePulsesBuilder
    {
        private readonly ushort[] pulses;

        public LinearPulsesBuilder(ushort startingPulseWidth, ushort accelerationSteps, ushort cruisingPulseWidth)
        {
            if(startingPulseWidth <= cruisingPulseWidth)
            {
                throw new ArgumentException($"Invalid pulse width: {startingPulseWidth} <= {cruisingPulseWidth}");
            }
            if(accelerationSteps < 1)
            {
                throw new ArgumentException($"Invalid accelerating steps: {accelerationSteps}");
            }

            double acceleratingRate = startingPulseWidth - cruisingPulseWidth;
            acceleratingRate /= accelerationSteps;
            
            pulses = new ushort[accelerationSteps];
            for(int i=0; i<accelerationSteps; i++)
            {
                pulses[i] = (ushort)Math.Round(startingPulseWidth - i * acceleratingRate);
            }
        }

        public ushort[] GetAcceleratingPulses()
        {
            return pulses;
        }

        public ushort[] GetDeacceleratingPulses()
        {
            var reversedPulses = new ushort[pulses.Length];
            for (int i = 0, j = pulses.Length - 1; i < pulses.Length; i++, j--)
            {
                reversedPulses[i] = pulses[j];
            }

            return reversedPulses;
        }
    }

}
