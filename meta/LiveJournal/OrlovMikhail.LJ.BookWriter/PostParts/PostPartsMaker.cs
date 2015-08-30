using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.BookWriter
{
    public class PostPartsMaker : IPostPartsMaker
    {
        static readonly ILog log = LogManager.GetLogger(typeof(PostPartsMaker));

        public IEnumerable<PostPartBase> CreateTextParts(HTMLTokenBase[] tokens, IFileStorage fs)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                HTMLTokenBase t = tokens[i];
                if (t is TextHTMLToken)
                {
                    TextHTMLToken textToken = t as TextHTMLToken;
                    yield return new RawTextPostPart(textToken.Text);
                }
                else
                {
                    TagHTMLToken tagToken = t as TagHTMLToken;
                    TagHTMLToken next = (i != tokens.Length - 1) ? tokens[i + 1] as TagHTMLToken : null;
                    bool isPair = next != null && next.Kind == tagToken.Kind
                                  && (tagToken.IsOpening && !tagToken.IsClosing)
                                  && (next.IsClosing && !next.IsOpening);

                    switch (tagToken.Kind)
                    {
                        default:
                        case HTMLElementKind.Other:
                            break;

                        case HTMLElementKind.LineBreak:
                            bool theNextIsAlsoABreak = (next != null) && next.Kind == HTMLElementKind.LineBreak;
                            if (theNextIsAlsoABreak)
                            {
                                // Two line breaks mean a paragraph.
                                yield return new ParagraphStartPart();
                                i++;
                            }
                            else
                                yield return new LineBreakPart();
                            break;

                        case HTMLElementKind.Bold:
                            if (isPair)
                            {
                                // Skip both.
                                i++;
                            }
                            else
                            {
                                if (tagToken.IsOpening)
                                    yield return new BoldStartPart();
                                else
                                    yield return new BoldEndPart();
                            }
                            break;

                        case HTMLElementKind.Span:
                            // If it has lj:user attribute, we consider it
                            // a username link.
                            string username;
                            if (tagToken.IsOpening && tagToken.Attributes.TryGetValue("lj:user", out username))
                            {
                                yield return new UserLinkPart(username);
                                int closingIndex = FindClosingTag(tokens, i, HTMLElementKind.Span);
                                i = closingIndex;
                            }
                            break;

                        case HTMLElementKind.Underline:
                        case HTMLElementKind.Italic:
                            if (isPair)
                            {
                                // Skip both.
                                i++;
                            }
                            else
                            {
                                if (tagToken.IsOpening)
                                    yield return new ItalicStartPart();
                                else
                                    yield return new ItalicEndPart();
                            }
                            break;

                        case HTMLElementKind.Center:
                            if (tagToken.IsClosing)
                            {
                                // All <br/>'sboth 1 or 2 after it should
                                // be treated as a new paragraph.
                                int brsFound = 0;
                                for (int p = i + 1; p < tokens.Length && p < i + 3; p++)
                                {
                                    TagHTMLToken someToken = tokens[p] as TagHTMLToken;
                                    if (someToken != null && someToken.Kind == HTMLElementKind.LineBreak)
                                        brsFound++;
                                    else
                                        break;
                                }
                                if (brsFound > 0)
                                {
                                    // If we've found some <br/>s, step over them.
                                    yield return new ParagraphStartPart();
                                    i += brsFound;
                                }
                            }
                            break;

                        case HTMLElementKind.Image:
                            string src = tagToken.Attributes["src"];
                            FileInfoBase local = fs.TryGet(src);
                            if (local != null)
                                yield return new ImagePart(local);
                            else
                                log.WarnFormat("Encountered image {0} not local.", src);
                            break;
                    }
                }
            }
        }

        /// <summary>Returns the index of the specified closing tag.</summary>
        /// <param name="startIndex">Starting index.</param>
        /// <param name="kind">Element kind.</param>
        private int FindClosingTag(HTMLTokenBase[] tokens, int startIndex, HTMLElementKind kind)
        {
            int count = 1;
            for (int i = startIndex + 1; i < tokens.Length; i++)
            {
                TagHTMLToken t = tokens[i] as TagHTMLToken;
                if (t == null)
                    continue;

                bool isThisKind = t.Kind == kind;
                if (!isThisKind)
                    continue;

                if (t.IsOpening)
                    count++;
                if (t.IsClosing)
                    count--;

                // This is the closing tag.
                if (count == 0)
                    return i;
            }

            return -1;
        }
    }
}
