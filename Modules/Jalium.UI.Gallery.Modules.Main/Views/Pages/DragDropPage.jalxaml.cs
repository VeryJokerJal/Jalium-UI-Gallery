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

    // Shell enhancement 1 — a caller-supplied drag image on the source.
    private void StartDragWithCustomImage(Border source, ImageSource image)
    {
        var data = new DataObject();
        data.SetData(""Text"", ""Custom Image Item"");
        // The pointer sits at (16, 24) within the image.
        DragDrop.DoDragDrop(source, data, DragDropEffects.Copy, image, new Point(16, 24));
    }

    // Shell enhancement 2 — a Shell drop description on the target. Works for
    // external OLE drags (e.g. files from Explorer); Windows also renders its
    // own drag image automatically.
    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            // %1 is replaced with the insert text → ""Copy to Gallery demo"".
            e.SetDropDescription(DropImageType.Copy, ""Copy to %1"", ""Gallery demo"");
        }
        else
        {
            e.Effects = DragDropEffects.None;
            e.SetDropDescription(DropImageType.None, ""Only files are accepted"");
        }
    }

    // Shell enhancement 3 — drag files OUT to Explorer / other apps with a real
    // OLE drag source (DoShellDragDrop), showing the Windows system drag image.
    private void StartDragOut(Border source, string filePath, ImageSource image)
    {
        var data = new DataObject();
        data.SetData(DataFormats.FileDrop, new[] { filePath });
        var effect = DragDrop.DoShellDragDrop(source, data, DragDropEffects.Copy, image, new Point(16, 24));
        // effect == Copy when the file was dropped into a folder, None if cancelled.
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
        SetupCustomImageDemo();
        SetupShellDropDemo();
        SetupDragOutDemo();
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

    // ---- Custom drag image (drag source Shell enhancement) ----

    private BitmapImage? _dragBadge;

    private void SetupCustomImageDemo()
    {
        if (CustomImageDragSource != null)
        {
            CustomImageDragSource.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var data = new DataObject();
                    data.SetData("Text", "Custom Image Item");

                    // Build the drag image once and reuse it. The pointer sits 16px in
                    // from the badge's top-left corner so the badge trails the cursor.
                    _dragBadge ??= CreateBadgeBitmap();
                    DragDrop.DoDragDrop(CustomImageDragSource, data, DragDropEffects.Copy, _dragBadge, new Point(16, 24));
                }
            };
        }

        if (CustomImageDropTarget != null)
        {
            CustomImageDropTarget.DragEnter += (s, e) =>
            {
                CustomImageDropTarget.BorderBrush = _highlightZoneBorder;
                CustomImageDropTarget.Background = _highlightZoneBackground;
                e.Effects = DragDropEffects.Copy;
            };
            CustomImageDropTarget.DragOver += (s, e) => e.Effects = DragDropEffects.Copy;
            CustomImageDropTarget.DragLeave += (s, e) =>
            {
                CustomImageDropTarget.BorderBrush = _defaultZoneBorder;
                CustomImageDropTarget.Background = _defaultZoneBackground;
            };
            CustomImageDropTarget.Drop += (s, e) =>
            {
                CustomImageDropTarget.BorderBrush = _defaultZoneBorder;
                CustomImageDropTarget.Background = _defaultZoneBackground;
                var item = e.Data?.GetData("Text") as string ?? "?";
                if (CustomImageDropHint != null)
                    CustomImageDropHint.Text = $"Dropped: {item}";
                LogEvent("Drop", $"CustomImageDropTarget (received: {item})");
            };
        }
    }

    /// <summary>
    /// Creates a small blue vertical-gradient badge (with a white border) as a
    /// straight-BGRA <see cref="BitmapImage"/> to use as a custom drag image.
    /// </summary>
    private static BitmapImage CreateBadgeBitmap()
    {
        const int w = 168, h = 48, border = 2;
        var px = new byte[w * h * 4];
        for (int y = 0; y < h; y++)
        {
            double t = (double)y / (h - 1);
            // #2563EB (top) → #1E3A8A (bottom).
            byte r = (byte)(0x25 + ((0x1E - 0x25) * t));
            byte g = (byte)(0x63 + ((0x3A - 0x63) * t));
            byte b = (byte)(0xEB + ((0x8A - 0xEB) * t));
            bool edgeRow = y < border || y >= h - border;
            for (int x = 0; x < w; x++)
            {
                bool edge = edgeRow || x < border || x >= w - border;
                int i = ((y * w) + x) * 4;
                px[i + 0] = edge ? (byte)0xFF : b; // B
                px[i + 1] = edge ? (byte)0xFF : g; // G
                px[i + 2] = edge ? (byte)0xFF : r; // R
                px[i + 3] = 0xFF;                  // A (opaque)
            }
        }
        return BitmapImage.FromPixels(px, w, h);
    }

    // ---- Windows Shell drop (external files + drop description) ----

    private void SetupShellDropDemo()
    {
        if (ShellDropTarget == null) return;

        ShellDropTarget.DragEnter += OnShellDragEnter;
        ShellDropTarget.DragOver += OnShellDragOver;
        ShellDropTarget.DragLeave += OnShellDragLeave;
        ShellDropTarget.Drop += OnShellDrop;
    }

    private void OnShellDragEnter(object sender, DragEventArgs e)
    {
        if (ShellDropTarget != null)
        {
            ShellDropTarget.BorderBrush = _highlightZoneBorder;
            ShellDropTarget.Background = _highlightZoneBackground;
        }
        ApplyShellDropFeedback(e);
        LogEvent("DragEnter", "ShellDropTarget");
    }

    private void OnShellDragOver(object sender, DragEventArgs e) => ApplyShellDropFeedback(e);

    /// <summary>
    /// Sets the effect and Shell drop description based on whether files are being
    /// dragged. The description only appears for external OLE drags (e.g. from
    /// Explorer); for a purely in-app drag it is a silent no-op.
    /// </summary>
    private static void ApplyShellDropFeedback(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            // The Shell substitutes %1 with the insert text → "Copy to Gallery demo".
            e.SetDropDescription(DropImageType.Copy, "Copy to %1", "Gallery demo");
        }
        else
        {
            e.Effects = DragDropEffects.None;
            e.SetDropDescription(DropImageType.None, "Only files are accepted");
        }
    }

    private void OnShellDragLeave(object sender, DragEventArgs e)
    {
        if (ShellDropTarget != null)
        {
            ShellDropTarget.BorderBrush = _defaultZoneBorder;
            ShellDropTarget.Background = _defaultZoneBackground;
        }
        LogEvent("DragLeave", "ShellDropTarget");
    }

    private void OnShellDrop(object sender, DragEventArgs e)
    {
        if (ShellDropTarget != null)
        {
            ShellDropTarget.BorderBrush = _defaultZoneBorder;
            ShellDropTarget.Background = _defaultZoneBackground;
        }

        var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
        if (files is { Length: > 0 })
        {
            if (ShellDropHint != null)
                ShellDropHint.Text = $"Dropped {files.Length} file(s):";
            if (ShellDroppedFilesPanel != null)
            {
                ShellDroppedFilesPanel.Children.Clear();
                foreach (var file in files)
                {
                    ShellDroppedFilesPanel.Children.Add(new TextBlock
                    {
                        Text = System.IO.Path.GetFileName(file),
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Colors.White),
                        Margin = new Thickness(0, 2, 0, 0),
                    });
                }
            }
            LogEvent("Drop", $"ShellDropTarget (received {files.Length} file(s))");
        }
        else
        {
            var text = e.Data?.GetData("Text") as string;
            if (ShellDropHint != null)
                ShellDropHint.Text = text != null ? $"Dropped text: {text}" : "Drag files here from Explorer";
            LogEvent("Drop", "ShellDropTarget (no files)");
        }
    }

    // ---- Cross-process drag out to Explorer (real OLE drag source) ----

    private void SetupDragOutDemo()
    {
        if (DragOutSource == null) return;

        DragOutSource.MouseDown += (s, e) =>
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            string? path = CreateTempDragFile();
            if (path == null)
            {
                if (DragOutStatus != null)
                    DragOutStatus.Text = "Could not create the temp file.";
                return;
            }

            var data = new DataObject();
            data.SetData(DataFormats.FileDrop, new[] { path });
            data.SetText(System.IO.Path.GetFileName(path));

            _dragBadge ??= CreateBadgeBitmap();

            // Real OLE drag: the file can be dropped onto Explorer or any other app.
            var effect = DragDrop.DoShellDragDrop(DragOutSource, data, DragDropEffects.Copy, _dragBadge, new Point(16, 24));

            if (DragOutStatus != null)
                DragOutStatus.Text = effect == DragDropEffects.None
                    ? "Drag cancelled (dropped nowhere)."
                    : $"Dropped out with effect: {effect}.";
            LogEvent("DoShellDragDrop", $"DragOutSource → {effect}");
        };
    }

    /// <summary>
    /// Writes a small text file into the temp folder to hand to the Shell so the
    /// cross-process drag has something real to copy. Reused across drags.
    /// </summary>
    private static string? CreateTempDragFile()
    {
        try
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "JaliumGalleryDragDrop.txt");
            System.IO.File.WriteAllText(path, "Hello from the Jalium.UI Gallery — dragged out via DragDrop.DoShellDragDrop.");
            return path;
        }
        catch
        {
            return null;
        }
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
