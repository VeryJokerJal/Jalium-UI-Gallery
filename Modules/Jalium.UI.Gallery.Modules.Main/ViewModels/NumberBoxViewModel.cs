namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class NumberBoxViewModel
{
    private double _basicValue = 42;
    private double _inlineValue = 10;
    private double _rangedValue = 50;
    private double _steppedValue = 25;
    public double BasicValue { get => _basicValue; set => SetProperty(ref _basicValue, value); }
    public double InlineValue { get => _inlineValue; set => SetProperty(ref _inlineValue, value); }
    public double RangedValue { get => _rangedValue; set => SetProperty(ref _rangedValue, value); }
    public double SteppedValue { get => _steppedValue; set => SetProperty(ref _steppedValue, value); }
}
