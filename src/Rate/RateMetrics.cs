namespace Polly.Extensibility.Rate
{
    internal abstract class RateMetrics
    {
        private const short NumberOfWindows = 10;
        private static readonly TimeSpan ResolutionOfRateLimiterTimer = TimeSpan.FromMilliseconds(20);

        protected RateMetrics(TimeProvider timeProvider) => TimeProvider = timeProvider;

        public static RateMetrics Create(TimeSpan samplingDuration, TimeProvider timeProvider)
            => samplingDuration < TimeSpan.FromTicks(ResolutionOfRateLimiterTimer.Ticks * NumberOfWindows)
               ? new SingleRateMetrics(samplingDuration, timeProvider)
               : new RollingRateMetrics(samplingDuration, NumberOfWindows, timeProvider);

        protected TimeProvider TimeProvider { get; }

        public abstract void IncrementPrimaryAttempts();

        public abstract void IncrementSecondaryAttempts();

        public abstract void Reset();

        public abstract RateInfo GetRateInfo();
    }
}
