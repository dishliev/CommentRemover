using System.Linq;
using System.Text.RegularExpressions;

namespace CommentRemover
{
    [Command(PackageIds.Command)]
    internal sealed class CodeCommentCleanerCommand : BaseCommand<CodeCommentCleanerCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var docView = await VS.Documents.GetActiveDocumentViewAsync();
            var selection = docView?.TextView.Selection.SelectedSpans.FirstOrDefault();
            if (selection.HasValue)
            {
                var textBuffer = docView.TextBuffer;
                var snapshot = textBuffer.CurrentSnapshot;

                var selectedText = snapshot.GetText(selection.Value);

                var cleanedText = RemoveComments(selectedText);

                cleanedText = RemoveEmptyLines(cleanedText);

                using (var edit = textBuffer.CreateEdit())
                {
                    edit.Replace(selection.Value, cleanedText);
                    edit.Apply();
                }
            }
        }

        private string RemoveComments(string text)
        {
            var pattern = @"(/\*[\s\S]*?\*/)|(//.*)|(/\*[\s\S]*?$)|(///.*)";

            var cleanedText = Regex.Replace(text, pattern, "");

            return cleanedText;
        }

        private string RemoveEmptyLines(string text)
        {
            var pattern = @"^\s*$[\r\n]*";

            var cleanedText = Regex.Replace(text, pattern, "", RegexOptions.Multiline);

            return cleanedText;
        }
    }
}
