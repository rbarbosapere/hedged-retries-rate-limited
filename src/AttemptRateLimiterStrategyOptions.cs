using System.ComponentModel.DataAnnotations;

namespace Polly.Extensibility
{
    public class AttemptRateLimiterStrategyOptions : ResilienceStrategyOptions
    {
        public AttemptRateLimiterStrategyOptions() => Name = AttemptRateLimiterConstants.DefaultName;

        /// <summary>
        /// Gets or sets the failure-to-success ratio at which the circuit will break.
        /// </summary>
        /// <remarks>
        /// A number between zero and one (inclusive) e.g. 0.5 represents breaking if 50% or more of actions result in a handled failure.
        /// </remarks>
        /// <value>A ratio number higher than 0, up to 1. The default value is 0.1 (i.e. 10%).</value>
        [Range(0, 1.0)]
        public double SecondaryAttemptRatio { get; set; } = AttemptRateLimiterConstants.DefaultSecondaryAttemptRatio;

        /// <summary>
        /// Gets or sets the minimum throughput: this many actions or more must pass through the circuit in the time-slice,
        /// for statistics to be considered significant and the circuit-breaker to come into action.
        /// </summary>
        /// <value>
        /// The default value is 100. The value must be 2 or greater.
        /// </value>
        [Range(AttemptRateLimiterConstants.MinimumValidThroughput, int.MaxValue)]
        public int MinimumThroughput { get; set; } = AttemptRateLimiterConstants.DefaultMinimumThroughput;

        /// <summary>
        /// Gets or sets the duration of the sampling over which failure ratios are assessed.
        /// </summary>
        /// <value>
        /// The default value is 30 seconds. Value must be greater than 0.5 seconds.
        /// </value>
        [Range(typeof(TimeSpan), "00:00:00.500", "1.00:00:00")]
        public TimeSpan SamplingDuration { get; set; } = AttemptRateLimiterConstants.DefaultSamplingDuration;

        /// <summary>
        /// Gets or sets an event that is raised when the execution of user-provided callback is rejected by the rate limiter.
        /// </summary>
        /// <value>
        /// The default value is <see langword="null"/>.
        /// </value>
        public Func<OnAttemptRateLimiterRejectedArguments, ValueTask>? OnRejected { get; set; }
    }
}
