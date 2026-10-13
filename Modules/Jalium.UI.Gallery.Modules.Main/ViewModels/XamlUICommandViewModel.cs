using Jalium.UI.Input;
using GalleryRelayCommand = Jalium.UI.Gallery.Core.Mvvm.RelayCommand;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// Demonstrates the real Jalium XamlUICommand API: UI metadata lives on the
/// XamlUICommand while execution and CanExecute are delegated to ViewModel
/// commands.
/// </summary>
public sealed class XamlUICommandViewModel : GalleryPageViewModelBase
{
    private const string XamlCode = """
        <!-- These bindings resolve to real Jalium.UI.Input.XamlUICommand instances. -->
        <Page xmlns="http://schemas.jalium.ui/2024"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:behaviors="clr-namespace:Jalium.UI.Gallery.Modules.Main.Behaviors"
              behaviors:XamlUICommandInputBehavior.Commands="{Binding Commands}">
          <StackPanel>
          <CommandBar Width="500" OverflowButtonVisibility="Collapsed">
            <CommandBar.PrimaryCommands>
                <AppBarButton Label="{Binding AddToFavoritesCommand.Label}"
                              ToolTip="{Binding AddToFavoritesCommand.Description}"
                              Command="{Binding AddToFavoritesCommand}">
                    <AppBarButton.Icon>
                        <FontIcon Glyph="{Binding AddToFavoritesCommand.IconSource}" />
                    </AppBarButton.Icon>
                </AppBarButton>
                <AppBarButton Label="{Binding ShareViaLinkCommand.Label}"
                              ToolTip="{Binding ShareViaLinkCommand.Description}"
                              Command="{Binding ShareViaLinkCommand}">
                    <AppBarButton.Icon>
                        <FontIcon Glyph="{Binding ShareViaLinkCommand.IconSource}" />
                    </AppBarButton.Icon>
                </AppBarButton>
                <AppBarButton Label="{Binding ExportAsPdfCommand.Label}"
                              ToolTip="{Binding ExportAsPdfCommand.Description}"
                              Command="{Binding ExportAsPdfCommand}">
                    <AppBarButton.Icon>
                        <FontIcon Glyph="{Binding ExportAsPdfCommand.IconSource}" />
                    </AppBarButton.Icon>
                </AppBarButton>
            </CommandBar.PrimaryCommands>
        </CommandBar>

        <CheckBox Content="Allow PDF export"
                  IsChecked="{Binding CanExportAsPdf}" />
          <TextBlock Text="{Binding CommandOutputText}" />
          </StackPanel>
        </Page>
        """;

    private const string CSharpCode = """
        using Jalium.UI.Input;
        using GalleryRelayCommand = Jalium.UI.Gallery.Core.Mvvm.RelayCommand;

        public sealed class CommandsViewModel : ViewModelBase
        {
            private readonly GalleryRelayCommand _exportAction;
            private bool _canExportAsPdf = true;

            public CommandsViewModel()
            {
                AddToFavoritesCommand = CreateCommand(
                    "Add to Favorites", "\uE734",
                    "Add the current item to your favorites list.",
                    new KeyGesture(Key.D, ModifierKeys.Control),
                    new GalleryRelayCommand(() => Record("Add to Favorites")));

                ShareViaLinkCommand = CreateCommand(
                    "Share via Link", "\uE72D",
                    "Generate a shareable link for the current item.",
                    new KeyGesture(
                        Key.L,
                        ModifierKeys.Control | ModifierKeys.Shift),
                    new GalleryRelayCommand(() => Record("Share via Link")));

                _exportAction = new GalleryRelayCommand(
                    () => Record("Export as PDF"),
                    () => CanExportAsPdf);
                ExportAsPdfCommand = CreateCommand(
                    "Export as PDF", "\uEA90",
                    "Export the current document as a PDF file.",
                    new KeyGesture(
                        Key.E,
                        ModifierKeys.Control | ModifierKeys.Shift),
                    _exportAction);
                Commands =
                [
                    AddToFavoritesCommand,
                    ShareViaLinkCommand,
                    ExportAsPdfCommand
                ];
            }

            public XamlUICommand AddToFavoritesCommand { get; }
            public XamlUICommand ShareViaLinkCommand { get; }
            public XamlUICommand ExportAsPdfCommand { get; }
            public IReadOnlyList<XamlUICommand> Commands { get; }

            public bool CanExportAsPdf
            {
                get => _canExportAsPdf;
                set
                {
                    if (SetProperty(ref _canExportAsPdf, value))
                    {
                        _exportAction.NotifyCanExecuteChanged();
                    }
                }
            }

            private static XamlUICommand CreateCommand(
                string label,
                string iconGlyph,
                string description,
                KeyGesture accelerator,
                GalleryRelayCommand action)
            {
                var command = new XamlUICommand
                {
                    Label = label,
                    IconSource = iconGlyph,
                    Description = description,
                    Command = action
                };
                command.KeyboardAccelerators.Add(accelerator);
                return command;
            }

            private void Record(string label)
            {
                // Update application state here.
            }
        }
        """;

    private readonly List<string> _commandHistory = [];
    private readonly GalleryRelayCommand _exportAction;
    private string _commandOutputText = "No command executed yet.";
    private bool _canExportAsPdf = true;

    public XamlUICommandViewModel()
    {
        AddToFavoritesCommand = CreateCommand(
            "Add to Favorites",
            "\uE734",
            "Add the current item to your favorites list.",
            "F",
            new KeyGesture(Key.D, ModifierKeys.Control),
            new GalleryRelayCommand(() => RecordCommand("Add to Favorites")));

        ShareViaLinkCommand = CreateCommand(
            "Share via Link",
            "\uE72D",
            "Generate a shareable link for the current item.",
            "S",
            new KeyGesture(
                Key.L,
                ModifierKeys.Control | ModifierKeys.Shift),
            new GalleryRelayCommand(() => RecordCommand("Share via Link")));

        _exportAction = new GalleryRelayCommand(
            () => RecordCommand("Export as PDF"),
            () => CanExportAsPdf);
        ExportAsPdfCommand = CreateCommand(
            "Export as PDF",
            "\uEA90",
            "Export the current document as a PDF file.",
            "E",
            new KeyGesture(
                Key.E,
                ModifierKeys.Control | ModifierKeys.Shift),
            _exportAction);
        Commands =
        [
            AddToFavoritesCommand,
            ShareViaLinkCommand,
            ExportAsPdfCommand
        ];
    }

    public XamlUICommand AddToFavoritesCommand { get; }

    public XamlUICommand ShareViaLinkCommand { get; }

    public XamlUICommand ExportAsPdfCommand { get; }

    public IReadOnlyList<XamlUICommand> Commands { get; }

    public bool CanExportAsPdf
    {
        get => _canExportAsPdf;
        set
        {
            if (SetProperty(ref _canExportAsPdf, value))
            {
                _exportAction.NotifyCanExecuteChanged();
            }
        }
    }

    public string CommandOutputText
    {
        get => _commandOutputText;
        private set => SetProperty(ref _commandOutputText, value);
    }

    public string CommandHistoryText =>
        _commandHistory.Count == 0
            ? string.Empty
            : "Recent: " + string.Join(" | ", _commandHistory);

    public override string XamlExample => XamlCode;

    public override string CSharpExample => CSharpCode;

    private static XamlUICommand CreateCommand(
        string label,
        string iconGlyph,
        string description,
        string accessKey,
        KeyGesture accelerator,
        GalleryRelayCommand action)
    {
        var command = new XamlUICommand
        {
            Label = label,
            IconSource = iconGlyph,
            Description = description,
            AccessKey = accessKey,
            Command = action
        };
        command.KeyboardAccelerators.Add(accelerator);
        return command;
    }

    private void RecordCommand(string label)
    {
        CommandOutputText = $"{label} executed.";
        _commandHistory.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {label}");
        if (_commandHistory.Count > 5)
        {
            _commandHistory.RemoveAt(_commandHistory.Count - 1);
        }

        OnPropertyChanged(nameof(CommandHistoryText));
    }
}
