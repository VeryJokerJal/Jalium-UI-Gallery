using System.ComponentModel;
using Jalium.UI.Automation;
using Jalium.UI.Automation.Peers;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Controls.Themes;
using Jalium.UI.Data;
using Jalium.UI.Input;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Behaviors;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Presentation;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Themes;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class CustomControlContractsFixture
{
    public CustomControlContractsFixture() => Keyboard.Initialize();

    [Fact]
    public void DocumentDemoAndPreviewShareContentWithIndependentTemplatedPages()
    {
        RegisterDocumentDefaults();
        var model = new DocumentViewerViewModel();
        var page = new DocumentViewerPage { DataContext = model };
        Layout(page);
        var viewer = Assert.IsType<DocumentViewer>(page.FindName("DemoViewer"));
        Assert.Same(model.SampleDocument, viewer.Document);
        Assert.True(viewer.PageCount > 1);
        var panel = Assert.IsType<DocumentPagePanel>(viewer.FindName("PART_PagePanel"));
        Assert.NotEmpty(panel.Children);
        model.NextPageCommand.Execute(null);
        Assert.Equal(2, viewer.CurrentPage);
        model.SearchText = "document"; model.FindCommand.Execute(null);
        Assert.True(viewer.SearchResultCount > 12);
        model.FitPageCommand.Execute(null);
        model.PageDisplay = DocumentViewerPageDisplay.TwoUpFacing;
        Assert.Equal(model.PageDisplay, viewer.PageDisplay);
        Assert.NotNull(viewer.GetBindingExpression(DocumentViewer.PageDisplayProperty));

        Assert.False(string.IsNullOrEmpty(model.SearchResult));
        var template = Assert.IsType<DataTemplate>(GalleryControlThemes.FindPreviewTemplate("Gallery.Preview.DocumentViewerPage"));
        double originalZoom = model.Zoom;
        var preview = Assert.IsAssignableFrom<FrameworkElement>(template.LoadContent());
        preview.DataContext = model; Layout(preview); Jalium.UI.Threading.Dispatcher.CurrentDispatcher.ProcessQueue(); Layout(preview);
        var previewViewer = Descendants(preview).OfType<DocumentViewer>().Single();
        Assert.Same(model.SampleDocument, previewViewer.Document);
        Assert.Same(model.PreviewState, DocumentViewerBehavior.GetStateSink(previewViewer));
        Assert.Same(model, previewViewer.DataContext);
        Assert.Equal(originalZoom, model.Zoom);
        Assert.Equal(previewViewer.Zoom * model.SampleDocument.PageWidth / 100, previewViewer.DisplayPageSize.Width, 5);
        Assert.NotSame(viewer.PageViews[0], previewViewer.PageViews[0]);
        Assert.Same(panel, viewer.PageViews[0].VisualParent);
    }

    [Fact]
    public void DocumentRebindingPublishesPageCountWithoutAnotherNavigation()
    {
        RegisterDocumentDefaults();
        var first = new DocumentViewerViewModel();
        var page = new DocumentViewerPage { DataContext = first }; Layout(page);
        var viewer = Assert.IsType<DocumentViewer>(page.FindName("DemoViewer"));
        var replacement = new DocumentViewerViewModel();
        page.DataContext = replacement; Layout(page);
        Assert.True(viewer.PageCount > 1);
        Jalium.UI.Threading.Dispatcher.CurrentDispatcher.ProcessQueue(); Layout(page);
        Assert.Equal($"Page {viewer.CurrentPage} / {viewer.PageCount}", replacement.PageInfo);
        Assert.Equal(100, replacement.Zoom);
        Assert.Equal(100, viewer.Zoom);
        int current = viewer.CurrentPage;
        first.NextPageCommand.Execute(null);
        Assert.Equal(current, viewer.CurrentPage);
    }

    private static void RegisterDocumentDefaults()
    {
        using var stream = new MemoryStream();
        var defaults = ThemeManager.XamlLoader!(stream, "Themes/Generic.jalxaml", typeof(Control).Assembly)!;
        ThemeManager.RegisterControlThemes(new ResourceDictionary { [typeof(DocumentViewer)] = defaults[typeof(DocumentViewer)], [typeof(ScrollViewer)] = defaults[typeof(ScrollViewer)] });
    }

    [Fact]
    public void DocumentToolbarTracksActualPageBoundariesSearchAndEmptyRestoration()
    {
        RegisterDocumentDefaults();
        var model = new DocumentViewerViewModel();
        var page = new DocumentViewerPage { DataContext = model }; Layout(page);
        var viewer = Assert.IsType<DocumentViewer>(page.FindName("DemoViewer"));
        Button Action(System.Windows.Input.ICommand command) => Descendants(page).OfType<Button>().Single(button => ReferenceEquals(button.Command, command));
        Assert.False(Action(model.FirstPageCommand).IsEnabled);
        Assert.False(Action(model.PreviousPageCommand).IsEnabled);
        Assert.True(Action(model.NextPageCommand).IsEnabled);
        Assert.False(Action(model.FindCommand).IsEnabled);
        Assert.False(Action(model.FindNextCommand).IsEnabled);
        Assert.False(Action(model.ClearSearchCommand).IsEnabled);
        var request = model.ViewerRequest;
        model.PreviousPageCommand.Execute(null);
        Assert.Same(request, model.ViewerRequest);
        Action(model.LastPageCommand).PerformClick();
        Assert.Equal(viewer.PageCount, viewer.CurrentPage);
        Assert.False(Action(model.NextPageCommand).IsEnabled);
        Assert.False(Action(model.LastPageCommand).IsEnabled);
        Assert.True(Action(model.FirstPageCommand).IsEnabled);
        Action(model.FirstPageCommand).PerformClick();
        model.SearchText = "document";
        Assert.True(Action(model.FindCommand).IsEnabled);
        Action(model.FindCommand).PerformClick();
        Assert.True(Action(model.FindNextCommand).IsEnabled);
        Assert.True(Action(model.ClearSearchCommand).IsEnabled);
        Action(model.ClearSearchCommand).PerformClick();
        Assert.False(Action(model.FindNextCommand).IsEnabled);
        model.SearchText = "missing-original-phrase";
        Action(model.FindCommand).PerformClick();
        Assert.False(Action(model.FindNextCommand).IsEnabled);
        Assert.True(Action(model.ClearSearchCommand).IsEnabled);
        viewer.SetCurrentValue(DocumentViewer.DocumentProperty, null);
        foreach (var command in new[] { model.FirstPageCommand, model.NextPageCommand, model.ZoomInCommand, model.FitPageCommand, model.PrintCommand, model.FindCommand, model.ClearSearchCommand })
        {
            Assert.False(command.CanExecute(null));
            Assert.False(Action(command).IsEnabled);
        }
        viewer.SetCurrentValue(DocumentViewer.DocumentProperty, model.SampleDocument);
        Assert.True(Action(model.NextPageCommand).IsEnabled);
        Assert.True(Action(model.PrintCommand).IsEnabled);
        Assert.False(Action(model.FindNextCommand).IsEnabled);
    }

    [Fact]
    public void DocumentZoomAvailabilityUsesTheSampleViewerBoundsAndPreviewStaysIndependent()
    {
        RegisterDocumentDefaults();
        var model = new DocumentViewerViewModel();
        var page = new DocumentViewerPage { DataContext = model }; Layout(page);
        var viewer = Assert.IsType<DocumentViewer>(page.FindName("DemoViewer"));
        model.Zoom = viewer.MinZoom;
        Assert.False(model.ZoomOutCommand.CanExecute(null));
        Assert.True(model.ZoomInCommand.CanExecute(null));
        model.Zoom = viewer.MaxZoom;
        Assert.False(model.ZoomInCommand.CanExecute(null));
        Assert.True(model.ZoomOutCommand.CanExecute(null));
        model.PreviewState.UpdatePageState(1, 4);
        model.PreviewState.Zoom = viewer.MinZoom;
        Assert.True(model.PreviewState.NextPageCommand.CanExecute(null));
        Assert.False(model.PreviewState.PreviousPageCommand.CanExecute(null));
        Assert.False(model.PreviewState.ZoomOutCommand.CanExecute(null));
        Assert.False(model.ZoomInCommand.CanExecute(null));
        Assert.Equal(viewer.MaxZoom, model.Zoom);
    }

    private static IEnumerable<Visual> Descendants(Visual root)
    {
        yield return root;
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
            if (VisualTreeHelper.GetChild(root, index) is Visual child)
                foreach (var descendant in Descendants(child)) yield return descendant;
    }

    [Fact]
    public void NumericStepperTakesFocusFromMouseInputAndRoutesArrowKeysToItsValue()
    {
        var previous = new Button();
        var stepper = new GalleryInspectorNumericStepper { Value = 10, Step = 2 };
        var host = new StackPanel();
        host.Children.Add(previous);
        host.Children.Add(stepper);
        Layout(host);
        previous.Focus();
        var button = Assert.IsAssignableFrom<ButtonBase>(stepper.FindName("PART_IncreaseButton"));
        button.RaiseEvent(new MouseButtonEventArgs(UIElement.MouseDownEvent, new Point(10, 10),
            MouseButton.Left, MouseButtonState.Pressed, 1, MouseButtonState.Pressed,
            MouseButtonState.Released, MouseButtonState.Released, MouseButtonState.Released,
            MouseButtonState.Released, ModifierKeys.None, 1));
        Assert.Same(stepper, Keyboard.FocusedElement);
        stepper.RaiseEvent(new KeyEventArgs(UIElement.KeyDownEvent, Key.Up, ModifierKeys.None, false, true, 1));
        Assert.Equal(12, stepper.Value);
        Assert.Equal("Focused", VisualStateManager.GetCurrentStateName(stepper, "FocusStates"));
        previous.Focus();
        Assert.Equal("Unfocused", VisualStateManager.GetCurrentStateName(stepper, "FocusStates"));
    }

    [Fact]
    public void NumericStepperPreservesBindingAndReleasesOldButtonSubscriptions()
    {
        var model = new NumericModel { Value = 10 };
        var stepper = new GalleryInspectorNumericStepper { Label = "Padding", Step = 5 };
        stepper.SetBinding(RangeBase.ValueProperty, new Binding(nameof(NumericModel.Value)) { Source = model, Mode = BindingMode.TwoWay });
        Layout(stepper);
        var oldButton = Assert.IsAssignableFrom<ButtonBase>(stepper.FindName("PART_IncreaseButton"));
        Click(oldButton);
        Assert.Equal(15, stepper.Value);
        Assert.Equal(15, model.Value);
        Assert.NotNull(stepper.GetBindingExpression(RangeBase.ValueProperty));

        var replacement = new ControlTemplate(typeof(GalleryInspectorNumericStepper));
        replacement.SetVisualTree(() => new Button { Name = "PART_IncreaseButton" });
        stepper.Template = replacement;
        Click(oldButton);
        Assert.Equal(15, model.Value);
        Click(Assert.IsType<Button>(stepper.FindName("PART_IncreaseButton")));
        Assert.Equal(20, model.Value);
        stepper.Template = null;
        Click(oldButton);
        Assert.Equal(20, model.Value);
    }

    [Fact]
    public void NumericStepperExposesRangeAutomationAndAcceptsMissingOptionalParts()
    {
        var stepper = new GalleryInspectorNumericStepper { Label = "Padding", Minimum = 0, Maximum = 20, Value = 20 };
        var template = new ControlTemplate(typeof(GalleryInspectorNumericStepper));
        template.SetVisualTree(() => new Border());
        stepper.Template = template;
        Assert.False(stepper.CanIncrease);
        Assert.True(stepper.CanDecrease);
        var peer = FrameworkElementAutomationPeer.CreatePeerForElement(stepper)!;
        Assert.Equal(AutomationControlType.Spinner, peer.GetAutomationControlType());
        Assert.Equal("Padding", peer.GetName());
        var range = Assert.IsAssignableFrom<Jalium.UI.Automation.Provider.IRangeValueProvider>(peer.GetPattern(PatternInterface.RangeValue));
        range.SetValue(15);
        Assert.Equal(15, stepper.Value);
    }

    [Fact]
    public void ColorChannelsPreserveAlphaAndTheOwningTwoWayBinding()
    {
        var model = new ColorModel { Color = Color.FromArgb(77, 10, 20, 30) };
        var editor = new GalleryInspectorColorEditor();
        editor.SetBinding(GalleryInspectorColorEditor.ColorProperty,
            new Binding(nameof(ColorModel.Color)) { Source = model, Mode = BindingMode.TwoWay });
        editor.Red = 123;
        Assert.Equal(Color.FromArgb(77, 123, 20, 30), model.Color);
        var current = editor.Color;
        editor.PaletteKind = "Surface";
        Assert.Equal(current, editor.Color);
        editor.SelectedSwatch = editor.Swatches[0];
        Assert.Equal(editor.Swatches[0].Color, model.Color);
        Assert.NotNull(editor.GetBindingExpression(GalleryInspectorColorEditor.ColorProperty));
    }

    [Fact]
    public void HorizontalPaletteArrowKeysUpdateTheBindingAndRetiredPalettesCannotWriteBack()
    {
        var model = new ColorModel();
        var editor = new GalleryInspectorColorEditor();
        editor.SetBinding(GalleryInspectorColorEditor.ColorProperty,
            new Binding(nameof(ColorModel.Color)) { Source = model, Mode = BindingMode.TwoWay });
        Layout(editor);
        var palette = Assert.IsType<ListBox>(editor.FindName("PART_Palette"));
        palette.SelectedIndex = 0;
        var swatch = Assert.IsType<ListBoxItem>(palette.ItemContainerGenerator.ContainerFromIndex(0));
        Assert.True(swatch.Focusable);
        Assert.True(swatch.Focus());
        Assert.True(palette.IsKeyboardFocusWithin);
        palette.RaiseEvent(new KeyEventArgs(UIElement.KeyDownEvent, Key.Right, ModifierKeys.None, true, false, 1));
        Assert.Equal(editor.Swatches[1].Color, model.Color);
        Assert.Equal(1, palette.SelectedIndex);
        var original = editor.Template;
        editor.Template = null;
        palette.SelectedIndex = 3;
        Assert.Equal(editor.Swatches[1].Color, model.Color);
        editor.Template = original;
        Layout(editor);
        Assert.Equal(1, Assert.IsType<ListBox>(editor.FindName("PART_Palette")).SelectedIndex);
    }

    [Fact]
    public void SectionsPreserveHeaderTypographyContentWidthAndDescriptionStates()
    {
        var surface = new GalleryDemoSurface { Content = new Border { Height = 40 } };
        var section = new GallerySection { Header = "Example", Description = "Description", Content = surface };
        Layout(section);
        var presenter = Assert.IsType<ContentPresenter>(section.FindName("PART_Header"));
        var header = Assert.IsType<TextBlock>(VisualTreeHelper.GetChild(presenter, 0));
        Assert.Equal("Example", header.Text);
        Assert.Equal(17, header.FontSize);
        Assert.Equal(458, surface.ActualWidth, 3);
        var description = Assert.IsType<TextBlock>(section.FindName("PART_Description"));
        section.Description = " ";
        Assert.Equal(Visibility.Collapsed, description.Visibility);
        section.Description = "Restored";
        Assert.Equal(Visibility.Visible, description.Visibility);
    }

    [Fact]
    public void PreviewTemplatesCanBeReplacedWithoutExtractingAPageVisual()
    {
        var template = new DataTemplate();
        template.SetVisualTree(() => new Border { Background = Brushes.Purple });
        var preview = new HomeComponentPreview { Item = HomeCatalog.Items[0], PreviewTemplate = template };
        Layout(preview);
        var presenter = Assert.IsType<ContentPresenter>(preview.FindName("PART_PreviewPresenter"));
        Assert.Same(template, preview.ContentTemplate);
        Assert.IsType<Border>(VisualTreeHelper.GetChild(presenter, 0));
        Assert.False(preview.Content is FrameworkElement);
    }

    [Fact]
    public void ScrollPreviewReleasesOldPartsAndReconnectsOnceToAReplacementTemplate()
    {
        var page = new ScrollViewerPage();
        Layout(page);
        var original = page.Template;
        var oldButton = Assert.IsType<Button>(page.FindName("ScrollRightButton"));
        var oldViewer = Assert.IsType<ScrollViewer>(page.FindName("HorizontalPreviewViewer"));
        page.Template = null;
        Click(oldButton);
        Assert.Equal(0, oldViewer.HorizontalOffset);
        page.Template = original;
        Layout(page);
        var viewer = Assert.IsType<ScrollViewer>(page.FindName("HorizontalPreviewViewer"));
        Click(Assert.IsType<Button>(page.FindName("ScrollRightButton")));
        Assert.Equal(viewer.ViewportWidth, viewer.HorizontalOffset, 3);
    }

    private static void Click(ButtonBase button) => button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
    private static void Layout(FrameworkElement control)
    {
        control.Measure(new Size(500, 3200));
        control.Arrange(new Rect(0, 0, 500, 3200));
        control.UpdateLayout();
    }
    private sealed class NumericModel : INotifyPropertyChanged
    {
        private double _value;
        public double Value { get => _value; set { _value = value; PropertyChanged?.Invoke(this, new(nameof(Value))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    private sealed class ColorModel : INotifyPropertyChanged
    {
        private Color _color;
        public Color Color { get => _color; set { _color = value; PropertyChanged?.Invoke(this, new(nameof(Color))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
