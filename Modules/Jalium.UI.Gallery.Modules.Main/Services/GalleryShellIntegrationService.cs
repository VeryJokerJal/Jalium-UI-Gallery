using Jalium.UI.Controls.Shell;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Shell;

namespace Jalium.UI.Gallery.Modules.Main.Services;

internal sealed class GalleryShellIntegrationService
    : IGalleryShellIntegrationService
{
    public bool TryApplySampleJumpList()
    {
        if (Jalium.UI.Application.Current is not { } application)
        {
            return false;
        }

        var jumpList = new JumpList
        {
            ShowRecentCategory = true,
            ShowFrequentCategory = true
        };

        jumpList.JumpItems.Add(new JumpTask
        {
            Title = "New Document",
            Description = "Create a new document",
            ApplicationPath = Environment.ProcessPath,
            Arguments = "--new",
            CustomCategory = "Tasks"
        });
        jumpList.JumpItems.Add(new JumpTask
        {
            Title = "Open Settings",
            Description = "Open application settings",
            ApplicationPath = Environment.ProcessPath,
            Arguments = "--settings",
            CustomCategory = "Tasks"
        });

        JumpList.SetJumpList(application, jumpList);
        return true;
    }

    public bool TrySetTaskbarProgress(double progress)
    {
        var taskbar = GetTaskbar();
        if (taskbar is null)
        {
            return false;
        }

        taskbar.ProgressValue = Math.Clamp(progress, 0d, 1d);
        return true;
    }

    public bool TrySetTaskbarState(TaskbarProgressMode mode)
    {
        var taskbar = GetTaskbar();
        if (taskbar is null)
        {
            return false;
        }

        taskbar.ProgressState = mode switch
        {
            TaskbarProgressMode.Normal =>
                TaskbarItemProgressState.Normal,
            TaskbarProgressMode.Paused =>
                TaskbarItemProgressState.Paused,
            TaskbarProgressMode.Error =>
                TaskbarItemProgressState.Error,
            _ => TaskbarItemProgressState.None
        };
        return true;
    }

    public SystemShellSnapshot ReadSystemParameters()
    {
        var color = SystemParameters2.WindowGlassColor;
        var border = SystemParameters2.WindowResizeBorderThickness;
        return new SystemShellSnapshot(
            SystemParameters2.IsGlassEnabled,
            $"#{color.R:X2}{color.G:X2}{color.B:X2}",
            SystemParameters2.WindowCaptionHeight,
            $"{border.Left},{border.Top},{border.Right},{border.Bottom}");
    }

    private static TaskbarItemInfo? GetTaskbar()
    {
        var window = Jalium.UI.Application.Current?.MainWindow;
        if (window is null)
        {
            return null;
        }

        window.TaskbarItemInfo ??= new TaskbarItemInfo();
        return window.TaskbarItemInfo;
    }
}
