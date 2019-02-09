// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.ApiDesign
{
    using Microsoft.CodeAnalysis;

    internal static class SymbolIsBannedAnalyzer
    {
        public const string BannedSymbolsFileName = "BannedSymbols.txt";

        private const string RS0030Identifier = "RS0030";
        private const string RS0031Identifier = "RS0031";

        public static DiagnosticDescriptor SymbolIsBannedRule { get; } = new DiagnosticDescriptor(
            id: RS0030Identifier,
            title: ApiDesignResources.SymbolIsBannedTitle,
            messageFormat: ApiDesignResources.SymbolIsBannedMessage,
            category: AnalyzerCategory.ApiDesign,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: AnalyzerConstants.EnabledByDefault,
            description: ApiDesignResources.SymbolIsBannedDescription,
            helpLinkUri: "https://github.com/DotNetAnalyzers/BannedApiAnalyzer/blob/master/docs/RS0030.md",
            customTags: WellKnownDiagnosticTags.Telemetry);

        public static DiagnosticDescriptor DuplicateBannedSymbolRule { get; } = new DiagnosticDescriptor(
            id: RS0031Identifier,
            title: ApiDesignResources.DuplicateBannedSymbolTitle,
            messageFormat: ApiDesignResources.DuplicateBannedSymbolMessage,
            category: AnalyzerCategory.ApiDesign,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: AnalyzerConstants.EnabledByDefault,
            description: ApiDesignResources.DuplicateBannedSymbolDescription,
            helpLinkUri: "https://github.com/DotNetAnalyzers/BannedApiAnalyzer/blob/master/docs/RS0031.md",
            customTags: WellKnownDiagnosticTags.Telemetry);
    }
}
