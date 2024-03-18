namespace Polly.Extensibility.Rate
{
    internal sealed class RollingRateMetrics : RateMetrics
    {
        private readonly object _lock = new();
        private readonly TimeSpan _samplingDuration;
        private readonly TimeSpan _windowDuration;
        private readonly Queue<RateWindow> _windows;

        private RateWindow? _currentWindow;

        public RollingRateMetrics(TimeSpan samplingDuration, short numberOfWindows, TimeProvider timeProvider)
            : base(timeProvider)
        {
            _samplingDuration = samplingDuration;
            _windowDuration = TimeSpan.FromTicks(_samplingDuration.Ticks / numberOfWindows);
            _windows = new Queue<RateWindow>();
        }

        public override void IncrementPrimaryAttempts() => UpdateCurrentWindow().PrimaryAttempts++;

        public override void IncrementSecondaryAttempts() => UpdateCurrentWindow().SecondaryAttempts++;

        public override void Reset()
        {
            _currentWindow = null;
            _windows.Clear();
        }

        public override RateInfo GetRateInfo()
        {
            UpdateCurrentWindow();

            var primaryAttempts = 0;
            var secondaryAttempts = 0;
            foreach (var window in _windows)
            {
                primaryAttempts += window.PrimaryAttempts;
                secondaryAttempts += window.SecondaryAttempts;
            }

            return RateInfo.Create(primaryAttempts, secondaryAttempts);
        }

        private RateWindow UpdateCurrentWindow()
        {
            var now = TimeProvider.GetUtcNow();
            if (_currentWindow == null || now - _currentWindow.StartedAt >= _windowDuration)
            {
                _currentWindow = new()
                {
                    StartedAt = now
                };
                _windows.Enqueue(_currentWindow);
            }

            while (now - _windows.Peek().StartedAt >= _samplingDuration)
            {
                _windows.Dequeue();
            }

            return _currentWindow;
        }

        private sealed class RateWindow
        {
            public int PrimaryAttempts { get; set; }

            public int SecondaryAttempts { get; set; }

            public DateTimeOffset StartedAt { get; set; }
        }
    }
}
