﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal static class AnalyzerConstants
    {
        static AnalyzerConstants()
        {
#if DEBUG
            // In DEBUG builds, the tests are enabled to simplify development and testing.
            DisabledNoTests = true;
#else
            DisabledNoTests = false;
#endif
        }

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to disable a diagnostic which is currently untested.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to disable a diagnostic which is currently untested.
        /// </value>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool DisabledNoTests { get; }

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be enabled by default.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be enabled by default.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool EnabledByDefault => true;

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be disabled by default.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be disabled by default.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool DisabledByDefault => false;
    }
}
