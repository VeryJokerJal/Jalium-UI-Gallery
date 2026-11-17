using System.Collections.ObjectModel;
using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Items and commands presented by the swipe-action demo.</summary>
public sealed class SwipeControlViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <ItemsControl ItemsSource="{Binding Items}">
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <SwipeControl Visibility="{Binding Visibility}">
                        <SwipeControl.LeftItems>
                            <SwipeItems Mode="Reveal">
                                <SwipeItem Text="Archive"
                                           Background="#0078D4"
                                           Command="{Binding ArchiveCommand}" />
                            </SwipeItems>
                        </SwipeControl.LeftItems>
                        <SwipeControl.RightItems>
                            <SwipeItems Mode="Reveal">
                                <SwipeItem Text="Flag"
                                           Background="#C18A00"
                                           Command="{Binding FlagCommand}" />
                                <SwipeItem Text="Delete"
                                           Background="#C42B1C"
                                           Command="{Binding DeleteCommand}" />
                            </SwipeItems>
                        </SwipeControl.RightItems>
                        <Border Background="{Binding Background}" Padding="16,12">
                            <TextBlock Text="{Binding Text}"
                                       Foreground="{Binding Foreground}" />
                        </Border>
                    </SwipeControl>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
        """;

    private const string CSharpCode = """
        SwipeControl CreateSwipeRow(SwipeItemViewModel item)
        {
            var leftItems = new SwipeItems { Mode = SwipeMode.Reveal };
            leftItems.Add(new SwipeItem
            {
                Text = "Archive",
                Command = item.ArchiveCommand
            });
            return new SwipeControl
            {
                LeftItems = leftItems,
                Content = new TextBlock { Text = item.Text }
            };
        }

        public sealed class SwipeItemViewModel : ViewModelBase
        {
            private Visibility _visibility = Visibility.Visible;
            private Brush _background = Brushes.Transparent;

            public string Text { get; } = "Swipe this row";

            public SwipeItemViewModel()
            {
                ArchiveCommand = new RelayCommand(() => { });
                DeleteCommand = new RelayCommand(Delete);
                FlagCommand = new RelayCommand(() => { });
            }

            public ICommand ArchiveCommand { get; }
            public ICommand DeleteCommand { get; }
            public ICommand FlagCommand { get; }

            public Visibility Visibility
            {
                get => _visibility;
                private set => SetProperty(ref _visibility, value);
            }

            private void Delete()
            {
                Visibility = Visibility.Collapsed;
            }
        }
        """;

    private string _statusText = "No swipe action yet.";

    public SwipeControlViewModel()
    {
        Items =
        [
            CreateItem("Meeting with Design Team"),
            CreateItem("Review pull request #42"),
            CreateItem("Update project documentation"),
            CreateItem("Deploy staging build"),
            CreateItem("Weekly standup notes")
        ];
    }

    public ObservableCollection<SwipeItemViewModel> Items { get; }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private SwipeItemViewModel CreateItem(string text) =>
        new(text, message => StatusText = message);
}

public sealed class SwipeItemViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private static readonly SolidColorBrush DefaultBackground =
        new(Color.FromRgb(0x25, 0x25, 0x25));
    private static readonly SolidColorBrush ArchivedBackground =
        new(Color.FromRgb(0x00, 0x3A, 0x6A));
    private static readonly SolidColorBrush DefaultForeground =
        new(Color.White);
    private static readonly SolidColorBrush FlaggedForeground =
        new(Color.FromRgb(0xFF, 0xC1, 0x07));

    private readonly Action<string> _reportStatus;
    private Visibility _visibility = Visibility.Visible;
    private SolidColorBrush _background = DefaultBackground;
    private SolidColorBrush _foreground = DefaultForeground;

    public SwipeItemViewModel(string text, Action<string> reportStatus)
    {
        Text = text;
        _reportStatus = reportStatus;
        ArchiveCommand = new RelayCommand(Archive);
        DeleteCommand = new RelayCommand(Delete);
        FlagCommand = new RelayCommand(Flag);
    }

    public string Text { get; }

    public Visibility Visibility
    {
        get => _visibility;
        private set => SetProperty(ref _visibility, value);
    }

    public SolidColorBrush Background
    {
        get => _background;
        private set => SetProperty(ref _background, value);
    }

    public SolidColorBrush Foreground
    {
        get => _foreground;
        private set => SetProperty(ref _foreground, value);
    }

    public ICommand ArchiveCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand FlagCommand { get; }

    private void Archive()
    {
        Background = ArchivedBackground;
        _reportStatus($"Archived: \"{Text}\"");
    }

    private void Delete()
    {
        Visibility = Visibility.Collapsed;
        _reportStatus($"Deleted: \"{Text}\"");
    }

    private void Flag()
    {
        Foreground = FlaggedForeground;
        _reportStatus($"Flagged: \"{Text}\"");
    }
}
