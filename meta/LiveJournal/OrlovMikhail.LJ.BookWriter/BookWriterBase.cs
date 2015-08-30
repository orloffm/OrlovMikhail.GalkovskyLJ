using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class BookWriterBase : IBookWriter
    {
        static readonly ILog log = LogManager.GetLogger(typeof(BookWriterBase));

        protected readonly DirectoryInfoBase Root;
        protected readonly FileInfoBase Target;
        protected ITextPreparator Tp { get; private set; }

        protected BookWriterBase(DirectoryInfoBase root, FileInfoBase target, ITextPreparator tp)
        {
            Root = root;
            Target = target;
            Tp = tp;
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
        public abstract void EntryHeader(DateTime dateTime, long id, string subject, UserLite user, string posterUserpicRelativeLocation);
        public abstract void CommentHeader(DateTime dateTime, long id, string subject, UserLite user, string commentUserpicRelativeLocation);

        public void WritePart(PostPartBase ppb)
        {
            if (ppb is RawTextPostPart)
            {
                // Text.
                RawTextPostPart rtpp = ppb as RawTextPostPart;
                string preparedText = Tp.Prepare(rtpp.Text);
                WritePreparedTextInternal(preparedText);
            }
            else if (ppb is ImagePart)
            {
                // Image
                ImagePart ip = ppb as ImagePart;
                string relativePath = IOTools.MakeRelativePath(Root, ip.Src);
                WriteImageInternal(relativePath);
            }
            else if (ppb is UserLinkPart)
            {
                UserLinkPart ip = ppb as UserLinkPart;
                WriteUsernameInternal(ip.Username);
            }
            else if (ppb is BoldStartPart)
                WriteBoldStartInternal();
            else if (ppb is BoldEndPart)
                WriteBoldEndInternal();
            else if (ppb is ItalicStartPart)
                WriteItalicStartInternal();
            else if (ppb is ItalicEndPart)
                WriteItalicEndInternal();
            else if (ppb is LineBreakPart)
                WriteLineBreakInternal();
            else if (ppb is ParagraphStartPart)
                WriteParagraphStartInternal();
            else
                log.WarnFormat("Post part of type {0} is not supported.", ppb.GetType().Name);
        }

        protected abstract void WriteUsernameInternal(string username);
        protected abstract void WriteParagraphStartInternal();
        protected abstract void WriteLineBreakInternal();
        protected abstract void WriteItalicEndInternal();
        protected abstract void WriteItalicStartInternal();
        protected abstract void WriteBoldEndInternal();
        protected abstract void WriteBoldStartInternal();
        protected abstract void WriteImageInternal(string relativePath);
        protected abstract void WritePreparedTextInternal(string preparedText);
    }
}
