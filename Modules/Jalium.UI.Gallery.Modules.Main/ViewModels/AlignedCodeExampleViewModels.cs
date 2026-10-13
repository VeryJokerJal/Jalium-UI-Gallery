namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

// C# shown by interactive pages mirrors the ViewModel/command path used by
// their live preview. Keeping these excerpts separate from the one-time
// migration file makes stale code-behind examples easy to retire in batches.
public sealed partial class AppBarButtonViewModel
{
    public override string CSharpExample => """
        public sealed class AppBarButtonViewModel : ViewModelBase
        {
            private string _basicStatus =
                "Click any button to see its action.";

            public AppBarButtonViewModel()
            {
                BasicCommand = new RelayCommand<string>(action =>
                    BasicStatus =
                        $"{action ?? "Unknown"} button clicked.");
            }

            public ICommand BasicCommand { get; }

            public string BasicStatus
            {
                get => _basicStatus;
                private set => SetProperty(ref _basicStatus, value);
            }
        }
        """;
}

public sealed partial class AppBarToggleButtonViewModel
{
    public override string CSharpExample => """
        public sealed class AppBarToggleButtonViewModel : ViewModelBase
        {
            private bool _isBold;
            private bool _isItalic;

            public bool IsBold
            {
                get => _isBold;
                set
                {
                    if (SetProperty(ref _isBold, value))
                        NotifyFormattingChanged();
                }
            }

            public bool IsItalic
            {
                get => _isItalic;
                set
                {
                    if (SetProperty(ref _isItalic, value))
                        NotifyFormattingChanged();
                }
            }

            public FontWeight PreviewFontWeight =>
                IsBold ? FontWeights.Bold : FontWeights.Normal;

            public string FormattingPreview =>
                $"Bold={IsBold}, Italic={IsItalic}";

            private void NotifyFormattingChanged()
            {
                OnPropertyChanged(nameof(PreviewFontWeight));
                OnPropertyChanged(nameof(FormattingPreview));
            }
        }
        """;
}

public sealed partial class AutoCompleteBoxViewModel
{
    public override string CSharpExample => """
        public sealed class AutoCompleteBoxViewModel : ViewModelBase
        {
            private string? _selectedLanguage;

            public IReadOnlyList<string> Languages { get; } =
                ["C#", "C++", "JavaScript", "TypeScript", "Python", "Rust"];

            public string? SelectedLanguage
            {
                get => _selectedLanguage;
                set
                {
                    if (SetProperty(ref _selectedLanguage, value))
                        OnPropertyChanged(nameof(SelectedLanguageText));
                }
            }

            public string SelectedLanguageText =>
                SelectedLanguage ?? "(none)";
        }
        """;
}

public sealed partial class BackdropEffectsViewModel
{
    public override string CSharpExample => """
        public sealed class BackdropEffectsViewModel : ViewModelBase
        {
            private readonly IGalleryWindowBackdropService _backdrops;
            private WindowBackdropMode _currentBackdrop;

            public BackdropEffectsViewModel(
                IGalleryWindowBackdropService backdrops)
            {
                _backdrops = backdrops;
                SetBackdropCommand =
                    new RelayCommand<WindowBackdropMode>(SetBackdrop);
            }

            public ICommand SetBackdropCommand { get; }

            public WindowBackdropMode CurrentBackdrop
            {
                get => _currentBackdrop;
                private set => SetProperty(ref _currentBackdrop, value);
            }

            private void SetBackdrop(WindowBackdropMode mode)
            {
                if (_backdrops.TrySetBackdrop(mode))
                    CurrentBackdrop = mode;
            }
        }
        """;
}

public sealed partial class ColorPickerViewModel
{
    public override string CSharpExample => """
        public sealed class ColorPickerViewModel : ViewModelBase
        {
            private Color _selectedColor =
                Color.FromRgb(0xFF, 0xD6, 0x0A);

            public Color SelectedColor
            {
                get => _selectedColor;
                set
                {
                    if (!SetProperty(ref _selectedColor, value))
                        return;
                    OnPropertyChanged(nameof(SelectedColorBrush));
                    OnPropertyChanged(nameof(SelectedColorText));
                }
            }

            public SolidColorBrush SelectedColorBrush =>
                new(SelectedColor);

            public string SelectedColorText =>
                $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        }
        """;
}

public sealed partial class CommandBarViewModel
{
    public override string CSharpExample => """
        public sealed class CommandBarViewModel : ViewModelBase
        {
            private string _basicStatus =
                "Click a command to see its action.";

            public CommandBarViewModel()
            {
                ExecuteBasicCommand =
                    new RelayCommand<string>(action =>
                        BasicStatus =
                            $"{action ?? "Unknown"} command executed.");
            }

            public ICommand ExecuteBasicCommand { get; }

            public string BasicStatus
            {
                get => _basicStatus;
                private set => SetProperty(ref _basicStatus, value);
            }
        }
        """;
}

public sealed partial class ContentDialogViewModel
{
    public override string CSharpExample => """
        public sealed class ContentDialogViewModel : ViewModelBase
        {
            private readonly IGalleryContentDialogService _dialogs;
            private string _popupResult =
                "Open a dialog to see the result here.";

            public ContentDialogViewModel(
                IGalleryContentDialogService dialogs)
            {
                _dialogs = dialogs;
                ShowBasicDialogCommand =
                    new AsyncRelayCommand(ShowBasicDialogAsync);
            }

            public ICommand ShowBasicDialogCommand { get; }

            public string PopupResult
            {
                get => _popupResult;
                private set => SetProperty(ref _popupResult, value);
            }

            private async Task ShowBasicDialogAsync() =>
                PopupResult = await _dialogs.ShowRenameAsync();
        }
        """;
}

public sealed partial class DataGridViewModel
{
    public override string CSharpExample => """
        public sealed class DataGridViewModel : ViewModelBase
        {
            private PersonData? _selectedPerson;

            public IReadOnlyList<PersonData> People { get; } =
            [
                new("Alice", "Smith", 28, "Engineering"),
                new("Bob", "Johnson", 35, "Marketing")
            ];

            public PersonData? SelectedPerson
            {
                get => _selectedPerson;
                set
                {
                    if (SetProperty(ref _selectedPerson, value))
                        OnPropertyChanged(nameof(SelectedPersonText));
                }
            }

            public string SelectedPersonText =>
                SelectedPerson is { } person
                    ? $"{person.FirstName} {person.LastName}"
                    : "(none)";
        }

        public sealed record PersonData(
            string FirstName,
            string LastName,
            int Age,
            string Department);
        """;
}

public sealed partial class DialogsViewModel
{
    public override string CSharpExample => """
        public sealed class DialogsViewModel : ViewModelBase
        {
            private readonly IGallerySystemDialogService _dialogs;
            private string _selectedFile = "No file selected";

            public DialogsViewModel(IGallerySystemDialogService dialogs)
            {
                _dialogs = dialogs;
                ShowDialogCommand =
                    new RelayCommand<string>(Execute);
            }

            public ICommand ShowDialogCommand { get; }

            public string SelectedFile
            {
                get => _selectedFile;
                private set => SetProperty(ref _selectedFile, value);
            }

            private void Execute(string? action)
            {
                if (action == "OpenFile" &&
                    _dialogs.OpenFile() is { } file)
                {
                    SelectedFile = $"Selected: {file}";
                }
            }
        }
        """;
}

public sealed partial class DiffViewerViewModel
{
    public override string CSharpExample => """
        public sealed class DiffViewerViewModel : ViewModelBase
        {
            private string _originalText =
                "Hello World\nThis is a test\nLine three";
            private string _modifiedText =
                "Hello World\nThis is modified\nLine four added";

            public string InteractiveOriginalText
            {
                get => _originalText;
                set => SetProperty(ref _originalText, value);
            }

            public string InteractiveModifiedText
            {
                get => _modifiedText;
                set => SetProperty(ref _modifiedText, value);
            }
        }
        """;
}

public sealed partial class DragDropViewModel
{
    public override string CSharpExample => """
        public sealed class DragDropViewModel : ViewModelBase
        {
            private string _dropTargetHint = "Drop items here";

            public ObservableCollection<string> DroppedItems { get; } = [];

            public string DropTargetHint
            {
                get => _dropTargetHint;
                private set => SetProperty(ref _dropTargetHint, value);
            }

            public void OnDragEntered(
                DragDropTargetRole role,
                string targetName)
            {
                if (role == DragDropTargetRole.ItemList)
                    DropTargetHint = "Release to drop";
            }

            public void OnTextDropped(
                DragDropTargetRole role,
                string targetName,
                string text)
            {
                if (role == DragDropTargetRole.ItemList)
                {
                    DroppedItems.Add(text);
                    DropTargetHint = "Drop items here";
                }
            }
        }
        """;
}

public sealed partial class ExpanderViewModel
{
    public override string CSharpExample => """
        public sealed class ExpanderViewModel : ViewModelBase
        {
            private bool _isControlledExpanderOpen;

            public ExpanderViewModel()
            {
                ExpandCommand = new RelayCommand(
                    () => IsControlledExpanderOpen = true);
                CollapseCommand = new RelayCommand(
                    () => IsControlledExpanderOpen = false);
                ToggleCommand = new RelayCommand(() =>
                    IsControlledExpanderOpen =
                        !IsControlledExpanderOpen);
            }

            public ICommand ExpandCommand { get; }
            public ICommand CollapseCommand { get; }
            public ICommand ToggleCommand { get; }

            public bool IsControlledExpanderOpen
            {
                get => _isControlledExpanderOpen;
                set => SetProperty(
                    ref _isControlledExpanderOpen,
                    value);
            }
        }
        """;
}

public sealed partial class HexEditorViewModel
{
    public override string CSharpExample => """
        public sealed class HexEditorViewModel : ViewModelBase
        {
            private bool _showAsciiColumn = true;
            private double _bytesPerRow = 16;

            public HexEditorViewModel()
            {
                SampleData = Encoding.ASCII.GetBytes(
                    "Hello from Jalium.UI HexEditor!");
            }

            public byte[] SampleData { get; }

            public bool ShowAsciiColumn
            {
                get => _showAsciiColumn;
                set => SetProperty(ref _showAsciiColumn, value);
            }

            public double BytesPerRow
            {
                get => _bytesPerRow;
                set => SetProperty(
                    ref _bytesPerRow,
                    Math.Round(value));
            }
        }
        """;
}

public sealed partial class IconElementViewModel
{
    public override string CSharpExample => """
        public sealed class IconElementViewModel : ViewModelBase
        {
            public IconElementViewModel()
            {
                FeaturedSymbols =
                [
                    new SymbolSample("Save", Symbol.Save),
                    new SymbolSample("Delete", Symbol.Delete),
                    new SymbolSample("Share", Symbol.Share)
                ];
            }

            public IReadOnlyList<SymbolSample> FeaturedSymbols { get; }
        }

        public sealed record SymbolSample(string Name, Symbol Symbol);
        """;
}

public sealed partial class ImageViewModel
{
    public override string CSharpExample => """
        public sealed class ImageViewModel : ViewModelBase
        {
            public ImageViewModel(
                IGalleryImageSourceService imageSources)
            {
                var sources = imageSources.CreateDemoSources();
                RasterSource = sources.Raster;
                SvgSource = sources.Vector;
                RasterStatus = sources.RasterStatus;
                SvgStatus = sources.VectorStatus;
            }

            public ImageSource RasterSource { get; }
            public ImageSource SvgSource { get; }
            public string RasterStatus { get; }
            public string SvgStatus { get; }
        }
        """;
}

public sealed partial class JsonTreeViewerViewModel
{
    public override string CSharpExample => """
        public sealed class JsonTreeViewerViewModel : ViewModelBase
        {
            private string _searchText = string.Empty;
            private string _interactiveJson =
                "{\n  \"greeting\": \"Hello World\",\n  \"count\": 42\n}";

            public string SearchText
            {
                get => _searchText;
                set => SetProperty(ref _searchText, value);
            }

            public string InteractiveJson
            {
                get => _interactiveJson;
                set => SetProperty(ref _interactiveJson, value);
            }
        }
        """;
}

public sealed partial class LiquidGlassViewModel
{
    public override string CSharpExample => """
        public sealed class LiquidGlassViewModel : ViewModelBase
        {
            private double _blurRadius = 8;
            private double _refractionAmount = 60;

            public double BlurRadius
            {
                get => _blurRadius;
                set
                {
                    if (SetProperty(ref _blurRadius, value))
                        OnPropertyChanged(nameof(SettingsSummary));
                }
            }

            public double RefractionAmount
            {
                get => _refractionAmount;
                set
                {
                    if (SetProperty(ref _refractionAmount, value))
                        OnPropertyChanged(nameof(SettingsSummary));
                }
            }

            public string SettingsSummary =>
                $"Blur={BlurRadius:F0}  " +
                $"Refraction={RefractionAmount:F0}";
        }
        """;
}

public sealed partial class ListBoxViewModel
{
    public override string CSharpExample => """
        public sealed class ListBoxViewModel : ViewModelBase
        {
            private string? _selectedItem;

            public IReadOnlyList<string> Items { get; } =
                ["Apple", "Banana", "Cherry", "Date"];

            public string? SelectedItem
            {
                get => _selectedItem;
                set
                {
                    if (SetProperty(ref _selectedItem, value))
                        OnPropertyChanged(nameof(SelectedItemText));
                }
            }

            public string SelectedItemText =>
                SelectedItem ?? "(none)";
        }
        """;
}

public sealed partial class MediaElementViewModel
{
    public override string CSharpExample => """
        public sealed class MediaElementViewModel : ViewModelBase
        {
            private long _requestSequence;
            private double _volumePercent = 75;
            private MediaPlayerRequest? _videoRequest;

            public MediaElementViewModel()
            {
                PlayVideoCommand = Video(MediaPlayerAction.Play);
                PauseVideoCommand = Video(MediaPlayerAction.Pause);
                StopVideoCommand = Video(MediaPlayerAction.Stop);
            }

            public ICommand PlayVideoCommand { get; }
            public ICommand PauseVideoCommand { get; }
            public ICommand StopVideoCommand { get; }

            public double VolumePercent
            {
                get => _volumePercent;
                set
                {
                    if (SetProperty(ref _volumePercent, value))
                        OnPropertyChanged(nameof(Volume));
                }
            }

            public double Volume => VolumePercent / 100;

            public MediaPlayerRequest? VideoRequest
            {
                get => _videoRequest;
                private set => SetProperty(ref _videoRequest, value);
            }

            private ICommand Video(MediaPlayerAction action) =>
                new RelayCommand(() => VideoRequest =
                    new MediaPlayerRequest(++_requestSequence, action));
        }
        """;
}

public sealed partial class MenuBarViewModel
{
    public override string CSharpExample => """
        public sealed class MenuBarViewModel : ViewModelBase
        {
            private string _menuStatus = "No menu action yet.";
            private bool _isWordWrapEnabled;

            public MenuBarViewModel()
            {
                ExecuteMenuCommand =
                    new RelayCommand<string>(path =>
                        MenuStatus = $"{path ?? "Unknown"} clicked");
            }

            public ICommand ExecuteMenuCommand { get; }

            public string MenuStatus
            {
                get => _menuStatus;
                private set => SetProperty(ref _menuStatus, value);
            }

            public bool IsWordWrapEnabled
            {
                get => _isWordWrapEnabled;
                set
                {
                    if (SetProperty(ref _isWordWrapEnabled, value))
                        MenuStatus = $"View > Word Wrap: {value}";
                }
            }
        }
        """;
}

public sealed partial class MenuFlyoutViewModel
{
    public override string CSharpExample => """
        public sealed class MenuFlyoutViewModel : ViewModelBase
        {
            private string _basicStatus =
                "Click the button to open the menu.";
            private bool _isToolbarVisible = true;

            public MenuFlyoutViewModel()
            {
                ExecuteBasicCommand =
                    new RelayCommand<string>(action =>
                        BasicStatus =
                            $"{action ?? "Unknown"} selected");
            }

            public ICommand ExecuteBasicCommand { get; }

            public string BasicStatus
            {
                get => _basicStatus;
                private set => SetProperty(ref _basicStatus, value);
            }

            public bool IsToolbarVisible
            {
                get => _isToolbarVisible;
                set => SetProperty(ref _isToolbarVisible, value);
            }
        }
        """;
}

public sealed partial class PopupViewModel
{
    public override string CSharpExample => """
        public sealed class PopupViewModel : ViewModelBase
        {
            private bool _isBasicPopupOpen;

            public PopupViewModel()
            {
                ToggleBasicPopupCommand = new RelayCommand(() =>
                    IsBasicPopupOpen = !IsBasicPopupOpen);
                CloseBasicPopupCommand = new RelayCommand(() =>
                    IsBasicPopupOpen = false);
            }

            public ICommand ToggleBasicPopupCommand { get; }
            public ICommand CloseBasicPopupCommand { get; }

            public bool IsBasicPopupOpen
            {
                get => _isBasicPopupOpen;
                set => SetProperty(ref _isBasicPopupOpen, value);
            }
        }
        """;
}

public sealed partial class NavigationDemoViewModel
{
    public override string CSharpExample => """
        public sealed class NavigationDemoViewModel : ViewModelBase
        {
            private readonly IGalleryNavigationService _navigation;
            private string _navigationStatus =
                "Navigation Status: Ready";

            public NavigationDemoViewModel(
                IGalleryNavigationService navigation)
            {
                _navigation = navigation;
                GoBackCommand = new RelayCommand(GoBack);
                GoForwardCommand = new RelayCommand(GoForward);
                RefreshCommand = new RelayCommand(Refresh);
                OpenNavigationWindowCommand =
                    new RelayCommand(OpenNavigationWindow);
            }

            public ICommand GoBackCommand { get; }
            public ICommand GoForwardCommand { get; }
            public ICommand RefreshCommand { get; }
            public ICommand OpenNavigationWindowCommand { get; }

            public string NavigationStatus
            {
                get => _navigationStatus;
                private set => SetProperty(ref _navigationStatus, value);
            }

            private void GoBack() => NavigationStatus =
                _navigation.TryGoBack()
                    ? "Navigation Status: Navigated back"
                    : "Navigation Status: Cannot go back";

            private void GoForward() => NavigationStatus =
                _navigation.TryGoForward()
                    ? "Navigation Status: Navigated forward"
                    : "Navigation Status: Cannot go forward";

            private void Refresh() => _navigation.Refresh();

            private void OpenNavigationWindow() =>
                _navigation.ShowNavigationWindowDemo();
        }
        """;
}

public sealed partial class PasswordBoxViewModel
{
    public override string CSharpExample => """
        var passwordBox = new PasswordBox
        {
            Width = 300,
            Height = 32,
            PasswordChar = '•',
            MaxLength = 64
        };

        // Read Password only when submitting; do not mirror it into ordinary
        // ViewModel string state or write it to logs.
        var password = passwordBox.Password;
        var canSubmit = !string.IsNullOrWhiteSpace(password);
        """;
}

public sealed partial class RadioButtonViewModel
{
    public override string CSharpExample => """
        public sealed class RadioButtonViewModel : ViewModelBase
        {
            private string _selectedColor = "Blue";

            public bool IsRedSelected
            {
                get => SelectedColor == "Red";
                set
                {
                    if (value)
                        SelectedColor = "Red";
                }
            }

            public string SelectedColor
            {
                get => _selectedColor;
                private set
                {
                    if (!SetProperty(ref _selectedColor, value))
                        return;
                    OnPropertyChanged(nameof(IsRedSelected));
                    OnPropertyChanged(nameof(SelectionText));
                }
            }

            public string SelectionText =>
                $"Selected: {SelectedColor}";
        }
        """;
}

public sealed partial class RangeSliderViewModel
{
    public override string CSharpExample => """
        public sealed class RangeSliderViewModel : ViewModelBase
        {
            private double _rangeStart = 20;
            private double _rangeEnd = 80;

            public double RangeStart
            {
                get => _rangeStart;
                set
                {
                    if (SetProperty(ref _rangeStart, value))
                        OnPropertyChanged(nameof(RangeStartText));
                }
            }

            public double RangeEnd
            {
                get => _rangeEnd;
                set
                {
                    if (SetProperty(ref _rangeEnd, value))
                        OnPropertyChanged(nameof(RangeEndText));
                }
            }

            public string RangeStartText =>
                ((int)RangeStart).ToString();
            public string RangeEndText =>
                ((int)RangeEnd).ToString();
        }
        """;
}

public sealed partial class RibbonViewModel
{
    public override string CSharpExample => """
        public sealed class RibbonViewModel : ViewModelBase
        {
            private string _ribbonStatus =
                "Click ribbon buttons to see interactions...";
            private bool _isRibbonMinimized;
            private int _selectedTabIndex;

            public RibbonViewModel()
            {
                ExecuteCommand = new RelayCommand<string>(action =>
                    RibbonStatus = $"{action ?? "Unknown"} clicked");
            }

            public ICommand ExecuteCommand { get; }

            public string RibbonStatus
            {
                get => _ribbonStatus;
                private set => SetProperty(ref _ribbonStatus, value);
            }

            public bool IsRibbonMinimized
            {
                get => _isRibbonMinimized;
                set => SetProperty(ref _isRibbonMinimized, value);
            }

            public int SelectedTabIndex
            {
                get => _selectedTabIndex;
                set => SetProperty(ref _selectedTabIndex, value);
            }
        }
        """;
}

public sealed partial class ShaderEffectsViewModel
{
    public override string XamlExample => """
        <StackPanel xmlns="http://schemas.jalium.com/jalxaml"
                    xmlns:behaviors="clr-namespace:Jalium.UI.Gallery.Modules.Main.Behaviors;assembly=Jalium.UI.Gallery.Modules.Main">
            <!-- ShaderEffectBehavior updates BlurEffect and DropShadowEffect from bound values. -->
            <Slider Width="200" Minimum="0" Maximum="50"
                    Value="{Binding BlurRadius, Mode=TwoWay}" />
            <CheckBox Content="Box 模糊核" IsChecked="{Binding UseBoxBlur, Mode=TwoWay}" />
            <Border Background="#191A2D" Width="200" Height="100"
                    behaviors:ShaderEffectBehavior.BlurRadius="{Binding BlurRadius}"
                    behaviors:ShaderEffectBehavior.UseBoxBlur="{Binding UseBoxBlur}">
                <TextBlock Text="模糊预览" Foreground="White" />
            </Border>
            <Slider Width="200" Minimum="0" Maximum="30"
                    Value="{Binding ShadowDepth, Mode=TwoWay}" />
            <Border Background="#08948A" Width="150" Height="80"
                    behaviors:ShaderEffectBehavior.ShadowDepth="{Binding ShadowDepth}">
                <TextBlock Text="投影预览" />
            </Border>
            <Slider Width="200" Minimum="0" Maximum="1"
                    Value="{Binding InvertAmount, Mode=TwoWay}" />
            <Border Background="#191A2D" Width="200" Height="100"
                    behaviors:ShaderEffectBehavior.InvertAmount="{Binding InvertAmount}">
                <TextBlock Text="HLSL 实时预览" Foreground="White" />
            </Border>
        </StackPanel>
        """;

    public override string CSharpExample => """"
        // The view adapter applies these properties to BlurEffect and DropShadowEffect.
        public sealed class ShaderEffectsViewModel : ViewModelBase
        {
            private double _blurRadius = 10;
            private double _shadowDepth = 5;
            private double _invertAmount = 0.75;
            private bool _useBoxBlur;

            public double BlurRadius
            {
                get => _blurRadius;
                set => SetProperty(ref _blurRadius, value);
            }
            public double ShadowDepth
            {
                get => _shadowDepth;
                set => SetProperty(ref _shadowDepth, value);
            }
            public double InvertAmount
            {
                get => _invertAmount;
                set => SetProperty(ref _invertAmount, value);
            }
            public bool UseBoxBlur
            {
                get => _useBoxBlur;
                set => SetProperty(ref _useBoxBlur, value);
            }
        }

        public sealed class GalleryInvertEffect : ShaderEffect
        {
            public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
                nameof(Amount), typeof(double), typeof(GalleryInvertEffect),
                new PropertyMetadata(1.0, PixelShaderConstantCallback(0)));

            public double Amount
            {
                get => (double)GetValue(AmountProperty)!;
                set => SetValue(AmountProperty, value);
            }

            public GalleryInvertEffect()
            {
                PixelShader = new PixelShader
                {
                    SourceHlsl = """
                        Texture2D image : register(t0);
                        SamplerState imageSampler : register(s0);
                        cbuffer Constants : register(b0) { float4 amount; }
                        float4 main(float2 uv : TEXCOORD0) : SV_Target
                        {
                            float4 color = image.Sample(imageSampler, uv);
                            return float4(lerp(color.rgb, color.a - color.rgb, amount.x), color.a);
                        }
                        """
                };
                UpdateShaderValue(AmountProperty);
            }
        }
        """";
}

public sealed partial class TabControlViewModel
{
    public override string CSharpExample => """
        public sealed class TabControlViewModel : ViewModelBase
        {
            private int _selectedTabIndex;

            public int SelectedTabIndex
            {
                get => _selectedTabIndex;
                set
                {
                    if (SetProperty(ref _selectedTabIndex, value))
                        OnPropertyChanged(nameof(SelectedTabStatus));
                }
            }

            public string SelectedTabStatus =>
                $"Selected tab: {SelectedTabIndex switch
                {
                    1 => "Documents",
                    2 => "Downloads",
                    _ => "Home"
                }}";
        }
        """;
}

public sealed partial class ToolBarViewModel
{
    public override string CSharpExample => """
        public sealed class ToolBarViewModel : ViewModelBase
        {
            private string _lastAction =
                "Click toolbar buttons to see interactions...";

            public ToolBarViewModel()
            {
                ExecuteCommand = new RelayCommand<string>(action =>
                    LastAction = $"{action ?? "Unknown"} clicked");
            }

            public ICommand ExecuteCommand { get; }

            public string LastAction
            {
                get => _lastAction;
                private set => SetProperty(ref _lastAction, value);
            }
        }
        """;
}

public sealed partial class TreeSelectorViewModel
{
    public override string CSharpExample => """
        public sealed class TreeSelectorViewModel :
            ViewModelBase,
            ITreeSelectorStateSink
        {
            private string _singleSelectionStatus = "未选择";
            private string _multipleSelectionStatus =
                "已选择 0 项 · 已勾选 0 项";
            private string _cascadeSelectionStatus = "已勾选 0 项";

            public IReadOnlyList<SelectionTreeNode> Regions { get; } =
            [
                new SelectionTreeNode(
                    "华东地区",
                    true,
                    new SelectionTreeNode("上海"),
                    new SelectionTreeNode("江苏"))
            ];

            public string SingleSelectionStatus
            {
                get => _singleSelectionStatus;
                private set => SetProperty(
                    ref _singleSelectionStatus,
                    value);
            }

            public string MultipleSelectionStatus
            {
                get => _multipleSelectionStatus;
                private set => SetProperty(
                    ref _multipleSelectionStatus,
                    value);
            }

            public string CascadeSelectionStatus
            {
                get => _cascadeSelectionStatus;
                private set => SetProperty(
                    ref _cascadeSelectionStatus,
                    value);
            }

            public void UpdateTreeSelectorState(
                string key,
                string? label,
                int selectedCount,
                int checkedCount)
            {
                if (key == "single")
                {
                    SingleSelectionStatus = label is null
                        ? "未选择"
                        : $"已选择: {label}";
                }
                else if (key == "multiple")
                {
                    MultipleSelectionStatus =
                        $"已选择 {selectedCount} 项 · " +
                        $"已勾选 {checkedCount} 项";
                }
                else if (key == "cascade")
                {
                    CascadeSelectionStatus =
                        $"已勾选 {checkedCount} 项";
                }
            }
        }
        """;
}

public sealed partial class ViewboxViewModel
{
    public override string CSharpExample => """
        public sealed class ViewboxViewModel : ViewModelBase
        {
            private double _previewWidth = 200;
            private double _previewHeight = 150;

            public double PreviewWidth
            {
                get => _previewWidth;
                set
                {
                    if (SetProperty(ref _previewWidth, value))
                        OnPropertyChanged(nameof(PreviewWidthText));
                }
            }

            public double PreviewHeight
            {
                get => _previewHeight;
                set
                {
                    if (SetProperty(ref _previewHeight, value))
                        OnPropertyChanged(nameof(PreviewHeightText));
                }
            }

            public string PreviewWidthText =>
                ((int)PreviewWidth).ToString();
            public string PreviewHeightText =>
                ((int)PreviewHeight).ToString();
        }
        """;
}

public sealed partial class MarkdownViewModel
{
    public override string CSharpExample => """
        public sealed class MarkdownViewModel : ViewModelBase
        {
            private string _interactiveText = "# Documentation";
            private string _liveText = "# Live Markdown Editor";

            public MarkdownViewModel()
            {
                SelectSampleCommand =
                    new RelayCommand<string>(SelectSample);
            }

            public ICommand SelectSampleCommand { get; }

            public string InteractiveText
            {
                get => _interactiveText;
                private set => SetProperty(ref _interactiveText, value);
            }

            public string LiveText
            {
                get => _liveText;
                set => SetProperty(ref _liveText, value);
            }

            private void SelectSample(string? sample)
            {
                InteractiveText = sample switch
                {
                    "ReleaseNotes" => "# Release Notes",
                    "Checklist" => "# Launch Checklist",
                    _ => "# Documentation"
                };
            }
        }
        """;
}

public sealed partial class ProgressBarViewModel
{
    public override string CSharpExample => """
        public sealed class ProgressBarViewModel : ViewModelBase
        {
            private double _progress = 45;

            public double Progress
            {
                get => _progress;
                set
                {
                    if (SetProperty(ref _progress, value))
                        OnPropertyChanged(nameof(ProgressText));
                }
            }

            public string ProgressText => $"{(int)Progress}%";
        }
        """;
}

public sealed partial class RichTextBoxViewModel
{
    public override string CSharpExample => """
        using Jalium.UI.Controls;
        using Jalium.UI.Documents;

        var editor = new RichTextBox
        {
            Width = 500,
            Height = 200
        };

        var boldButton = new Button
        {
            Command = EditingCommands.ToggleBold,
            CommandTarget = editor,
            ToolTip = "Bold"
        };
        var italicButton = new Button
        {
            Command = EditingCommands.ToggleItalic,
            CommandTarget = editor,
            ToolTip = "Italic"
        };
        var underlineButton = new Button
        {
            Command = EditingCommands.ToggleUnderline,
            CommandTarget = editor,
            ToolTip = "Underline"
        };
        """;
}

public sealed partial class SliderViewModel
{
    public override string CSharpExample => """
        public sealed class SliderViewModel : ViewModelBase
        {
            private double _value = 50;

            public double Value
            {
                get => _value;
                set
                {
                    if (SetProperty(ref _value, value))
                        OnPropertyChanged(nameof(ValueText));
                }
            }

            public string ValueText => ((int)Value).ToString();
        }
        """;
}

public sealed partial class TextBoxViewModel
{
    public override string CSharpExample => """
        public sealed class TextBoxViewModel : ViewModelBase
        {
            private string _text = string.Empty;

            public string Text
            {
                get => _text;
                set
                {
                    if (SetProperty(ref _text, value))
                        OnPropertyChanged(nameof(CharacterCountText));
                }
            }

            public string CharacterCountText =>
                $"Characters: {Text.Length}";
        }
        """;
}

public sealed partial class TextEffectPresenterViewModel
{
    public override string CSharpExample => """
        public sealed class TextEffectPresenterViewModel : ViewModelBase
        {
            private string _mutationText = "Type a lyric...";

            public TextEffectPresenterViewModel()
            {
                AppendCommand = new RelayCommand(
                    () => MutationText += "lyric ");
                InsertCommand = new RelayCommand(() =>
                {
                    var index = Math.Min(3, MutationText.Length);
                    MutationText = MutationText.Insert(index, "[INS]");
                });
                RemoveCommand = new RelayCommand(() =>
                {
                    var index = Math.Min(3, MutationText.Length);
                    var count = Math.Min(2, MutationText.Length - index);
                    if (count > 0)
                        MutationText = MutationText.Remove(index, count);
                });
                ClearCommand = new RelayCommand(
                    () => MutationText = string.Empty);
            }

            public string MutationText
            {
                get => _mutationText;
                set => SetProperty(ref _mutationText, value);
            }

            public ICommand AppendCommand { get; }
            public ICommand InsertCommand { get; }
            public ICommand RemoveCommand { get; }
            public ICommand ClearCommand { get; }
        }
        """;
}
