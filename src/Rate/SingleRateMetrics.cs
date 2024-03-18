namespace Polly.Extensibility.Rate
{
    internal sealed class SingleRateMetrics : RateMetrics
    {
        private readonly TimeSpan _samplingDuration;

        private int _primaryAttempts;
        private int _secondaryAttempts;
        private DateTimeOffset _startedAt;

        public SingleRateMetrics(TimeSpan samplingDuration, TimeProvider timeProvider)
            : base(timeProvider)
        {
            _samplingDuration = samplingDuration;
            _startedAt = timeProvider.GetUtcNow();
        }

        public override void IncrementPrimaryAttempts()
        {
            TryReset();
            _primaryAttempts++;
        }

        public override void IncrementSecondaryAttempts()
        {
            TryReset();
            _secondaryAttempts++;
        }

        public override void Reset()
        {
            _startedAt = TimeProvider.GetUtcNow();
            _primaryAttempts = 0;
            _secondaryAttempts = 0;
        }

        public override RateInfo GetRateInfo()
        {
            TryReset();

            return RateInfo.Create(_primaryAttempts, _secondaryAttempts);
        }

        private void TryReset()
        {
            if (TimeProvider.GetUtcNow() - _startedAt >= _samplingDuration)
            {
                Reset();
            }
        }
    }
}
