using Jalium.UI.Controls;
using Jalium.UI.Controls.Shell;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

public partial class ShellIntegrationPage : Page
{
    public ShellIntegrationPage()
    {
        InitializeComponent();
        SetupButtons();
        SetupSliders();
        UpdateSystemParameters();
    }

    private void SetupButtons()
    {
        if (ApplyJumpListButton != null)
        {
            ApplyJumpListButton.Click += (s, e) =>
            {
                var jumpList = new JumpList
                {
                    ShowRecentCategory = true,
                    ShowFrequentCategory = true
                };

                // Add sample tasks
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

                // Apply the jump list
                JumpList.SetJumpList(Application.Current, jumpList);
            };
        }

        // Progress state buttons
        if (ProgressNoneButton != null)
        {
            ProgressNoneButton.Click += (s, e) => SetTaskbarProgressState(TaskbarItemProgressState.None);
        }

        if (ProgressNormalButton != null)
        {
            ProgressNormalButton.Click += (s, e) => SetTaskbarProgressState(TaskbarItemProgressState.Normal);
        }

        if (ProgressPausedButton != null)
        {
            ProgressPausedButton.Click += (s, e) => SetTaskbarProgressState(TaskbarItemProgressState.Paused);
        }

        if (ProgressErrorButton != null)
        {
            ProgressErrorButton.Click += (s, e) => SetTaskbarProgressState(TaskbarItemProgressState.Error);
        }
    }

    private void SetupSliders()
    {
        if (TaskbarProgressSlider != null)
        {
            TaskbarProgressSlider.ValueChanged += (s, e) =>
            {
                var value = (int)e.NewValue;
                if (TaskbarProgressText != null)
                {
                    TaskbarProgressText.Text = $"{value}%";
                }

                // Update taskbar progress
                var window = FindParentWindow();
                if (window?.TaskbarItemInfo != null)
                {
                    window.TaskbarItemInfo.ProgressValue = value / 100.0;
                }
            };
        }
    }

    private void SetTaskbarProgressState(TaskbarItemProgressState state)
    {
        var window = FindParentWindow();
        if (window != null)
        {
            if (window.TaskbarItemInfo == null)
            {
                window.TaskbarItemInfo = new TaskbarItemInfo();
            }
            window.TaskbarItemInfo.ProgressState = state;

            if (TaskbarStatus != null)
            {
                TaskbarStatus.Text = $"Taskbar State: {state}";
            }
        }
    }

    private void UpdateSystemParameters()
    {
        if (GlassEnabledText != null)
        {
            GlassEnabledText.Text = $"DWM Glass Enabled: {SystemParameters2.IsGlassEnabled}";
        }

        if (GlassColorText != null)
        {
            var color = SystemParameters2.WindowGlassColor;
            GlassColorText.Text = $"Glass Color: #{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        if (CaptionHeightText != null)
        {
            CaptionHeightText.Text = $"Caption Height: {SystemParameters2.WindowCaptionHeight}";
        }

        if (ResizeBorderText != null)
        {
            var border = SystemParameters2.WindowResizeBorderThickness;
            ResizeBorderText.Text = $"Resize Border: {border.Left},{border.Top},{border.Right},{border.Bottom}";
        }
    }

    private Window? FindParentWindow()
    {
        Visual? current = this;
        while (current != null)
        {
            if (current is Window window)
                return window;
            current = current.VisualParent;
        }
        return null;
    }
}
