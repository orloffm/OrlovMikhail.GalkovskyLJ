using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace OrlovMikhail.LJ.Grabber
{
    public class EntryBaseHelper : IEntryBaseHelper
    {
        static readonly ILog log = LogManager.GetLogger(typeof(EntryBaseHelper));

        private readonly IFileUrlExtractor _fileExtractor;
        
        public EntryBaseHelper( IFileUrlExtractor fileExtractor)
        {
            _fileExtractor = fileExtractor;
        }

        public bool UpdateWith(EntryBase target, EntryBase source)
        {
            if(source == null || target == null)
                throw new ArgumentNullException();

            if(target.Id != 0 && target.Id != source.Id)
                throw new ArgumentException("Ids should match.");

            bool updated = false;

            updated |= UpdateStringProperty(source.Text, target.Text, s => target.Text = s);
            updated |= UpdateStringProperty(source.Subject, target.Subject, s => target.Subject = s);
            updated |= UpdateStringProperty(source.Url, target.Url, s => target.Url = s);

            if(String.IsNullOrEmpty(target.Poster.Username) && source.Poster != null && !String.IsNullOrEmpty(source.Poster.Username))
            {
                target.Poster = source.Poster;
                updated = true;
            }

            if(!target.PosterUserpicSpecified && source.PosterUserpicSpecified)
            {
                target.PosterUserpic = source.PosterUserpic;
                updated = true;
            }

            if(target.Date == null && source.Date != null)
            {
                target.Date = source.Date;
                updated = true;
            }

            return updated;
        }

        public bool UpdateStringProperty(string sourceValue, string targetValue,
            Action<string> targetSetter)
        {
            // Source value is not specified?
            if(String.IsNullOrWhiteSpace(sourceValue))
                return false;

            // Value exists and is larger?
            if(!String.IsNullOrWhiteSpace(targetValue) && targetValue.Length >= sourceValue.Length)
                return false;

            targetSetter(sourceValue);
            return true;
        }


        #region userpics
        public Tuple<string, Userpic>[] GetUserpics(IEnumerable<EntryBase> source)
        {
            IEnumerable<Tuple<string, Userpic>> all = source.Select(CreateUserpicTuple);
            Tuple<string, Userpic>[] result = all.Where(z => z != null).Distinct().ToArray();
            return result;
        }

        protected internal virtual Tuple<string, Userpic> CreateUserpicTuple(EntryBase e)
        {
            if(e.PosterUserpic != null && !String.IsNullOrWhiteSpace(e.PosterUserpic.Url))
                return Tuple.Create(e.Poster.Username, e.PosterUserpic);
            else
                return null;
        }
        #endregion

        #region files
        public Uri[] GetFiles(IEnumerable<EntryBase> source)
        {
            IEnumerable<string> all = source.Select(EnumerateFiles).SelectMany(a => a);
            Uri[] result = all.Distinct().Select(z => new Uri(z)).ToArray();
            return result;
        }

        protected internal virtual IEnumerable<string> EnumerateFiles(EntryBase e)
        {
            if (e.Text == null)
                yield break;

            string[] urls;

            urls = _fileExtractor.GetImagesURLs(e.Text);
            foreach(string url in urls)
                yield return url;
        }
        #endregion
    }
}
