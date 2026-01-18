using System;
using System.Collections.Generic;
using System.Web;

namespace OrlovMikhail.LJ.BookWriter.Tools
{
    public static class WebTools
    {
        public static Dictionary<string, string> ParseKeyValuePairsFromURL(string url)
        {
            var ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var uri = new Uri(url);
                var query = HttpUtility.ParseQueryString(uri.Query);
                foreach (string key in query.AllKeys)
                {
                    if (key != null)
                        ret[key] = query[key];
                }
            }
            catch
            {
            }

            return ret;
        }
    }

    public static class DictionaryExtensions
    {
        public static string GetExistingOrDefault(this Dictionary<string, string> dict, string key)
        {
            if (dict.TryGetValue(key, out string value))
                return value;
            return null;
        }
    }
}
