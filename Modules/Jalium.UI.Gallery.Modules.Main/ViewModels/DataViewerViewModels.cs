namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class DiffViewerViewModel
{
    private string _interactiveOriginalText =
        "Hello World\nThis is a test\nLine three";
    private string _interactiveModifiedText =
        "Hello World\nThis is modified\nLine three\nLine four added";

    public string SampleOriginalText { get; } =
        """
        using System;

        namespace MyApp;

        public class Calculator
        {
            public int Add(int a, int b)
            {
                return a + b;
            }

            public int Subtract(int a, int b)
            {
                return a - b;
            }
        }
        """;

    public string SampleModifiedText { get; } =
        """
        using System;
        using System.Diagnostics;

        namespace MyApp;

        public class Calculator
        {
            public int Add(int a, int b)
            {
                Debug.Assert(a >= 0 && b >= 0, "Values must be non-negative");
                return a + b;
            }

            public int Subtract(int a, int b)
            {
                return a - b;
            }

            public int Multiply(int a, int b)
            {
                return a * b;
            }
        }
        """;

    public string InteractiveOriginalText
    {
        get => _interactiveOriginalText;
        set => SetProperty(ref _interactiveOriginalText, value);
    }

    public string InteractiveModifiedText
    {
        get => _interactiveModifiedText;
        set => SetProperty(ref _interactiveModifiedText, value);
    }
}

public sealed partial class JsonTreeViewerViewModel
{
    private string _searchText = string.Empty;
    private string _interactiveJson =
        "{\n  \"greeting\": \"Hello World\",\n  \"count\": 42,\n  \"active\": true,\n  \"items\": [1, 2, 3]\n}";

    public string SampleJson { get; } =
        """
        {
          "application": {
            "name": "Jalium.UI Gallery",
            "version": "1.0.0",
            "framework": "Jalium.UI"
          },
          "settings": {
            "theme": "dark",
            "fontSize": 14,
            "showLineNumbers": true,
            "recentFiles": [
              "document1.txt",
              "image.png",
              "config.json"
            ],
            "window": {
              "width": 1280,
              "height": 720,
              "isMaximized": false
            }
          },
          "plugins": [
            {
              "id": "syntax-highlight",
              "enabled": true,
              "version": "2.1.0"
            },
            {
              "id": "auto-complete",
              "enabled": true,
              "version": "1.5.3"
            },
            {
              "id": "git-integration",
              "enabled": false,
              "version": null
            }
          ],
          "metadata": {
            "createdAt": "2026-01-15T10:30:00Z",
            "modifiedAt": "2026-03-31T14:00:00Z",
            "tags": ["ui", "desktop", "windows"]
          }
        }
        """;

    public string SearchSampleJson { get; } =
        """
        {
          "employees": [
            { "name": "Alice Johnson", "department": "Engineering", "salary": 95000 },
            { "name": "Bob Smith", "department": "Design", "salary": 82000 },
            { "name": "Charlie Brown", "department": "Engineering", "salary": 105000 },
            { "name": "Diana Prince", "department": "Marketing", "salary": 78000 },
            { "name": "Eve Adams", "department": "Engineering", "salary": 92000 }
          ],
          "departments": ["Engineering", "Design", "Marketing", "Sales"],
          "totalBudget": 1500000
        }
        """;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public string InteractiveJson
    {
        get => _interactiveJson;
        set => SetProperty(ref _interactiveJson, value);
    }
}

public sealed partial class FlowchartViewModel
{
    private string _liveSource =
        """
        flowchart TD
            Start([Start]) --> Input[/Read input/]
            Input --> Check{Valid?}
            Check -->|yes| Save[(Save)]
            Check -->|no| Input
            Save --> Done([Done])
        """;

    public string DecisionSource { get; } =
        """
        flowchart TD
            A[Start] --> B{Condition}
            B -->|yes| C[Execute operation A]
            B -->|no| D[Execute operation B]
            C --> E{Sub-condition}
            D --> F[Write log]
            E -->|pass| G[Commit result]
            E -->|fail| H[Roll back]
            H --> D
            F --> G
            G --> I([Finish])
        """;

    public string ShapesSource { get; } =
        """
        flowchart LR
            A[Rectangle] --> B(Rounded)
            B --> C([Stadium])
            C --> D{Decision}
            D -->|yes| E((Circle))
            D -->|no| F{{Hexagon}}
            E --> G[(Database)]
            F -.-> G
        """;

    public string LiveSource
    {
        get => _liveSource;
        set => SetProperty(ref _liveSource, value);
    }
}
