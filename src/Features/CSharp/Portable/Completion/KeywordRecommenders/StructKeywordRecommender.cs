// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;

namespace Microsoft.CodeAnalysis.CSharp.Completion.KeywordRecommenders
{
    internal class StructKeywordRecommender : AbstractSyntacticSingleKeywordRecommender
    {
        private static readonly ISet<SyntaxKind> s_validModifiers = new HashSet<SyntaxKind>(SyntaxFacts.EqualityComparer)
            {
                SyntaxKind.InternalKeyword,
                SyntaxKind.PublicKeyword,
                SyntaxKind.PrivateKeyword,
                SyntaxKind.ProtectedKeyword,
                SyntaxKind.UnmanagedKeyword,
                SyntaxKind.UnsafeKeyword,
                SyntaxKind.RefKeyword,
                SyntaxKind.ReadOnlyKeyword
            };

        public StructKeywordRecommender()
            : base(SyntaxKind.StructKeyword)
        {
        }

        protected override bool IsValidContext(int position, CSharpSyntaxContext context, CancellationToken cancellationToken)
        {
            var syntaxTree = context.SyntaxTree;
            return
                IsValidContextForTypeDeclarationKind(s_validModifiers, context, canBePartial: true, canBeUnmanaged: true, cancellationToken: cancellationToken) ||
                context.LeftToken.GetPreviousTokenIfTouchingWord(position).IsKind(SyntaxKind.RecordKeyword) ||
                syntaxTree.IsTypeParameterConstraintStartContext(position, context.LeftToken);
        }
    }
}
