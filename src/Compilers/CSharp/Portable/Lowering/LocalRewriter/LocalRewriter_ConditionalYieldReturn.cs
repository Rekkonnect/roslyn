// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
            // Errors should have been produced beforehand
            Debug.Assert(rewrittenExpressionValue is null);

            // This feels hacky, but binding must have been performed somehow, so this is how we handle nullable yield
            // returning
            var initializationAssignmentExpression = rewrittenExpression;
            PropertySymbol? propertySymbol = null;
            bool nullableValueAccess = false;
            if (rewrittenExpression is BoundCall rewrittenCallExpression)
            {
                propertySymbol = (rewrittenCallExpression.ExpressionSymbol as SubstitutedMethodSymbol)?.AssociatedSymbol as PropertySymbol;

                // CONSIDER: Add WellKnownMember.System_Nullable_T__Value_get
                if (propertySymbol?.Name == nameof(Nullable<int>.Value) &&
                    propertySymbol.ContainingType.IsNullableType())
                {
                    // Use parent to bind to
                    initializationAssignmentExpression = rewrittenCallExpression.ReceiverOpt;
                    nullableValueAccess = true;
                }
            }

            var expressionResultLocal = _factory.SynthesizedLocal(initializationAssignmentExpression.Type!, syntax);
            var expressionResultInitialization = _factory.Assignment(_factory.Local(expressionResultLocal), initializationAssignmentExpression);

            BoundExpression yieldedExpression = _factory.Local(expressionResultLocal);
            if (nullableValueAccess)
            {
                yieldedExpression = _factory.Property(yieldedExpression, propertySymbol);
            }

            var nullCheck = MakeNullCheck(syntax, _factory.Local(expressionResultLocal), BinaryOperatorKind.NotEqual);
            var yield = new BoundYieldReturnStatement(syntax, yieldedExpression);
            return _factory.Block(
                ImmutableArray.Create(expressionResultLocal),
                expressionResultInitialization,
                _factory.If(nullCheck, yield));
        }
    }
}
