namespace Polly.Extensibility.Rate
{
    internal readonly record struct RateInfo(int Throughput, double SecondaryAttemptRate, int SecondaryAttemptCount)
    {
        public static RateInfo Create(int primaryAttempt, int secondaryAttempt)
        {
            var total = primaryAttempt + secondaryAttempt;
            if (total == 0)
            {
                return new RateInfo(0, 0, secondaryAttempt);
            }

            return new(total, secondaryAttempt / (double)total, secondaryAttempt);
        }
    }
}
