using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.Tools
{
    public static class Extensions
    {
        public static string GetExistingOrDefault(this Dictionary<string, string> ret, string key)
        {
            string value;
            ret.TryGetValue(key, out value);
            return value;
        }
    }
}
