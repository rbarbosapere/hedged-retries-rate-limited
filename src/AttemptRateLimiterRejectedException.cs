namespace Polly.Extensibility
{
    /// <summary>
    /// Exception thrown when a attempt rate limiter rejects an execution.
    /// </summary>
    public sealed class AttemptRateLimiterRejectedException : ExecutionRejectedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptRateLimiterRejectedException"/> class.
        /// </summary>
        public AttemptRateLimiterRejectedException()
            : base("The operation could not be executed because it was rejected by the attempt rate limiter.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptRateLimiterRejectedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AttemptRateLimiterRejectedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiterRejectedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public AttemptRateLimiterRejectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
