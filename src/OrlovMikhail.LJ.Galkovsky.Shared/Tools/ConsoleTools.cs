using System;
using System.Collections.Generic;

namespace OrlovMikhail.LJ.Galkovsky.Tools
{
    public static class ConsoleTools
    {
        public static Dictionary<string, string> ArgumentsToDictionary(string[] args)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (!arg.StartsWith("-"))
                    continue;

                string key = arg.Substring(1);
                string value = null;

                // Check if next arg is a value (not starting with -)
                if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    value = args[i + 1];
                    i++;
                }

                ret[key] = value;
            }

            return ret;
        }

        public static string GetExistingOrDefault(this Dictionary<string, string> dict, string key)
        {
            if (dict.TryGetValue(key, out string value))
                return value;
            return null;
        }
    }
}
