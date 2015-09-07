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

        public PostPartBase[] CreateTextParts(HTMLTokenBase[] tokens, IFileStorage fs)
        {
            IProcessor[] processors = CreateProcessorsList();
            List<PostPartBase>[] results = new List<PostPartBase>[processors.Length + 1];

            // Convert as is, with minor merges.
            results[0] = CreatePartsFirstPass(tokens, fs).ToList();

            for(int i = 0; i < processors.Length; i++)
            {
                IProcessor p = processors[i];
                List<PostPartBase> source = results[i];
                List<PostPartBase> result = p.Process(source);

                results[i + 1] = result;
            }

            return results[processors.Length].ToArray();
        }

        private IProcessor[] CreateProcessorsList()
        {
            List<IProcessor> ret = new List<IProcessor>();

            Action addNormalizers = () =>
            {
                ret.Add(new TextMergingProcessor());
                ret.Add(new OpenCloseRemovalProcessor());
                ret.Add(new TextTrimmingProcessor());
                ret.Add(new LineBreaksMergingProcessor());
            };

            // Basic first pass. ============================

            addNormalizers();

            // Structural processing. =======================

            // Swap line breaks with formatting if it starts before or ends after it.
            ret.Add(new LineBreakAdjacentFormattingSwapProcessor());

            addNormalizers();

            // Images must be on separate lines.
            ret.Add(new ImagesExtralineProcessor());

            // Some people quote with --<br><text><br> --. We try to convert
            // them to paired italics and remove if we can't.
            // Remove artificial separators.
            // !!! Requires merged line breaks.
            ret.Add(new ArtificialLinesRemoverProcessor());

            addNormalizers();

            // Spaces after chevrons.
            ret.Add(new ChevronsProcessor());

            // Span formatting over paragraphs.
            ret.Add(new FormattingSpanningProcessor());

            // Final text processing ========================

            addNormalizers();

            ret.Add(new QuoteNormalizingProcessor());

            ret.Add(new BlocksAtTheEdgesRemover());
            ret.Add(new ListsDisablerProcessor());

            // Double spaces.
            ret.Add(new DoubleSpacesRemovalProcessor());

            return ret.ToArray();
        }

        IEnumerable<PostPartBase> CreatePartsFirstPass(HTMLTokenBase[] tokens, IFileStorage fs)
        {
            string src;

            for(int i = 0; i < tokens.Length; i++)
            {
                HTMLTokenBase t = tokens[i];
                if(t is TextHTMLToken)
                {
                    TextHTMLToken textToken = t as TextHTMLToken;
                    yield return new RawTextPostPart(textToken.Text);
                }
                else
                {
                    TagHTMLToken tagToken = t as TagHTMLToken;
                    TagHTMLToken nextTagToken = (i != tokens.Length - 1) ? tokens[i + 1] as TagHTMLToken : null;
                    bool isPairWithNext = nextTagToken != null && nextTagToken.Kind == tagToken.Kind
                                  && (tagToken.IsOpening && !tagToken.IsClosing)
                                  && (nextTagToken.IsClosing && !nextTagToken.IsOpening);

                    switch(tagToken.Kind)
                    {
                        default:
                        case HTMLElementKind.Other:
                            break;

                        case HTMLElementKind.LineBreak:
                            yield return LineBreakPart.Instance;
                            break;

                        case HTMLElementKind.Anchor:
                            if(!tagToken.IsOpening)
                                break;

                            int closingA = FindClosingTag(tokens, i, HTMLElementKind.Anchor);
                            string href = tagToken.Attributes.GetExistingOrDefault("href");

                            // Is it a real link?
                            bool isFake = closingA < i + 2 || String.IsNullOrWhiteSpace(href);
                            if(isFake)
                                break;

                            // What is the content?
                            string[] textsInside = Enumerable.Range(i + 1, closingA - i - 1)
                                                                .Select(z => tokens[z])
                                                                .OfType<TextHTMLToken>()
                                                                .Select(z => z.Text).ToArray();
                            string result = String.Join("", textsInside);

                            bool hrefIsAutomatedFromURL = (result == href);
                            if(!hrefIsAutomatedFromURL)
                            {
                                // Href differs from text inside.
                                // Write out the href explicitly.
                                string replacee = String.Format("({0}) ", href);
                                yield return new RawTextPostPart(replacee);
                            }
                            break;

                        case HTMLElementKind.Bold:
                            if(isPairWithNext)
                            {
                                // Skip both.
                                i++;
                            }
                            else
                            {
                                if(tagToken.IsOpening)
                                    yield return BoldStartPart.Instance;
                                else
                                    yield return BoldEndPart.Instance;
                            }
                            break;

                        case HTMLElementKind.Span:
                            // If it has lj:user attribute, we consider it
                            // a username link.
                            string username;
                            if(tagToken.IsOpening && tagToken.Attributes.TryGetValue("lj:user", out username))
                            {
                                string classValue = tagToken.Attributes.GetExistingOrDefault("class") ?? String.Empty;
                                bool isCommunity = classValue.Contains("i-ljuser-type-C");

                                yield return new UserLinkPart(username, isCommunity);
                                int closingIndex = FindClosingTag(tokens, i, HTMLElementKind.Span);
                                i = closingIndex;
                            }
                            break;

                        case HTMLElementKind.Underline:
                        case HTMLElementKind.Italic:
                            if(isPairWithNext)
                            {
                                // Skip both.
                                i++;
                            }
                            else
                            {
                                if(tagToken.IsOpening)
                                    yield return ItalicStartPart.Instance;
                                else
                                    yield return ItalicEndPart.Instance;
                            }
                            break;

                        case HTMLElementKind.Center:
                            if(!tagToken.IsClosing)
                                break;

                            // All <br/>'sboth 1 or 2 after it should
                            // be treated as a new paragraph.
                            int brsFound = 0;
                            for(int p = i + 1; p < tokens.Length && p < i + 3; p++)
                            {
                                TagHTMLToken someToken = tokens[p] as TagHTMLToken;
                                if(someToken != null && someToken.Kind == HTMLElementKind.LineBreak)
                                    brsFound++;
                                else
                                    break;
                            }
                            if(brsFound > 0)
                            {
                                // If we've found some <br/>s, step over them.
                                yield return new ParagraphStartPart();
                                i += brsFound;
                            }
                            break;

                        case HTMLElementKind.IFrame:
                            if(!tagToken.IsOpening)
                                break;

                            if(!tagToken.Attributes.TryGetValue("src", out src))
                            {
                                log.Warn("Encountered iframe tag without source.");
                                break;
                            }

                            // Trying to parse the video source.
                            VideoPart vp = TryCreateAVideoPart(src);
                            if(vp != null)
                                yield return vp;

                            break;

                        case HTMLElementKind.Image:
                            if(!tagToken.Attributes.TryGetValue("src", out src))
                            {
                                log.Warn("Encountered image tag without source.");
                                break;
                            }

                            FileInfoBase local = fs.TryGet(src);
                            if(local != null)
                            {
                                if(!local.Exists)
                                    log.WarnFormat("Encountered image {0} does not exist.", src);
                                else
                                    yield return new ImagePart(local);
                            }
                            else
                            {
                                log.WarnFormat("Encountered image {0} not local.", src);

                                // No image saved, so we will just write the URL.
                                string replaceString = String.Format("({0})", src);
                                yield return new RawTextPostPart(replaceString);
                            }

                            break;
                    }
                }
            }
        }

        private VideoPart TryCreateAVideoPart(string src)
        {
            var kvps = WebTools.ParseKeyValuePairsFromURL(src);

            string videoSource;
            if(!kvps.TryGetValue("source", out videoSource))
                return null;
            string videoId;
            if(!kvps.TryGetValue("vid", out videoId))
                return null;

            string fullUrl;
            switch(videoSource)
            {
                case "youtube":
                    fullUrl = String.Format(@"https://www.youtube.com/watch?v={0}", videoId);
                    break;

                default:
                    fullUrl = null;
                    break;
            }

            if(fullUrl == null)
                return null;

            return new VideoPart(fullUrl);
        }

        /// <summary>Returns the index of the specified closing tag.</summary>
        /// <param name="startIndex">Starting index.</param>
        /// <param name="kind">Element kind.</param>
        static int FindClosingTag(HTMLTokenBase[] tokens, int startIndex, HTMLElementKind kind)
        {
            int count = 1;
            for(int i = startIndex + 1; i < tokens.Length; i++)
            {
                TagHTMLToken t = tokens[i] as TagHTMLToken;
                if(t == null)
                    continue;

                bool isThisKind = t.Kind == kind;
                if(!isThisKind)
                    continue;

                if(t.IsOpening)
                    count++;
                if(t.IsClosing)
                    count--;

                // This is the closing tag.
                if(count == 0)
                    return i;
            }

            return -1;
        }
    }
}
