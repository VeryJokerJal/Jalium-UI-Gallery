using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;
public sealed partial class ContextMenuViewModel
{
    private string _selectedAction = string.Empty;
    public ContextMenuViewModel()
    {
        SelectActionCommand = new RelayCommand<string>(action =>
        {
            if (!string.IsNullOrWhiteSpace(action))
                SelectedAction = action;
        });
    }

    public ICommand SelectActionCommand { get; }
    public bool HasSelection => !string.IsNullOrEmpty(SelectedAction);
    public bool IsSelectionEmpty => !HasSelection;

    public string SelectedAction
    {
        get => _selectedAction;
        private set
        {
            if (SetProperty(ref _selectedAction, value))
            {
                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(IsSelectionEmpty));
            }
        }
    }
}
