namespace Polly.Extensibility
{
    /// <summary>
    /// The arguments used by the <see cref="AttemptRateLimiterStrategyOptions.OnRejected"/>.
    /// </summary>
    /// <remarks>
    /// Always use the constructor when creating this struct, otherwise we do not guarantee binary compatibility.
    /// </remarks>
    public readonly struct OnAttemptRateLimiterRejectedArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnRateLimiterRejectedArguments"/> struct.
        /// </summary>
        /// <param name="context">The context associated with the execution of a user-provided callback.</param>
        /// <param name="lease">The lease that has no permits and was rejected by the rate limiter.</param>
        public OnAttemptRateLimiterRejectedArguments(ResilienceContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the context associated with the execution of a user-provided callback.
        /// </summary>
        public ResilienceContext Context { get; }
    }
}
