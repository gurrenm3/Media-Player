using System.Windows.Controls;
using System.Windows.Documents;

namespace Media_Player.Extensions
{
    public static class RichTextBoxExt
    {
        public static bool IsEmpty(this RichTextBox rtb) => string.IsNullOrEmpty(rtb.GetText().Trim());

        public static string GetText(this RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;
        }

        public static void SetText(this RichTextBox rtb, string text)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            textRange.Text = text;
        }
    }
}
