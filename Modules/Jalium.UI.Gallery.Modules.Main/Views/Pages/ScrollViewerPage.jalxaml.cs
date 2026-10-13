using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class ScrollViewerPage : Page
{

    public ScrollViewerPage()
    {
        InitializeComponent();
        if (!OperatingSystem.IsMacOS())
            BouncePreviewControls.Visibility = Visibility.Collapsed;
        Unloaded += OnPreviewUnloaded;
    }

    private void OnScrollLeft(object sender, RoutedEventArgs e) => HorizontalPreviewViewer.PageLeft();

    private void OnScrollRight(object sender, RoutedEventArgs e) => HorizontalPreviewViewer.PageRight();

    private void OnPullTopEdge(object sender, RoutedEventArgs e)
    {
        BouncePreviewViewer.ScrollToVerticalOffset(0);
        for (Visual? parent = VisualParent as Visual; parent != null; parent = parent.VisualParent)
            if (parent is ScrollViewer viewer) viewer.ScrollToVerticalOffset(0);
        RaisePreviewGesture(240, MouseWheelPhase.Began);
        ReleaseBounceButton.IsEnabled = true;
    }

    private void OnReleaseTopEdge(object sender, RoutedEventArgs e)
    {
        bool returnFocus = ReleaseBounceButton.IsKeyboardFocusWithin;
        RaisePreviewGesture(0, MouseWheelPhase.Ended);
        ReleaseBounceButton.IsEnabled = false;
        if (returnFocus)
            PullBounceButton.Focus();
    }

    private void OnPreviewUnloaded(object sender, RoutedEventArgs e) => ReleaseBounceButton.IsEnabled = false;

    private void RaisePreviewGesture(double delta, MouseWheelPhase phase)
    {
        var wheel = new MouseWheelEventArgs(UIElement.PreviewMouseWheelEvent, new Point(20, 20), 0, delta, true,
            MouseButtonState.Released, MouseButtonState.Released, MouseButtonState.Released,
            MouseButtonState.Released, MouseButtonState.Released, ModifierKeys.None, Environment.TickCount,
            phase, MouseWheelPhase.None);
        BouncePreviewViewer.RaiseEvent(wheel);
        wheel.RoutedEvent = UIElement.MouseWheelEvent;
        BouncePreviewViewer.RaiseEvent(wheel);
    }
}
