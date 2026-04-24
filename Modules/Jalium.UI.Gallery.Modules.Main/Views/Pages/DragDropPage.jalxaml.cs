using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Input;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Demonstrates DragEnter, DragOver, DragLeave, and Drop functionality.
/// </summary>
public partial class DragDropPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- Drag Source Items -->
    <TextBlock Text=""Drag Source"" FontSize=""14"" Margin=""0,0,0,8""/>
    <StackPanel Orientation=""Horizontal"" Margin=""0,0,0,16"">
        <Border x:Name=""DragItem1""
                Background=""#0078D4""
                CornerRadius=""4""
                Padding=""12,8""
                Margin=""0,0,8,0""
                Cursor=""Hand"">
            <TextBlock Text=""Item A"" Foreground=""#FFFFFF""/>
        </Border>
        <Border x:Name=""DragItem2""
                Background=""#107C10""
                CornerRadius=""4""
                Padding=""12,8""
                Margin=""0,0,8,0""
                Cursor=""Hand"">
            <TextBlock Text=""Item B"" Foreground=""#FFFFFF""/>
        </Border>
        <Border x:Name=""DragItem3""
                Background=""#D83B01""
                CornerRadius=""4""
                Padding=""12,8""
                Cursor=""Hand"">
            <TextBlock Text=""Item C"" Foreground=""#FFFFFF""/>
        </Border>
    </StackPanel>

    <!-- Drop Target Area -->
    <TextBlock Text=""Drop Target"" FontSize=""14"" Margin=""0,0,0,8""/>
    <Border x:Name=""DropTarget""
            Background=""#252525""
            BorderBrush=""#3D3D3D""
            BorderThickness=""2""
            CornerRadius=""8""
            MinHeight=""120""
            Padding=""16""
            AllowDrop=""True"">
        <TextBlock x:Name=""DropHint""
                   Text=""Drop items here""
                   Foreground=""#555555""
                   HorizontalAlignment=""Center""
                   VerticalAlignment=""Center""/>
    </Border>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Input;
using Jalium.UI.Media;

public partial class DragDropSample : Page
{
    public DragDropSample()
    {
        InitializeComponent();
        SetupDragDrop();
    }

    private void SetupDragDrop()
    {
        // Setup drag source - initiate drag on mouse down
        DragItem1.MouseDown += (s, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject();
                data.SetData(""Text"", ""Item A"");
                DragDrop.DoDragDrop(DragItem1, data, DragDropEffects.Copy);
            }
        };

        // Setup drop target events
        DropTarget.DragEnter += (s, e) =>
        {
            DropTarget.BorderBrush = new SolidColorBrush(
                Color.FromRgb(0x00, 0x78, 0xD4));
            DropTarget.Background = new SolidColorBrush(
                Color.FromRgb(0x00, 0x2B, 0x4E));
            DropHint.Text = ""Release to drop"";
            e.Effects = DragDropEffects.Copy;
        };

        DropTarget.DragLeave += (s, e) =>
        {
            ResetDropTargetVisuals();
        };

        DropTarget.Drop += (s, e) =>
        {
            var itemName = e.Data?.GetData(""Text"") as string;
            DropHint.Text = $""Dropped: {itemName}"";
            ResetDropTargetVisuals();

            // Add the dropped item to a collection
            DroppedItems.Add(itemName ?? ""Unknown"");
        };
    }

    private void ResetDropTargetVisuals()
    {
        DropTarget.BorderBrush = new SolidColorBrush(
            Color.FromRgb(0x3D, 0x3D, 0x3D));
        DropTarget.Background = new SolidColorBrush(
            Color.FromRgb(0x25, 0x25, 0x25));
    }
}";

    private int _logLineCount;
    private readonly SolidColorBrush _defaultZoneBorder = new(Color.FromRgb(0x3D, 0x3D, 0x3D));
    private readonly SolidColorBrush _highlightZoneBorder = new(Color.FromRgb(0x00, 0x78, 0xD4));
    private readonly SolidColorBrush _defaultZoneBackground = new(Color.FromRgb(0x25, 0x25, 0x25));
    private readonly SolidColorBrush _highlightZoneBackground = new(Color.FromRgb(0x00, 0x2B, 0x4E));

    public DragDropPage()
    {
        InitializeComponent();
        SetupDragSources();
        SetupDropTargets();
        SetupFeedbackDemo();
        SetupClearButton();
        LoadCodeExamples();
    }

    private void LoadCodeExamples()
    {
        if (XamlCodeEditor != null)
        {
            XamlCodeEditor.SyntaxHighlighter = JalxamlSyntaxHighlighter.Create();
            XamlCodeEditor.LoadText(XamlExample);
        }
        if (CSharpCodeEditor != null)
        {
            CSharpCodeEditor.SyntaxHighlighter = RegexSyntaxHighlighter.CreateCSharpHighlighter();
            CSharpCodeEditor.LoadText(CSharpExample);
        }
    }

    private void SetupDragSources()
    {
        SetupDragSource(DragItem1, "Item A");
        SetupDragSource(DragItem2, "Item B");
        SetupDragSource(DragItem3, "Item C");
    }

    private void SetupDragSource(Border? source, string itemName)
    {
        if (source == null) return;

        source.MouseDown += (s, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject();
                data.SetData("Text", itemName);
                DragDrop.DoDragDrop(source, data, DragDropEffects.Copy);
            }
        };
    }

    private void SetupDropTargets()
    {
        if (DropTarget == null) return;

        DropTarget.DragEnter += OnDropTargetDragEnter;
        DropTarget.DragOver += OnDropTargetDragOver;
        DropTarget.DragLeave += OnDropTargetDragLeave;
        DropTarget.Drop += OnDropTargetDrop;
    }

    private void OnDropTargetDragEnter(object sender, DragEventArgs e)
    {
        if (DropTarget != null)
        {
            DropTarget.BorderBrush = _highlightZoneBorder;
            DropTarget.Background = _highlightZoneBackground;
        }
        if (DropTargetHint != null)
            DropTargetHint.Text = "Release to drop";
        e.Effects = DragDropEffects.Copy;
        LogEvent("DragEnter", "DropTarget");
    }

    private void OnDropTargetDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        // DragOver fires continuously; only log occasionally to avoid flooding
    }

    private void OnDropTargetDragLeave(object sender, DragEventArgs e)
    {
        if (DropTarget != null)
        {
            DropTarget.BorderBrush = _defaultZoneBorder;
            DropTarget.Background = _defaultZoneBackground;
        }
        if (DropTargetHint != null)
            DropTargetHint.Text = "Drop items here";
        LogEvent("DragLeave", "DropTarget");
    }

    private void OnDropTargetDrop(object sender, DragEventArgs e)
    {
        if (DropTarget != null)
        {
            DropTarget.BorderBrush = _defaultZoneBorder;
            DropTarget.Background = _defaultZoneBackground;
        }

        var itemName = e.Data?.GetData("Text") as string ?? "Unknown";

        // Add dropped item to the panel
        if (DroppedItemsPanel != null)
        {
            var itemBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x0F, 0x6C, 0xBD)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10, 6, 10, 6),
                Margin = new Thickness(0, 0, 0, 4)
            };
            var itemText = new TextBlock
            {
                Text = itemName,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 13
            };
            itemBorder.Child = itemText;
            DroppedItemsPanel.Children.Add(itemBorder);
        }

        if (DropTargetHint != null)
            DropTargetHint.Text = "Drop items here";

        LogEvent("Drop", $"DropTarget (received: {itemName})");
    }

    private void SetupFeedbackDemo()
    {
        SetupDragSource(FeedbackDragSource, "Feedback Item");
        SetupFeedbackZone(FeedbackZone1, FeedbackZone1Text, "Zone 1");
        SetupFeedbackZone(FeedbackZone2, FeedbackZone2Text, "Zone 2");
        SetupFeedbackZone(FeedbackZone3, FeedbackZone3Text, "Zone 3");
    }

    private void SetupFeedbackZone(Border? zone, TextBlock? label, string zoneName)
    {
        if (zone == null) return;

        zone.DragEnter += (s, e) =>
        {
            zone.BorderBrush = _highlightZoneBorder;
            zone.Background = _highlightZoneBackground;
            if (label != null)
            {
                label.Text = $"{zoneName} (over)";
                label.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD4));
            }
            e.Effects = DragDropEffects.Copy;
            LogEvent("DragEnter", zoneName);
        };

        zone.DragLeave += (s, e) =>
        {
            zone.BorderBrush = _defaultZoneBorder;
            zone.Background = _defaultZoneBackground;
            if (label != null)
            {
                label.Text = zoneName;
                label.Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            }
            LogEvent("DragLeave", zoneName);
        };

        zone.Drop += (s, e) =>
        {
            zone.BorderBrush = _defaultZoneBorder;
            zone.Background = _defaultZoneBackground;
            if (label != null)
            {
                label.Text = zoneName;
                label.Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            }
            var item = e.Data?.GetData("Text") as string ?? "?";
            LogEvent("Drop", $"{zoneName} (received: {item})");
        };
    }

    private void SetupClearButton()
    {
        if (ClearLogButton != null)
        {
            ClearLogButton.Click += (s, e) =>
            {
                _logLineCount = 0;
                if (EventLogText != null)
                    EventLogText.Text = "(drag items to see events)";
            };
        }
    }

    private void LogEvent(string eventName, string target)
    {
        if (EventLogText == null) return;

        _logLineCount++;
        if (_logLineCount == 1)
            EventLogText.Text = $"[{eventName}] {target}";
        else
            EventLogText.Text += $"\n[{eventName}] {target}";

        // Keep log from growing too large
        if (_logLineCount > 50)
        {
            var lines = EventLogText.Text.Split('\n');
            EventLogText.Text = string.Join('\n', lines.AsSpan(lines.Length - 30));
            _logLineCount = 30;
        }

        // Auto-scroll to bottom
        LogScrollViewer?.ScrollToEnd();
    }
}
