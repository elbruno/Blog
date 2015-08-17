using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;

namespace ElBruno.RemoveComments
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ElBrunoRemoveCommentsCodeRefactoringProvider)), Shared]
    internal class ElBrunoRemoveCommentsCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var commentTrivia = root.DescendantTrivia();
            var found = commentTrivia.Any(comment => comment.IsKind(SyntaxKind.SingleLineCommentTrivia) || comment.IsKind(SyntaxKind.MultiLineCommentTrivia));
            if (!found)
            {
                return;
            }
            var action = CodeAction.Create("Remove comments", c => 
                RemoveAllComments(context, c));
            context.RegisterRefactoring(action);
        }

        private async Task<Document> RemoveAllComments(CodeRefactoringContext context, CancellationToken cancellationToken)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var triviaNodes = oldRoot.DescendantTrivia();
            var newRoot = oldRoot.ReplaceTrivia(triviaNodes, EmptyTriviaNodes);
            var doc = context.Document.WithSyntaxRoot(newRoot);
            return doc;
        }

        private SyntaxTrivia EmptyTriviaNodes(SyntaxTrivia arg1, SyntaxTrivia arg2)
        {
            if (arg1.IsKind(SyntaxKind.SingleLineCommentTrivia) || arg1.IsKind(SyntaxKind.MultiLineCommentTrivia))
            {
                arg2 = SyntaxFactory.CarriageReturn;
            }
            else
            {
                arg2 = arg1;
            }
            return arg2;
        }
    }
}