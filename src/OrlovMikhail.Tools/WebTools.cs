using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.Tools
{
    public static class WebTools
    {
        public static Dictionary<string, string> ParseKeyValuePairsFromURL(string url)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            const string regexPattern = "(?<key>[^=&]+)(?:=(?<value>[^=&]+))?&?";

            Match m = Regex.Match(url, regexPattern);
            while(m.Success)
            {
                string key = m.Groups["key"].Value;
                string value = m.Groups["value"].Value;

                ret[key] = value;
                m = m.NextMatch();
            }

            return ret;
        }
    }
}
