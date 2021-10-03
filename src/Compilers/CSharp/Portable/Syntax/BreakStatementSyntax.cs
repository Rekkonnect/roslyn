// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public partial class BreakStatementSyntax
    {
        public BreakStatementSyntax Update(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
            => Update(AttributeLists, breakKeyword, semicolonToken);
        public BreakStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
            => Update(attributeLists, breakKeyword, Expression, semicolonToken);
    }
}

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class SyntaxFactory
    {
        public static BreakStatementSyntax BreakStatement(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
            => BreakStatement(attributeLists: default, breakKeyword, semicolonToken);
        public static BreakStatementSyntax BreakStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
            => BreakStatement(attributeLists, breakKeyword, null, semicolonToken);
    }
}
