using Polly.Extensibility.Rate;
using Polly.Extensibility.Utils;

namespace Polly.Extensibility
{
    internal sealed class AttemptRateLimiterResiliencyStrategy : ResilienceStrategy
    {
        private readonly object _lock = new();
        private readonly RateMetrics _rateMetrics;

        public AttemptRateLimiterResiliencyStrategy(
            RateMetrics rateMetrics,
            AttemptRateLimiterStrategyOptions options)
        {
            _rateMetrics = rateMetrics;
            SecondaryAttemptRatio = options.SecondaryAttemptRatio;
            MinimumThroughput = options.MinimumThroughput;
            OnRejected = options.OnRejected;
        }

        public double SecondaryAttemptRatio { get; }

        public int MinimumThroughput { get; }

        public Func<OnAttemptRateLimiterRejectedArguments, ValueTask>? OnRejected { get; }

        protected override async ValueTask<Outcome<TResult>> ExecuteCore<TResult, TState>(Func<ResilienceContext, TState, ValueTask<Outcome<TResult>>> callback, ResilienceContext context, TState state)
        {
            if (context.Properties.TryGetValue(AttemptRateLimiterConstants.AttemptNumber, out int attemptNumber) && attemptNumber > 0)
            {
                RateInfo info;
                lock (_lock)
                {
                    info = _rateMetrics.GetRateInfo();
                }

                if (info.Throughput >= MinimumThroughput && info.SecondaryAttemptRate >= SecondaryAttemptRatio)
                {
                    if (OnRejected != null)
                    {
                        await OnRejected(new OnAttemptRateLimiterRejectedArguments(context)).ConfigureAwait(context.ContinueOnCapturedContext);
                    }

                    var exception = new AttemptRateLimiterRejectedException();

                    return Outcome.FromException<TResult>(exception.TrySetStackTrace());
                }

                // track metrics only when "accepted for processing"
                lock (_lock)
                {
                    _rateMetrics.IncrementSecondaryAttempts();
                }
            }
            else
            {
                lock (_lock)
                {
                    _rateMetrics.IncrementPrimaryAttempts();
                }
            }

            context.Properties.Set(AttemptRateLimiterConstants.AttemptNumber, ++attemptNumber);

            return await StrategyHelper.ExecuteCallbackSafeAsync(callback, context, state).ConfigureAwait(context.ContinueOnCapturedContext);
        }
    }
}
