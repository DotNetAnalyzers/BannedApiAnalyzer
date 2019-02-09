// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace BannedApiAnalyzer.Test.ApiDesign
{
    using System.Threading;
    using System.Threading.Tasks;
    using BannedApiAnalyzer.ApiDesign;
    using BannedApiAnalyzer.CSharp.ApiDesign;
    using BannedApiAnalyzer.VisualBasic.ApiDesign;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Microsoft.CodeAnalysis.VisualBasic.Testing;
    using Xunit;
    using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
        BannedApiAnalyzer.CSharp.ApiDesign.CSharpSymbolIsBannedAnalyzer,
        Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
    using VerifyVB = Microsoft.CodeAnalysis.VisualBasic.Testing.XUnit.CodeFixVerifier<
        BannedApiAnalyzer.VisualBasic.ApiDesign.VisualBasicSymbolIsBannedAnalyzer,
        Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

    // For specification of document comment IDs see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#processing-the-documentation-file
    public class SymbolIsBannedAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnosticForNoBannedTextCSharpAsync()
        {
            await VerifyCS.VerifyAnalyzerAsync("class C { }");
        }

        [Fact]
        public async Task NoDiagnosticForNoBannedTextVisualBasicAsync()
        {
            await VerifyVB.VerifyAnalyzerAsync(@"Class C
End Class");
        }

        [Fact]
        public async Task NoDiagnosticReportedForEmptyBannedTextAsync()
        {
            var source = string.Empty;

            var bannedText = string.Empty;

            await VerifyCSharpAsync(source, bannedText);
        }

        [Fact(Skip = "Not sure how to port this test.")]
        public async Task NoDiagnosticForInvalidBannedTextAsync()
        {
            await VerifyCSharpAsync(source: string.Empty, bannedApiText: null);
        }

        [Fact]
        public async Task DiagnosticReportedForDuplicateBannedApiLinesAsync()
        {
            var source = string.Empty;
            var bannedText = @"
T:System.Console
T:System.Console";

            await VerifyCSharpAsync(
                source,
                bannedText,
                new DiagnosticResult(SymbolIsBannedAnalyzer.DuplicateBannedSymbolRule)
                    .WithSpan(SymbolIsBannedAnalyzer.BannedSymbolsFileName, 3, 1, 3, 17)
                    .WithSpan(SymbolIsBannedAnalyzer.BannedSymbolsFileName, 2, 1, 2, 17)
                    .WithArguments("System.Console"));
        }

        [Fact]
        public async Task DiagnosticReportedForDuplicateBannedApiLinesWithDifferentIdsAsync()
        {
            // The colon in the documentation ID is optional.
            // Verify that it doesn't cause exceptions when building look ups.
            var source = string.Empty;
            var bannedText = @"
T:System.Console;Message 1
TSystem.Console;Message 2";

            await VerifyCSharpAsync(
                source,
                bannedText,
                new DiagnosticResult(SymbolIsBannedAnalyzer.DuplicateBannedSymbolRule)
                    .WithSpan(SymbolIsBannedAnalyzer.BannedSymbolsFileName, 3, 1, 3, 26)
                    .WithSpan(SymbolIsBannedAnalyzer.BannedSymbolsFileName, 2, 1, 2, 27)
                    .WithArguments("System.Console"));
        }

        [Fact]
        public async Task CSharp_BannedApiFile_MessageIncludedInDiagnosticAsync()
        {
            var source = @"
namespace N
{
    class Banned { }
    class C
    {
        void M()
        {
            var c = new Banned();
        }
    }
}";

            var bannedText = @"T:N.Banned;Use NonBanned instead";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(9, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", ": Use NonBanned instead"));
        }

        [Fact]
        public async Task CSharp_BannedApiFile_WhiteSpaceAsync()
        {
            var source = @"
namespace N
{
    class Banned { }
    class C
    {
        void M()
        {
            var c = new Banned();
        }
    }
}";

            var bannedText = @"
  T:N.Banned  ";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(9, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedApiFile_WhiteSpaceWithMessageAsync()
        {
            var source = @"
namespace N
{
    class Banned { }
    class C
    {
        void M()
        {
            var c = new Banned();
        }
    }
}";

            var bannedText = @"T:N.Banned ; Use NonBanned instead ";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(9, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", ": Use NonBanned instead"));
        }

        [Fact]
        public async Task CSharp_BannedApiFile_EmptyMessageAsync()
        {
            var source = @"
namespace N
{
    class Banned { }
    class C
    {
        void M()
        {
            var c = new Banned();
        }
    }
}";

            var bannedText = @"T:N.Banned;";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(9, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedType_ConstructorAsync()
        {
            var source = @"
namespace N
{
    class Banned { }
    class C
    {
        void M()
        {
            var c = new Banned();
        }
    }
}";

            var bannedText = @"
T:N.Banned";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(9, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedGenericType_ConstructorAsync()
        {
            var source = @"
class C
{
    void M()
    {
        var c = new System.Collections.Generic.List<string>();
    }
}";

            var bannedText = @"
T:System.Collections.Generic.List`1";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(6, 17, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "List<T>", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedNestedType_ConstructorAsync()
        {
            var source = @"
class C
{
    class Nested { }
    void M()
    {
        var n = new Nested();
    }
}";

            var bannedText = @"
T:C.Nested";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(7, 17, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Nested", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedType_MethodOnNestedTypeAsync()
        {
            var source = @"
class C
{
    public static class Nested
    {
        public static void M() { }
    }
}

class D
{
    void M2()
    {
        C.Nested.M();
    }
}";
            var bannedText = @"
T:C";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(14, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedInterface_MethodAsync()
        {
            var source = @"
interface I
{
    void M();
}

class C
{
    void M()
    {
        I i = null;
        i.M();
    }
}";
            var bannedText = @"T:I";

            await VerifyCSharpAsync(source, bannedText, GetCSharpResultAt(12, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "I", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedClass_PropertyAsync()
        {
            var source = @"
class C
{
    public int P { get; set; }
    void M()
    {
        P = P;
    }
}";
            var bannedText = @"T:C";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(7, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetCSharpResultAt(7, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedClass_FieldAsync()
        {
            var source = @"
class C
{
    public int F;
    void M()
    {
        F = F;
    }
}";
            var bannedText = @"T:C";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(7, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetCSharpResultAt(7, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedClass_EventAsync()
        {
            var source = @"
using System;

class C
{
    public event EventHandler E;
    void M()
    {
        E += null;
        E -= null;
        E(null, EventArgs.Empty);
    }
}";
            var bannedText = @"T:C";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(9, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetCSharpResultAt(10, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetCSharpResultAt(11, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedClass_MethodGroupAsync()
        {
            var source = @"
delegate void D();
class C
{
    void M()
    {
        D d = M;
    }
}
";
            var bannedText = @"T:C";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(7, 15, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedClass_DocumentationReferenceAsync()
        {
            var source = @"
class C { }

/// <summary><see cref=""C"" /></summary>
class D { }
";
            var bannedText = @"T:C";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(4, 25, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedAttribute_UsageOnTypeAsync()
        {
            var source = @"
using System;

[AttributeUsage(AttributeTargets.All, Inherited = true)]
class BannedAttribute : Attribute { }

[Banned]
class C { }
class D : C { }
";
            var bannedText = @"T:BannedAttribute";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(7, 2, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedAttribute_UsageOnMemberAsync()
        {
            var source = @"
using System;

[AttributeUsage(AttributeTargets.All, Inherited = true)]
class BannedAttribute : Attribute { }

class C 
{
    [Banned]
    public int Foo { get; }
}
";
            var bannedText = @"T:BannedAttribute";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(9, 6, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedAttribute_UsageOnAssemblyAsync()
        {
            var source = @"
using System;

[assembly: BannedAttribute]

[AttributeUsage(AttributeTargets.All, Inherited = true)]
class BannedAttribute : Attribute { }
";

            var bannedText = @"T:BannedAttribute";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(4, 12, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedAttribute_UsageOnModuleAsync()
        {
            var source = @"
using System;

[module: BannedAttribute]

[AttributeUsage(AttributeTargets.All, Inherited = true)]
class BannedAttribute : Attribute { }
";

            var bannedText = @"T:BannedAttribute";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(4, 10, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedConstructorAsync()
        {
            var source = @"
namespace N
{
    class Banned
    {
        public Banned() {}
        public Banned(int i) {}
    }
    class C
    {
        void M()
        {
            var c = new Banned();
            var d = new Banned(1);
        }
    }
}";

            var bannedText1 = @"M:N.Banned.#ctor";
            var bannedText2 = @"M:N.Banned.#ctor(System.Int32)";

            await VerifyCSharpAsync(
                source,
                bannedText1,
                GetCSharpResultAt(13, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned.Banned()", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText2,
                GetCSharpResultAt(14, 21, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned.Banned(int)", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedMethodAsync()
        {
            var source = @"
namespace N
{
    class C
    {
        public void Banned() {}
        public void Banned(int i) {}
        public void Banned<T>(T t) {}

        void M()
        {
            Banned();
            Banned(1);
            Banned<string>("""");
        }
    }

    class D<T>
    {
        public void Banned() {}
        public void Banned(int i) {}
        public void Banned<U>(U u) {}

        void M()
        {
            Banned();
            Banned(1);
            Banned<string>("""");
        }
    }
}";

            var bannedText1 = @"M:N.C.Banned";
            var bannedText2 = @"M:N.C.Banned(System.Int32)";
            var bannedText3 = @"M:N.C.Banned``1(``0)";
            var bannedText4 = @"M:N.D`1.Banned()";
            var bannedText5 = @"M:N.D`1.Banned(System.Int32)";
            var bannedText6 = @"M:N.D`1.Banned``1(``0)";

            await VerifyCSharpAsync(
                source,
                bannedText1,
                GetCSharpResultAt(12, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned()", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText2,
                GetCSharpResultAt(13, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned(int)", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText3,
                GetCSharpResultAt(14, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned<T>(T)", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText4,
                GetCSharpResultAt(26, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "D<T>.Banned()", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText5,
                GetCSharpResultAt(27, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "D<T>.Banned(int)", string.Empty));

            await VerifyCSharpAsync(
                source,
                bannedText6,
                GetCSharpResultAt(28, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "D<T>.Banned<U>(U)", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedPropertyAsync()
        {
            var source = @"
namespace N
{
    class C
    {
        public int Banned { get; set; }

        void M()
        {
            Banned = Banned;
        }
    }
}";

            var bannedText = @"P:N.C.Banned";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(10, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty),
                GetCSharpResultAt(10, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedFieldAsync()
        {
            var source = @"
namespace N
{
    class C
    {
        public int Banned;

        void M()
        {
            Banned = Banned;
        }
    }
}";

            var bannedText = @"F:N.C.Banned";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(10, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty),
                GetCSharpResultAt(10, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedEventAsync()
        {
            var source = @"
namespace N
{
    class C
    {
        public event System.Action Banned;

        void M()
        {
            Banned += null;
            Banned -= null;
            Banned();
        }
    }
}";

            var bannedText = @"E:N.C.Banned";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(10, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty),
                GetCSharpResultAt(11, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty),
                GetCSharpResultAt(12, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned", string.Empty));
        }

        [Fact]
        public async Task CSharp_BannedMethodGroupAsync()
        {
            var source = @"
namespace N
{
    class C
    {
        public void Banned() {}

        void M()
        {
            System.Action b = Banned;
        }
    }
}";

            var bannedText = @"M:N.C.Banned";

            await VerifyCSharpAsync(
                source,
                bannedText,
                GetCSharpResultAt(10, 31, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Banned()", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedType_ConstructorAsync()
        {
            var source = @"
Namespace N
    Class Banned : End Class
    Class C
        Sub M()
            Dim c As New Banned()
        End Sub
    End Class
End Namespace";

            var bannedText = @"T:N.Banned";

            await VerifyVisualBasicAsync(source, bannedText, GetVisualBasicResultAt(6, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Banned", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedGenericType_ConstructorAsync()
        {
            var source = @"
Class C
    Sub M()
        Dim c = New System.Collections.Generic.List(Of String)()
    End Sub
End Class";

            var bannedText = @"
T:System.Collections.Generic.List`1";

            await VerifyVisualBasicAsync(source, bannedText, GetVisualBasicResultAt(4, 17, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "List(Of T)", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedNestedType_ConstructorAsync()
        {
            var source = @"
Class C
    Class Nested : End Class
    Sub M()
        Dim n As New Nested()
    End Sub
End Class";

            var bannedText = @"
T:C.Nested";

            await VerifyVisualBasicAsync(source, bannedText, GetVisualBasicResultAt(5, 18, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C.Nested", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedType_MethodOnNestedTypeAsync()
        {
            var source = @"
Class C
    Public Class Nested
        Public Shared Sub M() : End Sub
    End Class
End Class

Class D
    Sub M2()
        C.Nested.M()
    End Sub
End Class
";
            var bannedText = @"
T:C";

            await VerifyVisualBasicAsync(source, bannedText, GetVisualBasicResultAt(10, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedInterface_MethodAsync()
        {
            var source = @"
Interface I
    Sub M()
End Interface

Class C
    Sub M()
        Dim i As I = Nothing
        i.M()
    End Sub
End Class";
            var bannedText = @"T:I";

            await VerifyVisualBasicAsync(source, bannedText, GetVisualBasicResultAt(9, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "I", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedClass_PropertyAsync()
        {
            var source = @"
Class C
    Public Property P As Integer
    Sub M()
        P = P
    End Sub
End Class";
            var bannedText = @"T:C";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(5, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetVisualBasicResultAt(5, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedClass_FieldAsync()
        {
            var source = @"
Class C
    Public F As Integer
    Sub M()
        F = F
    End Sub
End Class";
            var bannedText = @"T:C";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(5, 9, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetVisualBasicResultAt(5, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedClass_EventAsync()
        {
            var source = @"
Imports System

Class C
    public Event E As EventHandler
    Sub M()
        AddHandler E, Nothing
        RemoveHandler E, Nothing
        RaiseEvent E(Me, EventArgs.Empty)
    End Sub
End Class";
            var bannedText = @"T:C";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(7, 20, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetVisualBasicResultAt(8, 23, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty),
                GetVisualBasicResultAt(9, 20, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedClass_MethodGroupAsync()
        {
            var source = @"
Delegate Sub D()
Class C
    Sub M()
        Dim d as D = AddressOf M
    End Sub
End Class";
            var bannedText = @"T:C";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(5, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedAttribute_UsageOnTypeAsync()
        {
            var source = @"
Imports System

<AttributeUsage(AttributeTargets.All, Inherited:=true)>
Class BannedAttribute
    Inherits Attribute
End Class

<Banned>
Class C
End Class
Class D
    Inherits C
End Class
";
            var bannedText = @"T:BannedAttribute";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(9, 2, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedAttribute_UsageOnMemberAsync()
        {
            var source = @"
Imports System

<AttributeUsage(System.AttributeTargets.All, Inherited:=True)>
Class BannedAttribute
    Inherits System.Attribute
End Class

Class C
    <Banned>
    Public ReadOnly Property Foo As Integer
End Class
";
            var bannedText = @"T:BannedAttribute";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(10, 6, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedAttribute_UsageOnAssemblyAsync()
        {
            var source = @"
Imports System

<Assembly:BannedAttribute>

<AttributeUsage(AttributeTargets.All, Inherited:=True)>
Class BannedAttribute
    Inherits Attribute
End Class
";

            var bannedText = @"T:BannedAttribute";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(4, 2, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedAttribute_UsageOnModuleAsync()
        {
            var source = @"
Imports System

<Module:BannedAttribute>

<AttributeUsage(AttributeTargets.All, Inherited:=True)>
Class BannedAttribute
    Inherits Attribute
End Class
";

            var bannedText = @"T:BannedAttribute";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(4, 2, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "BannedAttribute", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedConstructorAsync()
        {
            var source = @"
Namespace N
    Class Banned
        Sub New : End Sub
        Sub New(ByVal I As Integer) : End Sub
    End Class
    Class C
        Sub M()
            Dim c As New Banned()
            Dim d As New Banned(1)
        End Sub
    End Class
End Namespace";

            var bannedText1 = @"M:N.Banned.#ctor";
            var bannedText2 = @"M:N.Banned.#ctor(System.Int32)";

            await VerifyVisualBasicAsync(
                source,
                bannedText1,
                GetVisualBasicResultAt(9, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Sub New()", string.Empty));

            await VerifyVisualBasicAsync(
                source,
                bannedText2,
                GetVisualBasicResultAt(10, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Sub New(I As Integer)", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedMethodAsync()
        {
            var source = @"
Namespace N
    Class C
        Sub Banned : End Sub
        Sub Banned(ByVal I As Integer) : End Sub
        Sub M()
            Me.Banned()
            Me.Banned(1)
        End Sub
    End Class
End Namespace";

            var bannedText1 = @"M:N.C.Banned";
            var bannedText2 = @"M:N.C.Banned(System.Int32)";

            await VerifyVisualBasicAsync(
                source,
                bannedText1,
                GetVisualBasicResultAt(7, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Sub Banned()", string.Empty));

            await VerifyVisualBasicAsync(
                source,
                bannedText2,
                GetVisualBasicResultAt(8, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Sub Banned(I As Integer)", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedPropertyAsync()
        {
            var source = @"
Namespace N
    Class C
        Public Property Banned As Integer
        Sub M()
            Banned = Banned
        End Sub
    End Class
End Namespace";

            var bannedText = @"P:N.C.Banned";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(6, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Property Banned As Integer", string.Empty),
                GetVisualBasicResultAt(6, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Property Banned As Integer", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedFieldAsync()
        {
            var source = @"
Namespace N
    Class C
        Public Banned As Integer
        Sub M()
            Banned = Banned
        End Sub
    End Class
End Namespace";

            var bannedText = @"F:N.C.Banned";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(6, 13, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Banned As Integer", string.Empty),
                GetVisualBasicResultAt(6, 22, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Banned As Integer", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedEventAsync()
        {
            var source = @"
Namespace N
    Class C
        Public Event Banned As System.Action
        Sub M()
            AddHandler Banned, Nothing
            RemoveHandler Banned, Nothing
            RaiseEvent Banned()
        End Sub
    End Class
End Namespace";

            var bannedText = @"E:N.C.Banned";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(6, 24, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Event Banned As Action", string.Empty),
                GetVisualBasicResultAt(7, 27, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Event Banned As Action", string.Empty),
                GetVisualBasicResultAt(8, 24, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Event Banned As Action", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedMethodGroupAsync()
        {
            var source = @"
Namespace N
    Class C
        Public Sub Banned() : End Sub
        Sub M()
            Dim b As System.Action = AddressOf Banned
        End Sub
    End Class
End Namespace";

            var bannedText = @"M:N.C.Banned";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(6, 38, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "Public Sub Banned()", string.Empty));
        }

        [Fact]
        public async Task VisualBasic_BannedClass_DocumentationReferenceAsync()
        {
            var source = @"
Class C : End Class

''' <summary><see cref=""C"" /></summary>
Class D : End Class
";
            var bannedText = @"T:C";

            await VerifyVisualBasicAsync(
                source,
                bannedText,
                GetVisualBasicResultAt(4, 25, SymbolIsBannedAnalyzer.SymbolIsBannedRule, "C", string.Empty));
        }

        private static DiagnosticResult GetCSharpResultAt(int line, int column, DiagnosticDescriptor descriptor, string v3, string empty)
        {
            return VerifyCS.Diagnostic(descriptor).WithLocation(line, column).WithArguments(v3, empty);
        }

        private static DiagnosticResult GetVisualBasicResultAt(int line, int column, DiagnosticDescriptor descriptor, string v3, string empty)
        {
            return VerifyVB.Diagnostic(descriptor).WithLocation(line, column).WithArguments(v3, empty);
        }

        private async Task VerifyVisualBasicAsync(string source, string bannedApiText, params DiagnosticResult[] expected)
        {
            var testState = new VisualBasicCodeFixTest<VisualBasicSymbolIsBannedAnalyzer, EmptyCodeFixProvider, XUnitVerifier>
            {
                VerifyExclusions = false,
                TestState =
                {
                    Sources = { source },
                    AdditionalFiles = { (SymbolIsBannedAnalyzer.BannedSymbolsFileName, bannedApiText) },
                },
            };

            testState.ExpectedDiagnostics.AddRange(expected);
            await testState.RunAsync(CancellationToken.None);
        }

        private async Task VerifyCSharpAsync(string source, string bannedApiText, params DiagnosticResult[] expected)
        {
            var testState = new CSharpCodeFixTest<CSharpSymbolIsBannedAnalyzer, EmptyCodeFixProvider, XUnitVerifier>
            {
                VerifyExclusions = false,
                TestState =
                {
                    Sources = { source },
                    AdditionalFiles = { (SymbolIsBannedAnalyzer.BannedSymbolsFileName, bannedApiText) },
                },
            };

            testState.ExpectedDiagnostics.AddRange(expected);
            await testState.RunAsync(CancellationToken.None);
        }
    }
}
