using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Basically represents a LiveJournal entry/comments
    /// URL. Is a separate class for ease of usage.</summary>
    public sealed class LiveJournalTarget
    {
        private const string extractionRegexString = @"^(?:https?://)?(?<username>[^\.]+)\.livejournal\.com/(?<postId>[0-9]*)\.html\??(?<parameters>[^#/]*)?(?:#.*)?$";

        private LiveJournalTarget(bool useStyleMine = false)
        {
            this.UseStyleMine = useStyleMine;
        }

        public LiveJournalTarget(string userName, long postId, long? commentId = null, int? page = null, bool useStyleMine = true)
            : this(useStyleMine)
        {
            this.Username = userName;
            this.PostId = postId;
            this.CommentId = commentId;
            this.Page = page;
        }

        public static LiveJournalTarget FromString(string rawString)
        {
            Regex r = new Regex(extractionRegexString, RegexOptions.Compiled);
            Match m = r.Match(rawString);

            if(!m.Success)
                throw new ArgumentException();

            LiveJournalTarget ret = new LiveJournalTarget(useStyleMine: false);
            ret.Username = m.Groups["username"].Value;
            ret.PostId = long.Parse(m.Groups["postId"].Value);

            string arguments = m.Groups["parameters"].Value;
            string[] kvps = arguments.Split('&', '=');

            for(int i = 0; i < kvps.Length - 1; i += 2)
            {
                string key = kvps[i];
                string value = kvps[i + 1];

                if(String.Equals(key, "style", StringComparison.OrdinalIgnoreCase))
                {
                    if(String.Equals(value, "mine", StringComparison.OrdinalIgnoreCase))
                        ret.UseStyleMine = true;
                }
                else if(String.Equals(key, "thread", StringComparison.OrdinalIgnoreCase))
                {
                    // Comment id.
                    ret.CommentId = long.Parse(value);
                }
                else if(String.Equals(key, "page", StringComparison.OrdinalIgnoreCase))
                {
                    // Paget id.
                    ret.Page = int.Parse(value);
                }
            }

            return ret;
        }

        public string Username { get; private set; }
        public long PostId { get; private set; }
        public long? CommentId { get; private set; }
        public int? Page { get; private set; }
        /// <summary>Whether to apply "style mine" to the url.</summary>
        public bool UseStyleMine { get; private set; }

        public LiveJournalTarget WithStyleMine(bool value)
        {
            LiveJournalTarget ret = this.MemberwiseClone() as LiveJournalTarget;
            ret.UseStyleMine = value;
            return ret;
        }

        /// <summary>Creates the URL string.</summary>
        public override string ToString()
        {
            string ret = MakeUrl();
            return ret;
        }

        private string MakeUrl()
        {
            // http://galkovsky.livejournal.com/247911.html?thread=91572583#t91572583  ?page=2
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"http://{0}.livejournal.com/{1}.html", Username, PostId);

            bool addedSomething = false;

            if(CommentId.HasValue)
            {
                sb.Append("?");
                sb.AppendFormat("thread={0}", CommentId.Value);
                addedSomething = true;
            }
            else if(Page.HasValue)
            {
                sb.Append(addedSomething ? "&" : "?");
                sb.AppendFormat("page={0}", Page.Value);
                addedSomething = true;
            }

            if(UseStyleMine)
            {
                sb.Append(addedSomething ? "&" : "?");
                sb.Append("style=mine");
                addedSomething = true;
            }

            string ret = sb.ToString();
            return ret;
        }

        public string ToShortString()
        {
            string s = String.Format("{0}/{1}", Username, PostId);
            if((Page ?? 1) > 1)
                s += "#" + Page.Value;
            return s;
        }

        public Uri GetUri()
        {
            return new Uri(MakeUrl());
        }

        public bool SameItem(LiveJournalTarget b)
        {
            return this.PostId == b.PostId && this.CommentId == b.CommentId && this.Username == b.Username;
        }
    }
}
