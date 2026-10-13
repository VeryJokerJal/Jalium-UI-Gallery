using System.Collections.ObjectModel;
using System.Windows.Input;
using Jalium.UI.Gallery.Core.Mvvm;

namespace Jalium.UI.Gallery.Modules.Main.ViewModels;

/// <summary>State, selection and commands for the ListView demo.</summary>
public sealed class ListViewViewModel : Jalium.UI.Gallery.Core.Mvvm.ViewModelBase
{
    private const string XamlCode = """
        <TextBox Text="{Binding NewItemText, Mode=TwoWay}" />
        <Button Content="Add" Command="{Binding AddItemCommand}" />
        <Button Content="Remove" Command="{Binding RemoveItemCommand}" />

        <ListView ItemsSource="{Binding Items}"
                  SelectedItem="{Binding SelectedItem, Mode=TwoWay}" />

        <TextBlock Text="{Binding SelectionText}" />
        """;

    private const string CSharpCode = """
        // ListViewViewModel.cs
        public sealed class ListViewViewModel : ViewModelBase
        {
            public ObservableCollection<string> Items { get; } =
                ["Sample Item 1", "Sample Item 2", "Sample Item 3"];

            public ICommand AddItemCommand { get; }
            public ICommand RemoveItemCommand { get; }

            // Commands update the collection; the View only binds to it.
        }
        """;

    private int _nextItemNumber = 4;
    private string _newItemText = string.Empty;
    private string? _selectedItem;

    public ListViewViewModel()
    {
        Files =
        [
            new("Document.docx", "Word Document", "245 KB", "2026-01-15"),
            new("Budget.xlsx", "Excel Spreadsheet", "128 KB", "2026-02-01"),
            new("Photo.png", "PNG Image", "1.2 MB", "2026-01-20"),
            new("Readme.md", "Markdown File", "4 KB", "2026-02-10"),
            new("App.exe", "Application", "3.8 MB", "2025-12-05"),
            new("Config.json", "JSON File", "2 KB", "2026-02-14"),
            new("Notes.txt", "Text File", "12 KB", "2026-01-28"),
            new("Backup.zip", "ZIP Archive", "56 MB", "2025-11-30")
        ];

        Items = ["Sample Item 1", "Sample Item 2", "Sample Item 3"];
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand(RemoveItem, () => SelectedItem != null);
    }

    public ObservableCollection<ListViewFileInfo> Files { get; }

    public ObservableCollection<string> Items { get; }

    public string NewItemText
    {
        get => _newItemText;
        set => SetProperty(ref _newItemText, value);
    }

    public string? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnPropertyChanged(nameof(SelectionText));
                ((RelayCommand)RemoveItemCommand).NotifyCanExecuteChanged();
            }
        }
    }

    public string SelectionText =>
        SelectedItem == null ? "No item selected" : $"Selected: {SelectedItem}";

    public ICommand AddItemCommand { get; }

    public ICommand RemoveItemCommand { get; }

    public string XamlExample => XamlCode;

    public string CSharpExample => CSharpCode;

    private void AddItem()
    {
        var text = string.IsNullOrWhiteSpace(NewItemText)
            ? $"Item {_nextItemNumber++}"
            : NewItemText.Trim();
        Items.Add(text);
        NewItemText = string.Empty;
    }

    private void RemoveItem()
    {
        if (SelectedItem == null)
        {
            return;
        }

        var selected = SelectedItem;
        SelectedItem = null;
        Items.Remove(selected);
    }
}

public sealed record ListViewFileInfo(
    string Name,
    string Type,
    string Size,
    string Modified);
