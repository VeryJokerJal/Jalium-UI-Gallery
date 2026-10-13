using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.ViewModels;

public sealed class ButtonViewModelFixture
{
    [Fact]
    public void IncrementCommand_UpdatesBindableCounterState()
    {
        var viewModel = new ButtonViewModel();
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName);

        viewModel.IncrementCommand.Execute(null);

        Assert.Equal(1, viewModel.Count);
        Assert.Equal("Count: 1", viewModel.CounterText);
        Assert.Contains(nameof(ButtonViewModel.Count), changedProperties);
        Assert.Contains(nameof(ButtonViewModel.CounterText), changedProperties);
    }
}
