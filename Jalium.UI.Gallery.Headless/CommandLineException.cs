namespace Jalium.UI.Gallery.Headless;

internal sealed class CommandLineException : Exception
{
    public CommandLineException(string message)
        : base(message)
    {
    }
}
