using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Navigation state and commands for the Frame demo.</summary>
public sealed class FrameViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private static readonly SolidColorBrush PageOneBrush =
        new(Color.FromArgb(255, 0, 120, 212));
    private static readonly SolidColorBrush PageTwoBrush =
        new(Color.FromArgb(255, 0, 204, 106));
    private static readonly SolidColorBrush PageThreeBrush =
        new(Color.FromArgb(255, 247, 99, 12));

    private const string XamlCode = """
        <Frame>
            <Border Background="{Binding CurrentPageBackground}">
                <TextBlock Text="{Binding CurrentPageTitle}"/>
            </Border>
        </Frame>

        <Button Content="Back" Command="{Binding BackCommand}"/>
        <Button Content="Forward" Command="{Binding ForwardCommand}"/>
        """;

    private const string CSharpCode = """
        // FrameViewModel.cs
        public sealed class FrameViewModel : ViewModelBase
        {
            public ICommand BackCommand { get; }
            public ICommand ForwardCommand { get; }
            public ICommand NavigatePageOneCommand { get; }

            // Navigation state changes in commands and is rendered by bindings.
        }
        """;

    private int _currentPage = 1;

    public FrameViewModel()
    {
        NavigatePageOneCommand = new RelayCommand(() => CurrentPage = 1);
        NavigatePageTwoCommand = new RelayCommand(() => CurrentPage = 2);
        NavigatePageThreeCommand = new RelayCommand(() => CurrentPage = 3);
        BackCommand = new RelayCommand(
            () => CurrentPage--,
            () => CurrentPage > 1);
        ForwardCommand = new RelayCommand(
            () => CurrentPage++,
            () => CurrentPage < 3);
        RefreshCommand = new RelayCommand(Refresh);
    }

    public int CurrentPage
    {
        get => _currentPage;
        private set
        {
            var normalized = Math.Clamp(value, 1, 3);
            if (!SetProperty(ref _currentPage, normalized))
            {
                return;
            }

            OnPropertyChanged(nameof(CurrentPageTitle));
            OnPropertyChanged(nameof(CurrentPageBackground));
            OnPropertyChanged(nameof(NavigationHistoryText));
            ((RelayCommand)BackCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ForwardCommand).NotifyCanExecuteChanged();
        }
    }

    public string CurrentPageTitle => $"Page {CurrentPage}";

    public SolidColorBrush CurrentPageBackground => CurrentPage switch
    {
        1 => PageOneBrush,
        2 => PageTwoBrush,
        _ => PageThreeBrush
    };

    public string NavigationHistoryText => $"Current page: {CurrentPage}";

    public ICommand NavigatePageOneCommand { get; }

    public ICommand NavigatePageTwoCommand { get; }

    public ICommand NavigatePageThreeCommand { get; }

    public ICommand BackCommand { get; }

    public ICommand ForwardCommand { get; }

    public ICommand RefreshCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private void Refresh()
    {
        OnPropertyChanged(nameof(CurrentPageTitle));
        OnPropertyChanged(nameof(CurrentPageBackground));
        OnPropertyChanged(nameof(NavigationHistoryText));
    }
}
