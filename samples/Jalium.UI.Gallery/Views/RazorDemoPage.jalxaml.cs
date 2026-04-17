using System.ComponentModel;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Demonstrates all supported Razor syntax features in JALXAML markup.
/// The @if directive (#12) uses runtime data binding with IsFeatureEnabled,
/// while code-block directives (@for, @foreach, @switch, etc.) are expanded at preprocess time.
/// </summary>
public partial class RazorDemoPage : Page
{
    private const string XamlExample = @"<Page xmlns=""http://schemas.jalium.ui/2024""
      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- 1. @identifier - Path Binding -->
    <TextBlock Text=""@UserName"" />

    <!-- 2. @(expression) - C# Expression -->
    <TextBlock Text='@(ItemCount > 0 ? ItemCount + "" items"" : ""Empty"")' />

    <!-- 3. Mixed Text - Path + Literal -->
    <TextBlock Text=""Hello, @UserName! You have @ItemCount notifications."" />

    <!-- 4. @{ for } - For Loop -->
    @{ for (var i = 1; i <= 5; i++) {
        <Border Width=""@(30 + i * 10)"">
            <TextBlock Text=""@(i.ToString())"" />
        </Border>
    } }

    <!-- 5. @{ foreach } - Collection Iteration -->
    @{ var colors = new[] { ""#E74C3C"", ""#2ECC71"" };
       foreach (var color in colors) {
        <Border Background=""@color"">
            <TextBlock Text=""@color"" />
        </Border>
    } }

    <!-- 6. @if - Conditional Visibility (Runtime) -->
    @if(IsFeatureEnabled){
        <Border Background=""#107C10"">
            <TextBlock Text=""Feature is ENABLED"" />
        </Border>
    }

    <!-- 13. $.Property - Self-Reference -->
    <TextBlock Text=""My font size: @$.FontSize"" FontSize=""14"" />
    <TextBlock Text=""Tag = @$.Tag"" Tag=""self-tag-value"" />
    <TextBlock Text=""@($.FontSize > 12 ? &quot;Large&quot; : &quot;Small&quot;)"" FontSize=""16"" />

    <!-- 14. #.Property - Data Model Reference -->
    <TextBlock Text=""@#.UserName"" />
    <TextBlock Text=""@(#.ItemCount > 10 ? &quot;Many&quot; : &quot;Few&quot;)"" />
</Page>";

    private const string CSharpExample = @"using System.ComponentModel;
using Jalium.UI;
using Jalium.UI.Controls;

public partial class RazorDemoPage : Page
{
    public RazorDemoPage()
    {
        var vm = new RazorDemoViewModel();
        DataContext = vm;

        // Register AOT-safe property accessors
        Markup.RazorExpressionRegistry.RegisterPropertyAccessors(
            typeof(RazorDemoViewModel), [
            (""UserName"", o => ((RazorDemoViewModel)o).UserName),
            (""ItemCount"", o => ((RazorDemoViewModel)o).ItemCount),
            (""IsFeatureEnabled"", o => ((RazorDemoViewModel)o).IsFeatureEnabled),
        ]);

        InitializeComponent();
    }

    // ViewModel with INotifyPropertyChanged for runtime @if
    private sealed class RazorDemoViewModel : INotifyPropertyChanged
    {
        private bool _isFeatureEnabled = true;

        public string UserName { get; set; } = ""Jalium Developer"";
        public int ItemCount { get; set; } = 42;

        public bool IsFeatureEnabled
        {
            get => _isFeatureEnabled;
            set {
                _isFeatureEnabled = value;
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(nameof(IsFeatureEnabled)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

// $.Property — Self-reference syntax
// Access the control's own properties (RelativeSource Self):
//   @$.FontSize           → reads FontSize set on the control itself
//   @($.FontSize > 12)    → expression using own property
//   @$.Tag                → reads Tag set on the control itself (must be on the same element)
//
// Note: Razor string templates evaluate once at load time. For live-updating bindings
// to layout-driven properties (ActualWidth, ActualHeight), use a plain Binding with
// {RelativeSource={RelativeSource Self}} instead of an @$.Property template expression.
//
// #.Property — Data model reference syntax
// Access DataContext directly (ideal for ItemTemplate):
//   @#.UserName           → reads UserName from DataContext
//   @(#.ItemCount > 10)   → expression using DataContext property";

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
        LoadCodeExamples();

        if (ToggleButton != null)
            ToggleButton.Click += OnToggleClick;
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
