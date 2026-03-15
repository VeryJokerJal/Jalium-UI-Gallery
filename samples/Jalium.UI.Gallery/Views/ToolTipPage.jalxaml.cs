using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for ToolTipPage.jalxaml demonstrating tooltip functionality.
/// </summary>
public partial class ToolTipPage : Page
{
    private TextBlock? _statusText;

    public ToolTipPage()
    {
        InitializeComponent();

        // Set ToolTip programmatically to test the full flow
        if (SaveButton != null)
        {
            SaveButton.ToolTip = "Save the current document (set from code)";
            SaveButton.Content = $"Save (ToolTip={SaveButton.ToolTip != null})";
        }

        // Add diagnostic section at the top of the page
        AddDiagnostics();
    }

    private void AddDiagnostics()
    {
        // Find the root StackPanel (first visual child of Page)
        if (Content is not StackPanel rootStack) return;

        // Create diagnostic card
        var diagBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x3A, 0x1A)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x2A, 0x6A, 0x2A)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 16)
        };

        var diagStack = new StackPanel { Orientation = Orientation.Vertical };

        var titleText = new TextBlock
        {
            Text = "Diagnostics",
            FontSize = 16,
            Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0xFF, 0x80)),
            Margin = new Thickness(0, 0, 0, 8)
        };
        diagStack.Children.Add(titleText);

        _statusText = new TextBlock
        {
            Text = "Status: Waiting for interaction...",
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8)
        };
        diagStack.Children.Add(_statusText);

        // Test button 1: MouseEnter event test
        var hoverTestBtn = new Button
        {
            Content = "Hover me (MouseEnter test)",
            Width = 220,
            Height = 32,
            Margin = new Thickness(0, 0, 8, 8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        hoverTestBtn.ToolTip = "This is a test tooltip";
        hoverTestBtn.MouseEnter += (s, e) =>
        {
            UpdateStatus($"MouseEnter fired on HoverTestBtn! ToolTip={hoverTestBtn.ToolTip}");
        };
        hoverTestBtn.MouseLeave += (s, e) =>
        {
            UpdateStatus("MouseLeave fired on HoverTestBtn");
        };
        diagStack.Children.Add(hoverTestBtn);

        // Test button 2: Manual tooltip show
        var manualShowBtn = new Button
        {
            Content = "Click: Manual ToolTip Show",
            Width = 220,
            Height = 32,
            Margin = new Thickness(0, 0, 8, 8),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        manualShowBtn.Click += (s, e) =>
        {
            try
            {
                UpdateStatus("Creating ToolTip manually...");
                var tt = new ToolTip();
                tt.Content = new TextBlock
                {
                    Text = "Manual tooltip!",
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                };
                tt.PlacementTarget = manualShowBtn;
                UpdateStatus($"ToolTip created. Setting IsOpen=true...");
                tt.IsOpen = true;
                UpdateStatus($"ToolTip.IsOpen set to true. Check if visible above.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"ERROR: {ex.GetType().Name}: {ex.Message}");
            }
        };
        diagStack.Children.Add(manualShowBtn);

        // Test button 3: Manual Popup show (bypasses ToolTip entirely)
        var popupTestBtn = new Button
        {
            Content = "Click: Direct Popup Test",
            Width = 220,
            Height = 32,
            Margin = new Thickness(0, 0, 8, 0),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        popupTestBtn.Click += (s, e) =>
        {
            try
            {
                UpdateStatus("Creating Popup directly...");
                var textContent = new TextBlock
                {
                    Text = "Direct Popup Content!",
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    FontSize = 14
                };
                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0xFF, 0x60, 0x00)),
                    Padding = new Thickness(12, 8, 12, 8),
                    CornerRadius = new CornerRadius(4),
                    Child = textContent
                };
                var popup = new Popup
                {
                    Child = border,
                    PlacementTarget = popupTestBtn,
                    Placement = PlacementMode.Bottom,
                    StaysOpen = false,
                    ShouldConstrainToRootBounds = true
                };
                UpdateStatus($"Popup created. PlacementTarget={popupTestBtn.GetType().Name}, Setting IsOpen=true...");
                popup.IsOpen = true;
                UpdateStatus($"Popup.IsOpen set to true. IsOpen={popup.IsOpen}");
            }
            catch (Exception ex)
            {
                UpdateStatus($"POPUP ERROR: {ex.GetType().Name}: {ex.Message}");
            }
        };
        diagStack.Children.Add(popupTestBtn);

        // Show static constructor status
        var delegateText = new TextBlock
        {
            Text = $"ToolTip static ctor ran: {typeof(ToolTip).TypeInitializer != null}",
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.FromRgb(0x80, 0xFF, 0x80)),
            Margin = new Thickness(0, 8, 0, 0)
        };
        diagStack.Children.Add(delegateText);

        diagBorder.Child = diagStack;

        // Insert at position 2 (after title and description)
        rootStack.Children.Insert(2, diagBorder);
    }

    private void UpdateStatus(string message)
    {
        if (_statusText != null)
        {
            _statusText.Text = $"Status: {message}";
        }
        System.Diagnostics.Debug.WriteLine($"[ToolTipPage] {message}");
    }
}
