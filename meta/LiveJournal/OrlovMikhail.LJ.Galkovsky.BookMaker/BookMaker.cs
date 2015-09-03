using log4net;
using OrlovMikhail.LJ.Grabber;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using OrlovMikhail.LJ.BookWriter;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    public class BookMaker : IBookMaker
    {
        ILayerParser _lp;
        IFileSystem _fs;
        IBookWriterFactory _f;
        ISuitableCommentsPicker _scp;
        IRelatedDataSaver _rds;
        IFileStorageFactory _fsf;
        IFileUrlExtractor _ext;
        IUserpicStorageFactory _usf;
        IPostPartsMaker _ppm;
        IHTMLParser _htmlParser;

        static readonly ILog log = LogManager.GetLogger(typeof(BookMaker));

        public BookMaker(ILayerParser lp, IFileSystem fs, IBookWriterFactory f,
            IFileStorageFactory fsf, IFileUrlExtractor ext, IUserpicStorageFactory usf,
            IPostPartsMaker ppm,
            ISuitableCommentsPicker scp, IRelatedDataSaver rds, IHTMLParser htmlParser)
        {
            this._ext = ext;
            this._lp = lp;
            this._fsf = fsf;
            this._fs = fs;
            this._f = f;
            this._scp = scp;
            this._usf = usf;
            _ppm = ppm;
            this._rds = rds;
            this._htmlParser = htmlParser;
        }

        public void Make(DirectoryInfoBase bookRootLocation, FileInfoBase sourceFile, FileInfoBase targetFile)
        {
            EntryPage ep = _lp.ParseAsAnEntryPage(_fs.File.ReadAllText(sourceFile.FullName));
            string html = ep.Entry.Text;

            Entry e = ep.Entry;
            List<Comment[]> comments = _scp.Pick(ep);

            EntryBase[] all = comments.SelectMany(a => a).Concat(new EntryBase[] { e }).ToArray();


            // Target book.
            using(IBookWriter w = _f.Create(bookRootLocation, targetFile))
            using(IFileStorage fs = _fsf.CreateOn(sourceFile.Directory.FullName))
            using(IUserpicStorage us = _usf.CreateOn(bookRootLocation.FullName))
            {
                // Parallelize conversion to text parts.
                Dictionary<long, PostPartBase[]> converted = all
                    .AsParallel()
                    .Select(z => new
                {
                    Id = (z is Comment) ? z.Id : 0,
                    C = HTMLToParts(z.Text, fs, w)
                })
                .ToDictionary(p => p.Id, p => p.C);
     
                w.EntryPageBegin();

                string userpicRelative = GetUserpicRelativeLocation(e, us, bookRootLocation);
                w.EntryHeader(e.Date.Value, e.Id, e.Subject, e.Poster, userpicRelative);
                WriteText(converted[0], w);
                w.EntryEnd();

                if(comments.Count > 0)
                {
                    w.CommentsBegin();

                    foreach(Comment[] thread in comments)
                    {
                        w.ThreadBegin();

                        foreach(Comment c in thread)
                        {
                            if(string.IsNullOrWhiteSpace(c.Text) && string.IsNullOrWhiteSpace(c.Subject))
                                continue;

                            userpicRelative = GetUserpicRelativeLocation(c, us, bookRootLocation);
                            w.CommentHeader(c.Date.Value, c.Id, c.Subject, c.Poster, userpicRelative);
                            WriteText(converted[c.Id], w);
                            w.CommentEnd();
                        }

                        w.ThreadEnd();
                    }

                    w.CommentsEnd();
                }

                w.EntryPageEnd();
            }
        }

        private string GetUserpicRelativeLocation(EntryBase e, IUserpicStorage us, DirectoryInfoBase bookRootLocation)
        {
            FileInfoBase posterUserpicLocation = us.TryGet(e.PosterUserpic.Url);
            string relativeUserpicLocation = posterUserpicLocation == null ? null : IOTools.MakeRelativePath(bookRootLocation, posterUserpicLocation);
            return relativeUserpicLocation;
        }

        protected internal virtual void WriteText(PostPartBase[] parts, IBookWriter w)
        {
            for(int i = 0; i < parts.Length; i++)
            {
                PostPartBase ppb = parts[i];
                w.WritePart(ppb);
            }
        }

        PostPartBase[] HTMLToParts(string html, IFileStorage fs, IBookWriter w)
        {
            if(string.IsNullOrWhiteSpace(html))
                return new PostPartBase[0];

            // Explicit tokens as they are in the file.
            HTMLTokenBase[] tokens = _htmlParser.Parse(html).ToArray();

            PostPartBase[] parts = _ppm.CreateTextParts(tokens, fs).ToArray();
            return parts;
        }
    }
}
