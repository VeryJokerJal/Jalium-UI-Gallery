using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for TerminalPage.jalxaml demonstrating the Terminal control.
/// </summary>
public partial class TerminalPage : Page
{
    private const string XamlExample = @"<!-- Interactive Terminal -->
<Border Background=""#0C0C0C""
        CornerRadius=""4""
        Padding=""0""
        HorizontalAlignment=""Stretch"">
    <Terminal x:Name=""MainTerminal""
             Height=""400""
             HorizontalAlignment=""Stretch""
             FontSize=""14""
             AutoSize=""True""
             AutoStartShell=""True""
             CursorStyle=""Bar""
             MaxScrollbackLines=""10000""/>
</Border>

<!-- Read-Only Demo Terminal -->
<Terminal x:Name=""DemoTerminal""
         Height=""200""
         HorizontalAlignment=""Stretch""
         FontSize=""14""
         AutoSize=""True""
         AutoStartShell=""False""
         IsReadOnly=""True""
         CursorStyle=""Bar""/>";

    private const string CSharpExample = @"using Jalium.UI.Controls;

// Wire up terminal events
MainTerminal.TitleChanged += (s, e) =>
{
    TitleText.Text = MainTerminal.Title;
};

MainTerminal.ProcessExited += (s, e) =>
{
    StatusText.Text = $""Exited (code {e.ExitCode})"";
};

// Change cursor style
MainTerminal.CursorStyle = TerminalCursorStyle.Block;

// Terminal operations
MainTerminal.Clear();
MainTerminal.Copy();
MainTerminal.Paste();

// Restart shell
MainTerminal.StopShell();
MainTerminal.Clear();
MainTerminal.StartShell();

// Write ANSI-colored text to read-only terminal
DemoTerminal.WriteLine(
    ""\x1b[1;36m=== Demo ===\x1b[0m"");
DemoTerminal.WriteLine(
    ""\x1b[1mBold\x1b[0m and \x1b[3mItalic\x1b[0m"");

// 256-color output
for (int i = 16; i < 232; i += 6)
    DemoTerminal.Write(
        $""\x1b[38;5;{i}m\u2588\x1b[0m"");

// True-color gradient
for (int i = 0; i < 60; i++)
{
    int r = (int)(255 * (1.0 - i / 60.0));
    int g = (int)(255 * (i / 60.0));
    DemoTerminal.Write(
        $""\x1b[38;2;{r};{g};128m\u2588\x1b[0m"");
}";

    public TerminalPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Wire up event handlers for interactive terminal
        if (MainTerminal != null)
        {
            MainTerminal.TitleChanged += OnTerminalTitleChanged;
            MainTerminal.ProcessExited += OnTerminalProcessExited;
        }

        // Wire up option controls
        if (CursorStyleComboBox != null)
            CursorStyleComboBox.SelectionChanged += OnCursorStyleChanged;

        if (FontSizeSlider != null)
            FontSizeSlider.ValueChanged += OnFontSizeChanged;

        if (ClearButton != null)
            ClearButton.Click += OnClearClick;

        if (CopyButton != null)
            CopyButton.Click += OnCopyClick;

        if (PasteButton != null)
            PasteButton.Click += OnPasteClick;

        if (RestartButton != null)
            RestartButton.Click += OnRestartClick;

        // Wire up demo terminal buttons
        if (AnsiColorsButton != null)
            AnsiColorsButton.Click += OnAnsiColorsClick;

        if (AnsiStylesButton != null)
            AnsiStylesButton.Click += OnAnsiStylesClick;

        if (ClearDemoButton != null)
            ClearDemoButton.Click += OnClearDemoClick;

        // Write initial demo content
        WriteDemoWelcome();
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

    #region Interactive Terminal Events

    private void OnTerminalTitleChanged(object sender, RoutedEventArgs e)
    {
        if (TitleText != null && MainTerminal != null)
            TitleText.Text = MainTerminal.Title;
    }

    private void OnTerminalProcessExited(object sender, TerminalProcessExitedEventArgs e)
    {
        if (ProcessStatusText != null)
            ProcessStatusText.Text = $"Exited (code {e.ExitCode})";
    }

    #endregion

    #region Options

    private void OnCursorStyleChanged(object? sender, EventArgs e)
    {
        if (MainTerminal == null || CursorStyleComboBox == null) return;

        MainTerminal.CursorStyle = CursorStyleComboBox.SelectedIndex switch
        {
            0 => TerminalCursorStyle.Bar,
            1 => TerminalCursorStyle.Block,
            2 => TerminalCursorStyle.Underline,
            _ => TerminalCursorStyle.Bar
        };
    }

    private void OnFontSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MainTerminal != null)
            MainTerminal.FontSize = (int)e.NewValue;

        if (FontSizeLabel != null)
            FontSizeLabel.Text = ((int)e.NewValue).ToString();
    }

    private void OnClearClick(object? sender, EventArgs e)
    {
        MainTerminal?.Clear();
    }

    private void OnCopyClick(object? sender, EventArgs e)
    {
        MainTerminal?.Copy();
    }

    private void OnPasteClick(object? sender, EventArgs e)
    {
        MainTerminal?.Paste();
    }

    private void OnRestartClick(object? sender, EventArgs e)
    {
        if (MainTerminal == null) return;

        MainTerminal.StopShell();
        MainTerminal.Clear();
        MainTerminal.StartShell();

        if (ProcessStatusText != null)
            ProcessStatusText.Text = "Running";
    }

    #endregion

    #region Demo Terminal

    private void WriteDemoWelcome()
    {
        if (DemoTerminal == null) return;

        DemoTerminal.WriteLine("\x1b[1;36m=== Jalium.UI Terminal Control Demo ===\x1b[0m");
        DemoTerminal.WriteLine("");
        DemoTerminal.WriteLine("This terminal supports \x1b[1mbold\x1b[0m, \x1b[3mitalic\x1b[0m, \x1b[4munderline\x1b[0m, and \x1b[9mstrikethrough\x1b[0m text.");
        DemoTerminal.WriteLine("Use the buttons below to explore ANSI color and style support.");
        DemoTerminal.WriteLine("");
    }

    private void OnAnsiColorsClick(object? sender, EventArgs e)
    {
        if (DemoTerminal == null) return;

        DemoTerminal.WriteLine("\x1b[1;33m--- Standard 16 Colors ---\x1b[0m");

        // Standard foreground colors
        DemoTerminal.Write("  FG: ");
        for (int i = 30; i <= 37; i++)
            DemoTerminal.Write($"\x1b[{i}m {i} \x1b[0m");
        DemoTerminal.WriteLine("");

        // Bright foreground colors
        DemoTerminal.Write("  BF: ");
        for (int i = 90; i <= 97; i++)
            DemoTerminal.Write($"\x1b[{i}m {i} \x1b[0m");
        DemoTerminal.WriteLine("");

        // Standard background colors
        DemoTerminal.Write("  BG: ");
        for (int i = 40; i <= 47; i++)
            DemoTerminal.Write($"\x1b[{i}m {i} \x1b[0m");
        DemoTerminal.WriteLine("");

        // Bright background colors
        DemoTerminal.Write("  BB: ");
        for (int i = 100; i <= 107; i++)
            DemoTerminal.Write($"\x1b[{i}m{i} \x1b[0m");
        DemoTerminal.WriteLine("");

        // 256-color palette sample
        DemoTerminal.WriteLine("");
        DemoTerminal.WriteLine("\x1b[1;33m--- 256 Color Palette (sample) ---\x1b[0m");
        DemoTerminal.Write("  ");
        for (int i = 16; i < 232; i += 6)
            DemoTerminal.Write($"\x1b[38;5;{i}m\u2588\x1b[0m");
        DemoTerminal.WriteLine("");

        // True-color gradient
        DemoTerminal.WriteLine("");
        DemoTerminal.WriteLine("\x1b[1;33m--- True Color Gradient ---\x1b[0m");
        DemoTerminal.Write("  ");
        for (int i = 0; i < 60; i++)
        {
            int r = (int)(255 * (1.0 - i / 60.0));
            int g = (int)(255 * (i / 60.0));
            int b = 128;
            DemoTerminal.Write($"\x1b[38;2;{r};{g};{b}m\u2588\x1b[0m");
        }
        DemoTerminal.WriteLine("");
        DemoTerminal.WriteLine("");
    }

    private void OnAnsiStylesClick(object? sender, EventArgs e)
    {
        if (DemoTerminal == null) return;

        DemoTerminal.WriteLine("\x1b[1;33m--- Text Styles ---\x1b[0m");
        DemoTerminal.WriteLine("  \x1b[1mBold text\x1b[0m              (ESC[1m)");
        DemoTerminal.WriteLine("  \x1b[2mDim text\x1b[0m               (ESC[2m)");
        DemoTerminal.WriteLine("  \x1b[3mItalic text\x1b[0m            (ESC[3m)");
        DemoTerminal.WriteLine("  \x1b[4mUnderlined text\x1b[0m        (ESC[4m)");
        DemoTerminal.WriteLine("  \x1b[7mInverse text\x1b[0m           (ESC[7m)");
        DemoTerminal.WriteLine("  \x1b[9mStrikethrough text\x1b[0m     (ESC[9m)");
        DemoTerminal.WriteLine("  \x1b[1;3;4mBold+Italic+Underline\x1b[0m  (ESC[1;3;4m)");
        DemoTerminal.WriteLine("");

        DemoTerminal.WriteLine("\x1b[1;33m--- Color + Style Combinations ---\x1b[0m");
        DemoTerminal.WriteLine("  \x1b[1;31mBold Red\x1b[0m    \x1b[1;32mBold Green\x1b[0m    \x1b[1;34mBold Blue\x1b[0m");
        DemoTerminal.WriteLine("  \x1b[3;35mItalic Magenta\x1b[0m  \x1b[4;36mUnderline Cyan\x1b[0m");
        DemoTerminal.WriteLine("  \x1b[1;33;44m Yellow on Blue \x1b[0m  \x1b[30;43m Black on Yellow \x1b[0m");
        DemoTerminal.WriteLine("");
    }

    private void OnClearDemoClick(object? sender, EventArgs e)
    {
        if (DemoTerminal == null) return;

        DemoTerminal.Clear();
        WriteDemoWelcome();
    }

    #endregion
}
