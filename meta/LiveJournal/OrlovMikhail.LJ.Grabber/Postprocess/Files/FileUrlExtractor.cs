using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class FileUrlExtractor : IFileUrlExtractor
    {
        private const string Pattern = @"(?<=\s*(?i)(?:href|src)\s*=\s*[""']?)([^'"">\s]+)(?=[""']?)";
        private Regex _regex;

        public FileUrlExtractor()
        {
            _regex = new Regex(Pattern);
        }

        public string[] GetFileUrls(string html)
        {
            HashSet<string> ret = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            MatchCollection mc = _regex.Matches(html);
            foreach(Match m in mc)
            {
                string url = m.Groups[0].Value;
                bool urlIsToAFile = Tools.IsFileWorthDownloading(url);
                if(urlIsToAFile)
                    ret.Add(url);
            }

            return ret.ToArray();
        }
        
        public string ReplaceFileUrls(string html, Func<string, string> matcher)
        {
            Dictionary<string, string> replacees = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            MatchEvaluator me = _m =>
            {
                string url = _m.Groups[0].Value;
                string replacee;
                if(!replacees.TryGetValue(url, out replacee))
                {
                    replacee = matcher(url);
                    replacees[url] = replacee;
                }

                if(String.IsNullOrWhiteSpace(replacee))
                    return url;
                else
                    return replacee;
            };

            string result = _regex.Replace(html, me);

            return result;
        }
    }
}
