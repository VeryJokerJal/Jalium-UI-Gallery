using System.Text;
using System.Windows.Input;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class TerminalViewModel : ITerminalStateSink
{
    private long _sequence;
    private TerminalCursorStyle _cursorStyle = TerminalCursorStyle.Bar;
    private double _terminalFontSize = 14;
    private string _terminalTitle = "Terminal";
    private string _processStatus = "Running";
    private TerminalRequest? _mainRequest;
    private TerminalRequest? _demoRequest;

    public TerminalViewModel()
    {
        CursorStyles = Enum.GetValues<TerminalCursorStyle>();
        ClearCommand = Main(TerminalAction.Clear);
        CopyCommand = Main(TerminalAction.Copy);
        PasteCommand = Main(TerminalAction.Paste);
        RestartCommand = Main(TerminalAction.Restart);
        ShowAnsiColorsCommand = new RelayCommand(() =>
            DemoRequest = Request(
                TerminalAction.Write,
                BuildAnsiColors()));
        ShowTextStylesCommand = new RelayCommand(() =>
            DemoRequest = Request(
                TerminalAction.Write,
                AnsiStyles));
        ClearDemoCommand = new RelayCommand(() =>
            DemoRequest = Request(
                TerminalAction.ClearAndWrite,
                Welcome));
        DemoRequest = Request(TerminalAction.Write, Welcome);
    }

    public IReadOnlyList<TerminalCursorStyle> CursorStyles { get; }

    public ICommand ClearCommand { get; }

    public ICommand CopyCommand { get; }

    public ICommand PasteCommand { get; }

    public ICommand RestartCommand { get; }

    public ICommand ShowAnsiColorsCommand { get; }

    public ICommand ShowTextStylesCommand { get; }

    public ICommand ClearDemoCommand { get; }

    public TerminalCursorStyle CursorStyle
    {
        get => _cursorStyle;
        set => SetProperty(ref _cursorStyle, value);
    }

    public double TerminalFontSize
    {
        get => _terminalFontSize;
        set => SetProperty(ref _terminalFontSize, Math.Round(value));
    }

    public string TerminalTitle
    {
        get => _terminalTitle;
        private set => SetProperty(ref _terminalTitle, value);
    }

    public string ProcessStatus
    {
        get => _processStatus;
        private set => SetProperty(ref _processStatus, value);
    }

    public TerminalRequest? MainRequest
    {
        get => _mainRequest;
        private set => SetProperty(ref _mainRequest, value);
    }

    public TerminalRequest? DemoRequest
    {
        get => _demoRequest;
        private set => SetProperty(ref _demoRequest, value);
    }

    public void SetTerminalTitle(string title) =>
        TerminalTitle = title;

    public void SetProcessExited(int exitCode) =>
        ProcessStatus = $"Exited (code {exitCode})";

    public void SetProcessRunning() =>
        ProcessStatus = "Running";

    private ICommand Main(TerminalAction action) =>
        new RelayCommand(() => MainRequest = Request(action));

    private TerminalRequest Request(
        TerminalAction action,
        string output = "") =>
        new(++_sequence, action, output);

    private static string BuildAnsiColors()
    {
        var builder = new StringBuilder();
        builder.AppendLine("\x1b[1;33m--- Standard 16 Colors ---\x1b[0m");
        builder.Append("  FG: ");
        for (var color = 30; color <= 37; color++)
        {
            builder.Append($"\x1b[{color}m {color} \x1b[0m");
        }

        builder.AppendLine();
        builder.Append("  BF: ");
        for (var color = 90; color <= 97; color++)
        {
            builder.Append($"\x1b[{color}m {color} \x1b[0m");
        }

        builder.AppendLine();
        builder.AppendLine();
        return builder.ToString();
    }

    private const string Welcome =
        "\x1b[1;36m=== Jalium.UI Terminal Control Demo ===\x1b[0m\r\n\r\n" +
        "This terminal supports \x1b[1mbold\x1b[0m, " +
        "\x1b[3mitalic\x1b[0m, and \x1b[4munderline\x1b[0m text.\r\n" +
        "Use the buttons below to explore ANSI color and style support.\r\n\r\n";

    private const string AnsiStyles =
        "\x1b[1;33m--- Text Styles ---\x1b[0m\r\n" +
        "  \x1b[1mBold text\x1b[0m\r\n" +
        "  \x1b[2mDim text\x1b[0m\r\n" +
        "  \x1b[3mItalic text\x1b[0m\r\n" +
        "  \x1b[4mUnderlined text\x1b[0m\r\n" +
        "  \x1b[7mInverse text\x1b[0m\r\n" +
        "  \x1b[9mStrikethrough text\x1b[0m\r\n\r\n";
}
