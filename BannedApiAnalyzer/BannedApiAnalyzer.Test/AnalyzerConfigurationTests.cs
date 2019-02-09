// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using BannedApiAnalyzer.CSharp.ApiDesign;
    using BannedApiAnalyzer.VisualBasic.ApiDesign;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Microsoft.CodeAnalysis.VisualBasic;
    using Xunit;

    public class AnalyzerConfigurationTests
    {
        public static IEnumerable<object[]> AllAnalyzers
        {
            get
            {
                foreach (var type in typeof(AnalyzerCategory).Assembly.DefinedTypes)
                {
                    if (type.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any())
                    {
                        yield return new object[] { type };
                    }
                }

                foreach (var type in typeof(CSharpSymbolIsBannedAnalyzer).Assembly.DefinedTypes)
                {
                    if (type.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any())
                    {
                        yield return new object[] { type };
                    }
                }

                foreach (var type in typeof(VisualBasicSymbolIsBannedAnalyzer).Assembly.DefinedTypes)
                {
                    if (type.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any())
                    {
                        yield return new object[] { type };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(AllAnalyzers))]
        public async Task TestEmptySourceAsync(Type analyzerType)
        {
            if (analyzerType.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any(attr => ((DiagnosticAnalyzerAttribute)attr).Languages.Contains(LanguageNames.CSharp)))
            {
                await new CSharpTest(analyzerType)
                {
                    TestCode = string.Empty,
                }.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }

            if (analyzerType.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), true).Any(attr => ((DiagnosticAnalyzerAttribute)attr).Languages.Contains(LanguageNames.VisualBasic)))
            {
                await new BasicTest(analyzerType)
                {
                    TestCode = string.Empty,
                }.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Theory]
        [MemberData(nameof(AllAnalyzers))]
        public void TestHelpLink(Type analyzerType)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                if (diagnostic.DefaultSeverity == DiagnosticSeverity.Hidden && diagnostic.CustomTags.Contains(WellKnownDiagnosticTags.NotConfigurable))
                {
                    // This diagnostic will never appear in the UI
                    continue;
                }

                string expected = $"https://github.com/DotNetAnalyzers/BannedApiAnalyzer/blob/master/docs/{diagnostic.Id}.md";
                Assert.Equal(expected, diagnostic.HelpLinkUri);
            }
        }

        private class CSharpTest : CodeFixTest<XUnitVerifier>
        {
            private readonly Type _analyzerType;

            public CSharpTest(Type analyzerType)
            {
                _analyzerType = analyzerType;
            }

            public override string Language => LanguageNames.CSharp;

            protected override string DefaultFileExt => "cs";

            protected override CompilationOptions CreateCompilationOptions()
                => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
                => new CodeFixProvider[0];

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
                => new[] { (DiagnosticAnalyzer)Activator.CreateInstance(_analyzerType) };
        }

        private class BasicTest : CodeFixTest<XUnitVerifier>
        {
            private readonly Type _analyzerType;

            public BasicTest(Type analyzerType)
            {
                _analyzerType = analyzerType;
            }

            public override string Language => LanguageNames.VisualBasic;

            protected override string DefaultFileExt => "vb";

            protected override CompilationOptions CreateCompilationOptions()
                => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
                => new CodeFixProvider[0];

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
                => new[] { (DiagnosticAnalyzer)Activator.CreateInstance(_analyzerType) };
        }
    }
}
