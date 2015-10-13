using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace OrlovMikhail.LJ.BookWriter
{
    public enum HTMLElementKind
    {
        Other,
        Span,
        LineBreak,
        Bold,
        Italic,
        Underline,
        Center,
        Image,
        IFrame,
        Anchor
    }

    public class TagHTMLToken : HTMLTokenBase
    {
        const string attributesRegexString = @"(\S+?)=(?:""(?<a>\S+)""|'(?<a>\S+)'|(?<a>[^\s>]+))[>\s]+";
        const string elementNameRegexString = @"<\s*?/?\s*?([^\s/>]*)";

        static Regex attributesRegex;
        static Regex elementNameRegex;
        static Dictionary<string, HTMLElementKind> matches;

        static TagHTMLToken()
        {
            elementNameRegex = new Regex(elementNameRegexString, RegexOptions.Compiled);
            attributesRegex = new Regex(attributesRegexString, RegexOptions.Compiled);

            matches = new Dictionary<string, HTMLElementKind>(StringComparer.OrdinalIgnoreCase);
            matches["br"] = HTMLElementKind.LineBreak;
            matches["span"] = HTMLElementKind.Span;
            matches["b"] = HTMLElementKind.Bold;
            matches["strong"] = HTMLElementKind.Bold;
            matches["i"] = HTMLElementKind.Italic;
            matches["em"] = HTMLElementKind.Italic;
            matches["u"] = HTMLElementKind.Underline;
            matches["a"] = HTMLElementKind.Anchor;
            matches["center"] = HTMLElementKind.Center;
            matches["img"] = HTMLElementKind.Image;
            matches["em"] = HTMLElementKind.Italic;
            matches["iframe"] = HTMLElementKind.IFrame;
        }

        public TagHTMLToken(bool isOpening, bool isClosing, string elementName)
        {
            this.IsOpening = isOpening;
            this.IsClosing = isClosing;
            this.ElementName = elementName;
            this.Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            HTMLElementKind kind;
            if(matches.TryGetValue(elementName, out kind))
                this.Kind = kind;
            else
                this.Kind = HTMLElementKind.Other;
        }

        public override string ToString()
        {
            string slashA = (IsClosing && !IsOpening) ? @"/" : "";
            string slashB = (IsOpening && IsClosing) ? "/" : "";
            string extra = Kind == HTMLElementKind.Other ? "" : " (" + Kind.ToString() + ")";
            string attrs = String.Join(" ", Attributes.Select(kvp => kvp.Key + "='" + kvp.Value + "'").ToArray());
            if(attrs.Length > 0)
                attrs = " " + attrs;

            return String.Format("<{0}{1}{2}{3}>{4}", slashA, ElementName, attrs, slashB, extra);
        }

        public bool IsOpening { get; private set; }
        public bool IsClosing { get; private set; }
        public HTMLElementKind Kind { get; private set; }
        public string ElementName { get; private set; }
        public Dictionary<string, string> Attributes { get; private set; }

        public static TagHTMLToken FromTag(string rawElement)
        {
            if(!rawElement.StartsWith("<") || !rawElement.EndsWith(">"))
                throw new ArgumentException("Argument must start and end with triangular brackets.");

            // Without brackets.
            string content = rawElement.Substring(1, rawElement.Length - 2).Trim();
            string elementName = elementNameRegex.Match(rawElement).Groups[1].Value;

            bool slashStart = content.StartsWith("/");
            bool slashEnd = content.EndsWith("/");
            bool isOpening = !slashStart;
            bool isClosing = slashStart || slashEnd;

            TagHTMLToken ret = new TagHTMLToken(isOpening, isClosing, elementName);

            Match m = attributesRegex.Match(rawElement);
            while(m.Success)
            {
                string key = m.Groups[1].Value;
                string value = WebUtility.HtmlDecode(m.Groups["a"].Value);
                ret.Attributes[key] = value;
                m = m.NextMatch();
            }

            return ret;
        }
    }
}