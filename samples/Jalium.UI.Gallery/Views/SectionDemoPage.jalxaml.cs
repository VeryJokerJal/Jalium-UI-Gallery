using Jalium.UI.Controls;
using Jalium.UI.Markup;

namespace Jalium.UI.Gallery.Views;

public partial class SectionDemoPage : Page
{
    public SectionDemoPage()
    {
        InitializeComponent();
        Unloaded += (_, _) => RazorExpressionRegistry.UnregisterSection("PaneFooterSection");
    }
}
