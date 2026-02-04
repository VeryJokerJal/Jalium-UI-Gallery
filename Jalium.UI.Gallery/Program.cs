using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery;

namespace Jalium.UI.Gallery;

/// <summary>
/// Entry point for the Jalium.UI Gallery application.
/// </summary>
internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Create and run the application
        var app = new Application();

        // Create the main window
        var window = new GalleryWindow();

        // Run the application with the main window
        app.Run(window);
    }
}
