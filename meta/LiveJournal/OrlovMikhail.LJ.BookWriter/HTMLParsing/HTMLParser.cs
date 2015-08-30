using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OrlovMikhail.LJ.BookWriter
{
    public class HTMLParser : IHTMLParser
    {
        static readonly ILog log = LogManager.GetLogger(typeof(HTMLParser));

        public IEnumerable<HTMLTokenBase> Parse(string html)
        {
            StringBuilder sb = new StringBuilder();
            char[] chars = html.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '<')
                {
                    // It's a tag.
                    int closingIndex = html.IndexOf('>', i + 1);
                    if (closingIndex == -1)
                        throw new InvalidOperationException();

                    TagHTMLToken tag = TagHTMLToken.FromTag(html.Substring(i, closingIndex - i + 1));
                    yield return tag;
                    i = closingIndex;
                }
                else
                {
                    int nextTagIndex = html.IndexOf('<', i + 1);
                    if (nextTagIndex == -1)
                        nextTagIndex = html.Length;
                    string text = html.Substring(i, nextTagIndex - i);
                    if (text.Length > 0)
                        yield return new TextHTMLToken(text);
                    i = nextTagIndex - 1;
                }
            }
        }
    }
}
