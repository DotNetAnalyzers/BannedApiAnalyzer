// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class ITypeSymbolExtensions
    {
        public static bool IsAttribute(this ITypeSymbol symbol)
        {
            for (INamedTypeSymbol b = symbol.BaseType; b != null; b = b.BaseType)
            {
                if (b.MetadataName == nameof(Attribute) &&
                     b.ContainingType == null &&
                     b.ContainingNamespace != null &&
                     b.ContainingNamespace.Name == nameof(System) &&
                     b.ContainingNamespace.ContainingNamespace != null &&
                     b.ContainingNamespace.ContainingNamespace.IsGlobalNamespace)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
