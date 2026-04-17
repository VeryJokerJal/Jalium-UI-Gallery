using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Views;

/// <summary>
/// Code-behind for DiffViewerPage.jalxaml demonstrating the DiffViewer control.
/// </summary>
public partial class DiffViewerPage : Page
{
    private const string XamlExample = """
        <!-- Side-by-Side Diff -->
        <DiffViewer x:Name="SideBySideDiff"
                    ViewMode="SideBySide"
                    Height="300"
                    ShowLineNumbers="True"
                    ShowMinimap="True"/>

        <!-- Unified Diff -->
        <DiffViewer x:Name="UnifiedDiff"
                    ViewMode="Unified"
                    Height="300"
                    ShowLineNumbers="True"/>

        <!-- Interactive: edit text and see diff update -->
        <TextBox x:Name="OriginalTextInput"
                 AcceptsReturn="True" Height="120"/>
        <TextBox x:Name="ModifiedTextInput"
                 AcceptsReturn="True" Height="120"/>
        <DiffViewer x:Name="InteractiveDiff"
                    ViewMode="SideBySide"
                    Height="250"
                    ShowLineNumbers="True"/>
        """;

    private const string CSharpExample = """
        using Jalium.UI.Controls;

        // Set up side-by-side diff
        diffViewer.ViewMode = DiffViewMode.SideBySide;
        diffViewer.OriginalText = originalCode;
        diffViewer.ModifiedText = modifiedCode;
        diffViewer.ShowLineNumbers = true;
        diffViewer.ShowMinimap = true;

        // Set up unified diff
        unifiedDiff.ViewMode = DiffViewMode.Unified;
        unifiedDiff.OriginalText = originalCode;
        unifiedDiff.ModifiedText = modifiedCode;

        // Interactive demo: update diff on text change
        originalInput.TextChanged += (s, e) =>
        {
            interactiveDiff.OriginalText = originalInput.Text ?? "";
        };
        modifiedInput.TextChanged += (s, e) =>
        {
            interactiveDiff.ModifiedText = modifiedInput.Text ?? "";
        };
        """;

    private const string SampleOriginalText =
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

    private const string SampleModifiedText =
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

    public DiffViewerPage()
    {
        InitializeComponent();
        LoadCodeExamples();

        // Set up side-by-side diff
        if (SideBySideDiff != null)
        {
            SideBySideDiff.OriginalText = SampleOriginalText;
            SideBySideDiff.ModifiedText = SampleModifiedText;
        }

        // Set up unified diff with the same sample text
        if (UnifiedDiff != null)
        {
            UnifiedDiff.OriginalText = SampleOriginalText;
            UnifiedDiff.ModifiedText = SampleModifiedText;
        }

        // Set up interactive demo
        if (OriginalTextInput != null)
        {
            OriginalTextInput.Text = "Hello World\nThis is a test\nLine three";
            OriginalTextInput.TextChanged += OnInteractiveTextChanged;
        }

        if (ModifiedTextInput != null)
        {
            ModifiedTextInput.Text = "Hello World\nThis is modified\nLine three\nLine four added";
            ModifiedTextInput.TextChanged += OnInteractiveTextChanged;
        }

        UpdateInteractiveDiff();
    }

    private void OnInteractiveTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateInteractiveDiff();
    }

    private void UpdateInteractiveDiff()
    {
        if (InteractiveDiff != null)
        {
            InteractiveDiff.OriginalText = OriginalTextInput?.Text ?? "";
            InteractiveDiff.ModifiedText = ModifiedTextInput?.Text ?? "";
        }
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
}
