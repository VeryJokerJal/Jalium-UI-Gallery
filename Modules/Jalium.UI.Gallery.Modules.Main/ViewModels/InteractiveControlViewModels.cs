using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Ink;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class InkCanvasViewModel : IInkCanvasStateSink
{
    private InkCanvasEditingMode _editingMode =
        InkCanvasEditingMode.Ink;
    private Color _strokeColor = Color.FromRgb(0, 0, 0);
    private double _strokeWidth = 2;
    private bool _isHighlighter;
    private BrushType _brushType = Jalium.UI.Ink.BrushType.Round;
    private StrokeTaperMode _taperMode = StrokeTaperMode.None;
    private int _strokeCount;
    private InkCanvasRequest? _canvasRequest;
    private long _requestSequence;

    public InkCanvasViewModel()
    {
        DrawCommand = new RelayCommand(
            () => EditingMode = InkCanvasEditingMode.Ink);
        EraseCommand = new RelayCommand(
            () => EditingMode =
                InkCanvasEditingMode.EraseByStroke);
        ClearCommand = new RelayCommand(
            () => CanvasRequest = new InkCanvasRequest(
                InkCanvasAction.Clear,
                ++_requestSequence));

        UseBlackCommand = ColorCommand(Color.FromRgb(0, 0, 0));
        UseRedCommand = ColorCommand(Color.FromRgb(232, 17, 35));
        UseBlueCommand = ColorCommand(Color.FromRgb(0, 120, 212));
        UseGreenCommand = ColorCommand(Color.FromRgb(0, 204, 106));
        UseOrangeCommand = ColorCommand(Color.FromRgb(247, 99, 12));
        UsePurpleCommand = ColorCommand(Color.FromRgb(136, 108, 228));

        UseRoundBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Round);
        UseCalligraphyBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Calligraphy);
        UsePenBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Pen);
        UseAirbrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Airbrush);
        UseOilBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Oil);
        UseCrayonBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Crayon);
        UseMarkerBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Marker);
        UsePencilBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Pencil);
        UseWatercolorBrushCommand =
            BrushCommand(Jalium.UI.Ink.BrushType.Watercolor);

        UseNormalTaperCommand =
            TaperCommand(StrokeTaperMode.None);
        UseTaperedStartCommand =
            TaperCommand(StrokeTaperMode.TaperedStart);
        UseTaperedEndCommand =
            TaperCommand(StrokeTaperMode.TaperedEnd);
    }

    public InkCanvasEditingMode EditingMode
    {
        get => _editingMode;
        set
        {
            if (SetProperty(ref _editingMode, value))
            {
                OnPropertyChanged(nameof(ModeStatus));
            }
        }
    }

    public Color StrokeColor
    {
        get => _strokeColor;
        set
        {
            if (SetProperty(ref _strokeColor, value))
            {
                OnPropertyChanged(nameof(DrawingSettings));
            }
        }
    }

    public double StrokeWidth
    {
        get => _strokeWidth;
        set
        {
            if (SetProperty(ref _strokeWidth, value))
            {
                OnPropertyChanged(nameof(WidthStatus));
                OnPropertyChanged(nameof(DrawingSettings));
            }
        }
    }

    public bool IsHighlighter
    {
        get => _isHighlighter;
        set
        {
            if (SetProperty(ref _isHighlighter, value))
            {
                OnPropertyChanged(nameof(DrawingSettings));
            }
        }
    }

    public BrushType BrushType
    {
        get => _brushType;
        set
        {
            if (SetProperty(ref _brushType, value))
            {
                OnPropertyChanged(nameof(BrushTypeStatus));
                OnPropertyChanged(nameof(DrawingSettings));
            }
        }
    }

    public StrokeTaperMode TaperMode
    {
        get => _taperMode;
        set
        {
            if (SetProperty(ref _taperMode, value))
            {
                OnPropertyChanged(nameof(TaperStatus));
                OnPropertyChanged(nameof(DrawingSettings));
            }
        }
    }

    public InkCanvasDrawingSettings DrawingSettings =>
        new(
            StrokeColor,
            StrokeWidth,
            IsHighlighter,
            BrushType,
            TaperMode);

    public int StrokeCount
    {
        get => _strokeCount;
        private set
        {
            if (SetProperty(ref _strokeCount, value))
            {
                OnPropertyChanged(nameof(StrokeCountStatus));
            }
        }
    }

    public InkCanvasRequest? CanvasRequest
    {
        get => _canvasRequest;
        private set => SetProperty(ref _canvasRequest, value);
    }

    public string ModeStatus => $"Mode: {EditingMode switch
    {
        InkCanvasEditingMode.Ink => "Ink",
        InkCanvasEditingMode.EraseByStroke => "Erase",
        InkCanvasEditingMode.EraseByPoint => "Erase (Point)",
        InkCanvasEditingMode.Select => "Select",
        _ => "None"
    }}";

    public string WidthStatus =>
        $"Width: {Math.Round(StrokeWidth)}";

    public string BrushTypeStatus =>
        $"Brush Type: {BrushType}";

    public string TaperStatus => $"Taper: {TaperMode switch
    {
        StrokeTaperMode.TaperedStart => "Tapered Start",
        StrokeTaperMode.TaperedEnd => "Tapered End",
        _ => "Normal"
    }}";

    public string StrokeCountStatus =>
        $"Strokes: {StrokeCount}";

    public ICommand DrawCommand { get; }

    public ICommand EraseCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand UseBlackCommand { get; }

    public ICommand UseRedCommand { get; }

    public ICommand UseBlueCommand { get; }

    public ICommand UseGreenCommand { get; }

    public ICommand UseOrangeCommand { get; }

    public ICommand UsePurpleCommand { get; }

    public ICommand UseRoundBrushCommand { get; }

    public ICommand UseCalligraphyBrushCommand { get; }

    public ICommand UsePenBrushCommand { get; }

    public ICommand UseAirbrushCommand { get; }

    public ICommand UseOilBrushCommand { get; }

    public ICommand UseCrayonBrushCommand { get; }

    public ICommand UseMarkerBrushCommand { get; }

    public ICommand UsePencilBrushCommand { get; }

    public ICommand UseWatercolorBrushCommand { get; }

    public ICommand UseNormalTaperCommand { get; }

    public ICommand UseTaperedStartCommand { get; }

    public ICommand UseTaperedEndCommand { get; }

    public void UpdateStrokeCount(int count) =>
        StrokeCount = count;

    private RelayCommand ColorCommand(Color color) =>
        new(() => StrokeColor = color);

    private RelayCommand BrushCommand(BrushType brushType) =>
        new(() => BrushType = brushType);

    private RelayCommand TaperCommand(
        StrokeTaperMode taperMode) =>
        new(() => TaperMode = taperMode);
}

public sealed partial class EditControlViewModel
    : IEditControlStateSink
{
    private const string XamlCode = """
        <EditControl Height="400"
                     Text="{Binding EditorText, Mode=TwoWay}"
                     Language="{Binding EditorLanguage}"
                     ShowLineNumbers="{Binding ShowLineNumbers}"
                     HighlightCurrentLine="{Binding HighlightCurrentLine}"
                     IsReadOnly="{Binding IsReadOnly}"
                     TabSize="4"
                     ConvertTabsToSpaces="True"
                     behaviors:EditControlBehavior.IsEnabled="True"
                     behaviors:EditControlBehavior.Request="{Binding EditorRequest}" />

        <CheckBox Content="Show Line Numbers"
                  IsChecked="{Binding ShowLineNumbers, Mode=TwoWay}" />
        <Button Content="Undo" Command="{Binding UndoCommand}" />
        <Button Content="Redo" Command="{Binding RedoCommand}" />
        <Button Content="Select All" Command="{Binding SelectAllCommand}" />
        <Button Content="Clear" Command="{Binding ClearCommand}" />
        """;

    private const string CSharpCode = """
        public sealed class EditControlViewModel :
            ViewModelBase,
            IEditControlStateSink
        {
            private string _editorText = "// Start editing";
            private string _editorLanguage = "csharp";
            private EditorRequest? _editorRequest;
            private long _requestSequence;

            public EditControlViewModel()
            {
                UndoCommand = RequestCommand(EditorAction.Undo);
                RedoCommand = RequestCommand(EditorAction.Redo);
                SelectAllCommand = RequestCommand(EditorAction.SelectAll);
                ClearCommand = new RelayCommand(
                    () => EditorText = string.Empty);
            }

            public string EditorText
            {
                get => _editorText;
                set => SetProperty(ref _editorText, value);
            }

            public string EditorLanguage
            {
                get => _editorLanguage;
                set => SetProperty(ref _editorLanguage, value);
            }

            public EditorRequest? EditorRequest
            {
                get => _editorRequest;
                private set => SetProperty(ref _editorRequest, value);
            }

            public ICommand UndoCommand { get; }
            public ICommand RedoCommand { get; }
            public ICommand SelectAllCommand { get; }
            public ICommand ClearCommand { get; }

            public void UpdateEditorPosition(
                int lineCount,
                int textLength,
                int line,
                int column)
            {
                DocumentStatus =
                    $"Lines: {lineCount}  |  Length: {textLength}";
                CaretStatus = $"Ln {line}, Col {column}";
            }

            private RelayCommand RequestCommand(EditorAction action) =>
                new(() => EditorRequest = new EditorRequest(
                    action,
                    ++_requestSequence));
        }
        """;

    private const string CSharpSample = """
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Jalium.UI.Gallery;

        public sealed class Calculator
        {
            private readonly Dictionary<string, Func<double, double, double>>
                _operations = new()
                {
                    ["+"] = (left, right) => left + right,
                    ["-"] = (left, right) => left - right,
                    ["*"] = (left, right) => left * right,
                    ["/"] = (left, right) =>
                        right != 0 ? left / right : double.NaN
                };

            public double Evaluate(
                double left,
                string operation,
                double right) =>
                _operations.TryGetValue(operation, out var evaluate)
                    ? evaluate(left, right)
                    : throw new ArgumentException(
                        $"Unknown operator: {operation}");
        }
        """;

    private const string JalxamlSample = """
        <Window xmlns="http://schemas.jalium.ui/2024"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                x:Class="Sample.MainWindow"
                Title="Jalium UI">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="280" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>
                <NavigationView Grid.Column="0" />
                <TextBlock Grid.Column="1"
                           Text="{Binding Greeting}"
                           HorizontalAlignment="Center"
                           VerticalAlignment="Center" />
            </Grid>
        </Window>
        """;

    private string _editorText = CSharpSample;
    private string _editorLanguage = "csharp";
    private bool _showLineNumbers = true;
    private bool _highlightCurrentLine = true;
    private bool _isReadOnly;
    private string _documentStatus = "Ready";
    private string _caretStatus = "Ln 1, Col 0";
    private EditorRequest? _editorRequest;
    private long _requestSequence;

    public EditControlViewModel()
    {
        UndoCommand = RequestCommand(EditorAction.Undo);
        RedoCommand = RequestCommand(EditorAction.Redo);
        SelectAllCommand = RequestCommand(EditorAction.SelectAll);
        ClearCommand = new RelayCommand(
            () => EditorText = string.Empty);
        LoadCSharpSampleCommand = new RelayCommand(() =>
        {
            EditorLanguage = "csharp";
            EditorText = CSharpSample;
        });
        LoadJalxamlSampleCommand = new RelayCommand(() =>
        {
            EditorLanguage = "jalxaml";
            EditorText = JalxamlSample;
        });
    }

    public string EditorText
    {
        get => _editorText;
        set => SetProperty(ref _editorText, value);
    }

    public string EditorLanguage
    {
        get => _editorLanguage;
        set => SetProperty(ref _editorLanguage, value);
    }

    public bool ShowLineNumbers
    {
        get => _showLineNumbers;
        set => SetProperty(ref _showLineNumbers, value);
    }

    public bool HighlightCurrentLine
    {
        get => _highlightCurrentLine;
        set => SetProperty(ref _highlightCurrentLine, value);
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }

    public string DocumentStatus
    {
        get => _documentStatus;
        private set => SetProperty(ref _documentStatus, value);
    }

    public string CaretStatus
    {
        get => _caretStatus;
        private set => SetProperty(ref _caretStatus, value);
    }

    public EditorRequest? EditorRequest
    {
        get => _editorRequest;
        private set => SetProperty(ref _editorRequest, value);
    }

    public ICommand UndoCommand { get; }

    public ICommand RedoCommand { get; }

    public ICommand SelectAllCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand LoadCSharpSampleCommand { get; }

    public ICommand LoadJalxamlSampleCommand { get; }

    public override string XamlExample => XamlCode;

    public override string CSharpExample => CSharpCode;

    public void UpdateEditorPosition(
        int lineCount,
        int textLength,
        int line,
        int column)
    {
        DocumentStatus =
            $"Lines: {lineCount}  |  Length: {textLength}";
        CaretStatus = $"Ln {line}, Col {column}";
    }

    private RelayCommand RequestCommand(EditorAction action) =>
        new(() => EditorRequest = new EditorRequest(
            action,
            ++_requestSequence));
}

public sealed partial class TreeDataGridViewModel
{
    private GalleryTreeNode? _selectedItem;
    private TreeDataGridRequest? _treeRequest;
    private long _requestSequence;

    public TreeDataGridViewModel()
    {
        Items = CreateItems();
        ExpandAllCommand = RequestCommand(
            TreeDataGridAction.ExpandAll);
        CollapseAllCommand = RequestCommand(
            TreeDataGridAction.CollapseAll);
    }

    public IReadOnlyList<GalleryTreeNode> Items { get; }

    public GalleryTreeNode? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnPropertyChanged(nameof(SelectedItemText));
            }
        }
    }

    public string SelectedItemText => SelectedItem is null
        ? "(none)"
        : $"{SelectedItem.Name} ({SelectedItem.Type})";

    public TreeDataGridRequest? TreeRequest
    {
        get => _treeRequest;
        private set => SetProperty(ref _treeRequest, value);
    }

    public ICommand ExpandAllCommand { get; }

    public ICommand CollapseAllCommand { get; }

    private RelayCommand RequestCommand(
        TreeDataGridAction action) =>
        new(() => TreeRequest = new TreeDataGridRequest(
            action,
            ++_requestSequence));

    private static IReadOnlyList<GalleryTreeNode> CreateItems() =>
        new GalleryTreeNode[]
        {
            new(
                "src",
                "Folder",
                0,
                new GalleryTreeNode[]
                {
                    new(
                        "Controls",
                        "Folder",
                        0,
                        new GalleryTreeNode[]
                        {
                            new("Button.cs", "C# Source", 4_200),
                            new("TextBox.cs", "C# Source", 8_750),
                            new("TreeDataGrid.cs", "C# Source", 12_400),
                            new("DataGrid.cs", "C# Source", 15_600)
                        }),
                    new(
                        "Core",
                        "Folder",
                        0,
                        new GalleryTreeNode[]
                        {
                            new("FrameworkElement.cs", "C# Source", 22_100),
                            new("DependencyObject.cs", "C# Source", 9_800),
                            new("DependencyProperty.cs", "C# Source", 6_300)
                        }),
                    new(
                        "Media",
                        "Folder",
                        0,
                        new GalleryTreeNode[]
                        {
                            new("Brush.cs", "C# Source", 3_500),
                            new("Color.cs", "C# Source", 2_100)
                        })
                }),
            new(
                "tests",
                "Folder",
                0,
                new GalleryTreeNode[]
                {
                    new("ButtonTests.cs", "C# Source", 3_400),
                    new("TreeDataGridTests.cs", "C# Source", 5_600),
                    new("DataGridTests.cs", "C# Source", 4_800)
                }),
            new("README.md", "Markdown", 2_048),
            new("Directory.Build.props", "MSBuild", 1_024),
            new(".gitignore", "Git Config", 512)
        };
}

public sealed partial class TextEffectPresenterViewModel
{
    private const string InitialMutationText = "Type a lyric...";
    private const string EffectSwitchText = "Animate me";
    private const string InitialComposeText = "Hello Jalium";

    private string _mutationText = InitialMutationText;
    private string _effectText = EffectSwitchText;
    private string _composeText = InitialComposeText;
    private TextAnimationEffect _animationEffect =
        TextAnimationEffect.RiseSettle;
    private TextGpuEffect _gpuEffect = TextGpuEffect.None;

    public TextEffectPresenterViewModel()
    {
        AppendCommand = new RelayCommand(
            () => MutationText += "lyric ");
        AppendLineCommand = new RelayCommand(
            () => MutationText += Environment.NewLine + "new line");
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
            {
                MutationText = MutationText.Remove(index, count);
            }
        });
        ClearCommand = new RelayCommand(
            () => MutationText = string.Empty);
        ResetCommand = new RelayCommand(
            () => MutationText = InitialMutationText);

        UseRiseSettleCommand =
            AnimationCommand(TextAnimationEffect.RiseSettle);
        UseFadeInCommand =
            AnimationCommand(TextAnimationEffect.FadeIn);
        UseTypewriterCommand =
            AnimationCommand(TextAnimationEffect.Typewriter);
        UseWaveBounceCommand =
            AnimationCommand(TextAnimationEffect.WaveBounce);
        UsePulsingBlurCommand =
            AnimationCommand(TextAnimationEffect.PulsingBlur);

        UseNoGpuEffectCommand =
            GpuCommand(TextGpuEffect.None);
        UseBlur4Command =
            GpuCommand(TextGpuEffect.Blur4);
        UseBlur10Command =
            GpuCommand(TextGpuEffect.Blur10);
    }

    public string MutationText
    {
        get => _mutationText;
        set => SetProperty(ref _mutationText, value);
    }

    public string EffectText
    {
        get => _effectText;
        private set => SetProperty(ref _effectText, value);
    }

    public string ComposeText
    {
        get => _composeText;
        private set => SetProperty(ref _composeText, value);
    }

    public TextAnimationEffect AnimationEffect
    {
        get => _animationEffect;
        private set => SetProperty(ref _animationEffect, value);
    }

    public TextGpuEffect GpuEffect
    {
        get => _gpuEffect;
        private set => SetProperty(ref _gpuEffect, value);
    }

    public ICommand AppendCommand { get; }

    public ICommand AppendLineCommand { get; }

    public ICommand InsertCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand UseRiseSettleCommand { get; }

    public ICommand UseFadeInCommand { get; }

    public ICommand UseTypewriterCommand { get; }

    public ICommand UseWaveBounceCommand { get; }

    public ICommand UsePulsingBlurCommand { get; }

    public ICommand UseNoGpuEffectCommand { get; }

    public ICommand UseBlur4Command { get; }

    public ICommand UseBlur10Command { get; }

    private RelayCommand AnimationCommand(
        TextAnimationEffect effect) =>
        new(() =>
        {
            AnimationEffect = effect;
            ReplayEffectText();
        });

    private RelayCommand GpuCommand(TextGpuEffect effect) =>
        new(() =>
        {
            GpuEffect = effect;
            ReplayComposeText();
        });

    private void ReplayEffectText()
    {
        EffectText = string.Empty;
        EffectText = EffectSwitchText;
    }

    private void ReplayComposeText()
    {
        ComposeText = string.Empty;
        ComposeText = InitialComposeText;
    }
}

public sealed partial class ElementEffectsViewModel
{
    private double _glowSize = 10;
    private double _glowIntensity = 1;
    private double _innerShadowBlur = 8;
    private double _innerShadowDepth = 5;
    private double _embossAmount = 1.5;
    private double _embossAngle = 45;
    private double _saturation = 1;
    private ColorMatrixPreset _colorMatrixPreset =
        ColorMatrixPreset.Saturation;

    public ElementEffectsViewModel()
    {
        UseGrayscaleCommand =
            PresetCommand(ColorMatrixPreset.Grayscale);
        UseSepiaCommand =
            PresetCommand(ColorMatrixPreset.Sepia);
        UseInvertCommand =
            PresetCommand(ColorMatrixPreset.Invert);
        UseHueRotateCommand =
            PresetCommand(ColorMatrixPreset.HueRotate);
        ResetColorMatrixCommand = new RelayCommand(() =>
        {
            Saturation = 1;
            ColorMatrixPreset = ColorMatrixPreset.None;
        });
    }

    public double GlowSize
    {
        get => _glowSize;
        set => SetProperty(ref _glowSize, value);
    }

    public double GlowIntensity
    {
        get => _glowIntensity;
        set => SetProperty(ref _glowIntensity, value);
    }

    public double InnerShadowBlur
    {
        get => _innerShadowBlur;
        set => SetProperty(ref _innerShadowBlur, value);
    }

    public double InnerShadowDepth
    {
        get => _innerShadowDepth;
        set => SetProperty(ref _innerShadowDepth, value);
    }

    public double EmbossAmount
    {
        get => _embossAmount;
        set => SetProperty(ref _embossAmount, value);
    }

    public double EmbossAngle
    {
        get => _embossAngle;
        set => SetProperty(ref _embossAngle, value);
    }

    public double Saturation
    {
        get => _saturation;
        set
        {
            if (SetProperty(ref _saturation, value))
            {
                ColorMatrixPreset = ColorMatrixPreset.Saturation;
            }
        }
    }

    public ColorMatrixPreset ColorMatrixPreset
    {
        get => _colorMatrixPreset;
        private set => SetProperty(ref _colorMatrixPreset, value);
    }

    public ICommand UseGrayscaleCommand { get; }

    public ICommand UseSepiaCommand { get; }

    public ICommand UseInvertCommand { get; }

    public ICommand UseHueRotateCommand { get; }

    public ICommand ResetColorMatrixCommand { get; }

    private RelayCommand PresetCommand(ColorMatrixPreset preset) =>
        new(() => ColorMatrixPreset = preset);
}

public sealed partial class TabControlViewModel
{
    private int _selectedTabIndex;

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                OnPropertyChanged(nameof(SelectedTabStatus));
            }
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
