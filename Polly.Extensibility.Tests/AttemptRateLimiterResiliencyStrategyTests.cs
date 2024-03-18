using Polly.Extensibility.Rate;
using Polly.Hedging;
using Polly.Retry;

namespace Polly.Extensibility.Tests
{
    [TestClass]
    public class AttemptRateLimiterResiliencyStrategyTests
    {
        private readonly RetryStrategyOptions _defaultRetryOptions = new();
        private readonly HedgingStrategyOptions<int> _defaultHedgingStrategyOptions = new();
        private RateMetrics _rateMetrics;
        private AttemptRateLimiterStrategyOptions _options;

        [TestInitialize]
        public void Initialize()
        {
            _options = new AttemptRateLimiterStrategyOptions
            {
                SecondaryAttemptRatio = 0.5,
                MinimumThroughput = 2
            };
            _rateMetrics = RateMetrics.Create(_options.SamplingDuration, TimeProvider.System);
        }

        [TestMethod]
        public void Execute_PipelineWithRetryStrategy_ShouldNotRejectPrimaryAttempt()
        {
            int retries = 0;
            _defaultRetryOptions.OnRetry = _ => { retries++; return default; };
            var pipeline = CreatePipelineWithRetryStrategy();

            var result = pipeline.Execute(() => 1);

            Assert.AreEqual(1, result);
            Assert.AreEqual(0, retries);
        }

        [TestMethod]
        public void Execute_PipelineWithRetryStrategy_SecondaryAttemptRatioReached_ShouldRejectSecondaryAttempt()
        {
            int retries = 0;
            _defaultRetryOptions.OnRetry = _ => { retries++; return default; };
            _defaultRetryOptions.ShouldHandle = args => args.Outcome.Exception switch
            {
                AttemptRateLimiterRejectedException => PredicateResult.False(),
                _ => PredicateResult.True()
            };
            _defaultRetryOptions.MaxRetryAttempts = 12;
            var pipeline = CreatePipelineWithRetryStrategy();

            Assert.ThrowsException<AttemptRateLimiterRejectedException>(() => pipeline.Execute(() => 0));

            Assert.AreEqual(2, retries);
        }

        [TestMethod]
        public void Execute_PipelineWithHedgingStrategy_ShouldNotRejectPrimaryAttempt()
        {
            int hedging = 0;
            _defaultHedgingStrategyOptions.OnHedging = _ => { hedging++; return default; };
            var pipeline = CreatePipelineWithHedgingStrategy();

            var result = pipeline.Execute(() => 1);

            Assert.AreEqual(1, result);
            Assert.AreEqual(0, hedging);
        }

        [TestMethod]
        public void Execute_PipelineWithHedgingStrategy_SecondaryAttemptRatioReached_ShouldRejectSecondaryAttempt()
        {
            int hedging = 0;
            _defaultHedgingStrategyOptions.OnHedging = _ => { hedging++; return default; };
            _defaultHedgingStrategyOptions.ShouldHandle = args => args.Outcome.Exception switch
            {
                AttemptRateLimiterRejectedException => PredicateResult.False(),
                _ => PredicateResult.True()
            };
            _defaultHedgingStrategyOptions.MaxHedgedAttempts = 3;

            var pipeline = CreatePipelineWithHedgingStrategy();

            Assert.ThrowsException<AttemptRateLimiterRejectedException>(() => pipeline.Execute(() => 0));

            Assert.AreEqual(2, hedging);
        }

        private ResiliencePipeline CreatePipelineWithRetryStrategy()
        {
            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(_defaultRetryOptions)
                .AddStrategy(context => new AttemptRateLimiterResiliencyStrategy(_rateMetrics, _options), _options)
                .Build();

            return pipeline;
        }

        private ResiliencePipeline<int> CreatePipelineWithHedgingStrategy()
        {
            var pipeline = new ResiliencePipelineBuilder<int>()
                .AddHedging(_defaultHedgingStrategyOptions)
                .AddStrategy(context => new AttemptRateLimiterResiliencyStrategy(_rateMetrics, _options), _options)
                .Build();

            return pipeline;
        }
    }
}
