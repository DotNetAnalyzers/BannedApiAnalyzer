﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.Test
{
    using System;
    using System.Text;
    using BannedApiAnalyzer.Helpers;
    using Xunit;

    /// <summary>
    /// Unit tests related to the public API surface of BannedApiAnalyzer.dll.
    /// </summary>
    public class PublicApiTests
    {
        /// <summary>
        /// This test ensures all types in BannedApiAnalyzer.dll are marked internal.
        /// </summary>
        [Fact]
        public void TestAllAnalyzerTypesAreInternal()
        {
            var publicTypes = new StringBuilder();
            foreach (Type type in typeof(AnalyzerCategory).Assembly.ExportedTypes)
            {
                if (publicTypes.Length > 0)
                {
                    publicTypes.Append(", ");
                }

                publicTypes.Append(type.Name);
            }

            Assert.Equal(string.Empty, publicTypes.ToString());
        }

        /// <summary>
        /// This test ensures all types in BannedApiAnalyzer.CodeFixes.dll are marked internal.
        /// </summary>
        [Fact]
        public void TestAllCodeFixTypesAreInternal()
        {
            var publicTypes = new StringBuilder();
            foreach (Type type in typeof(CustomBatchFixAllProvider).Assembly.ExportedTypes)
            {
                if (publicTypes.Length > 0)
                {
                    publicTypes.Append(", ");
                }

                publicTypes.Append(type.Name);
            }

            Assert.Equal(string.Empty, publicTypes.ToString());
        }
    }
}
