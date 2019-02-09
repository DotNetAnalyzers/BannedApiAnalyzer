// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.Helpers
{
    using Microsoft.CodeAnalysis;

    internal static class SyntaxNodeExtensions
    {
        public static ISymbol GetDeclaredOrReferencedSymbol(this SyntaxNode node, SemanticModel model)
        {
            if (node == null)
            {
                return null;
            }

            return model.GetDeclaredSymbol(node) ?? model.GetSymbolInfo(node).Symbol;
        }
    }
}
