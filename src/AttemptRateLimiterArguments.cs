namespace Polly.Extensibility
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct AttemptRateLimiterArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptRateLimiterArguments"/> struct.
        /// </summary>
        /// <param name="context">Context associated with the execution of a user-provided callback.</param>
        public AttemptRateLimiterArguments(ResilienceContext context) => Context = context;

        /// <summary>
        /// Gets the context associated with the execution of a user-provided callback.
        /// </summary>
        public ResilienceContext Context { get; }
    }
}
