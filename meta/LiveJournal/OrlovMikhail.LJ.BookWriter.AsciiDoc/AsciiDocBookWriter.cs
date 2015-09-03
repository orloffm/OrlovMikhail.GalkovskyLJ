using OrlovMikhail.LJ.Grabber;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrlovMikhail.LJ.BookWriter;
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
            _sr.AutoFlush = true;

            currentQuotationLevel = 0;
            wroteAfterItemBegin = false;
        }

        public override void Dispose()
        {
            if(_sr != null)
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

        public override void EntryHeader(DateTime dateTime, long id, string subject, UserLite user, string posterUserpicRelativeLocation)
        {
            currentQuotationLevel = 0;

            if(subject.Length == 0)
                subject = id.ToString();

            subject = subject.Trim();
            if(subject.EndsWith("."))
                subject = subject.Substring(0, subject.Length - 1);
            subject = Tp.Prepare(subject);

            PL(String.Format("== {0}", subject));
            PL("");

            if(posterUserpicRelativeLocation != null)
                PL(String.Format("image:{0}[userpic, 40, 40]", posterUserpicRelativeLocation));
            PL(String.Format("{0:dd-MM-yyy HH:mm}", dateTime));
            PL("");

            wroteAfterItemBegin = false;
        }

        public override void CommentHeader(DateTime dateTime, long id, string subject, UserLite user, string commentUserpicRelativeLocation)
        {
            currentQuotationLevel = 0;

            PL("");
            if(commentUserpicRelativeLocation != null)
                PL(String.Format("image:{0}[userpic, 40, 40]", commentUserpicRelativeLocation));

            WriteUsernameInternal(user.Username, user.UserType == UserLiteType.C);
            PL(String.Format(" {0:dd-MM-yyy HH:mm}", dateTime));
            if(!String.IsNullOrWhiteSpace(subject))
            {
                subject = Tp.Prepare(subject);
                PL(String.Format("{0}", subject));
            }

            PL("");

            wroteAfterItemBegin = false;
        }

        protected override void WriteImageInternal(string relativePath)
        {
            P("image::" + relativePath + "[]");
        }

        protected override void WritePreparedTextInternal(string preparedText)
        {
            string[] lines = SplitToLines(preparedText);
            for(int i = 0; i < lines.Length; i++)
            {
                P(lines[i]);
                if(i < lines.Length - 1)
                {
                    // Ends line and starts the next one.
                    StartNewLine(currentQuotationLevel);
                }
            }
        }

        protected override void WriteUsernameInternal(string username, bool isCommunity = false)
        {
            if(!isCommunity)
                P(String.Format("image:{0}[userinfo, 17, 17]", userInfoIconRelativePath));
            else
                P(String.Format("image:{0}[community, 17, 17]", communityIconRelativePath));
            P("{nbsp}");
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
            if(wroteAfterItemBegin)
            {
                // End previous line.
                PL("");
            }

            for(int i = 0; i < chevrons; i++)
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
            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if(sb.Length >= lineWidth)
                {
                    char previous = sb[sb.Length - 1];
                    // We can't skip after 
                    bool canSkipHere = previous != ';';

                    if(canSkipHere && Char.IsWhiteSpace(c))
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                        continue;
                    }
                }
                sb.Append(c);
            }

            if(sb.Length > 0)
                ret.Add(sb.ToString());

            return ret.ToArray();
        }
    }
}
