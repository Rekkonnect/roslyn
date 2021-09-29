// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Extensions
{
    internal static class SyntaxKindExtensions
    {
        /// <summary>
        /// Determine if the given <see cref="SyntaxKind"/> array contains the given kind.
        /// </summary>
        /// <param name="kinds">Array to search</param>
        /// <param name="kind">Sought value</param>
        /// <returns>True if <paramref name = "kinds"/> contains the value<paramref name= "kind"/>.</returns>
        /// <remarks>PERF: Not using Array.IndexOf here because it results in a call to IndexOf on the
        /// default EqualityComparer for SyntaxKind.The default comparer for SyntaxKind is the
        /// ObjectEqualityComparer which results in boxing allocations.</remarks>
        public static bool Contains(this SyntaxKind[] kinds, SyntaxKind kind)
        {
            foreach (var k in kinds)
            {
                if (k == kind)
                {
                    return true;
                }
            }

            return false;
        }

        public static SyntaxKind MapCompoundAssignmentKindToBinaryExpressionKind(this SyntaxKind syntaxKind)
        {
            return syntaxKind switch
            {
                SyntaxKind.AddAssignmentExpression => SyntaxKind.AddExpression,
                SyntaxKind.SubtractAssignmentExpression => SyntaxKind.SubtractExpression,
                SyntaxKind.MultiplyAssignmentExpression => SyntaxKind.MultiplyExpression,
                SyntaxKind.DivideAssignmentExpression => SyntaxKind.DivideExpression,
                SyntaxKind.ModuloAssignmentExpression => SyntaxKind.ModuloExpression,
                SyntaxKind.AndAssignmentExpression => SyntaxKind.BitwiseAndExpression,
                SyntaxKind.ExclusiveOrAssignmentExpression => SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.OrAssignmentExpression => SyntaxKind.BitwiseOrExpression,
                SyntaxKind.LogicalAndAssignmentExpression => SyntaxKind.LogicalAndExpression,
                SyntaxKind.LogicalOrAssignmentExpression => SyntaxKind.LogicalOrExpression,
                SyntaxKind.LeftShiftAssignmentExpression => SyntaxKind.LeftShiftExpression,
                SyntaxKind.RightShiftAssignmentExpression => SyntaxKind.RightShiftExpression,
                SyntaxKind.CoalesceAssignmentExpression => SyntaxKind.CoalesceExpression,
                _ => SyntaxKind.None,
            };
        }
        public static SyntaxKind MapCompoundAssignmentKindToBinaryExpressionKindWithFailAssertion(this SyntaxKind syntaxKind)
        {
            var result = MapCompoundAssignmentKindToBinaryExpressionKind(syntaxKind);
#if DEBUG
            if (result is SyntaxKind.None)
                Debug.Fail($"Unhandled compound assignment kind: {syntaxKind}");
#endif
            return result;
        }
    }
}
