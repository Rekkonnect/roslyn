// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public partial class ContinueStatementSyntax
    {
        public ContinueStatementSyntax Update(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
            => Update(AttributeLists, continueKeyword, semicolonToken);
        public ContinueStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
            => Update(attributeLists, continueKeyword, Expression, semicolonToken);
    }
}

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class SyntaxFactory
    {
        public static ContinueStatementSyntax ContinueStatement(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
            => ContinueStatement(attributeLists: default, continueKeyword, semicolonToken);
        public static ContinueStatementSyntax ContinueStatement(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
            => ContinueStatement(attributeLists, continueKeyword, null, semicolonToken);
    }
}
