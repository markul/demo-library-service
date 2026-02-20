using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace LibraryService.Utf8BomAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class Utf8BomAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "LSUTF8BOM001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Source file encoding must be UTF-8 BOM",
        messageFormat: "File '{0}' must be saved as UTF-8 with BOM",
        category: "Formatting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var filePath = context.Tree.FilePath;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        if (filePath.IndexOf($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) >= 0 ||
            filePath.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return;
        }

        var sourceText = context.Tree.GetText(context.CancellationToken);
        if (sourceText.Encoding is UTF8Encoding utf8Encoding && utf8Encoding.GetPreamble().Length > 0)
        {
            return;
        }

        var location = context.Tree.GetRoot(context.CancellationToken).GetLocation();
        var diagnostic = Diagnostic.Create(Rule, location, Path.GetFileName(filePath));
        context.ReportDiagnostic(diagnostic);
    }
}

