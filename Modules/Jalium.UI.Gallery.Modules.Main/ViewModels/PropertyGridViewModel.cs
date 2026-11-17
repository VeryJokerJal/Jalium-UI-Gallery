using Jalium.UI.Gallery.Modules.Main.Models;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class PropertyGridViewModel
{
    public PropertyGridButtonSample BasicSample { get; } = new();

    public PropertyGridWindowSample WindowSample { get; } = new();
}
