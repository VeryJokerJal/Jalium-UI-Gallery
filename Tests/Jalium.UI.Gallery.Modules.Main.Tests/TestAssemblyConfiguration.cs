using System.Runtime.CompilerServices;
using Jalium.UI.Markup;
using Xunit;

// Gallery tests replace process-wide Application/theme/resource state and construct
// controls on the current test thread. Running collections concurrently lets one
// fixture reset those singletons while another is still building its visual tree.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Jalium.UI.Gallery.Modules.Main.Tests;

internal static class TestRuntimeInitialization
{
    [ModuleInitializer]
    internal static void Initialize() =>
        ThemeLoader.Initialize();
}
