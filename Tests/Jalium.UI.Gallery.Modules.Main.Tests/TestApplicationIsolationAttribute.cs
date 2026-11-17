using System.Reflection;
using Jalium.UI.Controls.Themes;
using Xunit.v3;

[assembly: Jalium.UI.Gallery.Modules.Main.Tests.TestApplicationIsolation]

namespace Jalium.UI.Gallery.Modules.Main.Tests;

// Serial collections can still run on different worker threads. A test that
// creates an Application must not leave its dispatcher-owned theme dictionaries
// for the next test. Clean up after the test so fixture constructors retain the
// application context they deliberately create for their own assertions.
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class TestApplicationIsolationAttribute : BeforeAfterTestAttribute
{
    private static readonly FieldInfo CurrentApplication = typeof(Application).GetField(
        "_current", BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly Action ResetTheme = typeof(ThemeManager).GetMethod(
        "Reset", BindingFlags.Static | BindingFlags.NonPublic)!.CreateDelegate<Action>();

    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        CurrentApplication.SetValue(null, null);
        ResetTheme();
    }
}
