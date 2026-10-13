using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Jalium.UI.Gallery.Modules.Main.Models;
using Jalium.UI.Gallery.Modules.Main.ViewModels;
using Jalium.UI.Gallery.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace Jalium.UI.Gallery.Modules.Main.Tests.Architecture;

/// <summary>
/// Guards the contract between a rendered gallery page and the code shown under
/// it. A code editor must read a real, non-empty ViewModel property, and every
/// unqualified element in a JALXAML example must name a type shipped by Jalium,
/// the Gallery, or the BCL.
/// </summary>
public sealed class GalleryCodeSampleFixture
{
    private static readonly Regex EditControlPattern = new(
        @"<EditControl\b(?<attributes>.*?)/>",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex BindingPattern = new(
        @"\bText\s*=\s*""\{Binding\s+(?<path>[A-Za-z_][A-Za-z0-9_.]*)[^""}]*\}""",
        RegexOptions.CultureInvariant);

    private static readonly Regex BindingMarkupPattern = new(
        @"\{Binding\s+(?<body>[^}]*)\}",
        RegexOptions.CultureInvariant);

    private static readonly Regex BindingAttributePattern = new(
        @"(?<attribute>[A-Za-z_][A-Za-z0-9_.]*)\s*=\s*""\{Binding\s+(?<body>[^}]*)\}""",
        RegexOptions.CultureInvariant);

    private static readonly Regex LanguagePattern = new(
        @"\bLanguage\s*=\s*""(?<language>[^""]+)""",
        RegexOptions.CultureInvariant);

    private static readonly Regex ElementPattern = new(
        @"<\s*(?![!/?])(?:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*):)?(?<name>[A-Za-z_][A-Za-z0-9_.-]*)\b",
        RegexOptions.CultureInvariant);

    private static readonly Regex StaticMemberPattern = new(
        @"\{x:Static\s+(?:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*):)?(?<type>[A-Za-z_][A-Za-z0-9_]*)\.(?<member>[A-Za-z_][A-Za-z0-9_]*)\s*\}",
        RegexOptions.CultureInvariant);

    private static readonly Regex OpeningElementPattern = new(
        @"<\s*(?![!/?])(?:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*):)?(?<name>[A-Za-z_][A-Za-z0-9_.-]*)\b(?<attributes>[^<>]*?)/?>",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex AttributePattern = new(
        @"(?:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*):)?(?<name>[A-Za-z_][A-Za-z0-9_.-]*)\s*=\s*""(?<value>[^""]*)""",
        RegexOptions.CultureInvariant);

    private static readonly Regex SingleQuotedAttributeValuePattern = new(
        @"'[^']*'",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex ClrNamespacePattern = new(
        @"xmlns:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*)\s*=\s*""clr-namespace:(?<namespace>[^;""]+)(?:;assembly=[^""]+)?""",
        RegexOptions.CultureInvariant);

    private static readonly Regex ViewOwnedCSharpPattern = new(
        @"(?<construct>InitializeComponent\s*\(|\bpartial\s+class\s+\w+\s*:\s*(?:Page|Window)\b|\.(?:Click|Checked|Unchecked|SelectionChanged|ValueChanged|TextChanged|Toggled)\s*\+=)",
        RegexOptions.CultureInvariant);

    private static readonly Regex CSharpObjectInitializerPattern = new(
        @"\bnew\s+(?<type>[A-Za-z_][A-Za-z0-9_]*)(?:\s*\([^{};]*\))?\s*\{(?<body>[^{}]*)\}",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static readonly Regex CSharpInitializerMemberPattern = new(
        @"^\s*(?<member>[A-Z][A-Za-z0-9_]*)\s*=",
        RegexOptions.CultureInvariant | RegexOptions.Multiline);

    private static readonly Regex CSharpConstructedTypePattern = new(
        @"\bnew\s+(?<type>[A-Z][A-Za-z0-9_]*)\b",
        RegexOptions.CultureInvariant);

    private static readonly Regex CSharpDeclaredTypePattern = new(
        @"\b(?:class|record|struct|enum)\s+(?<type>[A-Za-z_][A-Za-z0-9_]*)\b",
        RegexOptions.CultureInvariant);

    private static readonly Regex CSharpAliasPattern = new(
        @"\busing\s+(?<type>[A-Za-z_][A-Za-z0-9_]*)\s*=",
        RegexOptions.CultureInvariant);

    private static readonly Regex DataTemplateBlockPattern = new(
        @"<(?:DataTemplate|HierarchicalDataTemplate)\b.*?</(?:DataTemplate|HierarchicalDataTemplate)>",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    [Fact]
    public void EveryCodeEditor_BindsToANonEmptyViewModelString()
    {
        using var provider = BuildProvider();
        var violations = new List<string>();

        foreach (var file in EnumeratePageFiles())
        {
            var source = File.ReadAllText(file);
            var editorMatches = EditControlPattern.Matches(source);
            if (editorMatches.Count == 0)
            {
                continue;
            }

            var viewModelType = ResolveViewModelType(file);
            var viewModel = provider.GetService(viewModelType);
            if (viewModel == null)
            {
                violations.Add($"{Path.GetFileName(file)}: ViewModel is not registered.");
                continue;
            }
            PrepareViewModel(viewModel);

            foreach (RegexMatch editorMatch in editorMatches)
            {
                var attributes = editorMatch.Groups["attributes"].Value;
                var language = LanguagePattern.Match(attributes).Groups["language"].Value;
                var binding = BindingPattern.Match(attributes);
                if (!binding.Success)
                {
                    violations.Add(
                        $"{Path.GetFileName(file)} [{language}]: EditControl has no Text binding.");
                    continue;
                }

                var path = binding.Groups["path"].Value;
                if (!TryReadPropertyPath(viewModel, path, out var value) ||
                    value is not string text ||
                    string.IsNullOrWhiteSpace(text))
                {
                    violations.Add(
                        $"{Path.GetFileName(file)} [{language}]: " +
                        $"{viewModelType.Name}.{path} is missing or empty.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Every code editor must be backed by a non-empty ViewModel string:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void EveryJalxamlExample_UsesResolvableElementTypes()
    {
        using var provider = BuildProvider();
        var knownTypes = LoadKnownElementTypeNames();
        var violations = new List<string>();

        foreach (var file in EnumeratePageFiles())
        {
            var source = File.ReadAllText(file);
            var viewModelType = ResolveViewModelType(file);
            var viewModel = provider.GetService(viewModelType);
            if (viewModel == null)
            {
                continue;
            }

            foreach (RegexMatch editorMatch in EditControlPattern.Matches(source))
            {
                var attributes = editorMatch.Groups["attributes"].Value;
                var language = LanguagePattern.Match(attributes).Groups["language"].Value;
                if (!language.Equals("jalxaml", StringComparison.OrdinalIgnoreCase) &&
                    !language.Equals("xaml", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var binding = BindingPattern.Match(attributes);
                if (!binding.Success ||
                    !TryReadPropertyPath(
                        viewModel,
                        binding.Groups["path"].Value,
                        out var value) ||
                    value is not string example)
                {
                    continue;
                }

                foreach (RegexMatch elementMatch in ElementPattern.Matches(example))
                {
                    var prefix = elementMatch.Groups["prefix"].Value;
                    var name = elementMatch.Groups["name"].Value;
                    if (!string.IsNullOrEmpty(prefix) ||
                        name.Contains('.', StringComparison.Ordinal) ||
                        knownTypes.Contains(name))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{Path.GetFileName(file)}: <{name}> is not a resolvable element type.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "JALXAML examples reference element types that do not exist:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void EveryJalxamlExample_UsesResolvableElementMembers()
    {
        using var provider = BuildProvider();
        var knownTypes = LoadKnownTypes()
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var sample in EnumerateJalxamlExamples(provider))
        {
            foreach (RegexMatch element in
                     OpeningElementPattern.Matches(sample.Example))
            {
                var typeName = element.Groups["name"].Value;
                if (element.Groups["prefix"].Success ||
                    typeName.Contains('.', StringComparison.Ordinal) ||
                    !knownTypes.TryGetValue(typeName, out var types))
                {
                    continue;
                }

                foreach (RegexMatch attribute in AttributePattern.Matches(
                             SanitizeAttributeText(
                                 element.Groups["attributes"].Value)))
                {
                    var memberName = attribute.Groups["name"].Value;
                    if (attribute.Groups["prefix"].Success ||
                        memberName.Equals("xmlns", StringComparison.Ordinal) ||
                        memberName.Contains('.', StringComparison.Ordinal) ||
                        types.Any(type => HasPublicInstanceMember(
                            type,
                            memberName)))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{sample.Page}: <{typeName}> has no public " +
                        $"property or event named {memberName}.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "JALXAML examples contain members that do not exist on their element type:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void EveryActualPage_UsesResolvableElementMembers()
    {
        var knownTypes = LoadKnownTypes()
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var file in EnumeratePageFiles())
        {
            var source = File.ReadAllText(file);
            var clrNamespaces = ClrNamespacePattern
                .Matches(source)
                .Cast<RegexMatch>()
                .ToDictionary(
                    match => match.Groups["prefix"].Value,
                    match => match.Groups["namespace"].Value,
                    StringComparer.Ordinal);
            foreach (RegexMatch element in OpeningElementPattern.Matches(source))
            {
                var typeName = element.Groups["name"].Value;
                if (typeName.Contains('.', StringComparison.Ordinal) ||
                    !knownTypes.TryGetValue(typeName, out var candidates))
                {
                    continue;
                }

                var prefix = element.Groups["prefix"].Value;
                var types = string.IsNullOrEmpty(prefix)
                    ? candidates
                    : clrNamespaces.TryGetValue(prefix, out var clrNamespace)
                        ? candidates
                            .Where(type => type.Namespace == clrNamespace)
                            .ToArray()
                        : [];
                if (types.Length == 0)
                {
                    continue;
                }

                foreach (RegexMatch attribute in AttributePattern.Matches(
                             SanitizeAttributeText(
                                 element.Groups["attributes"].Value)))
                {
                    var memberName = attribute.Groups["name"].Value;
                    if (attribute.Groups["prefix"].Success ||
                        memberName.Equals("xmlns", StringComparison.Ordinal) ||
                        memberName.Contains('.', StringComparison.Ordinal) ||
                        types.Any(type => HasPublicInstanceMember(
                            type,
                            memberName)))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{Path.GetFileName(file)}: <{typeName}> has no public " +
                        $"property or event named {memberName}.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Actual Gallery pages contain members that do not exist on their element type:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void EveryXStaticReference_ResolvesToARealStaticMember()
    {
        using var provider = BuildProvider();
        var knownTypes = LoadKnownTypes()
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var (page, example) in EnumerateJalxamlExamples(provider))
        {
            foreach (RegexMatch match in StaticMemberPattern.Matches(example))
            {
                if (match.Groups["prefix"].Success)
                {
                    continue;
                }

                var typeName = match.Groups["type"].Value;
                var memberName = match.Groups["member"].Value;
                if (!knownTypes.TryGetValue(typeName, out var types) ||
                    !types.Any(type => HasPublicStaticMember(type, memberName)))
                {
                    violations.Add(
                        $"{page}: {typeName}.{memberName} does not exist as a public static member.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "XAML examples contain invalid x:Static references:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void JalxamlExamples_DoNotTeachUnshownCodeBehindHandlers()
    {
        using var provider = BuildProvider();
        var knownTypes = LoadKnownTypes()
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var sample in EnumerateJalxamlExamples(provider))
        {
            foreach (RegexMatch element in
                     OpeningElementPattern.Matches(sample.Example))
            {
                var typeName = element.Groups["name"].Value;
                if (element.Groups["prefix"].Success ||
                    typeName.Contains('.', StringComparison.Ordinal) ||
                    !knownTypes.TryGetValue(typeName, out var types))
                {
                    continue;
                }

                foreach (RegexMatch attribute in AttributePattern.Matches(
                             SanitizeAttributeText(
                                 element.Groups["attributes"].Value)))
                {
                    var value = attribute.Groups["value"].Value;
                    var memberName = attribute.Groups["name"].Value;
                    if (attribute.Groups["prefix"].Success ||
                        value.StartsWith('{') ||
                        !types.Any(type => HasPublicInstanceEvent(
                            type,
                            memberName)))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{sample.Page}: <{typeName}> {memberName}=\"{value}\"");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Gallery previews use ViewModel bindings, so their XAML examples " +
            "must not switch to hidden code-behind handlers:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void CSharpExamples_MatchTheGallerysViewModelArchitecture()
    {
        using var provider = BuildProvider();
        var violations = EnumerateExamples(provider, "csharp")
            .Where(sample => PagePreviewUsesViewModel(sample.Page))
            .SelectMany(sample => ViewOwnedCSharpPattern
                .Matches(sample.Example)
                .Cast<RegexMatch>()
                .Select(match =>
                    $"{sample.Page}: {match.Groups["construct"].Value.Trim()}"))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "C# examples must show the ViewModel/command path used by the live " +
            "preview, not a second code-behind implementation:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void CSharpObjectInitializers_UseRealFrameworkMembers()
    {
        using var provider = BuildProvider();
        var knownTypes = LoadKnownTypes()
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var sample in EnumerateExamples(provider, "csharp"))
        {
            foreach (RegexMatch initializer in
                     CSharpObjectInitializerPattern.Matches(sample.Example))
            {
                var typeName = initializer.Groups["type"].Value;
                if (!knownTypes.TryGetValue(typeName, out var types))
                {
                    continue;
                }

                foreach (RegexMatch member in
                         CSharpInitializerMemberPattern.Matches(
                             initializer.Groups["body"].Value))
                {
                    var memberName = member.Groups["member"].Value;
                    if (types.Any(type => HasPublicInstanceMember(
                            type,
                            memberName)))
                    {
                        continue;
                    }

                    violations.Add(
                        $"{sample.Page}: new {typeName} has no public " +
                        $"member named {memberName}.");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "C# examples contain object-initializer members that do not exist:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void CSharpExamples_ConstructResolvableTypes()
    {
        using var provider = BuildProvider();
        var knownTypeNames = LoadKnownTypes()
            .Select(type => type.Name.Split('`')[0])
            .ToHashSet(StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var sample in EnumerateExamples(provider, "csharp"))
        {
            var declaredTypes = CSharpDeclaredTypePattern
                .Matches(sample.Example)
                .Cast<RegexMatch>()
                .Select(match => match.Groups["type"].Value)
                .Concat(CSharpAliasPattern
                    .Matches(sample.Example)
                    .Cast<RegexMatch>()
                    .Select(match => match.Groups["type"].Value))
                .ToHashSet(StringComparer.Ordinal);

            foreach (RegexMatch construction in
                     CSharpConstructedTypePattern.Matches(sample.Example))
            {
                var typeName = construction.Groups["type"].Value;
                if (knownTypeNames.Contains(typeName) ||
                    declaredTypes.Contains(typeName) ||
                    typeName.Length == 1)
                {
                    continue;
                }

                violations.Add(
                    $"{sample.Page}: constructed type {typeName} cannot be resolved.");
            }
        }

        Assert.True(
            violations.Count == 0,
            "C# examples construct types that are neither shipped nor declared " +
            "inside the example:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    [Fact]
    public void ComponentExamples_MentionTheComponentTheyDemonstrate()
    {
        using var provider = BuildProvider();
        var xamlExamples = EnumerateExamples(provider, "jalxaml", "xaml")
            .GroupBy(sample => sample.Page, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => string.Join(Environment.NewLine, group.Select(item => item.Example)),
                StringComparer.Ordinal);
        var csharpExamples = EnumerateExamples(provider, "csharp")
            .GroupBy(sample => sample.Page, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => string.Join(Environment.NewLine, group.Select(item => item.Example)),
                StringComparer.Ordinal);
        var violations = new List<string>();

        foreach (var component in GalleryComponentCatalog.Items)
        {
            var page = component.PageType.Name + ".jalxaml";
            if (!xamlExamples.TryGetValue(page, out var xaml) ||
                !csharpExamples.TryGetValue(page, out var csharp))
            {
                continue;
            }

            var terms = GetSubjectTerms(component).ToArray();
            if (!terms.Any(term => xaml.Contains(
                    term,
                    StringComparison.OrdinalIgnoreCase)))
            {
                violations.Add(
                    $"{page} [XAML] does not mention {string.Join(" / ", terms)}.");
            }

            if (!terms.Any(term => csharp.Contains(
                    term,
                    StringComparison.OrdinalIgnoreCase)))
            {
                violations.Add(
                    $"{page} [C#] does not mention {string.Join(" / ", terms)}.");
            }
        }

        Assert.True(
            violations.Count == 0,
            "A component page must teach the same subject in its live preview, " +
            "XAML example, and C# example:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void VisualComponentPages_RenderTheirCataloguedControl()
    {
        var visualTypes = LoadKnownTypes()
            .Where(type =>
                !type.IsAbstract &&
                typeof(Jalium.UI.UIElement).IsAssignableFrom(type))
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray(),
                StringComparer.Ordinal);
        var pageRoot = Path.Combine(
            FindRepositoryRoot(),
            "Modules",
            "Jalium.UI.Gallery.Modules.Main",
            "Views",
            "Pages");
        var serviceOwnedControls = new HashSet<string>(StringComparer.Ordinal)
        {
            "NotifyIcon"
        };
        var violations = new List<string>();

        foreach (var component in GalleryComponentCatalog.Items)
        {
            var subject = component.ComponentTypeName;
            if (serviceOwnedControls.Contains(subject) ||
                !visualTypes.ContainsKey(subject))
            {
                continue;
            }

            var pageFile = component.PageType.Name + ".jalxaml";
            var source = File.ReadAllText(Path.Combine(pageRoot, pageFile));
            var codeSection = source.IndexOf(
                "<!-- XAML code example -->",
                StringComparison.Ordinal);
            if (codeSection >= 0)
            {
                source = source[..codeSection];
            }

            var renderedTypes = OpeningElementPattern
                .Matches(source)
                .Cast<RegexMatch>()
                .Select(match => match.Groups["name"].Value)
                .ToHashSet(StringComparer.Ordinal);
            if (!renderedTypes.Contains(subject))
            {
                violations.Add(
                    $"{pageFile}: catalog subject is {subject}, but the live " +
                    "preview never instantiates that control.");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Visual component pages must render the control named by the catalog:" +
            Environment.NewLine +
            string.Join(Environment.NewLine, violations));
    }

    [Fact]
    public void PageLevelBindings_ResolveAgainstThePagesViewModel()
    {
        var violations = new List<string>();

        foreach (var file in EnumeratePageFiles())
        {
            var source = DataTemplateBlockPattern.Replace(
                File.ReadAllText(file),
                string.Empty);
            var viewModelType = ResolveViewModelType(file);

            foreach (RegexMatch binding in BindingAttributePattern.Matches(source))
            {
                if (binding.Groups["attribute"].Value.Equals(
                    "DisplayMemberBinding",
                    StringComparison.Ordinal))
                {
                    continue;
                }

                var body = binding.Groups["body"].Value.Trim();
                if (body.Contains("ElementName=", StringComparison.Ordinal) ||
                    body.Contains("RelativeSource=", StringComparison.Ordinal) ||
                    body.Contains("Source=", StringComparison.Ordinal))
                {
                    continue;
                }

                var path = ExtractBindingPath(body);
                if (string.IsNullOrEmpty(path) || path == "." ||
                    ResolvesPropertyPath(viewModelType, path))
                {
                    continue;
                }

                violations.Add(
                    $"{Path.GetFileName(file)}: {viewModelType.Name}.{path} " +
                    "does not resolve.");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Page-level bindings reference missing ViewModel properties:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                violations.Distinct(StringComparer.Ordinal).OrderBy(value => value)));
    }

    private static string ExtractBindingPath(string body)
    {
        if (body.StartsWith("Path=", StringComparison.Ordinal))
        {
            body = body["Path=".Length..];
        }

        var separator = body.IndexOfAny([',', ' ', '}']);
        return (separator >= 0 ? body[..separator] : body).Trim();
    }

    private static bool ResolvesPropertyPath(Type sourceType, string path)
    {
        var type = sourceType;
        foreach (var rawSegment in path.Split('.'))
        {
            var bracket = rawSegment.IndexOf('[');
            var segment = bracket >= 0
                ? rawSegment[..bracket]
                : rawSegment;
            if (string.IsNullOrEmpty(segment))
            {
                continue;
            }

            var property = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(candidate => candidate.Name.Equals(
                    segment,
                    StringComparison.Ordinal));
            if (property == null)
            {
                return false;
            }

            type = property.PropertyType;
        }

        return true;
    }

    private static IEnumerable<string> GetSubjectTerms(
        GalleryComponentDescriptor component) => component.PageTag switch
    {
        "css" or "cssselectors" or "csslayout" or "cssmappings" =>
            ["Css.", "CSS"],
        "cssoutline" => ["Outline"],
        "razor" => ["@if", "@{", "Razor"],
        "section" => ["@section", "RenderSection"],
        "backdropeffects" => ["Backdrop", "FrostedGlassEffect"],
        "dragdrop" => ["DragDrop", "Drag"],
        "iconelement" => ["IconElement", "SymbolIcon", "FontIcon"],
        "shapes" => ["Shape", "Rectangle", "Ellipse", "Path"],
        "transitions" => ["Transition"],
        "navigationdemo" => ["NavigationWindow", "NavigationService", "Frame"],
        "shadereffects" => ["BlurEffect", "DropShadowEffect"],
        "elementeffects" => ["OuterGlowEffect", "InnerShadowEffect", "EffectGroup"],
        "dialogs" => ["Dialog"],
        "printing" => ["Print"],
        "shellintegration" => ["Shell", "WindowChrome"],
        "themecolors" => ["Theme", "Resource"],
        _ => [component.ComponentTypeName]
    };

    private static bool PagePreviewUsesViewModel(string pageFileName)
    {
        var path = EnumeratePageFiles().Single(file =>
            Path.GetFileName(file).Equals(
                pageFileName,
                StringComparison.Ordinal));
        var source = File.ReadAllText(path);
        var codeSection = source.IndexOf(
            "<!-- XAML code example -->",
            StringComparison.Ordinal);
        if (codeSection >= 0)
        {
            source = source[..codeSection];
        }

        return BindingMarkupPattern
            .Matches(source)
            .Cast<RegexMatch>()
            .Select(match => match.Groups["body"].Value)
            .Any(body =>
                !body.StartsWith("XamlExample", StringComparison.Ordinal) &&
                !body.StartsWith("CSharpExample", StringComparison.Ordinal) &&
                !body.Contains("ElementName=", StringComparison.Ordinal) &&
                !body.Contains("RelativeSource=", StringComparison.Ordinal) &&
                !body.Contains("Source=", StringComparison.Ordinal));
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IMessageService>());
        services.AddMainModule();
        return services.BuildServiceProvider();
    }

    private static IReadOnlySet<string> LoadKnownElementTypeNames()
        => LoadKnownTypes()
            .Select(type => type.Name)
            .ToHashSet(StringComparer.Ordinal);

    private static IReadOnlyList<Type> LoadKnownTypes()
    {
        var outputDirectory = Path.GetDirectoryName(
            typeof(GalleryPageViewModelBase).Assembly.Location)!;
        var assemblies = Directory
            .EnumerateFiles(outputDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Select(path =>
            {
                try
                {
                    return Assembly.LoadFrom(path);
                }
                catch (BadImageFormatException)
                {
                    return null;
                }
                catch (FileLoadException)
                {
                    return null;
                }
            })
            .Where(assembly => assembly != null)
            .Cast<Assembly>()
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .Append(typeof(object).Assembly)
            .Append(typeof(Uri).Assembly)
            .Append(typeof(System.Collections.ObjectModel.ObservableCollection<>).Assembly)
            .Distinct()
            .ToArray();

        return assemblies
            .SelectMany(GetLoadableTypes)
            .Distinct()
            .ToArray();
    }

    private static bool HasPublicStaticMember(Type type, string memberName)
    {
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        return type.GetField(memberName, flags) != null ||
               type.GetProperty(memberName, flags) != null;
    }

    private static bool HasPublicInstanceEvent(Type type, string eventName) =>
        type.GetEvents(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy)
            .Any(eventInfo => eventInfo.Name.Equals(
                eventName,
                StringComparison.Ordinal));

    private static bool HasPublicInstanceMember(Type type, string memberName)
    {
        const BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy;
        return type.GetProperties(flags).Any(property =>
                   property.Name.Equals(memberName, StringComparison.Ordinal)) ||
               type.GetFields(flags).Any(field =>
                   field.Name.Equals(memberName, StringComparison.Ordinal)) ||
               type.GetEvents(flags).Any(eventInfo =>
                   eventInfo.Name.Equals(memberName, StringComparison.Ordinal));
    }

    private static string SanitizeAttributeText(string attributes) =>
        SingleQuotedAttributeValuePattern.Replace(attributes, "''");

    private static IEnumerable<(string Page, string Example)>
        EnumerateJalxamlExamples(ServiceProvider provider)
        => EnumerateExamples(provider, "jalxaml", "xaml");

    private static IEnumerable<(string Page, string Example)>
        EnumerateExamples(ServiceProvider provider, params string[] languages)
    {
        foreach (var file in EnumeratePageFiles())
        {
            var source = File.ReadAllText(file);
            var viewModel = provider.GetService(ResolveViewModelType(file));
            if (viewModel == null)
            {
                continue;
            }
            PrepareViewModel(viewModel);

            foreach (RegexMatch editorMatch in EditControlPattern.Matches(source))
            {
                var attributes = editorMatch.Groups["attributes"].Value;
                var language = LanguagePattern.Match(attributes).Groups["language"].Value;
                if (!languages.Contains(language, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var binding = BindingPattern.Match(attributes);
                if (binding.Success &&
                    TryReadPropertyPath(
                        viewModel,
                        binding.Groups["path"].Value,
                        out var value) &&
                    value is string example)
                {
                    yield return (Path.GetFileName(file), example);
                }
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }

    private static bool TryReadPropertyPath(
        object instance,
        string path,
        out object? value)
    {
        value = instance;
        foreach (var segment in path.Split('.'))
        {
            if (value == null)
            {
                return false;
            }

            var property = value.GetType().GetProperty(
                segment,
                BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
            {
                return false;
            }

            value = property.GetValue(value);
        }

        return true;
    }

    private static void PrepareViewModel(object viewModel)
    {
        if (viewModel is GalleryDemoWorkbenchViewModel workbench)
        {
            workbench.Initialize(new GalleryWorkbenchDefinition(
                "button",
                "Button",
                "Controls",
                "A representative workbench sample.",
                "<Button Content=\"Run\" />",
                "var button = new Button { Content = \"Run\" };"));
        }
    }

    private static Type ResolveViewModelType(string pageFile)
    {
        var pageName = Path.GetFileNameWithoutExtension(pageFile);
        var baseName = pageName.EndsWith("Page", StringComparison.Ordinal)
            ? pageName[..^"Page".Length]
            : pageName;
        var fullName =
            $"Jalium.UI.Gallery.Modules.Main.ViewModels.{baseName}ViewModel";
        return typeof(GalleryPageViewModelBase).Assembly.GetType(fullName)
            ?? throw new InvalidOperationException(
                $"No ViewModel type exists for {Path.GetFileName(pageFile)}.");
    }

    private static IEnumerable<string> EnumeratePageFiles()
    {
        var root = FindRepositoryRoot();
        return Directory
            .EnumerateFiles(
                Path.Combine(
                    root,
                    "Modules",
                    "Jalium.UI.Gallery.Modules.Main",
                    "Views",
                    "Pages"),
                "*.jalxaml",
                SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal);
    }

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFile = "")
    {
        var directory = new DirectoryInfo(
            Path.GetDirectoryName(sourceFile)
            ?? throw new InvalidOperationException(
                "Unable to locate the repository root."));

        while (directory != null &&
               !File.Exists(Path.Combine(directory.FullName, "Jalium.UI.Gallery.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException(
                "Unable to locate the repository root.");
    }
}
