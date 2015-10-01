using log4net;
using OrlovMikhail.LJ.Grabber;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using OrlovMikhail.LJ.BookWriter;
using OrlovMikhail.Tools;
using System.Threading.Tasks;
using System;
using System.Threading;

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
            this._ppm = ppm;
            this._rds = rds;
            this._htmlParser = htmlParser;
        }

        public async Task Make(DirectoryInfoBase bookRootLocation, FileInfoBase[] dumps, bool overwrite)
        {
            // And now for each post...
#if DEBUG
            int CONCURRENCY_LEVEL = 1;
#else
            int CONCURRENCY_LEVEL = 10;
#endif
            int nextIndex = 0;
            var postTasks = new List<Task>();

            Action runTask = () =>
            {
                FileInfoBase dump = dumps[nextIndex];
                log.DebugFormat("Running {0}...", dump.Directory.Name);

                Task postProcessor = Task.Run(() => ProcessDump(dump, bookRootLocation, overwrite));
                postTasks.Add(postProcessor);
                nextIndex++;
            };

            log.Debug("Inititial population.");
            while(nextIndex < CONCURRENCY_LEVEL && nextIndex < dumps.Length)
            {
                // Adding tasks to process posts.
                runTask();
            }

            while(postTasks.Count > 0)
            {
                try
                {
                    Task postTask = await Task.WhenAny(postTasks);
                    postTasks.Remove(postTask);

                    int count = postTasks.Count;
                    int running = postTasks.Count(z => !(z.IsCompleted || z.IsCanceled || z.IsFaulted));

                    log.DebugFormat("Tasks in array: {0}, running: {1}.", count, running);

                    await postTask;
                }
                catch(Exception exc)
                {
                    log.Error(exc.ToString());
                }

                if(nextIndex < dumps.Length)
                {
                    // Empty slots, adding new tasks.
                    runTask();
                }
            }

        }

        void ProcessDump(FileInfoBase source, DirectoryInfoBase bookRootLocation, bool overWrite)
        {
            log.DebugFormat("{0} is on thread {1}.", source.Directory.Name, Thread.CurrentThread.ManagedThreadId);

            FileInfoBase target = _fs.FileInfo.FromFileName(_fs.Path.Combine(source.Directory.FullName, FragmentHelper.FRAGMENT_FILE_NAME));
            if(target.Exists && target.Length != 0 && !overWrite)
            {
                log.DebugFormat("Target already exists for {0}.", source.Directory.FullName);
                return;
            }

            log.Info(source.FullName);

            EntryPage ep = _lp.ParseAsAnEntryPage(_fs.File.ReadAllText(source.FullName));
            string html = ep.Entry.Text;

            Entry e = ep.Entry;
            List<Comment[]> comments = _scp.Pick(ep);

            EntryBase[] all = (new EntryBase[] { e }).Concat(comments.SelectMany(a => a)).ToArray();

            // Target book.
            using(IBookWriter w = _f.Create(bookRootLocation, target))
            using(IFileStorage fs = _fsf.CreateOn(source.Directory.FullName))
            using(IUserpicStorage us = _usf.CreateOn(bookRootLocation.FullName))
            {
                // Parallelize conversion to text parts.
                Dictionary<long, IPostPart[]> converted = all
                    .Select(z => new
                    {
                        Id = (z is Comment) ? z.Id : 0,
                        C = HTMLToParts(z.Text, fs, w)
                    })
                .ToDictionary(p => p.Id, p => p.C);

                w.EntryPageBegin();

                string userpicRelative = GetUserpicRelativeLocation(e, us, bookRootLocation);
                w.EntryHeader(e, userpicRelative);
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
                            userpicRelative = GetUserpicRelativeLocation(c, us, bookRootLocation);
                            w.CommentHeader(c, userpicRelative);
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

        protected internal virtual void WriteText(IPostPart[] parts, IBookWriter w)
        {
            for(int i = 0; i < parts.Length; i++)
            {
                IPostPart ppb = parts[i];
                w.WritePart(ppb);
            }
        }

        IPostPart[] HTMLToParts(string html, IFileStorage fs, IBookWriter w)
        {
            if(string.IsNullOrWhiteSpace(html))
                return new PostPartBase[0];

            // Explicit tokens as they are in the file.
            HTMLTokenBase[] tokens = _htmlParser.Parse(html).ToArray();

            IPostPart[] parts = _ppm.CreateTextParts(tokens, fs).ToArray();
            return parts;
        }
    }
}
