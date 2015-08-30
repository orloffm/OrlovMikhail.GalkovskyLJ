using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.BookWriter
{
    public class PostPartsMaker : IPostPartsMaker
    {
        static readonly ILog log = LogManager.GetLogger(typeof(PostPartsMaker));

        private const string lineStartRegexString = @"^[*\da-zА-ЯA-Zа-я]*\.";
        private const string artificialLineRegexString = @"^\s*(?:-|_|\*|\+|\.)+\s*$";
        private readonly Regex _lineStartRegex;
        private readonly Regex _artificialLineRegex;

        public PostPartsMaker()
        {
            _lineStartRegex = new Regex(lineStartRegexString, RegexOptions.Compiled);
            _artificialLineRegex = new Regex(artificialLineRegexString, RegexOptions.Compiled);
        }

        public PostPartBase[] CreateTextParts(HTMLTokenBase[] tokens, IFileStorage fs)
        {
            // Convert as is, with minor merges.
            List<PostPartBase> ret = CreatePartsFirstPass(tokens, fs).ToList();

            // Now we have to remove artificial separators because
            // we can live without them, merge line breaks into paragraphs,
            // and merge text entries together.

            // Consecutive texts into singles.
            MergeText(ret);

            // Some people quote with -- <text> --. We try to convert
            // them to paired italics and remove if we can't.
            // Remove artificial separators.
            RemoveArtificialLines(ret);

            // Trim text near breaks.
            SecondPassTextProcess(ret);

            // Multiple line breaks into paragraphs.
            MergeLineBreaksIntoParagraphs(ret);

            RemoveLineBreaksBeforeAndAfterFormatting(ret);

            return ret.ToArray();
        }

        private void RemoveLineBreaksBeforeAndAfterFormatting(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                // Try remove line break after formatting start.
                bool isBegin = items[i] is ItalicStartPart || items[i] is BoldStartPart;
                if (isBegin)
                {
                    PostPartBase next = (i < items.Count - 1 ? items[i + 1] : null);
                    bool nextIsBreak = next != null && (next is LineBreakPart);

                    if (nextIsBreak)
                        items.RemoveAt(i + 1);
                    continue;
                }

                // Try remove line break before formatting end.
                bool isEnd = items[i] is ItalicEndPart || items[i] is BoldEndPart;
                if (isEnd)
                {
                    PostPartBase previous = (i > 0 ? items[i - 1] : null);
                    bool previousIsBreak = previous != null && (previous is LineBreakPart);

                    if (previousIsBreak)
                    {
                        items.RemoveAt(i - 1);
                        i--;
                    }
                }
            }
        }

        private void RemoveArtificialLines(List<PostPartBase> items)
        {
            // At these indeces are the artificial lines.
            int[] artificialIndeces = Enumerable.Range(0, items.Count)
                .Where(z =>
                {
                    RawTextPostPart r = items[z] as RawTextPostPart;
                    if (r == null)
                        return false;

                    // Is it actually a line?
                    PostPartBase previous = (z > 0 ? items[z - 1] : null);
                    PostPartBase next = (z < items.Count - 1 ? items[z + 1] : null);
                    bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                    bool nextIsBreak = next == null || (next is LineBreakPart || next is ParagraphStartPart);

                    if (!(previousIsBreak && nextIsBreak))
                        return false;

                    bool isArtificialLine = _artificialLineRegex.IsMatch(r.Text);
                    return isArtificialLine;
                })
                .ToArray();

            for (int p = 0; p < artificialIndeces.Length - 1; p += 2)
            {
                int a = artificialIndeces[p];
                int b = artificialIndeces[p + 1];
                items[a] = new ItalicStartPart();
                items[b] = new ItalicEndPart();
            }

            if (artificialIndeces.Length % 2 != 0)
            {
                // Last unmatched, remove it.
                int i = artificialIndeces.Last();
                if (items[i] is RawTextPostPart)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }
        }

        private void MergeLineBreaksIntoParagraphs(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                // Is it a line break?
                LineBreakPart rtpp = items[i] as LineBreakPart;
                if (rtpp == null)
                    continue;

                // Are there line breaks later?
                int max = -1;
                for (int p = i + 1; p < items.Count; p++)
                {
                    if (items[p] is LineBreakPart)
                        max = p;
                    else
                        break;
                }

                // Delete those.
                if (max > i)
                {
                    for (int p = max; p > i; p--)
                        items.RemoveAt(p);

                    // Replace the original line break part.
                    items[i] = new ParagraphStartPart();
                }
            }
        }

        private void MergeText(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;
                if (rtpp == null)
                    continue;

                while (i < items.Count - 1)
                {
                    RawTextPostPart next = items[i + 1] as RawTextPostPart;
                    if (next == null)
                        break;

                    // Join text, remove next item.
                    rtpp.Text = rtpp.Text + next.Text;
                    items.RemoveAt(i + 1);
                }
            }
        }

        private void SecondPassTextProcess(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                PostPartBase previous = (i > 0 ? items[i - 1] : null);
                PostPartBase next = (i < items.Count - 1 ? items[i + 1] : null);

                if (items[i] is RawTextPostPart)
                {
                    RawTextPostPart rtpp = items[i] as RawTextPostPart;

                    // What should be trimmed?
                    bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                    bool nextIsBreak = next == null || (next is LineBreakPart || next is ParagraphStartPart);

                    // Trim accordingly.
                    if (previousIsBreak && nextIsBreak)
                        rtpp.Text = rtpp.Text.Trim();
                    else if (previousIsBreak)
                        rtpp.Text = rtpp.Text.TrimStart();
                    else if (nextIsBreak)
                        rtpp.Text = rtpp.Text.TrimEnd();

                    // Prepend numbers with {empty} to disable lists.
                    if (previousIsBreak)
                    {
                        bool startsWithNumberAndDot = _lineStartRegex.IsMatch(rtpp.Text);
                        if (startsWithNumberAndDot)
                            rtpp.Text = "{empty}" + rtpp.Text;
                    }
                }
                else if (items[i] is ParagraphStartPart || items[i] is LineBreakPart)
                {
                    if (previous == null)
                    {
                        // This is the first. Stay on it.
                        items.RemoveAt(i);
                        i--;
                    }
                    else if (next == null)
                    {
                        // This is the last. Go to previous item.
                        items.RemoveAt(i);
                        i -= 2;
                    }
                }
            }
        }

        IEnumerable<PostPartBase> CreatePartsFirstPass(HTMLTokenBase[] tokens, IFileStorage fs)
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

                        case HTMLElementKind.Anchor:
                            if (!tagToken.IsOpening)
                                break;

                            int closingA = FindClosingTag(tokens, i, HTMLElementKind.Anchor);
                            string href = tagToken.Attributes.GetExistingOrDefault("href");

                            // Is it a real link?
                            if (closingA < i + 2 || String.IsNullOrWhiteSpace(href))
                                break;

                            string[] textsInside = Enumerable.Range(i + 1, closingA - i - 1)
                                                                .Select(z => tokens[z])
                                                                .OfType<TextHTMLToken>()
                                                                .Select(z => z.Text).ToArray();

                            string result = String.Join("", textsInside);
                            if (result != href)
                            {
                                // Href differs from text inside.
                                // Write out the href explicitly.
                                string replacee = String.Format("({0}) ", href);
                                yield return new RawTextPostPart(replacee);
                            }
                            break;

                        case HTMLElementKind.LineBreak:
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
                            string src;
                            if (tagToken.Attributes.TryGetValue("src", out src))
                            {
                                FileInfoBase local = fs.TryGet(src);
                                if (local != null)
                                    yield return new ImagePart(local);
                                else
                                {
                                    log.WarnFormat("Encountered image {0} not local.", src);

                                    // No image saved, so we will just write the URL.
                                    string replaceString = String.Format("({0})", src);
                                    yield return new RawTextPostPart(replaceString);
                                }
                            }
                            else
                                log.Warn("Encountered image tag without source.");

                            break;
                    }
                }
            }
        }

        /// <summary>Returns the index of the specified closing tag.</summary>
        /// <param name="startIndex">Starting index.</param>
        /// <param name="kind">Element kind.</param>
        static int FindClosingTag(HTMLTokenBase[] tokens, int startIndex, HTMLElementKind kind)
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
