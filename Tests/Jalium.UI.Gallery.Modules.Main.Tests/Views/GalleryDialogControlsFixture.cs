using System.ComponentModel;
using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Controls.Primitives;
using Jalium.UI.Data;
using Jalium.UI.Gallery.Modules.Main.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.Services;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Media;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryDialogControlsFixture
{
    [Fact]
    public void FormAndPreviewConstructModelsWithoutAVisualTreeAndOptionalPartsAreSafe()
    {
        var form = new GalleryDialogForm { Text = "name", ErrorMessage = "Required", IsBusy = true };
        var preview = new GalleryDialogPreview { Description = "Review", PreviewText = "Preview" };
        Assert.Equal(0, VisualTreeHelper.GetChildrenCount(form));
        Assert.Equal(0, VisualTreeHelper.GetChildrenCount(preview));
        Assert.True(form.HasError);
        var template = new ControlTemplate(typeof(GalleryDialogForm));
        template.SetVisualTree(() => new Grid());
        form.Template = template;
        Layout(form, 280, 200);
        Assert.False(form.FocusInput());
        form.ErrorMessage = "";
        form.IsBusy = false;
        Assert.False(form.HasError);
    }

    [Theory]
    [InlineData(280)]
    [InlineData(600)]
    public void FormBindingsBusyAndValidationStatesUseTheSameTemplateParts(double width)
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        try
        {
            var model = new TextModel { Text = "draft" };
            var form = new GalleryDialogForm { Label = "Document", Description = "Choose a name." };
            BindingOperations.SetBinding(form, GalleryDialogForm.TextProperty, new Binding(nameof(TextModel.Text))
            { Source = model, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            Layout(form, width, 300);
            var input = Assert.IsType<TextBox>(form.FindName("PART_TextBox"));
            var error = Assert.IsType<TextBlock>(form.FindName("PART_Error"));
            var progress = Assert.IsType<ProgressBar>(form.FindName("PART_Progress"));
            Assert.Equal("draft", input.Text);
            Assert.Equal(Visibility.Collapsed, error.Visibility);
            Assert.Equal(Visibility.Collapsed, progress.Visibility);
            input.SetCurrentValue(TextBox.TextProperty, "edited");
            Assert.Equal("edited", form.Text);
            Assert.Equal("edited", model.Text);
            form.IsBusy = true;
            Assert.False(input.IsEnabled);
            Assert.Equal(Visibility.Visible, progress.Visibility);
            Assert.True(progress.IsIndeterminate);
            form.ErrorMessage = "A name is required. Please enter a name to continue.";
            Assert.True(form.HasError);
            Assert.Equal(Visibility.Visible, error.Visibility);
            Assert.Equal(TextWrapping.Wrap, error.TextWrapping);
            form.IsBusy = false;
            Assert.True(input.IsEnabled);
            Assert.Equal(Visibility.Collapsed, progress.Visibility);
            form.ErrorMessage = "";
            Assert.Equal(Visibility.Collapsed, error.Visibility);
            Assert.Same(input, form.FindName("PART_TextBox"));
            Layout(form, width, 300);
            Assert.InRange(input.ActualWidth, 1, width);
        }
        finally { LocalizationManager.SetLanguage(language); GC.KeepAlive(app); }
    }

    [Fact]
    public void RetiredFormInputsCannotWriteBackToTheCurrentForm()
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        try
        {
            var form = new GalleryDialogForm { Text = "current" };
            Layout(form, 280, 200);
            var oldInput = Assert.IsType<TextBox>(form.FindName("PART_TextBox"));
            var template = form.Template;
            form.Template = null;
            oldInput.SetCurrentValue(TextBox.TextProperty, "retired");
            Assert.Equal("current", form.Text);
            form.Template = template;
            Layout(form, 280, 200);
            Assert.NotSame(oldInput, form.FindName("PART_TextBox"));
            Assert.Equal("current", Assert.IsType<TextBox>(form.FindName("PART_TextBox")).Text);
        }
        finally { LocalizationManager.SetLanguage(language); GC.KeepAlive(app); }
    }

    [Theory]
    [InlineData(GalleryLanguage.English)]
    [InlineData(GalleryLanguage.ChineseSimplified)]
    [InlineData(GalleryLanguage.Japanese)]
    [InlineData(GalleryLanguage.Korean)]
    public void FactoriesProvideLocalizedContentControlsWithNoFixedEditorWidth(GalleryLanguage language)
    {
        var previous = LocalizationManager.CurrentLanguage;
        var app = new App();
        try
        {
            LocalizationManager.SetLanguage(language);
            var service = new GalleryContentDialogService();
            var (rename, form) = service.CreateRenameDialog();
            var window = new DisplayedWindow { Width = 390, Height = 844 };
            app.MainWindow = window;
            try
            {
                Assert.Same(form, rename.Content);
                Assert.Equal(LocalizationManager.Get("Loc.Dialog.RenameTitle"), rename.Title);
                _ = rename.ShowAsync();
                Layout(window, 390, 844);
                Assert.Equal(LocalizationManager.Get("Loc.Dialog.DocumentName"), form.Label);
                var input = Assert.IsType<TextBox>(form.FindName("PART_TextBox"));
                Assert.False(input.HasLocalValue(FrameworkElement.WidthProperty));
                Assert.InRange(input.ActualWidth, 1, 340);
                rename.Hide();
                var review = service.CreateFullSizeDialog();
                Assert.True(review.FullSizeDesired);
                Assert.IsType<GalleryDialogPreview>(review.Content);
                var validation = service.CreateValidationDialog(new Progress<string>());
                Assert.IsType<GalleryDialogForm>(validation.Dialog.Content);
                Assert.Equal("", validation.Form.Text);
                Assert.Equal(LocalizationManager.Get("Loc.Dialog.WorkspaceName"), validation.Form.Label);
            }
            finally { window.Close(); }
        }
        finally { LocalizationManager.SetLanguage(previous); GC.KeepAlive(app); }
    }

    [Fact]
    public void InlineDialogKeepsDetachedRequestsLocalAndPublishesResultsWithoutAViewModelInterface()
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var dialog = new GalleryInlineDialog { Title = "Review", CloseButtonText = "Close" };
        var model = new RequestModel();
        BindingOperations.SetBinding(dialog, GalleryInlineDialog.RequestProperty, new Binding(nameof(RequestModel.Request)) { Source = model });
        BindingOperations.SetBinding(dialog, GalleryInlineDialog.ResultProperty, new Binding(nameof(RequestModel.Result))
        { Source = model, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        model.Request = new ContentDialogRequest(1);
        Assert.False(dialog.IsShowing);
        var host = new Grid();
        host.Children.Add(dialog);
        var window = new DisplayedWindow { Width = 600, Height = 500, Content = host };
        app.MainWindow = window;
        try
        {
            Layout(window, 600, 500);
            Assert.Same(host, dialog.VisualParent);
            Assert.Same(window, Window.GetWindow(dialog));
            Assert.Equal(Visibility.Visible, window.Visibility);
            Assert.Same(model.Request, dialog.Request);
            SetLoaded(dialog, true);
            Assert.True(dialog.IsShowing);
            Layout(window, 600, 500);
            var oldClose = Assert.IsType<Button>(dialog.FindName("PART_CloseButton"));
            var template = dialog.Template;
            dialog.Template = null;
            oldClose.PerformClick();
            Assert.True(dialog.IsShowing);
            dialog.Template = template;
            Layout(window, 600, 500);
            Assert.NotSame(oldClose, dialog.FindName("PART_CloseButton"));
            model.Request = new ContentDialogRequest(2);
            Assert.True(dialog.IsShowing);
            dialog.Hide();
            Assert.False(dialog.IsShowing);
            Assert.Equal(ContentDialogResult.None, model.Result);
            SetLoaded(dialog, true);
            Assert.False(dialog.IsShowing);
            model.Request = new ContentDialogRequest(3);
            Assert.True(dialog.IsShowing);
            SetLoaded(dialog, false);
            Assert.False(dialog.IsShowing);
            host.Children.Remove(dialog);
            model.Request = new ContentDialogRequest(4);
            Assert.False(dialog.IsShowing);
        }
        finally
        {
            dialog.Hide(); window.Close(); LocalizationManager.SetLanguage(language); GC.KeepAlive(app);
        }
    }

    [Fact]
    public void InlinePageKeepsItsCardInsideTheHostAndAccentTextMatchesTheButton()
    {
        var language = LocalizationManager.CurrentLanguage;
        var app = new App();
        var model = new ContentDialogViewModel(new GalleryContentDialogService());
        var page = new ContentDialogPage { DataContext = model };
        var workbench = new GalleryDemoWorkbench { DataContext = new GalleryDemoWorkbenchViewModel(new Jalium.UI.Gallery.Modules.Main.Themes.GalleryThemeService()) };
        workbench.DemoContent = page;
        var window = new DisplayedWindow { Width = 390, Height = 844, Content = workbench };
        app.MainWindow = window;
        try
        {
            Layout(window, 390, 844);
            SetLoaded(workbench, true);
            model.ShowInlineDialogCommand.Execute(null);
            Layout(window, 390, 844);
            var dialog = Assert.IsType<GalleryInlineDialog>(page.FindName("InlineContentDialog"));
            var host = Assert.IsType<Grid>(page.FindName("InlineDialogHost"));
            var card = Assert.IsType<Border>(dialog.FindName("PART_DialogCard"));
            Assert.True(dialog.IsShowing);
            Assert.InRange(host.ActualWidth, 1, 390);
            Assert.InRange(card.ActualWidth, 1, host.ActualWidth - 48 + 1);
            Assert.InRange(card.ActualHeight, 1, host.ActualHeight - 48 + 1);
            var button = Assert.IsType<Button>(dialog.FindName("PART_PrimaryButton"));
            var foreground = Assert.IsType<SolidColorBrush>(button.Foreground).Color;
            Assert.Equal(Color.White, foreground);
            var label = Assert.IsType<TextBlock>(FindText(button));
            Assert.Equal(foreground, Assert.IsType<SolidColorBrush>(label.Foreground).Color);
            dialog.Hide();
            Assert.Equal(ContentDialogResult.None, model.InlineDialogResult);
        }
        finally { SetLoaded(workbench, false); window.Close(); LocalizationManager.SetLanguage(language); GC.KeepAlive(app); }
    }

    private static TextBlock? FindText(DependencyObject root)
    {
        if (root is TextBlock text) return text;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); ++i)
            if (VisualTreeHelper.GetChild(root, i) is { } child && FindText(child) is { } found) return found;
        return null;
    }

    private static void SetLoaded(FrameworkElement element, bool loaded) =>
        typeof(FrameworkElement).GetMethod("SetLoadedState", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(element, new object[] { loaded });

    private static void Layout(FrameworkElement element, double width, double height)
    {
        element.Measure(new Size(width, height)); element.Arrange(new Rect(0, 0, width, height)); element.UpdateLayout();
    }
    private sealed class DisplayedWindow : Window
    {
        public DisplayedWindow()
        {
            if (OperatingSystem.IsMacOS()) typeof(Window).GetMethod("SetMacOSVisibilityForDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, new object[] { Visibility.Visible });
        }
    }
    private sealed class TextModel : INotifyPropertyChanged
    {
        private string _text = "";
        public string Text { get => _text; set { _text = value; PropertyChanged?.Invoke(this, new(nameof(Text))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    private sealed class RequestModel : INotifyPropertyChanged
    {
        private ContentDialogRequest? _request;
        private ContentDialogResult? _result;
        public ContentDialogRequest? Request { get => _request; set { _request = value; PropertyChanged?.Invoke(this, new(nameof(Request))); } }
        public ContentDialogResult? Result { get => _result; set { _result = value; PropertyChanged?.Invoke(this, new(nameof(Result))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
