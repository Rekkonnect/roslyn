// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitConditionalYieldReturnStatement(BoundConditionalYieldReturnStatement node)
        {
            // Remove this if not valid
            Debug.Assert(node.Expression.ConstantValue is null);

            var rewrittenExpression = VisitExpression(node.Expression);

            return RewriteConditionalYieldReturnStatement(node.Syntax, rewrittenExpression);
        }

        private BoundNode? RewriteConditionalYieldReturnStatement(SyntaxNode syntax, BoundExpression rewrittenExpression)
        {
            var rewrittenExpressionValue = rewrittenExpression.ConstantValue;
            if (rewrittenExpressionValue == ConstantValue.Null || rewrittenExpressionValue?.IsDefaultValue == true)
            {
                return null;
            }
            else if (rewrittenExpressionValue is not null)
            {
                return new BoundYieldReturnStatement(syntax, rewrittenExpression);
            }

            var expressionResultLocal = _factory.SynthesizedLocal(rewrittenExpression.Type!, syntax);
            var expressionResultInitialization = _factory.Assignment(_factory.Local(expressionResultLocal), rewrittenExpression);

            var nullCheck = MakeNullCheck(syntax, _factory.Local(expressionResultLocal), BinaryOperatorKind.NotEqual);
            var yield = new BoundYieldReturnStatement(syntax, _factory.Local(expressionResultLocal));
            return _factory.Block(
                ImmutableArray.Create(expressionResultLocal),
                expressionResultInitialization,
                _factory.If(nullCheck, yield));
        }
    }
}
