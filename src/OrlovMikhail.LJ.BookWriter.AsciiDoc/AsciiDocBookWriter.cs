using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.BookWriter.AsciiDoc
{
    public class AsciiDocBookWriter : BookWriterBase, IBookWriter
    {
        StreamWriter _sr;
        int currentQuotationLevel;
        bool wroteAfterItemBegin;

        const string userInfoIconRelativePath = "resources\\userinfo.png";
        const string communityIconRelativePath = "resources\\community.png";

        #region ctor and write

        public AsciiDocBookWriter(DirectoryInfoBase root, FileInfoBase path)
            : base(root, path, new AsciiDocTextPreparator())
        {
            _sr = new StreamWriter(path.FullName, append: false, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            currentQuotationLevel = 0;
            wroteAfterItemBegin = false;
        }

        public override void Dispose()
        {
            if (_sr != null)
            {
                _sr.Flush();
                _sr.Dispose();
                _sr = null;
            }
        }

        void P(string s) { _sr.Write(s); wroteAfterItemBegin |= !String.IsNullOrEmpty(s); }
        void PL(string s) { _sr.WriteLine(s); wroteAfterItemBegin = true; }
        #endregion

        public override void ThreadBegin()
        {
            PL("");
            PL("''''");
        }

        public override void EntryEnd() { PL(""); }
        public override void CommentEnd() { PL(""); }

        protected override void EntryHeaderInternal(string subject, string url, DateTime date, string posterUserpicRelativeLocation)
        {
            currentQuotationLevel = 0;

            PL(String.Format("== {0}", subject));
            PL("");
            PL("++" + url + "++");
            PL("");

            if (posterUserpicRelativeLocation != null)
                PL(String.Format("image:{0}[userpic, 40, 40]", posterUserpicRelativeLocation));
            PL(String.Format("{0:dd-MM-yyy HH:mm} +", date));
            PL("");

            wroteAfterItemBegin = false;
        }

        protected override void CommentHeaderInternal(string subject, DateTime date, string usernamePrepared, bool isDeleted, bool isScreened, bool isSuspended, string commentUserpicRelativeLocation)
        {
            currentQuotationLevel = 0;

            PL("");
            if (!String.IsNullOrEmpty(commentUserpicRelativeLocation))
                PL(String.Format("image:{0}[\"userpic\", 40, 40]", commentUserpicRelativeLocation));

            WriteUsernameInternal(usernamePrepared);
            PL(String.Format(" {0:dd-MM-yyy HH:mm}", date));

            if (!String.IsNullOrWhiteSpace(subject))
                PL(String.Format("{0}", subject));

            PL("");
            wroteAfterItemBegin = false;

            if (isDeleted)
                P("(комментарий удалён)");
            else if (isScreened)
                P("(комментарий скрыт)");
            else if (isSuspended)
                P("(пользователь заблокирован)");
        }

        /// <summary>Guaranteed to be called with an existing image.</summary>
        protected override void WriteImageInternal(FileInfoBase toPath)
        {
            double hToW = ImageHelper.GetHeightToWidthRatio(toPath);
            const double ahToWRatio = 1.414285714285714d;

            double heightSpanned = hToW / ahToWRatio;
            const double maxHeight = 0.6d;

            string relativePath = IOTools.MakeRelativePath(Root, toPath);
            string scaleString = "";
            if (heightSpanned > maxHeight)
            {
                // Scale it to 60%.
                int scaleRatioPercents = (int)Math.Round(maxHeight / heightSpanned * 100);
                scaleString = string.Format("scaledwidth=\"{0}%\"", scaleRatioPercents);
            }

            P(string.Format("image::{0}[{1}]", relativePath, scaleString));
        }

        protected override void WriteVideoInternal(string url)
        {
            P("(Видео: " + url + ")");
        }

        protected override void WritePreparedTextInternal(string preparedText)
        {
            string[] lines = SplitToLines(preparedText);
            for (int i = 0; i < lines.Length; i++)
            {
                P(lines[i]);
                if (i < lines.Length - 1)
                {
                    // Ends line and starts the next one.
                    StartNewLine(currentQuotationLevel);
                }
            }
        }

        protected override void WriteEmptyPostPart()
        {
            P("{empty}");
        }

        protected override void WriteUsernameInternal(string username, bool isCommunity = false)
        {
            if (!isCommunity)
                P(String.Format("image:{0}[userinfo, 17, 17]", userInfoIconRelativePath));
            else
                P(String.Format("image:{0}[community, 17, 17]", communityIconRelativePath));
            P("{empty}");
            WriteBoldStartInternal();
            P(username);
            WriteBoldEndInternal();
        }

        protected override void WriteParagraphStartInternal(int quotationLevel)
        {
            // End the current line.
            // Put chevrons on the separating line.
            int chevronsInSeparatingLine = Math.Min(currentQuotationLevel, quotationLevel);
            StartNewLine(chevronsInSeparatingLine);

            // End the separating line.
            // Start new line.
            StartNewLine(quotationLevel);

            this.currentQuotationLevel = quotationLevel;
        }

        private void StartNewLine(int chevrons)
        {
            if (wroteAfterItemBegin)
            {
                // End previous line.
                PL("");
            }

            for (int i = 0; i < chevrons; i++)
                P("> ");
        }

        protected override void WriteLineBreakInternal() { PL(" +"); }
        protected override void WriteBoldStartInternal() { P("**"); }
        protected override void WriteBoldEndInternal() { P("**"); }
        protected override void WriteItalicStartInternal() { P("__"); }
        protected override void WriteItalicEndInternal() { P("__"); }

        public static string[] SplitToLines(string text, int lineWidth = 60)
        {
            List<string> ret = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (sb.Length >= lineWidth)
                {
                    char previous = sb[sb.Length - 1];
                    // We can't skip after 
                    bool canSkipHere = previous != ';';

                    if (canSkipHere && Char.IsWhiteSpace(c))
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                        continue;
                    }
                }
                sb.Append(c);
            }

            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret.ToArray();
        }
    }
}
