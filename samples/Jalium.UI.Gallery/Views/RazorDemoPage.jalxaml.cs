using System.ComponentModel;
using Jalium.UI;
using Jalium.UI.Controls;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Demonstrates all supported Razor syntax features in JALXAML markup.
/// The @if directive (#12) uses runtime data binding with IsFeatureEnabled,
/// while code-block directives (@for, @foreach, @switch, etc.) are expanded at preprocess time.
/// </summary>
public partial class RazorDemoPage : Page
{
    public RazorDemoPage()
    {
        // Set DataContext for @identifier and @(expression) binding demos
        var vm = new RazorDemoViewModel();
        DataContext = vm;

        // Register AOT-safe property accessors (no reflection needed at runtime)
        Markup.RazorExpressionRegistry.RegisterPropertyAccessors(typeof(RazorDemoViewModel), [
            ("UserName", o => ((RazorDemoViewModel)o).UserName),
            ("ItemCount", o => ((RazorDemoViewModel)o).ItemCount),
            ("IsFeatureEnabled", o => ((RazorDemoViewModel)o).IsFeatureEnabled),
        ]);

        InitializeComponent();

        if (ToggleButton != null)
            ToggleButton.Click += OnToggleClick;
    }

    private void OnToggleClick(object? sender, EventArgs e)
    {
        if (DataContext is RazorDemoViewModel vm)
        {
            vm.IsFeatureEnabled = !vm.IsFeatureEnabled;
        }
    }

    private sealed class RazorDemoViewModel : INotifyPropertyChanged
    {
        private string _userName = "Jalium Developer";
        private int _itemCount = 42;
        private bool _isFeatureEnabled = true;

        public string UserName
        {
            get => _userName;
            set { _userName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName))); }
        }

        public int ItemCount
        {
            get => _itemCount;
            set { _itemCount = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemCount))); }
        }

        public bool IsFeatureEnabled
        {
            get => _isFeatureEnabled;
            set { _isFeatureEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFeatureEnabled))); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
