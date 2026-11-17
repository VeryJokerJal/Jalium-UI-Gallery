using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Modules.Main.Localization;
using Jalium.UI.Gallery.Modules.Main.Services;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Views;

public sealed class GalleryDialogValidationFixture
{
    [Fact]
    public void ValidationDefersRejectsAnEmptyNameAndRecoversOnTheSameForm()
    {
        WithUiContext(context =>
        {
            var messages = new List<string>();
            var (dialog, form) = new GalleryContentDialogService().CreateValidationDialog(new Observer(messages.Add));
            var rejected = InvokePrimary(dialog);
            Assert.True(form.IsBusy);
            var pending = Completion(rejected);
            Assert.False(pending.IsCompleted);
            context.Complete(pending);
            Assert.True(rejected.Cancel);
            Assert.False(form.IsBusy);
            Assert.Equal(LocalizationManager.Get("Loc.Dialog.NameRequired"), form.ErrorMessage);
            Assert.Equal(LocalizationManager.Get("Loc.Dialog.NameRequired"), messages[^1]);
            form.Text = "Template workspace";
            var accepted = InvokePrimary(dialog);
            Assert.True(form.IsBusy);
            Assert.False(form.HasError);
            context.Complete(Completion(accepted));
            Assert.False(accepted.Cancel);
            Assert.False(form.IsBusy);
            Assert.False(form.HasError);
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void ProgressFailuresCannotLeaveTheFormBusyOrItsDeferralPending(int throwOnReport)
    {
        WithUiContext(context =>
        {
            var calls = 0;
            var (dialog, form) = new GalleryContentDialogService().CreateValidationDialog(new Observer(_ =>
            {
                if (++calls == throwOnReport) throw new InvalidOperationException("Observer failed");
            }));
            var args = InvokePrimary(dialog);
            context.Complete(Completion(args));
            Assert.True(args.Cancel);
            Assert.False(form.IsBusy);
            Assert.Equal(LocalizationManager.Get("Loc.Dialog.ValidationFailed"), form.ErrorMessage);
        });
    }

    private static ContentDialogButtonClickEventArgs InvokePrimary(ContentDialog dialog)
    {
        var args = new ContentDialogButtonClickEventArgs();
        typeof(ContentDialog).GetMethod("RaiseButtonClickEvent", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(dialog, new object[] { ContentDialogButton.Primary, args });
        return args;
    }
    private static Task Completion(ContentDialogDeferrableEventArgs args) =>
        (Task)typeof(ContentDialogDeferrableEventArgs).GetMethod("WaitForDeferralsAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(args, null)!;

    private static void WithUiContext(Action<UiContext> action)
    {
        var previousContext = SynchronizationContext.Current;
        var language = LocalizationManager.CurrentLanguage;
        var context = new UiContext();
        var app = new App();
        try { SynchronizationContext.SetSynchronizationContext(context); action(context); }
        finally { SynchronizationContext.SetSynchronizationContext(previousContext); LocalizationManager.SetLanguage(language); GC.KeepAlive(app); }
    }
    private sealed class Observer(Action<string> report) : IProgress<string>
    {
        public void Report(string value) => report(value);
    }
    private sealed class UiContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _queue = new();
        public override void Post(SendOrPostCallback callback, object? state) => _queue.Enqueue((callback, state));
        public void Complete(Task task)
        {
            var elapsed = Stopwatch.StartNew();
            while (!task.IsCompleted && elapsed.Elapsed < TimeSpan.FromSeconds(3))
            {
                while (_queue.TryDequeue(out var work)) work.Callback(work.State);
                if (!task.IsCompleted) Thread.Sleep(1);
            }
            Assert.True(task.IsCompletedSuccessfully, "Validation must finish its deferral on the owning UI context.");
        }
    }
}
