using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text.RegularExpressions;
using Jalium.UI;
using Jalium.UI.Controls;
using Jalium.UI.Gallery.Core.Mvvm;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Modules.Main.Views;
using Jalium.UI.Gallery.Modules.Main.Views.Pages;
using Jalium.UI.Gallery.Services.Interfaces;
using Jalium.UI.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Architecture;

/// <summary>
/// Executable boundaries for templated CustomControls with independent ViewModels.
/// Templates own the visual trees; ViewModels own application state.
/// </summary>
public sealed class MvvmArchitectureFixture
{
    [Fact]
    public void EveryConcreteView_HasOneUniqueRealViewModel()
    {
        using var provider = BuildProvider();
        var registry = provider.GetRequiredService<ViewRegistry>();
        var viewTypes = GetViewTypes();
        var mappings = new List<(Type View, Type ViewModel)>();

        Assert.NotEmpty(viewTypes);
        foreach (var viewType in viewTypes)
        {
            Assert.True(
                registry.TryGetViewModelType(viewType, out var viewModelType),
                $"Missing ViewModel registration for {viewType.FullName}.");
            Assert.NotNull(viewModelType);
            Assert.False(viewModelType!.IsAbstract);
            Assert.True(
                typeof(Jalium.UI.Gallery.Core.Mvvm.ViewModelBase)
                    .IsAssignableFrom(viewModelType),
                $"{viewType.FullName} maps to non-ViewModel type " +
                $"{viewModelType.FullName}.");
            Assert.Equal(ExpectedViewModelName(viewType), viewModelType.Name);
            mappings.Add((viewType, viewModelType));
        }

        Assert.Equal(
            mappings.Count,
            mappings.Select(mapping => mapping.ViewModel).Distinct().Count());
    }

    [Fact]
    public void EveryViewHasATypedDefaultTemplateInThemes()
    {
        var repositoryRoot = FindRepositoryRoot();
        var module = Path.Combine(repositoryRoot, "Modules", "Jalium.UI.Gallery.Modules.Main");
        foreach (var type in GetViewTypes())
        {
            var relative = type == typeof(MainWindow)
                ? "GalleryWindow.jalxaml" : Path.Combine("Pages", type.Name + ".jalxaml");
            var path = Path.Combine(module, "Themes", relative);
            Assert.True(File.Exists(path), $"Missing default template for {type.FullName}: {path}");
            var source = File.ReadAllText(path);
            var templateOwner = type == typeof(MainWindow) ? "GalleryWindow" : type.Name;
            Assert.Matches(@"<ControlTemplate\s+TargetType=""(?:pages|views|controls):" + templateOwner + @"""", source);
            Assert.Contains("hosting:ViewModelLocator.AutoWireViewModel", source, StringComparison.Ordinal);
            Assert.DoesNotContain("x:Class=", source, StringComparison.Ordinal);
            Assert.DoesNotContain("DataContext=", source, StringComparison.Ordinal);
            Assert.True(typeof(Page).IsAssignableFrom(type) || typeof(Window).IsAssignableFrom(type));
            Assert.All(type.GetConstructors(), constructor => Assert.Empty(constructor.GetParameters()));
        }
    }

    [Fact]
    public void ViewsHaveNoCodeBehindOrInitializeComponent()
    {
        var views = GetViewsRoot(FindRepositoryRoot());
        Assert.Empty(Directory.EnumerateFiles(views, "*.jalxaml.cs", SearchOption.AllDirectories));
        Assert.Empty(Directory.EnumerateFiles(views, "*.jalxaml", SearchOption.AllDirectories));
        var sources = Directory.EnumerateFiles(views, "*.cs", SearchOption.AllDirectories).ToArray();
        Assert.Equal(GetViewTypes().Count, sources.Length);
        foreach (var file in sources)
        {
            var source = RemoveComments(File.ReadAllText(file));
            Assert.DoesNotContain("InitializeComponent", source, StringComparison.Ordinal);
            Assert.DoesNotContain("partial class", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ViewClassesDoNotBuildTheirVisualTreesInCode()
    {
        var sources = Directory.EnumerateFiles(GetViewsRoot(FindRepositoryRoot()), "*.cs", SearchOption.AllDirectories);
        foreach (var file in sources)
        {
            var source = RemoveComments(File.ReadAllText(file));
            Assert.DoesNotMatch(@"new\s+(?:Grid|Border|TextBlock|StackPanel|ContentPresenter|Button|ScrollViewer)\b", source);
            Assert.DoesNotContain("Children.Add(", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ProductionSource_HasNoLegacyMvvmWrappersOrEscapeHatches()
    {
        var repositoryRoot = FindRepositoryRoot();
        var roots = new[]
        {
            Path.Combine(repositoryRoot, "Jalium.UI.Gallery.Core"),
            Path.Combine(
                repositoryRoot,
                "Modules",
                "Jalium.UI.Gallery.Modules.Main")
        };
        var forbidden = new Regex(
            @"\b(?:MvvmPage|MvvmWindow|RequiredViewModel|" +
            @"ViewOnlyState|FallbackViewModel)\b",
            RegexOptions.CultureInvariant);
        var violations = roots
            .SelectMany(root => Directory.EnumerateFiles(
                root,
                "*.*",
                SearchOption.AllDirectories))
            .Where(IsProductionSource)
            .SelectMany(file => File
                .ReadLines(file)
                .Select((line, index) => new
                {
                    File = file,
                    Line = line,
                    Number = index + 1
                })
                .Where(candidate => forbidden.IsMatch(candidate.Line)))
            .Select(candidate =>
                $"{Path.GetRelativePath(repositoryRoot, candidate.File)}:" +
                $"{candidate.Number}: {candidate.Line.Trim()}")
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Legacy MVVM wrappers and state escape hatches are forbidden:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void ViewModels_DoNotReferenceViewsOrFrameworkElements()
    {
        using var provider = BuildProvider();
        var registry = provider.GetRequiredService<ViewRegistry>();
        var violations = GetViewTypes()
            .Select(viewType =>
            {
                registry.TryGetViewModelType(viewType, out var viewModelType);
                return viewModelType!;
            })
            .Distinct()
            .SelectMany(GetMemberTypeReferences)
            .Where(reference =>
                ContainsViewOrFrameworkElement(reference.Type))
            .Select(reference =>
                $"{reference.ViewModel.Name}.{reference.Member} references " +
                reference.Type.FullName)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "ViewModels cannot reference Views or framework elements in " +
            "constructors, fields, properties, events, or methods:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void ViewFactory_AutoWiresEveryRealViewModel()
    {
        using var provider = BuildProvider();
        var factory = provider.GetRequiredService<IViewFactory>();
        var registry = provider.GetRequiredService<ViewRegistry>();

        foreach (var viewType in GetViewTypes())
        {
            Assert.True(
                registry.TryGetViewModelType(viewType, out var viewModelType),
                $"Missing ViewModel registration for {viewType.FullName}.");

            var view = Assert.IsAssignableFrom<FrameworkElement>(
                factory.CreateView(viewType));
            Assert.Equal(viewModelType, view.DataContext?.GetType());
        }
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IMessageService>());
        services.AddMainModule();
        return services.BuildServiceProvider();
    }

    private static IReadOnlyList<Type> GetViewTypes() =>
        MainModule.GetConcreteViewTypes(typeof(MainModule).Assembly);

    private static string ExpectedViewModelName(Type viewType)
    {
        var name = viewType.Name;
        if (name.EndsWith("Page", StringComparison.Ordinal))
        {
            name = name[..^"Page".Length];
        }

        return $"{name}ViewModel";
    }

    private static string RemoveComments(string source)
    {
        source = Regex.Replace(
            source,
            @"/\*.*?\*/",
            string.Empty,
            RegexOptions.Singleline | RegexOptions.CultureInvariant);
        return Regex.Replace(
            source,
            @"^\s*//.*(?:\r?\n|\z)",
            string.Empty,
            RegexOptions.Multiline | RegexOptions.CultureInvariant);
    }

    private static bool IsProductionSource(string file) =>
        !file.Contains(
            $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
            StringComparison.OrdinalIgnoreCase) &&
        !file.Contains(
            $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
            StringComparison.OrdinalIgnoreCase) &&
        (file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
         file.EndsWith(".jalxaml", StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<(
        Type ViewModel,
        string Member,
        Type Type)> GetMemberTypeReferences(Type viewModelType)
    {
        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        foreach (var constructor in viewModelType.GetConstructors(flags))
        {
            foreach (var parameter in constructor.GetParameters())
            {
                yield return (
                    viewModelType,
                    $".ctor({parameter.Name})",
                    parameter.ParameterType);
            }
        }

        foreach (var field in viewModelType.GetFields(flags))
        {
            yield return (viewModelType, field.Name, field.FieldType);
        }

        foreach (var property in viewModelType.GetProperties(flags))
        {
            yield return (
                viewModelType,
                property.Name,
                property.PropertyType);
            foreach (var parameter in property.GetIndexParameters())
            {
                yield return (
                    viewModelType,
                    $"{property.Name}[{parameter.Name}]",
                    parameter.ParameterType);
            }
        }

        foreach (var eventInfo in viewModelType.GetEvents(flags))
        {
            if (eventInfo.EventHandlerType != null)
            {
                yield return (
                    viewModelType,
                    eventInfo.Name,
                    eventInfo.EventHandlerType);
            }
        }

        foreach (var method in viewModelType.GetMethods(flags))
        {
            yield return (
                viewModelType,
                $"{method.Name} return",
                method.ReturnType);
            foreach (var parameter in method.GetParameters())
            {
                yield return (
                    viewModelType,
                    $"{method.Name}({parameter.Name})",
                    parameter.ParameterType);
            }
        }
    }

    private static bool ContainsViewOrFrameworkElement(Type type)
    {
        if (type.HasElementType)
        {
            return ContainsViewOrFrameworkElement(
                type.GetElementType()!);
        }

        if (typeof(FrameworkElement).IsAssignableFrom(type) ||
            type.Namespace?.StartsWith(
                "Jalium.UI.Gallery.Modules.Main.Views",
                StringComparison.Ordinal) == true)
        {
            return true;
        }

        return type.IsGenericType &&
            type.GetGenericArguments().Any(
                ContainsViewOrFrameworkElement);
    }

    private static string GetViewsRoot(string repositoryRoot) =>
        Path.Combine(
            repositoryRoot,
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views");

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(sourceFile)
            ?? throw new InvalidOperationException(
                "Unable to locate test source."));

        while (directory != null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "Jalium.UI.Gallery.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "Unable to locate repository root.");
    }
}
