using System.Collections.ObjectModel;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Charts;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Code-behind for GanttChartPage.jalxaml demonstrating GanttChart functionality.
/// </summary>
public partial class GanttChartPage : Page
{
    private const string XamlExample = @"<!-- Basic Gantt Chart -->
<GanttChart x:Name=""BasicGantt""
            Height=""250""
            ShowDependencies=""False""
            ShowProgress=""False""
            ShowToday=""True""/>

<!-- Dependencies & Milestones -->
<GanttChart x:Name=""DependenciesGantt""
            Height=""280""
            ShowDependencies=""True""
            ShowProgress=""False""
            ShowToday=""True""/>

<!-- Progress Tracking -->
<GanttChart x:Name=""ProgressGantt""
            Height=""280""
            ShowDependencies=""True""
            ShowProgress=""True""
            ShowToday=""True""/>";

    private const string CSharpExample = @"using System.Collections.ObjectModel;
using Jalium.UI.Controls.Charts;

var baseDate = new DateTime(2026, 4, 1);

var chart = new GanttChart
{
    Height = 280,
    ShowDependencies = true,
    ShowProgress = true,
    ShowToday = true,
    Tasks = new ObservableCollection<GanttTask>
    {
        new GanttTask
        {
            Id = ""req"",
            Name = ""Requirements"",
            StartDate = baseDate,
            EndDate = baseDate.AddDays(4),
            Progress = 1.0
        },
        new GanttTask
        {
            Id = ""arch"",
            Name = ""Architecture"",
            StartDate = baseDate.AddDays(4),
            EndDate = baseDate.AddDays(8),
            Progress = 0.75,
            DependsOn = new List<string> { ""req"" }
        },
        new GanttTask
        {
            Id = ""milestone"",
            Name = ""Review"",
            StartDate = baseDate.AddDays(8),
            EndDate = baseDate.AddDays(8),
            IsMilestone = true,
            DependsOn = new List<string> { ""arch"" }
        }
    }
};";

    public GanttChartPage()
    {
        InitializeComponent();
        SetupBasicGantt();
        SetupDependenciesGantt();
        SetupProgressGantt();
        LoadCodeExamples();
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

    private void SetupBasicGantt()
    {
        if (BasicGanttContainer == null) return;

        var baseDate = new DateTime(2026, 4, 1);

        var chart = new GanttChart
        {
            Height = 250,
            ShowDependencies = false,
            ShowProgress = false,
            ShowToday = true,
            Tasks = new ObservableCollection<GanttTask>
            {
                new GanttTask
                {
                    Id = "planning",
                    Name = "Planning",
                    StartDate = baseDate,
                    EndDate = baseDate.AddDays(5)
                },
                new GanttTask
                {
                    Id = "design",
                    Name = "UI Design",
                    StartDate = baseDate.AddDays(3),
                    EndDate = baseDate.AddDays(10)
                },
                new GanttTask
                {
                    Id = "backend",
                    Name = "Backend Dev",
                    StartDate = baseDate.AddDays(6),
                    EndDate = baseDate.AddDays(18)
                },
                new GanttTask
                {
                    Id = "frontend",
                    Name = "Frontend Dev",
                    StartDate = baseDate.AddDays(10),
                    EndDate = baseDate.AddDays(22)
                },
                new GanttTask
                {
                    Id = "testing",
                    Name = "Testing",
                    StartDate = baseDate.AddDays(18),
                    EndDate = baseDate.AddDays(28)
                },
                new GanttTask
                {
                    Id = "deployment",
                    Name = "Deployment",
                    StartDate = baseDate.AddDays(26),
                    EndDate = baseDate.AddDays(30)
                }
            }
        };

        BasicGanttContainer.Children.Add(chart);
    }

    private void SetupDependenciesGantt()
    {
        if (DependenciesGanttContainer == null) return;

        var baseDate = new DateTime(2026, 4, 1);

        var chart = new GanttChart
        {
            Height = 280,
            ShowDependencies = true,
            ShowProgress = false,
            ShowToday = true,
            Tasks = new ObservableCollection<GanttTask>
            {
                new GanttTask
                {
                    Id = "req",
                    Name = "Requirements",
                    StartDate = baseDate,
                    EndDate = baseDate.AddDays(4)
                },
                new GanttTask
                {
                    Id = "arch",
                    Name = "Architecture",
                    StartDate = baseDate.AddDays(4),
                    EndDate = baseDate.AddDays(8),
                    DependsOn = new List<string> { "req" }
                },
                new GanttTask
                {
                    Id = "proto",
                    Name = "Prototype",
                    StartDate = baseDate.AddDays(8),
                    EndDate = baseDate.AddDays(14),
                    DependsOn = new List<string> { "arch" }
                },
                new GanttTask
                {
                    Id = "review_ms",
                    Name = "Design Review",
                    StartDate = baseDate.AddDays(14),
                    EndDate = baseDate.AddDays(14),
                    IsMilestone = true,
                    DependsOn = new List<string> { "proto" }
                },
                new GanttTask
                {
                    Id = "impl",
                    Name = "Implementation",
                    StartDate = baseDate.AddDays(15),
                    EndDate = baseDate.AddDays(25),
                    DependsOn = new List<string> { "review_ms" }
                },
                new GanttTask
                {
                    Id = "qa",
                    Name = "QA Testing",
                    StartDate = baseDate.AddDays(22),
                    EndDate = baseDate.AddDays(30),
                    DependsOn = new List<string> { "impl" }
                },
                new GanttTask
                {
                    Id = "release_ms",
                    Name = "Release",
                    StartDate = baseDate.AddDays(30),
                    EndDate = baseDate.AddDays(30),
                    IsMilestone = true,
                    DependsOn = new List<string> { "qa" }
                }
            }
        };

        DependenciesGanttContainer.Children.Add(chart);
    }

    private void SetupProgressGantt()
    {
        if (ProgressGanttContainer == null) return;

        var baseDate = new DateTime(2026, 3, 15);

        var chart = new GanttChart
        {
            Height = 280,
            ShowDependencies = true,
            ShowProgress = true,
            ShowToday = true,
            Tasks = new ObservableCollection<GanttTask>
            {
                new GanttTask
                {
                    Id = "research",
                    Name = "Research",
                    StartDate = baseDate,
                    EndDate = baseDate.AddDays(7),
                    Progress = 1.0
                },
                new GanttTask
                {
                    Id = "wireframes",
                    Name = "Wireframes",
                    StartDate = baseDate.AddDays(5),
                    EndDate = baseDate.AddDays(12),
                    Progress = 0.9,
                    DependsOn = new List<string> { "research" }
                },
                new GanttTask
                {
                    Id = "db_schema",
                    Name = "DB Schema",
                    StartDate = baseDate.AddDays(7),
                    EndDate = baseDate.AddDays(14),
                    Progress = 0.75,
                    DependsOn = new List<string> { "research" }
                },
                new GanttTask
                {
                    Id = "api_dev",
                    Name = "API Development",
                    StartDate = baseDate.AddDays(12),
                    EndDate = baseDate.AddDays(24),
                    Progress = 0.4,
                    DependsOn = new List<string> { "db_schema" }
                },
                new GanttTask
                {
                    Id = "ui_dev",
                    Name = "UI Development",
                    StartDate = baseDate.AddDays(12),
                    EndDate = baseDate.AddDays(26),
                    Progress = 0.3,
                    DependsOn = new List<string> { "wireframes" }
                },
                new GanttTask
                {
                    Id = "integration",
                    Name = "Integration",
                    StartDate = baseDate.AddDays(22),
                    EndDate = baseDate.AddDays(30),
                    Progress = 0.0,
                    DependsOn = new List<string> { "api_dev", "ui_dev" }
                },
                new GanttTask
                {
                    Id = "launch",
                    Name = "Launch",
                    StartDate = baseDate.AddDays(30),
                    EndDate = baseDate.AddDays(30),
                    IsMilestone = true,
                    Progress = 0.0,
                    DependsOn = new List<string> { "integration" }
                }
            }
        };

        ProgressGanttContainer.Children.Add(chart);
    }
}
