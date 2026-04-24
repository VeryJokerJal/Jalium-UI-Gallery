using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Services.Interfaces;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>
/// Primary view model for the Main module. Exposes a bindable property and a
/// command that refreshes it from the injected <see cref="IMessageService"/>.
/// </summary>
public class ViewAViewModel : RegionViewModelBase
{
    private readonly IMessageService _messageService;
    private string _message;

    public ViewAViewModel(IMessageService messageService)
    {
        ArgumentNullException.ThrowIfNull(messageService);
        _messageService = messageService;
        _message = messageService.GetMessage();
        RefreshCommand = new RelayCommand(Refresh);
    }

    /// <summary>Latest message shown on the view.</summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>Pulls a fresh greeting from the message service.</summary>
    public ICommand RefreshCommand { get; }

    private void Refresh() => Message = _messageService.GetMessage();

    private sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
