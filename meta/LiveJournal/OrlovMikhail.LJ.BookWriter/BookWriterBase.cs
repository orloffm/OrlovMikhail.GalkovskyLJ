using log4net;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;
using System;
using System.IO.Abstractions;
using System.Net;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class BookWriterBase : IBookWriter
    {
        static readonly ILog log = LogManager.GetLogger(typeof(BookWriterBase));

        protected readonly DirectoryInfoBase Root;
        protected readonly FileInfoBase Target;
        ITextPreparator _tp;

        protected BookWriterBase(DirectoryInfoBase root, FileInfoBase target, ITextPreparator tp)
        {
            Root = root;
            Target = target;
            _tp = tp;
        }

        public virtual void Dispose() { }
        public virtual void EntryPageBegin() { }
        public virtual void EntryPageEnd() { }
        public virtual void CommentsBegin() { }
        public virtual void CommentsEnd() { }
        public virtual void ThreadBegin() { }
        public virtual void ThreadEnd() { }
        public virtual void EntryEnd() { }
        public virtual void CommentEnd() { }
        public void EntryHeader(Entry e, string posterUserpicRelativeLocation)
        {
            string subject = e.Subject;
            if(subject.Length == 0)
                subject = e.Id.ToString();

            subject = HTMLParser.StripOfTags(subject.Trim());
            if(subject.EndsWith(".") && !subject.EndsWith("..."))
                subject = subject.Substring(0, subject.Length - 1);
            subject = WebUtility.HtmlDecode(subject);
            subject = _tp.Prepare(subject);

            EntryHeaderInternal(subject, e.Url, e.Date.Value, posterUserpicRelativeLocation);
        }

        public void CommentHeader(Comment c, string commentUserpicRelativeLocation)
        {
            string subject = c.Subject;
            if(!String.IsNullOrWhiteSpace(subject))
            {
                subject = HTMLParser.StripOfTags(subject);
                subject = WebUtility.HtmlDecode(subject);
                subject = _tp.Prepare(subject);
            }

            CommentHeaderInternal(subject, c.Date.Value, c.Poster.Username, c.IsDeleted, c.IsScreened, c.IsSuspendedUser, commentUserpicRelativeLocation);
        }

        public void WritePart(PostPartBase ppb)
        {
            if (ppb is RawTextPostPart)
            {
                // Text.
                RawTextPostPart rtpp = ppb as RawTextPostPart;
                string preparedText = _tp.Prepare(rtpp.Text);
                WritePreparedTextInternal(preparedText);
            }
            else if (ppb is ImagePart)
            {
                // Image
                ImagePart ip = (ImagePart)ppb;
                string relativePath = IOTools.MakeRelativePath(Root, ip.Src);
                WriteImageInternal(relativePath);
            }
            else if(ppb is VideoPart)
            {
                VideoPart vp = (VideoPart)ppb;
                WriteVideoInternal(vp.URL);
            }
            else if(ppb is UserLinkPart)
            {
                UserLinkPart ip = (UserLinkPart)ppb;
                WriteUsernameInternal(ip.Username, ip.IsCommunity);
            }
            else if(ppb is BoldStartPart)
                WriteBoldStartInternal();
            else if(ppb is BoldEndPart)
                WriteBoldEndInternal();
            else if(ppb is ItalicStartPart)
                WriteItalicStartInternal();
            else if(ppb is ItalicEndPart)
                WriteItalicEndInternal();
            else if(ppb is LineBreakPart)
                WriteLineBreakInternal();
            else if(ppb is ParagraphStartPart)
                WriteParagraphStartInternal((ppb as ParagraphStartPart).QuotationLevel);
            else
                log.WarnFormat("Post part of type {0} is not supported.", ppb.GetType().Name);
        }


        protected abstract void EntryHeaderInternal(string subject, string url, DateTime date, string posterUserpicRelativeLocation);
        protected abstract void CommentHeaderInternal(string subject, DateTime date, string username, bool isDeleted, bool isScreened, bool isSuspended, string commentUserpicRelativeLocation);
        
        protected abstract void WriteUsernameInternal(string username, bool isCommunity = false);
        protected abstract void WriteParagraphStartInternal(int quotationLevel);
        protected abstract void WriteLineBreakInternal();
        protected abstract void WriteItalicEndInternal();
        protected abstract void WriteItalicStartInternal();
        protected abstract void WriteBoldEndInternal();
        protected abstract void WriteBoldStartInternal();
        protected abstract void WriteImageInternal(string relativePath);
        protected abstract void WriteVideoInternal(string url);
        protected abstract void WritePreparedTextInternal(string preparedText);
    }
}
