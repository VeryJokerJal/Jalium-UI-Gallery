using System.Diagnostics;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class ExternalUriService : IExternalUriService
{
    public void Open(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        Process.Start(new ProcessStartInfo
        {
            FileName = uri,
            UseShellExecute = true
        });
    }
}
