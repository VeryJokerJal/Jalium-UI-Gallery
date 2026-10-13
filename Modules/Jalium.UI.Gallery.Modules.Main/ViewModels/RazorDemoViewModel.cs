using System.Windows.Input;
using Jalium.UI;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>Bindable data used by the JALXAML Razor syntax demonstrations.</summary>
public sealed class RazorDemoViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <Page xmlns="http://schemas.jalium.ui/2024"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <TextBlock Text="@UserName" />
            <TextBlock Text='@(ItemCount > 0 ? ItemCount + " items" : "Empty")' />
            <TextBlock Text="Hello, @UserName! You have @ItemCount notifications." />

            @{ for (var i = 1; i <= 5; i++) {
                <Border Width="@(30 + i * 10)">
                    <TextBlock Text="@(i.ToString())" />
                </Border>
            } }

            @if(IsFeatureEnabled){
                <Border Background="#107C10">
                    <TextBlock Text="Feature is ENABLED" />
                </Border>
            }

            <TextBlock Text="My font size: @$.FontSize" FontSize="14" />
            <TextBlock Text="@#.UserName" />
        </Page>
        """;

    private const string CSharpCode = """
        // RazorDemoViewModel.cs
        using System.Windows.Input;
        using Jalium.UI.Gallery.Core.Mvvm;

        namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

        public sealed class RazorDemoViewModel : ViewModelBase
        {
            private bool _isFeatureEnabled = true;

            public RazorDemoViewModel()
            {
                ToggleFeatureCommand = new RelayCommand(
                    () => IsFeatureEnabled = !IsFeatureEnabled);
            }

            public string UserName { get; set; } = "Jalium Developer";
            public int ItemCount { get; set; } = 42;

            public bool IsFeatureEnabled
            {
                get => _isFeatureEnabled;
                set => SetProperty(ref _isFeatureEnabled, value);
            }

            public ICommand ToggleFeatureCommand { get; }
        }
        """;

    private string _userName = "Jalium Developer";
    private int _itemCount = 42;
    private bool _isFeatureEnabled = true;

    public RazorDemoViewModel()
    {
        Markup.RazorExpressionRegistry.RegisterPropertyAccessors(typeof(RazorDemoViewModel), [
            ("UserName", o => ((RazorDemoViewModel)o).UserName),
            ("ItemCount", o => ((RazorDemoViewModel)o).ItemCount),
            ("IsFeatureEnabled", o => ((RazorDemoViewModel)o).IsFeatureEnabled),
        ]);

        ToggleFeatureCommand = new RelayCommand(
            () => IsFeatureEnabled = !IsFeatureEnabled);
    }

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public int ItemCount
    {
        get => _itemCount;
        set => SetProperty(ref _itemCount, value);
    }

    public bool IsFeatureEnabled
    {
        get => _isFeatureEnabled;
        set => SetProperty(ref _isFeatureEnabled, value);
    }

    public ICommand ToggleFeatureCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;
}
