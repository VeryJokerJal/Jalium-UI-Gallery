using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Editor;
using Jalium.UI.Controls.TextEffects;
using Jalium.UI.Controls.TextEffects.Effects;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;

namespace Jalium.UI.Gallery.Modules.Main.Views.Pages;

/// <summary>
/// Gallery demo for <see cref="TextEffectPresenter"/>. Three demo hosts, each
/// exercises one concern:
///   (1) MutationDemoHost  — Append / Insert / Remove / Clear API, default effect
///   (2) EffectSwitchHost  — swap between the four built-in ITextEffect impls
///   (3) EffectComposeHost — compose per-cell CPU animation with a whole-element GPU pass
/// </summary>
public partial class TextEffectPresenterPage : Page
{
    private TextEffectPresenter _mutationPresenter = null!;
    private TextEffectPresenter _effectSwitchPresenter = null!;
    private TextEffectPresenter _composePresenter = null!;

    private const string InitialMutationText = "Type a lyric...";
    private const string EffectSwitchText = "Animate me";
    private const string ComposeText = "Hello Jalium";

    public TextEffectPresenterPage()
    {
        InitializeComponent();
        BuildDemoPresenters();
        LoadCodeExamples();
    }

    private void BuildDemoPresenters()
    {
        _mutationPresenter = CreatePresenter(InitialMutationText, new RiseSettleEffect());
        if (MutationDemoHost != null)
        {
            MutationDemoHost.Child = _mutationPresenter;
        }

        _effectSwitchPresenter = CreatePresenter(EffectSwitchText, new RiseSettleEffect());
        if (EffectSwitchDemoHost != null)
        {
            EffectSwitchDemoHost.Child = _effectSwitchPresenter;
        }

        _composePresenter = CreatePresenter(ComposeText, new WaveBounceEffect());
        if (EffectComposeDemoHost != null)
        {
            EffectComposeDemoHost.Child = _composePresenter;
        }
    }

    private static TextEffectPresenter CreatePresenter(string initialText, ITextEffect effect)
    {
        return new TextEffectPresenter
        {
            Text = initialText,
            TextEffect = effect,
            FontSize = 28,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(0xF3, 0xF3, 0xF3)),
        };
    }

    #region Card 1 — Mutation API

    private void OnAppendClick(object sender, RoutedEventArgs e)
    {
        _mutationPresenter.AppendText("lyric ");
    }

    private void OnAppendLineClick(object sender, RoutedEventArgs e)
    {
        _mutationPresenter.AppendTextLine("new line");
    }

    private void OnInsertClick(object sender, RoutedEventArgs e)
    {
        // Insert at cell (grapheme) index 3. If the current text is shorter,
        // the presenter clamps for us.
        _mutationPresenter.InsertText(3, "[INS]");
    }

    private void OnRemoveClick(object sender, RoutedEventArgs e)
    {
        _mutationPresenter.RemoveText(3, 2);
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        _mutationPresenter.ClearText();
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        _mutationPresenter.Text = InitialMutationText;
    }

    #endregion

    #region Card 2 — Effect switcher

    private void OnUseRiseSettleClick(object sender, RoutedEventArgs e) => ResetWithEffect(new RiseSettleEffect());
    private void OnUseFadeInClick(object sender, RoutedEventArgs e) => ResetWithEffect(new FadeInEffect());
    private void OnUseTypewriterClick(object sender, RoutedEventArgs e) => ResetWithEffect(new TypewriterEffect { CharacterDelayMs = 55 });
    private void OnUseWaveBounceClick(object sender, RoutedEventArgs e) => ResetWithEffect(new WaveBounceEffect());
    private void OnUsePulsingBlurClick(object sender, RoutedEventArgs e) => ResetWithEffect(new PulsingBlurTextEffect
    {
        AmplitudePx = 8.0,
        BaselinePx = 1.0,
        PeriodMs = 2400.0,
    });

    private void ResetWithEffect(ITextEffect effect)
    {
        // Clear first so the new effect gets to play the full enter curve on
        // every cell; otherwise unchanged cells would stay Visible and only
        // the diff would animate.
        _effectSwitchPresenter.ClearText();
        _effectSwitchPresenter.TextEffect = effect;
        _effectSwitchPresenter.AppendText(EffectSwitchText);
    }

    #endregion

    #region Card 3 — UIElement.Effect composition

    private void OnNoGpuEffectClick(object sender, RoutedEventArgs e)
    {
        _composePresenter.Effect = null;
        Replay(_composePresenter, ComposeText);
    }

    private void OnBlur4Click(object sender, RoutedEventArgs e)
    {
        _composePresenter.Effect = new Jalium.UI.Media.Effects.BlurEffect(4);
        Replay(_composePresenter, ComposeText);
    }

    private void OnBlur10Click(object sender, RoutedEventArgs e)
    {
        _composePresenter.Effect = new Jalium.UI.Media.Effects.BlurEffect(10);
        Replay(_composePresenter, ComposeText);
    }

    private static void Replay(TextEffectPresenter presenter, string text)
    {
        presenter.ClearText();
        presenter.AppendText(text);
    }

    #endregion

    #region Code samples

    private const string XamlExample = @"<!-- Jalium.UI.Controls.TextEffects.TextEffectPresenter -->
<!-- Wire up with code-behind: TextEffect defaults to RiseSettleEffect -->
<Border x:Name=""DemoHost""
        Padding=""24""
        Background=""#1E1E1E""
        CornerRadius=""8""/>
<StackPanel Orientation=""Horizontal"" Margin=""0,12,0,0"">
    <Button Content=""Append"" Click=""OnAppend""/>
    <Button Content=""Insert @3"" Click=""OnInsert"" Margin=""8,0,0,0""/>
    <Button Content=""Remove 2 @3"" Click=""OnRemove"" Margin=""8,0,0,0""/>
    <Button Content=""Clear"" Click=""OnClear"" Margin=""8,0,0,0""/>
</StackPanel>";

    private const string CSharpExample = @"using Jalium.UI.Controls.TextEffects;
using Jalium.UI.Controls.TextEffects.Effects;
using Jalium.UI.Media;
using Jalium.UI.Media.Effects;

public partial class LyricSample : Page
{
    private TextEffectPresenter _presenter = null!;

    public LyricSample()
    {
        InitializeComponent();

        // Default effect: new cells rise from below with overshoot-and-settle
        // while they come into focus; removed cells drift up and dissipate.
        _presenter = new TextEffectPresenter
        {
            Text = ""Type a lyric..."",
            TextEffect = new RiseSettleEffect(),
            FontSize = 28,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Colors.White),
        };
        DemoHost.Child = _presenter;
    }

    private void OnAppend(object s, RoutedEventArgs e)
        => _presenter.AppendText(""lyric "");

    private void OnInsert(object s, RoutedEventArgs e)
        => _presenter.InsertText(3, ""[INS]"");

    private void OnRemove(object s, RoutedEventArgs e)
        => _presenter.RemoveText(3, 2);

    private void OnClear(object s, RoutedEventArgs e)
        => _presenter.ClearText();

    // Swap drivers at will — existing cells keep their identity.
    private void UseTypewriter()
        => _presenter.TextEffect = new TypewriterEffect { CharacterDelayMs = 55 };

    // TextEffect (per-cell CPU) composes with UIElement.Effect (whole-surface GPU pass).
    private void EnableGpuBlur()
        => _presenter.Effect = new BlurEffect(6);
}";

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

    #endregion
}
