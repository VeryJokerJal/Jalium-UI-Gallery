using Jalium.UI.Media;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

public sealed partial class CalendarViewModel
{
    private DateTime? _selectedDate;

    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                OnPropertyChanged(nameof(SelectedDateText));
            }
        }
    }

    public string SelectedDateText => SelectedDate?.ToString("yyyy-MM-dd") ?? "None";
}

public sealed partial class DatePickerViewModel
{
    private DateTime? _selectedDate;

    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                OnPropertyChanged(nameof(SelectedDateText));
            }
        }
    }

    public string SelectedDateText =>
        SelectedDate is { } date ? $"Selected: {date:D}" : "No date selected";
}

public sealed partial class TimePickerViewModel
{
    private TimeSpan? _selectedTime;

    public TimeSpan? SelectedTime
    {
        get => _selectedTime;
        set
        {
            if (SetProperty(ref _selectedTime, value))
            {
                OnPropertyChanged(nameof(SelectedTimeText));
            }
        }
    }

    public string SelectedTimeText =>
        SelectedTime is { } time ? $"Selected: {time:hh\\:mm}" : "No time selected";
}

public sealed partial class ColorPickerViewModel
{
    private Color _selectedColor = Color.FromRgb(0xFF, 0xD6, 0x0A);

    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            if (!SetProperty(ref _selectedColor, value))
            {
                return;
            }

            OnPropertyChanged(nameof(SelectedColorBrush));
            OnPropertyChanged(nameof(SelectedColorText));
        }
    }

    public SolidColorBrush SelectedColorBrush => new(SelectedColor);

    public string SelectedColorText =>
        $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
}

public sealed partial class SliderViewModel
{
    private double _value = 50;

    public double Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                OnPropertyChanged(nameof(ValueText));
            }
        }
    }

    public string ValueText => ((int)Value).ToString();
}

public sealed partial class TextBoxViewModel
{
    private string _text = string.Empty;

    public string Text
    {
        get => _text;
        set
        {
            if (SetProperty(ref _text, value))
            {
                OnPropertyChanged(nameof(CharacterCountText));
            }
        }
    }

    public string CharacterCountText => $"Characters: {Text.Length}";
}

public sealed partial class RangeSliderViewModel
{
    private double _rangeStart = 20;
    private double _rangeEnd = 80;

    public double RangeStart
    {
        get => _rangeStart;
        set
        {
            if (SetProperty(ref _rangeStart, value))
            {
                OnPropertyChanged(nameof(RangeStartText));
            }
        }
    }

    public double RangeEnd
    {
        get => _rangeEnd;
        set
        {
            if (SetProperty(ref _rangeEnd, value))
            {
                OnPropertyChanged(nameof(RangeEndText));
            }
        }
    }

    public string RangeStartText => ((int)RangeStart).ToString();

    public string RangeEndText => ((int)RangeEnd).ToString();
}

public sealed partial class ProgressBarViewModel
{
    private double _progress = 45;

    public double Progress
    {
        get => _progress;
        set
        {
            if (SetProperty(ref _progress, value))
            {
                OnPropertyChanged(nameof(ProgressText));
            }
        }
    }

    public string ProgressText => $"{(int)Progress}%";
}

public sealed partial class RadioButtonViewModel
{
    private string _selectedColor = "Blue";

    public bool IsRedSelected
    {
        get => SelectedColor == "Red";
        set
        {
            if (value)
            {
                SelectedColor = "Red";
            }
        }
    }

    public bool IsGreenSelected
    {
        get => SelectedColor == "Green";
        set
        {
            if (value)
            {
                SelectedColor = "Green";
            }
        }
    }

    public bool IsBlueSelected
    {
        get => SelectedColor == "Blue";
        set
        {
            if (value)
            {
                SelectedColor = "Blue";
            }
        }
    }

    public string SelectedColor
    {
        get => _selectedColor;
        private set
        {
            if (!SetProperty(ref _selectedColor, value))
            {
                return;
            }

            OnPropertyChanged(nameof(IsRedSelected));
            OnPropertyChanged(nameof(IsGreenSelected));
            OnPropertyChanged(nameof(IsBlueSelected));
            OnPropertyChanged(nameof(SelectionText));
        }
    }

    public string SelectionText => $"Selected: {SelectedColor}";
}

public sealed partial class CheckBoxViewModel
{
    private bool _isChecked;
    private bool? _threeStateValue;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value))
            {
                OnPropertyChanged(nameof(CheckBoxStatus));
            }
        }
    }

    public bool? ThreeStateValue
    {
        get => _threeStateValue;
        set
        {
            if (SetProperty(ref _threeStateValue, value))
            {
                OnPropertyChanged(nameof(ThreeStateStatus));
            }
        }
    }

    public string CheckBoxStatus => IsChecked ? "Status: Checked" : "Status: Unchecked";

    public string ThreeStateStatus => ThreeStateValue switch
    {
        true => "Status: Checked",
        false => "Status: Unchecked",
        null => "Status: Indeterminate"
    };
}

public sealed partial class ListBoxViewModel
{
    private string? _selectedItem;

    public IReadOnlyList<string> Items { get; } =
        ["Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape"];

    public string? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnPropertyChanged(nameof(SelectedItemText));
            }
        }
    }

    public string SelectedItemText => SelectedItem ?? "(none)";
}

public sealed partial class DataGridViewModel
{
    private PersonData? _selectedPerson;

    public IReadOnlyList<PersonData> People { get; } =
    [
        new("Alice", "Smith", 28, "Engineering"),
        new("Bob", "Johnson", 35, "Marketing"),
        new("Charlie", "Brown", 42, "Sales"),
        new("Diana", "Williams", 31, "Engineering"),
        new("Edward", "Jones", 29, "HR"),
        new("Fiona", "Davis", 38, "Finance"),
        new("George", "Miller", 45, "Operations"),
        new("Hannah", "Wilson", 26, "Engineering")
    ];

    public PersonData? SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            if (SetProperty(ref _selectedPerson, value))
            {
                OnPropertyChanged(nameof(SelectedPersonText));
            }
        }
    }

    public string SelectedPersonText =>
        SelectedPerson is { } person
            ? $"{person.FirstName} {person.LastName}"
            : "(none)";
}

public sealed record PersonData(
    string FirstName,
    string LastName,
    int Age,
    string Department);

public sealed partial class AutoCompleteBoxViewModel
{
    private string? _selectedLanguage;

    public IReadOnlyList<string> Languages { get; } =
    [
        "C#", "C++", "C", "JavaScript", "TypeScript", "Python", "Java",
        "Kotlin", "Swift", "Rust", "Go", "Ruby", "PHP", "Scala", "F#",
        "Haskell", "Clojure", "Erlang", "Elixir", "Dart", "Lua", "R",
        "MATLAB", "Julia", "Perl"
    ];

    public string? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value))
            {
                OnPropertyChanged(nameof(SelectedLanguageText));
            }
        }
    }

    public string SelectedLanguageText => SelectedLanguage ?? "(none)";
}

public sealed partial class UniformGridViewModel
{
    private double _rows;
    private double _columns = 3;
    private double _firstColumn;

    public double RowsValue
    {
        get => _rows;
        set
        {
            if (!SetProperty(ref _rows, value))
            {
                return;
            }

            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(RowsText));
        }
    }

    public double ColumnsValue
    {
        get => _columns;
        set
        {
            if (!SetProperty(ref _columns, value))
            {
                return;
            }

            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(ColumnsText));
        }
    }

    public double FirstColumnValue
    {
        get => _firstColumn;
        set
        {
            if (!SetProperty(ref _firstColumn, value))
            {
                return;
            }

            OnPropertyChanged(nameof(FirstColumn));
            OnPropertyChanged(nameof(FirstColumnText));
        }
    }

    public int Rows => (int)RowsValue;
    public int Columns => (int)ColumnsValue;
    public int FirstColumn => (int)FirstColumnValue;
    public string RowsText => Rows == 0 ? "Auto" : Rows.ToString();
    public string ColumnsText => Columns == 0 ? "Auto" : Columns.ToString();
    public string FirstColumnText => FirstColumn.ToString();
}

public sealed partial class ViewboxViewModel
{
    private double _previewWidth = 200;
    private double _previewHeight = 150;

    public double PreviewWidth
    {
        get => _previewWidth;
        set
        {
            if (SetProperty(ref _previewWidth, value))
            {
                OnPropertyChanged(nameof(PreviewWidthText));
            }
        }
    }

    public double PreviewHeight
    {
        get => _previewHeight;
        set
        {
            if (SetProperty(ref _previewHeight, value))
            {
                OnPropertyChanged(nameof(PreviewHeightText));
            }
        }
    }

    public string PreviewWidthText => ((int)PreviewWidth).ToString();
    public string PreviewHeightText => ((int)PreviewHeight).ToString();
}
