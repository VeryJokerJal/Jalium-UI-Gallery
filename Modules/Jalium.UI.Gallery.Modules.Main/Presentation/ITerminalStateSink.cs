namespace Jalium.UI.Gallery.Modules.Main.Presentation;

public interface ITerminalStateSink
{
    void SetTerminalTitle(string title);

    void SetProcessExited(int exitCode);

    void SetProcessRunning();
}
