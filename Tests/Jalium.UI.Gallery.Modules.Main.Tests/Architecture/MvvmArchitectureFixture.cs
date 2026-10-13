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
/// Executable boundaries for a conventional WPF-style MVVM architecture.
/// These rules intentionally reject View-owned state and wrapper base classes.
/// </summary>
public sealed class MvvmArchitectureFixture
{
    private static readonly Regex CodeBehindPattern = new(
        """
        ^\s*
        (?:using\s+(?:static\s+)?[^;]+;\s*)*
        namespace\s+[A-Za-z_][A-Za-z0-9_.]*;\s*
        public\s+partial\s+class\s+(?<view>[A-Za-z_][A-Za-z0-9_]*)\s*
        :\s*(?:Page|Window)\s*
        \{\s*
            public\s+\k<view>\s*\(\s*\)\s*
            \{\s*
                InitializeComponent\s*\(\s*\)\s*;\s*
            \}\s*
        \}\s*$
        """,
        RegexOptions.CultureInvariant |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline);

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
    public void Views_AreNativeXamlBackedPagesOrWindows_WithAutoWiring()
    {
        var repositoryRoot = FindRepositoryRoot();
        var viewsRoot = GetViewsRoot(repositoryRoot);
        var viewTypes = GetViewTypes();
        var jalxamlFiles = Directory
            .EnumerateFiles(viewsRoot, "*.jalxaml", SearchOption.AllDirectories)
            .ToArray();
        var xamlClasses = new HashSet<string>(StringComparer.Ordinal);

        Assert.NotEmpty(jalxamlFiles);
        foreach (var file in jalxamlFiles)
        {
            var source = File.ReadAllText(file);
            var classMatch = Regex.Match(
                source,
                @"\bx:Class\s*=\s*""(?<class>[^""]+)""",
                RegexOptions.CultureInvariant);

            Assert.True(
                classMatch.Success,
                $"{Path.GetRelativePath(repositoryRoot, file)} has no x:Class.");
            Assert.Contains(
                "hosting:ViewModelLocator.AutoWireViewModel=\"True\"",
                source,
                StringComparison.Ordinal);
            Assert.DoesNotContain(
                "DataContext=",
                source,
                StringComparison.Ordinal);
            xamlClasses.Add(classMatch.Groups["class"].Value);
        }

        Assert.Equal(
            viewTypes.Select(type => type.FullName).OrderBy(value => value),
            xamlClasses.OrderBy(value => value));
        Assert.All(viewTypes, type =>
        {
            Assert.True(
                typeof(Page).IsAssignableFrom(type) ||
                typeof(Window).IsAssignableFrom(type));
            Assert.DoesNotContain(
                "Mvvm",
                type.BaseType?.Name ?? string.Empty,
                StringComparison.Ordinal);
            Assert.All(
                type.GetConstructors(),
                constructor => Assert.Empty(constructor.GetParameters()));
        });
    }

    [Fact]
    public void JalxamlCodeBehind_ContainsOnlyInitializeComponent()
    {
        var repositoryRoot = FindRepositoryRoot();
        var files = Directory
            .EnumerateFiles(
                GetViewsRoot(repositoryRoot),
                "*.jalxaml.cs",
                SearchOption.AllDirectories)
            .ToArray();
        var violations = files
            .Where(file => !CodeBehindPattern.IsMatch(
                RemoveComments(File.ReadAllText(file))))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(files);
        Assert.True(
            violations.Length == 0,
            "A JALXAML code-behind may contain only its parameterless " +
            "constructor and InitializeComponent():" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void ViewsFolder_ContainsNoProgrammaticViewImplementations()
    {
        var repositoryRoot = FindRepositoryRoot();
        var violations = Directory
            .EnumerateFiles(
                GetViewsRoot(repositoryRoot),
                "*.cs",
                SearchOption.AllDirectories)
            .Where(file => !file.EndsWith(
                ".jalxaml.cs",
                StringComparison.OrdinalIgnoreCase))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Views must be declarative JALXAML views:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
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
