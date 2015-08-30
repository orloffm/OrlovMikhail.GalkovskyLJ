using OrlovMikhail.LJ.Grabber;
using System;
using System.IO.Abstractions;
using OrlovMikhail.LJ.BookWriter;

namespace OrlovMikhail.LJ.BookWriter
{
    public interface IBookWriter : IDisposable
    {
        void EntryPageBegin();
        void EntryPageEnd();

        void CommentsBegin();
        void CommentsEnd();

        void ThreadBegin();
        void ThreadEnd();

        void EntryEnd();

        void CommentEnd();

        void EntryHeader(DateTime dateTime, long id, string subject, UserLite user, string posterUserpicRelativeLocation);

        void CommentHeader(DateTime dateTime, long id, string subject, UserLite user, string commentUserpicRelativeLocation);

        void WritePart(PostPartBase ppb);
    }
}