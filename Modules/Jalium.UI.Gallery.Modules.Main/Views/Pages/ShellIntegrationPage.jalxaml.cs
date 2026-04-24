using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.Shell;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

public partial class ShellIntegrationPage : Page
{
    public ShellIntegrationPage()
    {
        InitializeComponent();
        SetupButtons();
        SetupSliders();
        UpdateSystemParameters();
        LoadCodeExamples();
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

    private const string XamlExample =
@"<!-- Apply custom WindowChrome to a Window -->
<Window Title=""Custom Chrome Window"">
    <WindowChrome.WindowChrome>
        <WindowChrome CaptionHeight=""30""
                      CornerRadius=""8""
                      GlassFrameThickness=""-1""
                      ResizeBorderThickness=""4""
                      UseAeroCaptionButtons=""False""/>
    </WindowChrome.WindowChrome>

    <!-- Window content here -->
</Window>

<!-- TaskbarItemInfo for progress and overlay -->
<Window.TaskbarItemInfo>
    <TaskbarItemInfo ProgressState=""Normal""
                     ProgressValue=""0.5""
                     Description=""My Application""/>
</Window.TaskbarItemInfo>";

    private const string CSharpExample =
@"// Apply custom WindowChrome programmatically
var chrome = new WindowChrome
{
    CaptionHeight = 30,
    CornerRadius = new CornerRadius(8),
    GlassFrameThickness = new Thickness(-1),
    ResizeBorderThickness = new Thickness(4),
    UseAeroCaptionButtons = false
};
WindowChrome.SetWindowChrome(window, chrome);

// Configure JumpList for the taskbar
var jumpList = new JumpList
{
    ShowRecentCategory = true,
    ShowFrequentCategory = true
};

jumpList.JumpItems.Add(new JumpTask
{
    Title = ""New Document"",
    Description = ""Create a new document"",
    ApplicationPath = Environment.ProcessPath,
    Arguments = ""--new"",
    CustomCategory = ""Tasks""
});

JumpList.SetJumpList(Application.Current, jumpList);

// Set taskbar progress indicator
window.TaskbarItemInfo = new TaskbarItemInfo
{
    ProgressState = TaskbarItemProgressState.Normal,
    ProgressValue = 0.75
};

// Read system parameters
bool glassEnabled = SystemParameters2.IsGlassEnabled;
Color glassColor = SystemParameters2.WindowGlassColor;
double captionHeight = SystemParameters2.WindowCaptionHeight;";
}
