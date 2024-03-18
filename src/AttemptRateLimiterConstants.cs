namespace Polly.Extensibility
{
    internal static class AttemptRateLimiterConstants
    {
        public const string DefaultName = "AttemptRateLimiter";

        public const double DefaultSecondaryAttemptRatio = 0.1;

        public const int DefaultMinimumThroughput = 100;

        public const int MinimumValidThroughput = 2;

        public static readonly TimeSpan DefaultSamplingDuration = TimeSpan.FromSeconds(30);

        public const string OnAttemptRateLimiterRejectedEvent = "OnAttemptRateLimiterRejectedEvent";

        public static readonly ResiliencePropertyKey<int> AttemptNumber = new ResiliencePropertyKey<int>($"X-Polly-AttemptRateLimiter-{nameof(AttemptNumber)}");
    }
}
