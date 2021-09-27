// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public partial class YieldStatementSyntax
    {
        /// <summary>
        /// Gets the first expression of the yield return statement, if it is one. Otherwise, it returns <see langword="null"/>.
        /// </summary>
        public ExpressionSyntax? Expression => ExpressionList.FirstOrDefault();

        public YieldStatementSyntax Update(SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            => Update(AttributeLists, yieldKeyword, returnOrBreakKeyword, SyntaxFactory.SingletonOrEmptySeparatedList(expression), semicolonToken);

        public YieldStatementSyntax Update(SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, SeparatedSyntaxList<ExpressionSyntax> expressionList, SyntaxToken semicolonToken)
            => Update(AttributeLists, yieldKeyword, returnOrBreakKeyword, expressionList, semicolonToken);
    }
}

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class SyntaxFactory
    {
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            => YieldStatement(kind, attributeLists, yieldKeyword, returnOrBreakKeyword, SingletonOrEmptySeparatedList(expression), semicolonToken);

        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
            => YieldStatement(kind, attributeLists: default, yieldKeyword, returnOrBreakKeyword, SingletonOrEmptySeparatedList(expression), semicolonToken);

        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, SeparatedSyntaxList<ExpressionSyntax> expressionList, SyntaxToken semicolonToken)
            => YieldStatement(kind, attributeLists: default, yieldKeyword, returnOrBreakKeyword, expressionList, semicolonToken);

        /// <summary>Creates a new YieldStatementSyntax instance.</summary>
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax? expression)
            => YieldStatement(kind, attributeLists, SingletonOrEmptySeparatedList(expression));

#pragma warning disable RS0027
        /// <summary>Creates a new YieldStatementSyntax instance.</summary>
        public static YieldStatementSyntax YieldStatement(SyntaxKind kind, ExpressionSyntax? expression = null)
            => YieldStatement(kind, SingletonOrEmptySeparatedList(expression));
#pragma warning restore RS0027

        /// <summary>Creates a new YieldStatementSyntax instance of kind YieldBreakStatement.</summary>
        public static YieldStatementSyntax YieldBreakStatement()
            => YieldStatement(SyntaxKind.YieldBreakStatement, SeparatedList<ExpressionSyntax>());
    }
}
