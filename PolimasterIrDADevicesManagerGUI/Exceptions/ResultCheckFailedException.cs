namespace PolimasterIrDADevicesManagerGUI.Exceptions
{
    public sealed class ResultCheckFailedException : Exception
    {

        public ResultCheckFailedException(string message = "Result check failed") : base(message) { }

    }
}
