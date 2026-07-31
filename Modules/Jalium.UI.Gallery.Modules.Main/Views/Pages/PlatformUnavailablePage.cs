using System.Runtime.InteropServices;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Themes;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

internal sealed class PlatformUnavailablePage : Page
{
    internal PlatformUnavailablePage(
        string feature,
        string reason,
        string guidance)
    {
        Content = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(8),
            Children =
            {
                new TextBlock
                {
                    Text = feature,
                    FontSize = 32,
                    Foreground = GalleryTheme.TextPrimaryBrush,
                    Margin = new Thickness(0, 0, 0, 8)
                },
                new TextBlock
                {
                    Text = "Unavailable on this platform",
                    FontSize = 16,
                    Foreground = GalleryTheme.WarningBrush,
                    Margin = new Thickness(0, 0, 0, 24)
                },
                new Border
                {
                    Background = GalleryTheme.BackgroundCardBrush,
                    BorderBrush = GalleryTheme.BorderDefaultBrush,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(20),
                    Child = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = reason,
                                TextWrapping = TextWrapping.Wrap,
                                FontSize = 15,
                                Foreground = GalleryTheme.TextPrimaryBrush,
                                Margin = new Thickness(0, 0, 0, 12)
                            },
                            new TextBlock
                            {
                                Text = guidance,
                                TextWrapping = TextWrapping.Wrap,
                                FontSize = 13,
                                Foreground = GalleryTheme.TextSecondaryBrush,
                                Margin = new Thickness(0, 0, 0, 16)
                            },
                            new TextBlock
                            {
                                Text = $"Runtime: {RuntimeInformation.OSDescription} ({RuntimeInformation.RuntimeIdentifier})",
                                FontSize = 12,
                                Foreground = GalleryTheme.TextMutedBrush
                            }
                        }
                    }
                }
            }
        };
    }
}
