using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Gallery page showcasing the WinUI-style Icon system:
/// IconElement (base), SymbolIcon, FontIcon, PathIcon.
/// Displays 600+ icons from the Segoe Fluent Icons font.
/// </summary>
public partial class IconElementPage : Page
{
    private const string XamlExample = @"<StackPanel Orientation=""Vertical"" Margin=""16"">
    <!-- SymbolIcon using the Symbol enum -->
    <SymbolIcon Symbol=""Save""
                Width=""24"" Height=""24""
                Foreground=""#FFFFFF""
                Margin=""0,0,0,16""/>

    <!-- FontIcon with custom glyph code -->
    <FontIcon Glyph=""&#xE713;""
              FontSize=""24""
              Foreground=""#FFFFFF""
              Margin=""0,0,0,16""/>

    <!-- FontIcon with custom font family and size -->
    <FontIcon Glyph=""&#xE768;""
              FontSize=""32""
              Foreground=""#9ECE6A""
              Margin=""0,0,0,16""/>

    <!-- AppBarButton with SymbolIcon -->
    <AppBarButton Label=""Save"" IsCompact=""True"">
        <AppBarButton.Icon>
            <SymbolIcon Symbol=""Save""/>
        </AppBarButton.Icon>
    </AppBarButton>

    <!-- Multiple icons in a toolbar -->
    <StackPanel Orientation=""Horizontal"">
        <AppBarButton Icon=""{SymbolIcon Save}"" Label=""Save""/>
        <AppBarButton Icon=""{SymbolIcon Undo}"" Label=""Undo""/>
        <AppBarButton Icon=""{SymbolIcon Redo}"" Label=""Redo""/>
        <AppBarSeparator Height=""48""/>
        <AppBarButton Icon=""{SymbolIcon Play}"" Label=""Start""/>
    </StackPanel>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls;
using Jalium.UI.Media;

public partial class IconElementSample : Page
{
    public IconElementSample()
    {
        InitializeComponent();
        CreateIconsInCode();
    }

    private void CreateIconsInCode()
    {
        // Create a SymbolIcon from the Symbol enum
        var saveIcon = new SymbolIcon(Symbol.Save)
        {
            Width = 24,
            Height = 24,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF))
        };

        // Create a FontIcon with a glyph code
        var settingsIcon = new FontIcon
        {
            Glyph = ""\uE713"",
            FontSize = 24,
            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF))
        };

        // Use icons in AppBarButton
        var button = new AppBarButton
        {
            Icon = new SymbolIcon(Symbol.Save),
            Label = ""Save"",
            IsCompact = true
        };

        // Change icon color dynamically
        saveIcon.Foreground = new SolidColorBrush(
            Color.FromRgb(0x9E, 0xCE, 0x6A));
    }
}";

    private static readonly SolidColorBrush White = new(Color.FromRgb(0xFF, 0xFF, 0xFF));
    private static readonly SolidColorBrush Gray = new(Color.FromRgb(0x88, 0x88, 0x88));
    private static readonly SolidColorBrush DimGray = new(Color.FromRgb(0x66, 0x66, 0x66));
    private static readonly SolidColorBrush CardBg = new(Color.FromRgb(0x2D, 0x2D, 0x2D));
    private static readonly SolidColorBrush CardBorder = new(Color.FromRgb(0x3D, 0x3D, 0x3D));
    private static readonly SolidColorBrush DarkBg = new(Color.FromRgb(0x1E, 0x1E, 0x1E));
    private static readonly SolidColorBrush Accent = new(Color.FromRgb(0x7A, 0xA2, 0xF7));

    public IconElementPage()
    {
        InitializeComponent();
        LoadCodeExamples();
        if (RootPanel == null) return;

        AddHeader();
        AddSymbolIconSection();
        AddFontIconSection();
        AddAppBarButtonWithIconSection();
        AddSymbolGallerySection();
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

    private void AddHeader()
    {
        RootPanel.Children.Add(new TextBlock
        {
            Text = "IconElement",
            FontSize = 32,
            Foreground = White,
            Margin = new Thickness(0, 0, 0, 8)
        });
        RootPanel.Children.Add(new TextBlock
        {
            Text = "WinUI-style icon system: SymbolIcon, FontIcon, and PathIcon. 600+ icons from Segoe Fluent Icons, organized by category.",
            FontSize = 14,
            Foreground = Gray,
            Margin = new Thickness(0, 0, 0, 24)
        });
    }

    // ==================== Section 1: SymbolIcon ====================

    private void AddSymbolIconSection()
    {
        var card = CreateCard("SymbolIcon",
            "Use the Symbol enum to display common icons from Segoe Fluent Icons. Default size: 20x20.");

        var grid = new WrapPanel();

        Symbol[] showcase =
        [
            Symbol.Save, Symbol.OpenFile, Symbol.Document, Symbol.Delete,
            Symbol.Undo, Symbol.Redo, Symbol.Copy, Symbol.Cut, Symbol.Paste,
            Symbol.Play, Symbol.Pause, Symbol.Stop, Symbol.Refresh,
            Symbol.Find, Symbol.Search, Symbol.Settings, Symbol.Help,
            Symbol.Home, Symbol.Back, Symbol.Forward, Symbol.Add,
            Symbol.Edit, Symbol.Pin, Symbol.FavoriteStar, Symbol.Share,
            Symbol.Download, Symbol.Upload, Symbol.Print, Symbol.Lock,
        ];

        foreach (var symbol in showcase)
            grid.Children.Add(CreateSymbolTile(symbol));

        var container = new Border
        {
            Background = DarkBg,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12),
            Child = grid
        };

        ((StackPanel)card.Child!).Children.Add(container);
        RootPanel.Children.Add(card);
    }

    private static Border CreateSymbolTile(Symbol symbol)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(4)
        };

        var icon = new SymbolIcon(symbol)
        {
            Width = 24,
            Height = 24,
            Foreground = White,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        stack.Children.Add(icon);

        stack.Children.Add(new TextBlock
        {
            Text = symbol.ToString(),
            FontSize = 9,
            Foreground = DimGray,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxWidth = 72,
            Margin = new Thickness(0, 4, 0, 0)
        });

        return new Border
        {
            Width = 80,
            Height = 64,
            ClipToBounds = true,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(4, 8, 4, 4),
            Child = stack
        };
    }

    // ==================== Section 2: FontIcon ====================

    private void AddFontIconSection()
    {
        var card = CreateCard("FontIcon",
            "Use any glyph from any font. Defaults to Segoe Fluent Icons. Supports custom FontFamily and FontSize.");

        var grid = new WrapPanel();

        (string glyph, string label, double size, SolidColorBrush color)[] fontIcons =
        [
            ("\uE713", "Settings 16px", 16, White),
            ("\uE713", "Settings 24px", 24, White),
            ("\uE713", "Settings 32px", 32, White),
            ("\uE768", "Play (Green)", 24, new SolidColorBrush(Color.FromRgb(0x9E, 0xCE, 0x6A))),
            ("\uE71A", "Stop (Red)", 24, new SolidColorBrush(Color.FromRgb(0xF7, 0x76, 0x8E))),
            ("\uE7BA", "Warning (Yellow)", 24, new SolidColorBrush(Color.FromRgb(0xE0, 0xAF, 0x68))),
            ("\uE946", "Info (Blue)", 24, Accent),
        ];

        foreach (var (glyph, label, size, color) in fontIcons)
            grid.Children.Add(CreateFontIconTile(glyph, label, size, color));

        var container = new Border
        {
            Background = DarkBg,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12),
            Child = grid
        };

        ((StackPanel)card.Child!).Children.Add(container);
        RootPanel.Children.Add(card);
    }

    private static Border CreateFontIconTile(string glyph, string label, double size, SolidColorBrush color)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(4)
        };

        var icon = new FontIcon
        {
            Glyph = glyph,
            FontSize = size,
            Width = 36,
            Height = 36,
            Foreground = color,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        stack.Children.Add(icon);

        stack.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 10,
            Foreground = DimGray,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 0)
        });

        return new Border
        {
            Width = 100,
            Height = 72,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(4, 8, 4, 4),
            Child = stack
        };
    }

    // ==================== Section 3: AppBarButton + Icon ====================

    private void AddAppBarButtonWithIconSection()
    {
        var card = CreateCard("AppBarButton with SymbolIcon",
            "AppBarButton.Icon accepts IconElement (SymbolIcon, FontIcon, PathIcon) instead of raw strings.");

        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        panel.Children.Add(CreateDemoAppBarButton(Symbol.Save, "Save"));
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Undo, "Undo"));
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Redo, "Redo"));
        panel.Children.Add(new AppBarSeparator { Height = 48 });
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Play, "Start", new SolidColorBrush(Color.FromRgb(0x9E, 0xCE, 0x6A))));
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Stop, "Stop", new SolidColorBrush(Color.FromRgb(0xF7, 0x76, 0x8E))));
        panel.Children.Add(new AppBarSeparator { Height = 48 });
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Bug, "Debug"));
        panel.Children.Add(CreateDemoAppBarButton(Symbol.Find, "Search"));

        var container = new Border
        {
            Background = DarkBg,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12),
            Child = panel
        };

        ((StackPanel)card.Child!).Children.Add(container);

        ((StackPanel)card.Child!).Children.Add(new TextBlock
        {
            Text = "new AppBarButton { Icon = new SymbolIcon(Symbol.Save), Label = \"Save\" }",
            FontSize = 12,
            Foreground = Accent,
            Margin = new Thickness(0, 12, 0, 0)
        });

        RootPanel.Children.Add(card);
    }

    private static AppBarButton CreateDemoAppBarButton(Symbol symbol, string label, SolidColorBrush? foreground = null)
    {
        var btn = new AppBarButton
        {
            Icon = new SymbolIcon(symbol),
            Label = label,
            IsCompact = true,
            MinWidth = 40,
            Height = 48,
        };
        if (foreground != null)
            btn.Foreground = foreground;
        return btn;
    }

    // ==================== Section 4: Full Symbol Gallery ====================

    private void AddSymbolGallerySection()
    {
        var card = CreateCard("Symbol Enum Gallery — 600+ Icons",
            "All available Symbol values organized by category. Each maps to a Segoe Fluent Icons glyph code point.");

        var categories = new (string Name, Symbol[] Symbols)[]
        {
            ("Navigation", [
                Symbol.GlobalNavButton, Symbol.Home, Symbol.Back, Symbol.Forward, Symbol.Up, Symbol.Down,
                Symbol.ChevronDown, Symbol.ChevronUp, Symbol.ChevronLeft, Symbol.ChevronRight,
                Symbol.ChevronUpSmall, Symbol.ChevronDownSmall, Symbol.ChevronLeftSmall, Symbol.ChevronRightSmall,
                Symbol.ChevronUpMed, Symbol.ChevronDownMed, Symbol.ChevronLeftMed, Symbol.ChevronRightMed,
                Symbol.ChromeBack, Symbol.PageLeft, Symbol.PageRight,
                Symbol.ReturnToWindow, Symbol.BackToWindow, Symbol.FullScreen,
                Symbol.ArrowUp8, Symbol.ArrowDown8, Symbol.ArrowRight8, Symbol.ArrowLeft8,
            ]),
            ("Common Actions", [
                Symbol.Accept, Symbol.Cancel, Symbol.Add, Symbol.Remove, Symbol.Delete,
                Symbol.Clear, Symbol.Refresh, Symbol.Share, Symbol.Send, Symbol.SendFill,
                Symbol.Download, Symbol.Upload, Symbol.Sync, Symbol.SyncError,
                Symbol.Sort, Symbol.Filter, Symbol.Attach, Symbol.AttachCamera,
                Symbol.ClosePane, Symbol.OpenPane, Symbol.MoveToFolder,
                Symbol.Import, Symbol.ImportAll, Symbol.Permissions,
                Symbol.SelectAll, Symbol.ClearSelection, Symbol.ShowResults,
                Symbol.Trim, Symbol.Switch, Symbol.OpenWith, Symbol.OpenInNewWindow,
                Symbol.Click, Symbol.Completed, Symbol.CompletedSolid,
                Symbol.AddTo, Symbol.RemoveFrom, Symbol.Export, Symbol.Broom, Symbol.Replay,
            ]),
            ("File Operations", [
                Symbol.Document, Symbol.ProtectedDocument, Symbol.Page, Symbol.PageSolid,
                Symbol.NewFolder, Symbol.Folder, Symbol.FolderOpen, Symbol.FolderFill,
                Symbol.FolderHorizontal, Symbol.PersonalFolder,
                Symbol.Save, Symbol.SaveAs, Symbol.SaveLocal, Symbol.SaveCopy,
                Symbol.OpenFile, Symbol.OpenLocal, Symbol.NewWindow,
                Symbol.Print, Symbol.UnsyncFolder, Symbol.SyncFolder,
                Symbol.ReportDocument, Symbol.PDF,
            ]),
            ("Editing", [
                Symbol.Edit, Symbol.Copy, Symbol.Cut, Symbol.Paste,
                Symbol.Undo, Symbol.Redo, Symbol.Find, Symbol.Replace, Symbol.Rename,
                Symbol.Highlight, Symbol.HighlightFill, Symbol.FontColor,
                Symbol.Bold, Symbol.Italic, Symbol.Underline, Symbol.Strikethrough,
                Symbol.FontSize, Symbol.Font, Symbol.FontDecrease, Symbol.FontIncrease,
                Symbol.AlignLeft, Symbol.AlignCenter, Symbol.AlignRight,
                Symbol.BulletedList, Symbol.Scan, Symbol.ClearFormatting,
                Symbol.Link, Symbol.Characters, Symbol.Label,
                Symbol.Annotation, Symbol.IBeam, Symbol.BidiLtr, Symbol.BidiRtl,
            ]),
            ("Transport / Media", [
                Symbol.Play, Symbol.Pause, Symbol.Stop,
                Symbol.Record, Symbol.Record2,
                Symbol.FastForward, Symbol.Rewind, Symbol.Previous, Symbol.Next,
                Symbol.SkipBack10, Symbol.SkipForward30,
                Symbol.RepeatOne, Symbol.Shuffle,
                Symbol.Video, Symbol.VideoChat, Symbol.VideoSolid, Symbol.Video360,
                Symbol.Movies, Symbol.Streaming, Symbol.SlowMotionOn,
            ]),
            ("Volume / Audio", [
                Symbol.Volume, Symbol.Volume0, Symbol.Volume1, Symbol.Volume2, Symbol.Volume3,
                Symbol.Mute, Symbol.Speakers, Symbol.VolumeDisabled, Symbol.VolumeBars,
                Symbol.Audio, Symbol.MusicNote, Symbol.MusicAlbum, Symbol.MusicInfo,
                Symbol.Headphone, Symbol.Headphone0, Symbol.Headphone1, Symbol.Headphone2, Symbol.Headphone3,
                Symbol.Headset, Symbol.Equalizer,
                Symbol.SpatialVolume0, Symbol.SpatialVolume1, Symbol.SpatialVolume2, Symbol.SpatialVolume3,
            ]),
            ("Microphone", [
                Symbol.Microphone, Symbol.MicOff, Symbol.MicSleep, Symbol.MicError,
                Symbol.MicOn, Symbol.MicClipping, Symbol.MicrophoneListening, Symbol.Speech,
            ]),
            ("View / Layout", [
                Symbol.ZoomIn, Symbol.ZoomOut, Symbol.Zoom,
                Symbol.View, Symbol.ViewAll, Symbol.PreviewLink, Symbol.Preview,
                Symbol.List, Symbol.Grid, Symbol.GridView, Symbol.TwoPage, Symbol.FitPage,
                Symbol.DockLeft, Symbol.DockRight, Symbol.DockBottom,
                Symbol.Orientation, Symbol.LandscapeOrientation,
                Symbol.Rotate, Symbol.Crop, Symbol.AspectRatio,
                Symbol.Slideshow, Symbol.MultiSelect, Symbol.ReadingMode,
                Symbol.MapLayers, Symbol.SplitView,
                Symbol.TaskView, Symbol.TaskViewExpanded,
                Symbol.BrowsePhotos, Symbol.ExpandTile,
                Symbol.ScrollMode, Symbol.ZoomMode, Symbol.PanMode, Symbol.ScrollUpDown,
            ]),
            ("Settings & System", [
                Symbol.Settings, Symbol.Help, Symbol.Info, Symbol.Info2,
                Symbol.Warning, Symbol.Error, Symbol.ErrorBadge,
                Symbol.Shield, Symbol.DefenderApp, Symbol.Repair, Symbol.Diagnostic,
                Symbol.Admin, Symbol.Lock, Symbol.Unlock, Symbol.LockFeedback,
                Symbol.AllApps, Symbol.PowerButton,
                Symbol.Personalize, Symbol.UpdateRestore,
                Symbol.TimeLanguage, Symbol.EaseOfAccess,
                Symbol.QuietHours, Symbol.DrivingMode, Symbol.RotationLock,
                Symbol.Process, Symbol.Processing,
                Symbol.Accounts, Symbol.SwitchUser, Symbol.OtherUser, Symbol.GuestUser,
            ]),
            ("Connectivity", [
                Symbol.Wifi, Symbol.Wifi1, Symbol.Wifi2, Symbol.Wifi3,
                Symbol.WifiHotspot, Symbol.WifiError0, Symbol.WifiWarning0,
                Symbol.Bluetooth, Symbol.Connect, Symbol.InternetSharing, Symbol.VPN,
                Symbol.Airplane, Symbol.AirplaneSolid,
                Symbol.USB, Symbol.WiredUSB, Symbol.WirelessUSB,
                Symbol.Ethernet, Symbol.EthernetError, Symbol.EthernetWarning,
                Symbol.NetworkTower, Symbol.Network,
                Symbol.SignalBars1, Symbol.SignalBars2, Symbol.SignalBars3, Symbol.SignalBars4, Symbol.SignalBars5,
                Symbol.SignalNotConnected, Symbol.SignalRoaming,
                Symbol.CastTo, Symbol.MapDrive, Symbol.DisconnectDrive, Symbol.DirectAccess,
                Symbol.DeviceDiscovery,
            ]),
            ("Devices", [
                Symbol.Tablet, Symbol.Laptop, Symbol.CellPhone, Symbol.MobileTablet,
                Symbol.TVMonitor, Symbol.PC1, Symbol.Remote, Symbol.AddRemoteDevice,
                Symbol.Devices, Symbol.Devices2,
                Symbol.Projector, Symbol.Mouse,
                Symbol.Keyboard, Symbol.KeyboardSplit, Symbol.KeyboardDismiss, Symbol.KeyboardFull,
                Symbol.SDCard, Symbol.HardDrive, Symbol.NetworkAdapter, Symbol.Touchscreen,
                Symbol.NetworkPrinter, Symbol.CloudPrinter, Symbol.Touchpad,
                Symbol.Webcam, Symbol.Webcam2, Symbol.Input, Symbol.Smartcard,
                Symbol.Game, Symbol.GameConsole, Symbol.XboxOneConsole,
                Symbol.Sensor, Symbol.HMD, Symbol.Printer3D,
                Symbol.SurfaceHub, Symbol.ThisPC,
            ]),
            ("Objects", [
                Symbol.Mail, Symbol.MailFill, Symbol.MailForward, Symbol.MailReply, Symbol.MailReplyAll,
                Symbol.Important,
                Symbol.Calendar, Symbol.CalendarDay, Symbol.CalendarWeek, Symbol.CalendarSolid,
                Symbol.Clock, Symbol.Stopwatch, Symbol.AlarmClock, Symbol.DateTime,
                Symbol.Tag, Symbol.Flag, Symbol.Camera, Symbol.Photo, Symbol.Photo2, Symbol.Picture,
                Symbol.Library, Symbol.Bookmarks, Symbol.Calculator,
                Symbol.Shop, Symbol.ShoppingCart, Symbol.Globe, Symbol.World,
                Symbol.Map, Symbol.MapPin, Symbol.MapPin2, Symbol.MapDirections, Symbol.Directions,
                Symbol.Car, Symbol.Walk, Symbol.Bus, Symbol.Train, Symbol.Ferry,
                Symbol.ParkingLocation, Symbol.IncidentTriangle, Symbol.Work, Symbol.Construction,
                Symbol.Bank, Symbol.Courthouse, Symbol.Groceries, Symbol.Cafe, Symbol.Street,
                Symbol.History, Symbol.Location, Symbol.PaymentCard,
                Symbol.Puzzle, Symbol.Certificate, Symbol.Lightbulb,
                Symbol.Brightness, Symbol.Flashlight, Symbol.Color,
            ]),
            ("People / Social", [
                Symbol.Account, Symbol.Contact2, Symbol.ContactSolid,
                Symbol.People, Symbol.AddFriend, Symbol.ContactInfo, Symbol.Group,
                Symbol.Pin, Symbol.Pinned, Symbol.PinFill, Symbol.Unpin,
                Symbol.FavoriteStar, Symbol.FavoriteStarFill,
                Symbol.HalfStarLeft, Symbol.HalfStarRight, Symbol.Unfavorite, Symbol.FavoriteList,
                Symbol.Like, Symbol.Dislike, Symbol.LikeDislike,
                Symbol.Comment, Symbol.Message, Symbol.ChatBubbles, Symbol.ThoughtBubble, Symbol.LeaveChat,
                Symbol.Phone, Symbol.PhoneBook, Symbol.IncomingCall, Symbol.HangUp,
                Symbol.PostUpdate, Symbol.Emoji, Symbol.Emoji2,
                Symbol.Heart, Symbol.HeartFill, Symbol.HeartBroken,
                Symbol.Reply, Symbol.Megaphone, Symbol.Family,
            ]),
            ("Status Indicators", [
                Symbol.StatusCircle, Symbol.StatusTriangle,
                Symbol.StatusError, Symbol.StatusWarning,
                Symbol.StatusCircleCheckmark, Symbol.StatusCircleExclamation,
                Symbol.StatusCircleErrorX, Symbol.StatusCircleInfo,
                Symbol.StatusCircleBlock, Symbol.StatusCircleQuestionMark,
                Symbol.StatusCircleSync, Symbol.StatusCircleRing,
                Symbol.StatusConnecting1, Symbol.StatusConnecting2, Symbol.StatusUnsecure,
                Symbol.ActionCenterAsterisk, Symbol.Asterisk,
                Symbol.CircleRing, Symbol.CircleFill, Symbol.FullCircleMask,
                Symbol.RadioBullet, Symbol.RadioBullet2, Symbol.RadioBtnOff, Symbol.RadioBtnOn,
                Symbol.Checkbox, Symbol.CheckboxComposite, Symbol.CheckboxFill,
                Symbol.CheckboxIndeterminate, Symbol.CheckMark,
                Symbol.ToggleFilled, Symbol.ToggleBorder,
            ]),
            ("Inking / Drawing", [
                Symbol.InkingTool, Symbol.InkingToolFill, Symbol.InkingToolFill2,
                Symbol.HighlightFill2, Symbol.EraseTool, Symbol.EraseToolFill,
                Symbol.Pencil, Symbol.PencilFill, Symbol.Marker,
                Symbol.InkingCaret, Symbol.InkingColorOutline, Symbol.InkingColorFill,
                Symbol.CalligraphyPen, Symbol.CalligraphyFill,
                Symbol.Draw, Symbol.DrawSolid, Symbol.Handwriting,
                Symbol.FingerInking, Symbol.StrokeErase, Symbol.PointErase, Symbol.ClearAllInk,
                Symbol.Ruler, Symbol.Protractor, Symbol.BrushSize, Symbol.Eyedropper,
                Symbol.PenWorkspace, Symbol.PenPalette,
                Symbol.Touch, Symbol.TouchPointer,
                Symbol.GripperTool, Symbol.GripperBarHorizontal, Symbol.GripperBarVertical,
            ]),
            ("IDE / Development", [
                Symbol.Code, Symbol.Bug, Symbol.CommandPrompt, Symbol.DeveloperTools,
                Symbol.FileExplorer, Symbol.FileExplorerApp,
                Symbol.Package, Symbol.Component, Symbol.DataSense,
                Symbol.Cloud, Symbol.CloudDownload, Symbol.CloudSearch,
                Symbol.Apps, Symbol.FeedbackApp, Symbol.Feedback,
                Symbol.QRCode, Symbol.ConnectApp,
            ]),
            ("Windows Chrome", [
                Symbol.ChromeClose, Symbol.ChromeMinimize, Symbol.ChromeMaximize, Symbol.ChromeRestore,
                Symbol.ChromeCloseContrast, Symbol.ChromeMinimizeContrast,
                Symbol.ChromeMaximizeContrast, Symbol.ChromeRestoreContrast,
                Symbol.ChromeBackContrast,
            ]),
            ("Education / Calculator", [
                Symbol.Education, Symbol.Dictionary, Symbol.DictionaryAdd, Symbol.ReadingList, Symbol.ToolTip,
                Symbol.CalculatorMultiply, Symbol.CalculatorAddition, Symbol.CalculatorSubtract,
                Symbol.CalculatorDivide, Symbol.CalculatorSquareroot,
                Symbol.CalculatorPercentage, Symbol.CalculatorNegate,
                Symbol.CalculatorEqualTo, Symbol.CalculatorBackspace,
            ]),
            ("Battery (0-10)", [
                Symbol.Battery0, Symbol.Battery1, Symbol.Battery2, Symbol.Battery3, Symbol.Battery4,
                Symbol.Battery5, Symbol.Battery6, Symbol.Battery7, Symbol.Battery8, Symbol.Battery9,
                Symbol.Battery10, Symbol.BatteryUnknown,
                Symbol.BatteryCharging0, Symbol.BatteryCharging1, Symbol.BatteryCharging2,
                Symbol.BatteryCharging3, Symbol.BatteryCharging4, Symbol.BatteryCharging5,
                Symbol.BatteryCharging6, Symbol.BatteryCharging7, Symbol.BatteryCharging8,
                Symbol.BatteryCharging9, Symbol.BatteryCharging10,
                Symbol.BatterySaver0, Symbol.BatterySaver1, Symbol.BatterySaver2,
                Symbol.BatterySaver3, Symbol.BatterySaver4, Symbol.BatterySaver5,
                Symbol.BatterySaver6, Symbol.BatterySaver7, Symbol.BatterySaver8,
            ]),
            ("Charts / Data", [
                Symbol.AreaChart, Symbol.CheckList, Symbol.ClipboardList,
                Symbol.PieSingle, Symbol.StockUp, Symbol.StockDown,
            ]),
            ("Star Variants", [
                Symbol.FavoriteStar, Symbol.FavoriteStarFill,
                Symbol.HalfStarLeft, Symbol.HalfStarRight,
                Symbol.QuarterStarLeft, Symbol.QuarterStarRight,
                Symbol.ThreeQuarterStarLeft, Symbol.ThreeQuarterStarRight,
            ]),
            ("Gamepad Buttons", [
                Symbol.ButtonA, Symbol.ButtonB, Symbol.ButtonX, Symbol.ButtonY,
                Symbol.LeftStick, Symbol.RightStick,
                Symbol.TriggerLeft, Symbol.TriggerRight,
                Symbol.BumperLeft, Symbol.BumperRight, Symbol.Dpad,
            ]),
            ("Status Dials", [
                Symbol.Dial1, Symbol.Dial2, Symbol.Dial3,
                Symbol.Dial4, Symbol.Dial5, Symbol.Dial6,
            ]),
            ("Emoji Tabs", [
                Symbol.EmojiTabPeople, Symbol.EmojiTabSmilesAnimals,
                Symbol.EmojiTabCelebrationObjects, Symbol.EmojiTabFoodPlants,
                Symbol.EmojiTabTransitPlaces, Symbol.EmojiTabSymbols,
            ]),
            ("Point-of-Sale", [
                Symbol.CashDrawer, Symbol.BarcodeScanner,
                Symbol.ReceiptPrinter, Symbol.MagStripeReader,
                Symbol.LineDisplay, Symbol.PINPad,
                Symbol.SignatureCapture, Symbol.ChipCardCreditCardReader,
            ]),
            ("Miscellaneous", [
                Symbol.LightningBolt, Symbol.Leaf, Symbol.InPrivate, Symbol.QuickNote,
                Symbol.ActionCenter, Symbol.ActionCenterNotification,
                Symbol.Ringer, Symbol.RingerSilent, Symbol.Vibrate,
                Symbol.Market, Symbol.Dialpad, Symbol.Tiles,
                Symbol.Robot, Symbol.TrafficLight, Symbol.Badge,
                Symbol.Blocked, Symbol.HomeSolid,
                Symbol.Reminder, Symbol.ReminderFill,
                Symbol.Design, Symbol.Website, Symbol.Drop, Symbol.Radar,
                Symbol.ConstructionCone, Symbol.Health,
                Symbol.BodyCam, Symbol.PoliceCar,
                Symbol.CityNext, Symbol.Sustainable, Symbol.BuildingEnergy,
                Symbol.PasswordKeyShow, Symbol.PasswordKeyHide,
                Symbol.GiftboxOpen, Symbol.NUIFace, Symbol.NUIIris,
                Symbol.Beta, Symbol.Project, Symbol.Flow, Symbol.Wheel,
                Symbol.CC, Symbol.Caption, Symbol.Subtitles,
                Symbol.ScreenCapture, Symbol.GenericScan, Symbol.ImageExport,
                Symbol.Uninstall, Symbol.KeyboardShortcut,
                Symbol.Connected, Symbol.HoloLens,
            ]),
        };

        var cardStack = (StackPanel)card.Child!;

        foreach (var (name, symbols) in categories)
        {
            cardStack.Children.Add(new TextBlock
            {
                Text = $"{name} ({symbols.Length})",
                FontSize = 14,
                Foreground = Accent,
                Margin = new Thickness(0, 12, 0, 4)
            });

            var wrap = new WrapPanel();
            foreach (var symbol in symbols)
                wrap.Children.Add(CreateSymbolTile(symbol));

            var container = new Border
            {
                Background = DarkBg,
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 4),
                Child = wrap
            };
            cardStack.Children.Add(container);
        }

        RootPanel.Children.Add(card);
    }

    // ==================== Helpers ====================

    private Border CreateCard(string title, string description)
    {
        var stack = new StackPanel { Orientation = Orientation.Vertical };
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 16,
            Foreground = White,
            Margin = new Thickness(0, 0, 0, 4)
        });
        stack.Children.Add(new TextBlock
        {
            Text = description,
            FontSize = 12,
            Foreground = DimGray,
            Margin = new Thickness(0, 0, 0, 16)
        });

        return new Border
        {
            Background = CardBg,
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 16),
            Child = stack
        };
    }
}
