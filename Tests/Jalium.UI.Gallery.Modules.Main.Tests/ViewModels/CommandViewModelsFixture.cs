using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Input;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.ViewModels;

public sealed class CommandViewModelsFixture
{
    [Fact]
    public void XamlUiCommandPage_UsesFrameworkCommandsAndDelegatesExecution()
    {
        var viewModel = new XamlUICommandViewModel();

        var favorite = Assert.IsType<XamlUICommand>(
            viewModel.AddToFavoritesCommand);
        Assert.Equal("Add to Favorites", favorite.Label);
        Assert.Equal("F", favorite.AccessKey);
        Assert.IsType<string>(favorite.IconSource);
        var accelerator = Assert.IsType<KeyGesture>(
            Assert.Single(favorite.KeyboardAccelerators));
        Assert.Equal(Key.D, accelerator.Key);
        Assert.Equal(ModifierKeys.Control, accelerator.Modifiers);

        favorite.Execute(null);

        Assert.Equal(
            "Add to Favorites executed.",
            viewModel.CommandOutputText);
        Assert.Contains("Add to Favorites", viewModel.CommandHistoryText);
        Assert.Contains("XamlUICommand", viewModel.XamlExample);
        Assert.DoesNotContain("GalleryCommandViewModel", viewModel.CSharpExample);
        Assert.DoesNotContain("SymbolIconSource", viewModel.XamlExample);
        Assert.DoesNotContain("KeyboardAccelerator ", viewModel.XamlExample);
    }

    [Fact]
    public void XamlUiCommandPage_PropagatesCanExecuteChanges()
    {
        var viewModel = new XamlUICommandViewModel();
        var command = viewModel.ExportAsPdfCommand;
        var changeCount = 0;
        command.CanExecuteChanged += (_, _) => changeCount++;

        Assert.True(command.CanExecute(null));

        viewModel.CanExportAsPdf = false;

        Assert.False(command.CanExecute(null));
        Assert.Equal(1, changeCount);

        viewModel.CanExportAsPdf = true;

        Assert.True(command.CanExecute(null));
        Assert.Equal(2, changeCount);
    }

    [Fact]
    public void XamlUiCommandPage_RegistersWorkingInputBindings()
    {
        var viewModel = new XamlUICommandViewModel();
        var page = new XamlUICommandPage
        {
            DataContext = viewModel
        };

        Assert.Equal(3, page.InputBindings.Count);
        var favoriteBinding = Assert.IsType<KeyBinding>(
            page.InputBindings[0]);
        Assert.Equal(Key.D, favoriteBinding.Key);
        Assert.Equal(ModifierKeys.Control, favoriteBinding.Modifiers);
        Assert.Same(
            viewModel.AddToFavoritesCommand,
            favoriteBinding.Command);
        Assert.Same(
            viewModel.ShareViaLinkCommand,
            page.InputBindings[1].Command);
        Assert.Same(
            viewModel.ExportAsPdfCommand,
            page.InputBindings[2].Command);

        page.DataContext = null;
        Assert.Empty(page.InputBindings);

        page.DataContext = viewModel;
        Assert.Equal(3, page.InputBindings.Count);
    }

    [Fact]
    public void StandardUiCommandPage_UsesRealStandardCommands()
    {
        var viewModel = new StandardUICommandViewModel();

        Assert.Equal(8, viewModel.Commands.Count);
        Assert.Collection(
            viewModel.Commands,
            item => Assert.Equal(StandardUICommandKind.Cut, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Copy, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Paste, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Delete, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Undo, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Redo, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.SelectAll, item.Command.Kind),
            item => Assert.Equal(StandardUICommandKind.Save, item.Command.Kind));

        var cut = viewModel.Commands[0];
        Assert.Equal("Cut", cut.Label);
        Assert.Equal(OperatingSystem.IsMacOS() ? "⌘X" : "Ctrl+X", cut.KeyboardShortcut);
        Assert.False(string.IsNullOrWhiteSpace(cut.IconGlyph));
        Assert.False(string.IsNullOrWhiteSpace(cut.Description));

        cut.Command.Execute(cut.Label);

        Assert.Equal("Executed: Cut", viewModel.CommandStatus);
        Assert.DoesNotContain(
            "x:Static StandardUICommand.",
            viewModel.XamlExample);
    }
}
