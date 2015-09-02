using System.Net;

namespace OrlovMikhail.LJ.BookWriter
{
    public sealed class TextHTMLToken : HTMLTokenBase
    {
        public TextHTMLToken(string rawText)
        {
            this.Text = WebUtility.HtmlDecode(rawText);
        }

        public string Text { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}