using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed class NavigationWindowSampleViewModel
{
    // Each journal entry owns immutable state; it is constructed with the
    // navigation action, rather than resolved as a shared DI screen model.
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(NavigationWindowSampleViewModel))]
    public NavigationWindowSampleViewModel(int number, Action nextPage)
    {
        Number = number;
        NextPageCommand = new RelayCommand(nextPage);
    }

    public int Number { get; }
    public string NumberText => Number.ToString("D2", CultureInfo.InvariantCulture);
    public ICommand NextPageCommand { get; }
}
