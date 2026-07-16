using Jalium.UI;
using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Declarative fallback content used when a gallery page is unavailable or fails to load.
/// </summary>
public partial class NavigationStatePage : Page
{
    public NavigationStatePage(string pageTag)
    {
        InitializeComponent();
        StateTitleText.Text = pageTag;
        StateDetailText.Text = "This page is coming soon.";
    }

    public NavigationStatePage(string pageTag, Exception exception)
    {
        InitializeComponent();
        PlaceholderIconContainer.Visibility = Visibility.Collapsed;
        ErrorIconContainer.Visibility = Visibility.Visible;
        StateTitleText.Text = $"{pageTag} - Load failed";
        StateDetailText.Text = $"{exception.GetType().Name}: {exception.Message}";
    }
}
