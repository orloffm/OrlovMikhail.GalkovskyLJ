using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.Tools
{
    public static class SettingsTools
    {
        static readonly ILog log = LogManager.GetLogger(typeof(SettingsTools));

        public static bool LoadValue<T>(string key, Dictionary<string, string> argsDic, T settings, Expression<Func<T, string>> propertyExpression)
        {
            string message;
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;

            string value;
            argsDic.TryGetValue(key, out value);
            if (String.IsNullOrEmpty(value))
            {
                value = propertyInfo.GetValue(settings) as string;
            }
            else
            {
                propertyInfo.SetValue(settings, value);
            }

            if (String.IsNullOrEmpty(value))
            {
                log.ErrorFormat("No {0} known. Please specify it as /{0}=\"abc\" in the command line.", key);
                return false;
            }
            else
            {
                log.DebugFormat("Using {0}={1}.", key, value);
                return true;
            }
        }
    }
}
