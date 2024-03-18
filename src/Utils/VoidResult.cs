namespace Polly.Extensibility.Utils
{
    internal sealed class VoidResult
    {
        private VoidResult()
        {
        }

        public static readonly VoidResult Instance = new();

        public override string ToString() => "void";
    }
}
